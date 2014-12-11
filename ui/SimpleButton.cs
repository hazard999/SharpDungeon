using pdsharp.noosa;
using pdsharp.noosa.ui;
using System;
using pdsharp.input;

namespace sharpdungeon.ui
{
    public class SimpleButton : Component
    {
        private Image _image;

        public SimpleButton(Image image)
        {
            _image.Copy(image);
            _Width = image.Width;
            _Height = image.Height;
        }

        protected override void CreateChildren()
        {
            _image = new Image();
            Add(_image);
            var touchArea = new TouchArea(_image);
            touchArea.TouchDownAction = TouchDownAction;
            touchArea.TouchUpAction = TouchUpAction;
            touchArea.ClickAction = SimpleButtonClickAction;
            Add(touchArea);
        }

        private void SimpleButtonClickAction(Touch obj)
        {
            OnClick();
        }

        private void TouchUpAction(Touch obj)
        {
            _image.Brightness(1.0f);
        }

        private void TouchDownAction(Touch obj)
        {
            _image.Brightness(1.2f); 
        }

        protected override void Layout()
        {
            _image.X = X;
            _image.Y = Y;
        }

        public Action ClickAction { get; set; }

        protected virtual void OnClick()
        {
            if (ClickAction != null)
                ClickAction();
        }
    }
}