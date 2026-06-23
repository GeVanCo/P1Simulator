namespace P1Simulator.Settings
{
    public class SimulatorSettings
    {
        public string Template { get; set; } = "3phase";
        public string Profile { get; set; } = "fixed";
        public int SpeedMs { get; set; } = 850; // Looks like the most optimal refresh frequency for the simulator:
                                                // - no double entries in one second
                                                // - no empty pages because of deduplication mechanism (if timestamp is same, send empty page)
        public int ConsoleWidth { get; set; } = 120;
        public int ConsoleHeight { get; set; } = 40;

        // ⭐ NEW: URL of the live P1 meter (HomeWizard)
        public string LiveP1Url { get; set; } = "http://192.168.1.60/api/v1/telegram";
    }
}
