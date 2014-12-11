using System.Linq;
using System.Collections.Generic;

namespace pdsharp.utils
{
    public class Graph
    {
        public static void SetPrice<T>(IList<T> nodes, int value) where T : Node
        {
            foreach (var node in nodes)
                node.Price(value);
        }

        public static void BuildDistanceMap<T>(ICollection<T> nodes, Node focus) where T : Node
        {
            foreach (var node in nodes)
                node.Distance(int.MaxValue);

            var queue = new List<Node>();

            focus.Distance(0);
            queue.Add(focus);

            while (queue.Count != 0)
            {
                var node = queue.First();
                queue.Remove(node);

                var distance = node.Distance();
                var price = node.Price();

                foreach (var edge in node.Edges())
                {
                    if (edge.Distance() <= distance + price)
                        continue;

                    queue.Add(edge);
                    edge.Distance(distance + price);
                }
            }
        }

        public static IList<T> BuildPath<T>(ICollection<T> nodes, T from, T to) where T : Node
        {
            var path = new List<T>();

            var room = from;
            while (room.GetHashCode() != to.GetHashCode())
            {
                var min = room.Distance();
                var next = default(T);

                var edges = room.Edges();

                foreach (var edge in edges)
                {
                    var distance = edge.Distance();
                    if (distance >= min) 
                        continue;

                    min = distance;
                    next = (T)edge;
                }

                if (next == null)
                    return null;

                path.Add(next);
                room = next;
            }

            return path;
        }
    }

    public interface Node
    {
        int Distance();

        void Distance(int value);

        int Price();

        void Price(int value);

        ICollection<Node> Edges();
    }
}