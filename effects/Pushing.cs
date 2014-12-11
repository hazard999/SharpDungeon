using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.sprites;

namespace sharpdungeon.effects
{
    public class Pushing : Actor
    {
        private readonly CharSprite _sprite;
        private readonly int _from;
        private readonly int _to;

        private Effect _effect;

        public Pushing(Character ch, int from, int to)
        {
            _sprite = ch.Sprite;
            _from = from;
            _to = to;
        }

        protected override bool Act()
        {
            if (_sprite != null)
            {
                if (_effect == null)
                    _effect = new Effect(_sprite, _from, _to, this);

                return false;
            }

            Remove(this);
            return true;
        }

        public class Effect : Visual
        {
            private readonly CharSprite _sprite;
            private readonly Pushing _pushing;
            private const float Delay = 0.15f;

            private readonly PointF _end;

            private float _delay;

            public Effect(CharSprite sprite, int fromPoint, int to, Pushing pushing)
                : base(0, 0, 0, 0)
            {
                _sprite = sprite;
                _pushing = pushing;
                Point(sprite.WorldToCamera(fromPoint));
                _end = sprite.WorldToCamera(to);

                Speed.Set(2 * (_end.X - X) / Delay, 2 * (_end.Y - Y) / Delay);
                Acc.Set(-Speed.X / Delay, -Speed.Y / Delay);

                _delay = 0;

                sprite.Parent.Add(this);
            }

            public override void Update()
            {
                base.Update();

                if ((_delay += Game.Elapsed) < Delay)
                {
                    _sprite.X = X;
                    _sprite.Y = Y;

                }
                else
                {
                    _sprite.Point(_end);

                    KillAndErase();
                    Actor.Remove(_pushing);

                    _pushing.Next();
                }
            }
        }
    }
}