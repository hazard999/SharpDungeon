using System;
using Android.Graphics;

namespace pdsharp.noosa
{
    public interface IListener
    {
        void OnComplete(MovieClip.Animation anim);
    }

    public class MovieClip : Image
    {

        protected internal Animation curAnim;
        protected internal int curFrame;
        protected internal float frameTimer;
        protected internal bool finished;

        public bool paused = false;

        public IListener Listener;

        public MovieClip()
        {
        }

        public MovieClip(object tx) : base(tx)
        {
        }

        public override void Update()
        {
            base.Update();
            if (!paused)
            {
                UpdateAnimation();
            }
        }

        protected internal virtual void UpdateAnimation()
        {
            if (curAnim != null && curAnim.delay > 0 && (curAnim.looped || !finished))
            {

                int lastFrame = curFrame;

                frameTimer += Game.Elapsed;
                while (frameTimer > curAnim.delay)
                {
                    frameTimer -= curAnim.delay;
                    if (curFrame == curAnim.frames.Length - 1)
                    {
                        if (curAnim.looped)
                        {
                            curFrame = 0;
                        }
                        finished = true;
                        if (Listener != null)
                        {
                            Listener.OnComplete(curAnim);
                        // This check can probably be removed
                            if (curAnim == null)
                            {
                                return;
                            }
                        }

                    }
                    else
                    {
                        curFrame++;
                    }
                }

                if (curFrame != lastFrame)
                {
                    Frame(curAnim.frames[curFrame]);
                }

            }
        }

        public virtual void Play(Animation anim)
        {
            Play(anim, false);
        }

        public virtual void Play(Animation anim, bool force)
        {
            if (!force && (curAnim != null) && (curAnim == anim) && (curAnim.looped || !finished))
                return;

            curAnim = anim;
            curFrame = 0;
            finished = false;

            frameTimer = 0;

            if (anim != null)
                Frame(anim.frames[curFrame]);
        }

        public class Animation
        {
            public float delay;
            public RectF[] frames;
            public bool looped;

            public Animation(int fps, bool looped)
            {
                this.delay = 1f / fps;
                this.looped = looped;
            }

            public virtual Animation Frames(params RectF[] frames)
            {
                this.frames = frames;
                return this;
            }

            public virtual Animation Frames(TextureFilm film, params object[] frames)
            {
                this.frames = new RectF[frames.Length];
                for (int i=0; i < frames.Length; i++)
                {
                    this.frames[i] = film.Get(frames[i]);
                }
                return this;
            }

            public virtual Animation Clone()
            {
                return new Animation((int)Math.Round(1 / delay), looped).Frames(frames);
            }
        }
    }
}