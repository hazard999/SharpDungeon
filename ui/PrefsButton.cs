using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.ui;
using sharpdungeon.windows;

namespace sharpdungeon.ui
{
    public class PrefsButton : Button
    {
        private Image _image;

        public PrefsButton()
        {
            _Width = _image.Width;
            _Height = _image.Height;
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _image = Icons.PREFS.Get();
            Add(_image);
        }

        protected override void Layout()
        {
            base.Layout();

            _image.X = X;
            _image.Y = Y;
        }

        protected override void OnTouchDown()
        {
            _image.Brightness(1.5f);
            Sample.Instance.Play(Assets.SND_CLICK);
        }

        protected override void OnTouchUp()
        {
            _image.ResetColor();
        }

        protected override void OnClick()
        {
            Parent.Add(new WndSettings(false));
        }
    }
}