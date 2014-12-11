using pdsharp.noosa;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;

namespace sharpdungeon.sprites
{
    public class ShamanSprite : MobSprite
    {
        private readonly int[] _points = new int[2];

        public ShamanSprite()
        {
            Texture(Assets.SHAMAN);

            var frames = new TextureFilm(texture, 12, 15);

            IdleAnimation = new Animation(2, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 1, 0, 0, 1, 1);

            RunAnimation = new Animation(12, true);
            RunAnimation.Frames(frames, 4, 5, 6, 7);

            AttackAnimation = new Animation(12, false);
            AttackAnimation.Frames(frames, 2, 3, 0);

            ZapAnimation = AttackAnimation.Clone();

            DieAnimation = new Animation(12, false);
            DieAnimation.Frames(frames, 8, 9, 10);

            Play(IdleAnimation);
        }

        public override void DoZap(int pos)
        {
            _points[0] = Ch.pos;
            _points[1] = pos;
            Parent.Add(new Lightning(_points, 2, (Shaman)Ch));

            TurnTo(Ch.pos, pos);
            Play(ZapAnimation);
        }
    }
}