using P1Simulator.Simulation;

namespace P1Simulator.Simulation
{
    public class Xt211RandomProfile : ISimulationProfile
    {
        public string Name => "xt211random";

        public bool EnableElectricity => true;
        public bool EnableGas => false;
        public bool EnableWater => false;
        public bool EnableHeat => false;
        public bool EnableCapacityTariff => true;

        private readonly Random _rnd = new Random();

        public Dictionary<string, string> GenerateValues()
        {
            // -----------------------------
            // 1) Fixed XT211 metadata
            // -----------------------------
            string meterType = "50221";
            string firmware = "400";
            string equipmentId = "3153414733323030323438313337";

            // -----------------------------
            // 2) Timestamps
            // -----------------------------
            string timestamp = DateTime.Now.ToString("yyMMddHHmmss") + "S";

            // -----------------------------
            // 3) Energy totals (slowly rising)
            // -----------------------------
            double importLow = 600 + _rnd.NextDouble() * 200;
            double importHigh = 700 + _rnd.NextDouble() * 200;
            double exportLow = 1500 + _rnd.NextDouble() * 600;
            double exportHigh = 500 + _rnd.NextDouble() * 300;

            // -----------------------------
            // 4) Instantaneous power
            // -----------------------------
            double actualImport = Math.Max(0, _rnd.NextDouble() * 3.0); // 0–3 kW
            double actualExport = Math.Max(0, _rnd.NextDouble() * 3.0); // 0–3 kW

            // -----------------------------
            // 5) Per‑phase import/export
            // -----------------------------
            double l1Imp = actualImport * _rnd.NextDouble();
            double l2Imp = actualImport * _rnd.NextDouble();
            double l3Imp = actualImport * _rnd.NextDouble();

            double l1Exp = actualExport * _rnd.NextDouble();
            double l2Exp = actualExport * _rnd.NextDouble();
            double l3Exp = actualExport * _rnd.NextDouble();

            // -----------------------------
            // 6) Voltages (realistic)
            // -----------------------------
            double v1 = 225 + _rnd.NextDouble() * 15;
            double v2 = 225 + _rnd.NextDouble() * 15;
            double v3 = 225 + _rnd.NextDouble() * 15;

            // -----------------------------
            // 7) Currents (0–20 A)
            // -----------------------------
            double c1 = _rnd.NextDouble() * 20;
            double c2 = _rnd.NextDouble() * 20;
            double c3 = _rnd.NextDouble() * 20;

            // -----------------------------
            // 8) Peak demand (monthly)
            // -----------------------------
            double peakValue = 1.0 + _rnd.NextDouble() * 3.0; // 1–4 kW
            string peakTimestamp = DateTime.Now.AddDays(-_rnd.Next(1, 25))
                                               .ToString("yyMMddHHmmss") + "S";

            // -----------------------------
            // 9) Peak events block (13 events)
            // -----------------------------
            string peakEvents = Xt211PeakEventsBuilder.BuildRandom(13);

            // -----------------------------
            // 10) Tariff indicator
            // -----------------------------
            string tariff = _rnd.Next(0, 2) == 0 ? "0001" : "0002";

            // -----------------------------
            // 11) Breaker state
            // -----------------------------
            string breaker = _rnd.Next(0, 10) == 0 ? "0" : "1"; // 10% chance off

            // -----------------------------
            // 12) Max power/current (fixed)
            // -----------------------------
            string maxPower = "99.999";
            string maxCurrent = "999.99";

            // -----------------------------
            // Return dictionary
            // -----------------------------
            return new Dictionary<string, string>
            {
                ["{METER_TYPE}"] = meterType,
                ["{FIRMWARE_VERSION}"] = firmware,
                ["{EQUIPMENT_ID}"] = equipmentId,

                ["{TIMESTAMP}"] = timestamp,

                ["{IMPORT_LOW}"] = importLow.ToString("F3"),
                ["{IMPORT_HIGH}"] = importHigh.ToString("F3"),
                ["{EXPORT_LOW}"] = exportLow.ToString("F3"),
                ["{EXPORT_HIGH}"] = exportHigh.ToString("F3"),

                ["{TARIFF_INDICATOR}"] = tariff,

                ["{ACTUAL_POWER_L1}"] = actualImport.ToString("F3"),
                ["{PEAK_TIMESTAMP}"] = peakTimestamp,
                ["{PEAK_VALUE}"] = peakValue.ToString("F3"),

                ["{PEAK_EVENTS_COUNT}"] = "13",
                ["{PEAK_EVENTS}"] = peakEvents,

                ["{ACTUAL_IMPORT}"] = actualImport.ToString("F3"),
                ["{ACTUAL_EXPORT}"] = actualExport.ToString("F3"),

                ["{L1_IMPORT}"] = l1Imp.ToString("F3"),
                ["{L2_IMPORT}"] = l2Imp.ToString("F3"),
                ["{L3_IMPORT}"] = l3Imp.ToString("F3"),

                ["{L1_EXPORT}"] = l1Exp.ToString("F3"),
                ["{L2_EXPORT}"] = l2Exp.ToString("F3"),
                ["{L3_EXPORT}"] = l3Exp.ToString("F3"),

                ["{VOLTAGE_L1}"] = v1.ToString("F1"),
                ["{VOLTAGE_L2}"] = v2.ToString("F1"),
                ["{VOLTAGE_L3}"] = v3.ToString("F1"),

                ["{CURRENT_L1}"] = c1.ToString("F2"),
                ["{CURRENT_L2}"] = c2.ToString("F2"),
                ["{CURRENT_L3}"] = c3.ToString("F2"),

                ["{BREAKER_STATE}"] = breaker,
                ["{MAX_POWER}"] = maxPower,
                ["{MAX_CURRENT}"] = maxCurrent
            };
        }
    }
}
