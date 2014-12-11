using Android.Graphics;
using pdsharp.noosa;
using sharpdungeon.actors;

namespace sharpdungeon.sprites
{
    public class ElementalSprite : MobSprite
    {
        public ElementalSprite()
        {
            Texture(Assets.ELEMENTAL);

            var frames = new TextureFilm(texture, 12, 14);

            IdleAnimation = new Animation(10, true);
            IdleAnimation.Frames(frames, 0, 1, 2);

            RunAnimation = new Animation(12, true);
            RunAnimation.Frames(frames, 0, 1, 3);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(frames, 4, 5, 6);

            DieAnimation = new Animation(15, false);
            DieAnimation.Frames(frames, 7, 8, 9, 10, 11, 12, 13, 12);

            Play(IdleAnimation);
        }

        public override void Link(Character ch)
        {
            base.Link(ch);
            Add(State.Burning);
        }

        public override void DoDie()
        {
            base.DoDie();
            Remove(State.Burning);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0xFF, 0x7D, 0x13);
        }
    }
}