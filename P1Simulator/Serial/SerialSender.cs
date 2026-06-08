using P1Simulator.Logging;
using System.IO.Ports;

namespace P1Simulator.Serial
{
    public class SerialSender : IDisposable
    {
        private readonly SerialPort _port;
        private readonly Logger _logger;

        public bool IsOpen => _port?.IsOpen ?? false;

        public SerialSender(Logger logger, string portName = "COM3", int baudRate = 115200)
        {
            _logger = logger;

            _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
            {
                NewLine = "\n",
                Encoding = System.Text.Encoding.ASCII,
                WriteTimeout = 500
            };
        }

        public int BaudRate => _port.BaudRate;

        public void Open()
        {
            if (_port.IsOpen)
                return;

            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Serial] ERROR opening port: {ex.Message}");
                _logger.Error($"Serial open error: {ex.Message}");
            }
        }

        public void Send(string data)
        {
            if (_port == null || !_port.IsOpen)
                throw new InvalidOperationException("Serial port not open.");

            _port.Write(data);

            _logger.WriteTelegram(data);
        }

        public void Dispose()
        {
            if (_port.IsOpen)
                _port.Close();

            _port.Dispose();
        }
    }
}
