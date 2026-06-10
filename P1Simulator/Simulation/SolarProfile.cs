
namespace P1Simulator.Simulation
{
    public class SolarProfile : ISimulationProfile
    {
        public string Name => "solar";

        public Dictionary<string, string> GenerateValues()
        {
            double voltage = 230.0;
            double exportPower = 2.5; // kW

            return new Dictionary<string, string>
            {
                ["{TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),
                ["{GAS_TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),

                ["{IMPORT_LOW}"] = "500.000",
                ["{IMPORT_HIGH}"] = "600.000",
                ["{EXPORT_LOW}"] = "100.000",
                ["{EXPORT_HIGH}"] = "200.000",

                ["{POWER_IMPORT}"] = "0.000",
                ["{POWER_EXPORT}"] = exportPower.ToString("F3"),

                ["{VOLTAGE_L1}"] = voltage.ToString("F1"),
                ["{VOLTAGE_L2}"] = voltage.ToString("F1"),
                ["{VOLTAGE_L3}"] = voltage.ToString("F1"),

                ["{CURRENT_L1}"] = "0.0",
                ["{CURRENT_L2}"] = "0.0",
                ["{CURRENT_L3}"] = "0.0",

                ["{GAS_M3}"] = "123.456",
                ["{MONTHLY_PEAK}"] = "1.234",

                ["{WATER_M3}"] = "123.456",
                ["{HEAT_GJ}"] = "4.567",

            };
        }
    }
}
