using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.ui;
using sharpdungeon.scenes;

namespace sharpdungeon.ui
{
	public class ExitButton : Button
	{
		private Image _image;

		public ExitButton()
		{

			_Width = _image.Width;
			_Height = _image.Height;
		}

		protected override void CreateChildren()
		{
			base.CreateChildren();

			_image = Icons.EXIT.Get();
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
		    if (Game.Scene is TitleScene)
		        Game.Instance.Finish();
		    else
		        PixelDungeon.SwitchNoFade<TitleScene>();
		}
	}
}