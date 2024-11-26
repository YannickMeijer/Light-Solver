
namespace BlueRockTest
{
    public class BoardTotals
    {
        public int IncrementNeededToZeroBoard { get; private set; }

        public Dictionary<int, int> kvp = [];
        public BoardTotals(BoardTotals toClone) : this(toClone.kvp[0], toClone.kvp[1], toClone.kvp[2], toClone.kvp[3]) { }
        public BoardTotals(int zeros, int ones, int twos, int threes)
        {
            kvp.Add(0, zeros);
            kvp.Add(1, ones);
            kvp.Add(2, twos);
            kvp.Add(3, threes);
            IncrementNeededToZeroBoard = TotalNeededToZeroBoard();
        }

        public BoardTotals CreateAlteredBoardTotals(List<int> additions)
        {
            var zeros = kvp[0] - additions[0];
            var ones = kvp[1] + additions[0] - additions[1];
            var twos = kvp[2];
            var threes = kvp[3];

            if (PlayField.Depth == 2)
            {
                zeros += additions[1];
            }
            else if (PlayField.Depth == 3)
            {
                twos += additions[1] - additions[2];
                zeros += additions[2];
            }
            else
            {
                threes += additions[2] - additions[3];
                zeros += additions[3];
            }
            return new BoardTotals(zeros, ones, twos, threes);
        }

        private int TotalNeededToZeroBoard()
        {
            var ones = kvp[1] * (PlayField.Depth - 1);
            var twos = int.Max(0, kvp[2] * (PlayField.Depth - 2));
            var threes = int.Max(0, kvp[3]);

            return ones + twos + threes;
        }

        public int TotalNeededToZeroBoardAfterChange(int value)
        {
            AddOntoValue(value);

            if (value == 0)
            {
                IncrementNeededToZeroBoard += PlayField.Depth - 1;
            }
            else
            {
                IncrementNeededToZeroBoard -= 1;
            }
            return IncrementNeededToZeroBoard;
        }

        public void AddOntoValue(int value)
        {
            kvp[value]--;
            kvp[(value + 1) % PlayField.Depth]++;
        }

        public bool HasValue(int value)
        {
            return kvp[value] > 0;
        }
    }
}
