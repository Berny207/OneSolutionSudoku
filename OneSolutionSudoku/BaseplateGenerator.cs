using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
namespace OneSolutionSudoku
{
	class Step
	{
		public Coordinates coordinates;
		public int value;
		public List<int> bannedValues;
		public List<Coordinates> affectedCoordinates;

		public Step(Coordinates coordinates, int value)
		{
			this.coordinates = coordinates;
			this.value = value;
			this.bannedValues = new List<int>();
			this.affectedCoordinates = new List<Coordinates>();
		}
	}
	internal static class BaseplateGenerator
	{
		static Random random = new Random();
		public static Coordinates GetMostConstrainedVariable(Sudoku sudoku)
		{
			List<Coordinates> mostConstrainedCells = new List<Coordinates>();
			int minDomainSize = 10; // More than max possible domain size
			for (int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					Coordinates coordinates = new Coordinates(row, column);
					if (sudoku.GetCell(coordinates).value == 0 && sudoku.GetCell(coordinates).possibleValues.Count < minDomainSize)
					{
						minDomainSize = sudoku.GetCell(coordinates).possibleValues.Count;
						mostConstrainedCells.Clear();
						mostConstrainedCells.Add(coordinates);
					}
					else if (sudoku.GetCell(coordinates).value == 0 && sudoku.GetCell(coordinates).possibleValues.Count == minDomainSize)
					{
						mostConstrainedCells.Add(coordinates);
					}
				}
			}
			Coordinates selectedCoordinates = mostConstrainedCells[random.Next(mostConstrainedCells.Count)];
			return selectedCoordinates;
		}

		public static int GetLeastConstrainedValue(Sudoku sudoku, Coordinates selectedCoordinates, List<int> bannedValues)
		{
			int minimalConstraintValue = 20;
			List<int> candidateValues = new List<int>();
			Cell selectedCell = sudoku.GetCell(selectedCoordinates);
			foreach (int value in selectedCell.possibleValues)
			{
				if (bannedValues.Contains(value))
				{
					continue;
				}
				int constraintValue = sudoku.AssigmentVariableWeight(selectedCoordinates, value);
				if (constraintValue < minimalConstraintValue)
				{
					minimalConstraintValue = constraintValue;
					candidateValues.Clear();
					candidateValues.Add(value);
				}
				else if (constraintValue == minimalConstraintValue)
				{
					candidateValues.Add(value);
				}
			}
			if( candidateValues.Count == 0)
			{
				throw new Exception("No valid candidate values found.");
			}
			return candidateValues[random.Next(candidateValues.Count)];
		}
		public static Sudoku GenerateBaseplate()
		{
			Sudoku sudoku = new Sudoku();
			// Create a stack for our steps taken so we can backtrack
			Stack<Step> stepsTaken = new Stack<Step>();
			bool backtrack = false;
			while (stepsTaken.Count < 81) {
				Console.WriteLine(sudoku.ToString());
				Console.WriteLine("Steps taken: " + stepsTaken.Count);
				Coordinates selectedCoordinates;
				Step currentStep;
				// If a step previously failed, we don't need to select a new cell
				if (backtrack == false)
				{
					selectedCoordinates = GetMostConstrainedVariable(sudoku);
					currentStep = new Step(selectedCoordinates, 0);
				}
				else
				{
					currentStep = stepsTaken.Pop();
					selectedCoordinates = currentStep.coordinates;
				}
				// Select random cell from most constrained cells
				Cell selectedCell = sudoku.GetCell(selectedCoordinates);
				// Select random value from cell's domain
				// Assign value to cell
				int selectedValue;
				try
				{
					selectedValue = GetLeastConstrainedValue(sudoku, selectedCoordinates, currentStep.bannedValues);
				}
				catch
				{
					// No valid value could be assigned, backtrack
					currentStep = stepsTaken.Peek();
					sudoku.revertAssignment(currentStep);
					backtrack = true;
					// Add value to banned values for this cell
					currentStep.bannedValues.Add(currentStep.value);
					continue;
				}
				List<Coordinates> toUpdate = sudoku.UpdateSpace(selectedCoordinates, selectedValue);

				// Update domains of affected cells
				List<Coordinates> updatedCellCoordinates = new List<Coordinates>();
				bool isPossibleAssignment = true;
				foreach (Coordinates affectedCoordinates in toUpdate)
				{
					Cell affectedCell = sudoku.GetCell(affectedCoordinates);
					// Remove assigned value from the domains nearby(forward-checking)
					if (affectedCell.possibleValues.Contains(selectedValue))
					{
						updatedCellCoordinates.Add(affectedCoordinates);
						affectedCell.possibleValues.Remove(selectedValue);
					}

					// What to do if domain is empty?
					if (affectedCell.possibleValues.Count == 0)
					{
						isPossibleAssignment = false;
						break;
					}
				}
				currentStep.affectedCoordinates = updatedCellCoordinates;

				// Removes last assigment, reverts domains of affected cells
				if (isPossibleAssignment == false)
				{
					sudoku.revertAssignment(currentStep);
					// Add value to banned values for this cell
					currentStep.bannedValues.Add(selectedValue);
					backtrack = true;
				}
				else
				{
					backtrack = false;
				}
				stepsTaken.Push(currentStep);
			}
			return sudoku;
		}
	}
}
