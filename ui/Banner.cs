using pdsharp.noosa;

namespace sharpdungeon.ui
{
    public class Banner : Image
    {
        private enum State
        {
            FADE_IN,
            STATIC,
            FADE_OUT
        }
        private State _state;

        private float _time;

        private int _color;
        private float _fadeTime;
        private float _showTime;

        public Banner(Image sample)
        {
            Copy(sample);
            Alpha(0);
        }

        public Banner(object tx)
            : base(tx)
        {
            Alpha(0);
        }

        public virtual void Show(int color, float fadeTime, float showTime)
        {
            _color = color;
            _fadeTime = fadeTime;
            _showTime = showTime;

            _state = State.FADE_IN;

            _time = fadeTime;
        }

        public virtual void Show(int color, float fadeTime)
        {
            Show(color, fadeTime, float.MaxValue);
        }

        public override void Update()
        {
            base.Update();

            _time -= Game.Elapsed;
            if (_time >= 0)
            {
                var p = _time / _fadeTime;

                switch (_state)
                {
                    case State.FADE_IN:
                        Tint(_color, p);
                        Alpha(1 - p);
                        break;
                    case State.STATIC:
                        break;
                    case State.FADE_OUT:
                        Alpha(p);
                        break;
                }
            }
            else
            {
                switch (_state)
                {
                    case State.FADE_IN:
                        _time = _showTime;
                        _state = State.STATIC;
                        break;
                    case State.STATIC:
                        _time = _fadeTime;
                        _state = State.FADE_OUT;
                        break;
                    case State.FADE_OUT:
                        KillAndErase();
                        break;
                }
            }
        }
    }
}