using pdsharp.utils;

namespace pdsharp.noosa.tweeners
{
	public class PosTweener : Tweener
	{
		public Visual Visual;

		public PointF Start;
		public PointF End;

		public PosTweener(Visual visual, PointF pos, float time) : base(visual, time)
		{

			Visual = visual;
			Start = visual.Point();
			End = pos;
		}

		protected override void UpdateValues(float progress)
		{
			Visual.Point(PointF.Inter(Start, End, progress));
		}
	}
}