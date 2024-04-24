using System.IO.Ports;
using System.Text;

namespace Project.Services
{
    public class SensorDataReader : ISensorDataReader
    {
        private readonly SerialPort _port;

        public SensorDataReader(string portName, int baudRate)
        {
            _port = new SerialPort();
            _port.BaudRate = baudRate;
            _port.PortName = portName;
        }

        public async Task<string> ReadLineAsync()
        {
            _port.Open();
            try
            {
                byte[] buffer = new byte[256];
                int bytesRead = await _port.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                return Encoding.ASCII.GetString(buffer, 0, bytesRead);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encountered error while reading from serial port: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _port?.Dispose();
        }

    }
}
