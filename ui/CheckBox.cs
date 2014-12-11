using sharpdungeon.scenes;

namespace sharpdungeon.ui
{
    public class CheckBox : RedButton
    {
        private bool _checked;

        public CheckBox(string label) : base(label)
        {
            Icon(Icons.UNCHECKED.Get());
        }

        protected override void Layout()
        {
            base.Layout();

            float margin = (Height - InternalText.BaseLine()) / 2;

            InternalText.X = PixelScene.Align(PixelScene.uiCamera, X + margin);
            InternalText.Y = PixelScene.Align(PixelScene.uiCamera, Y + margin);

            InternalIcon.X = PixelScene.Align(PixelScene.uiCamera, X + Width - margin - InternalIcon.Width);
            InternalIcon.Y = PixelScene.Align(PixelScene.uiCamera, Y + (Height - InternalIcon.Height) / 2);
        }

        public virtual bool Checked()
        {
            return _checked;
        }

        public virtual void SetChecked(bool value)
        {
            if (_checked != value)
            {
                _checked = value;
                InternalIcon.Copy(_checked ? Icons.CHECKED.Get() : Icons.UNCHECKED.Get());
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            SetChecked(!_checked);
        }
    }

}