using Serilog;

namespace BitcrackRandomiser
{
    /// <summary>
    /// Logger class
    /// </summary>
    internal class Logger
    {
        public static Serilog.Core.Logger? _logger = null;

        static Logger()
        {
            if (_logger == null && Program.isLoggingEnabled)
            {
                var fileName = DateTime.Now.ToString("dd-MM-yyyy");
                _logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File($"logs\\logs_{fileName}.txt")
                .CreateLogger();
                _logger.Information("Logger created/updated");
            }
        }

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void LogError(Exception? ex, string? message)
        {
            _logger?.Error(ex, message);
        }

        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="message"></param>
        public static void LogInformation(string? message)
        {
            _logger?.Information(message);
        }
    }
}
