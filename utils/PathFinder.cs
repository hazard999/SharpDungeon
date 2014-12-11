using System.Collections.Generic;
using Java.Util;

namespace pdsharp.utils
{
    public class PathFinder
    {
        public static int[] Distance;

        private static bool[] _goals;
        private static int[] _queue;

        private static int _size;

        private static int[] _dir;

        public static void SetMapSize(int width, int height)
        {
            var size = width * height;

            if (_size == size) 
                return;

            _size = size;
            Distance = new int[size];
            _goals = new bool[size];
            _queue = new int[size];

            _dir = new[] { -1, +1, -width, +width, -width - 1, -width + 1, +width - 1, +width + 1 };
        }

        public static Path Find(int from, int to, bool[] passable)
        {
            if (!BuildDistanceMap(from, to, passable))
                return null;

            var result = new Path();
            var s = from;

            // From the starting position we are moving downwards, 
            // until we reach the ending point
            do
            {
                var minD = Distance[s];
                var mins = s;

                foreach (var direction in _dir)
                {
                    var n = s + direction;

                    var thisD = Distance[n];
                    if (thisD >= minD) 
                        continue;

                    minD = thisD;
                    mins = n;
                }
                s = mins;
                result.Add(s);
            } while (s != to);

            return result;
        }

        public static int GetStep(int fromPosition, int to, bool[] passable)
        {
            if (!BuildDistanceMap(fromPosition, to, passable))
                return -1;

            // From the starting position we are making one step downwards
            var minD = Distance[fromPosition];
            var best = fromPosition;

            foreach (var direction in _dir)
            {
                int step;
                int stepD;
                if ((stepD = Distance[step = fromPosition + direction]) >= minD) 
                    continue;

                minD = stepD;
                best = step;
            }

            return best;
        }

        public static int GetStepBack(int cur, int from, bool[] passable)
        {
            var d = BuildEscapeDistanceMap(cur, from, 2f, passable);
            for (var i = 0; i < _size; i++)
                _goals[i] = Distance[i] == d;

            if (!BuildDistanceMap(cur, _goals, passable))
                return -1;

            var s = cur;

            // From the starting position we are making one step downwards
            var minD = Distance[s];
            var mins = s;

            foreach (var direction in _dir)
            {
                var n = s + direction;
                var thisD = Distance[n];

                if (thisD >= minD) 
                    continue;

                minD = thisD;
                mins = n;
            }

            return mins;
        }

        private static bool BuildDistanceMap(int fromPosition, int to, bool[] passable)
        {
            if (fromPosition == to)
                return false;

            Arrays.Fill(Distance, int.MaxValue);

            var pathFound = false;

            var head = 0;
            var tail = 0;

            // Add to queue
            _queue[tail++] = to;
            Distance[to] = 0;

            while (head < tail)
            {
                // Remove from queue
                var step = _queue[head++];
                if (step == fromPosition)
                {
                    pathFound = true;
                    break;
                }

                var nextDistance = Distance[step] + 1;

                foreach (var direction in _dir)
                {
                    var n = step + direction;
                    if (n != fromPosition && (n < 0 || n >= _size || !passable[n] || (Distance[n] <= nextDistance))) 
                        continue;

                    // Add to queue
                    _queue[tail++] = n;
                    Distance[n] = nextDistance;
                }
            }

            return pathFound;
        }

        public static void BuildDistanceMap(int to, bool[] passable, int limit)
        {
            Arrays.Fill(Distance, int.MaxValue);

            var head = 0;
            var tail = 0;

            // Add to queue
            _queue[tail++] = to;
            Distance[to] = 0;

            while (head < tail)
            {
                // Remove from queue
                var step = _queue[head++];

                var nextDistance = Distance[step] + 1;
                if (nextDistance > limit)
                    return;

                foreach (var direction in _dir)
                {
                    var n = step + direction;
                    if (n < 0 || n >= _size || !passable[n] || (Distance[n] <= nextDistance)) 
                        continue;

                    // Add to queue
                    _queue[tail++] = n;
                    Distance[n] = nextDistance;
                }
            }
        }

        private static bool BuildDistanceMap(int fromPosition, bool[] to, bool[] passable)
        {
            if (to[fromPosition])
                return false;

            Arrays.Fill(Distance, int.MaxValue);

            var pathFound = false;

            var head = 0;
            var tail = 0;

            // Add to queue
            for (var i = 0; i < _size; i++)
            {
                if (!to[i]) 
                    continue;

                _queue[tail++] = i;
                Distance[i] = 0;
            }

            while (head < tail)
            {
                // Remove from queue
                var step = _queue[head++];
                if (step == fromPosition)
                {
                    pathFound = true;
                    break;
                }

                var nextDistance = Distance[step] + 1;

                foreach (var direction in _dir)
                {
                    var n = step + direction;
                    
                    if (n != fromPosition && (n < 0 || n >= _size || !passable[n] || (Distance[n] <= nextDistance))) 
                        continue;
                    
                    // Add to queue
                    _queue[tail++] = n;
                    Distance[n] = nextDistance;
                }
            }

            return pathFound;
        }

        private static int BuildEscapeDistanceMap(int cur, int from, float factor, bool[] passable)
        {
            Arrays.Fill(Distance, int.MaxValue);

            var destDist = int.MaxValue;

            var head = 0;
            var tail = 0;

            // Add to queue
            _queue[tail++] = from;
            Distance[from] = 0;

            var dist = 0;

            while (head < tail)
            {

                // Remove from queue
                var step = _queue[head++];
                dist = Distance[step];

                if (dist > destDist)
                    return destDist;

                if (step == cur)
                    destDist = (int) (dist*factor) + 1;

                var nextDistance = dist + 1;

                foreach (int direction in _dir)
                {
                    var n = step + direction;
                    
                    if (n < 0 || n >= _size || !passable[n] || Distance[n] <= nextDistance) 
                        continue;

                    // Add to queue
                    _queue[tail++] = n;
                    Distance[n] = nextDistance;
                }
            }

            return dist;
        }

        private static void BuildDistanceMap(int to, bool[] passable)
        {
            Arrays.Fill(Distance, int.MaxValue);

            int head = 0;
            int tail = 0;

            // Add to queue
            _queue[tail++] = to;
            Distance[to] = 0;

            while (head < tail)
            {

                // Remove from queue
                int step = _queue[head++];
                int nextDistance = Distance[step] + 1;

                for (int i = 0; i < _dir.Length; i++)
                {

                    int n = step + _dir[i];
                    if (n >= 0 && n < _size && passable[n] && (Distance[n] > nextDistance))
                    {
                        // Add to queue
                        _queue[tail++] = n;
                        Distance[n] = nextDistance;
                    }

                }
            }
        }

        public class Path : List<int?>
        {
        }
    }
}