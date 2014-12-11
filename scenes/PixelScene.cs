using System;
using Android.Graphics;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.input;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.effects;
using Camera = pdsharp.noosa.Camera;

namespace sharpdungeon.scenes
{
    public class PixelScene : Scene
    {

        public const float MIN_WIDTH = 128;
        public const float MIN_HEIGHT = 224;

        public static float defaultZoom = 0;
        public static float minZoom;
        public static float maxZoom;

        public static bool landscapeAvailable;

        public static Camera uiCamera;

        public static Font font1x;
        public static Font font15x;
        public static Font font2x;
        public static Font font25x;
        public static Font font3x;

        public override void Create()
        {
            base.Create();

            GameScene.Scene = null;

            defaultZoom = (int)Math.Ceiling(Game.density * 2.5);
            while ((Game.Width / defaultZoom < MIN_WIDTH || Game.Height / defaultZoom < MIN_HEIGHT) && defaultZoom > 1)
            {

                defaultZoom--;
            }

            landscapeAvailable = Game.Height / defaultZoom >= MIN_WIDTH && Game.Width / defaultZoom >= MIN_HEIGHT;

            if (PixelDungeon.ScaleUp())
                while ((Game.Width / (defaultZoom + 1) >= MIN_WIDTH && Game.Height / (defaultZoom + 1) >= MIN_HEIGHT))
                    defaultZoom++;

            minZoom = 1;
            maxZoom = defaultZoom * 2;


            Camera.Reset(new PixelCamera(defaultZoom));

            float uiZoom = defaultZoom;
            uiCamera = Camera.CreateFullscreen(uiZoom);
            Camera.Add(uiCamera);

            if (font1x == null)
            {

                // 3x5 (6)
                font1x = Font.ColorMarked(BitmapCache.Get(Assets.FONTS1X), Android.Graphics.Color.Argb(0x00, 0x00, 0x00, 0x00), Font.LatinFull);
                font1x.BaseLine = 6;
                font1x.tracking = -1;

                // 5x8 (10)
                font15x = Font.ColorMarked(BitmapCache.Get(Assets.FONTS15X), 12, Android.Graphics.Color.Argb(0x00, 0x00, 0x00, 0x00), Font.LatinFull);
                font15x.BaseLine = 9;
                font15x.tracking = -1;

                // 6x10 (12)
                font2x = Font.ColorMarked(BitmapCache.Get(Assets.FONTS2X), 14, Android.Graphics.Color.Argb(0x00, 0x00, 0x00, 0x00), Font.LatinFull);
                font2x.BaseLine = 11;
                font2x.tracking = -1;

                // 7x12 (15)
                font25x = Font.ColorMarked(BitmapCache.Get(Assets.FONTS25X), 17, Android.Graphics.Color.Argb(0x00, 0x00, 0x00, 0x00), Font.LatinFull);
                font25x.BaseLine = 13;
                font25x.tracking = -1;

                // 9x15 (18)
                font3x = Font.ColorMarked(BitmapCache.Get(Assets.FONTS3X), 22, Android.Graphics.Color.Argb(0x00, 0x00, 0x00, 0x00), Font.LatinFull);
                font3x.BaseLine = 17;
                font3x.tracking = -2;
            }

            Sample.Instance.Load(Assets.SND_CLICK, Assets.SND_BADGE, Assets.SND_GOLD);
        }

        public override void Destroy()
        {
            base.Destroy();
            Touchscreen.Event.RemoveAll();
        }

        public static Font font;
        public static float scale;

        public static void ChooseFont(float size)
        {
            var pt = size * defaultZoom;

            if (pt >= 19)
            {

                scale = pt / 19;
                if (1.5 <= scale && scale < 2)
                {
                    font = font25x;
                    scale = (int)(pt / 14);
                }
                else
                {
                    font = font3x;
                    scale = (int)scale;
                }

            }
            else if (pt >= 14)
            {

                scale = pt / 14;
                if (1.8 <= scale && scale < 2)
                {
                    font = font2x;
                    scale = (int)(pt / 12);
                }
                else
                {
                    font = font25x;
                    scale = (int)scale;
                }

            }
            else if (pt >= 12)
            {

                scale = pt / 12;
                if (1.7 <= scale && scale < 2)
                {
                    font = font15x;
                    scale = (int)(pt / 10);
                }
                else
                {
                    font = font2x;
                    scale = (int)scale;
                }

            }
            else if (pt >= 10)
            {

                scale = pt / 10;
                if (1.4 <= scale && scale < 2)
                {
                    font = font1x;
                    scale = (int)(pt / 7);
                }
                else
                {
                    font = font15x;
                    scale = (int)scale;
                }

            }
            else
            {

                font = font1x;
                scale = Math.Max(1, (int)(pt / 7));

            }

            scale /= defaultZoom;
        }

        public static BitmapText CreateText(float size)
        {
            return CreateText(null, size);
        }

        public static BitmapText CreateText(string text, float size)
        {
            ChooseFont(size);

            var result = new BitmapText(text, font);
            result.Scale.Set(scale);

            return result;
        }

        public static BitmapTextMultiline CreateMultiline(float size)
        {
            return CreateMultiline(null, size);
        }

        public static BitmapTextMultiline CreateMultiline(string text, float size)
        {
            ChooseFont(size);

            var result = new BitmapTextMultiline(text, font);
            result.Scale.Set(scale);

            return result;
        }

        public static float Align(Camera camera, float pos)
        {
            return ((int)(pos * camera.Zoom)) / camera.Zoom;
        }

        public static float Align(float pos)
        {
            return ((int)(pos * defaultZoom)) / defaultZoom;
        }

        public static void Align(Visual v)
        {
            var c = v.Camera;
            v.X = Align(c, v.X);
            v.Y = Align(c, v.Y);
        }

        public static bool NoFade = false;
        protected internal virtual void FadeIn()
        {
            if (NoFade)
            {
                NoFade = false;
            }
            else
            {
                FadeIn(Color.Argb(0xFF, 0x00, 0x00, 0x00), false);
            }
        }

        protected internal virtual void FadeIn(Color color, bool light)
        {
            Add(new Fader(color, light));
        }

        public static void ShowBadge(Badge badge)
        {
            var banner = BadgeBanner.Show(badge.Image);
            banner.Camera = uiCamera;
            banner.X = Align(banner.Camera, (banner.Camera.CameraWidth - banner.Width) / 2);
            banner.Y = Align(banner.Camera, (banner.Camera.CameraHeight - banner.Height) / 3);
            Game.Scene.Add(banner);
        }

        protected internal class Fader : ColorBlock
        {
            private const float FadeTime = 1f;

            private readonly bool _light;

            private float _time;

            public Fader(Color color, bool light)
                : base(uiCamera.CameraWidth, uiCamera.CameraHeight, color)
            {

                _light = light;

                Camera = uiCamera;

                Alpha(1f);
                _time = FadeTime;
            }

            public override void Update()
            {

                base.Update();

                if ((_time -= Game.Elapsed) <= 0)
                {
                    Alpha(0f);
                    Parent.Remove(this);
                }
                else
                    Alpha(_time / FadeTime);
            }

            public override void Draw()
            {
                if (_light)
                {
                    GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOne);
                    base.Draw();
                    GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);
                }
                else
                {
                    base.Draw();
                }
            }
        }

        private class PixelCamera : Camera
        {

            public PixelCamera(float zoom)
                : base((int)(Game.Width - Math.Ceiling(Game.Width / zoom) * zoom) / 2, (int)(Game.Height - Math.Ceiling(Game.Height / zoom) * zoom) / 2, (int)Math.Ceiling(Game.Width / zoom), (int)Math.Ceiling(Game.Height / zoom), zoom)
            {
            }

            protected override void UpdateMatrix()
            {
                float sx = Align(this, Scroll.X + ShakeX);
                float sy = Align(this, Scroll.Y + ShakeY);

                Matrix[0] = +Zoom * InvW2;
                Matrix[5] = -Zoom * InvH2;

                Matrix[12] = -1 + X * InvW2 - sx * Matrix[0];
                Matrix[13] = +1 - Y * InvH2 - sy * Matrix[5];

            }
        }
    }

}