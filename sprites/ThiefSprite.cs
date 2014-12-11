using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class ThiefSprite : MobSprite
    {
        public ThiefSprite()
        {
            Texture(Assets.THIEF);

            var film = new TextureFilm(texture, 12, 13);

            IdleAnimation = new Animation(1, true);
            IdleAnimation.Frames(film, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            RunAnimation = new Animation(15, true);
            RunAnimation.Frames(film, 0, 0, 2, 3, 3, 4);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(film, 5, 6, 7, 8, 9);

            AttackAnimation = new Animation(12, false);
            AttackAnimation.Frames(film, 10, 11, 12, 0);

            Idle();
        }
    }
}