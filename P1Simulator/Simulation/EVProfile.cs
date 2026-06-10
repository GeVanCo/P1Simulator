using System;
using System.Collections.Generic;

namespace P1Simulator.Simulation
{
    public class EVProfile : ISimulationProfile
    {
        public string Name => "ev";

        public Dictionary<string, string> GenerateValues()
        {
            double voltage = 230.0;
            double current = 16.0; // typical EV charging

            return new Dictionary<string, string>
            {
                ["{TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),
                ["{GAS_TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),

                ["{IMPORT_LOW}"] = "1500.000",
                ["{IMPORT_HIGH}"] = "2500.000",
                ["{EXPORT_LOW}"] = "0.000",
                ["{EXPORT_HIGH}"] = "0.000",

                ["{POWER_IMPORT}"] = ((voltage * current) / 1000.0).ToString("F3"),
                ["{POWER_EXPORT}"] = "0.000",

                ["{VOLTAGE_L1}"] = voltage.ToString("F1"),
                ["{VOLTAGE_L2}"] = voltage.ToString("F1"),
                ["{VOLTAGE_L3}"] = voltage.ToString("F1"),

                ["{CURRENT_L1}"] = current.ToString("F1"),
                ["{CURRENT_L2}"] = "0.0",
                ["{CURRENT_L3}"] = "0.0",

                ["{GAS_M3}"] = "123.456",
                ["{MONTHLY_PEAK}"] = "5.000"
            };
        }
    }
}
