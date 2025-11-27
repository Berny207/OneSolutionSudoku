using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OneSolutionSudoku
{
	internal class SudokuPuncturer
	{
		static Random random = new Random();
		public static Sudoku SetPossibleValues(Sudoku sudoku)
		{
			for(int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					Coordinates cellCoordinates = new Coordinates(row, column);
					Cell cell = sudoku.GetCell(cellCoordinates);
					if (cell.value != 0)
					{
						cell.possibleValues.Clear();
						continue;
					}
					cell.possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
					List<Coordinates> neighbours = sudoku.GetSpaceNeighbours(cellCoordinates);
					foreach (Coordinates neighbourCoordinates in neighbours)
					{
						Cell neighbourCell = sudoku.GetCell(neighbourCoordinates);
						if (cell.possibleValues.Contains(neighbourCell.value))
						{
							cell.possibleValues.Remove(neighbourCell.value);
						}
					}
				}
			}
			return sudoku;
		}

		public static bool IsSudokuSolved(Sudoku sudoku)
		{
			for(int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					Cell cell = sudoku.GetCell(new Coordinates(row, column));
					if (cell.value == 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool isSudokuUnique(Sudoku sudoku, Coordinates newEmptyCell)
		{
			SetPossibleValues(sudoku);
			int solutionCount = 0;

			foreach(int testValue in sudoku.GetCell(newEmptyCell).possibleValues)
			{
				sudoku.SetCell(newEmptyCell, testValue);

			}
			return true;
		}
		/// <summary>
		/// Solves given Sudoku using it's possible values for empty cells.
		/// </summary>
		/// <param name="sudoku"></param>
		/// <returns></returns>
		public static bool SolveSudoku(Sudoku sudoku)
		{
			Stack<Step> stepsTaken = new Stack<Step>();
			while (true)
			{
				bool backtrack = false;
				Step currentStep = new Step();
				if (backtrack == true)
				{
					currentStep = stepsTaken.Pop();
					currentStep.bannedValues.Add(currentStep.value);
					sudoku.revertAssignment(currentStep);
				}
				else
				{
					List<Coordinates> constrainedVariables = CSPAlgorithms.GetMostConstrainedVariables(sudoku);
					if (constrainedVariables.Count > 0)
					{
						currentStep.coordinates = constrainedVariables[0];
					}
					else
					{
						// No empty cells left
						return true;
					}
				}
				bool isPossibleAssignment = false;
				while (isPossibleAssignment == false)
				{
					List<int> constrainedValues = CSPAlgorithms.GetLeastConstrainedValues(sudoku, currentStep.coordinates, currentStep.bannedValues);
					if (constrainedValues.Count > 0) {
						currentStep.value = constrainedValues[0];
					}
					else
					{
						backtrack = true;
						break;
					}
					sudoku.SetCell(currentStep.coordinates, currentStep.value);
					List<Coordinates> toUpdate = sudoku.GetSpaceNeighbours(currentStep.coordinates);

					(isPossibleAssignment, List<Coordinates>? updatedCellCoordinates) = CSPAlgorithms.AssignValueWithLookahead(sudoku, currentStep.coordinates, currentStep.value);

					if(isPossibleAssignment == false)
					{
						currentStep.bannedValues.Add(currentStep.value);
					}
					else
					{
						currentStep.affectedCoordinates = updatedCellCoordinates;
						stepsTaken.Push(currentStep);
					}
				}
				if(backtrack == false)
				{
					stepsTaken.Push(currentStep);

					// Check if solved

					if (IsSudokuSolved(sudoku) == true)
					{
						return true;
					}
				}
			}
		}
		public static Sudoku GenerateSudoku(int missingCells)
		{
			Sudoku sudoku = BaseplateGenerator.GenerateBaseplate();
			// Set possible values for all empty cells
			SetPossibleValues(sudoku);

			Stack<Step> removedCells = new Stack<Step>();
			bool backtrack = false;
			List<Coordinates> availibleCoordinates = new List<Coordinates>();
			// Select random cell to remove
			while (missingCells < removedCells.Count)
			{
				Step currentStep = new Step();
				if (backtrack == true)
				{

				}
				else
				{
					availibleCoordinates = new List<Coordinates>();
					for (int row = 0; row < 9; row++)
					{
						for (int column = 0; column < 9; column++)
						{
							availibleCoordinates.Add(new Coordinates(row, column));
						}
					}
				}
				if (availibleCoordinates.Count == 0)
				{
					backtrack = true;
					break;
				}
				Coordinates randomCoordinates = availibleCoordinates[random.Next(availibleCoordinates.Count)];
				currentStep = new Step(randomCoordinates, sudoku.GetCell(randomCoordinates).value);
				// Try to remove current step cell
				backtrack = false;
				bool isUnique = true;
				sudoku.SetCell(currentStep.coordinates, 0);
				sudoku.GetCell(currentStep.coordinates).possibleValues.Remove(currentStep.value);

				foreach (int startValue in sudoku.GetCell(currentStep.coordinates).possibleValues)
				{
					sudoku.SetCell(currentStep.coordinates, startValue);
					isUnique = SolveSudoku(sudoku.Clone());
					if (isUnique == true)
					{
						break;
					}
					sudoku.SetCell(currentStep.coordinates, 0);
				}
				sudoku.SetCell(currentStep.coordinates, 0);

				if (isUnique == true)
				{
					removedCells.Push(currentStep);
				}
				else
				{
					sudoku.SetCell(currentStep.coordinates, currentStep.value);
					availibleCoordinates.Remove(randomCoordinates);
				}
			}
			return sudoku;
		}
	}
}
