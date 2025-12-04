using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSolutionSudoku
{
	internal class Multimap<T1, T2>
	{
		public Dictionary<T1, List<T2>> data = new Dictionary<T1, List<T2>>();

		public void Add(T1 key, T2 value)
		{
			if (!data.ContainsKey(key))
			{
				data.Add(key, new List<T2> { value });
			}
			else
			{
				data[key].Add(value);
			}
		}
		public void Clear(T1 key)
		{
			data.Remove(key);
		}
		public void Remove(T1 key, T2 value)
		{
			if (data[key].Contains(value))
			{
				data[key].Remove(value);
			}
			if (data[key].Count == 0)
			{
				data.Remove(key);
			}
		}

		public Multimap()
		{
			this.data = new Dictionary<T1, List<T2>>();
		}
	}
}
