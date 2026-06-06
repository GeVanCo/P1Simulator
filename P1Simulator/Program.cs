using P1Simulator.Serial;
using P1Simulator.Telegrams;
using P1Simulator.Simulation;
using P1Simulator.Logging;

namespace P1Simulator
{
    internal class Program
    {
        private static readonly CancellationTokenSource _cts = new();

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== P1 Smart Meter Simulator ===");
            Console.WriteLine("Press Ctrl+C to stop.");

            // Register graceful shutdown handlers
            Console.CancelKeyPress += OnCancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            // Auto-detect COM port
            string? portName = ComPortDetectorHybrid.AutoDetect();

            if (portName == null)
            {
                Console.WriteLine("ERROR: No USB‑UART adapter detected.");
                return;
            }

            Console.WriteLine($"Using port: {portName}");
            Logger.Info($"Using COM port: {portName}");

            var sender = new SerialSender(portName, 115200);
            var generator = new TelegramGenerator();
            var profile = new SimulationProfile(SimulationMode.Normal);

            Logger.Info($"Opening serial port {portName}...");
            sender.Open();

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    string telegram = generator.GenerateTelegram(profile);

                    Console.WriteLine("Sending telegram:");
                    Console.WriteLine(telegram);

                    sender.Send(telegram);

                    await Task.Delay(profile.IntervalMs, _cts.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // Expected on shutdown
            }
            finally
            {
                Console.WriteLine("Shutting down...");
                Logger.Info("Shutting down simulator...");

                sender.Dispose();
                Logger.Info("Serial port closed.");

                Logger.Flush();
                Console.WriteLine("Goodbye!");
            }
        }

        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Ctrl+C detected. Stopping...");
            e.Cancel = true; // Prevent immediate termination
            _cts.Cancel();
        }

        private static void OnProcessExit(object? sender, EventArgs e)
        {
            _cts.Cancel();
        }
    }
}
