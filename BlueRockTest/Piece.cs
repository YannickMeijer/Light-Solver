
namespace BlueRockTest
{
    public class Piece : IComparable<Piece>
    {
        public int[,] Layout { get; private set; }
        public int IncrementAmount { get; private set; }
        public int DimY { get; private set; }
        public int DimX { get; private set; }


        public int TopLeftMaxY { get; private set; }
        public int TopLeftMaxX { get; private set; }

        public int FirstIncrementMaxX { get; private set; }
        public int FirstIncrementMaxY { get; private set; }


        public int SurfaceArea { get; private set; }
        public int PlayOrder { get; private set; }

        public List<(int, int)> IncrementLocations = [];

        public int minimumValue = 0;


        public Piece(string[] lines, int maxLength, int pieceIndex)
        {
            try
            {
                PlayOrder = pieceIndex;
                DimX = maxLength;
                DimY = lines.Length;

                TopLeftMaxX = PlayField.FieldXDimension - DimX;
                TopLeftMaxY = PlayField.FieldYDimension - DimY;


                var FirstIncrementMaxSet = false;

                SurfaceArea = DimX * DimY;
                Layout = new int[DimX, DimY];
                for (int Yindex = 0; Yindex < DimY; Yindex++)
                {
                    var line = lines[Yindex];
                    for (int Xindex = 0; Xindex < DimX; Xindex++)
                    {
                        var singleChar = line[Xindex];
                        switch (singleChar)
                        {
                            case ('.'):
                                Layout[Xindex, Yindex] = 0;
                                break;
                            case ('X'):
                                Layout[Xindex, Yindex] = 1;
                                IncrementLocations.Add((Xindex, Yindex));
                                IncrementAmount++;
                                if (!FirstIncrementMaxSet)
                                {
                                    FirstIncrementMaxX = TopLeftMaxX + Xindex;
                                    FirstIncrementMaxY = TopLeftMaxY + Yindex;
                                    FirstIncrementMaxSet = true;
                                }
                                break;
                            default:
                                throw new InvalidOperationException("You have provided a piece with a value other than . or X");
                        };
                    }
                }
            }
            catch (IndexOutOfRangeException _)
            {
                throw new InvalidOperationException("You have prodived a piece with a hole in it, this is not supported");

            }
        }

        public void SetMinimumValue(int index)
        {
            minimumValue = PlayField.Depth - index;
        }

        public int CompareTo(Piece? other)
        {
            var diff = SurfaceArea - other.SurfaceArea;
            return diff switch
            {
                0 => IncrementAmount - other.IncrementAmount,
                < 0 => -1,
                > 0 => 1
            };
        }
    }
}
