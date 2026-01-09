using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSolutionSudoku
{
	internal class Step
	{
		public Coordinates Coordinates;
		public int Value;
		internal List<int> bannedValues;
		internal Multimap<Coordinates, int> affectedCoordinates;

		internal Step(Coordinates coordinates, int value)
		{
			this.Coordinates = coordinates;
			this.Value = value;
			this.bannedValues = new List<int>();
			this.affectedCoordinates = new Multimap<Coordinates, int>();
		}
		internal Step()
		{
			this.Coordinates = new Coordinates(0, 0);
			this.Value = -1;
			this.bannedValues = new List<int>();
			this.affectedCoordinates = new Multimap<Coordinates, int>();
		}
	}
	internal class EliminationStep
	{
		public Coordinates Coordinates;
		public int Value;
		internal List<Coordinates> availibleCoordinates;

		internal EliminationStep(Coordinates coordinates, int value)
		{
			this.Coordinates = coordinates;
			this.Value = value;
			this.availibleCoordinates = new List<Coordinates>();
		}
		internal EliminationStep()
		{
			this.Coordinates = new Coordinates(0, 0);
			this.Value = -1;
			this.availibleCoordinates = new List<Coordinates>();
		}
	}
}
