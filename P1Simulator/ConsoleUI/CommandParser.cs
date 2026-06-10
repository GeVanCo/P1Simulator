using P1Simulator.Telegrams;
using P1Simulator.Simulation;

namespace P1Simulator.ConsoleUI
{
    /// <summary>
    /// Parses console commands and updates simulator state.
    /// </summary>
    public class CommandParser
    {
        private readonly TemplateManager _templates;
        private readonly ProfileManager _profiles;
        private readonly TelegramGenerator _generator;

        // ⭐ NEW: Dictionary of commands → description
        private readonly Dictionary<string, string> _commandDescriptions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "template", "template <1phase|3phase|gas>    - Switch to a different telegram template" },
                { "profile",  "profile <fixed|random|solar|ev> - Switch to a different meter profile" },
                { "crc",      "crc <good|bad>                  - Set CRC mode" },
                { "list",     "list <templates|profiles>       - List available templates or profiles" }
            };

        public string CurrentTemplate { get; private set; } = "3phase";
        public string CurrentProfile { get; private set; } = "random";

        public event Action<string, IEnumerable<string>>? OnListRequested;

        public CommandParser(
            TemplateManager templates,
            ProfileManager profiles,
            TelegramGenerator generator)
        {
            _templates = templates;
            _profiles = profiles;
            _generator = generator;
        }

        /// <summary>
        /// ⭐ NEW: Expose commands for dynamic help screen.
        /// </summary>
        public IReadOnlyDictionary<string, string> GetCommands()
        {
            return _commandDescriptions;
        }

        /// <summary>
        /// Handles a single console command.
        /// </summary>
        public void Handle(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return;

            string cmd = parts[0].ToLower();

            switch (cmd)
            {
                case "template":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: template <name>");
                        return;
                    }
                    SetTemplate(parts[1]);
                    break;

                case "profile":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: profile <name>");
                        return;
                    }
                    SetProfile(parts[1]);
                    break;

                case "crc":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: crc <good|bad>");
                        return;
                    }
                    SetCrcMode(parts[1]);
                    break;

                case "list":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: list <templates|profiles>");
                        return;
                    }
                    ListItems(parts[1]);
                    break;

                default:
                    Console.WriteLine($"Unknown command: {cmd}");
                    break;
            }
        }

        private void SetTemplate(string name)
        {
            if (_templates.Get(name) == null)
            {
                Console.WriteLine($"Template '{name}' not found.");
                return;
            }

            CurrentTemplate = name;
            Console.WriteLine($"Template set to: {name}");
        }

        private void SetProfile(string name)
        {
            if (_profiles.Get(name) == null)
            {
                Console.WriteLine($"Profile '{name}' not found.");
                return;
            }

            CurrentProfile = name;
            Console.WriteLine($"Profile set to: {name}");
        }

        private void SetCrcMode(string mode)
        {
            switch (mode.ToLower())
            {
                case "good":
                    _generator.ForceBadCrc = false;
                    Console.WriteLine("CRC mode: GOOD");
                    break;

                case "bad":
                    _generator.ForceBadCrc = true;
                    Console.WriteLine("CRC mode: BAD");
                    break;

                default:
                    Console.WriteLine("Usage: crc <good|bad>");
                    break;
            }
        }

        private void ListItems(string what)
        {
            switch (what.ToLower())
            {
                case "templates":
                    OnListRequested?.Invoke("Available Templates", _templates.List());
                    break;

                case "profiles":
                    OnListRequested?.Invoke("Available Profiles", _profiles.List());
                    break;

                default:
                    Console.WriteLine("Usage: list <templates|profiles>");
                    break;
            }
        }
    }
}
