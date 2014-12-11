using System;

namespace pdsharp.noosa.tweeners
{
    public abstract class Tweener : Gizmo
    {
        public Gizmo Target;

        public float Interval;
        public float Elapsed;

        public IListener Listener;

        protected Tweener(Gizmo target, float interval)
        {

            Target = target;
            Interval = interval;

            Elapsed = 0;
        }

        public override void Update()
        {
            Elapsed += Game.Elapsed;
            if (Elapsed >= Interval)
            {
                UpdateValues(1);
                OnComplete();
                Kill();
            }
            else
                UpdateValues(Elapsed/Interval);
        }

        public Action<Tweener> CompleteAction;

        protected internal virtual void OnComplete()
        {
            if (Listener != null)
                Listener.OnComplete(this);

            if (CompleteAction != null)
                CompleteAction(this);
        }

        protected abstract void UpdateValues(float progress);
    }
}