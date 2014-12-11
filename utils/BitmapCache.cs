using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Java.IO;

namespace pdsharp.utils
{
    public class BitmapCache
    {
        private const string Default = "__default";

        private static readonly Dictionary<string, Layer> Layers = new Dictionary<string, BitmapCache.Layer>();

        private static readonly BitmapFactory.Options Opts = new BitmapFactory.Options();
        static BitmapCache()
        {
            Opts.InDither = false;
        }

        public static Context Context;

        public static Bitmap Get(string assetName)
        {
            return Get(Default, assetName);
        }

        public static Bitmap Get(string layerName, string assetName)
        {
            Layer layer;
            if (!Layers.ContainsKey(layerName))
            {
                layer = new Layer();
                Layers.Add(layerName, layer);
            }
            else
                layer = Layers[layerName];

            if (layer.ContainsKey(assetName))
                return layer[assetName];

            try
            {
                var stream = Context.Resources.Assets.Open(assetName);
                var bmp = BitmapFactory.DecodeStream(stream, null, Opts);
                layer.Add(assetName, bmp);
                return bmp;
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static Bitmap Get(int resID)
        {
            return Get(Default, resID);
        }

        public static Bitmap Get(string layerName, int resID)
        {
            Layer layer;
            if (!Layers.ContainsKey(layerName))
            {
                layer = new Layer();
                Layers.Add(layerName, layer);
            }
            else
                layer = Layers[layerName];

            if (layer.ContainsKey(resID))
                return layer[resID];

            var bmp = BitmapFactory.DecodeResource(Context.Resources, resID);
            layer.Add(resID, bmp);
            return bmp;
        }

        public static void Clear(string layerName)
        {
            if (!Layers.ContainsKey(layerName)) 
                return;

            Layers[layerName].Clear();
            Layers.Remove(layerName);
        }

        public static void Clear()
        {
            foreach (var layer in Layers.Values)
                layer.Clear();

            Layers.Clear();
        }

        private class Layer : Dictionary<object, Bitmap>
        {
            public new void Clear()
            {
                foreach (var bmp in Values)
                    bmp.Recycle();

                base.Clear();
            }
        }
    }
}