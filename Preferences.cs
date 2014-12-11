using System;
using Android.Content;
using pdsharp.noosa;

namespace sharpdungeon
{
    internal class Preferences
    {
        private static Preferences _Instance;

        public static Preferences Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Preferences();

                return _Instance;
            }
        }

        public static String KEY_LANDSCAPE = "landscape";
        public static String KEY_IMMERSIVE = "immersive";
        public static String KEY_SCALE_UP = "scaleup";
        public static String KEY_MUSIC = "music";
        public static String KEY_SOUND_FX = "soundfx";
        public static String KEY_ZOOM = "zoom";
        public static String KEY_LAST_CLASS = "last_class";
        public static String KEY_CHALLENGES = "challenges";
        public static String KEY_DONATED = "donated";
        public static String KEY_INTRO = "intro";
        public static String KEY_BRIGHTNESS = "brightness";
        
        private ISharedPreferences _prefs;
        private ISharedPreferences Get()
        {
            if (_prefs == null)
                _prefs = Game.Instance.GetPreferences(FileCreationMode.Private);
            return _prefs;
        }

        internal int GetInt(string key, int defValue)
        {
            return Get().GetInt(key, defValue);
        }
        internal bool GetBoolean(string key, bool defValue)
        {
            return Get().GetBoolean(key, defValue);
        }
        internal string GetString(string key, string defValue)
        {
            return Get().GetString(key, defValue);
        }
        internal void Put(string key, int value)
        {
            Get().Edit().PutInt(key, value).Commit();
        }
        internal void Put(string key, bool value)
        {
            Get().Edit().PutBoolean(key, value).Commit();
        }
        internal void Put(string key, string value)
        {
            Get().Edit().PutString(key, value).Commit();
        }
    }
}