using Android.Opengl;
using Android.Util;
using Java.Nio;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.gltextures;
using pdsharp.noosa;

namespace sharpdungeon.effects
{
    public class Flare : Visual
    {
        private float _duration;
        private float _lifespan;

        private bool _lightMode = true;

        private readonly SmartTexture _texture;

        private readonly FloatBuffer _vertices;
        private readonly ShortBuffer _indices;

        private readonly int _nRays;

        public Flare(int nRays, float radius)
            : base(0, 0, 0, 0)
        {
            // FIXME
            // Texture is incorrectly created every time we need
            // to show the effect, it must be refactored

            var gradient = new[]{
                Android.Graphics.Color.Argb(0xFF, 0xFF, 0xFF, 0xFF), 
                Android.Graphics.Color.Argb(0x00, 0xFF, 0xFF, 0xFF)
            };

            _texture = new Gradient(gradient);

            _nRays = nRays;

            Angle = 45;
            AngularSpeed = 180;

            _vertices = ByteBuffer.AllocateDirect((nRays * 2 + 1) * 4 * (sizeof(float))).Order(ByteOrder.NativeOrder()).AsFloatBuffer();

            _indices = ByteBuffer.AllocateDirect(nRays * 3 * sizeof(short)).Order(ByteOrder.NativeOrder()).AsShortBuffer();

            var v = new float[4];

            v[0] = 0;
            v[1] = 0;
            v[2] = 0.25f;
            v[3] = 0;
            _vertices.Put(v);

            v[2] = 0.75f;
            v[3] = 0;

            for (var i = 0; i < nRays; i++)
            {
                var a = i * 3.1415926f * 2 / nRays;
                v[0] = FloatMath.Cos(a) * radius;
                v[1] = FloatMath.Sin(a) * radius;
                _vertices.Put(v);

                a += 3.1415926f * 2 / nRays / 2;
                v[0] = FloatMath.Cos(a) * radius;
                v[1] = FloatMath.Sin(a) * radius;
                _vertices.Put(v);

                _indices.Put(0);
                _indices.Put((short)(1 + i * 2));
                _indices.Put((short)(2 + i * 2));
            }

            _indices.Position(0);
        }

        public virtual Flare Color(int color, bool lightMode)
        {
            _lightMode = lightMode;
            Hardlight(color);

            return this;
        }

        public virtual Flare Show(Visual visual, float duration)
        {
            Point(visual.Center());
            visual.Parent.AddToBack(this);

            _lifespan = _duration = duration;

            return this;
        }

        public override void Update()
        {
            base.Update();

            if (!(_duration > 0))
                return;

            if ((_lifespan -= Game.Elapsed) > 0)
            {
                var p = 1 - _lifespan / _duration; // 0 -> 1
                p = p < 0.25f ? p * 4 : (1 - p) * 1.333f;
                Scale.Set(p);
                Alpha(p);
            }
            else
                KillAndErase();
        }

        public override void Draw()
        {
            base.Draw();

            if (_lightMode)
            {
                GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOne);
                DrawRays();
                GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);
            }
            else
                DrawRays();
        }

        private void DrawRays()
        {
            var script = NoosaScript.Get();

            _texture.Bind();

            script.UModel.valueM4(Matrix);
            script.Lighting(Rm, Gm, Bm, Am, RA, Ga, Ba, Aa);

            script.Camera(Camera);
            script.DrawElements(_vertices, _indices, _nRays * 3);
        }
    }
}