namespace sharpdungeon.levels
{
    public class Terrain
    {
        public const int CHASM = 0;
        public const int EMPTY = 1;
        public const int GRASS = 2;
        public const int EMPTY_WELL = 3;
        public const int WALL = 4;
        public const int DOOR = 5;
        public const int OPEN_DOOR = 6;
        public const int ENTRANCE = 7;
        public const int EXIT = 8;
        public const int EMBERS = 9;
        public const int LOCKED_DOOR = 10;
        public const int PEDESTAL = 11;
        public const int WALL_DECO = 12;
        public const int BARRICADE = 13;
        public const int EMPTY_SP = 14;
        public const int HIGH_GRASS = 15;
        public const int EMPTY_DECO = 24;
        public const int LOCKED_EXIT = 25;
        public const int UNLOCKED_EXIT = 26;
        public const int SIGN = 29;
        public const int WELL = 34;
        public const int STATUE = 35;
        public const int STATUE_SP = 36;
        public const int BOOKSHELF = 41;
        public const int ALCHEMY = 42;
        public const int CHASM_FLOOR = 43;
        public const int CHASM_FLOOR_SP = 44;
        public const int CHASM_WALL = 45;
        public const int CHASM_WATER = 46;

        public const int SECRET_DOOR = 16;
        public const int TOXIC_TRAP = 17;
        public const int SECRET_TOXIC_TRAP = 18;
        public const int FIRE_TRAP = 19;
        public const int SECRET_FIRE_TRAP = 20;
        public const int PARALYTIC_TRAP = 21;
        public const int SECRET_PARALYTIC_TRAP = 22;
        public const int INACTIVE_TRAP = 23;
        public const int POISON_TRAP = 27;
        public const int SECRET_POISON_TRAP = 28;
        public const int ALARM_TRAP = 30;
        public const int SECRET_ALARM_TRAP = 31;
        public const int LIGHTNING_TRAP = 32;
        public const int SECRET_LIGHTNING_TRAP = 33;
        public const int GRIPPING_TRAP = 37;
        public const int SECRET_GRIPPING_TRAP = 38;
        public const int SUMMONING_TRAP = 39;
        public const int SECRET_SUMMONING_TRAP = 40;

        public const int WATER_TILES = 48;
        public const int WATER = 63;

        public const int PASSABLE = 0x01;
        public const int LOS_BLOCKING = 0x02;
        public const int FLAMABLE = 0x04;
        public const int SECRET = 0x08;
        public const int SOLID = 0x10;
        public const int AVOID = 0x20;
        public const int LIQUID = 0x40;
        public const int PIT = 0x80;

        public const int UNSTITCHABLE = 0x100;

        public static readonly int[] Flags = new int[256];
        static Terrain()
        {
            Flags[CHASM] = AVOID | PIT | UNSTITCHABLE;
            Flags[EMPTY] = PASSABLE;
            Flags[GRASS] = PASSABLE | FLAMABLE;
            Flags[EMPTY_WELL] = PASSABLE;
            Flags[WATER] = PASSABLE | LIQUID | UNSTITCHABLE;
            Flags[WALL] = LOS_BLOCKING | SOLID | UNSTITCHABLE;
            Flags[DOOR] = PASSABLE | LOS_BLOCKING | FLAMABLE | SOLID | UNSTITCHABLE;
            Flags[OPEN_DOOR] = PASSABLE | FLAMABLE | UNSTITCHABLE;
            Flags[ENTRANCE] = PASSABLE; // | SOLID
            Flags[EXIT] = PASSABLE;
            Flags[EMBERS] = PASSABLE;
            Flags[LOCKED_DOOR] = LOS_BLOCKING | SOLID | UNSTITCHABLE;
            Flags[PEDESTAL] = PASSABLE | UNSTITCHABLE;
            Flags[WALL_DECO] = Flags[WALL];
            Flags[BARRICADE] = FLAMABLE | SOLID | LOS_BLOCKING;
            Flags[EMPTY_SP] = Flags[EMPTY] | UNSTITCHABLE;
            Flags[HIGH_GRASS] = PASSABLE | LOS_BLOCKING | FLAMABLE;
            Flags[EMPTY_DECO] = Flags[EMPTY];
            Flags[LOCKED_EXIT] = SOLID;
            Flags[UNLOCKED_EXIT] = PASSABLE;
            Flags[SIGN] = PASSABLE | FLAMABLE;
            Flags[WELL] = AVOID;
            Flags[STATUE] = SOLID;
            Flags[STATUE_SP] = Flags[STATUE] | UNSTITCHABLE;
            Flags[BOOKSHELF] = Flags[BARRICADE] | UNSTITCHABLE;
            Flags[ALCHEMY] = PASSABLE;

            Flags[CHASM_WALL] = Flags[CHASM];
            Flags[CHASM_FLOOR] = Flags[CHASM];
            Flags[CHASM_FLOOR_SP] = Flags[CHASM];
            Flags[CHASM_WATER] = Flags[CHASM];

            Flags[SECRET_DOOR] = Flags[WALL] | SECRET | UNSTITCHABLE;
            Flags[TOXIC_TRAP] = AVOID;
            Flags[SECRET_TOXIC_TRAP] = Flags[EMPTY] | SECRET;
            Flags[FIRE_TRAP] = AVOID;
            Flags[SECRET_FIRE_TRAP] = Flags[EMPTY] | SECRET;
            Flags[PARALYTIC_TRAP] = AVOID;
            Flags[SECRET_PARALYTIC_TRAP] = Flags[EMPTY] | SECRET;
            Flags[POISON_TRAP] = AVOID;
            Flags[SECRET_POISON_TRAP] = Flags[EMPTY] | SECRET;
            Flags[ALARM_TRAP] = AVOID;
            Flags[SECRET_ALARM_TRAP] = Flags[EMPTY] | SECRET;
            Flags[LIGHTNING_TRAP] = AVOID;
            Flags[SECRET_LIGHTNING_TRAP] = Flags[EMPTY] | SECRET;
            Flags[GRIPPING_TRAP] = AVOID;
            Flags[SECRET_GRIPPING_TRAP] = Flags[EMPTY] | SECRET;
            Flags[SUMMONING_TRAP] = AVOID;
            Flags[SECRET_SUMMONING_TRAP] = Flags[EMPTY] | SECRET;
            Flags[INACTIVE_TRAP] = Flags[EMPTY];

            for (int i = WATER_TILES; i < WATER_TILES + 16; i++)
            {
                Flags[i] = Flags[WATER];
            }
        }

        public static int discover(int terr)
        {
            switch (terr)
            {
                case SECRET_DOOR:
                    return DOOR;
                case SECRET_FIRE_TRAP:
                    return FIRE_TRAP;
                case SECRET_PARALYTIC_TRAP:
                    return PARALYTIC_TRAP;
                case SECRET_TOXIC_TRAP:
                    return TOXIC_TRAP;
                case SECRET_POISON_TRAP:
                    return POISON_TRAP;
                case SECRET_ALARM_TRAP:
                    return ALARM_TRAP;
                case SECRET_LIGHTNING_TRAP:
                    return LIGHTNING_TRAP;
                case SECRET_GRIPPING_TRAP:
                    return GRIPPING_TRAP;
                case SECRET_SUMMONING_TRAP:
                    return SUMMONING_TRAP;
                default:
                    return terr;
            }
        }
    }

}