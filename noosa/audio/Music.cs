using System;
using Android.Media;
using Java.IO;

namespace pdsharp.noosa.audio
{
    public class Music : object, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnErrorListener
    {
        private static Music _instance;
        public static Music Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Music();

                return _instance;
            }
        }

        private MediaPlayer _player;

        private String _lastPlayed;

        private bool _lastLooping;

        private bool _enabled = true;


        public void Dispose()
        {

        }

        public IntPtr Handle { get; private set; }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
            if (_player == null)
                return true;

            _player.Release();
            _player = null;
            return true;
        }

        public void OnPrepared(MediaPlayer mp)
        {
            _player.Start();
        }

        public void Play(string assetName, bool looping)
        {
            if (_lastPlayed != null && (_player != null && (_player.IsPlaying && _lastPlayed.Equals(assetName))))
                return;

            Stop();

            _lastPlayed = assetName;
            _lastLooping = looping;

            if (!_enabled || assetName == null)
                return;

            try
            {

                var afd = Game.Instance.Assets.OpenFd(assetName);

                _player = new MediaPlayer();
                _player.SetAudioStreamType(Stream.Music);
                _player.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
                _player.SetOnPreparedListener(_instance);
                _player.Prepared += _player_Prepared;
                _player.Error += _player_Error;
                _player.SetOnErrorListener(_instance);
                _player.Looping = looping;
                _player.PrepareAsync();

            }
            catch (IOException)
            {
                _player.Release();
                _player = null;
            }
        }

        void _player_Error(object sender, MediaPlayer.ErrorEventArgs e)
        {
            OnError(sender as MediaPlayer, e.What, e.Extra);
        }

        void _player_Prepared(object sender, EventArgs e)
        {
            OnPrepared(sender as MediaPlayer);
        }

        public void Mute()
        {
            _lastPlayed = null;
            Stop();
        }

        public void Pause()
        {
            if (_player != null)
                _player.Pause();
        }

        public void Resume()
        {
            if (_player != null)
                _player.Start();
        }

        public void Stop()
        {
            if (_player == null)
                return;

            _player.Stop();
            _player.Prepared -= _player_Prepared;
            _player.Error -= _player_Error;
            _player.Release();
            _player = null;
        }

        public void Volume(float value)
        {
            if (_player != null)
                _player.SetVolume(value, value);
        }

        public bool IsPlaying()
        {
            return _player != null && _player.IsPlaying;
        }

        public void Enable(bool value)
        {
            _enabled = value;
            if (IsPlaying() && !value)
                Stop();
            else
                if (!IsPlaying() && value)
                    Play(_lastPlayed, _lastLooping);
        }

        public bool IsEnabled()
        {
            return _enabled;
        }
    }
}