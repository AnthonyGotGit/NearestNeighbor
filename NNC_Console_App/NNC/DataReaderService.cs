using System.Collections.Concurrent;
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

                            // Read and populate the null-terminated ASCII string
                            StringBuilder registrationBuilder = new StringBuilder();
                            char nextChar;
                            while ((nextChar = reader.ReadChar()) != '\0')
                            {
                                registrationBuilder.Append(nextChar);
                            }
                            vehicleData.VehicleRegistration = registrationBuilder.ToString();

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

            private static void ReadChunk(ConcurrentBag<VehicleKdTreeNode> data, FileStream fs, long start, long end)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(fs, Encoding.ASCII, true))
                {
                    while (reader.PeekChar() != -1)
                    {

                        fs.Seek(start, SeekOrigin.Begin);

                        if (end > fs.Length)
                        {
                            end = fs.Length;
                        }

                        while (fs.Position < end)
                        {
                            try
                            {
                                var vehicleData = new VehicleKdTreeNode
                                {
                                    VehicleId = reader.ReadInt32(),
                                    VehicleRegistration = ReadNullTerminatedString(reader),
                                    Latitude = reader.ReadSingle(),
                                    Longitude = reader.ReadSingle(),
                                    RecordedTimeUTC = reader.ReadUInt64()
                                };

                                data.Add(vehicleData);
                            }
                            catch (Exception e)
                            {

                                Console.WriteLine($"Failed to create node: Position: {fs.Position}, VehicleId: {ReadNullTerminatedString(reader)} | " + e.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured in ReadChunk(Start: {start}, End: {end}): " + e.Message);
            }
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
