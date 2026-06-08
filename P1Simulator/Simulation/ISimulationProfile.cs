using System.Collections.Generic;

namespace P1Simulator.Simulation
{
    /// <summary>
    /// Defines a simulation profile that generates placeholder values
    /// for DSMR telegram templates (e.g. %IMPORT, %VOLTAGE_L1, etc.).
    /// </summary>
    public interface ISimulationProfile
    {
        /// <summary>
        /// Unique name of the profile (e.g. "fixed", "random", "solar", "ev").
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Generates a dictionary of placeholder → value pairs.
        /// Example: { "%IMPORT": "1234.567", "%VOLTAGE_L1": "230.1" }
        /// </summary>
        Dictionary<string, string> GenerateValues();
    }
}
