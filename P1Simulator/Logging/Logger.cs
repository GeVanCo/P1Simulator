
namespace P1Simulator.Logging
{
    public static class Logger
    {
        private static readonly object _lock = new object();
        private static readonly string _logFile;

        static Logger()
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(folder);

            _logFile = Path.Combine(folder, $"p1sim_{DateTime.Now:yyyyMMdd_HHmmss}.log");
        }

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Error(string message)
        {
            Write("ERROR", message);
        }

        public static void WriteTelegram(string telegram)
        {
            Write("TELEGRAM", telegram.Replace("\r", "").Replace("\n", "\\n"));
        }

        private static void Write(string level, string message)
        {
            lock (_lock)
            {
                File.AppendAllText(_logFile,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}{Environment.NewLine}");
            }
        }
    }
}

