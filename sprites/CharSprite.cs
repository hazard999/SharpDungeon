using Android.Graphics;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.particles;
using pdsharp.noosa.tweeners;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items.potions;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.utils;
using System;
using PointF = pdsharp.utils.PointF;

namespace sharpdungeon.sprites
{
    public class CharSprite : MovieClip, pdsharp.noosa.IListener, pdsharp.noosa.tweeners.IListener
    {
        // Color constants for floating bitmapText
        public const int Default = 0xFFFFFF;
        public const int Positive = 0x00FF00;
        public const int Negative = 0xFF0000;
        public const int Warning = 0xFF8800;
        public const int Neutral = 0xFFFF00;

        private const float MoveInterval = 0.1f;
        private const float FlashInterval = 0.05f;

        public enum State
        {
            Burning,
            Levitating,
            Invisible,
            Paralysed,
            Frozen,
            Illuminated
        }

        protected internal Animation IdleAnimation;
        protected internal Animation RunAnimation;
        protected internal Animation AttackAnimation;
        protected internal Animation OperateAnimation;
        protected internal Animation ZapAnimation;
        protected internal Animation DieAnimation;

        protected internal ICallback AnimCallback;

        protected internal Tweener Motion;

        protected internal Emitter Burning;
        protected internal Emitter Levitation;

        protected internal IceBlock IceBlock;
        protected internal TorchHalo Halo;

        protected internal EmoIcon Emo;

        private float _flashTime;

        protected internal bool Sleeping;

        // Character owner
        public Character Ch;

        // The sprite is currently in motion
        public bool IsMoving;

        public CharSprite()
        {
            Listener = this;
        }

        public virtual void Link(Character ch)
        {
            Ch = ch;
            ch.Sprite = this;

            Place(ch.pos);
            TurnTo(ch.pos, pdsharp.utils.Random.Int(Level.Length));

            ch.UpdateSpriteState();
        }

        public virtual PointF WorldToCamera(int cell)
        {
            const int csize = DungeonTilemap.Size;

            return new PointF(((cell % Level.Width) + 0.5f) * csize - Width * 0.5f, ((cell / Level.Width) + 1.0f) * csize - Height);
        }

        public virtual void Place(int cell)
        {
            Point(WorldToCamera(cell));
        }

        public virtual void ShowStatus(int color, string text, params object[] args)
        {
            if (!Visible) return;
            if (args.Length > 0)
                text = Utils.Format(text, args);
            if (Ch != null)
                FloatingText.Show(X + Width * 0.5f, Y, Ch.pos, text, color);
            else
                FloatingText.Show(X + Width * 0.5f, Y, text, color);
        }

        public virtual void Idle()
        {
            Play(IdleAnimation);
        }

        public virtual void Move(int from, int to)
        {
            Play(RunAnimation);

            Motion = new PosTweener(this, WorldToCamera(to), MoveInterval);
            Motion.Listener = this;
            Parent.Add(Motion);

            IsMoving = true;

            TurnTo(from, to);

            if (Visible && Level.water[from] && !Ch.Flying)
                GameScene.Ripple(from);

            Ch.OnMotionComplete();
        }

        public virtual void InterruptMotion()
        {
            if (Motion != null)
                OnComplete(Motion);
        }

        public virtual void DoAttack(int cell)
        {
            TurnTo(Ch.pos, cell);
            Play(AttackAnimation);
        }

        public virtual void DoAttack(int cell, ICallback callback)
        {
            AnimCallback = callback;
            TurnTo(Ch.pos, cell);
            Play(AttackAnimation);
        }

        public virtual void DoOperate(int cell)
        {
            TurnTo(Ch.pos, cell);
            Play(OperateAnimation);
        }

        public virtual void DoZap(int cell)
        {
            TurnTo(Ch.pos, cell);
            Play(ZapAnimation);
        }

        public virtual void TurnTo(int from, int to)
        {
            var fx = from % Level.Width;
            var tx = to % Level.Width;
            if (tx > fx)
                flipHorizontal = false;
            else if (tx < fx)
                flipHorizontal = true;
        }

        public virtual void DoDie()
        {
            Sleeping = false;
            Play(DieAnimation);

            if (Emo != null)
                Emo.KillAndErase();
        }

        public virtual Emitter Emitter()
        {
            var emitter = GameScene.Emitter();
            emitter.Pos(this);
            return emitter;
        }

        public virtual Emitter CenterEmitter()
        {
            var emitter = GameScene.Emitter();
            emitter.Pos(Center());
            return emitter;
        }

        public virtual Emitter BottomEmitter()
        {
            var emitter = GameScene.Emitter();
            emitter.Pos(X, Y + Height, Width, 0);
            return emitter;
        }

        public virtual void Burst(Color color, int n)
        {
            if (Visible)
                Splash.At(Center(), color, n);
        }

        public virtual void BloodBurstA(PointF fromPoint, int damage)
        {
            if (!Visible) 
                return;

            var c = Center();
            var n = (int)Math.Min(9 * Math.Sqrt((double)damage / Ch.HT), 9);
            Splash.At(c, PointF.Angle(fromPoint, c), 3.1415926f / 2, Blood(), n);
        }

        // Blood color
        public virtual Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0xBB, 0x00, 0x00);
        }

        public virtual void Flash()
        {
            RA = Ba = Ga = 1f;
            _flashTime = FlashInterval;
        }

        public virtual void Add(State state)
        {
            switch (state)
            {
                case State.Burning:
                    Burning = Emitter();
                    Burning.Pour(FlameParticle.Factory, 0.06f);
                    if (Visible)
                        Sample.Instance.Play(Assets.SND_BURNING);
                    break;
                case State.Levitating:
                    Levitation = Emitter();
                    Levitation.Pour(Speck.Factory(Speck.JET), 0.02f);
                    break;
                case State.Invisible:
                    PotionOfInvisibility.Melt(Ch);
                    break;
                case State.Paralysed:
                    paused = true;
                    break;
                case State.Frozen:
                    IceBlock = IceBlock.Freeze(this);
                    paused = true;
                    break;
                case State.Illuminated:
                    GameScene.Effect(Halo = new TorchHalo(this));
                    break;
            }
        }

        public virtual void Remove(State state)
        {
            switch (state)
            {
                case State.Burning:
                    if (Burning != null)
                    {
                        Burning.On = false;
                        Burning = null;
                    }
                    break;
                case State.Levitating:
                    if (Levitation != null)
                    {
                        Levitation.On = false;
                        Levitation = null;
                    }
                    break;
                case State.Invisible:
                    Alpha(1f);
                    break;
                case State.Paralysed:
                    paused = false;
                    break;
                case State.Frozen:
                    if (IceBlock != null)
                    {
                        IceBlock.Melt();
                        IceBlock = null;
                    }
                    paused = false;
                    break;
                case State.Illuminated:
                    if (Halo != null)
                        Halo.PutOut();
                    break;
            }
        }

        public override void Update()
        {
            base.Update();

            if (paused && Listener != null)
                Listener.OnComplete(curAnim);

            if (_flashTime > 0 && (_flashTime -= Game.Elapsed) <= 0)
                ResetColor();

            if (Burning != null)
                Burning.Visible = Visible;

            if (Levitation != null)
                Levitation.Visible = Visible;

            if (IceBlock != null)
                IceBlock.Visible = Visible;

            if (Sleeping)
                ShowSleep();
            else
                HideSleep();

            if (Emo != null)
                Emo.Visible = Visible;
        }

        public virtual void ShowSleep()
        {
            var sleep = Emo as EmoIcon.Sleep;
            if (sleep == null)
            {
                if (Emo != null)
                    Emo.KillAndErase();
                Emo = new EmoIcon.Sleep(this);
            }
        }

        public virtual void HideSleep()
        {
            var sleep = Emo as EmoIcon.Sleep;
            
            if (sleep == null) 
                return;

            sleep.KillAndErase();
            Emo = null;
        }

        public virtual void ShowAlert()
        {
            var alert = Emo as EmoIcon.Alert;
            if (alert == null)
            {
                if (Emo != null)
                    Emo.KillAndErase();
                Emo = new EmoIcon.Alert(this);
            }
        }

        public virtual void HideAlert()
        {
            if (!(Emo is EmoIcon.Alert)) 
                return;
            Emo.KillAndErase();
            Emo = null;
        }

        public override void Kill()
        {
            base.Kill();

            if (Emo == null) 
                return;
            Emo.KillAndErase();
            Emo = null;
        }

        public virtual void OnComplete(Tweener tweener)
        {
            if (tweener != Motion) 
                return;
            IsMoving = false;

            Motion.KillAndErase();
            Motion = null;
        }

        public virtual void OnComplete(Animation anim)
        {
            if (AnimCallback != null)
            {
                AnimCallback.Call();
                AnimCallback = null;
            }
            else
            {
                if (anim == AttackAnimation)
                {
                    Idle();
                    Ch.OnAttackComplete();
                }
                else if (anim == OperateAnimation)
                {
                    Idle();
                    Ch.OnOperateComplete();
                }
            }
        }
    }
}