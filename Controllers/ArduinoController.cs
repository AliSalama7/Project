using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Project.Services;
using System.IO.Ports;
using System.IO;
using System.Composition;
using System.Text;
using Project.Models;
using Project.SensorsData;
using System.Net.Sockets;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArduinoController : ControllerBase
    {
        private readonly SerialPort _serialPort;
        private readonly ApplicationDbContext _context;


        public ArduinoController(ApplicationDbContext context)
        {
            _context = context;  
            _serialPort = new SerialPort("COM3", 9600);
            _serialPort.Open();
        }
    
        private string ReadLineFromSerialPort()
        {
             byte[] buffer = new byte[256];
             int bytesRead =  _serialPort.BaseStream.Read(buffer, 0, buffer.Length);
             return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        [HttpGet("data")]
        public IActionResult GetTempAndHumidity()
        {
            try
            {
                string data = ReadLineFromSerialPort();
                if (!string.IsNullOrEmpty(data))
                {
                    var sensorData = new SensorData_TempAndHumidity
                    {
                        Timestamp = DateTime.Now,
                        SensorValue = data
                    };
                    _context.Add(sensorData);
                    _context.SaveChanges();
                    _serialPort.Close();
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
            _serialPort.Write("1");
            _serialPort.Close();
            return Ok("LED is on");
        }

        [HttpPost("led/off")]
        public IActionResult TurnLedOff()
        {
            _serialPort.Write("0");
            _serialPort.Close();
            return Ok("LED is off");
        }
    }
    }
