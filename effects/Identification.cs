using System;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.noosa;
using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects
{
    public class Identification : Group
    {
        private static readonly int[] Dots = { -1, -3, 0, -3, +1, -3, -1, -2, +1, -2, +1, -1, 0, 0, +1, 0, 0, +1, 0, +3 };

        public Identification(PointF p)
        {
            for (var i = 0; i < Dots.Length; i += 2)
            {
                Add(new Speck(p.X, p.Y, Dots[i], Dots[i + 1]));
                Add(new Speck(p.Y, p.Y, Dots[i], Dots[i + 1]));
            }
        }

        public override void Update()
        {
            base.Update();
            if (CountLiving() == 0)
                KillAndErase();
        }

        public override void Draw()
        {
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOne);
            base.Draw();
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);
        }

        public class Speck : PixelParticle
        {
            public Speck(float x0, float y0, int mx, int my)
            {
                Color(0x4488CC);

                var x1 = x0 + mx * 3;
                var y1 = y0 + my * 3;

                var p = new PointF().Polar(pdsharp.utils.Random.Float(2 * PointF.Pi), 8);
                x0 += p.X;
                y0 += p.Y;

                var dx = x1 - x0;
                var dy = y1 - y0;

                X = x0;
                Y = y0;
                Speed.Set(dx, dy);
                Acc.Set(-dx / 4, -dy / 4);

                Left = Lifespan = 2f;
            }

            public override void Update()
            {
                base.Update();

                Am = 1 - Math.Abs(Left / Lifespan - 0.5f) * 2;
                Am *= Am;
                Size(Am * 2);
            }
        }
    }
}