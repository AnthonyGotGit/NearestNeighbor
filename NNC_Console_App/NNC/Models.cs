namespace NNC
{
    public class VehicleKdTreeNode
    {
        public int VehicleId;
        public string VehicleRegistration;
        public float Latitude;
        public float Longitude;
        public ulong RecordedTimeUTC;
        public VehicleKdTreeNode Left;
        public VehicleKdTreeNode Right;
    }

    public class TargetCoordinate
    {
        public int Position;
        public float Latitude;
        public float Longitude;
    }
}