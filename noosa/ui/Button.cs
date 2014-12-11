using System;
using pdsharp.input;

namespace pdsharp.noosa.ui
{
    public class Button : Component
    {
        public static float LongClick = 1f;

        protected HotArea hotArea;

        protected internal static bool Pressed;
        protected internal static float PressTime;

        protected internal static bool Processed;

        protected override void CreateChildren()
        {
            hotArea = new HotArea(this);
            Add(hotArea);
        }
        
        public override void Update()
        {
            base.Update();

            hotArea.Active = Visible;

            if (!Pressed)
                return;

            if (!((PressTime += Game.Elapsed) >= LongClick))
                return;

            Pressed = false;
            if (!OnLongClick())
                return;

            hotArea.Reset();
            Processed = true;
            OnTouchUp();

            Game.Vibrate(50);
        }

        public Action<Button> TouchDownAction { get; set; }

        protected virtual void OnTouchDown()
        {
            if (TouchDownAction != null)
                TouchDownAction(this);
        }

        public Action<Button> TouchUpAction { get; set; }
        protected virtual void OnTouchUp()
        {
            if (TouchUpAction != null)
                TouchUpAction(this);
        }

        public Action<Button> ClickAction { get; set; }
        public int Index { get; set; }

        protected virtual void OnClick()
        {
            if (ClickAction != null)
                ClickAction(this);
        }

        public Func<Button, bool> LongClickAction { get; set; }

        protected virtual bool OnLongClick()
        {
            if (LongClickAction != null)
                return LongClickAction(this);

            return false;
        }

        protected override void Layout()
        {
            hotArea.X = X;
            hotArea.Y = Y;

            hotArea.Width = _Width;
            hotArea.Height = _Height;
        }


        protected class HotArea : TouchArea
        {
            //public new float Width;
            //public new float Height;

            public HotArea(Visual target)
                : base(target)
            {
            }

            public HotArea(float x, float y, float width, float height)
                : base(x, y, width, height)
            {
            }

            public HotArea(Button button)
                : base(button.X, button.Y, button.Width, button.Height)
            {
                Button = button;
            }

            public Button Button { get; set; }

            protected override void OnTouchDown(Touch touch)
            {
                Pressed = true;
                PressTime = 0;
                Processed = false;
                Button.OnTouchDown();
            }
            protected override void OnTouchUp(Touch touch)
            {
                Pressed = false;
                Button.OnTouchUp();
            }
            protected override void OnClick(Touch touch)
            {
                if (!Processed)
                    Button.OnClick();
            }
        }
    }
}