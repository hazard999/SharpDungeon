using Android.Graphics;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.glwrap;
using pdsharp.noosa;
using pdsharp.noosa.particles;
using pdsharp.noosa.ui;
using pdsharp.utils;

namespace sharpdungeon.effects
{
    public class Fireball : Component
    {
        private static readonly RectF Blight = new RectF(0, 0, 0.25f, 1);
        private static readonly RectF Flight = new RectF(0.25f, 0, 0.5f, 1);
        private static readonly RectF Flame1 = new RectF(0.50f, 0, 0.75f, 1);
        private static readonly RectF Flame2 = new RectF(0.75f, 0, 1.00f, 1);

        private const int Color = 0xFF66FF;

        private Image _bLight;
        private Image _fLight;
        private Emitter _emitter;
        private Group _sparks;

        protected override void CreateChildren()
        {
            _sparks = new Group();
            Add(_sparks);

            _bLight = new Image(Assets.FIREBALL);
            _bLight.Frame(Blight);
            _bLight.Origin.Set(_bLight.Width / 2);
            _bLight.AngularSpeed = -90;
            Add(_bLight);

            _emitter = new Emitter();
            _emitter.Pour(new FireballEmitterFactory(), 0.1f);
            Add(_emitter);

            _fLight = new Image(Assets.FIREBALL);
            _fLight.Frame(Flight);
            _fLight.Origin.Set(_fLight.Width / 2);
            _fLight.AngularSpeed = 360;
            Add(_fLight);

            _bLight.texture.Filter(Texture.Linear, Texture.Linear);
        }

        protected override void Layout()
        {
            _bLight.X = Y - _bLight.Width / 2;
            _bLight.Y = Y - _bLight.Height / 2;

            _emitter.Pos(X - _bLight.Width / 4, Y - _bLight.Height / 4, _bLight.Width / 2, _bLight.Height / 2);

            _fLight.X = X - _fLight.Width / 2;
            _fLight.Y = Y - _fLight.Height / 2;
        }

        public override void Update()
        {
            base.Update();

            if (!(pdsharp.utils.Random.Float() < Game.Elapsed))
                return;

            var spark = (PixelParticle)_sparks.Recycle<PixelParticle.Shrinking>();
            spark.reset(X, Y, ColorMath.Random(Color, 0x66FF66), 2, pdsharp.utils.Random.Float(0.5f, 1.0f));
            spark.Speed.Set(pdsharp.utils.Random.Float(-40, +40), pdsharp.utils.Random.Float(-60, +20));
            spark.Acc.Set(0, +80);
            _sparks.Add(spark);
        }

        public override void Draw()
        {
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOne);
            base.Draw();
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);
        }

        public class Flame : Image
        {
            private const float Lifespan = 1f;

            private const float SPEED = -40f;
            private const float ACC = -20f;

            private float _timeLeft;

            public Flame()
                : base(Assets.FIREBALL)
            {
                Frame(pdsharp.utils.Random.Int(2) == 0 ? Flame1 : Flame2);
                Origin.Set(Width / 2, Height / 2);
                Acc.Set(0, ACC);
            }

            public virtual void Reset()
            {
                Revive();
                _timeLeft = Lifespan;
                Speed.Set(0, SPEED);
            }

            public override void Update()
            {
                base.Update();

                if ((_timeLeft -= Game.Elapsed) <= 0)
                    Kill();
                else
                {
                    var p = _timeLeft / Lifespan;
                    Scale.Set(p);
                    Alpha(p > 0.8f ? (1 - p) * 5f : p * 1.25f);
                }
            }
        }
    }

    public class FireballEmitterFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            var p = emitter.Recycle<Fireball.Flame>();
            p.Reset();
            p.X = x - p.Width / 2;
            p.Y = y - p.Height / 2;
        }
    }
}