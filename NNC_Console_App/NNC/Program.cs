using ModelsNamespace;
using NNC;

class Program
{
    static void Main(string[] args)
    {
        string dataFilePath = "C:\\Dev\\Personal\\NNC\\NNC_Console_App\\NNC\\Data\\VehiclePositions.dat";

        var totalTimer = new System.Diagnostics.Stopwatch();
        var dataReadTimer = new System.Diagnostics.Stopwatch();
        var closestNodeTimer = new System.Diagnostics.Stopwatch();

        totalTimer.Start();

        // Check if data file is empty
        FileInfo fileInfo = new FileInfo(dataFilePath);
        if (fileInfo.Length == 0)
        {
            Console.WriteLine("The file is empty.");
        }

        Console.WriteLine("Starting to read binary file...");
        dataReadTimer.Start();

        // Read Binary Data
        List<VehicleKdTreeNode> vehicleList = DataReaderService.ReadBinaryData(dataFilePath);

        dataReadTimer.Stop();
    }
}