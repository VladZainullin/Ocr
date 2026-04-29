using Microsoft.Extensions.Logging;
using UglyToad.PdfPig.Logging;

namespace Application;

internal sealed partial class PdfLogger(ILogger<PdfLogger> logger) : ILog
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "{Message}")]
    private static partial void LogDebug(ILogger logger, string message);

    [LoggerMessage(Level = LogLevel.Debug, Message = "{Message}")]
    private static partial void LogDebug(ILogger logger, Exception exception, string message);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Message}")]
    private static partial void LogWarning(ILogger logger, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "{Message}")]
    private static partial void LogError(ILogger logger, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "{Message}")]
    private static partial void LogError(ILogger logger, Exception exception, string message);

    public void Debug(string message)
    {
        LogDebug(logger, message);
    }

    public void Debug(string message, Exception ex)
    {
        LogDebug(logger, ex, message);
    }

    public void Warn(string message)
    {
        LogWarning(logger, message);
    }

    public void Error(string message)
    {
        LogError(logger, message);
    }

    public void Error(string message, Exception ex)
    {
        LogError(logger, ex, message);
    }
}