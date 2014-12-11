using pdsharp.noosa;
using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects.particles
{
    public class WindParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new WindParticleFactory();

        private static float _angle = Random.Float(PointF.Pi * 2);
        private static PointF _speed = new PointF().Polar(_angle, 5);

        private readonly float _size;

        public WindParticle()
        {

            Lifespan = Random.Float(1, 2);
            Scale.Set(_size = Random.Float(3));
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            Left = Lifespan;

            Speed.Set(_speed);
            Speed.Scale(_size);

            X = x - Speed.X * Lifespan / 2;
            Y = y - Speed.Y * Lifespan / 2;

            _angle += Random.Float(-0.1f, +0.1f);
            _speed = new PointF().Polar(_angle, 5);

            Am = 0;
        }

        public override void Update()
        {
            base.Update();

            var p = Left / Lifespan;
            Am = (p < 0.5f ? p : 1 - p) * _size * 0.2f;
        }

        public class Wind : Group
        {
            private readonly int _pos;

            private readonly float _x;
            private readonly float _y;

            private float _delay;

            public Wind(int pos)
            {

                _pos = pos;
                var p = DungeonTilemap.TileToWorld(pos);
                _x = p.X;
                _y = p.Y;

                _delay = pdsharp.utils.Random.Float(5);
            }

            public override void Update()
            {
                if (!(Visible = Dungeon.Visible[_pos])) 
                    return;

                base.Update();

                if (!((_delay -= Game.Elapsed) <= 0)) 
                    return;

                _delay = pdsharp.utils.Random.Float(5);

                Recycle<WindParticle>().Reset(_x + pdsharp.utils.Random.Float(DungeonTilemap.Size), _y + pdsharp.utils.Random.Float(DungeonTilemap.Size));
            }
        }
    }

    public class WindParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<WindParticle>().Reset(x, y);
        }
    }
}