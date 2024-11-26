// See https://aka.ms/new-console-template for more information
using BlueRockTest;

string filename;

if (args.Length == 0)
{
    Console.WriteLine("Please provide a relative path to a text file");
    Console.WriteLine($"current directory: {Directory.GetCurrentDirectory()}");
    filename = Console.ReadLine();
}
else
{
    filename = args[0];
}

var file = Directory.GetCurrentDirectory() + "\\" + filename;
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

if (depthAsInt > 4 || depthAsInt < 2)
{
    throw new ArgumentException("depth is an invalid number");
}

var field = new PlayField(board, depthAsInt, pieces);

var success = field.TrySolvePuzzle();

if (success)
{
    Environment.Exit(0);
}
else
{
    Environment.Exit(-1);
}