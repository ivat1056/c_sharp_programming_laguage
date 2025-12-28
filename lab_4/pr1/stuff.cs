using System;
using System.Reflection;
namespace MinesweeperCalculator
{
    /// <summary>
    /// Encapsulates the minesweeper-style board logic and state.
    /// </summary>
    public class stuff
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
        private readonly int mines;
        private int[,] cellValues;
        private bool[,] revealedCells;
        private bool[,] flaggedCells;
        private bool isGameOver;
        private bool hasWon;
        private int revealedCellsCount;
        private static bool aiDetected = false;

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

        /// <summary>
        /// Gets the number of rows on the board.
        /// </summary>
        public int RowCount => rows;

        /// <summary>
        /// Gets the number of columns on the board.
        /// </summary>
        public int ColumnCount => columns;

        /// <summary>
        /// Gets the number of mines placed on the board.
        /// </summary>
        public int MineCount => mines;

        /// <summary>
        /// Gets a value indicating whether the game is lost.
        /// </summary>
        public bool IsGameOver => isGameOver;

        /// <summary>
        /// Gets a value indicating whether the game is won.
        /// </summary>
        public bool HasWon => hasWon;

        /// <summary>
        /// Occurs when the board state changes.
        /// </summary>
        public event Action? BoardStateChanged;

        public stuff(int rowCount, int columnCount, int mineCount)
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

        /// <summary>
        /// Resets the board, places mines, and calculates neighbor counts.
        /// </summary>
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

        /// <summary>
        /// Returns the value for a cell: mine marker, zero, or neighbor count.
        /// </summary>
        public int GetCellValue(int row, int col)
        {
            return cellValues[row, col];
        }

        /// <summary>
        /// Returns true if the cell is already revealed.
        /// </summary>
        public bool IsCellRevealed(int row, int col)
        {
            return revealedCells[row, col];
        }

        /// <summary>
        /// Returns true if the cell is flagged.
        /// </summary>
        public bool IsCellFlagged(int row, int col)
        {
            return flaggedCells[row, col];
        }

        /// <summary>
        /// Reveals the requested cell and propagates through empty neighbors.
        /// </summary>
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

        /// <summary>
        /// Toggles flag state for a cell when possible.
        /// </summary>
        public void ToggleFlag(int row, int col)
        {
            if (!isGameOver && !revealedCells[row, col])
            {
                flaggedCells[row, col] = !flaggedCells[row, col];
                BoardStateChanged?.Invoke();
            }
        }

        /// <summary>
        /// Returns how many mines are still unflagged.
        /// </summary>
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

            int result = mines - flaggedCount;
            return result;
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
