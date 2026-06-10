namespace P1Simulator.Telegrams
{
    public static class DsmrTemplates
    {
        // --------------------------------------------------------------------
        // BASIC TEMPLATE (legacy)
        // --------------------------------------------------------------------
        public const string Basic =
@"/FLU5\253769484_A

1-3:0.2.8(50)
0-0:1.0.0({TIMESTAMP})
0-0:96.1.1(4530303435303030303030303030303136)
1-0:1.8.1({IMPORT_LOW}*kWh)
1-0:1.8.2({IMPORT_HIGH}*kWh)
1-0:2.8.1({EXPORT_LOW}*kWh)
1-0:2.8.2({EXPORT_HIGH}*kWh)
1-0:1.7.0({POWER_IMPORT}*kW)
1-0:2.7.0({POWER_EXPORT}*kW)
1-0:32.7.0({VOLTAGE}*V)
1-0:31.7.0({CURRENT}*A)
!";


        // --------------------------------------------------------------------
        // ELECTRICITY — SINGLE PHASE
        // --------------------------------------------------------------------
        public const string ElectricitySinglePhase =
@"/FLU5\253769484_A

1-3:0.2.8(50)
0-0:1.0.0({TIMESTAMP})
0-0:96.1.1(4530303435303030303030303030303136)
1-0:1.8.1({IMPORT_LOW}*kWh)
1-0:1.8.2({IMPORT_HIGH}*kWh)
1-0:2.8.1({EXPORT_LOW}*kWh)
1-0:2.8.2({EXPORT_HIGH}*kWh)
1-0:1.7.0({POWER_IMPORT}*kW)
1-0:2.7.0({POWER_EXPORT}*kW)
1-0:32.7.0({VOLTAGE_L1}*V)
1-0:31.7.0({CURRENT_L1}*A)
!";


        // --------------------------------------------------------------------
        // ELECTRICITY — THREE PHASE
        // --------------------------------------------------------------------
        public const string ElectricityThreePhase =
@"/FLU5\253769484_A

1-3:0.2.8(50)
0-0:1.0.0({TIMESTAMP})
0-0:96.1.1(4530303435303030303030303030303136)
1-0:1.8.1({IMPORT_LOW}*kWh)
1-0:1.8.2({IMPORT_HIGH}*kWh)
1-0:2.8.1({EXPORT_LOW}*kWh)
1-0:2.8.2({EXPORT_HIGH}*kWh)
1-0:1.7.0({POWER_IMPORT}*kW)
1-0:2.7.0({POWER_EXPORT}*kW)
1-0:32.7.0({VOLTAGE_L1}*V)
1-0:52.7.0({VOLTAGE_L2}*V)
1-0:72.7.0({VOLTAGE_L3}*V)
1-0:31.7.0({CURRENT_L1}*A)
1-0:51.7.0({CURRENT_L2}*A)
1-0:71.7.0({CURRENT_L3}*A)
!";


        // --------------------------------------------------------------------
        // GAS
        // --------------------------------------------------------------------
        public const string Gas =
@"/FLU5\253769484_A

1-3:0.2.8(50)
0-0:1.0.0({TIMESTAMP})
0-0:96.1.1(4530303435303030303030303030303136)
0-1:24.2.1({GAS_TIMESTAMP})({GAS_M3}*m3)
!";


        // --------------------------------------------------------------------
        // CAPACITY TARIFF (MONTHLY PEAK DEMAND)
        // --------------------------------------------------------------------
        public const string CapacityTariff =
@"/FLU5\253769484_A

1-3:0.2.8(50)
0-0:1.0.0({TIMESTAMP})
1-0:1.6.0({MONTHLY_PEAK}*kW)
!";


        // --------------------------------------------------------------------
        // MINIMAL
        // --------------------------------------------------------------------
        public const string Minimal =
@"/FLU5\TEST

1-3:0.2.8(50)
0-0:1.0.0({TIMESTAMP})
1-0:1.7.0({POWER_IMPORT}*kW)
!";


        // --------------------------------------------------------------------
        // FULL DSMR (ALL COMMON OBIS CODES)
        // Note: This is a comprehensive template that includes many common OBIS codes.
        //       It can be used for testing or as a base for custom templates.
        // --------------------------------------------------------------------
        public const string FullDsmr =
@"/FLU5\253769484_A

1-3:0.2.8(50)
0-0:1.0.0({TIMESTAMP})
0-0:96.1.1(4530303435303030303030303030303136)

1-0:1.8.1({IMPORT_LOW}*kWh)
1-0:1.8.2({IMPORT_HIGH}*kWh)
1-0:2.8.1({EXPORT_LOW}*kWh)
1-0:2.8.2({EXPORT_HIGH}*kWh)

1-0:1.7.0({POWER_IMPORT}*kW)
1-0:2.7.0({POWER_EXPORT}*kW)

1-0:32.7.0({VOLTAGE_L1}*V)
1-0:52.7.0({VOLTAGE_L2}*V)
1-0:72.7.0({VOLTAGE_L3}*V)

1-0:31.7.0({CURRENT_L1}*A)
1-0:51.7.0({CURRENT_L2}*A)
1-0:71.7.0({CURRENT_L3}*A)

0-1:24.2.1({GAS_TIMESTAMP})({GAS_M3}*m3)

1-0:1.6.0({MONTHLY_PEAK}*kW)

!";

    }
}
