using System;
using Java.Nio;
using pdsharp.gltextures;
using pdsharp.glwrap;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.ui;
using sharpdungeon.actors.hero;
using sharpdungeon.ui;
using Android.Graphics;

namespace sharpdungeon.scenes
{
    public class SurfaceScene : PixelScene
    {
        private const int WIDTH = 80;
        private const int HEIGHT = 112;

        private const int Nstars = 100;
        private const int Nclouds = 5;

        private pdsharp.noosa.Camera _viewport;
        public override void Create()
        {
            base.Create();

            Music.Instance.Play(Assets.HAPPY, true);
            Music.Instance.Volume(1f);

            uiCamera.Visible = false;

            var w = pdsharp.noosa.Camera.Main.CameraWidth;
            var h = pdsharp.noosa.Camera.Main.CameraHeight;

            var archs = new Archs();
            archs.Reversed = true;
            archs.SetSize(w, h);
            Add(archs);

            var vx = Align((w - WIDTH) / 2);
            var vy = Align((h - HEIGHT) / 2);

            var s = pdsharp.noosa.Camera.Main.CameraToScreen(vx, vy);
            _viewport = new pdsharp.noosa.Camera(s.X, s.Y, WIDTH, HEIGHT, defaultZoom);
            pdsharp.noosa.Camera.Add(_viewport);

            var window = new Group();
            window.Camera = _viewport;
            Add(window);

            var dayTime = !Dungeon.NightMode;

            var sky = new Sky(dayTime);
            sky.Scale.Set(WIDTH, HEIGHT);
            window.Add(sky);

            if (!dayTime)
            {
                for (var i = 0; i < Nstars; i++)
                {
                    var size = pdsharp.utils.Random.Float();
                    var star = new ColorBlock(size, size, Color.Argb(255, 255, 255, 255));
                    star.X = pdsharp.utils.Random.Float(WIDTH) - size / 2;
                    star.Y = pdsharp.utils.Random.Float(HEIGHT) - size / 2;
                    star.Am = size * (1 - star.Y / HEIGHT);
                    window.Add(star);
                }
            }

            const float range = HEIGHT * 2 / 3;
            for (var i = 0; i < Nclouds; i++)
            {
                var cloud = new Cloud((Nclouds - 1 - i) * (range / Nclouds) + pdsharp.utils.Random.Float(range / Nclouds), dayTime);
                window.Add(cloud);
            }

            var nPatches = (int)(sky.Width / GrassPatch.WIDTH + 1);

            for (var i = 0; i < nPatches * 4; i++)
            {
                var patch = new GrassPatch((i - 0.75f) * GrassPatch.WIDTH / 4, HEIGHT + 1, dayTime);
                patch.Brightness(dayTime ? 0.7f : 0.4f);
                window.Add(patch);
            }

            var a = new Avatar(Dungeon.Hero.heroClass);
            a.X = Align((WIDTH - a.Width) / 2);
            a.Y = HEIGHT - a.Height + 1;
            window.Add(a);

            var pet = new Pet();
            pet.Rm = pet.Gm = pet.Bm = 1.2f;
            pet.X = WIDTH / 2 + 2;
            pet.Y = HEIGHT - pet.Height;
            window.Add(pet);

            if (dayTime)
            {
                a.Brightness(1.2f);
                pet.Brightness(1.2f);
            }

            //window.Add(new TouchArea(sky) { protected void OnClick(Touch touch) { pet.Jump(); }; });

            for (var i = 0; i < nPatches; i++)
            {
                var patch = new GrassPatch((i - 0.5f) * GrassPatch.WIDTH, HEIGHT, dayTime);
                patch.Brightness(dayTime ? 1.0f : 0.8f);
                window.Add(patch);
            }

            var frame = new Image(Assets.SURFACE);
            if (!dayTime)
                frame.Hardlight(0xDDEEFF);

            frame.Frame(0, 0, 88, 125);
            frame.X = vx - 4;
            frame.Y = vy - 9;
            Add(frame);

            var gameOver = new RedButton("Game Over");
            gameOver.ClickAction = GameOverClickAction;
            gameOver.SetSize(WIDTH - 10, 20);
            gameOver.SetPos(5 + frame.X + 4, frame.Y + frame.Height + 4);
            Add(gameOver);

            Badge.ValidateHappyEnd();

            FadeIn();
        }

        private void GameOverClickAction(Button button)
        {
            Game.SwitchScene<TitleScene>();
        }

        public override void Destroy()
        {
            Badge.SaveGlobal();

            pdsharp.noosa.Camera.Remove(_viewport);
            base.Destroy();
        }

        protected override void OnBackPressed()
        {
        }

        private class Sky : Visual
        {
            private static readonly Color[] Day = { Android.Graphics.Color.Argb(0xFF, 0x44, 0x88, 0xFF), Android.Graphics.Color.Argb(0xFF, 0xCC, 0xEE, 0xFF) };
            private static readonly Color[] Night = { Android.Graphics.Color.Argb(0xFF, 0x00, 0x11, 0x55), Android.Graphics.Color.Argb(0xFF, 0x33, 0x59, 0x80) };

            private readonly SmartTexture _texture;
            private readonly FloatBuffer _verticesBuffer;

            public Sky(bool dayTime)
                : base(0, 0, 1, 1)
            {
                _texture = new Gradient(dayTime ? Day : Night);

                var vertices = new float[16];
                _verticesBuffer = Quad.Create();

                vertices[2] = 0.25f;
                vertices[6] = 0.25f;
                vertices[10] = 0.75f;
                vertices[14] = 0.75f;

                vertices[3] = 0;
                vertices[7] = 1;
                vertices[11] = 1;
                vertices[15] = 0;


                vertices[0] = 0;
                vertices[1] = 0;

                vertices[4] = 1;
                vertices[5] = 0;

                vertices[8] = 1;
                vertices[9] = 1;

                vertices[12] = 0;
                vertices[13] = 1;

                _verticesBuffer.Position(0);
                _verticesBuffer.Put(vertices);
            }

            public override void Draw()
            {
                base.Draw();

                var script = NoosaScript.Get();

                _texture.Bind();

                script.Camera(Camera);

                script.UModel.valueM4(Matrix);
                script.Lighting(Rm, Gm, Bm, Am, RA, Ga, Ba, Aa);

                script.DrawQuad(_verticesBuffer);
            }
        }

        private class Cloud : Image
        {
            private static int _lastIndex = -1;

            public Cloud(float y, bool dayTime)
                : base(Assets.SURFACE)
            {

                int index;
                do
                {
                    index = pdsharp.utils.Random.Int(3);
                }
                while (index == _lastIndex);

                switch (index)
                {
                    case 0:
                        Frame(88, 0, 49, 20);
                        break;
                    case 1:
                        Frame(88, 20, 49, 22);
                        break;
                    case 2:
                        Frame(88, 42, 50, 18);
                        break;
                }

                _lastIndex = index;

                Y = y;

                Scale.Set(1 - y / HEIGHT);
                X = pdsharp.utils.Random.Float(WIDTH + Width) - Width;
                Speed.X = Scale.X * (dayTime ? +8 : -8);

                if (dayTime)
                    Tint(0xCCEEFF, 1 - Scale.Y);
                else
                {
                    Rm = Gm = Bm = +3.0f;
                    RA = Ga = Ba = -2.1f;
                }
            }

            public override void Update()
            {
                base.Update();
                if (Speed.X > 0 && X > WIDTH)
                    X = -Width;
                else
                    if (Speed.X < 0 && X < -Width)
                        X = WIDTH;
            }
        }

        private class Avatar : Image
        {
            private const int WIDTH = 24;
            private const int HEIGHT = 28;

            public Avatar(HeroClass cl)
                : base(Assets.AVATARS)
            {
                Frame(new TextureFilm(texture, WIDTH, HEIGHT).Get(cl.Ordinal()));
            }
        }

        private class Pet : MovieClip, IListener
        {
            readonly Animation _idle;
            readonly Animation _jump;

            public Pet()
                : base(Assets.PET)
            {
                var frames = new TextureFilm(texture, 16, 16);


                _idle = new Animation(2, true);
                _idle.Frames(frames, 0, 0, 0, 0, 0, 0, 1);


                _jump = new Animation(10, false);
                _jump.Frames(frames, 2, 3, 4, 5, 6);

                Listener = this;

                Play(_idle);
            }

            public virtual void Jump()
            {
                Play(_jump);
            }

            public void OnComplete(Animation anim)
            {
                if (anim == _jump)
                    Play(_idle);
            }
        }

        private class GrassPatch : Image
        {

            public const int WIDTH = 16;
            public const int HEIGHT = 14;

            private readonly float _tx;
            private readonly float _ty;

            private double _a = pdsharp.utils.Random.Float(5);
            private double _angle;

            private readonly bool _forward;

            public GrassPatch(float tx, float ty, bool forward)
                : base(Assets.SURFACE)
            {
                Frame(88 + pdsharp.utils.Random.Int(4) * WIDTH, 60, WIDTH, HEIGHT);

                _tx = tx;
                _ty = ty;

                _forward = forward;
            }

            public override void Update()
            {
                base.Update();
                _a += pdsharp.utils.Random.Float(Game.Elapsed * 5);
                _angle = (2 + Math.Cos(_a)) * (_forward ? +0.2 : -0.2);

                Scale.Y = (float)Math.Cos(_angle);

                X = _tx + (float)Math.Tan(_angle) * Width;
                Y = _ty - Scale.Y * Height;
            }

            protected override void UpdateMatrix()
            {
                base.UpdateMatrix();
                pdsharp.glwrap.Matrix.SkewX(Matrix, (float)(_angle / pdsharp.glwrap.Matrix.G2RAD));
            }
        }
    }
}