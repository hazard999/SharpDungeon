using System.Collections.Generic;

namespace pdsharp.utils
{
    public class SparseArray<T> : Dictionary<int, T>
    {
        public virtual int[] KeyArray()
        {
            var size = Count;
            var array = new int[size];
            var i = 0;
            foreach (var key in Keys)
            {
                array[i] = key;
                i++;
            }

            return array;
        }

        public new T this[int key]
        {
            get
            {
                return ContainsKey(key) ? base[key] : default(T);
            }
            set { base[key] = value; }
        }
    }
}