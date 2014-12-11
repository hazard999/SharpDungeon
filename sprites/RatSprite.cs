using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class RatSprite : MobSprite
    {
        public RatSprite()
        {
            Texture(Assets.RAT);

            var frames = new TextureFilm(texture, 16, 15);

            IdleAnimation = new Animation(2, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 1);

            RunAnimation = new Animation(10, true);
            RunAnimation.Frames(frames, 6, 7, 8, 9, 10);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(frames, 2, 3, 4, 5, 0);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(frames, 11, 12, 13, 14);

            Play(IdleAnimation);
        }
    }
}