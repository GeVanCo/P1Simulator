namespace P1Simulator.Telegrams
{
    /// <summary>
    /// Base class for DSMR telegram templates using placeholder replacement.
    /// </summary>
    public abstract class TemplateBase
    {
        /// <summary>
        /// Unique name of the template (e.g. "1phase", "3phase", "gas").
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Raw DSMR telegram text containing placeholders like %IMPORT, %VOLTAGE_L1, etc.
        /// Must end with '!' because CRC is calculated up to and including '!'.
        /// </summary>
        public abstract string RawTemplate { get; }

        /// <summary>
        /// Replaces placeholders in the template with actual values.
        /// </summary>
        public string ApplyPlaceholders(Dictionary<string, string> values)
        {
            string output = RawTemplate;

            foreach (var kv in values)
            {
                output = output.Replace(kv.Key, kv.Value);
            }

            return output;
        }
    }
}
