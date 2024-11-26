using System.Data;

namespace BlueRockTest
{
    public class SmartTreeNode(BoardTotals boardTotals, Dictionary<(int, int), int> currentBoard)
    {
        private readonly BoardTotals _boardTotals = boardTotals;
        public Dictionary<(int, int), int> CurrentBoard = currentBoard;

        public List<(int, (int, int))> BuildAndSolveTree(List<Piece> pieceList, int pieceIndex, int remainingIncrements)
        {
            var piece = pieceList[pieceIndex];

            var incrementsLeftExclusive = remainingIncrements - piece.IncrementAmount;
            List<(int, (int, int))> result;

            for (int indexY = 0; indexY < (piece.TopLeftMaxY + 1); indexY++)
            {
                for (int indexX = 0; indexX < (piece.TopLeftMaxX + 1); indexX++)
                {
                    var workingBoard = CurrentBoard.ToDictionary();
                    var workingTotals = new BoardTotals(_boardTotals);
                    var workingIncrements = remainingIncrements;
                    bool earlyStop = false;

                    foreach (var incrementPosition in piece.IncrementLocations)
                    {
                        var posToIncrement = (incrementPosition.Item1 + indexX, incrementPosition.Item2 + indexY);
                        var numberInPlace = workingBoard[posToIncrement];
                        workingIncrements--;
                        if (workingTotals.TotalNeededToZeroBoardAfterChange(numberInPlace) > workingIncrements || (numberInPlace + 1 ) < piece.minimumValue)
                        {
                            earlyStop = true;
                            break;
                        }
                        workingBoard[posToIncrement] = (numberInPlace + 1) % PlayField.Depth;
                    }

                    if (earlyStop
                        || piece.minimumValue > 1 && workingTotals.HasValue(1)
                        || piece.minimumValue > 2 && workingTotals.HasValue(2))
                    {
                        continue;
                    }

                    result = SelectNextSolve(workingBoard, workingTotals, pieceIndex, pieceList, incrementsLeftExclusive);
                    if (result.Count == 0)
                    {
                        continue;
                    }

                    result.Add((piece.PlayOrder, (indexX, indexY)));
                    return result;
                }
            }
            return [];
        }

        public static List<(int, (int, int))> SelectNextSolve(Dictionary<(int, int), int> workingBoard, BoardTotals workingTotals, int pieceIndex, List<Piece> pieceList, int incrementsLeftExclusive)
        {
            if (pieceIndex > 0)
            {
                var nextNode = new SmartTreeNode(workingTotals, workingBoard);

                List<(int, (int, int))> result;
                if (workingTotals.IncrementNeededToZeroBoard == incrementsLeftExclusive)
                {
                    result = nextNode.PerfectFit(pieceList, pieceIndex - 1);
                }
                else
                {
                    result = nextNode.BuildAndSolveTree(pieceList, pieceIndex - 1, incrementsLeftExclusive);
                }
                return result;
            }
            return [];
        }

        public List<(int, (int, int))> PerfectFit(List<Piece> pieceList, int pieceIndex)
        {
            var piece = pieceList[pieceIndex];

            List<(int, (int, int))>? result = [];
            foreach ((var coordinate, _) in CurrentBoard.Where(kvp => kvp.Value != 0))
            {
                if (piece.FirstIncrementMaxY < coordinate.Item2)
                {
                    break;
                }
                else if (piece.FirstIncrementMaxX < coordinate.Item1)
                {
                    continue;
                }
                var firstIncrementCoordinate = piece.IncrementLocations[0];

                if (coordinate.Item1 < firstIncrementCoordinate.Item1 || coordinate.Item2 < firstIncrementCoordinate.Item2)
                {
                    continue;
                }

                var pieceTopLeftCoordinate = (coordinate.Item1 - firstIncrementCoordinate.Item1, coordinate.Item2 - firstIncrementCoordinate.Item2);
                var workingBoard = CurrentBoard.ToDictionary();
                var workingTotals = new BoardTotals(_boardTotals);

                workingTotals.AddOntoValue(workingBoard[coordinate]);
                workingBoard[coordinate] = (workingBoard[coordinate] + 1) % PlayField.Depth;

                var earlyExit = CanSuccesfullyApplyPiece(piece, pieceTopLeftCoordinate, workingBoard, workingTotals);

                if (earlyExit
                        || piece.minimumValue > 1 && workingTotals.HasValue(1)
                        || piece.minimumValue > 2 && workingTotals.HasValue(2))
                {
                    if (pieceIndex == 0)
                    {
                        break;
                    }
                    continue;
                }

                if (pieceIndex > 0)
                {
                    var nextNode = new SmartTreeNode(workingTotals, workingBoard);
                    result = nextNode.PerfectFit(pieceList, pieceIndex - 1);
                    if (result.Count == 0)
                    {
                        continue;
                    }
                }
                result.Add((piece.PlayOrder, pieceTopLeftCoordinate));
                return result;
            }
            return [];
        }

        private static bool CanSuccesfullyApplyPiece(Piece piece, (int, int) pieceTopLeftCoordinate, Dictionary<(int, int), int> workingBoard, BoardTotals workingTotals)
        {
            List<(int, int)> coordsToApply = [];
            for (int index = 1; index < piece.IncrementLocations.Count; index++)
            {
                var currentIncrement = piece.IncrementLocations[index];
                var coordianteToCheck = (pieceTopLeftCoordinate.Item1 + currentIncrement.Item1, pieceTopLeftCoordinate.Item2 + currentIncrement.Item2);

                if (workingBoard[coordianteToCheck] == 0)
                {
                    return true;
                }
                coordsToApply.Add(coordianteToCheck);
            }

            foreach (var coordinate in coordsToApply)
            {
                workingTotals.AddOntoValue(workingBoard[coordinate]);
                workingBoard[coordinate] = (workingBoard[coordinate] + 1) % PlayField.Depth;
            }

            return false;
        }
    }
}
