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
			sudoku.SetPossibleValues();
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
			sudoku.SetPossibleValues();
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
					// Create a new Step
					List<Coordinates> constrainedVariables = CSPAlgorithms.GetMostConstrainedVariables(sudoku);
					if(constrainedVariables.Count == 0)
					{
						// No empty cells left, so it's full
						return true;
					}
					if (sudoku.GetCell(constrainedVariables[0]).possibleValues.Count > 0)
					{
						currentStep.coordinates = constrainedVariables[0];
					}
					else
					{
						// There is an empty cell with 0 possible values.
						return false;
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
			Stack<Elimination_Step> removedCells = new Stack<Elimination_Step>();
			bool backtrack = false;
			// Select random cell to remove
			while (missingCells >= removedCells.Count)
			{

				Elimination_Step currentStep;
				if (backtrack == true)
				{
					// Revert last removed cell
					currentStep = removedCells.Pop();
					currentStep.availibleCoordinates.Remove(currentStep.coordinates);
					sudoku.SetCell(currentStep.coordinates, currentStep.value);
				}
				else
				{
					// Create new step
					currentStep = new Elimination_Step();
					for (int row = 0; row < 9; row++)
					{
						for (int column = 0; column < 9; column++)
						{
							if (sudoku.GetCell(new Coordinates(row, column)).value != 0)
							{
								currentStep.availibleCoordinates.Add(new Coordinates(row, column));
							}
						}
					}
				}

				// 
				bool removedValue = false;
				while(removedValue == false)
				{
					backtrack = false;
					if (currentStep.availibleCoordinates.Count == 0)
					{
						backtrack = true;
						continue;
					}
					currentStep.coordinates = currentStep.availibleCoordinates[random.Next(currentStep.availibleCoordinates.Count)];
					currentStep.value = sudoku.GetCell(currentStep.coordinates).value;
					// Try to remove current step cell
					bool isDuplicate = false;

					// Try to find a solution with different value in the removed cell
					foreach (int startValue in sudoku.GetCell(currentStep.coordinates).possibleValues)
					{
						try
						{
							sudoku.SetCell(currentStep.coordinates, startValue);
						}
						catch
						{
							continue;
						}
						isDuplicate = SolveSudoku(sudoku);
						if(isDuplicate == true)
						{
							break;
						}
					}
					sudoku.SetCell(currentStep.coordinates, 0);

					// If at least one other solution found, revert the change
					if (isDuplicate == false)
					{
						// Confirm our step
						removedCells.Push(currentStep);
						removedValue = true;
					}
					else
					{
						// Revert made step, try again with different cell
						sudoku.SetCell(currentStep.coordinates, currentStep.value);
						currentStep.availibleCoordinates.Remove(currentStep.coordinates);
					}
				}
			}
			return sudoku;
		}
	}
}
