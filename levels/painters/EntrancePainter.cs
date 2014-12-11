namespace sharpdungeon.levels.painters
{
	public class EntrancePainter : Painter
	{
		public override void Paint(Level level, Room room)
		{
			Fill(level, room, Terrain.WALL);
			Fill(level, room, 1, Terrain.EMPTY);

		    foreach (var door in room.Connected.Values)
		        door.Set(Room.Door.DoorType.REGULAR);

		    level.entrance = room.Random(1);
			Set(level, level.entrance, Terrain.ENTRANCE);
		}
	}
}