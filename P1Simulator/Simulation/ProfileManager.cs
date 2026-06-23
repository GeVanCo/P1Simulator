
namespace P1Simulator.Simulation
{
    /// <summary>
    /// Manages all simulation profiles and provides lookup by name.
    /// </summary>
    public class ProfileManager
    {
        private readonly Dictionary<string, ISimulationProfile> _profiles = [];

        public ProfileManager()
        {
            // Register built‑in profiles
            Register(new FixedProfile());
            Register(new RandomProfile());
            Register(new SolarProfile());
            Register(new EVProfile());
            Register(new Xt211FixedProfile());
            Register(new Xt211RandomProfile());

            // ⭐ NEW: live P1 profile (HTTP passthrough)
            Register(new LiveP1Profile());
        }

        /// <summary>
        /// Registers a profile by its unique Name.
        /// </summary>
        public void Register(ISimulationProfile profile)
        {
            _profiles[profile.Name] = profile;
        }

        /// <summary>
        /// Retrieves a profile by name (e.g. "fixed", "random", "solar", "ev").
        /// Returns null if not found.
        /// </summary>
        public ISimulationProfile? Get(string name)
        {
            return _profiles.TryGetValue(name, out var p) ? p : null;
        }

        /// <summary>
        /// Returns all registered profile names.
        /// </summary>
        public IEnumerable<string> List()
        {
            return _profiles.Keys;
        }
    }
}
