using System.Collections.Generic;

namespace P1Simulator.Telegrams
{
    /// <summary>
    /// DSMR 5.x single‑phase telegram template using placeholder replacement.
    /// </summary>
    public class Template1Phase : TemplateBase
    {
        public override string Name => "1phase";

        /// <summary>
        /// Raw DSMR telegram with placeholders.
        /// Must end with '!' because CRC is calculated up to and including '!'.
        /// </summary>
        public override string RawTemplate =>
@"1-0:1.8.0(%IMPORT*kWh)
1-0:2.8.0(%EXPORT*kWh)
1-0:32.7.0(%VOLTAGE_L1*V)
1-0:31.7.0(%CURRENT_L1*A)
!";
    }
}
