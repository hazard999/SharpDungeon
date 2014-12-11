using pdsharp.noosa;
using sharpdungeon.plants;

namespace sharpdungeon.sprites
{
    public class PlantSprite : Image
    {
        private const float Delay = 0.2f;

        private enum State
        {
            Growing,
            Normal,
            Withering
        }

        private State _state = State.Normal;
        private float _time;

        private static TextureFilm _frames;

        private int _pos = -1;

        public PlantSprite()
            : base(Assets.PLANTS)
        {
            if (_frames == null)
            {
                // Hardcoded size
                _frames = new TextureFilm(texture, 16, 16);
            }

            // Hardcoded origin
            Origin.Set(8, 12);
        }

        public PlantSprite(int image)
            : this()
        {
            Reset(image);
        }

        public virtual void Reset(Plant plant)
        {
            Revive();

            Reset(plant.Image);
            Alpha(1f);

            _pos = plant.Pos;
            var p = DungeonTilemap.TileToWorld(plant.Pos);
            X = p.X;
            Y = p.Y;

            _state = State.Growing;
            _time = Delay;
        }

        public virtual void Reset(int image)
        {
            Frame(_frames.Get(image));
        }

        public override void Update()
        {
            base.Update();

            Visible = _pos == -1 || Dungeon.Visible[_pos];

            switch (_state)
            {
                case State.Growing:
                    if ((_time -= Game.Elapsed) <= 0)
                    {
                        _state = State.Normal;
                        Scale.Set(1);
                    }
                    else
                        Scale.Set(1 - _time/Delay);

                    break;
                case State.Withering:
                    if ((_time -= Game.Elapsed) <= 0)
                        base.Kill();
                    else
                        Alpha(_time/Delay);
                    break;
            }
        }

        public override void Kill()
        {
            _state = State.Withering;
            _time = Delay;
        }
    }
}