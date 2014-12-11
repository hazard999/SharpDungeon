namespace pdsharp.utils
{
	public class Point
	{
	    protected bool Equals(Point other)
	    {
	        return X == other.X && Y == other.Y;
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            return (X*397) ^ Y;
	        }
	    }

	    public int X;
		public int Y;

		public Point()
		{
		}

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Point(Point p)
		{
			X = p.X;
			Y = p.Y;
		}

		public virtual Point Set(int x, int y)
		{
			X = x;
			Y = y;
			return this;
		}

		public virtual Point Set(Point p)
		{
			X = p.X;
			Y = p.Y;
			return this;
		}

		public virtual Point Clone()
		{
			return new Point(this);
		}

		public virtual Point Scale(float f)
		{
            X = (int)(X * f);
            Y = (int)(Y * f);
			return this;
		}

		public virtual Point Offset(int dx, int dy)
		{
			X += dx;
			Y += dy;
			return this;
		}

		public virtual Point Offset(Point d)
		{
			X += d.X;
			Y += d.Y;
			return this;
		}

		public override bool Equals(object obj)
		{
		    if (ReferenceEquals(null, obj)) 
                return false;

		    if (ReferenceEquals(this, obj)) 
                return true;

		    return obj.GetType() == GetType() && Equals((Point) obj);
		}
	}
}