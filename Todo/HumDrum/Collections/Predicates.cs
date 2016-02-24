using System;
using System.Collections.Generic;

using HumDrum.Collections;

namespace Todo
{
	public static class Predicates
	{
		public static bool Any(IEnumerable<bool> list)
		{
			if (list.Length().Equals(0))
				return true;
			else
				return list.Get (0) || Any (list.Tail ());
		}

		public static bool All(IEnumerable<bool> list)
		{
			if (list.Length().Equals(0))
				return true;
			else
				return list.Get (0) && Any (list.Tail ());
		}
	}
}

