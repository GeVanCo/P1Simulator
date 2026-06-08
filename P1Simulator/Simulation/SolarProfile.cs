using System;
using System.Collections.Generic;

namespace P1Simulator.Simulation
{
    /// <summary>
    /// Simulation profile that mimics solar panel production.
    /// Export power rises and falls smoothly using a sine wave.
    /// </summary>
    public class SolarProfile : ISimulationProfile
    {
        public string Name => "solar";

        private double _t = 0.0;   // internal time counter

        public Dictionary<string, string> GenerateValues()
        {
            _t += 0.1; // controls speed of change

            // Simulate solar export: 0 → 3000 W → 0 (smooth sinusoidal)
            double exportWatt = Math.Max(0, Math.Sin(_t) * 3000.0);

            // Convert to kWh for DSMR total export placeholder
            double exportKwh = exportWatt / 1000.0;

            // Current = P / V
            double voltage = 230.0;
            double current = exportWatt / voltage;

            return new Dictionary<string, string>
            {
                ["%IMPORT"] = "0.000",                     // no import during solar export
                ["%EXPORT"] = exportKwh.ToString("F3"),    // export energy
                ["%VOLTAGE_L1"] = voltage.ToString("F1"),      // stable voltage
                ["%CURRENT_L1"] = current.ToString("F1"),      // export current
                ["%VOLTAGE_L2"] = voltage.ToString("F1"),      // optional for 3‑phase
                ["%VOLTAGE_L3"] = voltage.ToString("F1"),
                ["%CURRENT_L2"] = "0.0",
                ["%CURRENT_L3"] = "0.0",
                ["%GAS"] = "0.000"                      // unchanged
            };
        }
    }
}
