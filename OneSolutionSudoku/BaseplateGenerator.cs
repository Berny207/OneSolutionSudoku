using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
namespace OneSolutionSudoku
{
	internal static class BaseplateGenerator
	{
		static Random random = new Random();
		public static Sudoku GenerateBaseplate()
		{
			Sudoku sudoku = new Sudoku();
			// Create a stack for our steps taken so we can backtrack
			Stack<Step> stepsTaken = new Stack<Step>();
			bool backtrack = false;
			while (stepsTaken.Count < 81) {
				Step currentStep = new Step();
				// If a step previously failed, we don't need to select a new cell
				if (backtrack == true)
				{
					try
					{
						currentStep = stepsTaken.Pop();
					}
					catch
					{
						throw new Exception("No solution found");
					}
					currentStep.bannedValues.Add(currentStep.value);
					sudoku.revertAssignment(currentStep);
				}
				else
				{
					List<Coordinates> constrainedVariables = CSPAlgorithms.GetMostConstrainedVariables(sudoku);
					currentStep.coordinates = constrainedVariables[random.Next(constrainedVariables.Count)];
				}
				int testValue = 0;
				for (int row = 0; row < 9; row++)
				{
					for (int column = 0; column < 9; column++)
					{
						if(sudoku.GetCell(new Coordinates(row, column)).value != 0)
						{
							testValue++;
						}
					}
				}
				Cell selectedCell = sudoku.GetCell(currentStep.coordinates);
				bool isPossibleAssignment = false;
				backtrack = false;
				while (isPossibleAssignment == false)
				{
					// Select random value from cell's domain
					// Assign value to cell
					List<int> constrainedValues = CSPAlgorithms.GetLeastConstrainedValues(sudoku, currentStep.coordinates, currentStep.bannedValues);
					if (constrainedValues.Count > 0) {
						currentStep.value = constrainedValues[random.Next(constrainedValues.Count)];
					}
					else
					{
						// No valid values left, need to backtrack
						backtrack = true;
						break;
					}
					sudoku.SetCell(currentStep.coordinates, currentStep.value);
					List<Coordinates> toUpdate = sudoku.FilterOutFullCells(sudoku.GetSpaceNeighbours(currentStep.coordinates));

					(isPossibleAssignment, List<Coordinates>? updatedCellCoordinates) = CSPAlgorithms.AssignValueWithLookahead(sudoku, currentStep.coordinates, currentStep.value);


					// Removes last assigment, reverts domains of affected cells
					if (isPossibleAssignment == false)
					{
						// Add value to banned values for this cell
						currentStep.bannedValues.Add(currentStep.value);
					}
					else
					{
						currentStep.affectedCoordinates = updatedCellCoordinates!;
					}
				}
				if(backtrack == false)
				{
					stepsTaken.Push(currentStep);
				}
			}
			return sudoku;
		}
	}
}
