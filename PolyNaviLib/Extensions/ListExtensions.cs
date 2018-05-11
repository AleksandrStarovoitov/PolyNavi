using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolyNaviLib
{
	public static class ListExtensions
	{
		public static T First<T>(this List<T> self)
		{
			return self[0];
		}

		public static T Last<T>(this List<T> self)
		{
			return self[self.Count - 1];
		}
	}
}