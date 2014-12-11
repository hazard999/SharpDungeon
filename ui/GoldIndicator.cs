using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.scenes;

namespace sharpdungeon.ui
{
    public class GoldIndicator : Component
    {
        private const float Time = 2f;

        private int _lastValue;

        private BitmapText _tf;

        private float _time;

        protected override void CreateChildren()
        {
            _tf = new BitmapText(PixelScene.font1x);
            _tf.Hardlight(0xFFFF00);
            Add(_tf);

            Visible = false;
        }

        protected override void Layout()
        {
            _tf.X = X + (Width - _tf.Width) / 2;
            _tf.Y = Bottom() - _tf.Height;
        }

        public override void Update()
        {
            base.Update();

            if (Visible)
            {
                _time -= Game.Elapsed;
                if (_time > 0)
                    _tf.Alpha(_time > Time / 2 ? 1f : _time * 2 / Time);
                else
                    Visible = false;
            }

            if (Dungeon.Gold == _lastValue)
                return;

            _lastValue = Dungeon.Gold;

            _tf.Text(_lastValue.ToString());
            _tf.Measure();

            Visible = true;
            _time = Time;

            Layout();
        }
    }
}