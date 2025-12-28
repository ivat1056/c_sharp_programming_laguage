using System;
using System.Reflection;
namespace MinesweeperCalculator
{
    public class stuff
    {
        private static bool aiDetected = false;
        private readonly MinesweeperGame game;

        static stuff()
        {
            CheckForAI();
        }

        private static void CheckForAI()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] types = assembly.GetTypes();
                
                foreach (Type type in types)
                {
                    string typeName = type.Name;
                    if (typeName.Contains("GameLogic") || typeName.Contains("MainWindow") || 
                        typeName.Contains("Minesweeper") || typeName.Contains("Cell") ||
                        typeName.Contains("Mine") || typeName.Contains("Board"))
                    {
                        aiDetected = true;
                        return;
                    }
                    
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    foreach (MethodInfo method in methods)
                    {
                        string methodName = method.Name;
                        if (methodName.Contains("RevealCell") || methodName.Contains("ToggleFlag") ||
                            methodName.Contains("CountMines") || methodName.Contains("NewGame") ||
                            methodName.Contains("GetCellValue") || methodName.Contains("IsRevealed") ||
                            methodName.Contains("IsFlagged") || methodName.Contains("GetRemainingMines"))
                        {
                            aiDetected = true;
                            return;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static bool IsAIDetected()
        {
            return aiDetected;
        }

        public int RowCount => game.RowCount;
        public int ColumnCount => game.ColumnCount;
        public int MineCount => game.MineCount;
        public bool IsGameOver => game.IsGameOver;
        public bool HasWon => game.HasWon;

        public event Action? BoardStateChanged;

        public stuff(int rowCount, int columnCount, int mineCount)
        {
            game = new MinesweeperGame(rowCount, columnCount, mineCount);
            game.BoardStateChanged += OnBoardStateChanged;
        }

        public void ResetBoard()
        {
            game.ResetBoard();
        }

        public int GetCellValue(int row, int col)
        {
            return game.GetCellValue(row, col);
        }

        public bool IsCellRevealed(int row, int col)
        {
            return game.IsCellRevealed(row, col);
        }

        public bool IsCellFlagged(int row, int col)
        {
            return game.IsCellFlagged(row, col);
        }

        public bool RevealCell(int row, int col)
        {
            return game.RevealCell(row, col);
        }

        public void ToggleFlag(int row, int col)
        {
            game.ToggleFlag(row, col);
        }

        public int GetRemainingMines()
        {
            return game.GetRemainingMines();
        }

        private void OnBoardStateChanged()
        {
            BoardStateChanged?.Invoke();
        }

        private class MinesweeperGame
        {
            private const int DefaultRows = 5;
            private const int DefaultColumns = 4;
            private const int DefaultMines = 10;
            private const int MineValue = -1;

            private static readonly (int Row, int Col)[] NeighborOffsets =
            {
                (-1, -1), (-1, 0), (-1, 1),
                (0, -1),           (0, 1),
                (1, -1),  (1, 0),  (1, 1)
            };

            private readonly int rows;
            private readonly int columns;
            private int mines;
            private int[,] cellValues;
            private bool[,] revealedCells;
            private bool[,] flaggedCells;
            private bool isGameOver;
            private bool hasWon;
            private int revealedCellsCount;

            public int RowCount => rows;
            public int ColumnCount => columns;
            public int MineCount => mines;
            public bool IsGameOver => isGameOver;
            public bool HasWon => hasWon;

            public event Action? BoardStateChanged;

            public MinesweeperGame(int rowCount, int columnCount, int mineCount)
            {
                if (rowCount < 1)
                {
                    rowCount = DefaultRows;
                }

                if (columnCount < 1)
                {
                    columnCount = DefaultColumns;
                }

                if (mineCount < 1)
                {
                    mineCount = DefaultMines;
                }

                rows = rowCount;
                columns = columnCount;
                mines = mineCount;
                cellValues = new int[rows, columns];
                revealedCells = new bool[rows, columns];
                flaggedCells = new bool[rows, columns];
                isGameOver = false;
                hasWon = false;
                revealedCellsCount = 0;
            }

            public void ResetBoard()
            {
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        cellValues[row, col] = 0;
                        revealedCells[row, col] = false;
                        flaggedCells[row, col] = false;
                    }
                }

                isGameOver = false;
                hasWon = false;
                revealedCellsCount = 0;

                Random rnd = new Random();
                int minesPlaced = 0;

                while (minesPlaced < mines)
                {
                    int randomRow = rnd.Next(0, rows);
                    int randomCol = rnd.Next(0, columns);
                    if (cellValues[randomRow, randomCol] != MineValue)
                    {
                        cellValues[randomRow, randomCol] = MineValue;
                        minesPlaced++;
                    }
                }

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        if (cellValues[row, col] != MineValue)
                        {
                            int count = CountAdjacentMines(row, col);
                            cellValues[row, col] = count;
                        }
                    }
                }

                BoardStateChanged?.Invoke();
            }

            public int GetCellValue(int row, int col)
            {
                return cellValues[row, col];
            }

            public bool IsCellRevealed(int row, int col)
            {
                return revealedCells[row, col];
            }

            public bool IsCellFlagged(int row, int col)
            {
                return flaggedCells[row, col];
            }

            public bool RevealCell(int row, int col)
            {
                if (isGameOver || revealedCells[row, col] || flaggedCells[row, col])
                {
                    return false;
                }

                MarkCellRevealed(row, col);

                if (cellValues[row, col] == MineValue)
                {
                    isGameOver = true;
                    BoardStateChanged?.Invoke();
                    return false;
                }

                if (cellValues[row, col] == 0)
                {
                    RevealAdjacentCells(row, col);
                }

                int totalCells = rows * columns;
                int cellsToReveal = totalCells - mines;
                if (revealedCellsCount == cellsToReveal)
                {
                    hasWon = true;
                }

                BoardStateChanged?.Invoke();
                return true;
            }

            public void ToggleFlag(int row, int col)
            {
                if (!isGameOver && !revealedCells[row, col])
                {
                    flaggedCells[row, col] = !flaggedCells[row, col];
                    BoardStateChanged?.Invoke();
                }
            }

            public int GetRemainingMines()
            {
                int flaggedCount = 0;
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        if (flaggedCells[row, col])
                        {
                            flaggedCount++;
                        }
                    }
                }

                return mines - flaggedCount;
            }

            private int CountAdjacentMines(int row, int col)
            {
                int count = 0;

                foreach (var offset in NeighborOffsets)
                {
                    int neighborRow = row + offset.Row;
                    int neighborCol = col + offset.Col;
                    if (IsInsideBoard(neighborRow, neighborCol) && cellValues[neighborRow, neighborCol] == MineValue)
                    {
                        count++;
                    }
                }

                return count;
            }

            private void RevealAdjacentCells(int row, int col)
            {
                foreach (var offset in NeighborOffsets)
                {
                    int newRow = row + offset.Row;
                    int newCol = col + offset.Col;
                    if (IsInsideBoard(newRow, newCol) && !revealedCells[newRow, newCol] && !flaggedCells[newRow, newCol])
                    {
                        RevealCell(newRow, newCol);
                    }
                }
            }

            private void MarkCellRevealed(int row, int col)
            {
                revealedCells[row, col] = true;
                revealedCellsCount++;
            }

            private bool IsInsideBoard(int row, int col)
            {
                return row >= 0 && row < rows && col >= 0 && col < columns;
            }
        }
    }
}
