using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;

namespace sharpdungeon.effects
{
    public class BadgeBanner : Image
    {
        private enum State
        {
            FadeIn,
            Static,
            FadeOut
        }

        private State _state;

        private const float DefaultScale = 3;

        private const float FadeInTime = 0.2f;
        private const float StaticTime = 1f;
        private const float FadeOutTime = 1.0f;

        private readonly int _index;
        private float _time;

        private static TextureFilm _atlas;

        private static BadgeBanner _current;

        private BadgeBanner(int index)
            : base(Assets.BADGES)
        {
            if (_atlas == null)
                _atlas = new TextureFilm(texture, 16, 16);

            _index = index;

            Frame(_atlas.Get(index));
            Origin.Set(Width / 2, Height / 2);

            Alpha(0);
            Scale.Set(2 * DefaultScale);

            _state = State.FadeIn;
            _time = FadeInTime;

            Sample.Instance.Play(Assets.SND_BADGE);
        }

        public override void Update()
        {
            base.Update();

            _time -= Game.Elapsed;
            if (_time >= 0)
            {
                switch (_state)
                {
                    case State.FadeIn:
                        var p = _time / FadeInTime;
                        Scale.Set((1 + p) * DefaultScale);
                        Alpha(1 - p);
                        break;
                    case State.Static:
                        break;
                    case State.FadeOut:
                        Alpha(_time / FadeOutTime);
                        break;
                }

            }
            else
            {
                switch (_state)
                {
                    case State.FadeIn:
                        _time = StaticTime;
                        _state = State.Static;
                        Scale.Set(DefaultScale);
                        Alpha(1);
                        Highlight(this, _index);
                        break;
                    case State.Static:
                        _time = FadeOutTime;
                        _state = State.FadeOut;
                        break;
                    case State.FadeOut:
                        KillAndErase();
                        break;
                }

            }
        }

        public override void Kill()
        {
            if (_current == this)
                _current = null;

            base.Kill();
        }

        public static void Highlight(Image image, int index)
        {
            var p = new PointF();

            switch (index)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    p.Offset(7, 3);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                    p.Offset(6, 5);
                    break;
                case 8:
                case 9:
                case 10:
                case 11:
                    p.Offset(6, 3);
                    break;
                case 12:
                case 13:
                case 14:
                case 15:
                    p.Offset(7, 4);
                    break;
                case 16:
                    p.Offset(6, 3);
                    break;
                case 17:
                    p.Offset(5, 4);
                    break;
                case 18:
                    p.Offset(7, 3);
                    break;
                case 20:
                    p.Offset(7, 3);
                    break;
                case 21:
                    p.Offset(7, 3);
                    break;
                case 22:
                    p.Offset(6, 4);
                    break;
                case 23:
                    p.Offset(4, 5);
                    break;
                case 24:
                    p.Offset(6, 4);
                    break;
                case 25:
                    p.Offset(6, 5);
                    break;
                case 26:
                    p.Offset(5, 5);
                    break;
                case 27:
                    p.Offset(6, 4);
                    break;
                case 28:
                    p.Offset(3, 5);
                    break;
                case 29:
                    p.Offset(5, 4);
                    break;
                case 30:
                    p.Offset(5, 4);
                    break;
                case 31:
                    p.Offset(5, 5);
                    break;
                case 32:
                case 33:
                    p.Offset(7, 4);
                    break;
                case 34:
                    p.Offset(6, 4);
                    break;
                case 35:
                    p.Offset(6, 4);
                    break;
                case 36:
                    p.Offset(6, 5);
                    break;
                case 37:
                    p.Offset(4, 4);
                    break;
                case 38:
                    p.Offset(5, 5);
                    break;
                case 39:
                    p.Offset(5, 4);
                    break;
                case 40:
                case 41:
                case 42:
                case 43:
                    p.Offset(5, 4);
                    break;
                case 44:
                case 45:
                case 46:
                case 47:
                    p.Offset(5, 5);
                    break;
                case 48:
                case 49:
                case 50:
                case 51:
                    p.Offset(7, 4);
                    break;
                case 52:
                case 53:
                case 54:
                case 55:
                    p.Offset(4, 4);
                    break;
                case 56:
                    p.Offset(3, 7);
                    break;
                case 57:
                    p.Offset(4, 5);
                    break;
                case 58:
                    p.Offset(6, 4);
                    break;
                case 59:
                    p.Offset(7, 4);
                    break;
                case 60:
                case 61:
                case 62:
                case 63:
                    p.Offset(4, 4);
                    break;
            }

            p.X *= image.Scale.X;
            p.X *= image.Scale.Y;
            p.Offset(-image.Origin.X * (image.Scale.X - 1), -image.Origin.Y * (image.Scale.Y - 1));
            p.Offset(image.Point());

            var star = new Speck();
            star.Reset(0, p.X, p.Y, Speck.DISCOVER);
            star.Camera = image.Camera;
            image.Parent.Add(star);
        }

        public static BadgeBanner Show(int image)
        {
            if (_current != null)
                _current.KillAndErase();

            return (_current = new BadgeBanner(image));
        }

        public static Image Image(int index)
        {
            var image = new Image(Assets.BADGES);
            if (_atlas == null)
                _atlas = new TextureFilm(image.texture, 16, 16);

            image.Frame(_atlas.Get(index));
            return image;
        }
    }
}