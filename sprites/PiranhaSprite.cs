using pdsharp.noosa;
using sharpdungeon.scenes;

namespace sharpdungeon.sprites
{
    public class PiranhaSprite : MobSprite
    {
        public PiranhaSprite()
        {
            Texture(Assets.PIRANHA);

            var frames = new TextureFilm(texture, 12, 16);

            IdleAnimation = new Animation(8, true);
            IdleAnimation.Frames(frames, 0, 1, 2, 1);

            RunAnimation = new Animation(20, true);
            RunAnimation.Frames(frames, 0, 1, 2, 1);

            AttackAnimation = new Animation(20, false);
            AttackAnimation.Frames(frames, 3, 4, 5, 6, 7, 8, 9, 10, 11);

            DieAnimation = new Animation(4, false);
            DieAnimation.Frames(frames, 12, 13, 14);

            Play(IdleAnimation);
        }

        public override void OnComplete(Animation anim)
        {
            base.OnComplete(anim);

            if (anim == AttackAnimation)
                GameScene.Ripple(Ch.pos);
        }
    }
}