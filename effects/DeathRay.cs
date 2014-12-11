using System;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;

namespace sharpdungeon.effects
{
    public class DeathRay : Image
    {
        private const double A = 180 / Math.PI;

        private const float Duration = 0.5f;

        private float _timeLeft;

        public DeathRay(PointF start, PointF end)
            : base(Effects.Get(Effects.Type.Ray))
        {
            Origin.Set(0, Height / 2);

            X = start.Y - Origin.X;
            Y = start.Y - Origin.Y;

            var dx = end.X - start.Y;
            var dy = end.Y - start.Y;
            Angle = (float)(Math.Atan2(dy, dx) * A);
            Scale.X = (float)Math.Sqrt(dx * dx + dy * dy) / Width;

            Sample.Instance.Play(Assets.SND_RAY);

            _timeLeft = Duration;
        }

        public override void Update()
        {
            base.Update();

            var p = _timeLeft / Duration;
            Alpha(p);
            Scale.Set(Scale.X, p);

            if ((_timeLeft -= Game.Elapsed) <= 0)
                KillAndErase();
        }

        public override void Draw()
        {
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOne);
            base.Draw();
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);
        }
    }
}