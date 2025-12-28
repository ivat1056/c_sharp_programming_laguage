using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MinesweeperCalculator;
using Xunit;

namespace MinesweeperCalculator.Tests
{
    public class StuffTests
    {
        private static readonly (int Row, int Col)[] NeighborOffsets =
        {
            (-1, -1), (-1, 0), (-1, 1),
            (0, -1),           (0, 1),
            (1, -1),  (1, 0),  (1, 1)
        };

        [Fact]
        public void RevealEmptyBoard_RevealsAllAndSetsWin()
        {
            stuff game = PrepareGame(2, 2, Array.Empty<(int, int)>());

            bool revealResult = game.RevealCell(0, 0);

            Assert.True(revealResult);
            Assert.False(game.IsGameOver);
            Assert.True(game.HasWon);
            Assert.All(AllCoords(2, 2), coord => Assert.True(game.IsCellRevealed(coord.Row, coord.Col)));
            Assert.Equal(0, game.GetRemainingMines());
        }

        [Fact]
        public void RevealMine_SetsGameOver()
        {
            var mines = new[] { (0, 1) };
            stuff game = PrepareGame(2, 2, mines);

            bool revealResult = game.RevealCell(0, 1);

            Assert.False(revealResult);
            Assert.True(game.IsGameOver);
            Assert.False(game.HasWon);
            Assert.Equal(-1, game.GetCellValue(0, 1));
            Assert.True(game.IsCellRevealed(0, 1));
        }

        [Fact]
        public void ToggleFlag_TogglesAndRaisesEvent()
        {
            stuff game = PrepareGame(3, 3, Array.Empty<(int, int)>());
            int eventCount = 0;
            game.BoardStateChanged += () => eventCount++;

            game.ToggleFlag(1, 1);
            Assert.True(game.IsCellFlagged(1, 1));
            game.ToggleFlag(1, 1);
            Assert.False(game.IsCellFlagged(1, 1));
            Assert.Equal(2, eventCount);

            // revealed cell should ignore toggling
            game.RevealCell(0, 0);
            game.ToggleFlag(0, 0);
            Assert.False(game.IsCellFlagged(0, 0));
        }

        private static stuff PrepareGame(int rows, int cols, IEnumerable<(int Row, int Col)> mines)
        {
            var mineList = mines.ToList();
            var game = new stuff(rows, cols, mineList.Count);
            var innerGame = GetInnerGame(game);

            SetField(innerGame, "isGameOver", false);
            SetField(innerGame, "hasWon", false);
            SetField(innerGame, "revealedCellsCount", 0);
            SetField(innerGame, "cellValues", BuildBoard(rows, cols, mineList));
            SetField(innerGame, "revealedCells", new bool[rows, cols]);
            SetField(innerGame, "flaggedCells", new bool[rows, cols]);
            SetField(innerGame, "mines", mineList.Count);

            return game;
        }

        private static int[,] BuildBoard(int rows, int cols, List<(int Row, int Col)> mines)
        {
            int[,] board = new int[rows, cols];
            foreach (var (row, col) in mines)
            {
                board[row, col] = -1;
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (board[row, col] == -1)
                    {
                        continue;
                    }

                    int count = 0;
                    foreach (var offset in NeighborOffsets)
                    {
                        int nr = row + offset.Row;
                        int nc = col + offset.Col;
                        if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && board[nr, nc] == -1)
                        {
                            count++;
                        }
                    }

                    board[row, col] = count;
                }
            }

            return board;
        }

        private static IEnumerable<(int Row, int Col)> AllCoords(int rows, int cols)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    yield return (r, c);
                }
            }
        }

        private static void SetField<T>(object target, string fieldName, T value)
        {
            FieldInfo? field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new InvalidOperationException($"Field '{fieldName}' not found");
            }
            field.SetValue(target, value);
        }

        private static object GetInnerGame(stuff game)
        {
            FieldInfo? field = typeof(stuff).GetField("game", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new InvalidOperationException("Поле 'game' не найдено");
            }

            var value = field.GetValue(game);
            if (value == null)
            {
                throw new InvalidOperationException("Поле 'game' не инициализировано");
            }

            return value;
        }
    }
}
