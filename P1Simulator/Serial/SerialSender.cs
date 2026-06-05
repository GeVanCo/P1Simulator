using P1Simulator.Logging;
using System.IO.Ports;

namespace P1Simulator.Serial
{
    public class SerialSender : IDisposable
    {
        private readonly SerialPort _port;

        public bool IsOpen => _port?.IsOpen ?? false;

        public SerialSender(string portName, int baudRate = 115200)
        {
            _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
            {
                NewLine = "\n",
                Encoding = System.Text.Encoding.ASCII,
                WriteTimeout = 500
            };
        }

        public void Open()
        {
            Console.WriteLine($"[Serial] Open() called");
            if (_port.IsOpen)
                return;

            try
            {
                _port.Open();
                Console.WriteLine($"[Serial] Opened {_port.PortName} at {_port.BaudRate} baud");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Serial] ERROR opening port: {ex.Message}");
            }
        }

        public void Send(string data)
        {
            if (_port == null || !_port.IsOpen)
            {
                throw new InvalidOperationException("Serial port not open.");
            }

            _port.Write(data);

            Logger.WriteTelegram(data);
        }

        public void Dispose()
        {
            if (_port.IsOpen)
                _port.Close();

            _port.Dispose();
        }
    }
}
