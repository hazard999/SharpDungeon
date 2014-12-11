using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class SheepSprite : MobSprite
    {
        public SheepSprite()
        {
            Texture(Assets.SHEEP);

            var frames = new TextureFilm(texture, 16, 15);

            IdleAnimation = new Animation(8, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 0);

            RunAnimation = IdleAnimation.Clone();
            AttackAnimation = IdleAnimation.Clone();

            DieAnimation = new Animation(20, false);
            DieAnimation.Frames(frames, 0);

            Play(IdleAnimation);
            curFrame = pdsharp.utils.Random.Int(curAnim.frames.Length);
        }
    }
}