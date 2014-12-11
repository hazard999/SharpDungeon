using pdsharp.noosa;
using pdsharp.noosa.ui;

namespace sharpdungeon.ui
{
	public class Archs : Component
	{
		private const float ScrollSpeed = 20f;

		private SkinnedBlock _arcsBg;
		private SkinnedBlock _arcsFg;

		private static float _offsB;
		private static float _offsF;

		public bool Reversed = false;

		protected override void CreateChildren()
		{
			_arcsBg = new SkinnedBlock(1, 1, Assets.ARCS_BG);
			_arcsBg.OffsetTo(0, _offsB);
			Add(_arcsBg);

			_arcsFg = new SkinnedBlock(1, 1, Assets.ARCS_FG);
			_arcsFg.OffsetTo(0, _offsF);
			Add(_arcsFg);
		}

		protected override void Layout()
		{
			_arcsBg.Size(Width, Height);
			_arcsBg.Offset(_arcsBg.texture.Width / 4 - (Width % _arcsBg.texture.Width) / 2, 0);

			_arcsFg.Size(Width, Height);
			_arcsFg.Offset(_arcsFg.texture.Width / 4 - (Width % _arcsFg.texture.Width) / 2, 0);
		}

		public override void Update()
		{
			base.Update();

			var shift = Game.Elapsed * ScrollSpeed;
		    if (Reversed)
		        shift = -shift;

		    _arcsBg.Offset(0, shift);
			_arcsFg.Offset(0, shift * 2);

			_offsB = _arcsBg.OffsetY();
			_offsF = _arcsFg.OffsetY();
		}
	}

}