using P1Simulator.Networking;
using P1Simulator.Settings;

namespace P1Simulator.Simulation
{
    public class LiveP1Profile : ISimulationProfile
    {
        public string Name => "live";

        public bool EnableElectricity => true;
        public bool EnableGas => true;
        public bool EnableWater => true;
        public bool EnableHeat => true;
        public bool EnableCapacityTariff => true;

        private readonly P1HttpClient _client;

        public LiveP1Profile()
        {
            // Load settings to get the URL
            var settings = SettingsManager.Load();
            _client = new P1HttpClient(settings.LiveP1Url);
        }

        public Dictionary<string, string> GenerateValues()
        {
            // Blocking call is fine here; generator is already synchronous
            string raw = _client.FetchTelegramAsync().Result;

            return new Dictionary<string, string>
            {
                ["{RAW_TELEGRAM}"] = raw
            };
        }
    }
}
