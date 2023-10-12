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

        Console.WriteLine("Done reading data from binary file");
        Console.WriteLine("");
        Console.WriteLine("Starting to find closest vehicles...");
        Console.WriteLine($"");

        closestNodeTimer.Start();

        // Build the k-d tree
        KdTreeService builder = new KdTreeService();
        VehicleKdTreeNode root = builder.BuildKdTree(vehicleList);

        // Target coordinates for the search.
        List<TargetCoordinate> targetCoordinates = new List<TargetCoordinate>() {
           new  TargetCoordinate(){ Position = 1, Latitude = 34.544909f, Longitude = -102.100843f },
           new TargetCoordinate{ Position = 2, Latitude = 32.345544f, Longitude = -99.123124f },
           new  TargetCoordinate(){ Position = 3, Latitude = 33.234235f, Longitude = -100.214124f },
           new TargetCoordinate{Position = 4, Latitude = 35.195739f, Longitude = -95.348899f },
           new  TargetCoordinate(){Position = 5, Latitude = 31.895839f, Longitude = -97.789573f },
           new TargetCoordinate{Position = 6, Latitude = 32.895839f, Longitude = -101.789573f },
           new  TargetCoordinate(){Position = 7, Latitude = 34.115839f, Longitude = -100.225732f },
           new TargetCoordinate{Position = 8, Latitude = 32.335839f, Longitude = -99.992232f },
           new  TargetCoordinate(){Position = 9, Latitude = 33.535339f, Longitude = -94.792232f },
           new TargetCoordinate{Position = 10, Latitude = 32.234235f, Longitude = -100.222222f },
        };

        foreach (var target in targetCoordinates)
        {
            // Call the SearchKdTree method to find the closest node to the target position.
            VehicleKdTreeNode closestNode = builder.SearchKdTree(root, target.Latitude, target.Longitude);
            if (closestNode != null)
            {
                Console.WriteLine($"Target Location: (Position: {target.Position}, Latitude: {target.Latitude}, Longitude: {target.Longitude})");
                Console.WriteLine($"Closest Vehicle Registration: ({closestNode.VehicleRegistration})");
                Console.WriteLine($"");
            }
            else
            {
                Console.WriteLine($"Closest Node NOT found");
            }
        };

        closestNodeTimer.Stop();
        totalTimer.Stop();

        Console.WriteLine($"Data file Read Execution Time: {dataReadTimer.ElapsedMilliseconds} ms");
        Console.WriteLine($"Closest Position Execution Time: {closestNodeTimer.ElapsedMilliseconds} ms");
        Console.WriteLine($"Total Execution Time: {totalTimer.ElapsedMilliseconds} ms");
    }
}