using P1Simulator.Networking;
using P1Simulator.Settings;
using P1Simulator.Telegrams;

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
        private string? _lastTimestamp = null;

        public LiveP1Profile()
        {
            var settings = SettingsManager.Load();
            _client = new P1HttpClient(settings.LiveP1Url);
        }

        public Dictionary<string, string> GenerateValues()
        {
            string raw = _client.FetchTelegramAsync().Result;

            string? ts = TimestampExtractor.Extract(raw);

            // ⭐ If timestamp is missing, forward anyway
            if (ts == null)
            {
                return new Dictionary<string, string>
                {
                    ["{RAW_TELEGRAM}"] = raw
                };
            }

            // ⭐ If timestamp unchanged → return EMPTY telegram (skip)
            if (ts == _lastTimestamp)
            {
                return new Dictionary<string, string>
                {
                    ["{RAW_TELEGRAM}"] = ""   // empty means "skip"
                };
            }

            // ⭐ New telegram → forward it
            _lastTimestamp = ts;

            return new Dictionary<string, string>
            {
                ["{RAW_TELEGRAM}"] = raw
            };
        }
    }
}
