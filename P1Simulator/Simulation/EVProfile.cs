using System;
using System.Collections.Generic;

namespace P1Simulator.Simulation
{
    /// <summary>
    /// Simulation profile that mimics EV charging behavior.
    /// High import power, stable voltage, and strong current draw.
    /// </summary>
    public class EVProfile : ISimulationProfile
    {
        public string Name => "ev";

        private readonly Random _rnd = new Random();

        public Dictionary<string, string> GenerateValues()
        {
            // EV charging current typically 10–32 A depending on charger
            double current = 16 + _rnd.NextDouble() * 8; // 16–24 A

            double voltage = 230.0;
            double powerWatt = voltage * current;        // P = U * I
            double importKwh = powerWatt / 1000.0;       // convert to kWh

            return new Dictionary<string, string>
            {
                ["%IMPORT"] = importKwh.ToString("F3"), // EV draws power
                ["%EXPORT"] = "0.000",                  // no export
                ["%VOLTAGE_L1"] = voltage.ToString("F1"),   // stable voltage
                ["%CURRENT_L1"] = current.ToString("F1"),   // high current

                // Optional 3‑phase values (EV chargers can be 1‑ or 3‑phase)
                ["%VOLTAGE_L2"] = voltage.ToString("F1"),
                ["%VOLTAGE_L3"] = voltage.ToString("F1"),
                ["%CURRENT_L2"] = "0.0",
                ["%CURRENT_L3"] = "0.0",

                // Gas unchanged
                ["%GAS"] = "0.000"
            };
        }
    }
}
