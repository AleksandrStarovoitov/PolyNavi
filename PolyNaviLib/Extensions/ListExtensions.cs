using System.Collections.Generic;

namespace PolyNaviLib.Extensions
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