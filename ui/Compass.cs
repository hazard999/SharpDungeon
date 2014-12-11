using Java.Lang;
using pdsharp.noosa;
using pdsharp.utils;

namespace sharpdungeon.ui
{
    public class Compass : Image
    {
        private const float Rad2G = 180f / 3.1415926f;
        private const float Radius = 12;

        private readonly int _cell;
        private readonly PointF _cellCenter;

        private readonly PointF _lastScroll = new PointF();

        public Compass(int cell)
        {
            Copy(Icons.COMPASS.Get());
            Origin.Set(Width / 2, Radius);

            _cell = cell;
            _cellCenter = DungeonTilemap.TileCenterToWorld(cell);
            Visible = false;
        }

        public override void Update()
        {
            base.Update();

            if (!Visible)
                Visible = Dungeon.Level.visited[_cell] || Dungeon.Level.mapped[_cell];

            if (!Visible)
                return;

            var scroll = Camera.Main.Scroll;

            if (scroll.Equals(_lastScroll))
                return;

            _lastScroll.Set(scroll);
            var center = Camera.Main.Center().Offset(scroll);
            Angle = (float)Math.Atan2(_cellCenter.X - center.Y, center.Y - _cellCenter.Y) * Rad2G;
        }
    }
}