using System;
using System.Collections.Generic;

namespace P1Simulator.Simulation
{
    /// <summary>
    /// A simulation profile that generates realistic random DSMR values.
    /// Useful for stress testing and dynamic behavior.
    /// </summary>
    public class RandomProfile : ISimulationProfile
    {
        public string Name => "random";

        private readonly Random _rnd = new Random();

        public Dictionary<string, string> GenerateValues()
        {
            // Generate realistic DSMR-like values
            double import = _rnd.NextDouble() * 5000;          // 0–5000 kWh
            double export = _rnd.NextDouble() * 2000;          // 0–2000 kWh
            double voltageL1 = 230 + (_rnd.NextDouble() * 4 - 2); // 228–232 V
            double voltageL2 = 230 + (_rnd.NextDouble() * 4 - 2);
            double voltageL3 = 230 + (_rnd.NextDouble() * 4 - 2);
            double currentL1 = _rnd.NextDouble() * 25;         // 0–25 A
            double currentL2 = _rnd.NextDouble() * 25;
            double currentL3 = _rnd.NextDouble() * 25;
            double gas = _rnd.NextDouble() * 500;              // 0–500 m³

            return new Dictionary<string, string>
            {
                ["%IMPORT"] = import.ToString("F3"),
                ["%EXPORT"] = export.ToString("F3"),

                ["%VOLTAGE_L1"] = voltageL1.ToString("F1"),
                ["%VOLTAGE_L2"] = voltageL2.ToString("F1"),
                ["%VOLTAGE_L3"] = voltageL3.ToString("F1"),

                ["%CURRENT_L1"] = currentL1.ToString("F1"),
                ["%CURRENT_L2"] = currentL2.ToString("F1"),
                ["%CURRENT_L3"] = currentL3.ToString("F1"),

                ["%GAS"] = gas.ToString("F3")
            };
        }
    }
}
