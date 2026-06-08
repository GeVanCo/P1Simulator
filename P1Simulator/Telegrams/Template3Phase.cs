using System.Collections.Generic;

namespace P1Simulator.Telegrams
{
    /// <summary>
    /// DSMR 5.x three‑phase telegram template using placeholder replacement.
    /// Includes L1, L2, L3 voltage and current OBIS codes.
    /// </summary>
    public class Template3Phase : TemplateBase
    {
        public override string Name => "3phase";

        /// <summary>
        /// Raw DSMR telegram with placeholders.
        /// Must end with '!' because CRC is calculated up to and including '!'.
        /// </summary>
        public override string RawTemplate =>
@"1-0:1.8.0(%IMPORT*kWh)
1-0:2.8.0(%EXPORT*kWh)
1-0:32.7.0(%VOLTAGE_L1*V)
1-0:52.7.0(%VOLTAGE_L2*V)
1-0:72.7.0(%VOLTAGE_L3*V)
1-0:31.7.0(%CURRENT_L1*A)
1-0:51.7.0(%CURRENT_L2*A)
1-0:71.7.0(%CURRENT_L3*A)
!";
    }
}
