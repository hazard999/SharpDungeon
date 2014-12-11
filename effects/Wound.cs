using pdsharp.noosa;
using sharpdungeon.actors;
using sharpdungeon.levels;

namespace sharpdungeon.effects
{
    public class Wound : Image
    {
        private const float TimeToFade = 0.8f;

        private float _time;

        public Wound()
            : base(Effects.Get(Effects.Type.Wound))
        {
            Origin.Set(Width / 2, Height / 2);
        }

        public virtual void Reset(int p)
        {
            Revive();

            X = (p % Level.Width) * DungeonTilemap.Size + (DungeonTilemap.Size - Width) / 2;
            Y = (p / Level.Width) * DungeonTilemap.Size + (DungeonTilemap.Size - Height) / 2;

            _time = TimeToFade;
        }

        public override void Update()
        {
            base.Update();

            if ((_time -= Game.Elapsed) <= 0)
                Kill();
            else
            {
                var p = _time / TimeToFade;
                Alpha(p);
                Scale.X = 1 + p;
            }
        }

        public static void Hit(Character ch)
        {
            Hit(ch, 0);
        }

        public static void Hit(Character ch, float angle)
        {
            var w = ch.Sprite.Parent.Recycle<Wound>();
            ch.Sprite.Parent.BringToFront(w);
            w.Reset(ch.pos);
            w.Angle = angle;
        }

        public static void Hit(int pos)
        {
            Hit(pos, 0);
        }

        public static void Hit(int pos, float angle)
        {
            var parent = Dungeon.Hero.Sprite.Parent;
            var w = parent.Recycle<Wound>();
            parent.BringToFront(w);
            w.Reset(pos);
            w.Angle = angle;
        }
    }
}