using Android.Graphics;
using pdsharp.noosa;
using sharpdungeon.effects;

namespace sharpdungeon.sprites
{
    public class UndeadSprite : MobSprite
    {
        public UndeadSprite()
        {
            Texture(Assets.UNDEAD);

            var frames = new TextureFilm(texture, 12, 16);

            IdleAnimation = new Animation(12, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3);

            RunAnimation = new Animation(15, true);
            RunAnimation.Frames(frames, 4, 5, 6, 7, 8, 9);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(frames, 14, 15, 16);

            DieAnimation = new Animation(12, false);
            DieAnimation.Frames(frames, 10, 11, 12, 13);

            Play(IdleAnimation);
        }

        public override void DoDie()
        {
            base.DoDie();

            if (Dungeon.Visible[Ch.pos])
                Emitter().Burst(Speck.Factory(Speck.BONE), 3);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0xcc, 0xcc, 0xcc);
        }
    }
}