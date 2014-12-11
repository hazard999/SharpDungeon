using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.particles;
using sharpdungeon.actors;
using sharpdungeon.effects;
using sharpdungeon.levels;

namespace sharpdungeon.sprites
{
    public class BlacksmithSprite : MobSprite
    {
        private Emitter _emitter;

        public BlacksmithSprite()
        {
            Texture(Assets.TROLL);

            var frames = new TextureFilm(texture, 13, 16);

            IdleAnimation = new Animation(15, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 0, 0, 0, 0, 1, 2, 2, 2, 3);

            RunAnimation = new Animation(20, true);
            RunAnimation.Frames(frames, 0);

            DieAnimation = new Animation(20, false);
            DieAnimation.Frames(frames, 0);

            Play(IdleAnimation);
        }

        public override void Link(Character ch)
        {
            base.Link(ch);

            _emitter = new Emitter();
            _emitter.AutoKill = false;
            _emitter.Pos(X + 7, Y + 12);
            Parent.Add(_emitter);
        }

        public override void Update()
        {
            base.Update();

            if (_emitter != null)
                _emitter.Visible = Visible;
        }

        public override void OnComplete(Animation anim)
        {
            base.OnComplete(anim);

            if (!Visible || _emitter == null || anim != IdleAnimation)
                return;

            _emitter.Burst(Speck.Factory(Speck.FORGE), 3);

            var volume = 0.2f / (Level.Distance(Ch.pos, Dungeon.Hero.pos));

            Sample.Instance.Play(Assets.SND_EVOKE, volume, volume, 0.8f);
        }
    }
}