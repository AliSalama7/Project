using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.SensorsData;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("AddRoom")]
        public IActionResult AddRoom(string RoomName)
        {
            if (!string.IsNullOrEmpty(RoomName))
            {
                var room = new Room
                {
                    RoomName = RoomName
                };
                _context.Add(room);
                _context.SaveChanges();
                return Ok("Room has been added successfully");
            }
            else
                return StatusCode(500, "Something Wrong");
        }
        [HttpPost("AddDevice")]
        public IActionResult AddDevice(string DeviceName , string? DevicePhoto)
        {
            if (!string.IsNullOrEmpty(DeviceName) && string.IsNullOrEmpty(DevicePhoto))
            {
                var device = new Device
                {
                    DeviceName = DeviceName
                };
                _context.Add(device);
                _context.SaveChanges();
                return Ok("Device has been added successfully");
            }
            else if(!string.IsNullOrEmpty(DeviceName) && !string.IsNullOrEmpty(DevicePhoto))
            {
                var device = new Device
                {
                    DeviceName = DeviceName,
                    DevicePhoto = DevicePhoto
                };
                _context.Add(device);
                _context.SaveChanges();
                return Ok("Device has been added successfully");
            }
            else
                return StatusCode(500, "Something Wrong");
        }
        [HttpPost("AddDevicetoroom")]
        public  IActionResult AddDeviceToRoom([FromBody] AddDeviceToRoomDto model)
        {
            var room =  _context.Rooms.FirstOrDefault(r => r.RoomName == model.RoomName);
            if (room == null)
            {
                return NotFound("Room not found.");
            }

            var device =  _context.Devices.FirstOrDefault(d => d.DeviceName == model.DeviceName);
            if (device == null)
            {
                return NotFound("Device not found.");
            }
            var roomDevice = _context.RoomDevices.FirstOrDefault(rd => rd.RoomId == room.RoomId && rd.DeviceId == device.DeviceId);
            if (roomDevice != null)
            {
                return Conflict("Device is already added to the room.");
            }

             roomDevice = new RoomDevice
            {
                RoomId = room.RoomId,
                DeviceId = device.DeviceId
            };

            _context.RoomDevices.Add(roomDevice);
             _context.SaveChanges();

            return Ok("Device added to room successfully.");
        }
    }
}
