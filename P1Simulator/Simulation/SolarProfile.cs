
namespace P1Simulator.Simulation
{
    public class SolarProfile : ISimulationProfile
    {
        public string Name => "solar";

        public bool EnableElectricity => true;
        public bool EnableGas => true;
        public bool EnableWater => false;
        public bool EnableHeat => false;
        public bool EnableCapacityTariff => true;
        public Dictionary<string, string> GenerateValues()
        {
            double voltage = 230.0;

            // Simulated solar export (2–4 kW)
            double exportKw = 2.0 + new Random().NextDouble() * 2.0;

            // Water & heat: unrelated to solar, so stable realistic values
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
                ["{IMPORT_LOW}"] = "500.000",
                ["{IMPORT_HIGH}"] = "600.000",
                ["{EXPORT_LOW}"] = "100.000",
                ["{EXPORT_HIGH}"] = "200.000",

                // Instantaneous power
                ["{POWER_IMPORT}"] = "0.000",
                ["{POWER_EXPORT}"] = exportKw.ToString("F3"),

                // Voltages
                ["{VOLTAGE_L1}"] = voltage.ToString("F1"),
                ["{VOLTAGE_L2}"] = voltage.ToString("F1"),
                ["{VOLTAGE_L3}"] = voltage.ToString("F1"),

                // Currents (export → negative current is possible, but we keep it simple)
                ["{CURRENT_L1}"] = "0.0",
                ["{CURRENT_L2}"] = "0.0",
                ["{CURRENT_L3}"] = "0.0",

                // Gas / Water / Heat
                ["{GAS_M3}"] = "123.456",
                ["{WATER_M3}"] = waterM3.ToString("F3"),
                ["{HEAT_GJ}"] = heatGj.ToString("F3"),

                // Capacity tariff
                ["{MONTHLY_PEAK}"] = "1.234"
            };
        }
    }
}
