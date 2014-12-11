using System;
using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.Media;
using Android.Opengl;
using Android.OS;
using Android.Util;
using Android.Views;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.glscripts;
using pdsharp.gltextures;
using pdsharp.input;
using pdsharp.noosa.audio;
using pdsharp.utils;
using Bundle = Android.OS.Bundle;
using EGLConfig = Javax.Microedition.Khronos.Egl.EGLConfig;

namespace pdsharp.noosa
{
    public class Game : Activity, GLSurfaceView.IRenderer, View.IOnTouchListener
    {
        public static Game Instance;

        public Game(Type sceneToShow)
        {
            sceneClass = sceneToShow;
            now = SystemTime.Now;
        }

        // Actual Size of the screen
        public static int Width;
        public static int Height;

        // Density: mdpi=1, hdpi=1.5, xhdpi=2...
        public static float density = 1;

        public static string version;

        // Current scene
        protected internal Scene scene;
        // New scene we are going to switch to
        protected internal Scene requestedScene;
        // true if scene switch is requested
        protected internal bool requestedReset = true;

        // New scene class
        protected internal Type sceneClass;

        // Current time in milliseconds
        protected internal long now;
        // Milliseconds passed since previous update 
        protected internal long step;

        public static float timeScale = 1f;
        public static float Elapsed = 0f;

        protected internal GLSurfaceView view;
        //protected internal SurfaceHolder holder;

        // Accumulated touch events
        protected internal List<MotionEvent> motionEvents = new List<MotionEvent>();

        // Accumulated key events
        protected internal List<KeyEvent> keysEvents = new List<KeyEvent>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            BitmapCache.Context = TextureCache.Context = Instance = this;

            DisplayMetrics m = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(m);
            density = m.Density;

            try
            {
                version = PackageManager.GetPackageInfo(PackageName, 0).VersionName;
            }
            catch (PackageManager.NameNotFoundException)
            {
                version = "???";
            }

            VolumeControlStream = Stream.Music;

            view = new GLSurfaceView(this);
            view.SetEGLContextClientVersion(2);
            view.SetEGLConfigChooser(false);
            view.SetRenderer(this);
            view.SetOnTouchListener(this);
            SetContentView(view);
        }

        protected override void OnResume()
        {
            base.OnResume();

            now = SystemTime.Now;
            view.OnResume();

            Music.Instance.Resume();
            Sample.Instance.Resume();
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (Scene != null)
                Scene.Pause();

            view.OnPause();
            Script.Reset();

            Music.Instance.Pause();
            Sample.Instance.Pause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DestroyGame();

            Music.Instance.Mute();
            Sample.Instance.Reset();
        }

        public bool OnTouch(View view, MotionEvent @event)
        {
            lock (motionEvents)
            {
                motionEvents.Add(MotionEvent.Obtain(@event));
            }
            return true;
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent keyEvent)
        {
            if (keyCode == Keys.VolumeDown || keyCode == Keys.VolumeUp)
            {

                return false;
            }

            lock (motionEvents)
            {
                keysEvents.Add(keyEvent);
            }
            return true;
        }

        public override bool OnKeyUp(Keycode keyCode, KeyEvent keyEvent)
        {
            if (keyCode == Keys.VolumeDown || keyCode == Keys.VolumeUp)
                return false;

            lock (motionEvents)
            {
                keysEvents.Add(keyEvent);
            }
            return true;
        }

        public void OnDrawFrame(IGL10 gl)
        {
            if (Width == 0 || Height == 0)
                return;

            SystemTime.Tick();
            var rightNow = SystemTime.Now;
            step = (now == 0 ? 0 : rightNow - now);
            now = rightNow;

            if (step <= 0)
                return;

            Step();

            NoosaScript.Get().ResetCamera();
            GLES20.GlScissor(0, 0, Width, Height);
            GLES20.GlClear(GLES20.GlColorBufferBit);
            Draw();
        }

        public virtual void OnSurfaceChanged(IGL10 gl, int width, int height)
        {
            GLES20.GlViewport(0, 0, width, height);

            Width = width;
            Height = height;
        }

        public void OnSurfaceCreated(IGL10 gl, EGLConfig config)
        {
            GLES20.GlEnable(GL10.GlBlend);
            // For premultiplied alpha:
            // GLES20.GlBlendFunc( GL10.GL_ONE, GL10.GL_ONE_MINUS_SRC_ALPHA );
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);

            GLES20.GlEnable(GL10.GlScissorTest);

            TextureCache.Reload();
        }

        protected internal virtual void DestroyGame()
        {
            if (Scene != null)
            {
                Scene.Destroy();
                sceneClass = null;
            }

            Instance = null;
        }

        public static void ResetScene()
        {
            SwitchScene(Instance.sceneClass);
        }

        public static void SwitchScene(Type typeOfScene)
        {
            Instance.sceneClass = typeOfScene;
            Instance.requestedReset = true;
        }

        public static void SwitchScene<T1>() where T1 : Scene
        {
            Instance.sceneClass = typeof(T1);
            Instance.requestedReset = true;
        }

        public static Scene Scene
        {
            get
            {
                return Instance.scene;
            }
        }

        protected internal virtual void Step()
        {
            if (requestedReset)
            {
                requestedReset = false;
                try
                {
                    requestedScene = (Scene)Activator.CreateInstance(sceneClass);
                    SwitchScene();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine((e.StackTrace));
                }
            }

            Update();
        }

        protected internal virtual void Draw()
        {
            Scene.Draw();
        }

        protected internal virtual void SwitchScene()
        {
            Camera.Reset();

            if (Scene != null)
                Scene.Destroy();

            scene = requestedScene;
            scene.Create();

            Elapsed = 0f;
            timeScale = 1f;
        }

        protected internal virtual void Update()
        {
            Elapsed = timeScale * step * 0.001f;

            lock (motionEvents)
            {
                Touchscreen.ProcessTouchEvents(motionEvents);
                motionEvents.Clear();
            }
            lock (keysEvents)
            {
                Keys.ProcessTouchEvents(keysEvents);
                keysEvents.Clear();
            }

            Scene.Update();
            Camera.UpdateAll();
        }

        public static void Vibrate(int milliseconds)
        {
            ((Vibrator)Instance.GetSystemService(VibratorService)).Vibrate(milliseconds);
        }
    }

}