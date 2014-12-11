using System.Collections.Generic;
using System.Linq;

namespace pdsharp.utils
{
    public class Signal<T>
    {
        private readonly List<IListener<T>> _listeners = new List<IListener<T>>();

        private bool _canceled;

        private readonly bool _stackMode;

        public Signal()
            : this(false)
        {
        }

        public Signal(bool stackMode)
        {
            _stackMode = stackMode;
        }

        public virtual void Add(IListener<T> listener)
        {
            if (_listeners.Contains(listener))
                return;

            if (_stackMode)
                _listeners.Insert(0, listener);
            else
                _listeners.Add(listener);
        }

        public virtual void Remove(IListener<T> listener)
        {
            _listeners.Remove(listener);
        }

        public virtual void RemoveAll()
        {
            _listeners.Clear();
        }

        public virtual void Replace(IListener<T> listener)
        {
            RemoveAll();
            Add(listener);
        }

        public virtual int NumListeners()
        {
            return _listeners.Count;
        }

        public virtual void Dispatch(T t)
        {
            var list = _listeners.ToArray();

            _canceled = false;
            foreach (var listener in list)
            {
                if (!_listeners.Contains(listener))
                    continue;

                listener.OnSignal(t);

                if (_canceled)
                    return;
            }
        }

        public virtual void Cancel()
        {
            _canceled = true;
        }
    }
}