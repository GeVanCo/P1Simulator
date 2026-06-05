
namespace P1Simulator.Simulation
{
    public class SimulationProfile
    {
        public SimulationMode Mode { get; }
        public int IntervalMs { get; }

        public SimulationProfile(SimulationMode mode, int intervalMs = 1000)
        {
            Mode = mode;
            IntervalMs = intervalMs;
        }
    }
}
