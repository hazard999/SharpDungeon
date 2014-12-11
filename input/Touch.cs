using Android.Views;
using pdsharp.utils;

namespace pdsharp.input
{
    public class Touch
    {
        public PointF Start;
        public PointF Current;
        public bool Down;

        public Touch(MotionEvent e, int index)
        {
            var x = e.GetX(index);
            var y = e.GetY(index);

            Start = new PointF(x, y);
            Current = new PointF(x, y);

            Down = true;
        }

        public virtual void Update(MotionEvent e, int index)
        {
            Current.Set(e.GetX(index), e.GetY(index));
        }

        public virtual Touch Up()
        {
            Down = false;
            return this;
        }
    }
}