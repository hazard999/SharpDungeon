using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.noosa;
using sharpdungeon.actors;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;

namespace sharpdungeon.sprites
{
    public class WandmakerSprite : MobSprite
    {
        private Shield _shield;

        public WandmakerSprite()
        {
            Texture(Assets.MAKER);

            var frames = new TextureFilm(texture, 12, 14);

            IdleAnimation = new Animation(10, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 3, 3, 3, 3, 3, 2, 1);

            RunAnimation = new Animation(20, true);
            RunAnimation.Frames(frames, 0);

            DieAnimation = new Animation(20, false);
            DieAnimation.Frames(frames, 0);

            Play(IdleAnimation);
        }

        public override void Link(Character ch)
        {
            base.Link(ch);

            if (_shield == null)
                Parent.Add(_shield = new Shield(this));
        }

        public override void DoDie()
        {
            base.DoDie();

            if (_shield != null)
                _shield.PutOut();

            Emitter().Start(ElmoParticle.Factory, 0.03f, 60);
        }

        public class Shield : Halo
        {
            private readonly WandmakerSprite _wandmakerSprite;
            private float _phase;

            public Shield(WandmakerSprite wandmakerSprite)
                : base(14, 0xBBAACC, 1f)
            {
                _wandmakerSprite = wandmakerSprite;
                Am = -1;
                Aa = +1;

                _phase = 1;
            }

            public override void Update()
            {
                base.Update();

                if (_phase < 1)
                {
                    if ((_phase -= Game.Elapsed) <= 0)
                    {
                        KillAndErase();
                    }
                    else
                    {
                        Scale.Set((2 - _phase) * radius / RADIUS);
                        Am = _phase * (-1);
                        Aa = _phase * (+1);
                    }
                }

                Visible = _wandmakerSprite.Visible;
                if (!Visible) 
                    return;

                var p = _wandmakerSprite.Center();
                Point(p.X, p.Y);
            }

            public override void Draw()
            {
                GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOne);
                base.Draw();
                GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);
            }

            public virtual void PutOut()
            {
                _phase = 0.999f;
            }
        }
    }
}