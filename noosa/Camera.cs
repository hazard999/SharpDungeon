using System;
using System.Collections.Generic;
using System.Linq;
using pdsharp.utils;
using Random = pdsharp.utils.Random;

namespace pdsharp.noosa
{
    public class Camera : Gizmo
    {
        public static Camera CreateFullscreen(float zoom)
        {
            var w = (int)Math.Ceiling(Game.Width / zoom);
            var h = (int)Math.Ceiling(Game.Height / zoom);
            return new Camera((int)(Game.Width - w * zoom) / 2, (int)(Game.Height - h * zoom) / 2, w, h, zoom);
        }

        public Camera(int x, int y, int cameraWidth, int cameraHeight, float zoomTo)
        {
            X = x;
            Y = y;
            CameraWidth = cameraWidth;
            CameraHeight = cameraHeight;
            Zoom = zoomTo;

            Scroll = new PointF();

            Matrix = new float[16];
            glwrap.Matrix.Identity = Matrix;
        }

        protected internal static List<Camera> AllCameras = new List<Camera>();

        protected internal static float InvW2;
        protected internal static float InvH2;

        public static Camera Main;

        public float Zoom { get; set; }

        public int X;
        public int Y;
        public int CameraWidth;
        public int CameraHeight;

        public float[] Matrix;

        public PointF Scroll;
        public Visual Target;

        private float _shakeMagX = 10f;
        private float _shakeMagY = 10f;
        private float _shakeTime;
        private float _shakeDuration = 1f;

        protected internal float ShakeX;
        protected internal float ShakeY;

        public static Camera Reset()
        {
            return Reset(CreateFullscreen(1));
        }

        public static Camera Reset(Camera newCamera)
        {
            InvW2 = 2f / Game.Width;
            InvH2 = 2f / Game.Height;

            foreach (var camera in AllCameras)
                camera.Destroy();
            AllCameras.Clear();

            return Main = Add(newCamera);
        }

        public static Camera Add(Camera camera)
        {
            AllCameras.Add(camera);
            return camera;
        }

        public static Camera Remove(Camera camera)
        {
            AllCameras.Remove(camera);
            return camera;
        }

        public static void UpdateAll()
        {
            foreach (var c in AllCameras.Where(c => c.Exists && c.Active))
                c.Update();
        }

        public override void Destroy()
        {
            Target = null;
            Matrix = null;
        }

        public virtual void ZoomTo(float value)
        {
            ZoomTo(value, Scroll.X + CameraWidth / 2, Scroll.Y + CameraHeight / 2);
        }

        public virtual void ZoomTo(float value, float fx, float fy)
        {
            Zoom = value;
            CameraWidth = (int)(ScreenWidth / Zoom);
            CameraHeight = (int)(ScreenHeight / Zoom);

            FocusOn(fx, fy);
        }

        public virtual void Resize(int resizeToWidth, int resizeToHeight)
        {
            CameraWidth = resizeToWidth;
            CameraHeight = resizeToHeight;
        }

        public override void Update()
        {
            base.Update();

            if (Target != null)
                FocusOn(Target);

            if ((_shakeTime -= Game.Elapsed) > 0)
            {
                var damping = _shakeTime / _shakeDuration;
                ShakeX = Random.Float(-_shakeMagX, +_shakeMagX) * damping;
                ShakeY = Random.Float(-_shakeMagY, +_shakeMagY) * damping;
            }
            else
            {
                ShakeX = 0;
                ShakeY = 0;
            }

            UpdateMatrix();
        }

        public virtual PointF Center()
        {
            return new PointF(CameraWidth / 2, CameraHeight / 2);
        }

        public virtual bool HitTest(float hitX, float hitY)
        {
            return hitX >= X && hitY >= Y && hitX < X + ScreenWidth && hitY < Y + ScreenHeight;
        }

        public virtual void FocusOn(float focusX, float focusY)
        {
            Scroll.Set(focusX - CameraWidth / 2, focusY - CameraHeight / 2);
        }

        public virtual void FocusOn(PointF point)
        {
            FocusOn(point.X, point.Y);
        }

        public virtual void FocusOn(Visual visual)
        {
            FocusOn(visual.Center());
        }

        public virtual PointF ScreenToCamera(int screenX, int screenY)
        {
            return new PointF((screenX - X) / Zoom + Scroll.X, (screenY - Y) / Zoom + Scroll.Y);
        }

        public virtual Point CameraToScreen(float cameraX, float cameraY)
        {
            return new Point((int)((cameraX - Scroll.X) * Zoom + X), (int)((cameraY - Scroll.Y) * Zoom + Y));
        }

        public virtual float ScreenWidth
        {
            get { return CameraWidth * Zoom; }
        }

        public virtual float ScreenHeight
        {
            get { return CameraHeight * Zoom; }
        }

        protected virtual void UpdateMatrix()
        {
            Matrix[0] = +Zoom * InvW2;
            Matrix[5] = -Zoom * InvH2;

            Matrix[12] = -1 + X * InvW2 - (Scroll.X + ShakeX) * Matrix[0];
            Matrix[13] = +1 - Y * InvH2 - (Scroll.Y + ShakeY) * Matrix[5];

        }

        public virtual void Shake(float magnitude, float duration)
        {
            _shakeMagX = _shakeMagY = magnitude;
            _shakeTime = _shakeDuration = duration;
        }
    }
}