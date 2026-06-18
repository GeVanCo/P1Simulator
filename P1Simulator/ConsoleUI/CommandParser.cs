using P1Simulator.Telegrams;
using P1Simulator.Simulation;
using P1Simulator.Settings;

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
        private readonly SimulatorSettings _settings;

        // ⭐ NEW: Dictionary of commands → description
        private readonly Dictionary<string, string> _commandDescriptions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "template", "template <name>               - Switch template (use 'list templates')" },
                { "profile",  "profile  <name>               - Switch profile  (use 'list profiles')" },
                { "crc",      "crc      <good|bad>           - Set CRC mode" },
                { "speed",    "speed    <ms>                 - Set telegram interval in milliseconds" },
                { "list",     "list     <templates|profiles> - List templates or profiles" }
            };

        public string CurrentTemplate { get; private set; } = "3phase";
        public string CurrentProfile { get; private set; } = "random";

        public event Action<string, IEnumerable<string>>? OnListRequested;

        public CommandParser(
            TemplateManager templates,
            ProfileManager profiles,
            TelegramGenerator generator,
            SimulatorSettings settings)
        {
            _templates = templates;
            _profiles = profiles;
            _generator = generator;
            _settings = settings;

            // ⭐ Apply persisted settings
            if (_templates.Get(settings.Template) != null)
                CurrentTemplate = settings.Template;

            if (_profiles.Get(settings.Profile) != null)
                CurrentProfile = settings.Profile;

            _generator.SetTemplate(CurrentTemplate);
            _generator.SetProfile(CurrentProfile);
        }

        /// <summary>
        /// ⭐ Expose commands for dynamic help screen.
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

                case "speed":
                    if (parts.Length < 2 || !int.TryParse(parts[1], out int ms) || ms < 100)
                    {
                        Console.WriteLine("Usage: speed <milliseconds> (min 100)");
                        return;
                    }
                    SetSpeed(ms);
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
            _generator.SetTemplate(name);

            // ⭐ Persist
            _settings.Template = name;
            SettingsManager.Save(_settings);

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
            _generator.SetProfile(name);

            // ⭐ Persist
            _settings.Profile = name;
            SettingsManager.Save(_settings);

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

        private void SetSpeed(int ms)
        {
            _settings.SpeedMs = ms;
            SettingsManager.Save(_settings);

            Console.WriteLine($"Telegram speed set to {ms} ms.");
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
