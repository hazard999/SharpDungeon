using Android.Graphics;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using sharpdungeon.sprites;

namespace sharpdungeon.effects
{
    public class IceBlock : Gizmo
    {
        private float _phase;

        private readonly CharSprite _target;

        public IceBlock(CharSprite target)
        {
            _target = target;
            _phase = 0;
        }

        public override void Update()
        {
            base.Update();

            if ((_phase += Game.Elapsed * 2) < 1)
                _target.Tint(0.83f, 1.17f, 1.33f, _phase * 0.6f);
            else
                _target.Tint(0.83f, 1.17f, 1.33f, 0.6f);
        }

        public virtual void Melt()
        {
            _target.ResetColor();
            KillAndErase();

            if (!Visible)
                return;

            Splash.At(_target.Center(), Color.Argb(0xFF, 0xB2, 0xD6, 0xFF), 5);
            Sample.Instance.Play(Assets.SND_SHATTER);
        }

        public static IceBlock Freeze(CharSprite sprite)
        {
            var iceBlock = new IceBlock(sprite);
            sprite.Parent.Add(iceBlock);

            return iceBlock;
        }
    }
}