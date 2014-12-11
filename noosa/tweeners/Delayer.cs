namespace pdsharp.noosa.tweeners
{
	public class Delayer : Tweener
	{
		public Delayer() : base(null, 0)
		{
		}

		public Delayer(float time) : base(null, time)
		{
		}

		protected override void UpdateValues(float progress)
		{
		}
	}
}