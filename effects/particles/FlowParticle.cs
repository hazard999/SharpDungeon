using pdsharp.noosa;
using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects.particles
{
    public class FlowParticle : PixelParticle
    {
        public FlowParticle()
        {
            Lifespan = 0.6f;
            Acc.Set(0, 32);
            AngularSpeed = Random.Float(-360, +360);
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            Left = Lifespan;

            X = x;
            Y = y;

            Am = 0;
            Size(0);
            Speed.Set(0);
        }

        public override void Update()
        {
            base.Update();

            var p = Left / Lifespan;
            Am = (p < 0.5f ? p : 1 - p) * 0.6f;
            Size((1 - p) * 4);
        }

        public class Flow : Group
        {
            private const float Delay = 0.1f;

            private readonly int _pos;

            private readonly float _x;
            private readonly float _y;

            private float _delay;

            public Flow(int pos)
            {
                _pos = pos;

                var p = DungeonTilemap.TileToWorld(pos);
                _y = p.Y + DungeonTilemap.Size - 1;
                _x = p.X;

                _delay = pdsharp.utils.Random.Float(Delay);
            }

            public override void Update()
            {
                if (!(Visible = Dungeon.Visible[_pos])) 
                    return;

                base.Update();

                if (!((_delay -= Game.Elapsed) <= 0)) 
                    return;

                _delay = pdsharp.utils.Random.Float(Delay);

                Recycle<FlowParticle>().Reset(_x + pdsharp.utils.Random.Float(DungeonTilemap.Size), _y);
            }
        }
    }

    public class FlowParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<FlowParticle>().Reset(x, y);
        }
    }
}