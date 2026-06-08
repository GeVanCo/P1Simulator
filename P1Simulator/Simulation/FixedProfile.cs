using System.Collections.Generic;

namespace P1Simulator.Simulation
{
    /// <summary>
    /// A simple fixed-value simulation profile.
    /// Produces stable placeholder values for deterministic testing.
    /// </summary>
    public class FixedProfile : ISimulationProfile
    {
        public string Name => "fixed";

        // Default values (you can change these at runtime later)
        public double Import { get; set; } = 1234.567;
        public double Export { get; set; } = 0.000;
        public double VoltageL1 { get; set; } = 230.0;
        public double CurrentL1 { get; set; } = 5.0;

        // Optional 3‑phase values
        public double VoltageL2 { get; set; } = 230.0;
        public double VoltageL3 { get; set; } = 230.0;
        public double CurrentL2 { get; set; } = 5.0;
        public double CurrentL3 { get; set; } = 5.0;

        // Optional gas value
        public double Gas { get; set; } = 123.456;

        public Dictionary<string, string> GenerateValues()
        {
            return new Dictionary<string, string>
            {
                ["%IMPORT"] = Import.ToString("F3"),
                ["%EXPORT"] = Export.ToString("F3"),

                ["%VOLTAGE_L1"] = VoltageL1.ToString("F1"),
                ["%VOLTAGE_L2"] = VoltageL2.ToString("F1"),
                ["%VOLTAGE_L3"] = VoltageL3.ToString("F1"),

                ["%CURRENT_L1"] = CurrentL1.ToString("F1"),
                ["%CURRENT_L2"] = CurrentL2.ToString("F1"),
                ["%CURRENT_L3"] = CurrentL3.ToString("F1"),

                ["%GAS"] = Gas.ToString("F3")
            };
        }
    }
}
