using Microsoft.AspNetCore.Mvc;
using Project.Services;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        private readonly ISensorDataReader _sensorDataReader;

        public SensorController(ISensorDataReader sensorDataReader)
        {
            _sensorDataReader = sensorDataReader;
        }

        [HttpGet("gettemp")]
        public async Task<ActionResult<string>> GetTemperature()
        {
            try
            {
                string temperatureLine = await _sensorDataReader.ReadLineAsync();

                return Ok(temperatureLine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Encountered error while reading temperature: {ex.Message}");
            }
        }

        [HttpGet("gethumidity")]
        public async Task<ActionResult<string>> GetHumidity()
        {
            try
            {
                string humidityLine = await _sensorDataReader.ReadLineAsync();

                return Ok(humidityLine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Encountered error while reading humidity: {ex.Message}");
            }

        }
    }
    }
