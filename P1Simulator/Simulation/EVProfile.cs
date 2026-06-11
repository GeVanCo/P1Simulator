
namespace P1Simulator.Simulation
{
    public class EVProfile : ISimulationProfile
    {
        public string Name => "ev";

        public bool EnableElectricity => true;
        public bool EnableGas => true;
        public bool EnableWater => false;
        public bool EnableHeat => false;
        public bool EnableCapacityTariff => true;
        public Dictionary<string, string> GenerateValues()
        {
            double voltage = 230.0;
            double current = 16.0; // typical EV charging current
            double powerImportKw = (voltage * current) / 1000.0;

            // Water & heat: EV charging does not affect these,
            // so we use stable, realistic values.
            double waterM3 = 123.456;
            double heatGj = 4.567;

            return new Dictionary<string, string>
            {
                // Timestamps
                ["{TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),
                ["{GAS_TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),
                ["{WATER_TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),
                ["{HEAT_TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),

                // Totals
                ["{IMPORT_LOW}"] = "1500.000",
                ["{IMPORT_HIGH}"] = "2500.000",
                ["{EXPORT_LOW}"] = "0.000",
                ["{EXPORT_HIGH}"] = "0.000",

                // Instantaneous power
                ["{POWER_IMPORT}"] = powerImportKw.ToString("F3"),
                ["{POWER_EXPORT}"] = "0.000",

                // Voltages
                ["{VOLTAGE_L1}"] = voltage.ToString("F1"),
                ["{VOLTAGE_L2}"] = voltage.ToString("F1"),
                ["{VOLTAGE_L3}"] = voltage.ToString("F1"),

                // Currents
                ["{CURRENT_L1}"] = current.ToString("F1"),
                ["{CURRENT_L2}"] = "0.0",
                ["{CURRENT_L3}"] = "0.0",

                // Gas / Water / Heat
                ["{GAS_M3}"] = "123.456",
                ["{WATER_M3}"] = waterM3.ToString("F3"),
                ["{HEAT_GJ}"] = heatGj.ToString("F3"),

                // Capacity tariff
                ["{MONTHLY_PEAK}"] = "5.000"
            };
        }

        Dictionary<string, string> ISimulationProfile.GenerateValues()
        {
            throw new NotImplementedException();
        }
    }
}
