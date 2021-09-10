using NUnit.Framework;
using minesweeper;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace minesweeperTest {
  public class MinesweeperTest {

    private Minesweeper minesweeper;


    [SetUp]
    public void Setup() {
      minesweeper = new Minesweeper();
    }

    [Test]
    public void CanaryTest() { 
      Assert.IsTrue(true);
    }

    [Test]
    public void CheckInitialStateIsUnexposed() {
      Assert.AreEqual(Minesweeper.CellState.UNEXPOSED, minesweeper.CellStates[2, 3]);
    }

    [Test]
    public void ExposeAnUnexposedCell() {
      minesweeper.ExposeCell(2, 3);

      Assert.AreEqual(Minesweeper.CellState.EXPOSED, minesweeper.CellStates[2, 3]);
    }

    [Test]
    public void ExposeAnExposedCell() {
      minesweeper.ExposeCell(2, 3);

      minesweeper.ExposeCell(2, 3);

      Assert.AreEqual(Minesweeper.CellState.EXPOSED, minesweeper.CellStates[2, 3]);
    }

    [Test]
    public void CellOutOfBounds() {
      Assert.Throws<IndexOutOfRangeException>(() => minesweeper.ExposeCell(11, 7));
    }

    [Test]
    public void SealUnexposedCell() {
      minesweeper.ToggleSeal(2, 3);

      Assert.AreEqual(Minesweeper.CellState.SEALED, minesweeper.CellStates[2, 3]);
    }

    [Test]
    public void UnsealASealedCell() {
      minesweeper.ToggleSeal(2, 3);

      minesweeper.ToggleSeal(2, 3);

      Assert.AreEqual(Minesweeper.CellState.UNEXPOSED, minesweeper.CellStates[2, 3]);
    }

    [Test]
    public void SealAnExposedCell() {
      minesweeper.ExposeCell(2, 3); 

      minesweeper.ToggleSeal(2, 3);

      Assert.AreEqual(Minesweeper.CellState.EXPOSED, minesweeper.CellStates[2, 3]);
    }

    [Test]
    public void ExposeASealedCell() {
      minesweeper.ToggleSeal(2, 3);

      minesweeper.ExposeCell(2, 3);

      Assert.AreEqual(Minesweeper.CellState.SEALED, minesweeper.CellStates[2, 3]);
    }

    class MinesweeperWithExposeNeighborStubbed : Minesweeper {
      public StringBuilder CalledFor { get; private set; } = new StringBuilder();

      override public void ExposeNeighbourCells(int row, int column)
      {
        CalledFor.Append(row + "," + column);
      }
    }

    [Test]
    public void ExposeCallsExposeNeighbours() {
      MinesweeperWithExposeNeighborStubbed minesweeper = new MinesweeperWithExposeNeighborStubbed();

      minesweeper.ExposeCell(5, 5);

      Assert.AreEqual("5,5", minesweeper.CalledFor.ToString().Trim());
    }

    [Test]
    public void ExposeOnAlreadyExposedCellDoesNotCallExposeNeighbours() {
      MinesweeperWithExposeNeighborStubbed minesweeper = new MinesweeperWithExposeNeighborStubbed();

      minesweeper.ExposeCell(5, 5);

      minesweeper.CalledFor.Clear();

      minesweeper.ExposeCell(5, 5);

      Assert.AreEqual("", minesweeper.CalledFor.ToString());
    }

    [Test]
    public void ExposeOnAlreadySealedCellDoesNotCallExposeNeighbours() {
      MinesweeperWithExposeNeighborStubbed minesweeper = new MinesweeperWithExposeNeighborStubbed();

      minesweeper.ToggleSeal(5, 5);

      minesweeper.CalledFor.Clear(); 

      minesweeper.ExposeCell(5, 5);

      Assert.AreEqual("", minesweeper.CalledFor.ToString());
    }

    class MinesweeperWithExposeCellStubbed : Minesweeper {
      public StringBuilder CalledCells { get; private set; } = new StringBuilder();

      override public void ExposeCell(int row, int column)
      {
        CalledCells.Append(row + "," + column + " ");
      }
    }

    [Test]
    public void ExposeNeighboursCallsExposeOnEightNeighbours() {
      MinesweeperWithExposeCellStubbed minesweeper = new MinesweeperWithExposeCellStubbed();

      minesweeper.ExposeNeighbourCells(5, 5);

      Assert.AreEqual("4,4 4,5 4,6 5,4 5,5 5,6 6,4 6,5 6,6", minesweeper.CalledCells.ToString().Trim());

    }

    [Test]
    public void ExposeNeighboursCalledOnTopLeftCell() {
      MinesweeperWithExposeCellStubbed minesweeper = new MinesweeperWithExposeCellStubbed();

      minesweeper.ExposeNeighbourCells(0, 0);

      Assert.AreEqual("0,0 0,1 1,0 1,1", minesweeper.CalledCells.ToString().Trim());

    }

    [Test]
    public void ExposeNeighboursCalledOnBottomRightCell() {
      MinesweeperWithExposeCellStubbed minesweeper = new MinesweeperWithExposeCellStubbed();

      minesweeper.ExposeNeighbourCells(9, 9);

      Assert.AreEqual("8,8 8,9 9,8 9,9", minesweeper.CalledCells.ToString().Trim());
    }

    [Test]
    public void ExposeNeighboursCalledOnBorderCell() {
      MinesweeperWithExposeCellStubbed minesweeper = new MinesweeperWithExposeCellStubbed();

      minesweeper.ExposeNeighbourCells(9, 8);

      Assert.AreEqual("8,7 8,8 8,9 9,7 9,8 9,9", minesweeper.CalledCells.ToString().Trim());
    }

    [Test]
    public void CheckIfMineIsAtAExistantCell() {
      Assert.IsFalse(minesweeper.IsMineAt(3, 2));
    }

    [Test]
    public void SetAndCheckIfMineIsAtAExistantCell() {
      minesweeper.mines[3, 2] = true;

      Assert.IsTrue(minesweeper.IsMineAt(3, 2));

    }

    [TestCase(-1, 4)]
    [TestCase(10, 5)]
    [TestCase(5, -1)]
    [TestCase(7, 10)]
    public void CheckIfMineIsAtANonExistantCell(int row, int column) {
      Assert.AreEqual(false, minesweeper.IsMineAt(row, column));
    }

    [Test]
    public void ExposingAnAdjacentCellDoesNotCallExposeNeighbors() {
      MinesweeperWithExposeNeighborStubbed minesweeper = new MinesweeperWithExposeNeighborStubbed();

      minesweeper.mines[3, 2] = true;

      minesweeper.ExposeCell(3, 1);

      Assert.AreEqual("", minesweeper.CalledFor.ToString());
    }

    [Test]
    public void CheckIfACellWithNoMineNeighboursReturnsZero() {
      Assert.AreEqual(0, minesweeper.AdjacentMinesCountAt(4, 6));
    }

    [Test]
    public void CheckIfAMineCellWithNoMineNeighboursReturnsZero() {
      minesweeper.mines[3, 4] = true;

      Assert.AreEqual(0, minesweeper.AdjacentMinesCountAt(4, 6));
    }

    [Test]
    public void CheckIfACellWithMineNeighbourReturnsOne() {
      minesweeper.mines[3, 4] = true;

      Assert.AreEqual(1, minesweeper.AdjacentMinesCountAt(3, 5));
    }

    [Test]
    public void CheckIfCellWithMultipleMineNeighboursReturnsCount() {
      minesweeper.mines[3, 4] = true;
      minesweeper.mines[2, 6] = true;

      Assert.AreEqual(2, minesweeper.AdjacentMinesCountAt(3, 5));
    }

    [Test]
    public void CheckNeighbourMineCountBySettingAtTopRow() {
      minesweeper.mines[0, 1] = true;

      Assert.AreEqual(1, minesweeper.AdjacentMinesCountAt(0, 0));
    }

    [Test]
    public void CheckCellMineCountAtTopRightCorner() {
      Assert.AreEqual(0, minesweeper.AdjacentMinesCountAt(0, 9));
    }

    [Test]
    public void CheckCellMineCountAtBottomLeftCorner() {
      Assert.AreEqual(0, minesweeper.AdjacentMinesCountAt(9, 0));
    }

    [Test]
    public void CheckNeighbourMineCountBySettingAtBottomRow() {
      minesweeper.mines[9, 8] = true;

      Assert.AreEqual(1, minesweeper.AdjacentMinesCountAt(9, 9));
    }

    [Test]
    public void CheckIfGameIsInprogress() {
      minesweeper.mines[1, 1] = true;

      Assert.AreEqual(Minesweeper.GameStatus.INPROGRESS, minesweeper.GetGameStatus());
    }

    [Test]
    public void ExposeMinedCellAndGameStatusReturnsLost(){
      minesweeper.mines[2, 2] = true;

      minesweeper.ExposeCell(2, 2);

      Assert.AreEqual(Minesweeper.GameStatus.LOST, minesweeper.GetGameStatus());
    }

    [Test]
    public void GameInProgressAfterMinesSealedAndCellsUnexposed(){
      minesweeper.mines[2, 2] = true;

      minesweeper.ToggleSeal(2, 2);

      Assert.AreEqual(Minesweeper.GameStatus.INPROGRESS, minesweeper.GetGameStatus());
    }

    [Test]
    public void GameInProgressAfterMinesSealedAndEmptyCellIsSealed(){
      minesweeper.mines[2, 2] = true;

      minesweeper.ToggleSeal(2, 2);

      minesweeper.ToggleSeal(2, 4);

      minesweeper.ExposeCell(0, 0);

      Assert.AreEqual(Minesweeper.GameStatus.INPROGRESS, minesweeper.GetGameStatus());
    }

    [Test]
    public void GameInProgressAfterMinesSealedAndAdjacentCellUnexposed(){
      minesweeper.mines[2, 2] = true;

      minesweeper.ToggleSeal(2, 2);

      minesweeper.ToggleSeal(2, 3);

      minesweeper.ExposeCell(0, 0);

      minesweeper.ToggleSeal(2,3);

      Assert.AreEqual(Minesweeper.GameStatus.INPROGRESS, minesweeper.GetGameStatus());

    }

    [Test]
    public void GameWinWhenAllMinesSealedAndOtherCellsExposed(){
      minesweeper.mines[2,2] = true;

      minesweeper.ToggleSeal(2, 2);

      minesweeper.ExposeCell(0, 0);

      Assert.AreEqual(Minesweeper.GameStatus.WIN, minesweeper.GetGameStatus());
    }

    [Test]
    public void PlacingTenMinesWithASeedValue() {
      minesweeper.SetMines(0);

      Assert.AreEqual(10, minesweeper.mines.Cast<bool>().ToArray().Where(c => c).Count());
    }

    [Test]
    public void CheckIfTwoDiffrentSeedValesReturnAtleastOneDiffrentIndex()
    {
      minesweeper.SetMines(0);
      bool[,] mines1 = minesweeper.mines;

      minesweeper.SetMines(1);
      bool[,] mines2 = minesweeper.mines;

      Assert.AreNotEqual(mines1, mines2);
    }
  }
}
