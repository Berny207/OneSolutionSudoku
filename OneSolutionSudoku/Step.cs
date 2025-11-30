using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSolutionSudoku
{
	internal class Step
	{
		public Coordinates coordinates;
		public int value;
		internal List<int> bannedValues;
		internal List<Coordinates> affectedCoordinates;

		internal Step(Coordinates coordinates, int value)
		{
			this.coordinates = coordinates;
			this.value = value;
			this.bannedValues = new List<int>();
			this.affectedCoordinates = new List<Coordinates>();
		}
		internal Step()
		{
			this.coordinates = new Coordinates(0, 0);
			this.value = -1;
			this.bannedValues = new List<int>();
			this.affectedCoordinates = new List<Coordinates>();
		}
	}
	internal class Elimination_Step
	{
		public Coordinates coordinates;
		public int value;
		internal List<Coordinates> availibleCoordinates;

		internal Elimination_Step(Coordinates coordinates, int value)
		{
			this.coordinates = coordinates;
			this.value = value;
			this.availibleCoordinates = new List<Coordinates>();
		}
		internal Elimination_Step()
		{
			this.coordinates = new Coordinates(0, 0);
			this.value = -1;
			this.availibleCoordinates = new List<Coordinates>();
		}
	}
}
