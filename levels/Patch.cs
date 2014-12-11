namespace sharpdungeon.levels
{
	public class Patch
	{
		private static bool[] _cur = new bool[Level.Length];
		private static bool[] _off = new bool[Level.Length];

		public static bool[] Generate(float seed, int nGen)
		{
			const int w = Level.Width;
			const int h = Level.Height;

		    for (var i = 0; i < Level.Length; i++)
		        _off[i] = pdsharp.utils.Random.Float() < seed;

		    for (var i=0; i < nGen; i++)
			{
			    for (var y = 1; y < h - 1; y++)
			        for (var x = 1; x < w - 1; x++)
			        {
			            var pos = x + y*w;
			            var count = 0;
			            if (_off[pos - w - 1])
			                count++;
			            if (_off[pos - w])
			                count++;
			            if (_off[pos - w + 1])
			                count++;
			            if (_off[pos - 1])
			                count++;
			            if (_off[pos + 1])
			                count++;
			            if (_off[pos + w - 1])
			                count++;
			            if (_off[pos + w])
			                count++;
			            if (_off[pos + w + 1])
			                count++;

			            if (!_off[pos] && count >= 5)
			                _cur[pos] = true;
			            else if (_off[pos] && count >= 4)
			                _cur[pos] = true;
			            else
			                _cur[pos] = false;
			        }

			    var tmp = _cur;
				_cur = _off;
				_off = tmp;
			}

			return _off;
		}
	}
}