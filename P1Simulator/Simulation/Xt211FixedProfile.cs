using P1Simulator.Simulation;

namespace P1Simulator.Simulation
{
    public class Xt211FixedProfile : ISimulationProfile
    {
        public string Name => "xt211fixed";

        public bool EnableElectricity => true;
        public bool EnableGas => false;
        public bool EnableWater => false;
        public bool EnableHeat => false;
        public bool EnableCapacityTariff => true;

        public Dictionary<string, string> GenerateValues()
        {
            return new Dictionary<string, string>
            {
                ["{METER_TYPE}"] = "50221",
                ["{FIRMWARE_VERSION}"] = "400",
                ["{EQUIPMENT_ID}"] = "3153414733323030323438313337",

                ["{TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss") + "S",

                ["{IMPORT_LOW}"] = "000678.652",
                ["{IMPORT_HIGH}"] = "000792.636",
                ["{EXPORT_LOW}"] = "001996.611",
                ["{EXPORT_HIGH}"] = "000716.496",

                ["{TARIFF_INDICATOR}"] = "0002",

                ["{ACTUAL_POWER_L1}"] = "00.000",
                ["{PEAK_TIMESTAMP}"] = "260610173000S",
                ["{PEAK_VALUE}"] = "01.660",

                ["{PEAK_EVENTS_COUNT}"] = "13",
                ["{PEAK_EVENTS}"] = Xt211PeakEventsBuilder.BuildFixed(),

                ["{ACTUAL_IMPORT}"] = "00.000",
                ["{ACTUAL_EXPORT}"] = "02.445",

                ["{L1_IMPORT}"] = "00.000",
                ["{L2_IMPORT}"] = "00.000",
                ["{L3_IMPORT}"] = "00.049",

                ["{L1_EXPORT}"] = "00.411",
                ["{L2_EXPORT}"] = "02.083",
                ["{L3_EXPORT}"] = "00.000",

                ["{VOLTAGE_L1}"] = "232.9",
                ["{VOLTAGE_L2}"] = "236.4",
                ["{VOLTAGE_L3}"] = "231.6",

                ["{CURRENT_L1}"] = "001.90",
                ["{CURRENT_L2}"] = "008.83",
                ["{CURRENT_L3}"] = "000.37",

                ["{BREAKER_STATE}"] = "1",
                ["{MAX_POWER}"] = "99.999",
                ["{MAX_CURRENT}"] = "999.99"
            };
        }
    }
}
