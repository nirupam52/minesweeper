using System;
using System.Linq;

namespace minesweeper {

  public class Minesweeper {
    private const int SIZE = 10;
    public enum CellState { UNEXPOSED, EXPOSED, SEALED }
    public enum GameStatus {  WIN, LOST, INPROGRESS}
    public CellState[,] CellStates { get; private set; } = new CellState[SIZE, SIZE];
    public bool[,] mines { get; private set; } = new bool[SIZE, SIZE];

    virtual public void ExposeCell(int row, int column) {
      Console.WriteLine(row.ToString() + "," + column.ToString()); //Feedback: we don't print stuff from arbitrary places in code.
      if (CellStates[row, column] == CellState.UNEXPOSED) {
        CellStates[row, column] = CellState.EXPOSED;

        if (AdjacentMinesCountAt(row, column) == 0 && !mines[row, column]) { //Feedback: we do not need the part after &&
          ExposeNeighbourCells(row, column);
        }
      }
    }

    virtual public void ExposeNeighbourCells(int row, int column) {
      for (int x = Math.Max(0, row - 1); x < Math.Min(SIZE, row + 2); x++) {
        for (int y = Math.Max(0, column - 1); y < Math.Min(SIZE, column + 2); y++) {
          ExposeCell(x, y);
        }
      }
    }

    public void ToggleSeal(int row, int column) {
      CellStates[row, column] = CellStates[row, column] switch {
        CellState.UNEXPOSED => CellState.SEALED,

        CellState.SEALED => CellState.UNEXPOSED,

        _ => CellState.EXPOSED,
      };
    }

    public bool IsMineAt(int row, int column) {
      return row >= 0 && row < SIZE && column >= 0 && column < SIZE && mines[row, column];
    }

    internal void SetMines(int seedValue) {
      Random getRandomIndex = new Random(seedValue);
      mines = new bool[SIZE, SIZE];

      while (mines.Cast<bool>().ToArray().Where(c => c).Count() < 10) {
        mines[getRandomIndex.Next(0, SIZE), getRandomIndex.Next(0, SIZE)] = true;
      }
    }

    public int AdjacentMinesCountAt(int row, int column) {
      int minesCount = 0;

      for (int x = Math.Max(0, row - 1); x < Math.Min(SIZE, row + 2); x++) {
        for (int y = Math.Max(0, column - 1); y < Math.Min(SIZE, column + 2); y++) {
           minesCount += IsMineAt(x, y) ? 1 : 0;
        }
      }

      return IsMineAt(row, column) ? minesCount - 1 : minesCount;
    }

    public GameStatus GetGameStatus() {
      GameStatus gameStatus = GameStatus.WIN;

      for (int i = 0; i < SIZE; i++) {
        for (int j = 0; j < SIZE; j++) {
          if (mines[i,j] && CellStates[i, j] == CellState.EXPOSED) {
            return gameStatus = GameStatus.LOST;
          }

          else if ((mines[i, j] && CellStates[i, j] == CellState.UNEXPOSED) || (!mines[i, j] && CellStates[i, j] != CellState.EXPOSED)) {
            gameStatus = GameStatus.INPROGRESS;
          }
        }
      }

      return gameStatus;
    }
  }
}
