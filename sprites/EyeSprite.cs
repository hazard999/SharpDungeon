using pdsharp.noosa;
using sharpdungeon.effects;

namespace sharpdungeon.sprites
{
    public class EyeSprite : MobSprite
    {
        private int _attackPos;

        public EyeSprite()
        {
            Texture(Assets.EYE);

            var frames = new TextureFilm(texture, 16, 18);

            IdleAnimation = new Animation(8, true);
            IdleAnimation.Frames(frames, 0, 1, 2);

            RunAnimation = new Animation(12, true);
            RunAnimation.Frames(frames, 5, 6);

            AttackAnimation = new Animation(8, false);
            AttackAnimation.Frames(frames, 4, 3);

            DieAnimation = new Animation(8, false);
            DieAnimation.Frames(frames, 7, 8, 9);

            Play(IdleAnimation);
        }

        public override void Attack(int pos)
        {
            _attackPos = pos;
            base.Attack(pos);
        }

        public override void OnComplete(Animation anim)
        {
            base.OnComplete(anim);

            if (anim != AttackAnimation)
                return;

            if (Dungeon.Visible[Ch.pos] || Dungeon.Visible[_attackPos])
                Parent.Add(new DeathRay(Center(), DungeonTilemap.TileCenterToWorld(_attackPos)));
        }
    }
}