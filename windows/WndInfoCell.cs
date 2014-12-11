using Java.Lang;
using pdsharp.noosa;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
    public class WndInfoCell : Window
    {
        private const float Gap = 2;

        private const int WIDTH = 120;

        private const string TxtNothing = "There is nothing here.";

        public WndInfoCell(int cell)
        {
            var tile = Dungeon.Level.map[cell];
            if (Level.water[cell])
                tile = Terrain.WATER;
            else
                if (Level.pit[cell])
                    tile = Terrain.CHASM;

            var titlebar = new IconTitle();
            if (tile == Terrain.WATER)
            {
                var water = new Image(Dungeon.Level.WaterTex());
                water.Frame(0, 0, DungeonTilemap.Size, DungeonTilemap.Size);
                titlebar.Icon(water);
            }
            else
                titlebar.Icon(DungeonTilemap.Tile(tile));

            titlebar.Label(Dungeon.Level.TileName(tile));
            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            var info = PixelScene.CreateMultiline(6);
            Add(info);

            var desc = new StringBuilder(Dungeon.Level.TileDesc(tile));

            const string newLine = "\\Negative";
            foreach (var blob in Dungeon.Level.Blobs.Values)
            {
                if (blob.Cur[cell] <= 0 || blob.TileDesc() == null) 
                    continue;

                if (desc.Length() > 0)
                    desc.Append(newLine);

                desc.Append(blob.TileDesc());
            }

            info.Text(desc.Length() > 0 ? desc.ToString() : TxtNothing);
            info.MaxWidth = WIDTH;
            info.Measure();
            info.X = titlebar.Left();
            info.Y = titlebar.Bottom() + Gap;

            Resize(WIDTH, (int)(info.Y + info.Height));
        }
    }
}