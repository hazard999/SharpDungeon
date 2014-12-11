using pdsharp.noosa;
using pdsharp.noosa.tweeners;
using pdsharp.utils;
using sharpdungeon.scenes;

namespace sharpdungeon
{
    public class DungeonTilemap : Tilemap
    {
        public const int Size = 16;

        private static DungeonTilemap _instance;

        public DungeonTilemap()
            : base(Dungeon.Level.TilesTex(), new TextureFilm(Dungeon.Level.TilesTex(), Size, Size))
        {
            Map(Dungeon.Level.map, levels.Level.Width);

            _instance = this;
        }

        public virtual int ScreenToTile(int x, int y)
        {
            var p = Camera.ScreenToCamera(x, y).Offset(Point().Negate()).InvScale(Size).Floor();
            return p.X >= 0 && p.X < levels.Level.Width && p.Y >= 0 && p.Y < levels.Level.Height ? p.X + p.Y * levels.Level.Width : -1;
        }

        public override bool OverlapsPoint(float x, float y)
        {
            return true;
        }

        public virtual void Discover(int pos, int oldValue)
        {
            var tile = Tile(oldValue);
            tile.Point(TileToWorld(pos));

            // For bright mode
            tile.Rm = tile.Gm = tile.Bm = Rm;
            tile.RA = tile.Ga = tile.Ba = RA;
            Parent.Add(tile);
            
            var tweener = new AlphaTweener(tile, 0, 0.6f);
            tweener.CompleteAction = (action) =>
            {
                tile.KillAndErase();
                KillAndErase();
            };
            Parent.Add(tweener);
        }

        public static PointF TileToWorld(int pos)
        {
            return new PointF(pos % levels.Level.Width, pos / levels.Level.Width).Scale(Size);
        }

        public static PointF TileCenterToWorld(int pos)
        {
            return new PointF((pos % levels.Level.Width + 0.5f) * Size, (pos / levels.Level.Width + 0.5f) * Size);
        }

        public static Image Tile(int index)
        {
            var img = new Image(_instance.texture);
            img.Frame(_instance.tileset.Get(index));
            return img;
        }

        public override bool OverlapsScreenPoint(int x, int y)
        {
            return true;
        }

        public override void Draw()
        {
            base.Draw();
            var p = Camera.ScreenToCamera((int)X, (int)Y).Offset(Point().Negate()).InvScale(Size).Floor();

            var txt = new BitmapText(p.X + "," + p.Y, PixelScene.font25x);
            txt.X = X;
            txt.Y = Y;
            txt.Draw();
        }
    }
}