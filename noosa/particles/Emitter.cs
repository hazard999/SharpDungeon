using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.utils;

namespace pdsharp.noosa.particles
{
    public class Emitter : Group
    {
        protected internal bool LightMode;

        public float x;
        public float y;
        public float Width;
        public float Height;

        protected internal Visual Target;

        protected internal float Interval;
        protected internal int Quantity;

        public bool On;

        public bool AutoKill = true;

        protected internal int Count;
        protected internal float Time;

        protected internal Factory factory;

        public virtual void Pos(float x, float y)
        {
            Pos(x, y, 0, 0);
        }

        public virtual void Pos(PointF p)
        {
            Pos(p.X, p.Y, 0, 0);
        }

        public virtual void Pos(float x, float y, float Width, float Height)
        {
            this.x = x;
            this.y = y;
            this.Width = Width;
            this.Height = Height;

            Target = null;
        }

        public virtual void Pos(Visual target)
        {
            this.Target = target;
        }

        public virtual void Burst(Factory factory, int quantity)
        {
            Start(factory, 0, quantity);
        }

        public virtual void Pour(Factory factory, float interval)
        {
            Start(factory, interval, 0);
        }

        public virtual void Start(Factory factory, float interval, int quantity)
        {

            this.factory = factory;
            this.LightMode = factory.LightMode();

            this.Interval = interval;
            this.Quantity = quantity;

            Count = 0;
            Time = utils.Random.Float(interval);

            On = true;
        }

        public override void Update()
        {

            if (On)
            {
                Time += Game.Elapsed;
                while (Time > Interval)
                {
                    Time -= Interval;
                    Emit(Count);
                    if (Quantity > 0 && ++Count >= Quantity)
                    {
                        On = false;
                        break;
                    }
                }
            }
            else if (AutoKill && CountLiving() == 0)
            {
                Kill();
            }

            base.Update();
        }

        protected virtual void Emit(int index)
        {
            if (Target == null)
            {
                factory.Emit(this, index, x + utils.Random.Float(Width), y + utils.Random.Float(Height));
            }
            else
            {
                factory.Emit(this, index, Target.X + utils.Random.Float(Target.Width), Target.Y + utils.Random.Float(Target.Height));
            }
        }

        public override void Draw()
        {
            if (LightMode)
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

        public abstract class Factory
        {
            public abstract void Emit(Emitter emitter, int index, float x, float y);

            public virtual bool LightMode()
            {
                return false;
            }
        }
    }
}