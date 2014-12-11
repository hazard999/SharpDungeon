namespace pdsharp.noosa.tweeners
{
	public class AlphaTweener : Tweener
	{
		public Visual Image;

		public float Start;
		public float Delta;

		public AlphaTweener(Visual image, float alpha, float time) : base(image, time)
		{
			Image = image;
			Start = image.Alpha();
			Delta = alpha - Start;
		}

		protected override void UpdateValues(float progress)
		{
			Image.Alpha(Start + Delta * progress);
		}
	}
}