using System;
using System.Collections.Generic;

namespace P1Simulator.Simulation
{
    public class RandomProfile : ISimulationProfile
    {
        public string Name => "random";
        private readonly Random _rnd = new Random();

        public Dictionary<string, string> GenerateValues()
        {
            double v1 = 225 + _rnd.NextDouble() * 10;
            double v2 = 225 + _rnd.NextDouble() * 10;
            double v3 = 225 + _rnd.NextDouble() * 10;

            double c1 = _rnd.NextDouble() * 20;
            double c2 = _rnd.NextDouble() * 20;
            double c3 = _rnd.NextDouble() * 20;

            double importLow = 1000 + _rnd.NextDouble() * 100;
            double importHigh = 2000 + _rnd.NextDouble() * 100;

            double gas = 100 + _rnd.NextDouble() * 10;

            return new Dictionary<string, string>
            {
                ["{TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),
                ["{GAS_TIMESTAMP}"] = DateTime.Now.ToString("yyMMddHHmmss"),

                ["{IMPORT_LOW}"] = importLow.ToString("F3"),
                ["{IMPORT_HIGH}"] = importHigh.ToString("F3"),
                ["{EXPORT_LOW}"] = "0.000",
                ["{EXPORT_HIGH}"] = "0.000",

                ["{POWER_IMPORT}"] = ((v1 * c1) / 1000.0).ToString("F3"),
                ["{POWER_EXPORT}"] = "0.000",

                ["{VOLTAGE_L1}"] = v1.ToString("F1"),
                ["{VOLTAGE_L2}"] = v2.ToString("F1"),
                ["{VOLTAGE_L3}"] = v3.ToString("F1"),

                ["{CURRENT_L1}"] = c1.ToString("F1"),
                ["{CURRENT_L2}"] = c2.ToString("F1"),
                ["{CURRENT_L3}"] = c3.ToString("F1"),

                ["{GAS_M3}"] = gas.ToString("F3"),
                ["{MONTHLY_PEAK}"] = (2 + _rnd.NextDouble() * 3).ToString("F3")
            };
        }
    }
}
