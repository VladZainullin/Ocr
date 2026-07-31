using System;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Vlad.Tesseract;

internal sealed class TesseractEnginePooledObjectPolicy(TesseractNativeLogBridge _) : IPooledObjectPolicy<TesseractEngine>
{
    private readonly TesseractNativeLogBridge _unknown = _;

    public TesseractEngine Create()
    {
        var engine = new TesseractEngine();

        // engine.SetVariable("debug_file", "/dev/stderr");
        
        if (!engine.TryInitialization("/Users/vadislavzainullin/MEGAsync/tesseract", "rus+eng",
                TessOcrEngineMode.OemLstmOnly)) 
            throw new InvalidOperationException("Cannot create Tesseract engine");

        engine.SetSegmentationMode(PageSegmentationMode.Auto);
            
        return engine;

    }

    public bool Return(TesseractEngine engine)
    {
        engine.Clear();
        return true;
    }
}

internal sealed partial class TesseractNativeLogBridge : IAsyncDisposable
{
    private const int StandardErrorFileDescriptor = 2;

    private readonly AnonymousPipeServerStream _readPipe;
    private readonly int _originalStandardError;
    private readonly Task _readerTask;

    private int _disposed;

    public TesseractNativeLogBridge(ILoggerFactory loggerFactory)
    {
        if (!OperatingSystem.IsMacOS() &&
            !OperatingSystem.IsLinux())
        {
            throw new PlatformNotSupportedException(
                "Native stderr interception is implemented only for macOS and Linux.");
        }

        var logger = loggerFactory.CreateLogger("Vlad.Tesseract.Native");
        
        _readPipe = new AnonymousPipeServerStream(
            PipeDirection.In,
            HandleInheritability.None);

        var pipeWriteDescriptor = checked(
            (int)_readPipe.ClientSafePipeHandle
                .DangerousGetHandle());
        
        _originalStandardError =
            NativeMethods.Dup(StandardErrorFileDescriptor);

        if (_originalStandardError < 0)
        {
            _readPipe.Dispose();
            throw CreateNativeException("dup(stderr) failed");
        }
        
        if (NativeMethods.Dup2(
                pipeWriteDescriptor,
                StandardErrorFileDescriptor) < 0)
        {
            NativeMethods.Close(_originalStandardError);
            _readPipe.Dispose();

            throw CreateNativeException("dup2(pipe, stderr) failed");
        }
        
        _readPipe.DisposeLocalCopyOfClientHandle();

        _readerTask = ReadNativeLogsAsync(logger);
    }

    private async Task ReadNativeLogsAsync(ILogger logger)
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
            while (await reader.ReadLineAsync().ConfigureAwait(false)
                   is { } line)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                LogTesseractMessage(logger, line);
            }
        }
        catch (IOException) when (Volatile.Read(ref _disposed) != 0)
        {
        }
        catch (ObjectDisposedException)
            when (Volatile.Read(ref _disposed) != 0)
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Exception? restoreException = null;
        
        if (NativeMethods.Dup2(
                _originalStandardError,
                StandardErrorFileDescriptor) < 0)
        {
            restoreException =
                CreateNativeException("Cannot restore stderr");
        }

        NativeMethods.Close(_originalStandardError);

        if (restoreException is null)
        {
            await _readerTask.ConfigureAwait(false);
        }

        await _readPipe.DisposeAsync().ConfigureAwait(false);

        if (restoreException is not null)
        {
            throw restoreException;
        }
    }

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Debug,
        Message = "Tesseract native: {NativeMessage}")]
    private static partial void LogTesseractMessage(
        ILogger logger,
        string nativeMessage);

    private static Win32Exception CreateNativeException(
        string message)
    {
        var error = Marshal.GetLastPInvokeError();

        return new Win32Exception(
            error,
            $"{message}. Native error: {error}");
    }

    private static partial class NativeMethods
    {
        public static int Dup(int descriptor)
        {
            return OperatingSystem.IsMacOS()
                ? MacOS.Dup(descriptor)
                : Linux.Dup(descriptor);
        }

        public static int Dup2(
            int sourceDescriptor,
            int targetDescriptor)
        {
            return OperatingSystem.IsMacOS()
                ? MacOS.Dup2(sourceDescriptor, targetDescriptor)
                : Linux.Dup2(sourceDescriptor, targetDescriptor);
        }

        public static int Close(int descriptor)
        {
            return OperatingSystem.IsMacOS()
                ? MacOS.Close(descriptor)
                : Linux.Close(descriptor);
        }

        private static partial class MacOS
        {
            private const string LibSystem = "libSystem.B.dylib";

            [LibraryImport(
                LibSystem,
                EntryPoint = "dup",
                SetLastError = true)]
            [UnmanagedCallConv(
                CallConvs = [typeof(CallConvCdecl)])]
            internal static partial int Dup(int descriptor);

            [LibraryImport(
                LibSystem,
                EntryPoint = "dup2",
                SetLastError = true)]
            [UnmanagedCallConv(
                CallConvs = [typeof(CallConvCdecl)])]
            internal static partial int Dup2(
                int sourceDescriptor,
                int targetDescriptor);

            [LibraryImport(
                LibSystem,
                EntryPoint = "close",
                SetLastError = true)]
            [UnmanagedCallConv(
                CallConvs = [typeof(CallConvCdecl)])]
            internal static partial int Close(int descriptor);
        }

        private static partial class Linux
        {
            // Подходит для Debian/Ubuntu-образов .NET.
            private const string LibC = "libc.so.6";

            [LibraryImport(
                LibC,
                EntryPoint = "dup",
                SetLastError = true)]
            [UnmanagedCallConv(
                CallConvs = [typeof(CallConvCdecl)])]
            internal static partial int Dup(int descriptor);

            [LibraryImport(
                LibC,
                EntryPoint = "dup2",
                SetLastError = true)]
            [UnmanagedCallConv(
                CallConvs = [typeof(CallConvCdecl)])]
            internal static partial int Dup2(
                int sourceDescriptor,
                int targetDescriptor);

            [LibraryImport(
                LibC,
                EntryPoint = "close",
                SetLastError = true)]
            [UnmanagedCallConv(
                CallConvs = [typeof(CallConvCdecl)])]
            internal static partial int Close(int descriptor);
        }
    }
}