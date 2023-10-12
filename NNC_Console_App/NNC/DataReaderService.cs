using System.Collections.Concurrent;
using System.Text;

namespace NNC
{
    internal class DataReaderService
    {
        // Reads Binary data from file, and populate VehicleKdTreeNode list
        public static List<VehicleKdTreeNode> ReadBinaryData(string dataFilePath)
        {
            ConcurrentBag<VehicleKdTreeNode> vehicleList = new ConcurrentBag<VehicleKdTreeNode>();
            try
            {
                int chunkSize = 1024 * 1024; // Adjust the chunk size as needed.
                using (FileStream fs = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
                {
                    long fileLength = fs.Length;
                    List<Task> tasks = new List<Task>();

                    for (long position = 0; position < fileLength; position += chunkSize)
                    {
                        long chunkStart = position;
                        long chunkEnd = Math.Min(chunkStart + chunkSize, fileLength);

                        // Checks if the values are in the valid range.
                        if (chunkStart < 0 || chunkEnd < 0)
                        {
                            Console.WriteLine($"Invalid chunk values. Start: {chunkStart}, End: {chunkEnd}");
                            break;
                        }

                        tasks.Add(Task.Run(() => ReadChunk(vehicleList, fs, chunkStart, chunkEnd)));
                    }

                    Task.WhenAll(tasks).Wait(); // Wait for all tasks to complete.
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            return vehicleList.ToList();
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
