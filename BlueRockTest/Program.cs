// See https://aka.ms/new-console-template for more information
using BlueRockTest;
using System.Diagnostics;

Console.WriteLine("Hello, World!");
string filename;

if(args.Length == 0)
{
    Console.WriteLine("Please provide a relative path to a text file");
    Console.WriteLine($"current directory: {Directory.GetCurrentDirectory()}");
    filename = Console.ReadLine();
}
else
{
    filename = args[0];
}

var file = Directory.GetCurrentDirectory() + "\\"  + filename;
string depth;
string board;
string pieces;

    using (StreamReader stream = new StreamReader(file))
    {
        string line;
        depth = stream.ReadLine() ?? throw new InvalidOperationException("The file you have provided does not have a first line");
        board = stream.ReadLine() ?? throw new InvalidOperationException("The file you have provided does not have a second line");
        pieces = stream.ReadLine() ?? throw new InvalidOperationException("The file you have provided does not have a third line");
        if (stream.ReadLine() != null)
        {
            throw new InvalidOperationException("The file you have provided contains extraneous lines");
        }

    }

    var depthAsInt = int.Parse(depth);

    if(depthAsInt > 4 || depthAsInt < 2)
    {
        throw new ArgumentException("depth is an invalid number");
    }
    
    Stopwatch stopWatch = Stopwatch.StartNew();
    var field = new PlayField(board, depthAsInt,pieces);

    Console.WriteLine($"Depth provided: {depthAsInt}");
    Console.WriteLine($"Board provided:");
    field.PrintField();
    Console.WriteLine($"Pieces provided: {pieces}");

    Console.WriteLine("");
    Console.WriteLine("Beginning solve");

    var success = field.TrySolvePuzzle();

    if (success)
    {
        Console.WriteLine("Success");
        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);
    }
    else
    {
        Console.WriteLine("Failure");
    }

    Console.ReadLine();
