using pdsharp.noosa;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;

namespace sharpdungeon.effects
{
    public class EmoIcon : Image
    {
        protected internal float MaxSize = 2;
        protected internal float TimeScale = 1;

        protected internal bool Growing = true;

        protected internal CharSprite Owner;

        public EmoIcon(CharSprite owner)
        {
            Owner = owner;
            GameScene.Add(this);
        }

        public override void Update()
        {
            base.Update();

            if (!Visible) 
                return;

            if (Growing)
            {
                Scale.Set(Scale.X + Game.Elapsed * TimeScale);
                if (Scale.X > MaxSize)
                    Growing = false;
            }
            else
            {
                Scale.Set(Scale.X - Game.Elapsed * TimeScale);
                if (Scale.X < 1)
                    Growing = true;
            }

            X = Owner.X + Owner.Width - Width / 2;
            Y = Owner.Y - Height;
        }

        public class Sleep : EmoIcon
        {
            public Sleep(CharSprite owner)
                : base(owner)
            {
                Copy(Icons.SLEEP.Get());

                MaxSize = 1.2f;
                TimeScale = 0.5f;

                Origin.Set(Width / 2, Height / 2);
                Scale.Set(pdsharp.utils.Random.Float(1, MaxSize));
            }
        }

        public class Alert : EmoIcon
        {
            public Alert(CharSprite owner)
                : base(owner)
            {
                Copy(Icons.ALERT.Get());

                MaxSize = 1.3f;
                TimeScale = 2;

                Origin.Set(2.5f, Height - 2.5f);
                Scale.Set(pdsharp.utils.Random.Float(1, MaxSize));
            }
        }
    }
}