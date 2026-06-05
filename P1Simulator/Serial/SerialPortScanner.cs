using System.IO.Ports;

namespace P1Simulator.Serial
{
    public static class SerialPortScanner
    {
        public static List<string> GetAvailablePorts()
        {
            // Get all COM ports from Windows
            var ports = SerialPort.GetPortNames()
                                  .OrderBy(p => p)
                                  .ToList();

            return ports;
        }

        public static string? AutoDetectEsp32()
        {
            var ports = GetAvailablePorts();

            // Heuristic: ESP32-C3 USB-CDC ports often appear as COMx with no description
            // or with "USB Serial Device" in Windows Device Manager.
            foreach (var port in ports)
            {
                Console.WriteLine($"Checking port {port} for ESP32-C3...");
                if (IsLikelyEsp32(port))
                    return port;
            }

            return null;
        }

        private static bool IsLikelyEsp32(string portName)
        {
            // Simple heuristic: ESP32-C3 often enumerates as COMx with no special ID.
            // We can try opening it to see if it's responsive.
            try
            {
                using var sp = new SerialPort(portName, 115200);
                sp.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

