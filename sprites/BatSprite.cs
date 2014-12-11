using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class BatSprite : MobSprite
    {
        public BatSprite()
        {
            Texture(Assets.BAT);

            TextureFilm frames = new TextureFilm(texture, 15, 15);

            IdleAnimation = new Animation(8, true);
            IdleAnimation.Frames(frames, 0, 1);

            RunAnimation = new Animation(12, true);
            RunAnimation.Frames(frames, 0, 1);

            AttackAnimation = new Animation(12, false);
            AttackAnimation.Frames(frames, 2, 3, 0, 1);

            DieAnimation = new Animation(12, false);
            DieAnimation.Frames(frames, 4, 5, 6);

            Play(IdleAnimation);
        }
    }
}