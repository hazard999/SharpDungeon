using pdsharp.noosa.particles;
using sharpdungeon.actors;
using sharpdungeon.effects;

namespace sharpdungeon.sprites
{
    public class FetidRatSprite : RatSprite
    {
        private Emitter _cloud;

        public override void Link(Character ch)
        {
            base.Link(ch);

            if (_cloud != null) 
                return;

            _cloud = Emitter();
            _cloud.Pour(Speck.Factory(Speck.PARALYSIS), 0.7f);
        }

        public override void Update()
        {
            base.Update();

            if (_cloud != null)
                _cloud.Visible = Visible;
        }

        public override void DoDie()
        {
            base.DoDie();

            if (_cloud != null)
                _cloud.On = false;
        }
    }
}