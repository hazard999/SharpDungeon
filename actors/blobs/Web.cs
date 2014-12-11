using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;

namespace sharpdungeon.actors.blobs
{
	public class Web : Blob
	{
		protected internal override void Evolve()
		{
			for (var i=0; i < Length; i++)
			{
				var offv = Cur[i] > 0 ? Cur[i] - 1 : 0;
				Off[i] = offv;

			    if (offv <= 0) 
                    continue;

			    Volume += offv;

			    var ch = FindChar(i);
			    if (ch != null)
			        Buff.Prolong<Roots>(ch, Tick);
			}
		}

		public override void Use(BlobEmitter emitter)
		{
			base.Use(emitter);

			emitter.Pour(WebParticle.Factory, 0.4f);
		}

		public override void Seed(int cell, int amount)
		{
			var diff = amount - Cur[cell];
		    
            if (diff <= 0) 
                return;

		    Cur[cell] = amount;
		    Volume += diff;
		}

		public override string TileDesc()
		{
			return "Everything is covered with a thick web here.";
		}
	}
}