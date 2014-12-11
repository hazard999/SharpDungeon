using pdsharp.noosa;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.effects;

namespace sharpdungeon.sprites
{
    public class ImpSprite : MobSprite
    {
        public ImpSprite()
        {
            Texture(Assets.IMP);

            var frames = new TextureFilm(texture, 12, 14);

            IdleAnimation = new Animation(10, true);
            IdleAnimation.Frames(frames, 0, 1, 2, 3, 0, 1, 2, 3, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 3, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4);

            RunAnimation = new Animation(20, true);
            RunAnimation.Frames(frames, 0);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(frames, 0, 3, 2, 1, 0, 3, 2, 1, 0);

            Play(IdleAnimation);
        }

        public override void Link(Character ch)
        {
            base.Link(ch);

            if (ch is Imp)
                Alpha(0.4f);
        }

        public override void OnComplete(Animation anim)
        {
            if (anim == DieAnimation)
            {
                Emitter().Burst(Speck.Factory(Speck.WOOL), 15);
                KillAndErase();
            }
            else
                base.OnComplete(anim);
        }
    }
}