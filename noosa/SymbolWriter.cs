using pdsharp.utils;

namespace pdsharp.noosa
{
    public class SymbolWriter
    {
        private readonly Font _font;
        private readonly int _maxWidht;
        private readonly PointF _scale;

        public SymbolWriter(Font font, int maxWidht, PointF scale)
        {
            _font = font;
            _maxWidht = maxWidht;
            _scale = scale;
        }

        public float Width = 0;
        public float Height = 0;

        public float lineWidth = 0;
        public float lineHeight = 0;

        public float x = 0;
        public float y = 0;

        public virtual void AddSymbol(float w, float h)
        {
            if (lineWidth > 0 && lineWidth + _font.tracking + w > _maxWidht/_scale.X)
                NewLine(w, h);
            else
            {
                x = lineWidth;

                lineWidth += (lineWidth > 0 ? _font.tracking : 0) + w;
                if (h > lineHeight)
                {
                    lineHeight = h;
                }
            }
        }

        public virtual void AddSpace(float w)
        {
            if (lineWidth > 0 && lineWidth + _font.tracking + w > _maxWidht/_scale.X)
                NewLine(0, 0);
            else
            {
                x = lineWidth;
                lineWidth += (lineWidth > 0 ? _font.tracking : 0) + w;
            }
        }

        public virtual void NewLine(float w, float h)
        {
            Height += lineHeight;
            if (Width < lineWidth)
                Width = lineWidth;

            lineWidth = w;
            lineHeight = h;

            x = 0;
            y = Height;
        }
    }
}