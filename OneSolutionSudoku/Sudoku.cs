using System;
using System.Collections.Generic;
using System.Linq;
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
	internal struct Cell
	{
		public int value;
		public List<int> possibleValues;

		public Cell(int initialValue)
		{
			if(initialValue < 0 || initialValue > 9)
			{
				throw new ArgumentOutOfRangeException("initialValue", "Cell value must be between 0 and 9 inclusive.");
			}
			this.value = initialValue;
			if(initialValue == 0)
			{
				possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			}
			else
			{
				possibleValues = new List<int>();
			}
		}
	}
	/// <summary>
	/// 2 - number coordinates for Sudoku grid consisting of row and column, indexed from 0 to 8
	/// </summary>
	internal struct Coordinates
	{
		public int row;
		public int column;
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
			this.row = row;
			this.column = column;
		}
	}
	/// <summary>
	/// main Sudoku class containing 9x9 grid of Cells and methods to manipulate it
	/// </summary>
	internal class Sudoku
	{
		// 9x9 grid of Cells
		Cell[,] grid = new Cell[9,9];
		public Sudoku()
		{
			for (int row = 0; row < 9; row++)
			{
				Cell[] newRow = new Cell[9];
				for (int number = 0; number < 9; number++)
				{
					this.grid[row, number] = new Cell(0);
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
			return this.grid[coordinates.row, coordinates.column];
		}
		/// <summary>
		/// Updates the cell at the given coordinates with the given value<br/>
		/// Returns a list of coordinates of all affected cells in the same row, column, and 3x3 box
		/// </summary>
		/// <param name="coordinates"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public List<Coordinates> UpdateSpace(Coordinates coordinates, int value)
		{
			List<Coordinates> affectedCells = new List<Coordinates>();
			// Update the cell at the given coordinates
			this.grid[coordinates.row, coordinates.column].value = value;
			// Add all affected cells in the same row
			for (int col = 0; col < 9; col++)
			{
				if (col != coordinates.column)
				{
					affectedCells.Add(new Coordinates(coordinates.row, col));
				}
			}
			// Add all affected cells in the same column
			for (int row = 0; row < 9; row++)
			{
				if (row != coordinates.row)
				{
					affectedCells.Add(new Coordinates(row, coordinates.column));
				}
			}
			// Add all affected cells in the same 3x3 box EXCLUDING the row and column already added
			int boxStartRow = (coordinates.row / 3) * 3;
			int boxStartCol = (coordinates.column / 3) * 3;
			for (int row = boxStartRow; row < boxStartRow + 3; row++)
			{
				for (int col = boxStartCol; col < boxStartCol + 3; col++)
				{
					if (row != coordinates.row && col != coordinates.column)
					{
						affectedCells.Add(new Coordinates(row, col));
					}
				}
			}
			// Remove cells that have set value
			affectedCells = affectedCells.Where(coord => this.grid[coord.row, coord.column].value != 0).ToList();
			return affectedCells;
		}


		public int AssigmentVariableWeight(Coordinates coordinates, int value)
		{
			List<Coordinates> affectedCells = new List<Coordinates>();
			for (int col = 0; col < 9; col++)
			{
				if (col != coordinates.column)
				{
					affectedCells.Add(new Coordinates(coordinates.row, col));
				}
			}
			// Add all affected cells in the same column
			for (int row = 0; row < 9; row++)
			{
				if (row != coordinates.row)
				{
					affectedCells.Add(new Coordinates(row, coordinates.column));
				}
			}
			// Add all affected cells in the same 3x3 box EXCLUDING the row and column already added
			int boxStartRow = (coordinates.row / 3) * 3;
			int boxStartCol = (coordinates.column / 3) * 3;
			for (int row = boxStartRow; row < boxStartRow + 3; row++)
			{
				for (int col = boxStartCol; col < boxStartCol + 3; col++)
				{
					if (row != coordinates.row && col != coordinates.column)
					{
						affectedCells.Add(new Coordinates(row, col));
					}
				}
			}
			affectedCells = affectedCells.Where(coord => this.grid[coord.row, coord.column].value != 0).ToList();
			affectedCells = affectedCells.Where(coord => this.grid[coord.row, coord.column].possibleValues.Contains(value) == true).ToList();
			return affectedCells.Count;
		}
	}
}
