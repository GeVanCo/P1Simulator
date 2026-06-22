using System.Text.Json;

namespace P1Simulator.Settings
{
    public static class SettingsManager
    {
        private static readonly string FilePath = "settings.json";

        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public static SimulatorSettings Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    var defaults = new SimulatorSettings();
                    Save(defaults);          // ⭐ create settings.json immediately
                    return defaults;
                }

                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<SimulatorSettings>(json)
                       ?? new SimulatorSettings();
            }
            catch
            {
                return new SimulatorSettings();
            }
        }

        public static void Save(SimulatorSettings settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(
                    settings,
                    _jsonOptions
                );

                File.WriteAllText(FilePath, json);
            }
            catch
            {
                // ignore write errors
            }
        }
    }
}
