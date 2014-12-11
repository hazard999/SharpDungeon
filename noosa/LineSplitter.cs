using System.Collections.Generic;
using System.Linq;
using PointF = pdsharp.utils.PointF;
using StringBuilder = System.Text.StringBuilder;
using System;

namespace pdsharp.noosa
{
    public class LineSplitter
    {
        private readonly Font _font;
        private readonly PointF _scale;
        private readonly string _text;

        public LineSplitter(Font font, PointF scale, string text)
        {
            _font = font;
            _scale = scale;
            _text = text;
            _spaceSize = font.Width(((TextureFilm) font).Get(' '));
        }

        private List<BitmapText> _lines;

        private StringBuilder _curLine;
        private float _curLineWidth;

        private readonly PointF _metrics = new PointF();
        private readonly float _spaceSize;

        private void NewLine(string str, float width)
        {
            var txt = new BitmapText(_curLine.ToString(), _font);
            txt.Scale.Set(_scale.X);
            _lines.Add(txt);

            _curLine = new StringBuilder(str);
            _curLineWidth = width;
        }

        private void Append(string str, float width)
        {
            _curLineWidth += (_curLineWidth > 0 ? _font.tracking : 0) + width;
            _curLine.Append(str);
        }

        private void GetWordMetrics(string word, PointF metrics)
        {
            float w = 0;
            float h = 0;

            var length = word.Length;
            for (var i = 0; i < length; i++)
            {
                var rect = ((TextureFilm) _font).Get(word[i]);
                w += _font.Width(rect) + (w > 0 ? _font.tracking : 0);
                h = Math.Max(h, _font.Height(rect));
            }

            metrics.Set(w, h);
        }

        public virtual List<BitmapText> Split()
        {
            _lines = new List<BitmapText>();

            _curLine = new StringBuilder();
            _curLineWidth = 0;

            var paragraphs = BitmapTextMultiline.Paragraph.Split(_text);

            foreach (var words in paragraphs.Select(paragraph => BitmapTextMultiline.Word.Split(paragraph)))
            {
                foreach (var word in words.Where(word => word.Length != 0))
                {
                    GetWordMetrics(word, _metrics);

                    if (_curLineWidth > 0 && _curLineWidth + _font.tracking + _metrics.X > BitmapTextMultiline.StaticMaxWidth / _scale.X)
                        NewLine(word, _metrics.X);
                    else
                        Append(word, _metrics.X);

                    if (_curLineWidth > 0 && _curLineWidth + _font.tracking + _spaceSize > BitmapTextMultiline.StaticMaxWidth / _scale.X)
                        NewLine("", 0);
                    else
                        Append(" ", _spaceSize);
                }

                NewLine("", 0);
            }

            return _lines;
        }
    }
}