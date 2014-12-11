using pdsharp.gltextures;
using pdsharp.noosa;
using pdsharp.noosa.tweeners;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.levels;
using sharpdungeon.scenes;

namespace sharpdungeon.sprites
{
    public class HeroSprite : CharSprite
    {
        private const int FrameWidth = 12;
        private const int FrameHeight = 15;

        private const int RunFramerate = 20;

        private static TextureFilm _tiers;

        private Animation _fly;

        private Tweener _jumpTweener;
        private ICallback _jumpCallback;

        public HeroSprite()
        {
            Link(Dungeon.Hero);

            Texture(Dungeon.Hero.heroClass.Spritesheet());
            UpdateArmor();

            Idle();
        }

        public virtual void UpdateArmor()
        {
            var film = new TextureFilm(Tiers(), ((Hero)Ch).Tier(), FrameWidth, FrameHeight);

            IdleAnimation = new Animation(1, true);
            IdleAnimation.Frames(film, 0, 0, 0, 1, 0, 0, 1, 1);

            RunAnimation = new Animation(RunFramerate, true);
            RunAnimation.Frames(film, 2, 3, 4, 5, 6, 7);

            DieAnimation = new Animation(20, false);
            DieAnimation.Frames(film, 8, 9, 10, 11, 12, 11);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(film, 13, 14, 15, 0);

            ZapAnimation = AttackAnimation.Clone();

            OperateAnimation = new Animation(8, false);
            OperateAnimation.Frames(film, 16, 17, 16, 17);

            _fly = new Animation(1, true);
            _fly.Frames(film, 18);
        }

        public override void Place(int p)
        {
            base.Place(p);
            Camera.Main.Target = this;
        }

        public override void Move(int from, int to)
        {
            base.Move(from, to);
            
            if (Ch.Flying)
                Play(_fly);
            
            Camera.Main.Target = this;
        }

        public virtual void Jump(int from, int to, ICallback callback)
        {
            _jumpCallback = callback;

            var distance = Level.Distance(from, to);
            _jumpTweener = new JumpTweener(this, WorldToCamera(to), distance * 4, distance * 0.1f);
            _jumpTweener.Listener = this;
            Parent.Add(_jumpTweener);

            TurnTo(from, to);
            Play(_fly);
        }

        public override void OnComplete(Tweener tweener)
        {
            if (tweener == _jumpTweener)
            {
                if (Visible && Level.water[Ch.pos] && !Ch.Flying)
                    GameScene.Ripple(Ch.pos);

                if (_jumpCallback != null)
                    _jumpCallback.Call();
            }
            else
                base.OnComplete(tweener);
        }

        public override void Update()
        {
            Sleeping = ((Hero)Ch).RestoreHealth;

            base.Update();
        }

        public virtual bool Sprint(bool on)
        {
            RunAnimation.delay = on ? 0.625f / RunFramerate : 1f / RunFramerate;
            return on;
        }

        public static TextureFilm Tiers()
        {
            if (_tiers != null)
                return _tiers;

            // Sprites for All classes are the same in size
            var texture = TextureCache.Get(Assets.ROGUE);
            _tiers = new TextureFilm(texture, texture.Width, FrameHeight);

            return _tiers;
        }

        public static Image Avatar(HeroClass cl, int armorTier)
        {
            var patch = Tiers().Get(armorTier);
            var avatar = new Image(cl.Spritesheet());
            var frame = avatar.texture.UvRect(1, 0, FrameWidth, FrameHeight);
            frame.Offset(patch.Left, patch.Top);
            avatar.Frame(frame);

            return avatar;
        }

        private class JumpTweener : Tweener
        {
            private readonly Visual _visual;

            private readonly PointF _start;
            private readonly PointF _end;

            private readonly float _height;

            public JumpTweener(Visual visual, PointF pos, float height, float time)
                : base(visual, time)
            {
                _visual = visual;
                _start = visual.Point();
                _end = pos;

                _height = height;
            }

            protected override void UpdateValues(float progress)
            {
                _visual.Point(PointF.Inter(_start, _end, progress).Offset(0, -_height * 4 * progress * (1 - progress)));
            }
        }
    }
}