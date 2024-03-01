namespace Project.SensorsData
{
    public class RoomDevice
    {
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public int DeviceId {  get; set; }
        public Device Device { get; set; }
    }
}
