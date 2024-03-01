using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.SensorsData
{
    public class SensorData_WaterLevel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SensorID { get; set; }
        public DateTime? Timestamp { get; set; }
        public int WaterLevel { get; set; }
    }
}
