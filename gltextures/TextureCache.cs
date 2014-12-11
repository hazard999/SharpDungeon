using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;

namespace pdsharp.gltextures
{
    public class TextureCache
    {
        public static Context Context;

        private static readonly Dictionary<object, SmartTexture> All = new Dictionary<object, SmartTexture>();

        // No dithering, no scaling, 32 bits per pixel
        private static readonly BitmapFactory.Options BitmapOptions = new BitmapFactory.Options();
        static TextureCache()
        {
            BitmapOptions.InScaled = false;
            BitmapOptions.InDither = false;
            BitmapOptions.InPreferredConfig = Bitmap.Config.Argb8888;
        }

        public static SmartTexture CreateSolid(Color color)
        {
            var key = "1x1:" + color;

            if (All.ContainsKey(key))
                return All[key];

            var bmp = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888);
            bmp.EraseColor(color);

            var tx = new SmartTexture(bmp);
            All.Add(key, tx);

            return tx;
        }

        public static SmartTexture CreateGradient(int Width, int Height, params int[] colors)
        {
            var key = "" + Width + "x" + Height + ":" + colors;

            if (All.ContainsKey(key))
                return All[key];

            var bmp = Bitmap.CreateBitmap(Width, Height, Bitmap.Config.Argb8888);
            var canvas = new Canvas(bmp);
            var paint = new Paint();
            paint.SetShader(new LinearGradient(0, 0, 0, Height, colors, null, Shader.TileMode.Clamp));
            canvas.DrawPaint(paint);

            var tx = new SmartTexture(bmp);
            All.Add(key, tx);
            return tx;
        }

        public static void Add(object key, SmartTexture tx)
        {
            if (All.ContainsKey(key))
                All[key] = tx;
            else
                All.Add(key, tx);
        }

        public static SmartTexture Get(object src)
        {
            if (All.ContainsKey(src))
                return All[src];

            var texture = src as SmartTexture;
            if (texture != null)
                return texture;

            var tx = new SmartTexture(GetBitmap(src));
            All.Add(src, tx);
            return tx;
        }

        public static void Clear()
        {
            foreach (var txt in All.Values)
                txt.Delete();

            All.Clear();
        }

        public static void Reload()
        {
            foreach (var tx in All.Values)
                tx.Reload();
        }

        public static Bitmap GetBitmap(object src)
        {
            try
            {
                if (src is int?)
                    return BitmapFactory.DecodeResource(Context.Resources, (int)src, BitmapOptions);

                var strSrc = src as string;
                if (strSrc != null)
                    return BitmapFactory.DecodeStream(Context.Assets.Open(strSrc), null, BitmapOptions);

                var bitmap = src as Bitmap;
                if (bitmap != null)
                    return bitmap;

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        public static bool Contains(object key)
        {
            return All.ContainsKey(key);
        }
    }
}