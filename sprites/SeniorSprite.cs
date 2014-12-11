using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class SeniorSprite : MobSprite
    {
        private readonly Animation _kick;

        public SeniorSprite()
        {
            Texture(Assets.MONK);

            var frames = new TextureFilm(texture, 15, 14);

            IdleAnimation = new Animation(6, true);
            IdleAnimation.Frames(frames, 18, 17, 18, 19);

            RunAnimation = new Animation(15, true);
            RunAnimation.Frames(frames, 28, 29, 30, 31, 32, 33);

            AttackAnimation = new Animation(12, false);
            AttackAnimation.Frames(frames, 20, 21, 20, 21);

            _kick = new Animation(10, false);
            _kick.Frames(frames, 22, 23, 22);

            DieAnimation = new Animation(15, false);
            DieAnimation.Frames(frames, 18, 24, 25, 25, 26, 27);

            Play(IdleAnimation);
        }

        public override void Attack(int cell)
        {
            base.Attack(cell);
            
            if (pdsharp.utils.Random.Float() < 0.3f)
                Play(_kick);
        }

        public override void OnComplete(Animation anim)
        {
            base.OnComplete(anim == _kick ? AttackAnimation : anim);
        }
    }
}