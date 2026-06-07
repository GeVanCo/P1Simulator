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
            Console.Title = "P1 Dutch Smart Meter Reader Simulator";

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

                ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                char choice = char.ToLower(key.KeyChar);

                if (choice == 'q')
                {
                    Console.WriteLine("\nGoodbye!");
                    System.Threading.Thread.Sleep(1000);
                    return;
                }

                if (choice == 'r')
                {
                    Console.WriteLine("\nRestarting simulator...");
                    continue;
                }
            }
        }

        static void ShowSplash(string portName, int intervalMs, string mode, int baudRate)
        {
            Console.Clear();

            Console.WriteLine(@"
  _____  __    _____ _                 _       _             
 |  __ \/_ |  / ____(_)               | |     | |            
 | |__) || | | (___  _ _ __ ___  _   _| | __ _| |_ ___  _ __ 
 |  ___/ | |  \___ \| | '_ ` _ \| | | | |/ _` | __/ _ \| '__|
 | |     | |  ____) | | | | | | | |_| | | (_| | || (_) | |   
 |_|     |_| |_____/|_|_| |_| |_|\__,_|_|\__,_|\__\___/|_|   

 By Geert Vancompernolle (2026)
                                                             
");

            Console.WriteLine($" COM Port     : {portName}");
            Console.WriteLine($" Baudrate     : {baudRate} baud");
            Console.WriteLine($" Interval     : {intervalMs} ms");
            Console.WriteLine($" Mode         : {mode}");
            Console.WriteLine($" CRC          : DSMR CRC16 (0xA001)");
            Console.WriteLine();
            Console.WriteLine($" Press 'q' to stop or Ctrl+C to interrupt.");
            Console.WriteLine();

            string loading = "Starting";
            for (int i = 0; i < 3; i++)
            {
                Console.Write($"\r{loading}{new string('.', i + 1)}   ");
                Thread.Sleep(1000);
            }

            Console.WriteLine("\n");
        }

        private static async Task RunSimulator()
        {
            _cts = new CancellationTokenSource();

            Console.CancelKeyPress += OnCancelKeyPress;

            // Auto-detect COM port
            string? portName = ComPortDetectorHybrid.AutoDetect();

            if (portName == null)
            {
                Console.WriteLine("ERROR: No USB‑UART adapter detected.");
                return;
            }

            Logger.Info($"Using COM port: {portName}");

            var sender = new SerialSender(portName, 115200);
            var generator = new TelegramGenerator();
            var profile = new SimulationProfile(SimulationMode.Normal);

            Logger.Info($"Opening serial port {portName}...");
            sender.Open();

            // ⭐ Show splash screen here
            ShowSplash(portName, profile.IntervalMs, profile.Mode.ToString(), sender.BaudRate);

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
