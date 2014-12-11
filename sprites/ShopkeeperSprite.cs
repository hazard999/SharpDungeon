using pdsharp.noosa;
using pdsharp.noosa.particles;

namespace sharpdungeon.sprites
{
    public class ShopkeeperSprite : MobSprite
    {
        private PixelParticle _coin;

        public ShopkeeperSprite()
        {
            Texture(Assets.KEEPER);
            var film = new TextureFilm(texture, 14, 14);

            IdleAnimation = new Animation(10, true);
            IdleAnimation.Frames(film, 1, 1, 1, 1, 1, 0, 0, 0, 0);

            RunAnimation = IdleAnimation.Clone();
            DieAnimation = IdleAnimation.Clone();
            AttackAnimation = IdleAnimation.Clone();

            Idle();
        }

        public override void OnComplete(Animation anim)
        {
            base.OnComplete(anim);

            if (!Visible || anim != IdleAnimation)
                return;

            if (_coin == null)
            {
                _coin = new PixelParticle();
                Parent.Add(_coin);
            }

            _coin.reset(X + (flipHorizontal ? 0 : 13), Y + 7, 0xFFFF00, 1, 0.5f);
            _coin.Speed.Y = -40;
            _coin.Acc.Y = +160;
        }
    }
}