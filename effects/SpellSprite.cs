using System.Collections.Generic;
using pdsharp.noosa;
using sharpdungeon.actors;
using sharpdungeon.scenes;

namespace sharpdungeon.effects
{
    public class SpellSprite : Image
    {
        public const int Food = 0;
        public const int Map = 1;
        public const int Charge = 2;
        public const int Mastery = 3;

        private const int Size = 16;

        private enum Phase
        {
            FadeIn,
            Static,
            FadeOut
        }

        private const float FadeInTime = 0.2f;
        private const float StaticTime = 0.8f;
        private const float FadeOutTime = 0.4f;

        private static TextureFilm _film;

        private Character _target;

        private Phase _phase;
        private float _duration;
        private float _passed;

        private static readonly Dictionary<Character, SpellSprite> All = new Dictionary<Character, SpellSprite>();

        public SpellSprite()
            : base(Assets.SPELL_ICONS)
        {
            if (_film == null)
                _film = new TextureFilm(texture, Size);
        }

        public virtual void Reset(int index)
        {
            Frame(_film.Get(index));
            Origin.Set(Width / 2, Height / 2);

            _phase = Phase.FadeIn;

            _duration = FadeInTime;
            _passed = 0;
        }

        public override void Update()
        {
            base.Update();

            X = _target.Sprite.Center().X - Size / 2;
            Y = _target.Sprite.Y - Size;

            switch (_phase)
            {
                case Phase.FadeIn:
                    Alpha(_passed / _duration);
                    Scale.Set(_passed / _duration);
                    break;
                case Phase.Static:
                    break;
                case Phase.FadeOut:
                    Alpha(1 - _passed / _duration);
                    break;
            }

            if (!((_passed += Game.Elapsed) > _duration)) 
                return;

            switch (_phase)
            {
                case Phase.FadeIn:
                    _phase = Phase.Static;
                    _duration = StaticTime;
                    break;
                case Phase.Static:
                    _phase = Phase.FadeOut;
                    _duration = FadeOutTime;
                    break;
                case Phase.FadeOut:
                    Kill();
                    break;
            }

            _passed = 0;
        }

        public override void Kill()
        {
            base.Kill();
            All.Remove(_target);
        }

        public static void Show(Character ch, int index)
        {
            if (!ch.Sprite.Visible)
                return;

            var old = All[ch];
            if (old != null)
                old.Kill();

            var sprite = GameScene.SpellSprite();
            sprite.Revive();
            sprite.Reset(index);
            sprite._target = ch;
            All.Add(ch, sprite);
        }
    }
}