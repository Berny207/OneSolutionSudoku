using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
namespace OneSolutionSudoku
{
	internal class CSPAlgorithms
	{
		static Random random = new Random();

		/// <summary>
		/// Returns list of least constrained values
		/// </summary>
		/// <param name="sudoku"></param>
		/// <param name="selectedCoordinates"></param>
		/// <param name="bannedValues"></param>
		/// <returns></returns>
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
		/// <summary>
		/// Returns list of variables with least possible numbers
		/// </summary>
		/// <param name="sudoku"></param>
		/// <returns></returns>
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
		internal static (bool, Multimap<Coordinates, int>) AssignValue(Sudoku sudoku, Coordinates coordinates, int value)
		{
			(bool result, Multimap<Coordinates, int> updatedCoordinates) = EnforceArcConsistency(sudoku, coordinates, value);
			if (!result)
			{
				foreach(var kvp in updatedCoordinates.data)
				{
					foreach (int affectedValue in kvp.Value)
					{
						sudoku.GetCell(kvp.Key).possibleValues.Add(affectedValue);
					}
				}
				return (false, updatedCoordinates);
			}

			sudoku.SetCell(coordinates, value);

			return (true, updatedCoordinates);
		}
		/// <summary>
		/// Recursively enforced arc consistency on cells
		/// </summary>
		/// <param name="sudoku"></param>
		/// <param name="coordinates"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		internal static (bool, Multimap<Coordinates, int>) EnforceArcConsistency(Sudoku sudoku, Coordinates coordinates, int value)
		{
			Dictionary<Coordinates, int> cellsToBeEnforcedAgain = new Dictionary<Coordinates, int>();
			Multimap<Coordinates, int> updatedCoordinates = new Multimap<Coordinates, int>();
			List<Coordinates> listOfCoordinates = sudoku.GetSpaceNeighbours(coordinates);
			foreach (Coordinates updatingCoordinates in listOfCoordinates)
			{
				if(sudoku.GetCell(updatingCoordinates).value != 0)
				{
					continue;
				}
				List<int> cellPossibleValues = sudoku.GetCell(updatingCoordinates).possibleValues;
				if (cellPossibleValues.Contains(value))
				{
					updatedCoordinates.Add(updatingCoordinates, value);
					cellPossibleValues.Remove(value);
					if (cellPossibleValues.Count == 0)
					{
						return (false, updatedCoordinates);
						// We're fucked
					}
					if (cellPossibleValues.Count == 1)
					{
						// Enforce arc consistency again
						cellsToBeEnforcedAgain.Add(updatingCoordinates, cellPossibleValues[0]);
					}
				}
			}
			if(cellsToBeEnforcedAgain.Count > 0)
			{
				foreach (Coordinates coordinatesToEnforce in cellsToBeEnforcedAgain.Keys)
				{
					(bool result, Multimap<Coordinates, int> additionalUdpatedCoordinates) = EnforceArcConsistency(sudoku, coordinatesToEnforce, cellsToBeEnforcedAgain[coordinatesToEnforce]);
					// Merge additional updated coordinates
					foreach(var kvp in additionalUdpatedCoordinates.data)
					{
						foreach (int affectedValue in kvp.Value)
						{
							updatedCoordinates.Add(kvp.Key, affectedValue);
						}
					}
					if (!result)
					{
						return(false, updatedCoordinates);
					}
				}
			}
			return (true, updatedCoordinates);
		}
	}
}
