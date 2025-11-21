using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OneSolutionSudoku
{
	class Step
	{
		public Coordinates coordinates;
		public int value;
		public List<int> bannedValues;
		public List<Coordinates> affectedCellCoordinates;

		public Step(Coordinates coordinates, int value)
		{
			this.coordinates = coordinates;
			this.value = value;
			this.bannedValues = new List<int>();
			this.affectedCellCoordinates = new List<Coordinates>();
		}
	}
	internal static class BaseplateGenerator
	{
		public static Sudoku GenerateValidSudoku()
		{
			Sudoku sudoku = new Sudoku();
			// Create a stack for our steps taken so we can backtrack
			Stack<Step> stepsTaken = new Stack<Step>();

			Random random = new Random();

			// Some looping should happen here


			// Get list of most constrained cells
			List<Coordinates> mostConstrainedCells = new List<Coordinates>();
			int minDomainSize = 10; // More than max possible domain size
			for (int row=0; row<9; row++)
			{
				for(int column=0; column<9; column++)
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
			// Select random cell from most constrained cells
			Coordinates selectedCoordinates = mostConstrainedCells[random.Next(mostConstrainedCells.Count)];
			Cell selectedCell = sudoku.GetCell(selectedCoordinates);

			// Select random value from cell's domain
			int minimalConstraintValue = 20;
			List<int> candidateValues = new List<int>();
			foreach (int value in selectedCell.possibleValues)
			{
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
			int randomValue = candidateValues[random.Next(candidateValues.Count)];

			// Assign value to cell
			List<Coordinates> toUpdate = sudoku.UpdateSpace(selectedCoordinates, randomValue);

			// Update domains of affected cells
			List<Coordinates> updatedCellCoordinates = new List<Coordinates>();
			bool isPossibleAssignment = true;
			foreach(Coordinates affectedCoordinates in toUpdate)
			{
				// Remove assigned value from the domains nearby(forward-checking)
				if (sudoku.GetCell(affectedCoordinates).possibleValues.Contains(randomValue))
				{
					updatedCellCoordinates.Add(affectedCoordinates);
					sudoku.GetCell(affectedCoordinates).possibleValues.Remove(randomValue);
				}

				// What to do if domain is empty?
				if(sudoku.GetCell(affectedCoordinates).possibleValues.Count == 0)
				{
					isPossibleAssignment = false;
					break;
				}
			}
			Step newStep = new Step(selectedCoordinates, randomValue);
			newStep.affectedCellCoordinates = updatedCellCoordinates;
			stepsTaken.Push(newStep);

			// Invalid assignment, need to backtrack
			if (isPossibleAssignment == false)
			{
				Step lastStep = stepsTaken.Pop();
				sudoku.UpdateSpace(lastStep.coordinates, 0);
				foreach(Coordinates updatedCellCoordinate in lastStep.affectedCellCoordinates)
				{
					sudoku.GetCell(updatedCellCoordinate).possibleValues.Add(randomValue);
				}
			}
			return sudoku;
		}
	}
}
