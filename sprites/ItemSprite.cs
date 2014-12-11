using Android.Graphics;
using Android.Util;
using pdsharp.gltextures;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.scenes;
using PointF = pdsharp.utils.PointF;

namespace sharpdungeon.sprites
{
    public class ItemSprite : MovieClip
    {
        public const int SIZE = 16;

        private const float DROP_INTERVAL = 0.4f;

        protected internal static TextureFilm film;

        public Heap heap;

        private Glowing glowing;
        private float phase;
        private bool glowUp;

        private float dropInterval;

        public ItemSprite()
            : this(ItemSpriteSheet.SMTH, null)
        {
        }

        public ItemSprite(Item item)
            : this(item.Image, item.Glowing())
        {
        }

        public ItemSprite(int image, Glowing glowing)
            : base(Assets.ITEMS)
        {
            if (film == null)
                film = new TextureFilm(texture, SIZE, SIZE);

            View(image, glowing);
        }

        public virtual void OriginToCenter()
        {
            Origin.Set(SIZE / 2);
        }

        public virtual void Link()
        {
            Link(heap);
        }

        public virtual void Link(Heap heap)
        {
            this.heap = heap;
            View(heap.Image(), heap.Glowing());
            Place(heap.Pos);
        }

        public override void Revive()
        {
            base.Revive();

            Speed.Set(0);
            Acc.Set(0);
            dropInterval = 0;

            heap = null;
        }

        public virtual PointF WorldToCamera(int cell)
        {
            const int csize = DungeonTilemap.Size;

            return new PointF(cell % levels.Level.Width * csize + (csize - SIZE) * 0.5f, cell / levels.Level.Width * csize + (csize - SIZE) * 0.5f);
        }

        public virtual void Place(int p)
        {
            Point(WorldToCamera(p));
        }

        public virtual void Drop()
        {
            if (heap.IsEmpty)
                return;

            dropInterval = DROP_INTERVAL;

            Speed.Set(0, -100);
            Acc.Set(0, -Speed.Y / DROP_INTERVAL * 2);

            if (IsVisible && heap != null && heap.Peek() is Gold)
            {
                CellEmitter.Center(heap.Pos).Burst(Speck.Factory(Speck.COIN), 5);
                Sample.Instance.Play(Assets.SND_GOLD, 1, 1, pdsharp.utils.Random.Float(0.9f, 1.1f));
            }
        }

        public virtual void Drop(int from)
        {
            if (heap.Pos == from)
                Drop();
            else
            {
                var px = X;
                var py = Y;
                Drop();

                Place(from);

                Speed.Offset((px - X)/DROP_INTERVAL, (py - Y)/DROP_INTERVAL);

                Log.Debug("GAME", ToString());
                Log.Debug("GAME", string.Format("drop aside: {0:F1} {1:F1}", Speed.X, Speed.Y));
            }
        }

        public virtual ItemSprite View(int image, Glowing glowing)
        {
            Frame(film.Get(image));
            if ((this.glowing = glowing) == null)
                ResetColor();

            return this;
        }

        public override void Update()
        {
            base.Update();

            // Visibility
            Visible = heap == null || Dungeon.Visible[heap.Pos];

            // Dropping
            if (dropInterval > 0 && (dropInterval -= Game.Elapsed) <= 0)
            {

                Speed.Set(0);
                Acc.Set(0);
                Place(heap.Pos);

                if (levels.Level.water[heap.Pos])
                    GameScene.Ripple(heap.Pos);
            }

            // Glowing
            if (Visible && glowing != null)
            {
                if (glowUp && (phase += Game.Elapsed) > glowing.Period)
                {

                    glowUp = false;
                    phase = glowing.Period;

                }
                else if (!glowUp && (phase -= Game.Elapsed) < 0)
                {

                    glowUp = true;
                    phase = 0;

                }

                float value = phase / glowing.Period * 0.6f;

                Rm = Gm = Bm = 1 - value;
                RA = glowing.Red * value;
                Ga = glowing.Green * value;
                Ba = glowing.Blue * value;
            }
        }

        public static Color Pick(int index, int x, int y)
        {
            var bmp = TextureCache.Get(Assets.ITEMS).bitmap;
            var rows = bmp.Width / SIZE;
            var row = index / rows;
            var col = index % rows;
            return new Color(bmp.GetPixel(col * SIZE + x, row * SIZE + y));
        }

        public class Glowing
        {
            public static readonly Glowing White = new Glowing(0xFFFFFF, 0.6f);

            public float Red;
            public float Green;
            public float Blue;
            public float Period;

            public Glowing(int color)
                : this(color, 1f)
            {
            }

            public Glowing(int color, float period)
            {
                Red = (color >> 16) / 255f;
                Green = ((color >> 8) & 0xFF) / 255f;
                Blue = (color & 0xFF) / 255f;

                Period = period;
            }
        }
    }
}