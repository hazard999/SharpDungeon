using pdsharp.noosa;
using sharpdungeon.levels;

namespace sharpdungeon.effects
{
	public class Ripple : Image
	{
		private const float TimeToFade = 0.5f;

		private float _time;

		public Ripple() : base(Effects.Get(Effects.Type.Ripple))
		{
		}

		public virtual void Reset(int p)
		{
			Revive();

			X = (p % Level.Width) * DungeonTilemap.Size;
			Y = (p / Level.Width) * DungeonTilemap.Size;

			Origin.Set(Width / 2, Height / 2);
			Scale.Set(0);

			_time = TimeToFade;
		}

		public override void Update()
		{
			base.Update();

		    if ((_time -= Game.Elapsed) <= 0)
		        Kill();
		    else
		    {
		        var p = _time/TimeToFade;
		        Scale.Set(1 - p);
		        Alpha(p);
		    }
		}
	}
}