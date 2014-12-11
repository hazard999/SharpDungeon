using pdsharp.gltextures;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.actors;

namespace sharpdungeon.ui
{
    public class HealthIndicator : Component
    {
        private const float HEIGHT = 2;

        public static HealthIndicator Instance;

        private Character _target;

        private Image _bg;
        private Image _level;

        public HealthIndicator()
        {
            Instance = this;
        }

        protected override void CreateChildren()
        {
            _bg = new Image(TextureCache.CreateSolid(Android.Graphics.Color.Argb(0xFF, 0xcc, 0x00, 0x00)));
            _bg.Scale.Y = HEIGHT;
            Add(_bg);

            _level = new Image(TextureCache.CreateSolid(Android.Graphics.Color.Argb(0xFF, 0x00, 0xcc, 0x00)));
            _level.Scale.Y = HEIGHT;
            Add(_level);
        }

        public override void Update()
        {
            base.Update();

            if (_target != null && _target.IsAlive && _target.Sprite.Visible)
            {
                var sprite = _target.Sprite;
                _bg.Scale.X = sprite.Width;
                _level.Scale.X = sprite.Width*_target.HP/_target.HT;
                _bg.X = _level.X = sprite.X;
                _bg.Y = _level.Y = sprite.Y - HEIGHT - 1;

                Visible = true;
            }
            else
                Visible = false;
        }

        public virtual void Target(Character ch)
        {
            if (ch != null && ch.IsAlive)
                _target = ch;
            else
                _target = null;
        }

        public virtual Character Target()
        {
            return _target;
        }
    }
}