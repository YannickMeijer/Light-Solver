namespace BlueRockTest
{
    internal class PlayField
    {
        public static int FieldYDimension { get; private set; }
        public static int FieldXDimension { get; private set; }


        public static int Surface => FieldXDimension * FieldYDimension;

        public static Dictionary<int, int> CanIncrementTopLeftCorner = [];
        public static Dictionary<int, int> CanIncrementBottomLeftCorner = [];
        public static Dictionary<int, int> CanIncrementTopRightCorner = [];
        public static Dictionary<int, int> CanIncrementBottomRightCorner = [];

        private Dictionary<int, Piece> _pieceMap = [];
        private BoardTotals _InitialTotals;
        private int _totalIncrement;

        private List<Piece> UnMovingPieces = [];

        private List<Piece> _pieces = [];

        private Dictionary<(int, int), int> initialBoardState = [];

        public int EarlyStops { get; private set; }

        public static int Depth;

        public PlayField(string boardstate, int depth, string pieces)
        {
            Depth = depth;

            var splitLines = boardstate.Split(',');
            FieldXDimension = splitLines[0].Length;
            if (!splitLines.All(line => line.Length == FieldXDimension))
            {
                throw new InvalidOperationException("Your board has a hole in it, this is not supported");
            }

            FieldYDimension = splitLines.Length;

            FillField(splitLines);
            CreatePieces(pieces);
        }

        private void FillField(string[] lines)
        {
            var numbers = new int[4];
            for (int Yindex = 0; Yindex < FieldYDimension; Yindex++)
            {
                var line = lines[Yindex];
                for (int Xindex = 0; Xindex < FieldXDimension; Xindex++)
                {
                    var singleChar = line[Xindex].ToString();
                    if (!int.TryParse(singleChar, out int value))
                    {
                        throw new InvalidOperationException("One of your  lines contains a charater that is not a comma or a number");
                    }

                    if (value >= Depth)
                    {
                        throw new InvalidOperationException("One of your board lines contains a number that is deeper than the allowed depth");
                    }

                    if (0 > Depth)
                    {
                        throw new InvalidOperationException("One of your board lines contains a number that is negative");
                    }


                    numbers[value]++;
                    initialBoardState.Add((Xindex, Yindex), value);
                }
            }
            _InitialTotals = new BoardTotals(numbers[0], numbers[1], numbers[2], numbers[3]);
        }

        private void CreatePieces(string pieces)
        {
            var individualPieces = pieces.Split(' ');
            for (var index = 0; index < individualPieces.Count(); index++)
            {
                var pieceLines = individualPieces[index].Split(",");
                var maxLength = pieceLines.Max(line => line.Length);
                var newPiece = new Piece(pieceLines, maxLength, index);
                _pieceMap.Add(index, newPiece);

                _totalIncrement += newPiece.IncrementAmount;
                if (Surface == newPiece.SurfaceArea)
                {
                    UnMovingPieces.Add(newPiece);
                }
                else
                {
                    _pieces.Add(newPiece);
                }
            }
            _pieces.Sort();

            for(int index =0; index < _pieces.Count(); index++)
            {
                var piece = _pieces[index];
                piece.SetMinimumValue(index);
            }
        }

        public void ApplyUnmovingPieces(Dictionary<int, (int, int)> resultDictionary)
        {
            foreach (var piece in UnMovingPieces)
            {
                _totalIncrement -= piece.IncrementAmount;
                foreach (var incrementPosition in piece.IncrementLocations)
                {
                    var numberInPlace = initialBoardState[incrementPosition];
                    _InitialTotals.AddOntoValue(numberInPlace);
                    initialBoardState[incrementPosition] = (numberInPlace + 1) % PlayField.Depth;
                }
                resultDictionary.Add(piece.PlayOrder, (0, 0));
            }
        }

        public bool TrySolvePuzzle()
        {
            var resultDictionary = new Dictionary<int, (int, int)>();
            ApplyUnmovingPieces(resultDictionary);

            var treeRoot = new SmartTreeNode(_InitialTotals, initialBoardState);
            var coordinates = treeRoot.BuildAndSolveTree(_pieces, _pieces.Count - 1, _totalIncrement);

            var success = false;
            if (coordinates.Count > 0)
            {
                foreach (var coordinate in coordinates)
                {
                    resultDictionary.Add(coordinate.Item1, coordinate.Item2);
                }
                var result = string.Empty;
                for (int index = 0; index < _pieces.Count; index++)
                {
                    result += resultDictionary[index].Item1.ToString() + ',' + resultDictionary[index].Item2.ToString();

                    if (index != _pieces.Count - 1) {
                        result += ' ';
                    }
                }
                success = Checker(_pieces,coordinates);
                if (success)
                {
                    Console.WriteLine(result);
                }
            }
            return success;
        }

        public bool Checker(List<Piece> pieces, List<(int, (int, int))> coordinates)
        {
            var copy = initialBoardState.ToDictionary();
            foreach (var coord in coordinates)
            {
                var PieceNumbar = coord.Item1;
                var xY = coord.Item2;
                var matchingPiece = pieces.First(piece => piece.PlayOrder == PieceNumbar);
                foreach (var inc in matchingPiece.IncrementLocations)
                {
                    var loc = (inc.Item1 + xY.Item1, inc.Item2 + xY.Item2);
                    copy[loc] = (copy[loc] + 1) % Depth;
                }
            }
            return !(copy.ContainsValue(1) || copy.ContainsValue(2) || copy.ContainsValue(3));
        }
    }
}
