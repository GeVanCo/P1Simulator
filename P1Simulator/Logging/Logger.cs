using System;
using System.IO;

namespace P1Simulator.Logging
{
    public class Logger
    {
        private readonly object _lock = new object();
        private readonly string _logFile;

        public Logger()
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(folder);

            _logFile = Path.Combine(folder, $"p1sim_{DateTime.Now:yyyyMMdd_HHmmss}.log");
        }

        public void Info(string message)
        {
            Write("INFO", message);
        }

        public void Error(string message)
        {
            Write("ERROR", message);
        }

        public void WriteTelegram(string telegram)
        {
            Write("TELEGRAM", telegram.Replace("\r", "").Replace("\n", "\\n"));
        }

        private void Write(string level, string message)
        {
            lock (_lock)
            {
                File.AppendAllText(_logFile,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}{Environment.NewLine}");
            }
        }

        public void Flush()
        {
            // No buffering, nothing to flush.
        }

        public void Dispose()
        {
            // No resources yet, but method exists for future expansion.
        }
    }
}
