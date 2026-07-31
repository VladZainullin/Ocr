using System.IO.Pipes;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;

namespace Vlad.Tesseract;

internal abstract partial class TesseractNativeLogBridgeBase
    : ITesseractNativeLogBridge
{
    protected const int StandardErrorFileDescriptor = 2;

    private readonly AnonymousPipeServerStream _readPipe;
    private readonly ILogger _logger;

    private Task? _readerTask;
    private int _started;
    private int _disposed;

    protected TesseractNativeLogBridgeBase(
        ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _logger = loggerFactory.CreateLogger(
            "Vlad.Tesseract.Native");

        _readPipe = new AnonymousPipeServerStream(
            PipeDirection.In,
            HandleInheritability.None);
    }

    protected SafePipeHandle PipeWriteHandle =>
        _readPipe.ClientSafePipeHandle;

    /// <summary>
    /// Вызывается после успешного перенаправления stderr.
    /// Закрывает локальную write-сторону pipe и запускает чтение.
    /// </summary>
    protected void StartReading()
    {
        if (Interlocked.Exchange(ref _started, 1) != 0)
        {
            throw new InvalidOperationException(
                "Native log bridge has already been started.");
        }

        /*
         * После dup2 или _dup2 файловый дескриптор stderr
         * уже владеет собственной ссылкой на write-сторону pipe.
         */
        _readPipe.DisposeLocalCopyOfClientHandle();

        _readerTask = ReadNativeLogsAsync();
    }

    /// <summary>
    /// Используется производным классом, если конструктор завершился ошибкой.
    /// </summary>
    protected void DisposeAfterFailedConstruction()
    {
        _readPipe.Dispose();
    }

    protected abstract void RestoreStandardError();

    private async Task ReadNativeLogsAsync()
    {
        using var reader = new StreamReader(
            _readPipe,
            new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: false,
                throwOnInvalidBytes: false),
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 4096,
            leaveOpen: true);

        try
        {
            while (await reader.ReadLineAsync()
                       .ConfigureAwait(false)
                   is { } line)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                LogNativeMessage(_logger, line);
            }
        }
        catch (IOException)
            when (Volatile.Read(ref _disposed) != 0)
        {
            // Pipe закрывается при остановке приложения.
        }
        catch (ObjectDisposedException)
            when (Volatile.Read(ref _disposed) != 0)
        {
            // Ожидаемое завершение.
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Exception? restoreException = null;

        if (Volatile.Read(ref _started) != 0)
        {
            try
            {
                RestoreStandardError();
            }
            catch (Exception exception)
            {
                restoreException = exception;
            }
        }

        if (restoreException is null)
        {
            /*
             * После восстановления stderr write-сторона pipe
             * должна закрыться, и reader получит EOF.
             */
            if (_readerTask is not null)
            {
                await _readerTask.ConfigureAwait(false);
            }

            await _readPipe.DisposeAsync()
                .ConfigureAwait(false);
        }
        else
        {
            /*
             * Если восстановить stderr не удалось, EOF может
             * не появиться. Закрываем read-сторону принудительно.
             */
            await _readPipe.DisposeAsync()
                .ConfigureAwait(false);

            if (_readerTask is not null)
            {
                try
                {
                    await _readerTask.ConfigureAwait(false);
                }
                catch (IOException)
                {
                }
                catch (ObjectDisposedException)
                {
                }
            }

            throw restoreException;
        }
    }

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Debug,
        Message = "Tesseract native: {NativeMessage}")]
    private static partial void LogNativeMessage(
        ILogger logger,
        string nativeMessage);
}