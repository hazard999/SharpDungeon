using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class AlbinoSprite : MobSprite
    {
        public AlbinoSprite()
        {
            Texture(Assets.RAT);

            var frames = new TextureFilm(texture, 16, 15);

            IdleAnimation = new Animation(2, true);
            IdleAnimation.Frames(frames, 16, 16, 16, 17);

            RunAnimation = new Animation(10, true);
            RunAnimation.Frames(frames, 22, 23, 24, 25, 26);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(frames, 18, 19, 20, 21);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(frames, 27, 28, 29, 30);

            Play(IdleAnimation);
        }
    }
}