using pdsharp.noosa;
using pdsharp.noosa.ui;

namespace sharpdungeon.ui
{
    public class Tag : Button
    {
        private readonly float _r;
        private readonly float _g;
        private readonly float _b;
        protected internal NinePatch Bg;

        protected internal float Lightness = 0;

        public Tag(int color)
        {
            _r = (color >> 16) / 255f;
            _g = ((color >> 8) & 0xFF) / 255f;
            _b = (color & 0xFF) / 255f;
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            Bg = Chrome.Get(Chrome.Type.TAG);
            Add(Bg);
        }

        protected override void Layout()
        {
            base.Layout();

            Bg.X = X;
            Bg.Y = Y;
            Bg.Size(Width, Height);
        }

        public virtual void Flash()
        {
            Lightness = 1f;
        }

        public override void Update()
        {
            base.Update();

            if (!Visible || !(Lightness > 0.5))
                return;

            if ((Lightness -= Game.Elapsed) > 0.5)
            {
                Bg.RA = Bg.Ga = Bg.Ba = 2 * Lightness - 1;
                Bg.Rm = 2 * _r * (1 - Lightness);
                Bg.Gm = 2 * _g * (1 - Lightness);
                Bg.Bm = 2 * _b * (1 - Lightness);
            }
            else
                Bg.Hardlight(_r, _g, _b);
        }
    }
}