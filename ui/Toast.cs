using System;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.scenes;

namespace sharpdungeon.ui
{
    public class Toast : Component
    {
        private const float MarginHor = 2;
        private const float MarginVer = 2;

        protected internal NinePatch Bg;
        protected internal SimpleButton Close;
        protected internal BitmapText BitmapText;

        public Toast(string text)
        {
            Text(text);

            _Width = BitmapText.Width + Close.Width + Bg.MarginHor() + MarginHor * 3;
            _Height = Math.Max(BitmapText.Height, Close.Height) + Bg.MarginVer() + MarginVer * 2;
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            Bg = Chrome.Get(Chrome.Type.TOAST_TR);
            Add(Bg);

            Close = new SimpleButton(Icons.CLOSE.Get());
            Close.ClickAction = OnClose;
            Add(Close);

            BitmapText = PixelScene.CreateText(8);
            Add(BitmapText);
        }

        protected override void Layout()
        {
            base.Layout();

            Bg.X = X;
            Bg.Y = Y;
            Bg.Size(Width, Height);

            Close.SetPos(Bg.X + Bg.Width - Bg.MarginHor() / 2 - MarginHor - Close.Width, Y + (Height - Close.Height) / 2);

            BitmapText.X = Close.Left() - MarginHor - BitmapText.Width;
            BitmapText.Y = Y + (Height - BitmapText.Height) / 2;
            PixelScene.Align(BitmapText);
        }

        public virtual void Text(string txt)
        {
            BitmapText.Text(txt);
            BitmapText.Measure();
        }

        public Action CloseAction;

        protected internal virtual void OnClose()
        {
            if (CloseAction != null)
                CloseAction();
        }
    }
}