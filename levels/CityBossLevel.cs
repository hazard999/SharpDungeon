using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.items;
using sharpdungeon.items.keys;
using sharpdungeon.levels.painters;
using sharpdungeon.scenes;

namespace sharpdungeon.levels
{
    public class CityBossLevel : Level
    {
        public CityBossLevel()
        {
            color1 = 0x4b6636;
            color2 = 0xf2f2f2;
        }

        private const int TOP = 2;
        private const int HALL_WIDTH = 7;
        private const int HALL_HEIGHT = 15;
        private const int CHAMBER_HEIGHT = 3;

        private const int LEFT = (Width - HALL_WIDTH) / 2;
        private const int CENTER = LEFT + HALL_WIDTH / 2;

        private int arenaDoor;
        private bool enteredArena = false;
        private bool keyDropped = false;

        public override string TilesTex()
        {
            return Assets.TILES_CITY;
        }

        public override string WaterTex()
        {
            return Assets.WATER_CITY;
        }

        private const string DOOR = "door";
        private const string ENTERED = "entered";
        private const string DROPPED = "droppped";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(DOOR, arenaDoor);
            bundle.Put(ENTERED, enteredArena);
            bundle.Put(DROPPED, keyDropped);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            arenaDoor = bundle.GetInt(DOOR);
            enteredArena = bundle.GetBoolean(ENTERED);
            keyDropped = bundle.GetBoolean(DROPPED);
        }

        protected override bool Build()
        {
            Painter.Fill(this, LEFT, TOP, HALL_WIDTH, HALL_HEIGHT, Terrain.EMPTY);
            Painter.Fill(this, CENTER, TOP, 1, HALL_HEIGHT, Terrain.EMPTY_SP);

            var y = TOP + 1;
            while (y < TOP + HALL_HEIGHT)
            {
                map[y * Width + CENTER - 2] = Terrain.STATUE_SP;
                map[y * Width + CENTER + 2] = Terrain.STATUE_SP;
                y += 2;
            }

            var left = Pedestal(true);
            var right = Pedestal(false);
            
            map[left] = map[right] = Terrain.PEDESTAL;

            for (var i = left + 1; i < right; i++)
                map[i] = Terrain.EMPTY_SP;

            exit = (TOP - 1) * Width + CENTER;
            map[exit] = Terrain.LOCKED_EXIT;

            arenaDoor = (TOP + HALL_HEIGHT) * Width + CENTER;
            map[arenaDoor] = Terrain.DOOR;

            Painter.Fill(this, LEFT, TOP + HALL_HEIGHT + 1, HALL_WIDTH, CHAMBER_HEIGHT, Terrain.EMPTY);
            Painter.Fill(this, LEFT, TOP + HALL_HEIGHT + 1, 1, CHAMBER_HEIGHT, Terrain.BOOKSHELF);
            Painter.Fill(this, LEFT + HALL_WIDTH - 1, TOP + HALL_HEIGHT + 1, 1, CHAMBER_HEIGHT, Terrain.BOOKSHELF);

            entrance = (TOP + HALL_HEIGHT + 2 + pdsharp.utils.Random.Int(CHAMBER_HEIGHT - 1)) * Width + LEFT + (Random.Int(HALL_WIDTH - 2)); //1 +
            map[entrance] = Terrain.ENTRANCE;

            return true;
        }

        protected internal override void Decorate()
        {
            for (var i = 0; i < Length; i++)
            {
                if (map[i] == Terrain.EMPTY && Random.Int(10) == 0)
                    map[i] = Terrain.EMPTY_DECO;
                else 
                    if (map[i] == Terrain.WALL && Random.Int(8) == 0)
                    map[i] = Terrain.WALL_DECO;
            }

            var sign = arenaDoor + Width + 1;
            map[sign] = Terrain.SIGN;
        }

        public static int Pedestal(bool left)
        {
            if (left)
                return (TOP + HALL_HEIGHT/2)*Width + CENTER - 2;
            
            return (TOP + HALL_HEIGHT/2)*Width + CENTER + 2;
        }

        protected internal override void CreateMobs()
        {
        }

        public override Actor Respawner()
        {
            return null;
        }

        protected internal override void CreateItems()
        {
            var item = Bones.Get();
            
            if (item == null) 
                return;

            int pos;
            do
            {
                pos = Random.IntRange(LEFT + 1, LEFT + HALL_WIDTH - 2) + Random.IntRange(TOP + HALL_HEIGHT + 1, TOP + HALL_HEIGHT + CHAMBER_HEIGHT) * Width;
            } 
            while (pos == entrance || map[pos] == Terrain.SIGN);

            Drop(item, pos).HeapType = Heap.Type.Skeleton;
        }

        public override int RandomRespawnCell()
        {
            return -1;
        }

        public override void Press(int cell, Character hero)
        {
            base.Press(cell, hero);

            if (enteredArena || !OutsideEntraceRoom(cell) || hero != Dungeon.Hero) 
                return;

            enteredArena = true;

            var boss = Bestiary.Mob(Dungeon.Depth);
            boss.State = boss.HUNTING;
            do
            {
                boss.pos = pdsharp.utils.Random.Int(Length);
            } 
            while (!passable[boss.pos] || !OutsideEntraceRoom(boss.pos) || Dungeon.Visible[boss.pos]);

            GameScene.Add(boss);

            Set(arenaDoor, Terrain.LOCKED_DOOR);
            GameScene.UpdateMap(arenaDoor);
            Dungeon.Observe();
        }

        public override Heap Drop(Item item, int cell)
        {
            if (keyDropped || !(item is SkeletonKey)) 
                return base.Drop(item, cell);

            keyDropped = true;

            Set(arenaDoor, Terrain.DOOR);
            GameScene.UpdateMap(arenaDoor);
            Dungeon.Observe();

            return base.Drop(item, cell);
        }

        private bool OutsideEntraceRoom(int cell)
        {
            return cell / Width < arenaDoor / Width;
        }

        public override string TileName(int tile)
        {
            switch (tile)
            {
                case Terrain.WATER:
                    return "Suspiciously colored water";
                case Terrain.HIGH_GRASS:
                    return "High blooming flowers";
                default:
                    return base.TileName(tile);
            }
        }

        public override string TileDesc(int tile)
        {
            switch (tile)
            {
                case Terrain.ENTRANCE:
                    return "A ramp leads up to the upper depth.";
                case Terrain.EXIT:
                    return "A ramp leads down to the lower depth.";
                case Terrain.WALL_DECO:
                case Terrain.EMPTY_DECO:
                    return "Several tiles are missing here.";
                case Terrain.EMPTY_SP:
                    return "Thick carpet covers the floor.";
                case Terrain.STATUE:
                case Terrain.STATUE_SP:
                    return "The statue depicts some dwarf standing in a heroic stance.";
                case Terrain.BOOKSHELF:
                    return "The rows of books on different disciplines fill the bookshelf.";
                default:
                    return base.TileDesc(tile);
            }
        }

        public override void AddVisuals(Scene scene)
        {
            CityLevel.AddVisuals(this, scene);
        }
    }
}