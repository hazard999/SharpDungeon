using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using sharpdungeon.actors.hero;
using sharpdungeon.scenes;

namespace sharpdungeon
{
    [Activity(MainLauncher = true)]
    public class PixelDungeon : Game
    {
        public PixelDungeon()
            : base(typeof(TitleScene))
        {
            Instance = this;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            UpdateImmersiveMode();

            var metrics = new DisplayMetrics();
            Instance.WindowManager.DefaultDisplay.GetMetrics(metrics);
            bool landscape = metrics.WidthPixels > metrics.HeightPixels;

            if (Preferences.Instance.GetBoolean(Preferences.KEY_LANDSCAPE, false) != landscape)
                Landscape(!landscape);

            pdsharp.noosa.audio.Music.Instance.Enable(Music());
            Sample.Instance.Enable(SoundFx());
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
                UpdateImmersiveMode();
        }

        public static void SwitchNoFade<T>() where T : PixelScene
        {
            PixelScene.NoFade = true;
            SwitchScene<T>();
        }

        public static void Landscape(bool value)
        {
            Instance.RequestedOrientation = value ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;
            Preferences.Instance.Put(Preferences.KEY_LANDSCAPE, value);
        }

        public static bool Landscape()
        {
            return Width > Height;
        }

        private static bool _immersiveModeChanged;
        public static void Immerse(bool value)
        {
            Preferences.Instance.Put(Preferences.KEY_IMMERSIVE, value);
            
            //Instance.RunOnUiThread(new Runnable() { public void run() { updateImmersiveMode(); immersiveModeChanged = true; } });
        }

        public override void OnSurfaceChanged(IGL10 gl, int width, int height)
        {
            base.OnSurfaceChanged(gl, width, height);

            if (_immersiveModeChanged)
            {
                requestedReset = true;
                _immersiveModeChanged = false;
            }
        }

        public static void UpdateImmersiveMode()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat) 
                return;

            const SystemUiFlags flags = SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky;
            var flagResult = flags & Instance.Window.DecorView.WindowSystemUiVisibility;
            if (Immersed() || flagResult != SystemUiFlags.Visible)
                Instance.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            else
                Instance.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
        }

        public static bool Immersed()
        {
            return Preferences.Instance.GetBoolean(Preferences.KEY_IMMERSIVE, false);
        }

        // *****************************

        public static void ScaleUp(bool value)
        {
            Preferences.Instance.Put(Preferences.KEY_SCALE_UP, value);
            SwitchScene<TitleScene>();
        }

        public static bool ScaleUp()
        {
            return Preferences.Instance.GetBoolean(Preferences.KEY_SCALE_UP, true);
        }

        public static void Zoom(int value)
        {
            Preferences.Instance.Put(Preferences.KEY_ZOOM, value);
        }

        public static int Zoom()
        {
            return Preferences.Instance.GetInt(Preferences.KEY_ZOOM, 0);
        }

        public static void Music(bool value)
        {
            pdsharp.noosa.audio.Music.Instance.Enable(value);
            Preferences.Instance.Put(Preferences.KEY_MUSIC, value);
        }

        public static bool Music()
        {
            return Preferences.Instance.GetBoolean(Preferences.KEY_MUSIC, true);
        }

        public static void SoundFx(bool value)
        {
            Sample.Instance.Enable(value);
            Preferences.Instance.Put(Preferences.KEY_SOUND_FX, value);
        }

        public static bool SoundFx()
        {
            return Preferences.Instance.GetBoolean(Preferences.KEY_SOUND_FX, true);
        }

        public static void Brightness(bool value)
        {
            Preferences.Instance.Put(Preferences.KEY_BRIGHTNESS, value);
            var scene = Scene as GameScene;
            if (scene != null)
                scene.Brightness(value);
        }

        public static bool Brightness()
        {
            return Preferences.Instance.GetBoolean(Preferences.KEY_BRIGHTNESS, false);
        }

        public static void Donated(string value)
        {
            Preferences.Instance.Put(Preferences.KEY_DONATED, value);
        }

        public static string Donated()
        {
            return Preferences.Instance.GetString(Preferences.KEY_DONATED, "");
        }

        public static void LastClass(HeroClassType value)
        {
            Preferences.Instance.Put(Preferences.KEY_LAST_CLASS, (int)value);
        }

        public static int LastClass()
        {
            return Preferences.Instance.GetInt(Preferences.KEY_LAST_CLASS, 0);
        }

        public static void Challenges(int value)
        {
            Preferences.Instance.Put(Preferences.KEY_CHALLENGES, value);
        }

        public static int Challenges()
        {
            return Preferences.Instance.GetInt(Preferences.KEY_CHALLENGES, 0);
        }

        public static void Intro(bool value)
        {
            Preferences.Instance.Put(Preferences.KEY_INTRO, value);
        }

        public static bool Intro()
        {
            return Preferences.Instance.GetBoolean(Preferences.KEY_INTRO, true);
        }

        public static void ReportException(Exception e)
        {
            Log.Error("PD", e.StackTrace);
        }
    }
}