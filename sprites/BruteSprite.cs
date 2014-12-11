using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class BruteSprite : MobSprite
    {
        public BruteSprite()
        {
            Texture(Assets.BRUTE);

            var frames = new TextureFilm(texture, 12, 16);

            IdleAnimation = new Animation(2, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 1, 0, 0, 1, 1);

            RunAnimation = new Animation(12, true);
            RunAnimation.Frames(frames, 4, 5, 6, 7);

            AttackAnimation = new Animation(12, false);
            AttackAnimation.Frames(frames, 2, 3, 0);

            DieAnimation = new Animation(12, false);
            DieAnimation.Frames(frames, 8, 9, 10);

            Play(IdleAnimation);
        }
    }
}