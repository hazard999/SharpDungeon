using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.ui;
using sharpdungeon.scenes;

namespace sharpdungeon.ui
{
    public class RedButton : Button
    {
        protected NinePatch Bg;
        protected BitmapText InternalText;
        protected Image InternalIcon;

        public RedButton(string label)
        {
            InternalText.Text(label);
            InternalText.Measure();
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            Bg = Chrome.Get(Chrome.Type.BUTTON);
            Add(Bg);

            InternalText = PixelScene.CreateText(9);
            Add(InternalText);
        }

        protected override void Layout()
        {

            base.Layout();

            Bg.X = X;
            Bg.Y = Y;
            Bg.Size(Width, Height);

            InternalText.X = X + (int)(Width - InternalText.Width) / 2;
            InternalText.Y = Y + (int)(Height - InternalText.BaseLine()) / 2;

            if (InternalIcon == null)
                return;

            InternalIcon.X = X + InternalText.X - InternalIcon.Width - 2;
            InternalIcon.Y = Y + (Height - InternalIcon.Height) / 2;
        }

        protected override void OnTouchDown()
        {
            Bg.Brightness(1.2f);
            Sample.Instance.Play(Assets.SND_CLICK);
        }

        protected override void OnTouchUp()
        {
            Bg.ResetColor();
        }

        public virtual void Enable(bool value)
        {
            Active = value;
            InternalText.Alpha(value ? 1.0f : 0.3f);
        }

        public virtual void Text(string value)
        {
            InternalText.Text(value);
            InternalText.Measure();
            Layout();
        }

        public virtual void Icon(Image icon)
        {
            if (InternalIcon != null)
                Remove(InternalIcon);

            InternalIcon = icon;

            if (InternalIcon == null)
                return;

            Add(InternalIcon);
            Layout();
        }

        public virtual float ReqWidth()
        {
            return InternalText.Width + 4;
        }

        public virtual float ReqHeight()
        {
            return InternalText.BaseLine() + 4;
        }
    }
}