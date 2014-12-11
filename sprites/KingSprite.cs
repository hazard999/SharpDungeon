using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class KingSprite : MobSprite
    {
        public KingSprite()
        {
            Texture(Assets.KING);

            var frames = new TextureFilm(texture, 16, 16);

            IdleAnimation = new Animation(12, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2);

            RunAnimation = new Animation(15, true);
            RunAnimation.Frames(frames, 3, 4, 5, 6, 7, 8);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(frames, 9, 10, 11);

            DieAnimation = new Animation(8, false);
            DieAnimation.Frames(frames, 12, 13, 14, 15);

            Play(IdleAnimation);
        }
    }
}