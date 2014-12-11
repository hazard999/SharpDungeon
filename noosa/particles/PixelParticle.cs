namespace pdsharp.noosa.particles
{
	public class PixelParticle : PseudoPixel
	{
		protected internal float _Size;

		protected internal float Lifespan;
		protected internal float Left;

		public PixelParticle()
		{
			Origin.Set(+0.5f);
		}

		public virtual void reset(float x, float y, int color, float size, float lifespan)
		{
			Revive();

			X = x;
			Y = y;

			Color(color);
			Size(_Size = size);

			Left = Lifespan = lifespan;
		}

		public override void Update()
		{
			base.Update();

		    if ((Left -= Game.Elapsed) <= 0)
		        Kill();
		}

		public class Shrinking : PixelParticle
		{
			public override void Update()
			{
				base.Update();
				Size(_Size * Left / Lifespan);
			}
		}
	}
}