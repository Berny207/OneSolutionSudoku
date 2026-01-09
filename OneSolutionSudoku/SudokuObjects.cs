using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OneSolutionSudoku
{
	/// <summary>
	/// Base element of Sudoku grid
	/// Contains current value and list of possible values
	/// If value is 0, cell is empty<br/>
	/// If value is non-zero, possibleValues is empty and should not be used
	/// </summary>
	internal class Cell
	{
		public int Value;
		public List<int> PossibleValues;

		public Cell(int initialValue)
		{
			if(initialValue < 0 || initialValue > 9)
			{
				throw new ArgumentOutOfRangeException("initialValue", "Cell value must be between 0 and 9 inclusive.");
			}
			this.Value = initialValue;
			if(initialValue == 0)
			{
				PossibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			}
			else
			{
				PossibleValues = new List<int>();
			}
		}
	}
	/// <summary>
	/// 2 - number coordinates for Sudoku grid consisting of row and column, indexed from 0 to 8
	/// </summary>
	internal struct Coordinates
	{
		public int Row;
		public int Column;
		public Coordinates(int row, int column)
		{
			if(row < 0 || row > 8)
			{
				throw new ArgumentOutOfRangeException("row", "Row must be between 0 and 8 inclusive.");
			}
			if(column < 0 || column > 8)
			{
				throw new ArgumentOutOfRangeException("column", "Column must be between 0 and 8 inclusive.");
			}
			this.Row = row;
			this.Column = column;
		}
		public override string ToString()
		{
			return $"{this.Row} {this.Column}";
		}
	}
	/// <summary>
	/// main Sudoku class containing 9x9 grid of Cells and methods to manipulate it
	/// </summary>
	internal class Sudoku
	{
		// 9x9 grid of Cells
		Cell[,] Grid = new Cell[9,9];
		public Sudoku()
		{
			for (int row = 0; row < 9; row++)
			{
				Cell[] newRow = new Cell[9];
				for (int number = 0; number < 9; number++)
				{
					this.Grid[row, number] = new Cell(0);
				}
			}
		}
		/// <summary>
		/// Returns cell at coordinates
		/// </summary>
		/// <param name="coordinates"></param>
		/// <returns></returns>
		public Cell GetCell(Coordinates coordinates)
		{
			return this.Grid[coordinates.Row, coordinates.Column];
		}
		public void SetCell(Coordinates coordinates, int value)
		{
			List<Coordinates> neighbours = GetSpaceNeighbours(coordinates);
			/*foreach (Coordinates neighbour in neighbours)
			{
				Cell neighbourCell = this.GetCell(neighbour);
				if (neighbourCell.value == value && value != 0)
				{
					throw new WarningException("Invalid Sudoku created");
				}
			}*/
			this.Grid[coordinates.Row, coordinates.Column].Value = value;
		}

		/// <summary>
		/// Updates the cell at the given coordinates with the given value<br/>
		/// Returns a list of coordinates of all affected cells in the same row, column, and 3x3 box
		/// </summary>
		/// <param name="coordinates"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public List<Coordinates> GetSpaceNeighbours(Coordinates coordinates)
		{
			List<Coordinates> affectedCells = new List<Coordinates>();
			// Add all affected cells in the same row
			for (int col = 0; col < 9; col++)
			{
				if (col != coordinates.Column)
				{
					affectedCells.Add(new Coordinates(coordinates.Row, col));
				}
			}
			// Add all affected cells in the same column
			for (int row = 0; row < 9; row++)
			{
				if (row != coordinates.Row)
				{
					affectedCells.Add(new Coordinates(row, coordinates.Column));
				}
			}
			// Add all affected cells in the same 3x3 box EXCLUDING the row and column already added
			int boxStartRow = (coordinates.Row / 3) * 3;
			int boxStartCol = (coordinates.Column / 3) * 3;
			for (int row = boxStartRow; row < boxStartRow + 3; row++)
			{
				for (int col = boxStartCol; col < boxStartCol + 3; col++)
				{
					if (row != coordinates.Row && col != coordinates.Column)
					{
						affectedCells.Add(new Coordinates(row, col));
					}
				}
			}
			return affectedCells;
		}

		/// <summary>
		/// Checks if sudoku is valid - it doesn't have duplicate values in regions
		/// </summary>
		/// <returns></returns>
		public bool IsValid()
		{
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					Coordinates coordinates = new Coordinates(row, col);
					Cell cell = this.GetCell(coordinates);
					if(cell.Value == 0)
					{
						continue;
					}
					List<Coordinates> neighbours = this.GetSpaceNeighbours(coordinates);
					foreach (Coordinates neighbour in neighbours)
					{
						Cell neighbourCell = this.GetCell(neighbour);
						if(neighbourCell.Value == cell.Value)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Filters inputted list of coordinates to contain only empty cells
		/// </summary>
		/// <param name="coordinatesList"></param>
		/// <returns></returns>
		public List<Coordinates> FilterOutFullCells(List<Coordinates> coordinatesList)
		{
			List<Coordinates> filteredList = new List<Coordinates>();
			foreach (Coordinates coord in coordinatesList)
			{
				Cell cell = this.GetCell(coord);
				if (cell.Value == 0)
				{
					filteredList.Add(coord);
				}
			}
			return filteredList;
		}

		/// <summary>
		/// Calculates the number of cells in the grid that are affected by assigning a specific value to the specified
		/// coordinates
		/// </summary>
		public int AssigmentVariableWeight(Coordinates coordinates, int value)
		{
			List<Coordinates> affectedCells = new List<Coordinates>();
			for (int col = 0; col < 9; col++)
			{
				if (col != coordinates.Column)
				{
					affectedCells.Add(new Coordinates(coordinates.Row, col));
				}
			}
			// Add all affected cells in the same column
			for (int row = 0; row < 9; row++)
			{
				if (row != coordinates.Row)
				{
					affectedCells.Add(new Coordinates(row, coordinates.Column));
				}
			}
			// Add all affected cells in the same 3x3 box EXCLUDING the row and column already added
			int boxStartRow = (coordinates.Row / 3) * 3;
			int boxStartCol = (coordinates.Column / 3) * 3;
			for (int row = boxStartRow; row < boxStartRow + 3; row++)
			{
				for (int col = boxStartCol; col < boxStartCol + 3; col++)
				{
					if (row != coordinates.Row && col != coordinates.Column)
					{
						affectedCells.Add(new Coordinates(row, col));
					}
				}
			}
			affectedCells = affectedCells.Where(coord => this.Grid[coord.Row, coord.Column].Value != 0).ToList();
			affectedCells = affectedCells.Where(coord => this.Grid[coord.Row, coord.Column].PossibleValues.Contains(value) == true).ToList();
			return affectedCells.Count;
		}

		/// <summary>
		/// returns a printable string showing amounts of possible values in each cell
		/// </summary>
		/// <returns></returns>
		public string PrintPossibleValuesCounts()
		{
			StringBuilder sb = new StringBuilder();
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					sb.Append(this.Grid[row, col].PossibleValues.Count);
					sb.Append(" ");
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}

		/// <summary>
		/// returns printable string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					sb.Append(this.Grid[row, col].Value);
					sb.Append(" ");
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}

		/// <summary>
		/// returns printable string with rows and columns titled
		/// </summary>
		/// <returns></returns>
		public string PrintSudoku()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("  1 2 3 4 5 6 7 8 9 ");
			for (int row = 0; row < 9; row++)
			{
				sb.Append($"{row+1} ");
				for (int col = 0; col < 9; col++)
				{
					sb.Append(this.Grid[row, col].Value);
						sb.Append(" ");
				}
				sb.Append($"{row+1}");
				sb.AppendLine();
			}
			sb.AppendLine("  1 2 3 4 5 6 7 8 9 ");
			return sb.ToString();
		}

		/// <summary>
		/// reverts value assignment by readding deleted values into affected possible values
		/// </summary>
		/// <param name="step"></param>
		public void RevertAssignment(Step step)
		{
			this.Grid[step.Coordinates.Row, step.Coordinates.Column].Value = 0;
			foreach (var kvp in step.affectedCoordinates.data)
			{
				foreach (int affectedValue in kvp.Value)
				{
					this.GetCell(kvp.Key).PossibleValues.Add(affectedValue);
				}
			}
		}
		/// <summary>
		/// returns a list of empty cell coordinates
		/// </summary>
		/// <returns></returns>
		public List<Coordinates> GetEmptyCells()
		{
			List<Coordinates> emptyCells = new List<Coordinates>();
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					if (this.Grid[row, col].Value == 0)
					{
						emptyCells.Add(new Coordinates(row, col));
					}
				}
			}
			return emptyCells;
		}
		/// <summary>
		/// returns a list of full cell coordinates
		/// </summary>
		/// <returns></returns>
		public List<Coordinates> GetFullCells()
		{
			List<Coordinates> emptyCells = new List<Coordinates>();
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					if (this.Grid[row, col].Value != 0)
					{
						emptyCells.Add(new Coordinates(row, col));
					}
				}
			}
			return emptyCells;
		}

		/// <summary>
		/// returns copy of sudoku
		/// </summary>
		/// <returns></returns>
		public Sudoku Clone()
		{
			Sudoku clone = new Sudoku();
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					Cell originalCell = this.Grid[row, col];
					Cell clonedCell = new Cell(originalCell.Value);
					clonedCell.PossibleValues = new List<int>(originalCell.PossibleValues);
					clone.Grid[row, col] = clonedCell;
				}
			}
			return clone;
		}
		/// <summary>
		/// sets possible values based from cell values
		/// </summary>
		public void SetPossibleValues()
		{
			for (int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					Coordinates cellCoordinates = new Coordinates(row, column);
					this.SetPossibleValuesForCell(cellCoordinates);
				}
			}
		}
		/// <summary>
		/// sets possible values for one cell
		/// </summary>
		/// <param name="cellCoordinates"></param>
		public void SetPossibleValuesForCell(Coordinates cellCoordinates)
		{
			//Cell cell = this.GetCell(cellCoordinates);
			if (this.GetCell(cellCoordinates).Value != 0)
			{
				this.GetCell(cellCoordinates).PossibleValues = new List<int>();
				return;
			}
			this.GetCell(cellCoordinates).PossibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			List<Coordinates> neighbours = this.GetSpaceNeighbours(cellCoordinates);
			foreach (Coordinates neighbourCoordinates in neighbours)
			{
				Cell neighbourCell = this.GetCell(neighbourCoordinates);
				if (this.GetCell(cellCoordinates).PossibleValues.Contains(neighbourCell.Value))
				{
					this.GetCell(cellCoordinates).PossibleValues.Remove(neighbourCell.Value);
				}
			}
		}
	}
}
