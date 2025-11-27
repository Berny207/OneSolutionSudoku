using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
namespace OneSolutionSudoku
{
	internal class CSPAlgorithms
	{
		static Random random = new Random();
		public static List<int> GetLeastConstrainedValues(Sudoku sudoku, Coordinates selectedCoordinates, List<int> bannedValues)
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
			return candidateValues;
		}
		public static List<Coordinates> GetMostConstrainedVariables(Sudoku sudoku)
		{
			List<Coordinates> mostConstrainedCells = new List<Coordinates>();
			int minDomainSize = 10; // More than max possible domain size
			for (int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					Coordinates coordinates = new Coordinates(row, column);
					if(sudoku.GetCell(coordinates).value != 0)
					{
						continue;
					}
					if (sudoku.GetCell(coordinates).possibleValues.Count < minDomainSize)
					{
						minDomainSize = sudoku.GetCell(coordinates).possibleValues.Count;
						mostConstrainedCells.Clear();
						mostConstrainedCells.Add(coordinates);
					}
					else if (sudoku.GetCell(coordinates).possibleValues.Count == minDomainSize)
					{
						mostConstrainedCells.Add(coordinates);
					}
				}
			}
			return mostConstrainedCells;
		}
		/// <summary>
		/// Tries to assign value to cell with lookahead<br/>
		/// If failed, reverts assignment and returns false and nill<br/>
		/// If succeeded, returns true and list of affected coordinates<br/>
		/// </summary>
		/// <param name="sudoku"></param>
		/// <param name="coordinates"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		internal static (bool, List<Coordinates>?) AssignValueWithLookahead(Sudoku sudoku, Coordinates coordinates, int value)
		{
			sudoku.SetCell(coordinates, value);
			List<Coordinates> toUpdate = sudoku.FilterOutFullCells(sudoku.GetSpaceNeighbours(coordinates));
			List<Coordinates> updatedCellCoordinates = new List<Coordinates>();
			foreach (Coordinates updatingCoordinates in toUpdate)
			{
				Cell cell = sudoku.GetCell(updatingCoordinates);
				if (cell.possibleValues.Contains(value))
				{
					cell.possibleValues.Remove(value);
					updatedCellCoordinates.Add(updatingCoordinates);
				}
				if (cell.possibleValues.Count == 0)
				{
					// Assigment failed, revert changes
					Step returnStep = new Step(coordinates, value);
					returnStep.affectedCoordinates = updatedCellCoordinates;
					sudoku.revertAssignment(returnStep);
					return (false, null);
				}
			}
			return (true, updatedCellCoordinates);
		}
	}
}
