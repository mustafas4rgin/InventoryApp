using InventoryApp.Domain.Contracts;

namespace InventoryApp.Data
{
    public class FileLoggerService : ILoggerService
    {
        private readonly string _logFilePath;

        public FileLoggerService()
        {
            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "log.txt");

            var directory = Path.GetDirectoryName(_logFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public void LogError(string message)
        {
            var logMessage = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
            File.AppendAllText(_logFilePath, logMessage);
        }
    }
}
