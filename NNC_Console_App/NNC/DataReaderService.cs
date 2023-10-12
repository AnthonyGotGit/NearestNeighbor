using System.Text;

namespace NNC
{
    internal class DataReaderService
    {
        public static List<VehicleKdTreeNode> ReadBinaryData(string dataFilePath)
        {
            List<VehicleKdTreeNode> vehicleList = new List<VehicleKdTreeNode>();
            try
            {
                using (FileStream fs = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        while (fs.Position < fs.Length)
                        {
                            var vehicleData = new VehicleKdTreeNode();
                            vehicleData.VehicleId = reader.ReadInt32();
                            vehicleData.VehicleRegistration = ReadNullTerminatedString(reader);
                            vehicleData.Latitude = reader.ReadSingle();
                            vehicleData.Longitude = reader.ReadSingle();
                            vehicleData.RecordedTimeUTC = reader.ReadUInt64();

                            vehicleList.Add(vehicleData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            return vehicleList;
        }            

        private static string ReadNullTerminatedString(BinaryReader reader)
        {
            StringBuilder registrationBuilder = new StringBuilder();
            char nextChar;
            while ((nextChar = reader.ReadChar()) != '\0')
            {
                registrationBuilder.Append(nextChar);
            }
            return registrationBuilder.ToString();
        }
    }
}
