using Android.Graphics;
using pdsharp.noosa.particles;
using sharpdungeon.scenes;
using PointF = pdsharp.utils.PointF;

namespace sharpdungeon.effects
{
	public class Splash
	{
        public static void At(int cell, Color color, int n)
		{
			At(DungeonTilemap.TileCenterToWorld(cell), color, n);
		}

		public static void At(PointF p, Color color, int n)
		{
		    if (n <= 0)
		        return;

		    var emitter = GameScene.Emitter();
			emitter.Pos(p);

			Factory.Color = color;
			Factory.Dir = -3.1415926f / 2;
			Factory.Cone = 3.1415926f;
			emitter.Burst(Factory, n);
		}

		public static void At(PointF p, float dir, float cone, Color color, int n)
		{
		    if (n <= 0)
		        return;

		    var emitter = GameScene.Emitter();
			emitter.Pos(p);

			Factory.Color = color;
			Factory.Dir = dir;
			Factory.Cone = cone;
			emitter.Burst(Factory, n);
		}

		private static readonly SplashFactory Factory = new SplashFactory();

		private class SplashFactory : Emitter.Factory
		{
			public Color Color;
			public float Dir;
			public float Cone;

			public override void Emit(Emitter emitter, int index, float x, float y)
			{
				var p = (PixelParticle)emitter.Recycle(typeof(PixelParticle.Shrinking));

				p.reset(x, y, Color, 4, pdsharp.utils.Random.Float(0.5f, 1.0f));
				p.Speed.Polar(pdsharp.utils.Random.Float(Dir - Cone / 2, Dir + Cone / 2), pdsharp.utils.Random.Float(40, 80));
				p.Acc.Set(0, +100);
			}
		}
	}
}