using System.Collections.Generic;
using Android.Views;
using pdsharp.utils;

namespace pdsharp.input
{
    public static class Touchscreen
    {
        public static Signal<Touch> Event = new Signal<Touch>(true);

        public static Dictionary<int?, Touch> Pointers = new Dictionary<int?, Touch>();

        public static float X;
        public static float Y;
        public static bool Touched;

        public static void ProcessTouchEvents(List<MotionEvent> events)
        {
            foreach (var e in events)
            {
                Touch touch;

                switch (e.ActionMasked)
                {
                    case MotionEventActions.Down:
                        Touched = true;
                        touch = new Touch(e, 0);
                        Pointers.Add(e.GetPointerId(0), touch);
                        Event.Dispatch(touch);
                        break;

                    case MotionEventActions.PointerDown:
                        int index = e.ActionIndex;
                        touch = new Touch(e, index);
                        Pointers.Add(e.GetPointerId(index), touch);
                        Event.Dispatch(touch);
                        break;

                    case MotionEventActions.Move:
                        int count = e.PointerCount;
                        for (int j = 0; j < count; j++)
                        {
                            Pointers[e.GetPointerId(j)].Update(e, j);
                        }
                        Event.Dispatch(null);
                        break;

                    case MotionEventActions.PointerUp:
                        var upId = e.GetPointerId(e.ActionIndex);
                        var pointer = Pointers[upId];
                        Event.Dispatch(pointer.Up());
                        Pointers.Remove(upId);
                        break;

                    case MotionEventActions.Up:
                        Touched = false;
                        var pointerUpId = e.GetPointerId(0);
                        var pointerUp = Pointers[pointerUpId];
                        Pointers.Remove(pointerUpId);
                        Event.Dispatch(pointerUp.Up());                        

                        break;

                }

                e.Recycle();
            }
        }

    }
}