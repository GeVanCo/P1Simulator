namespace P1Simulator.Telegrams
{
    /// <summary>
    /// DSMR gas meter template (0-1:24.2.1) using placeholder replacement.
    /// </summary>
    public class TemplateGas : TemplateBase
    {
        public override string Name => "gas";

        /// <summary>
        /// Raw DSMR gas telegram with placeholder %GAS.
        /// Must end with '!' because CRC is calculated up to and including '!'.
        /// </summary>
        public override string RawTemplate =>
@"0-1:24.2.1(%GAS*m3)
!";
    }
}
