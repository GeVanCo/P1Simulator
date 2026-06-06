using System.IO.Ports;
using System.Management;

namespace P1Simulator.Serial
{
    public static class ComPortDetectorHybrid
    {
        // Known USB‑UART VID/PID pairs
        private static readonly (string vid, string pid)[] KnownAdapters =
        {
            ("VID_1A86", "PID_7523"), // CH340
            ("VID_10C4", "PID_EA60"), // CP210x
            ("VID_0403", "PID_6001"), // FTDI
            ("VID_067B", "PID_2303"), // Prolific PL2303
        };

        /// <summary>
        /// Main autodetect entry point.
        /// Tries loopback → VID/PID → friendly name → fallback.
        /// </summary>
        public static string? AutoDetect()
        {
            // 1. Loopback detection (best when TX/RX shorted)
            string? port = DetectByLoopback();
            if (port != null)
                return port;

            // 2. VID/PID detection (best when TX connected to ESP32)
            port = DetectByVidPid();
            if (port != null)
                return port;

            // 3. Friendly name detection
            port = DetectByFriendlyName();
            if (port != null)
                return port;

            // 4. Fallback: first available COM port
            return SerialPort.GetPortNames().FirstOrDefault();
        }

        // --------------------------------------------------------------------
        // LOOPBACK DETECTION
        // --------------------------------------------------------------------
        private static string? DetectByLoopback()
        {
            foreach (string portName in SerialPort.GetPortNames())
            {
                if (TestLoopback(portName))
                    return portName;
            }
            return null;
        }

        private static bool TestLoopback(string portName)
        {
            try
            {
                using var port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One)
                {
                    ReadTimeout = 50,
                    WriteTimeout = 50
                };

                port.Open();

                byte testByte = 0x55;
                port.DiscardInBuffer();
                port.Write(new[] { testByte }, 0, 1);

                Thread.Sleep(20);

                if (port.BytesToRead > 0)
                {
                    int received = port.ReadByte();
                    return received == testByte;
                }
            }
            catch
            {
                // Ignore and try next port
            }

            return false;
        }

        // --------------------------------------------------------------------
        // VID/PID DETECTION
        // --------------------------------------------------------------------
        private static string? DetectByVidPid()
        {
#pragma warning disable CA1416 // Validate platform compatibility
            using var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'");
#pragma warning restore CA1416 // Validate platform compatibility

#pragma warning disable CA1416 // Validate platform compatibility
            foreach (var device in searcher.Get())
            {
#pragma warning disable CA1416 // Validate platform compatibility
                string? name = device["Name"]?.ToString();
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
                string? deviceId = device["DeviceID"]?.ToString();
#pragma warning restore CA1416 // Validate platform compatibility

                if (name == null || deviceId == null)
                    continue;

                foreach (var (vid, pid) in KnownAdapters)
                {
                    if (deviceId.Contains(vid) && deviceId.Contains(pid))
                        return ExtractComPort(name);
                }
            }
#pragma warning restore CA1416 // Validate platform compatibility

            return null;
        }

        // --------------------------------------------------------------------
        // FRIENDLY NAME DETECTION
        // --------------------------------------------------------------------
        private static string? DetectByFriendlyName()
        {
            foreach (string port in SerialPort.GetPortNames())
            {
#pragma warning disable CA1416 // Validate platform compatibility
                using var searcher = new ManagementObjectSearcher(
                    $"SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%{port}%'");
#pragma warning restore CA1416 // Validate platform compatibility

#pragma warning disable CA1416 // Validate platform compatibility
                foreach (var device in searcher.Get())
                {
#pragma warning disable CA1416 // Validate platform compatibility
                    string? name = device["Name"]?.ToString();
#pragma warning restore CA1416 // Validate platform compatibility
                    if (name == null)
                        continue;

                    if (name.Contains("USB", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("UART", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("CH340", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("CP210", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("FTDI", StringComparison.OrdinalIgnoreCase))
                    {
                        return port;
                    }
                }
#pragma warning restore CA1416 // Validate platform compatibility
            }

            return null;
        }

        // --------------------------------------------------------------------
        // HELPERS
        // --------------------------------------------------------------------
        private static string? ExtractComPort(string name)
        {
            int start = name.IndexOf("(COM", StringComparison.OrdinalIgnoreCase);
            if (start < 0)
                return null;

            int end = name.IndexOf(")", start);
            if (end < 0)
                return null;

            return name.Substring(start + 1, end - start - 1);
        }
    }
}
