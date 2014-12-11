using pdsharp.input;
using pdsharp.utils;
using System;

namespace pdsharp.noosa
{
    public class TouchArea : Visual, IListener<Touch>
    {
        // Its target can be toucharea itself
        public Visual Target;

        protected internal Touch Touch = null;
        
        public TouchArea(Visual target)
            : base(0, 0, 0, 0)
        {
            Target = target;

            Touchscreen.Event.Add(this);
        }

        public TouchArea(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
            Target = this;

            Visible = false;

            Touchscreen.Event.Add(this);
        }

        public void OnSignal(Touch touch)
        {
            if (!Active)
                return;

            var hit = touch != null && Target.OverlapsScreenPoint((int)touch.Start.X, (int)touch.Start.Y);

            if (hit)
            {
                Touchscreen.Event.Cancel();

                if (touch.Down)
                {
                    if (Touch == null)
                        Touch = touch;

                    OnTouchDown(touch);
                }
                else
                {
                    OnTouchUp(touch);

                    if (Touch != touch)
                        return;

                    Touch = null;
                    OnClick(touch);
                }
            }
            else
            {
                if (touch == null && Touch != null)
                    OnDrag(Touch);
                else
                    if (Touch != null && touch != null && !touch.Down)
                    {
                        OnTouchUp(touch);
                        Touch = null;
                    }
            }
        }

        public Action<Touch> TouchDownAction { get; set; }

        protected virtual void OnTouchDown(Touch touch)
        {
            if (TouchDownAction != null)
                TouchDownAction(touch);
        }

        public Action<Touch> TouchUpAction { get; set; }

        protected virtual void OnTouchUp(Touch touch)
        {
            if (TouchUpAction != null)
                TouchUpAction(touch);
        }

        public Action<Touch> ClickAction { get; set; }

        protected virtual void OnClick(Touch touch)
        {
            if (ClickAction != null)
                ClickAction(touch);
        }

        protected virtual void OnDrag(Touch touch)
        {
        }

        public virtual void Reset()
        {
            Touch = null;
        }

        public override void Destroy()
        {
            Touchscreen.Event.Remove(this);
            base.Destroy();
        }
    }
}