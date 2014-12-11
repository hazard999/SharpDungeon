using System;
using System.Drawing;
using pdsharp.utils;
using Point = pdsharp.utils.Point;
using PointF = pdsharp.utils.PointF;

namespace pdsharp.noosa
{
    public class Visual : Gizmo
    {
        public float X;
        public float Y;
        protected float _Width;
        protected float _Height;

        public PointF Scale;
        public PointF Origin;

        protected internal float[] Matrix;

        public float Rm;
        public float Gm;
        public float Bm;
        public float Am;
        public float RA;
        public float Ga;
        public float Ba;
        public float Aa;

        public PointF Speed;
        public PointF Acc;

        public float Angle;
        public float AngularSpeed;

        public Visual(float x, float y, float Width, float Height)
        {
            X = x;
            Y = y;
            _Width = Width;
            _Height = Height;

            Scale = new PointF(1, 1);
            Origin = new PointF();

            Matrix = new float[16];

            ResetColor();

            Speed = new PointF();
            Acc = new PointF();
        }


        public override void Update()
        {
            UpdateMotion();
        }

        public override void Draw()
        {
            UpdateMatrix();
        }

        protected virtual void UpdateMatrix()
        {
            glwrap.Matrix.Identity = Matrix;
            glwrap.Matrix.Translate(Matrix, X, Y);
            glwrap.Matrix.Translate(Matrix, Origin.X, Origin.Y);

            if (Math.Abs(Angle) > Tolerance)
                glwrap.Matrix.Rotate(Matrix, Angle);

            if (Math.Abs(Scale.X - 1) > Tolerance || Math.Abs(Scale.Y - 1) > 0.0001)
                glwrap.Matrix.Scale(Matrix, Scale.X, Scale.Y);

            glwrap.Matrix.Translate(Matrix, -Origin.X, -Origin.Y);
        }

        private static double Tolerance
        {
            get { return 0.0001; }
        }

        public virtual PointF Point()
        {
            return new PointF(X, Y);
        }

        public virtual PointF Point(PointF p)
        {
            X = p.X;
            Y = p.Y;
            return p;
        }

        public virtual Point Point(Point p)
        {
            X = p.X;
            Y = p.Y;
            return p;
        }

        public virtual PointF Center()
        {
            return new PointF(X + _Width / 2, Y + _Height / 2);
        }

        public virtual PointF Center(PointF p)
        {
            X = p.X - _Width / 2;
            Y = p.Y - _Height / 2;
            return p;
        }

        public virtual float Width
        {
            get { return _Width * Scale.X; }
            set { _Width = value; }
        }

        public virtual float Height
        {
            get { return _Height * Scale.Y; }
            set { _Height = value; }
        }

        protected internal virtual void UpdateMotion()
        {

            float elapsed = Game.Elapsed;

            float d = (GameMath.Speed(Speed.X, Acc.X) - Speed.X) / 2;
            Speed.X += d;
            X += Speed.X * elapsed;
            Speed.X += d;

            d = (GameMath.Speed(Speed.Y, Acc.Y) - Speed.Y) / 2;
            Speed.Y += d;
            Y += Speed.Y * elapsed;
            Speed.Y += d;

            Angle += AngularSpeed * elapsed;
        }

        public virtual void Alpha(float value)
        {
            Am = value;
            Aa = 0;
        }

        public virtual float Alpha()
        {
            return Am + Aa;
        }

        public virtual void Invert()
        {
            Rm = Gm = Bm = -1f;
            RA = Ga = Ba = +1f;
        }

        public virtual void Lightness(float value)
        {
            if (value < 0.5f)
            {
                Rm = Gm = Bm = value * 2f;
                RA = Ga = Ba = 0;
            }
            else
            {
                Rm = Gm = Bm = 2f - value * 2f;
                RA = Ga = Ba = value * 2f - 1f;
            }
        }

        public virtual void Brightness(float value)
        {
            Rm = Gm = Bm = value;
        }

        public virtual void Tint(float r, float g, float b, float strength)
        {
            Rm = Gm = Bm = 1f - strength;
            RA = r * strength;
            Ga = g * strength;
            Ba = b * strength;
        }

        public virtual void Tint(int color, float strength)
        {
            Rm = Gm = Bm = 1f - strength;
            RA = ((color >> 16) & 0xFF) / 255f * strength;
            Ga = ((color >> 8) & 0xFF) / 255f * strength;
            Ba = (color & 0xFF) / 255f * strength;
        }

        public virtual void Color(float r, float g, float b)
        {
            Rm = Gm = Bm = 0;
            RA = r;
            Ga = g;
            Ba = b;
        }

        public virtual void Color(int color)
        {
            Color(((color >> 16) & 0xFF) / 255f, ((color >> 8) & 0xFF) / 255f, (color & 0xFF) / 255f);
        }

        public virtual void Hardlight(float r, float g, float b)
        {
            RA = Ga = Ba = 0;
            Rm = r;
            Gm = g;
            Bm = b;
        }

        public virtual void Hardlight(int color)
        {
            Hardlight((color >> 16) / 255f, ((color >> 8) & 0xFF) / 255f, (color & 0xFF) / 255f);
        }

        public virtual void ResetColor()
        {
            Rm = Gm = Bm = Am = 1;
            RA = Ga = Ba = Aa = 0;
        }

        public virtual bool OverlapsPoint(float x, float y)
        {
            var rect = new RectangleF(X, Y, Width, Height);
            var p = new System.Drawing.PointF(x, y);
            var result = rect.Contains(p);

            return result;

            //var Result = x >= X && x < X + Width && y >= Y && y < Y + Height;

            //return Result;
        }

        public virtual bool OverlapsScreenPoint(int x, int y)
        {
            var c = this.Camera;
            if (c == null)
                return false;

            var p = c.ScreenToCamera(x, y);
            return OverlapsPoint(p.X, p.Y);
        }

        // true if its bounding box intersects its camera's bounds
        public override bool IsVisible
        {
            get
            {
                var c = this.Camera;
                var cx = c.Scroll.X;
                var cy = c.Scroll.Y;
                var w = Width;
                var h = Height;
                return X + w >= cx && Y + h >= cy && X < cx + c.CameraWidth && Y < cy + c.CameraHeight;
            }
        }
    }

}