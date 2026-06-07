using P1Simulator.Serial;
using P1Simulator.Telegrams;
using P1Simulator.Simulation;
using P1Simulator.Logging;

namespace P1Simulator
{
    internal class Program
    {
        private static CancellationTokenSource _cts = new();

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== P1 Smart Meter Simulator ===");

            while (true)
            {
                await RunSimulator();

                Console.WriteLine();
                Console.WriteLine("=========================");
                Console.WriteLine("    Simulator stopped    ");
                Console.WriteLine("=========================");
                Console.WriteLine("Press:");
                Console.WriteLine("  r -> Restart application");
                Console.WriteLine("  q -> Quit");
                Console.Write("> ");

                string? choice = Console.ReadLine()?.Trim().ToLower();

                if (choice == "q")
                {
                    Console.WriteLine("Goodbye!");
                    System.Threading.Thread.Sleep(1000);
                    return;
                }

                if (choice == "r")
                {
                    Console.WriteLine("Restarting simulator...");
                    continue;
                }
            }
        }

        private static async Task RunSimulator()
        {
            _cts = new CancellationTokenSource();

            Console.WriteLine("Press 'q' to stop or Ctrl+C to interrupt.");

            Console.CancelKeyPress += OnCancelKeyPress;

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
                    // Check for 'q'
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true);
                        if (key.Key == ConsoleKey.Q)
                        {
                            Console.WriteLine("Stopping simulator...");
                            _cts.Cancel();
                            break;
                        }
                    }

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
            }
        }

        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Ctrl+C detected. Stopping...");
            e.Cancel = true; // Prevent immediate termination
            _cts.Cancel();
        }
    }
}
