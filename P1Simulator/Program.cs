using P1Simulator.Serial;
using P1Simulator.Telegrams;
using P1Simulator.Simulation;
using P1Simulator.Logging;
using System.Runtime.InteropServices;

namespace P1Simulator
{
    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        //============================================================================

        private static CancellationTokenSource _cts = new();
        private static int _telegramCount = 0;
        static int FooterRow => Console.WindowHeight - 2;

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

        // ───────────────────────────────────────────────────────────────
        //  SPLASH SCREEN (unchanged)
        // ───────────────────────────────────────────────────────────────
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

            Console.Clear();
        }

        // ───────────────────────────────────────────────────────────────
        //  FIXED HEADER + STATUS BAR
        // ───────────────────────────────────────────────────────────────
        static void DrawFixedHeader()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("──────────────────────────────────────────────────────────────");
            Console.WriteLine("         Press 'q' to stop | Press 'r' to restart");
            Console.WriteLine("──────────────────────────────────────────────────────────────");
            Console.WriteLine(); // empty line
        }

        static void DrawStatusBar()
        {
            Console.SetCursorPosition(0, 4);
            Console.WriteLine(
                $"   Time: {DateTime.Now:HH:mm:ss}   " +
                $"Telegrams sent: {_telegramCount}   " +
                $"Status: Running      "
            );
            Console.WriteLine("──────────────────────────────────────────────────────────────");
        }

        static void DrawFooter(string portName, int intervalMs, string mode, int baudRate)
        {
            int row = FooterRow;

            // Clamp to avoid beep if window is too small
            if (row < 0)
            {
                row = 0;
            }

            Console.SetCursorPosition(0, row);
            Console.WriteLine(
                $" Port: {portName} | Baudrate: {baudRate} | Interval: {intervalMs} ms | Mode: {mode} | Press 'q' to stop, Ctrl-C to interrupt "
                    .PadRight(Console.WindowWidth - 1)
            );
        }

        // ───────────────────────────────────────────────────────────────
        //  MAIN SIMULATOR LOOP
        // ───────────────────────────────────────────────────────────────
        private static async Task RunSimulator()
        {
            MoveConsoleTo(100, 100);
            _cts = new CancellationTokenSource();
            _telegramCount = 0;

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

            // Splash screen
            ShowSplash(portName, profile.IntervalMs, profile.Mode.ToString(), sender.BaudRate);

            // Draw fixed UI
            DrawFixedHeader();
            DrawStatusBar();
            DrawFooter(portName, profile.IntervalMs, profile.Mode.ToString(), sender.BaudRate);

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

                    // Update counters + status bar
                    _telegramCount++;
                    DrawStatusBar();

                    // Generate telegram
                    string telegram = generator.GenerateTelegram(profile);

                    // Print telegram below UI
                    Console.SetCursorPosition(0, 7);
                    Console.WriteLine("Sending telegram:");
                    Console.WriteLine();
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
        static void SetConsoleWindowLocation(int x, int y)
        {
            IntPtr handle = GetConsoleWindow();
            if (handle == IntPtr.Zero)
                return;

            // Get current size
            int width = Console.WindowWidth * 8;   // approx. pixel width per char
            int height = Console.WindowHeight * 16; // approx. pixel height per line

            MoveWindow(handle, x, y, width, height, true);
        }
        static void MoveConsoleTo(int x, int y)
        {
            IntPtr handle = GetConsoleWindow();
            if (handle == IntPtr.Zero)
                return;

            // Get current window rectangle
            GetWindowRect(handle, out RECT rect);

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            // Move window to (x,y) but keep same width/height
            MoveWindow(handle, x, y, width, height, true);
        }

        static void ShowAboutPopup()
        {
            Console.Clear();

            string[] lines =
            {
                "P1 Simulator",
                "Version 1.0",
                "By Geert Vancompernolle (2026)",
                "",
                "A Dutch Smart Meter (DSMR/SMR) telegram generator",
                "for testing UART serial receivers.",
                "",
                "Press any key to return..."
            };

            int boxWidth = lines.Max(l => l.Length) + 4;
            int boxHeight = lines.Length + 4;

            int left = (Console.WindowWidth - boxWidth) / 2;
            int top = (Console.WindowHeight - boxHeight) / 2;

            // Draw box
            Console.SetCursorPosition(left, top);
            Console.WriteLine("+" + new string('-', boxWidth - 2) + "+");

            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(left, top + 1 + i);
                Console.WriteLine("| " + lines[i].PadRight(boxWidth - 4) + " |");
            }

            Console.SetCursorPosition(left, top + boxHeight - 1);
            Console.WriteLine("+" + new string('-', boxWidth - 2) + "+");

            Console.ReadKey(true); // wait for any key
        }

    }
}
