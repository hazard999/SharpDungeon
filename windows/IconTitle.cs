using System;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.items;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
	public class IconTitle : Component
	{
		private const float FontSize = 9;

		private const float Gap = 2;

		protected internal Image ImIcon;
		protected internal BitmapTextMultiline TfLabel;

		public IconTitle()
		{
		}

		public IconTitle(Item item) : this(new ItemSprite(item.Image, item.Glowing()), Utils.Capitalize(item.ToString()))
		{
		}

		public IconTitle(Image icon, string label)
		{
			Icon(icon);
			Label(label);
		}

		protected override void CreateChildren()
		{
			ImIcon = new Image();
			Add(ImIcon);

			TfLabel = PixelScene.CreateMultiline(FontSize);
			TfLabel.Hardlight(Window.TitleColor);
			Add(TfLabel);
		}

		protected override void Layout()
		{
			ImIcon.X = 0;
			ImIcon.Y = 0;

			TfLabel.X = PixelScene.Align(PixelScene.uiCamera, ImIcon.X + ImIcon.Width + Gap);
			TfLabel.MaxWidth = (int)(Width - TfLabel.X);
			TfLabel.Measure();
			TfLabel.Y = PixelScene.Align(PixelScene.uiCamera, ImIcon.Height > TfLabel.Height ? (ImIcon.Height - TfLabel.BaseLine()) / 2 : ImIcon.Y);

			_Height = Math.Max(ImIcon.Y + ImIcon.Height, TfLabel.Y + TfLabel.Height);
		}

		public virtual void Icon(Image icon)
		{
			Remove(ImIcon);
			Add(ImIcon = icon);
		}

		public virtual void Label(string label)
		{
			TfLabel.Text(label);
		}

		public virtual void Label(string label, int color)
		{
			TfLabel.Text(label);
			TfLabel.Hardlight(color);
		}

		public virtual void Color(int color)
		{
			TfLabel.Hardlight(color);
		}
	}
}