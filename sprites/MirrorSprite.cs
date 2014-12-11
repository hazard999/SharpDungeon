using pdsharp.noosa;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs.npcs;

namespace sharpdungeon.sprites
{
    public class MirrorSprite : MobSprite
    {
        private const int FrameWidth = 12;
        private const int FrameHeight = 15;

        public MirrorSprite()
        {
            Texture(Dungeon.Hero.heroClass.Spritesheet());
            UpdateArmor(0);
            Idle();
        }

        public override void Link(Character ch)
        {
            base.Link(ch);
            UpdateArmor(((MirrorImage)ch).tier);
        }

        public virtual void UpdateArmor(int tier)
        {
            var film = new TextureFilm(HeroSprite.Tiers(), tier, FrameWidth, FrameHeight);

            IdleAnimation = new Animation(1, true);
            IdleAnimation.Frames(film, 0, 0, 0, 1, 0, 0, 1, 1);

            RunAnimation = new Animation(20, true);
            RunAnimation.Frames(film, 2, 3, 4, 5, 6, 7);

            DieAnimation = new Animation(20, false);
            DieAnimation.Frames(film, 0);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(film, 13, 14, 15, 0);

            Idle();
        }
    }
}