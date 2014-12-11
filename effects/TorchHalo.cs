using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.noosa;
using sharpdungeon.sprites;

namespace sharpdungeon.effects
{
    public class TorchHalo : Halo
    {
        private readonly CharSprite _target;

        private float _phase;

        public TorchHalo(CharSprite sprite)
            : base(24, 0xFFDDCC, 0.15f)
        {
            _target = sprite;
            Am = 0;
        }

        public override void Update()
        {
            base.Update();

            if (_phase < 0)
            {
                if ((_phase += Game.Elapsed) >= 0)
                    KillAndErase();
                else
                {
                    Scale.Set((2 + _phase) * radius / RADIUS);
                    Am = -_phase * brightness;
                }
            }
            else
                if (_phase < 1)
                {
                    if ((_phase += Game.Elapsed) >= 1)
                        _phase = 1;

                    Scale.Set(_phase * radius / RADIUS);
                    Am = _phase * brightness;
                }

            Point(_target.X + _target.Width / 2, _target.Y + _target.Height / 2);
        }

        public override void Draw()
        {
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOne);
            base.Draw();
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);
        }

        public virtual void PutOut()
        {
            _phase = -1;
        }
    }
}