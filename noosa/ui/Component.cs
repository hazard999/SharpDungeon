namespace pdsharp.noosa.ui
{
    public class Component : Group
    {

        protected internal float X;
        protected internal float Y;
        protected float _Width;
        protected float _Height;

        public Component()
        {
            CreateChildren();
        }
    
        public virtual Component SetPos(float x, float y)
        {
            X = x;
            Y = y;
            Layout();

            return this;
        }

        public virtual Component SetSize(float Width, float Height)
        {
            _Width = Width;
            _Height = Height;
            Layout();

            return this;
        }

        public virtual Component SetRect(float x, float y, float Width, float Height)
        {
            X = x;
            Y = y;
            _Width = Width;
            _Height = Height;
            Layout();

            return this;
        }

        public virtual bool Inside(float x, float y)
        {
            return x >= X && y >= Y && x < X + _Width && y < Y + _Height;
        }

        public virtual void Fill(Component c)
        {
            SetRect(c.X, c.Y, c._Width, c._Height);
        }

        public virtual float Left()
        {
            return X;
        }

        public virtual float Right()
        {
            return X + _Width;
        }

        public virtual float CenterX()
        {
            return X + _Width / 2;
        }

        public virtual float Top()
        {
            return Y;
        }

        public virtual float Bottom()
        {
            return Y + _Height;
        }

        public virtual float CenterY()
        {
            return Y + _Height / 2;
        }

        public virtual float Width
        {
            get { return _Width; }
        }

        public virtual float Height
        {
            get { return _Height; }
        }

        protected virtual void CreateChildren()
        {
        }

        protected virtual void Layout()
        {
        }
    }

}