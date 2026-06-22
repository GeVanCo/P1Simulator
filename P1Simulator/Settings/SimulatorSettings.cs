namespace P1Simulator.Settings
{
    public class SimulatorSettings
    {
        public string Template { get; set; } = "3phase";
        public string Profile { get; set; } = "fixed";
        public int SpeedMs { get; set; } = 1000;
        public int ConsoleWidth { get; set; } = 120;
        public int ConsoleHeight { get; set; } = 40;

    }
}
