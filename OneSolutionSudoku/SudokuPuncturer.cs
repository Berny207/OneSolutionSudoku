using OneSolutionSudoku;
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
		/// <summary>
		/// Solves given Sudoku using it's possible values for empty cells.
		/// </summary>
		/// <param name="sudoku"></param>
		/// <returns></returns>
		public static bool SolveSudoku(Sudoku sudoku)
		{
			if (!sudoku.IsValid())
			{
				return false;
			}
			sudoku.SetPossibleValues();
			Stack<Step> stepsTaken = new Stack<Step>();
			bool backtrack = false;
			while (true)
			{
				Step currentStep = new Step();
				if (backtrack == true)
				{
					if(stepsTaken.Count == 0)
					{
						return false;
					}
					currentStep = stepsTaken.Pop();
					currentStep.bannedValues.Add(currentStep.value);
					sudoku.revertAssignment(currentStep);
					backtrack = false;
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
					// Select least constrained unbanned variable 
					List<int> constrainedValues = CSPAlgorithms.GetLeastConstrainedValues(sudoku, currentStep.coordinates, currentStep.bannedValues);
					if (constrainedValues.Count > 0) {
						currentStep.value = constrainedValues[0];
					}
					else
					{
						// No possible value could be selected, backtrack
						backtrack = true;
						break;
					}

					// Try to assign value to cell
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
		// Lmao doesn't work
		public static Sudoku GenerateSudoku(int missingCells)
		{
			Sudoku sudoku = BaseplateGenerator.GenerateBaseplate();
			Sudoku solvedSudoku = sudoku.Clone();
			// Set possible values for all empty cells
			Stack<Elimination_Step> removedCells = new Stack<Elimination_Step>();
			bool backtrack = false;
			// Select random cell to remove
			while (missingCells >= removedCells.Count)
			{
				Elimination_Step currentStep;
				if (backtrack == true)
				{
					// Revert last removed cell, backtrack
					currentStep = removedCells.Pop();
					currentStep.availibleCoordinates.Remove(currentStep.coordinates);
					sudoku.SetCell(currentStep.coordinates, currentStep.value);
				}
				else
				{
					// Create new step
					currentStep = new Elimination_Step();
					currentStep.availibleCoordinates = sudoku.GetFullCells();
				}
				bool hasValueBeenRemoved = false;

				// Try to remove at least ONE cell
				while(hasValueBeenRemoved == false)
				{
					// Trying a fresh value, set backtrack to false
					backtrack = false;

					// If no value can be selected, backtrack
					if (currentStep.availibleCoordinates.Count == 0)
					{
						backtrack = true;
						break;
					}

					// Select random coordinates from the availible ones
					currentStep.coordinates = currentStep.availibleCoordinates[random.Next(currentStep.availibleCoordinates.Count)];
					currentStep.value = sudoku.GetCell(currentStep.coordinates).value;
					// Try to remove current step cell
					bool isDuplicate = false;
					// Try to find a solution with different value in the removed cell

					sudoku.SetCell(currentStep.coordinates, 0);
					sudoku.SetPossibleValuesForCell(currentStep.coordinates);
					List<int> possibleValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
					possibleValues.Remove(solvedSudoku.GetCell(currentStep.coordinates).value);
					foreach (int startValue in possibleValues)
					{
						Sudoku cloneDoku = sudoku.Clone();
						cloneDoku.SetCell(currentStep.coordinates, startValue);
						isDuplicate = SolveSudoku(cloneDoku);
						if(isDuplicate == true)
						{
							// Found another solution with different value in removed cell
							// That cell CANNOT be removed
							// Revert and try different cell
							sudoku.SetCell(currentStep.coordinates, currentStep.value);
							currentStep.availibleCoordinates.Remove(currentStep.coordinates);
							break;
						}
					}
					// If no solutions have been found, proceed with change
					if (isDuplicate == false)
					{
						// Confirm our step
						removedCells.Push(currentStep);
						hasValueBeenRemoved = true;
					}
				}
			}
			return sudoku;
		}
	}
}