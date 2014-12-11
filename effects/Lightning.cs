using System.Collections.Generic;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.levels;
using System;

namespace sharpdungeon.effects
{
    public class Lightning : Group
    {
        private const float Duration = 0.3f;

        private float _life;

        private readonly int _length;
        private readonly float[] _cx;
        private readonly float[] _cy;

        private readonly Image[] _arcsS;
        private readonly Image[] _arcsE;

        private readonly ICallback _callback;

        public Lightning(IList<int> cells, int length, ICallback callback)
        {
            _callback = callback;

            var proto = Effects.Get(Effects.Type.Lightning);
            const float ox = 0;
            var oy = proto.Height / 2;

            _length = length;
            _cx = new float[length];
            _cy = new float[length];

            for (var i = 0; i < length; i++)
            {
                var c = cells[i];
                _cx[i] = (c % Level.Width + 0.5f) * DungeonTilemap.Size;
                _cy[i] = (c / Level.Width + 0.5f) * DungeonTilemap.Size;
            }

            _arcsS = new Image[length - 1];
            _arcsE = new Image[length - 1];
            for (var i = 0; i < length - 1; i++)
            {
                var arc = _arcsS[i] = new Image(proto);

                arc.X = _cx[i] - arc.Origin.X;
                arc.Y = _cy[i] - arc.Origin.Y;
                arc.Origin.Set(ox, oy);
                Add(arc);

                arc = _arcsE[i] = new Image(proto);
                arc.Origin.Set(ox, oy);
                Add(arc);
            }

            _life = Duration;

            Sample.Instance.Play(Assets.SND_LIGHTNING);
        }

        private const double A = 180 / Math.PI;

        public override void Update()
        {
            base.Update();

            if ((_life -= Game.Elapsed) < 0)
            {
                KillAndErase();
                if (_callback != null)
                    _callback.Call();
            }
            else
            {
                var alpha = _life / Duration;

                for (var i = 0; i < _length - 1; i++)
                {
                    var sx = _cx[i];
                    var sy = _cy[i];
                    var ex = _cx[i + 1];
                    var ey = _cy[i + 1];

                    var x2 = (sx + ex) / 2 + pdsharp.utils.Random.Float(-4, +4);
                    var y2 = (sy + ey) / 2 + pdsharp.utils.Random.Float(-4, +4);

                    var dx = x2 - sx;
                    var dy = y2 - sy;
                    var arc = _arcsS[i];
                    arc.Am = alpha;
                    arc.Angle = (float)(Math.Atan2(dy, dx) * A);
                    arc.Scale.X = (float)Math.Sqrt(dx * dx + dy * dy) / arc.Width;

                    dx = ex - x2;
                    dy = ey - y2;
                    arc = _arcsE[i];
                    arc.Am = alpha;
                    arc.Angle = (float)(Math.Atan2(dy, dx) * A);
                    arc.Scale.X = (float)Math.Sqrt(dx * dx + dy * dy) / arc.Width;
                    arc.X = x2 - arc.Origin.X;
                    arc.Y = y2 - arc.Origin.Y;
                }
            }
        }

        public override void Draw()
        {
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOne);
            base.Draw();
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);
        }
    }
}