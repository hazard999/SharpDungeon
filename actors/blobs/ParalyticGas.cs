using sharpdungeon.actors.buffs;
using sharpdungeon.effects;

namespace sharpdungeon.actors.blobs
{
    public class ParalyticGas : Blob
	{
		protected internal override void Evolve()
		{
			base.Evolve();

		    for (var i=0; i < Length; i++)
			{
			    Character ch;
			    if (Cur[i] > 0 && (ch = FindChar(i)) != null)
				{
					Buff.Prolong<Paralysis>(ch, Paralysis.Duration(ch));
				}
			}
		}

		public override void Use(BlobEmitter emitter)
		{
			base.Use(emitter);

			emitter.Pour(Speck.Factory(Speck.PARALYSIS), 0.6f);
		}

		public override string TileDesc()
		{
			return "A cloud of paralytic gas is swirling here.";
		}
	}

}