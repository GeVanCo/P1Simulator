using P1Simulator.Logging;
using P1Simulator.Serial;
using P1Simulator.Simulation;
using P1Simulator.Telegrams;
using P1Simulator.ConsoleUI;
using P1Simulator.Settings;

using System.Collections.Concurrent;
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
        // GLOBAL STATE
        //============================================================================

        private static CancellationTokenSource _cts = new();
        private static int _telegramCount = 0;

        static int FooterRow => Console.WindowHeight - 3;

        private static readonly List<string> _history = [];
        private static int _historyIndex = -1;

        private static UInt32 _totalBytesSent = 0;
        private static UInt32 _lastTelegramBytes = 0;

        // Key listener / input
        private static bool _keyListenerRunning = false;
        private static readonly ConcurrentQueue<ConsoleKeyInfo> _inputQueue = new();
        private static string _cmdBuffer = "";

        // Popup / control flags
        private static volatile bool _requestAbout = false;
        private static volatile bool _requestHelp = false;
        private static volatile bool _requestRestart = false;
        private static volatile bool _requestQuit = false;

        // Persisted settings
        private static SimulatorSettings _settings = new();

        //============================================================================
        // MAIN ENTRY POINT
        //============================================================================

        //static async Task Main(string[] args)
        static async Task Main()
        {
            Console.Title = "P1 Dutch Smart Meter Reader Simulator";

            _settings = SettingsManager.Load();

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
            Console.WriteLine($" Speed        : {_settings.SpeedMs} ms");
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
            Console.WriteLine("──────────────────────────────────────────────────────────────────────────────────────────────");
            Console.WriteLine("           'q' -> Stop, Ctrl+C -> interrupt, 'a' -> About, 'h' -> help, 'r' -> restart");
            Console.WriteLine("──────────────────────────────────────────────────────────────────────────────────────────────");
        }

        // ───────────────────────────────────────────────────────────────
        //  FIXED STATUS BAR
        // ───────────────────────────────────────────────────────────────
        static void DrawStatusBar(string template, string profile, bool badCrc)
        {
            Console.SetCursorPosition(0, 3);
            Console.WriteLine(
                $"   Time: {DateTime.Now:HH:mm:ss}   " +
                $"Telegrams sent: {_telegramCount:D5}   " +
                $"Template: {template}   " +
                $"Profile: {profile}   " +
                $"CRC: {(badCrc ? "BAD" : "GOOD")}   " +
                $"Speed: {_settings.SpeedMs}ms   "
            );
            Console.WriteLine("──────────────────────────────────────────────────────────────────────────────────────────────");
        }

        // ───────────────────────────────────────────────────────────────
        //  FIXED FOOTER
        // ───────────────────────────────────────────────────────────────
        static void DrawFooter(string portName, int baudRate, UInt32 lastTelegramBytes, UInt32 totalBytesSent)
        {
            int row = FooterRow - 1;
            if (row < 0) row = 0;

            Console.SetCursorPosition(0, row);
            Console.WriteLine("──────────────────────────────────────────────────────────────────────────────────────────────");
            Console.WriteLine(
                $" Port: {portName} | Baudrate: {baudRate} | Last telegram bytes: {lastTelegramBytes} | Total bytes sent: {totalBytesSent}"
                    .PadRight(Console.WindowWidth - 1)
            );
        }

        // ───────────────────────────────────────────────────────────────
        //  COMMAND PROMPT
        // ───────────────────────────────────────────────────────────────
        static void DrawCommandPrompt()
        {
            int row = FooterRow + 1;

            if (row >= Console.WindowHeight)
                row = Console.WindowHeight - 2;

            Console.SetCursorPosition(0, row);
            Console.Write(" Command: ");
        }

        private static void RedrawCommandLine()
        {
            int row = FooterRow + 1;
            if (row >= Console.WindowHeight)
                row = Console.WindowHeight - 2;

            Console.SetCursorPosition(0, row);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, row);
            Console.Write(" Command: " + _cmdBuffer);
        }

        // ───────────────────────────────────────────────────────────────
        //  CTRL+C HANDLER
        // ───────────────────────────────────────────────────────────────
        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Ctrl+C detected. Stopping...");
            e.Cancel = true;
            _cts.Cancel();
        }

        // ───────────────────────────────────────────────────────────────
        //  GLOBAL KEY LISTENER (input collector)
        // ───────────────────────────────────────────────────────────────
        private static void KeyListener()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    _inputQueue.Enqueue(key);
                }

                Thread.Sleep(5);
            }
        }

        // ───────────────────────────────────────────────────────────────
        //  INPUT PROCESSING (non-blocking)
        // ───────────────────────────────────────────────────────────────
        private static void ProcessKey(ConsoleKeyInfo key, CommandParser commands)
        {
            bool typing = _cmdBuffer.Length > 0;

            // HOTKEYS ONLY WHEN NOT TYPING
            if (!typing)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Q:
                        _requestQuit = true;
                        return;

                    case ConsoleKey.A:
                        _requestAbout = true;
                        return;

                    case ConsoleKey.H:
                        _requestHelp = true;
                        return;

                    case ConsoleKey.R:
                        _requestRestart = true;
                        return;
                }
            }

            // COMMAND INPUT
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    HandleCommand(commands);
                    return;

                case ConsoleKey.Backspace:
                    if (_cmdBuffer.Length > 0)
                        _cmdBuffer = _cmdBuffer[..^1];
                    break;

                case ConsoleKey.UpArrow:
                    NavigateHistory(-1);
                    break;

                case ConsoleKey.DownArrow:
                    NavigateHistory(+1);
                    break;

                default:
                    if (!char.IsControl(key.KeyChar))
                        _cmdBuffer += key.KeyChar;
                    break;
            }

            RedrawCommandLine();
        }

        private static void HandleCommand(CommandParser commands)
        {
            string cmd = _cmdBuffer.Trim();

            if (!string.IsNullOrEmpty(cmd))
            {
                _history.Add(cmd);
                _historyIndex = _history.Count;
                commands.Handle(cmd);
            }

            _cmdBuffer = "";
            RedrawCommandLine();
        }

        private static void NavigateHistory(int direction)
        {
            if (_history.Count == 0)
                return;

            _historyIndex = Math.Clamp(_historyIndex + direction, 0, _history.Count);

            if (_historyIndex == _history.Count)
                _cmdBuffer = "";
            else
                _cmdBuffer = _history[_historyIndex];
        }

        // ───────────────────────────────────────────────────────────────
        //  MAIN SIMULATOR LOOP (short ticks + timed telegrams)
        // ───────────────────────────────────────────────────────────────
        private static async Task RunSimulator()
        {
            MoveConsoleTo(100, 100);
            _cts = new CancellationTokenSource();
            _telegramCount = 0;
            _historyIndex = -1;
            _totalBytesSent = 0;
            _lastTelegramBytes = 0;
            _cmdBuffer = "";
            while (_inputQueue.TryDequeue(out _)) { } // clear queue

            Console.CancelKeyPress += OnCancelKeyPress;

            if (!_keyListenerRunning)
            {
                _keyListenerRunning = true;
                _ = Task.Run(() => KeyListener());
            }

            var logger = new Logger();
            var templates = new TemplateManager();
            var profiles = new ProfileManager();
            var generator = new TelegramGenerator(templates, profiles);

            var commands = new CommandParser(templates, profiles, generator, _settings);

            string? portName = ComPortDetectorHybrid.AutoDetect();
            if (portName == null)
            {
                Console.WriteLine("ERROR: No USB‑UART adapter detected.");
                return;
            }

            logger.Info($"Using COM port: {portName}");

            var sender = new SerialSender(logger, portName);
            sender.Open();

            commands.OnListRequested += (title, items) =>
            {
                ShowListPopup(title, items);
                Console.Clear();
                DrawFixedHeader();
                DrawStatusBar(commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc);
                DrawFooter(portName, sender.BaudRate, _lastTelegramBytes, _totalBytesSent);
                DrawCommandPrompt();
                RedrawCommandLine();
            };

            ShowSplash(portName, commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc, sender.BaudRate);

            Console.Clear();
            DrawFixedHeader();
            DrawStatusBar(commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc);
            DrawFooter(portName, sender.BaudRate, _lastTelegramBytes, _totalBytesSent);
            DrawCommandPrompt();
            RedrawCommandLine();

            int telegramIntervalMs = _settings.SpeedMs;
            DateTime nextTelegramTime = DateTime.UtcNow;

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (_requestQuit)
                    {
                        _requestQuit = false;
                        _cts.Cancel();
                        break;
                    }

                    if (_requestRestart)
                    {
                        _requestRestart = false;
                        _cts.Cancel();
                        break;
                    }

                    if (_requestAbout)
                    {
                        _requestAbout = false;
                        ShowAboutPopup();
                        Console.Clear();
                        DrawFixedHeader();
                        DrawStatusBar(commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc);
                        DrawFooter(portName, sender.BaudRate, _lastTelegramBytes, _totalBytesSent);
                        DrawCommandPrompt();
                        RedrawCommandLine();
                    }

                    if (_requestHelp)
                    {
                        _requestHelp = false;
                        ShowHelpPopup(commands);
                        Console.Clear();
                        DrawFixedHeader();
                        DrawStatusBar(commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc);
                        DrawFooter(portName, sender.BaudRate, _lastTelegramBytes, _totalBytesSent);
                        DrawCommandPrompt();
                        RedrawCommandLine();
                    }

                    if (_inputQueue.TryDequeue(out var key))
                    {
                        ProcessKey(key, commands);
                    }

                    telegramIntervalMs = _settings.SpeedMs;

                    if (DateTime.UtcNow >= nextTelegramTime)
                    {
                        _telegramCount++;
                        DrawStatusBar(commands.CurrentTemplate, commands.CurrentProfile, generator.ForceBadCrc);

                        ClearTelegramArea();

                        string telegram = generator.Generate();

                        Console.SetCursorPosition(0, 7);
                        Console.WriteLine("Sending telegram:");
                        Console.WriteLine();
                        Console.WriteLine(telegram);

                        sender.Send(telegram);
                        _lastTelegramBytes = (UInt32)telegram.Length;
                        _totalBytesSent += _lastTelegramBytes;

                        DrawFooter(portName, sender.BaudRate, _lastTelegramBytes, _totalBytesSent);
                        RedrawCommandLine();

                        nextTelegramTime = DateTime.UtcNow.AddMilliseconds(telegramIntervalMs);
                    }

                    await Task.Delay(50, _cts.Token);
                }
            }
            catch (TaskCanceledException)
            {
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

        // ───────────────────────────────────────────────────────────────
        //  ABOUT POPUP
        // ───────────────────────────────────────────────────────────────
        static void ShowAboutPopup()
        {
            Console.Clear();

            List<string> lines =      // instead of new() {...}, we can do [...] instead.
            [
                "",
                "Version 1.0",
                "By Geert Vancompernolle (c. 2026 - 2026)",
                "",
                "A Dutch Smart Meter (DSMR/SMR) telegram generator",
                "for testing UART serial receivers.",
                "",
                "Press any key to return..."
            ];

            DrawUnicodePopup("P1 Simulator - About", lines);
        }

        // ───────────────────────────────────────────────────────────────
        //  HELP POPUP (COMMAND-AWARE)
        // ───────────────────────────────────────────────────────────────
        static void ShowHelpPopup(CommandParser commands)
        {
            Console.Clear();

            var cmdList = commands.GetCommands();

            List<string> lines = 
            [
                "",
                "Available Commands:",
                ""
            ];

            foreach (var entry in cmdList)
                lines.Add("  " + entry.Value);

            lines.Add("");
            lines.Add("Hotkeys:");
            lines.Add("");
            lines.Add("  a   - About screen");
            lines.Add("  h   - Help screen");
            lines.Add("  q   - Quit simulator");
            lines.Add("  r   - Restart simulator");
            lines.Add("");
            lines.Add("Press any key to return...");

            DrawUnicodePopup("P1 Simulator - Help", lines);
        }

        // ───────────────────────────────────────────────────────────────
        //  LIST POPUP
        // ───────────────────────────────────────────────────────────────
        static void ShowListPopup(string title, IEnumerable<string> items)
        {
            Console.Clear();

            List<string> lines =
            [
                ""
            ];

            foreach (var item in items)
                lines.Add("  " + item);

            lines.Add("");
            lines.Add("Press any key to return...");

            DrawUnicodePopup(title, lines);
        }

        // ───────────────────────────────────────────────────────────────
        //  UNICODE POPUP RENDERER
        // ───────────────────────────────────────────────────────────────
        static void DrawUnicodePopup(string title, List<string> lines)
        {
            Console.Clear();

            int boxWidth = Math.Max(
                Math.Max(title.Length, lines.Max(l => l.Length)) + 4,
                30
            );

            int boxHeight = lines.Count + 4;

            int left = (Console.WindowWidth - boxWidth) / 2;
            int top = (Console.WindowHeight - boxHeight) / 2;

            Console.SetCursorPosition(left, top);
            Console.WriteLine("┌" + new string('─', boxWidth - 2) + "┐");

            Console.SetCursorPosition(left, top + 1);
            Console.WriteLine("│ " + title.PadRight(boxWidth - 4) + " │");

            Console.SetCursorPosition(left, top + 2);
            Console.WriteLine("├" + new string('─', boxWidth - 2) + "┤");

            for (int i = 0; i < lines.Count; i++)
            {
                Console.SetCursorPosition(left, top + 3 + i);
                Console.WriteLine("│ " + lines[i].PadRight(boxWidth - 4) + " │");
            }

            Console.SetCursorPosition(left, top + boxHeight - 1);
            Console.WriteLine("└" + new string('─', boxWidth - 2) + "┘");

            Console.ReadKey(true);
        }

        // ───────────────────────────────────────────────────────────────
        //  CLEAR TELEGRAM AREA
        // ───────────────────────────────────────────────────────────────
        static void ClearTelegramArea()
        {
            for (int row = 7; row < FooterRow - 2; row++)
            {
                Console.SetCursorPosition(0, row);
                Console.Write(new string(' ', Console.WindowWidth - 1));
            }
        }

        // ───────────────────────────────────────────────────────────────
        //  MOVE CONSOLE WINDOW
        // ───────────────────────────────────────────────────────────────
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
    }
}
