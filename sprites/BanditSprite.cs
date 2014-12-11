using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class BanditSprite : MobSprite
    {
        public BanditSprite()
        {
            Texture(Assets.THIEF);

            var film = new TextureFilm(texture, 12, 13);

            IdleAnimation = new Animation(1, true);
            IdleAnimation.Frames(film, 21, 21, 21, 22, 21, 21, 21, 21, 22);

            RunAnimation = new Animation(15, true);
            RunAnimation.Frames(film, 21, 21, 23, 24, 24, 25);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(film, 25, 27, 28, 29, 30);

            AttackAnimation = new Animation(12, false);
            AttackAnimation.Frames(film, 31, 32, 33);

            Idle();
        }
    }
}