using Java.Util.Regex;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using pdsharp.utils;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.ui
{
    public class GameLog : Component, IListener<string>
    {
        private const int MaxMessages = 3;

        private static readonly Pattern Punctuation = Pattern.Compile(".*[.,;?! ]$");

        private BitmapTextMultiline _lastEntry;
        private int _lastColor;

        public GameLog()
        {
            GLog.update.Add(this);

            NewLine();
        }

        public virtual void NewLine()
        {
            _lastEntry = null;
        }

        public void OnSignal(string text)
        {
            var color = CharSprite.Default;
            if (text.StartsWith(GLog.POSITIVE))
            {
                text = text.Substring(GLog.POSITIVE.Length);
                color = CharSprite.Positive;
            }
            else
                if (text.StartsWith(GLog.NEGATIVE))
                {
                    text = text.Substring(GLog.NEGATIVE.Length);
                    color = CharSprite.Negative;
                }
                else
                    if (text.StartsWith(GLog.WARNING))
                    {
                        text = text.Substring(GLog.WARNING.Length);
                        color = CharSprite.Warning;
                    }
                    else
                        if (text.StartsWith(GLog.HIGHLIGHT))
                        {
                            text = text.Substring(GLog.HIGHLIGHT.Length);
                            color = CharSprite.Neutral;
                        }

            text = Utils.Capitalize(text) + (Punctuation.Matcher(text).Matches() ? "" : ".");

            if (_lastEntry != null && color == _lastColor)
            {
                var lastMessage = _lastEntry.Text();
                _lastEntry.Text(lastMessage.Length == 0 ? text : lastMessage + " " + text);
                _lastEntry.Measure();

            }
            else
            {
                _lastEntry = PixelScene.CreateMultiline(text, 6);
                _lastEntry.MaxWidth = (int)Width;
                _lastEntry.Measure();
                _lastEntry.Hardlight(color);
                _lastColor = color;
                Add(_lastEntry);
            }

            if (Members.Count > MaxMessages)
                Remove(Members[0]);

            Layout();
        }

        protected override void Layout()
        {
            var pos = Y;
            for (var i = Members.Count - 1; i >= 0; i--)
            {
                var entry = (BitmapTextMultiline)Members[i];
                entry.X = X;
                entry.Y = pos - entry.Height;
                pos -= entry.Height;
            }
        }

        public override void Destroy()
        {
            GLog.update.Remove(this);
            base.Destroy();
        }
    }
}