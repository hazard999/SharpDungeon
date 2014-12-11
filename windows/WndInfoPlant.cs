
using pdsharp.noosa;
using sharpdungeon.plants;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
	public class WndInfoPlant : Window
	{
		private const float Gap = 2;

		private const int WIDTH = 120;

		public WndInfoPlant(Plant plant)
		{
			var titlebar = new IconTitle();
			titlebar.Icon(new PlantSprite(plant.Image));
			titlebar.Label(plant.PlantName);
			titlebar.SetRect(0, 0, WIDTH, 0);
			Add(titlebar);

			var info = PixelScene.CreateMultiline(6);
			Add(info);

			info.Text(plant.Desc());
			info.MaxWidth = WIDTH;
			info.Measure();
			info.X = titlebar.Left();
			info.Y = titlebar.Bottom() + Gap;

			Resize(WIDTH, (int)(info.Y + info.Height));
		}
	}
}