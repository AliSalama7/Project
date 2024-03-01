using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.SensorsData
{
    public class SensorData_TempAndHumidity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SensorID { get; set; }
        public DateTime? Timestamp { get; set; }
        public double TempValue { get; set; }
        public double HumidityValue { get; set; }
    }
}
