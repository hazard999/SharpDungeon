using System;
using System.Collections.Generic;
using System.Linq;
using Android.Media;
using Java.IO;

namespace pdsharp.noosa.audio
{
    public class Sample : object, SoundPool.IOnLoadCompleteListener
    {
        private static Sample _instance;
        public static Sample Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Sample();

                return _instance;
            }
        }

        public static int MaxNumberOfStreams = 8;

        private SoundPool Pool = new SoundPool(MaxNumberOfStreams, Stream.Music, 0);

        protected Dictionary<Object, int> Ids = new Dictionary<Object, int>();

        private bool _enabled = true;


        public void Dispose()
        {

        }

        public IntPtr Handle { get; private set; }


        public void Reset()
        {
            Pool.Release();

            Pool = new SoundPool(MaxNumberOfStreams, Stream.Music, 0);

            Ids.Clear();
        }

        public void Pause()
        {
            if (Pool != null)
                Pool.AutoPause();
        }

        public void Resume()
        {
            if (Pool != null)
                Pool.AutoResume();
        }

        public void Load(params string[] assets)
        {
            var manager = Game.Instance.Assets;

            foreach (var asset in assets.Where(asset => !Ids.ContainsKey(asset)))
            {
                try
                {
                    var fd = manager.OpenFd(asset);
                    var streamID = Pool.Load(fd, 1);
                    Ids.Add(asset, streamID);
                    fd.Close();
                }
                catch (IOException)
                {
                }
            }
        }

        public void Unload(object src)
        {
            if (!Ids.ContainsKey(src))
                return;

            Pool.Unload(Ids[src]);
            Ids.Remove(src);
        }

        public int Play(object id)
        {
            return Play(id, 1, 1, 1);
        }

        public int Play(object id, float volume)
        {
            return Play(id, volume, volume, 1);
        }

        public int Play(object id, float leftVolume, float rightVolume, float rate)
        {
            if (_enabled && Ids.ContainsKey(id))
                return Pool.Play(Ids[id], leftVolume, rightVolume, 0, 0, rate);

            return -1;
        }

        public void Enable(bool value)
        {
            _enabled = value;
        }

        public bool IsEnabled()
        {
            return _enabled;
        }

        public void OnLoadComplete(SoundPool soundPool, int sampleId, int status)
        {
        }
    }
}