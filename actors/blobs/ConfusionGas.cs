using sharpdungeon.actors.buffs;
using sharpdungeon.effects;

namespace sharpdungeon.actors.blobs
{
	public class ConfusionGas : Blob
	{
		protected internal override void Evolve()
		{
			base.Evolve();

		    for (var i=0; i < Length; i++)
			{
			    Character ch;
			    if (Cur[i] > 0 && (ch = FindChar(i)) != null)
			        Buff.Prolong<Vertigo>(ch, Vertigo.Duration(ch));
			}
		}

		public override void Use(BlobEmitter emitter)
		{
			base.Use(emitter);

			emitter.Pour(Speck.Factory(Speck.CONFUSION, true), 0.6f);
		}

		public override string TileDesc()
		{
			return "A cloud of confusion gas is swirling here.";
		}
	}

}