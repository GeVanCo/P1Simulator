using P1Simulator.CRC;
using P1Simulator.Simulation;
using P1Simulator.Telegrams.Templates;

using System.Text;

namespace P1Simulator.Telegrams
{
    public class TelegramGenerator
    {
        private readonly Random _rnd = new();

        public string GenerateTelegram(SimulationProfile profile)
        {
            string telegram = GenerateBaseTelegram(); // your existing method

            return profile.Mode switch
            {
                SimulationMode.Normal => AddCrc(telegram),
                SimulationMode.BadCrc => AddBadCrc(telegram),
                SimulationMode.PartialTelegram => MakePartial(telegram),
                SimulationMode.Burst => MakeBurst(telegram),
                SimulationMode.Noise => AddNoise(telegram),
                _ => AddCrc(telegram),
            };

            //// Generate realistic values for import, voltage, and current
            //double import = Math.Round(_rnd.NextDouble() * 5.0, 3); // 0–5 kW
            //double voltage = Math.Round(230 + _rnd.NextDouble() * 5, 1);
            //double current = Math.Round(import * 1000 / voltage, 1);

            //var sb = new StringBuilder();

            //// Header
            //sb.AppendLine("/FLU5\\P1_SIMULATOR");
            //sb.AppendLine();

            //// OBIS values
            //sb.AppendLine($"1-0:1.7.0({import:0.000}*kW)");
            //sb.AppendLine($"1-0:32.7.0({voltage:0.0}*V)");
            //sb.AppendLine($"1-0:31.7.0({current:0.0}*A)");

            //// End marker (CRC will be added later)
            //sb.Append("!");  // No newline here for CRC calculation

            //string withoutCrc = sb.ToString();
            //string crcHex = Crc16X25.ComputeHex(withoutCrc);

            //// Final telegram with CRC on same line as '!'
            //string final = withoutCrc + crcHex + "\r\n";

            //return final;
        }

        private static string AddCrc(string telegram)
        {
            string crc = Crc16X25.ComputeHex(telegram);
            return telegram + crc + "\r\n";
        }

        private static string AddBadCrc(string telegram)
        {
            return telegram + "FFFF\r\n"; // guaranteed wrong
        }

        private static string MakePartial(string telegram)
        {
            int cut = telegram.Length / 2;
            return string.Concat(telegram.AsSpan(0, cut), "\r\n");
        }

        private static string MakeBurst(string telegram)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                sb.Append(AddCrc(telegram));
            }
            return sb.ToString();
        }

        private static string AddNoise(string telegram)
        {
            return telegram.Insert(5, "###NOISE###");
        }

        private string GenerateBaseTelegram()
        {
            return GenerateFromTemplate(TemplateType.ElectricitySinglePhase);
            //double import = Math.Round(_rnd.NextDouble() * 5.0, 3); // 0–5 kW
            //double voltage = Math.Round(230 + _rnd.NextDouble() * 5, 1);
            //double current = Math.Round(import * 1000 / voltage, 1);

            //var sb = new StringBuilder();

            //sb.AppendLine("/FLU5\\P1_SIMULATOR");
            //sb.AppendLine();
            //sb.AppendLine($"1-0:1.7.0({import:0.000}*kW)");
            //sb.AppendLine($"1-0:32.7.0({voltage:0.0}*V)");
            //sb.AppendLine($"1-0:31.7.0({current:0.0}*A)");
            //sb.Append('!'); // IMPORTANT: no newline, CRC will be appended here

            //return sb.ToString();
        }

        private string GenerateFromTemplate(TemplateType type)
        {
            string template = GetTemplate(type);

            string timestamp = DateTime.Now.ToString("yyMMddHHmmss");

            double importLow = Math.Round(_rnd.NextDouble() * 1000, 3);
            double importHigh = Math.Round(_rnd.NextDouble() * 1000, 3);
            double exportLow = Math.Round(_rnd.NextDouble() * 1000, 3);
            double exportHigh = Math.Round(_rnd.NextDouble() * 1000, 3);

            double pImport = Math.Round(_rnd.NextDouble() * 5.0, 3);
            double pExport = Math.Round(_rnd.NextDouble() * 2.0, 3);

            double v1 = Math.Round(230 + _rnd.NextDouble() * 5, 1);
            double v2 = Math.Round(230 + _rnd.NextDouble() * 5, 1);
            double v3 = Math.Round(230 + _rnd.NextDouble() * 5, 1);

            double c1 = Math.Round(pImport * 1000 / v1, 1);
            double c2 = Math.Round(pImport * 1000 / v2, 1);
            double c3 = Math.Round(pImport * 1000 / v3, 1);

            double gas = Math.Round(_rnd.NextDouble() * 500, 3);

            return template
                .Replace("{TIMESTAMP}", timestamp)
                .Replace("{IMPORT_LOW}", importLow.ToString("0.000"))
                .Replace("{IMPORT_HIGH}", importHigh.ToString("0.000"))
                .Replace("{EXPORT_LOW}", exportLow.ToString("0.000"))
                .Replace("{EXPORT_HIGH}", exportHigh.ToString("0.000"))
                .Replace("{POWER_IMPORT}", pImport.ToString("0.000"))
                .Replace("{POWER_EXPORT}", pExport.ToString("0.000"))
                .Replace("{VOLTAGE_L1}", v1.ToString("0.0"))
                .Replace("{VOLTAGE_L2}", v2.ToString("0.0"))
                .Replace("{VOLTAGE_L3}", v3.ToString("0.0"))
                .Replace("{CURRENT_L1}", c1.ToString("0.0"))
                .Replace("{CURRENT_L2}", c2.ToString("0.0"))
                .Replace("{CURRENT_L3}", c3.ToString("0.0"))
                .Replace("{GAS_M3}", gas.ToString("0.000"));
        }

        private string GetTemplate(TemplateType type)
        {
            return type switch
            {
                TemplateType.ElectricitySinglePhase => DsmrTemplates.ElectricitySinglePhase,
                TemplateType.ElectricityThreePhase => DsmrTemplates.ElectricityThreePhase,
                TemplateType.Gas => DsmrTemplates.Gas,
                TemplateType.Minimal => DsmrTemplates.Minimal,
                _ => DsmrTemplates.ElectricitySinglePhase
            };
        }

    }
}
