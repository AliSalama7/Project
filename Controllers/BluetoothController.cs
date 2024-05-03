using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.SensorsData;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BluetoothController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _bluetoothDeviceAddress;
        private readonly string _bluetoothDevicePin;

        public BluetoothController(ApplicationDbContext context)
        {
            _bluetoothDeviceAddress = "00:23:05:00:99:F5"; 
            _bluetoothDevicePin = "1234"; 
            _context = context;
        }
         private string ReadDataFromBluetooth()
        {
            try
            {
                BluetoothAddress address = BluetoothAddress.Parse(_bluetoothDeviceAddress);
                BluetoothSecurity.PairRequest(address, _bluetoothDevicePin);

                using (BluetoothClient client = new BluetoothClient())
                {
                    BluetoothEndPoint endPoint = new BluetoothEndPoint(address, BluetoothService.SerialPort);
                    client.Connect(endPoint);

                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[256];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    return Encoding.ASCII.GetString(buffer, 0, bytesRead);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading data from Bluetooth module: {ex.Message}");
            }
        }

        [HttpGet("data")]
        public IActionResult GetTempAndHumidity()
        {
            try
            {
                string data = ReadDataFromBluetooth();

                if (!string.IsNullOrEmpty(data))
                {
                    var sensorData = new SensorData_TempAndHumidity
                    {
                        Timestamp = DateTime.Now,
                        SensorValue = data
                    };
                    _context.Add(sensorData);
                    _context.SaveChanges();
                    return Ok($"Received data from sensor: {data}");
                }
                else
                {
                    return StatusCode(500, "No data received from sensor");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error reading sensor data: {ex.Message}");
            }
        }

        [HttpPost("led/on")]
        public IActionResult TurnLedOn()
        {
            try
            {
                BluetoothAddress address = BluetoothAddress.Parse(_bluetoothDeviceAddress);
                BluetoothSecurity.PairRequest(address, _bluetoothDevicePin); 

                using (BluetoothClient client = new BluetoothClient())
                {
                    BluetoothEndPoint endPoint = new BluetoothEndPoint(address, BluetoothService.SerialPort);
                    client.Connect(endPoint);

                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.ASCII.GetBytes("1"); 
                    stream.Write(data, 0, data.Length);
                }

                return Ok("LED is on");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error turning LED on: {ex.Message}");
            }
        }

        [HttpPost("led/off")]
        public IActionResult TurnLedOff()
        {
            try
            {
                BluetoothAddress address = BluetoothAddress.Parse(_bluetoothDeviceAddress);
                BluetoothSecurity.PairRequest(address, _bluetoothDevicePin); 

                using (BluetoothClient client = new BluetoothClient())
                {
                    BluetoothEndPoint endPoint = new BluetoothEndPoint(address, BluetoothService.SerialPort);
                    client.Connect(endPoint);

                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.ASCII.GetBytes("0"); 
                    stream.Write(data, 0, data.Length);
                }

                return Ok("LED is off");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error turning LED off: {ex.Message}");
            }
        }
    }
}
