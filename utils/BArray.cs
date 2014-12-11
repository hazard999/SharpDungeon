namespace sharpdungeon.utils
{

	public class BArray
	{

		public static bool[] and(bool[] a, bool[] b, bool[] result)
		{

			int length = a.Length;

			if (result == null)
			{
				result = new bool[length];
			}

			for (int i=0; i < length; i++)
			{
				result[i] = a[i] && b[i];
			}

			return result;
		}

		public static bool[] or(bool[] a, bool[] b, bool[] result)
		{

			int length = a.Length;

			if (result == null)
			{
				result = new bool[length];
			}

			for (int i=0; i < length; i++)
			{
				result[i] = a[i] || b[i];
			}

			return result;
		}

		public static bool[] not(bool[] a, bool[] result)
		{

			int length = a.Length;

			if (result == null)
			{
				result = new bool[length];
			}

			for (int i=0; i < length; i++)
			{
				result[i] = !a[i];
			}

			return result;
		}

		public static bool[] @is(int[] a, bool[] result, int v1)
		{

			int length = a.Length;

			if (result == null)
			{
				result = new bool[length];
			}

			for (int i=0; i < length; i++)
			{
				result[i] = a[i] == v1;
			}

			return result;
		}

		public static bool[] isOneOf(int[] a, bool[] result, params int[] v)
		{

			int length = a.Length;
			int nv = v.Length;

			if (result == null)
			{
				result = new bool[length];
			}

			for (int i=0; i < length; i++)
			{
				result[i] = false;
				for (int j=0; j < nv; j++)
				{
					if (a[i] == v[j])
					{
						result[i] = true;
						break;
					}
				}
			}

			return result;
		}

		public static bool[] isNot(int[] a, bool[] result, int v1)
		{

			int length = a.Length;

			if (result == null)
			{
				result = new bool[length];
			}

			for (int i=0; i < length; i++)
			{
				result[i] = a[i] != v1;
			}

			return result;
		}

		public static bool[] isNotOneOf(int[] a, bool[] result, params int[] v)
		{

			int length = a.Length;
			int nv = v.Length;

			if (result == null)
			{
				result = new bool[length];
			}

			for (int i=0; i < length; i++)
			{
				result[i] = true;
				for (int j=0; j < nv; j++)
				{
					if (a[i] == v[j])
					{
						result[i] = false;
						break;
					}
				}
			}

			return result;
		}
	}

}