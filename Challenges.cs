//
// * Pixel Dungeon
// * Copyright (C) 2012-2014  Oleg Dolya
// *
// * This program is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * This program is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with this program.  If not, see <http://www.gnu.org/licenses/>
// 
namespace sharpdungeon
{

	public class Challenges
	{

		public const int NO_FOOD = 1;
		public const int NO_ARMOR = 2;
		public const int NO_HEALING = 4;
		public const int NO_HERBALISM = 8;
		public const int SWARM_INTELLIGENCE = 16;
		public const int DARKNESS = 32;

		public static readonly string[] NAMES = { "No food", "No armors", "No healing potions", "No dew, no seeds", "Swarm intelligence", "Darkness" };

		public static readonly int[] MASKS = { NO_FOOD, NO_ARMOR, NO_HEALING, NO_HERBALISM, SWARM_INTELLIGENCE, DARKNESS };

	}

}