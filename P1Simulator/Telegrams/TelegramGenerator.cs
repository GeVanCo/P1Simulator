using System.Text;
using P1Simulator.CRC;
using P1Simulator.Simulation;

namespace P1Simulator.Telegrams
{
    /// <summary>
    /// Generates DSMR telegrams by combining templates and simulation profiles.
    /// Handles placeholder replacement and CRC-16/X25 calculation.
    /// </summary>
    public class TelegramGenerator
    {
        private readonly TemplateManager _templates;
        private readonly ProfileManager _profiles;

        /// <summary>
        /// When true, the CRC is intentionally corrupted for testing.
        /// </summary>
        public bool ForceBadCrc { get; set; } = false;

        public TelegramGenerator(TemplateManager templates, ProfileManager profiles)
        {
            _templates = templates;
            _profiles = profiles;
        }

        /// <summary>
        /// Generates a complete DSMR telegram including CRC and CRLF.
        /// </summary>
        public string Generate(string templateName, string profileName)
        {
            var template = _templates.Get(templateName);
            var profile = _profiles.Get(profileName);

            if (template == null)
                return $"ERROR: Template '{templateName}' not found.\r\n";

            if (profile == null)
                return $"ERROR: Profile '{profileName}' not found.\r\n";

            // 1) Generate placeholder values
            var values = profile.GenerateValues();

            // 2) Apply placeholders to template
            string body = TemplateProcessor.ApplyPlaceholders(template, values);

            // 3) Convert to bytes for CRC calculation
            byte[] bytes = Encoding.ASCII.GetBytes(body);

            // CRC must include everything up to and including '!'
            int exclPos = body.IndexOf('!');
            if (exclPos < 0)
                return "ERROR: Template missing '!'\r\n";

            ushort crc = DsmrCrc16.Compute(bytes, exclPos + 1);

            if (ForceBadCrc)
                crc ^= 0xFFFF; // flip bits to corrupt CRC

            // 4) Append CRC and CRLF
            string fullTelegram = body + crc.ToString("X4") + "\r\n";

            return fullTelegram;
        }
    }
}
