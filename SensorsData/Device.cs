using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.SensorsData
{
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string? DevicePhoto { get; set; }
        public bool Status { get; set; }
        public ICollection<RoomDevice> RoomDevices { get; set;}
    }
}
