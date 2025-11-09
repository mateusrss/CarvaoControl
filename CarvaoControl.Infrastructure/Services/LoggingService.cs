using System;
using System.IO;
using System.Text;

namespace CarvaoControl.Infrastructure.Services
{
    public class LoggingService
    {
        private readonly string _logPath;
        private readonly object _lock = new object();

        public LoggingService(string? basePath = null)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _logPath = Path.Combine(basePath ?? Path.Combine(appData, "CarvaoControl"), "logs");
            Directory.CreateDirectory(_logPath);
        }

        public void LogInfo(string message)
        {
            LogMessage("INFO", message);
        }

        public void LogError(string message, Exception? ex = null)
        {
            var sb = new StringBuilder(message);
            if (ex != null)
            {
                sb.AppendLine();
                sb.AppendLine("Exception: " + ex.Message);
                sb.AppendLine("StackTrace: " + ex.StackTrace);
            }
            LogMessage("ERROR", sb.ToString());
        }

        public void LogAudit(string action, string details)
        {
            LogMessage("AUDIT", $"{action} - {details}");
        }

        private void LogMessage(string level, string message)
        {
            try
            {
                var logFile = Path.Combine(_logPath, $"{DateTime.Now:yyyy-MM-dd}.log");
                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}{Environment.NewLine}";

                lock (_lock)
                {
                    File.AppendAllText(logFile, logEntry);
                }
            }
            catch
            {
                // Falhas de logging não devem impactar a aplicação
            }
        }
    }
}