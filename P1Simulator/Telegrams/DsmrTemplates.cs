
namespace P1Simulator.Telegrams
{
    public static class DsmrTemplates
    {
        // Note: All strings are verbatim string literals (using @) to preserve formatting and line breaks exactly as intended.
        // This means we don't have to use \\, we can use \ instead.

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
1-0:32.7.0({VOLTAGE_L1}*V)
1-0:31.7.0({CURRENT_L1}*A)
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


        // --------------------------------------------------------------------
        // Water
        // --------------------------------------------------------------------
        public const string Water =
@"/FLU5\253769484_A

1-3:0.2.8(50)
0-0:1.0.0({TIMESTAMP})
0-0:96.1.1(4530303435303030303030303030303136)

0-2:24.2.1({WATER_TIMESTAMP})({WATER_M3}*m3)
!";


        // --------------------------------------------------------------------
        // Heating (similar to water, but with different OBIS code)
        // --------------------------------------------------------------------
        public const string Heat =
@"/FLU5\253769484_A

1-3:0.2.8(50)
0-0:1.0.0({TIMESTAMP})
0-0:96.1.1(4530303435303030303030303030303136)

0-3:24.2.1({HEAT_TIMESTAMP})({HEAT_GJ}*GJ)
!";


        // --------------------------------------------------------------------
        // Full DSMR with Electricity, Gas, Water, Heat, and Capacity Tariff
        // --------------------------------------------------------------------
        public const string FullDsmrExtended =
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
0-2:24.2.1({WATER_TIMESTAMP})({WATER_M3}*m3)
0-3:24.2.1({HEAT_TIMESTAMP})({HEAT_GJ}*GJ)

1-0:1.6.0({MONTHLY_PEAK}*kW)

!";

        // --------------------------------------------------------------------
        // XT211 — Fluvius Sagemcom Digital Meter (authentic structure)
        // --------------------------------------------------------------------
        public const string Xt211 =
        @"/FLU5\253967035_D

0-0:96.1.4({METER_TYPE})
1-0:94.32.1({FIRMWARE_VERSION})
0-0:96.1.1({EQUIPMENT_ID})
0-0:96.1.2()
0-0:1.0.0({TIMESTAMP})
1-0:1.8.1({IMPORT_LOW}*kWh)
1-0:1.8.2({IMPORT_HIGH}*kWh)
1-0:2.8.1({EXPORT_LOW}*kWh)
1-0:2.8.2({EXPORT_HIGH}*kWh)
0-0:96.14.0({TARIFF_INDICATOR})
1-0:1.4.0({ACTUAL_POWER_L1}*kW)
1-0:1.6.0({PEAK_TIMESTAMP})({PEAK_VALUE}*kW)
0-0:98.1.0({PEAK_EVENTS_COUNT}){PEAK_EVENTS}
1-0:1.7.0({ACTUAL_IMPORT}*kW)
1-0:2.7.0({ACTUAL_EXPORT}*kW)
1-0:21.7.0({L1_IMPORT}*kW)
1-0:41.7.0({L2_IMPORT}*kW)
1-0:61.7.0({L3_IMPORT}*kW)
1-0:22.7.0({L1_EXPORT}*kW)
1-0:42.7.0({L2_EXPORT}*kW)
1-0:62.7.0({L3_EXPORT}*kW)
1-0:32.7.0({VOLTAGE_L1}*V)
1-0:52.7.0({VOLTAGE_L2}*V)
1-0:72.7.0({VOLTAGE_L3}*V)
1-0:31.7.0({CURRENT_L1}*A)
1-0:51.7.0({CURRENT_L2}*A)
1-0:71.7.0({CURRENT_L3}*A)
0-0:96.3.10({BREAKER_STATE})
0-0:17.0.0({MAX_POWER}*kW)
1-0:31.4.0({MAX_CURRENT}*A)
0-1:96.3.10(0)
0-2:96.3.10(0)
0-3:96.3.10(0)
0-4:96.3.10(0)
0-0:96.13.0()
!";

        public const string Passthrough =
@"{RAW_TELEGRAM}";

    }
}
