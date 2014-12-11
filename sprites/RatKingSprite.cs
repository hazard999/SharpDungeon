using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class RatKingSprite : MobSprite
    {
        public RatKingSprite()
        {
            Texture(Assets.RATKING);

            var frames = new TextureFilm(texture, 16, 16);

            IdleAnimation = new Animation(2, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 1);

            RunAnimation = new Animation(10, true);
            RunAnimation.Frames(frames, 2, 3, 4, 5, 6);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(frames, 0);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(frames, 0);

            Play(IdleAnimation);
        }
    }
}