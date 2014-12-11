using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.items;
using sharpdungeon.items.potions;

namespace sharpdungeon.levels.painters
{
	public class PoolPainter : Painter
	{
		private const int Npiranhas = 3;

		public override void Paint(Level level, Room room)
		{
			Fill(level, room, Terrain.WALL);
			Fill(level, room, 1, Terrain.WATER);

			var door = room.Entrance();
			door.Set(Room.Door.DoorType.REGULAR);

			var x = -1;
			var y = -1;
			if (door.X == room.Left)
			{
				x = room.Right - 1;
				y = room.Top + room.Height() / 2;
			}
			else if (door.X == room.Right)
			{
				x = room.Left + 1;
				y = room.Top + room.Height() / 2;
			}
			else if (door.Y == room.Top)
			{
				x = room.Left + room.Width() / 2;
				y = room.Bottom - 1;
			}
			else if (door.Y == room.Bottom)
			{
				x = room.Left + room.Width() / 2;
				y = room.Top + 1;
			}

			var pos = x + y * Level.Width;
#if !DEBUG
			level.Drop(Prize(level), pos).HeapType = pdsharp.utils.Random.Int(3) == 0 ? Heap.Type.Chest : Heap.Type.Heap;
#endif
			Set(level, pos, Terrain.PEDESTAL);

			level.AddItemToSpawn(new PotionOfInvisibility());

			for (var i=0; i < Npiranhas; i++)
			{
				var piranha = new Piranha();
				do
				{
					piranha.pos = room.Random();
				} 
                while (level.map[piranha.pos] != Terrain.WATER|| Actor.FindChar(piranha.pos) != null);
				
                level.mobs.Add(piranha);
				
                Actor.OccupyCell(piranha);
			}
		}

		private static Item Prize(Level level)
		{
			var prize = level.ItemToSpanAsPrize();
		    
            if (prize != null)
		        return prize;

		    prize = Generator.Random(Random.OneOf(Generator.Category.WEAPON, Generator.Category.ARMOR));

			for (var i=0; i < 4; i++)
			{
				var another = Generator.Random(Random.OneOf(Generator.Category.WEAPON, Generator.Category.ARMOR));
			    if (another.level > prize.level)
			        prize = another;
			}

			return prize;
		}
	}
}