using P1Simulator.Logging;
using P1Simulator.Serial;
using P1Simulator.Simulation;
using P1Simulator.Telegrams;
using P1Simulator.ConsoleUI;
using System.Reflection;
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
                    Thread.Sleep(1000);
                    return;
                }

                if (choice == 'r')
                {
                    Console.WriteLine("\nRestarting simulator...");
                    continue;
                }

                if (choice == 'a')
                {
                    ShowAboutPopup();
                    Console.Clear();
                    continue;
                }
            }
        }

        // ───────────────────────────────────────────────────────────────
        //  SPLASH SCREEN
        // ───────────────────────────────────────────────────────────────
        static void ShowSplash(string portName, string template, string profile, bool badCrc, int baudRate)
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
            Console.WriteLine($" Template     : {template}");
            Console.WriteLine($" Profile      : {profile}");
            Console.WriteLine($" CRC Mode     : {(badCrc ? "BAD (forced)" : "GOOD")}");
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
        //  FIXED HEADER
        // ───────────────────────────────────────────────────────────────
        static void DrawFixedHeader()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("──────────────────────────────────────────────────────────────");
            Console.WriteLine("       'q' -> Stop, Ctrl+C -> interrupt, 'a' -> About");
            Console.WriteLine("──────────────────────────────────────────────────────────────");
        }

        // ───────────────────────────────────────────────────────────────
        //  FIXED STATUS BAR
        // ───────────────────────────────────────────────────────────────
        static void DrawStatusBar(string template, string profile, bool badCrc)
        {
            Console.SetCursorPosition(0, 3);
            Console.WriteLine(
                $"   Time: {DateTime.Now:HH:mm:ss}   " +
                $"Telegrams sent: {_telegramCount}   " +
                $"Template: {template}   " +
                $"Profile: {profile}   " +
                $"CRC: {(badCrc ? "BAD" : "GOOD")}   "
            );
            Console.WriteLine("──────────────────────────────────────────────────────────────");
        }

        // ───────────────────────────────────────────────────────────────
        //  FIXED FOOTER
        // ───────────────────────────────────────────────────────────────
        static void DrawFooter(string portName, int baudRate)
        {
            int row = FooterRow;
            if (row < 0) row = 0;

            Console.SetCursorPosition(0, row);
            Console.WriteLine(
                $" Port: {portName} | Baudrate: {baudRate}"
                    .PadRight(Console.WindowWidth - 1)
            );
        }

        static void DrawCommandPrompt()
        {
            int row = FooterRow + 1;

            if (row >= Console.WindowHeight)
                row = Console.WindowHeight - 1;

            Console.SetCursorPosition(0, row);
            Console.Write(" Command: ".PadRight(Console.WindowWidth - 1));
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

            // --- NEW: Logger + Managers + Generator + CommandParser ---
            var logger = new Logger();
            var templates = new TemplateManager();
            var profiles = new ProfileManager();
            var generator = new TelegramGenerator(templates, profiles);
            var commands = new CommandParser(templates, profiles, generator);

            // --- NEW: Auto-detect COM port ---
            string? portName = ComPortDetectorHybrid.AutoDetect();
            if (portName == null)
            {
                Console.WriteLine("ERROR: No USB‑UART adapter detected.");
                return;
            }

            logger.Info($"Using COM port: {portName}");

            // --- NEW: SerialSender with logger ---
            var sender = new SerialSender(logger, portName);
            sender.Open();

            // Splash screen
            ShowSplash(portName, commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc, sender.BaudRate);

            // Draw fixed UI
            DrawFixedHeader();
            DrawStatusBar(commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc);
            DrawFooter(portName, sender.BaudRate);
            DrawCommandPrompt();   // NEW

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    // ---------------------------------------------------------
                    // HOTKEYS (q, a) — single key, no Enter required
                    // ---------------------------------------------------------
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true);

                        // Quit
                        if (key.Key == ConsoleKey.Q)
                        {
                            Console.WriteLine("Stopping simulator...");
                            _cts.Cancel();
                            break;
                        }

                        // About popup
                        if (key.Key == ConsoleKey.A)
                        {
                            ShowAboutPopup();
                            Console.Clear();
                            DrawFixedHeader();
                            DrawStatusBar(commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc);
                            DrawFooter(portName, sender.BaudRate);
                            DrawCommandPrompt();
                            continue;
                        }

                        // -----------------------------------------------------
                        // COMMAND INPUT (dedicated footer line)
                        // -----------------------------------------------------
                        Console.SetCursorPosition(10, FooterRow + 1); // after "Command: "
                        string? rest = Console.ReadLine();
                        string fullCommand = key.KeyChar + (rest ?? "");

                        commands.Handle(fullCommand);

                        DrawStatusBar(commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc);
                        DrawFooter(portName, sender.BaudRate);
                        DrawCommandPrompt();

                        continue;
                    }

                    // ---------------------------------------------------------
                    // NORMAL TELEGRAM LOOP
                    // ---------------------------------------------------------
                    _telegramCount++;
                    DrawStatusBar(commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc);

                    string telegram = generator.Generate(commands.CurrentTemplate, commands.CurrentProfile);

                    Console.SetCursorPosition(0, 7);
                    Console.WriteLine("Sending telegram:");
                    Console.WriteLine();
                    Console.WriteLine(telegram);

                    sender.Send(telegram);

                    await Task.Delay(1000, _cts.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // Expected on shutdown
            }
            finally
            {
                Console.WriteLine("Shutting down...");
                logger.Info("Shutting down simulator...");

                sender.Dispose();
                logger.Info("Serial port closed.");

                logger.Flush();
            }
        }

        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Ctrl+C detected. Stopping...");
            e.Cancel = true;
            _cts.Cancel();
        }

        static void MoveConsoleTo(int x, int y)
        {
            IntPtr handle = GetConsoleWindow();
            if (handle == IntPtr.Zero)
                return;

            GetWindowRect(handle, out RECT rect);

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            MoveWindow(handle, x, y, width, height, true);
        }

        static void ShowAboutPopup()
        {
            Console.Clear();

            string[] lines =
            {
                "P1 Simulator",
                "Version 1.0",
                "By Geert Vancompernolle (c. 2026 - 2026)",
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

            Console.SetCursorPosition(left, top);
            Console.WriteLine("+" + new string('-', boxWidth - 2) + "+");

            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(left, top + 1 + i);
                Console.WriteLine("| " + lines[i].PadRight(boxWidth - 4) + " |");
            }

            Console.SetCursorPosition(left, top + boxHeight - 3);
            Console.WriteLine("+" + new string('-', boxWidth - 2) + "+");

            Console.ReadKey(true);
        }
    }
}
