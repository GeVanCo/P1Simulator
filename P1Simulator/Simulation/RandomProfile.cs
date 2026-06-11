
namespace P1Simulator.Simulation
{
    public class RandomProfile : ISimulationProfile
    {
        public string Name => "random";
        private readonly Random _rnd = new Random();

        public bool EnableElectricity => true;
        public bool EnableGas => true;
        public bool EnableWater => true;
        public bool EnableHeat => true;
        public bool EnableCapacityTariff => true;

        public Dictionary<string, string> GenerateValues()
        {
            // Voltages (realistic 225–235 V)
            double v1 = 225 + _rnd.NextDouble() * 10;
            double v2 = 225 + _rnd.NextDouble() * 10;
            double v3 = 225 + _rnd.NextDouble() * 10;

            // Currents (0–20 A)
            double c1 = _rnd.NextDouble() * 20;
            double c2 = _rnd.NextDouble() * 20;
            double c3 = _rnd.NextDouble() * 20;

            // Energy totals
            double importLow = 1000 + _rnd.NextDouble() * 100;
            double importHigh = 2000 + _rnd.NextDouble() * 100;

            // Gas (m³)
            double gas = 100 + _rnd.NextDouble() * 10;

            // Water (m³)
            double water = 10 + _rnd.NextDouble() * 5;

            // Heat (GJ)
            double heat = 3 + _rnd.NextDouble() * 2;

            // Monthly peak (kW)
            double monthlyPeak = 2 + _rnd.NextDouble() * 3;

            return new Dictionary<string, string>
            {
                // Timestamps
                ["{TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),
                ["{GAS_TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),
                ["{WATER_TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),
                ["{HEAT_TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),

                // Totals
                ["{IMPORT_LOW}"] = importLow.ToString("F3"),
                ["{IMPORT_HIGH}"] = importHigh.ToString("F3"),
                ["{EXPORT_LOW}"] = "0.000",
                ["{EXPORT_HIGH}"] = "0.000",

                // Instantaneous power
                ["{POWER_IMPORT}"] = ((v1 * c1) / 1000.0).ToString("F3"),
                ["{POWER_EXPORT}"] = "0.000",

                // Voltages
                ["{VOLTAGE_L1}"] = v1.ToString("F1"),
                ["{VOLTAGE_L2}"] = v2.ToString("F1"),
                ["{VOLTAGE_L3}"] = v3.ToString("F1"),

                // Currents
                ["{CURRENT_L1}"] = c1.ToString("F1"),
                ["{CURRENT_L2}"] = c2.ToString("F1"),
                ["{CURRENT_L3}"] = c3.ToString("F1"),

                // Gas / Water / Heat
                ["{GAS_M3}"] = gas.ToString("F3"),
                ["{WATER_M3}"] = water.ToString("F3"),
                ["{HEAT_GJ}"] = heat.ToString("F3"),

                // Capacity tariff
                ["{MONTHLY_PEAK}"] = monthlyPeak.ToString("F3")
            };
        }
    }
}
