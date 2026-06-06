using System;

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

        /// <summary>
        /// Exists for API completeness. No buffering is used, so nothing to flush.
        /// </summary>
        public static void Flush()
        {
            // No-op: File.AppendAllText writes immediately.
        }

        /// <summary>
        /// Included for graceful shutdown symmetry.
        /// </summary>
        public static void Dispose()
        {
            // No resources to dispose yet, but method exists for future expansion.
        }
    }
}
