using System.Text;

namespace P1Simulator.Simulation
{
    public static class Xt211PeakEventsBuilder
    {
        // ⭐ EXACT COPY of your real XT211 peak events
        private static readonly string[] FixedEvents =
        {
            "(1-0:1.6.0)",
            "(1-0:1.6.0)",
            "(250601000000S)",
            "(250511083000S)",
            "(02.017*kW)",
            "(250701000000S)",
            "(250614131500S)",
            "(01.683*kW)",
            "(250801000000S)",
            "(250731141500S)",
            "(01.879*kW)",
            "(250901000000S)",
            "(250829134500S)",
            "(02.193*kW)",
            "(251001000000S)",
            "(250920130000S)",
            "(01.990*kW)",
            "(251101000000W)",
            "(251019091500S)",
            "(02.373*kW)",
            "(251201000000W)",
            "(251122173000W)",
            "(02.409*kW)",
            "(260101000000W)",
            "(251205174500W)",
            "(02.341*kW)",
            "(260201000000W)",
            "(260116191500W)",
            "(02.520*kW)",
            "(260301000000W)",
            "(260224203000W)",
            "(02.427*kW)",
            "(260401000000S)",
            "(260307174500W)",
            "(02.223*kW)",
            "(260501000000S)",
            "(260403101500S)",
            "(01.966*kW)",
            "(260601000000S)",
            "(260515101500S)",
            "(02.033*kW)"
        };

        public static string BuildFixed()
        {
            return string.Join("", FixedEvents);
        }

        public static string BuildRandom(int count)
        {
            var rnd = new Random();
            var sb = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                // Random timestamp in last 180 days
                DateTime ts = DateTime.Now.AddDays(-rnd.Next(1, 180))
                                           .AddMinutes(rnd.Next(0, 1440));

                string tsStr = ts.ToString("yyMMddHHmmss") + (rnd.Next(0, 2) == 0 ? "S" : "W");

                // Random peak value 1–4 kW
                double peak = 1.0 + rnd.NextDouble() * 3.0;

                sb.Append($"({tsStr})({peak:F3}*kW)");
            }

            return sb.ToString();
        }
    }
}
