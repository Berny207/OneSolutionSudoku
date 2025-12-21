using OneSolutionSudoku;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OneSolutionSudoku
{
	internal class MainAlgoritmhs
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
				}
				else
				{
					if (IsSudokuSolved(sudoku) == true)
					{
						return true;
					}
					// Create a new Step
					List<Coordinates> constrainedVariables = CSPAlgorithms.GetMostConstrainedVariables(sudoku);
					currentStep.coordinates = constrainedVariables[random.Next(constrainedVariables.Count)];
				}
				backtrack = false;
				bool isPossibleAssignment = false;
				while (isPossibleAssignment == false)
				{
					// Select least constrained unbanned variable 
					List<int> possileAssignmentValues = CSPAlgorithms.GetLeastConstrainedValues(sudoku, currentStep.coordinates, currentStep.bannedValues);
					if (possileAssignmentValues.Count > 0) {
						currentStep.value = possileAssignmentValues[random.Next(possileAssignmentValues.Count)];
					}
					else
					{
						// No possible value could be selected, backtrack
						backtrack = true;
						break;
					}
					// Try to assign value to cell

					(isPossibleAssignment, Multimap<Coordinates, int> updatedCellCoordinates) = CSPAlgorithms.AssignValue(sudoku, currentStep.coordinates, currentStep.value);

					if(isPossibleAssignment == false)
					{
						currentStep.bannedValues.Add(currentStep.value);
					}
					else
					{
						currentStep.affectedCoordinates = updatedCellCoordinates;
					}
				}
				if(backtrack == false)
				{
					stepsTaken.Push(currentStep);
				}
			}
		}
		public static Coordinates SelectValueToRemove(Sudoku sudoku, List<Coordinates> availibleValues)
		{
			Dictionary<int, int> valueCounts = new Dictionary<int, int> 
			{ 
				{1, 0}, 
				{2, 0},
				{3, 0},
				{4, 0},
				{5, 0},
				{6, 0},
				{7, 0},
				{8, 0},
				{9, 0}
			};
			;
			foreach (Coordinates fullCellCoordinate in availibleValues)
			{
				Cell fullCell = sudoku.GetCell(fullCellCoordinate);
				valueCounts[fullCell.value]++;
			}
			int highestValueCount = 0;
			int mostFrequentValue = 0;
			foreach(var kvp in valueCounts)
			{
				if(highestValueCount < kvp.Value)
				{
					highestValueCount = kvp.Value;
					mostFrequentValue = kvp.Key;
				}
			}
			List<Coordinates> selection = new List<Coordinates>();
			foreach (Coordinates fullCellCoordinate in availibleValues)
			{
				Cell fullCell = sudoku.GetCell(fullCellCoordinate);
				if(fullCell.value == mostFrequentValue)
				{
					selection.Add(fullCellCoordinate);
				}
			}
			return selection[random.Next(selection.Count)];
		}

		public static Sudoku GenerateSudoku(int missingCells, CancellationToken token)
		{
			Sudoku sudoku = new Sudoku();
			SolveSudoku(sudoku);
			sudoku.SetPossibleValues();
			Sudoku solvedSudoku = sudoku.Clone();
			// Set possible values for all empty cells
			Stack<Elimination_Step> removedCells = new Stack<Elimination_Step>();
			bool backtrack = false;
			// Select random cell to remove
			while (missingCells >= removedCells.Count)
			{
				token.ThrowIfCancellationRequested();
				Elimination_Step currentStep = new Elimination_Step();
				if (backtrack == true)
				{
					currentStep = removedCells.Pop();
					currentStep.availibleCoordinates.Remove(currentStep.coordinates);
					sudoku.SetCell(currentStep.coordinates, currentStep.value);
					// Revert last removed cell, backtrack
					// Code commented is multi-step backtracking
					/*for (int i = 0; i < 10; i++)
					{
						if(removedCells.Count == 0)
						{
							break;
						}
						currentStep = removedCells.Pop();
						sudoku.SetCell(currentStep.coordinates, currentStep.value);
					}*/
				}
				else
				{
					// Create new step
					currentStep = new Elimination_Step();
					currentStep.availibleCoordinates = sudoku.GetFullCells();
				}

				// Try to remove at least ONE cell
				while (true)
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
					currentStep.coordinates = SelectValueToRemove(sudoku, currentStep.availibleCoordinates);
					currentStep.value = sudoku.GetCell(currentStep.coordinates).value;
					// Try to remove current step cell
					bool isDuplicate = false;
					// Try to find a solution with different value in the removed cell

					sudoku.SetCell(currentStep.coordinates, 0);
					sudoku.SetPossibleValuesForCell(currentStep.coordinates);
					List<int> possibleValues = sudoku.GetCell(currentStep.coordinates).possibleValues;
					possibleValues.Remove(solvedSudoku.GetCell(currentStep.coordinates).value);
					foreach (int startValue in possibleValues)
					{
						Sudoku cloneDoku = sudoku.Clone();
						cloneDoku.SetCell(currentStep.coordinates, startValue);
						isDuplicate = SolveSudoku(cloneDoku);
						if (isDuplicate == true)
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
						// UPDATE POSSIBLE VALUES
						break;
					}
				}
			}
			return sudoku;
		}
	}
}