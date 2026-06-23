using System.Text;
using P1Simulator.CRC;
using P1Simulator.Simulation;

namespace P1Simulator.Telegrams
{
    /// <summary>
    /// Generates DSMR telegrams by combining templates and simulation profiles.
    /// Handles placeholder replacement and DSMR CRC-16 calculation.
    /// </summary>
    public class TelegramGenerator
    {
        private readonly TemplateManager _templates;
        private readonly ProfileManager _profiles;

        /// <summary>
        /// When true, the CRC is intentionally corrupted for testing.
        /// </summary>
        public bool ForceBadCrc { get; set; } = false;
        public string CurrentTemplate { get; private set; } = "3phase";
        public string CurrentProfile { get; private set; } = "random";


        public TelegramGenerator(TemplateManager templates, ProfileManager profiles)
        {
            _templates = templates;
            _profiles = profiles;
        }
        public void SetTemplate(string name)
        {
            CurrentTemplate = name;
        }

        public void SetProfile(string name)
        {
            CurrentProfile = name;
        }

        public string Generate()
        {
            return Generate(CurrentTemplate, CurrentProfile);
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

            // ⭐ SPECIAL CASE: passthrough template + live profile
            if (templateName.Equals("passthrough", StringComparison.OrdinalIgnoreCase))
            {
                var life_values = profile.GenerateValues();

                if (!life_values.TryGetValue("{RAW_TELEGRAM}", out var raw))
                    return "ERROR: Live profile did not provide {RAW_TELEGRAM}.\r\n";

                // We assume raw already contains full telegram including CRC and line breaks.
                return raw.EndsWith("\r\n") ? raw : raw + "\r\n";
            }

            // 1) Generate placeholder values
            var values = profile.GenerateValues();

            // 2) Apply placeholders to template
            string body = TemplateProcessor.ApplyPlaceholders(template, values);

            // 3) Apply profile flags (remove OBIS blocks)
            body = ApplyProfileFlags(body, profile);

            // 4) Convert to bytes for CRC calculation
            byte[] bytes = Encoding.ASCII.GetBytes(body);

            // CRC must include everything up to and including '!'
            int exclPos = body.IndexOf('!');
            if (exclPos < 0)
                return "ERROR: Template missing '!'\r\n";

            ushort crc = DsmrCrc16.Compute(bytes, exclPos + 1);

            if (ForceBadCrc)
                crc ^= 0xFFFF; // flip bits to corrupt CRC

            // 5) Append CRC and CRLF
            string fullTelegram = body + crc.ToString("X4") + "\r\n";

            return fullTelegram;
        }

        private string ApplyProfileFlags(string body, ISimulationProfile profile)
        {
            if (!profile.EnableGas)
            {
                body = RemoveLinesContaining(body, "0-1:24.2.1");
            }

            if (!profile.EnableWater)
            {
                body = RemoveLinesContaining(body, "0-2:24.2.1");
            }

            if (!profile.EnableHeat)
            {
                body = RemoveLinesContaining(body, "0-3:24.2.1");
            }

            if (!profile.EnableCapacityTariff)
            {
                body = RemoveLinesContaining(body, "1-0:1.6.0");
            }

            if (!profile.EnableElectricity)
            {
                body = RemoveLinesContaining(body, "1-0:");
            }

            return body;
        }

        private string RemoveLinesContaining(string text, string pattern)
        {
            var lines = text.Split('\n');
            var filtered = lines.Where(l => !l.Contains(pattern));
            return string.Join("\n", filtered);
        }
    }
}
