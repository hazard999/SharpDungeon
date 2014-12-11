using System;
using System.Linq;
using Java.Util.Regex;
using pdsharp.glwrap;
using PointF = pdsharp.utils.PointF;

namespace pdsharp.noosa
{
    public class BitmapTextMultiline : BitmapText
    {
        public int MaxWidth = int.MaxValue;
        public static int StaticMaxWidth = int.MaxValue;

        public static readonly Pattern Paragraph = Pattern.Compile("\n");
        public static readonly Pattern Word = Pattern.Compile("\\s+");

        protected internal float SpaceSize;

        public bool[] Mask;

        public BitmapTextMultiline(Font font)
            : this("", font)
        {
        }

        public BitmapTextMultiline(string text, Font font)
            : base(text, font)
        {
            SpaceSize = font.Width(((TextureFilm) font).Get(' '));
        }

        protected internal override void UpdateVertices()
        {
            if (text == null)
                text = "";

            Quads = Quad.CreateSet(text.Length);
            RealLength = 0;

            // This object controls lines breaking
            var writer = new SymbolWriter(font, MaxWidth, Scale);

            // Word Size
            var metrics = new PointF();

            var paragraphs = Paragraph.Split(text);

            // Current character (used in masking)
            var pos = 0;

            foreach (var words in paragraphs.Select(paragraph => Word.Split(paragraph)))
            {
                foreach (var word in words.Where(word => word.Length != 0))
                {
                    GetWordMetrics(word, metrics);
                    writer.AddSymbol(metrics.X, metrics.Y);

                    var length = word.Length;
                    var shift = 0f; // Position in pixels relative to the beginning of the word

                    for (var k = 0; k < length; k++)
                    {
                        var rect = ((TextureFilm) font).Get(word[k]);

                        var w = font.Width(rect);
                        var h = font.Height(rect);

                        if (Mask == null || Mask[pos])
                        {
                            Vertices[0] = writer.x + shift;
                            Vertices[1] = writer.y;

                            Vertices[2] = rect.Left;
                            Vertices[3] = rect.Top;

                            Vertices[4] = writer.x + shift + w;
                            Vertices[5] = writer.y;

                            Vertices[6] = rect.Right;
                            Vertices[7] = rect.Top;

                            Vertices[8] = writer.x + shift + w;
                            Vertices[9] = writer.y + h;

                            Vertices[10] = rect.Right;
                            Vertices[11] = rect.Bottom;

                            Vertices[12] = writer.x + shift;
                            Vertices[13] = writer.y + h;

                            Vertices[14] = rect.Left;
                            Vertices[15] = rect.Bottom;

                            Quads.Put(Vertices);
                            RealLength++;
                        }

                        shift += w + font.tracking;

                        pos++;
                    }

                    writer.AddSpace(SpaceSize);
                }

                writer.NewLine(0, font.LineHeight);
            }

            Dirty = false;
        }

        private void GetWordMetrics(string word, PointF metrics)
        {
            float w = 0;
            float h = 0;

            var length = word.Length;
            for (var i = 0; i < length; i++)
            {
                var rect = ((TextureFilm) font).Get(word[i]);
                w += font.Width(rect) + (w > 0 ? font.tracking : 0);
                h = Math.Max(h, font.Height(rect));
            }

            metrics.Set(w, h);
        }

        public override void Measure()
        {
            var writer = new SymbolWriter(font, MaxWidth, Scale);

            var metrics = new PointF();

            var paragraphs = Paragraph.Split(text);

            foreach (var words in paragraphs.Select(paragrapgh => Word.Split(paragrapgh)))
            {
                foreach (var word in words.Where(word => word.Length != 0))
                {
                    GetWordMetrics(word, metrics);
                    writer.AddSymbol(metrics.X, metrics.Y);
                    writer.AddSpace(SpaceSize);
                }

                writer.NewLine(0, font.LineHeight);
            }

            _Width = writer.Width;
            _Height = writer.Height;
        }
    }
}