using pdsharp.utils;
using sharpdungeon.effects;

namespace sharpdungeon.actors.blobs
{
	public class Alchemy : Blob
	{
		protected internal int Pos;

		public override void RestoreFromBundle(Bundle bundle)
		{
			base.RestoreFromBundle(bundle);

		    for (var i = 0; i < Length; i++)
		        if (Cur[i] > 0)
		        {
		            Pos = i;
		            break;
		        }
		}

		protected internal override void Evolve()
		{
			Volume = Off[Pos] = Cur[Pos];

		    if (Dungeon.Visible[Pos])
		        Journal.Add(Journal.Feature.ALCHEMY);
		}

		public override void Seed(int cell, int amount)
		{
			Cur[Pos] = 0;
			Pos = cell;
			Volume = Cur[Pos] = amount;
		}

		public static void Transmute(int cell)
		{
			var heap = Dungeon.Level.heaps[cell];
		    
            if (heap == null) 
                return;

		    var result = heap.Transmute();
		    if (result != null)
		        Dungeon.Level.Drop(result, cell).Sprite.Drop(cell);
		}

		public override void Use(BlobEmitter emitter)
		{
			base.Use(emitter);
			emitter.Start(Speck.Factory(Speck.BUBBLE), 0.4f, 0);
		}
	}
}