using System.Collections.Generic;
using System.Linq;
using Java.Util;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects.particles;
using sharpdungeon.items;
using sharpdungeon.items.armor;
using sharpdungeon.items.food;
using sharpdungeon.items.potions;
using sharpdungeon.items.scrolls;
using sharpdungeon.levels.features;
using sharpdungeon.levels.painters;
using sharpdungeon.levels.traps;
using sharpdungeon.mechanics;
using sharpdungeon.plants;
using sharpdungeon.scenes;
using sharpdungeon.utils;
using System;
using sharpdungeon.actors.hero;

namespace sharpdungeon.levels
{
    public abstract class Level : Bundlable
    {
        public enum Feeling
        {
            NONE,
            CHASM,
            WATER,
            GRASS
        }

        public const int Width = 32;
        public const int Height = 32;
        public const int Length = Width * Height;

        public static readonly int[] NEIGHBOURS4 = { -Width, +1, +Width, -1 };
        public static readonly int[] NEIGHBOURS8 = { +1, -1, +Width, -Width, +1 + Width, +1 - Width, -1 + Width, -1 - Width };
        public static readonly int[] NEIGHBOURS9 = { 0, +1, -1, +Width, -Width, +1 + Width, +1 - Width, -1 + Width, -1 - Width };

        public const float TimeToRespawn = 50;

        private const string TXT_HIDDEN_PLATE_CLICKS = "A hidden pressure plate clicks!";

        public static bool resizingNeeded;
        // This one can be different from resizingNeeded if the Level
        // was created in the older version of the game
        public static int loadedMapSize;

        public int[] map;
        public bool[] visited;
        public bool[] mapped;

        public int viewDistance = Dungeon.IsChallenged(Challenges.DARKNESS) ? 3 : 8;

        public static bool[] fieldOfView = new bool[Length];

        public static bool[] passable = new bool[Length];
        public static bool[] losBlocking = new bool[Length];
        public static bool[] flamable = new bool[Length];
        public static bool[] secret = new bool[Length];
        public static bool[] solid = new bool[Length];
        public static bool[] avoid = new bool[Length];
        public static bool[] water = new bool[Length];
        public static bool[] pit = new bool[Length];

        public static bool[] discoverable = new bool[Length];

        public Feeling feeling = Feeling.NONE;

        public int entrance;
        public int exit;

        public HashSet<Mob> mobs;
        public SparseArray<Heap> heaps;

        public Dictionary<Type, Blob> Blobs;
        public SparseArray<Plant> plants;

        protected internal List<Item> itemsToSpawn = new List<Item>();

        public int color1 = 0x004400;
        public int color2 = 0x88CC44;

        protected internal static bool pitRoomNeeded = false;
        protected internal static bool weakFloorCreated = false;

        private const string MAP = "map";
        private const string VISITED = "visited";
        private const string MAPPED = "mapped";
        private const string ENTRANCE = "entrance";
        private const string EXIT = "exit";
        private const string HEAPS = "heaps";
        private const string PLANTS = "plants";
        private const string MOBS = "mobs";
        private const string BLOBS = "blobs";

        public virtual void Create()
        {
            resizingNeeded = false;

            map = new int[Length];
            visited = new bool[Length];
            mapped = new bool[Length];

            mobs = new HashSet<Mob>();
            heaps = new SparseArray<Heap>();
            Blobs = new Dictionary<Type, Blob>();
            plants = new SparseArray<Plant>();

            if (!Dungeon.BossLevel())
            {
                AddItemToSpawn(Generator.Random(Generator.Category.FOOD));
#if !DEBUG
                if (Dungeon.PosNeeded())
                {
                    AddItemToSpawn(new PotionOfStrength());
                    Dungeon.PotionOfStrength++;
                }
                if (Dungeon.SoeNeeded())
                {
                    AddItemToSpawn(new ScrollOfUpgrade());
                    Dungeon.ScrollsOfUpgrade++;
                }
                if (Dungeon.AsNeeded())
                {
                    AddItemToSpawn(new Stylus());
                    Dungeon.ArcaneStyli++;
                }

                if (Dungeon.Depth > 1)
                {
                    switch (pdsharp.utils.Random.Int(10))
                    {
                        case 0:
                            if (!Dungeon.BossLevel(Dungeon.Depth + 1))
                            {
                                feeling = Feeling.CHASM;
                            }
                            break;
                        case 1:
                            feeling = Feeling.WATER;
                            break;
                        case 2:
                            feeling = Feeling.GRASS;
                            break;
                    }
                }
#endif
            }


            var pitNeeded = Dungeon.Depth > 1 && weakFloorCreated;

            do
            {
                var terrain = feeling == Feeling.CHASM ? Terrain.CHASM : Terrain.WALL;
                for (var i = 0; i < map.Length; i++)
                    map[i] = terrain;

                pitRoomNeeded = pitNeeded;
                weakFloorCreated = false;

            }
            while (!Build());

            Decorate();

            BuildFlagMaps();
            CleanWalls();

            CreateMobs();
            CreateItems();
        }

        public virtual void Reset()
        {
            foreach (var mob in mobs.ToArray().Where(mob => !mob.Reset()))
                mobs.Remove(mob);

            CreateMobs();
        }

        public virtual void RestoreFromBundle(Bundle bundle)
        {
            mobs = new HashSet<Mob>();
            heaps = new SparseArray<Heap>();

            Blobs = new Dictionary<Type, Blob>();
            plants = new SparseArray<Plant>();

            map = bundle.GetIntArray(MAP);
            visited = bundle.GetBooleanArray(VISITED);
            mapped = bundle.GetBooleanArray(MAPPED);

            entrance = bundle.GetInt(ENTRANCE);
            exit = bundle.GetInt(EXIT);

            weakFloorCreated = false;

            AdjustMapSize();

            var collection = bundle.GetCollection(HEAPS);
            foreach (var h in collection)
            {
                var heap = (Heap)h;
                if (resizingNeeded)
                    heap.Pos = AdjustPos(heap.Pos);

                heaps.Add(heap.Pos, heap);
            }

            collection = bundle.GetCollection(PLANTS);
            foreach (var p in collection)
            {
                var plant = (Plant)p;
                if (resizingNeeded)
                    plant.Pos = AdjustPos(plant.Pos);

                plants.Add(plant.Pos, plant);
            }

            collection = bundle.GetCollection(MOBS);
            foreach (var m in collection)
            {
                var mob = (Mob)m;
                if (mob == null)
                    continue;

                if (resizingNeeded)
                    mob.pos = AdjustPos(mob.pos);

                mobs.Add(mob);
            }

            collection = bundle.GetCollection(BLOBS);
            foreach (var blob in collection.OfType<Blob>())
                Blobs.Add(blob.GetType(), blob);

            BuildFlagMaps();
            CleanWalls();
        }

        public virtual void StoreInBundle(Bundle bundle)
        {
            bundle.Put(MAP, map);
            bundle.Put(VISITED, visited);
            bundle.Put(MAPPED, mapped);
            bundle.Put(ENTRANCE, entrance);
            bundle.Put(EXIT, exit);
            bundle.Put(HEAPS, heaps);
            bundle.Put(PLANTS, plants);
            bundle.Put(MOBS, mobs);
            bundle.Put(BLOBS, Blobs.Values);
        }

        public virtual int TunnelTile()
        {
            return feeling == Feeling.CHASM ? Terrain.EMPTY_SP : Terrain.EMPTY;
        }

        private void AdjustMapSize()
        {
            // For levels from older saves
            if (map.Length < Length)
            {
                resizingNeeded = true;
                loadedMapSize = (int)Math.Sqrt(map.Length);

                var localMap = new int[Length];
                Arrays.Fill(localMap, Terrain.WALL);

                var visited = new bool[Length];
                Arrays.Fill(visited, false);

                var mapped = new bool[Length];
                Arrays.Fill(mapped, false);

                for (int i = 0; i < loadedMapSize; i++)
                {
                    Array.Copy(this.map, i * loadedMapSize, localMap, i * Width, loadedMapSize);
                    Array.Copy(this.visited, i * loadedMapSize, visited, i * Width, loadedMapSize);
                    Array.Copy(this.mapped, i * loadedMapSize, mapped, i * Width, loadedMapSize);
                }

                this.map = localMap;
                this.visited = visited;
                this.mapped = mapped;

                entrance = AdjustPos(entrance);
                exit = AdjustPos(exit);
            }
            else
                resizingNeeded = false;
        }

        public virtual int AdjustPos(int pos)
        {
            return (pos / loadedMapSize) * Width + (pos % loadedMapSize);
        }

        public virtual string TilesTex()
        {
            return null;
        }

        public virtual string WaterTex()
        {
            return null;
        }

        protected abstract bool Build();
        protected internal abstract void Decorate();
        protected internal abstract void CreateMobs();
        protected internal abstract void CreateItems();

        public virtual void AddVisuals(Scene scene)
        {
            for (var i = 0; i < Length; i++)
                if (pit[i])
                {
                    scene.Add(new WindParticle.Wind(i));
                    if (i >= Width && water[i - Width])
                        scene.Add(new FlowParticle.Flow(i - Width));
                }
        }

        public virtual int NMobs()
        {
            return 0;
        }

        public virtual Actor Respawner()
        {
            return new RespawnerActor(this);
        }

        public virtual int RandomRespawnCell()
        {
            int cell;
            do
            {
                cell = pdsharp.utils.Random.Int(Length);
            } while (!passable[cell] || Dungeon.Visible[cell] || Actor.FindChar(cell) != null);
            return cell;
        }

        public virtual int RandomDestination()
        {
            int cell;
            do
            {
                cell = pdsharp.utils.Random.Int(Length);
            } while (!passable[cell]);
            return cell;
        }

        public virtual void AddItemToSpawn(Item item)
        {
            if (item != null)
                itemsToSpawn.Add(item);
        }

        public virtual Item ItemToSpanAsPrize()
        {
            if (pdsharp.utils.Random.Int(itemsToSpawn.Count + 1) <= 0)
                return null;

            var item = pdsharp.utils.Random.Element(itemsToSpawn.ToArray());
            itemsToSpawn.Remove(item);
            return item;
        }

        private void BuildFlagMaps()
        {
            for (var i = 0; i < Length; i++)
            {
                var flags = Terrain.Flags[map[i]];
                passable[i] = (flags & Terrain.PASSABLE) != 0;
                losBlocking[i] = (flags & Terrain.LOS_BLOCKING) != 0;
                flamable[i] = (flags & Terrain.FLAMABLE) != 0;
                secret[i] = (flags & Terrain.SECRET) != 0;
                solid[i] = (flags & Terrain.SOLID) != 0;
                avoid[i] = (flags & Terrain.AVOID) != 0;
                water[i] = (flags & Terrain.LIQUID) != 0;
                pit[i] = (flags & Terrain.PIT) != 0;
            }

            const int lastRow = Length - Width;
            for (var i = 0; i < Width; i++)
            {
                passable[i] = avoid[i] = false;
                passable[lastRow + i] = avoid[lastRow + i] = false;
            }

            for (var i = Width; i < lastRow; i += Width)
            {
                passable[i] = avoid[i] = false;
                passable[i + Width - 1] = avoid[i + Width - 1] = false;
            }

            for (var i = Width; i < Length - Width; i++)
            {
                if (water[i])
                {
                    var t = Terrain.WATER_TILES;

                    for (var j = 0; j < NEIGHBOURS4.Length; j++)
                        if ((Terrain.Flags[map[i + NEIGHBOURS4[j]]] & Terrain.UNSTITCHABLE) != 0)
                            t += 1 << j;

                    map[i] = t;
                }

                if (!pit[i])
                    continue;

                if (pit[i - Width])
                    continue;

                var c = map[i - Width];
                if (c == Terrain.EMPTY_SP || c == Terrain.STATUE_SP)
                    map[i] = Terrain.CHASM_FLOOR_SP;
                else if (water[i - Width])
                    map[i] = Terrain.CHASM_WATER;
                else if ((Terrain.Flags[c] & Terrain.UNSTITCHABLE) != 0)
                    map[i] = Terrain.CHASM_WALL;
                else
                    map[i] = Terrain.CHASM_FLOOR;
            }
        }

        private void CleanWalls()
        {
            for (var i = 0; i < Length; i++)
            {
                var discover = false;

                foreach (var neighbour in NEIGHBOURS9)
                {
                    var n = i + neighbour;

                    if (n < 0 || n >= Length || map[n] == Terrain.WALL || map[n] == Terrain.WALL_DECO)
                        continue;

                    discover = true;
                    break;
                }

                if (discover)
                {
                    discover = false;

                    foreach (var neighbour in NEIGHBOURS9)
                    {
                        var n = i + neighbour;
                        if (n < 0 || n >= Length || pit[n])
                            continue;

                        discover = true;
                        break;
                    }
                }

                discoverable[i] = discover;
            }
        }

        public static void Set(int cell, int terrain)
        {
            Painter.Set(Dungeon.Level, cell, terrain);

            var flags = Terrain.Flags[terrain];
            passable[cell] = (flags & Terrain.PASSABLE) != 0;
            losBlocking[cell] = (flags & Terrain.LOS_BLOCKING) != 0;
            flamable[cell] = (flags & Terrain.FLAMABLE) != 0;
            secret[cell] = (flags & Terrain.SECRET) != 0;
            solid[cell] = (flags & Terrain.SOLID) != 0;
            avoid[cell] = (flags & Terrain.AVOID) != 0;
            pit[cell] = (flags & Terrain.PIT) != 0;
            water[cell] = terrain == Terrain.WATER || terrain >= Terrain.WATER_TILES;
        }

        public virtual Heap Drop(Item item, int cell)
        {
            if (Dungeon.IsChallenged(Challenges.NO_FOOD) && item is Food)
                item = new Gold(item.Price());
            else if (Dungeon.IsChallenged(Challenges.NO_ARMOR) && item is Armor)
                item = new Gold(item.Price());
            else if (Dungeon.IsChallenged(Challenges.NO_HEALING) && item is PotionOfHealing)
                item = new Gold(item.Price());

            if ((map[cell] == Terrain.ALCHEMY) && !(item is Plant.Seed))
            {
                int n;
                do
                {
                    n = cell + NEIGHBOURS8[pdsharp.utils.Random.Int(8)];
                } while (map[n] != Terrain.EMPTY_SP);
                cell = n;
            }

            var heap = heaps[cell];
            if (heap == null)
            {
                heap = new Heap();
                heap.Pos = cell;
                if (map[cell] == Terrain.CHASM || (Dungeon.Level != null && pit[cell]))
                    GameScene.Discard(heap);
                else
                {
                    heaps.Add(cell, heap);
                    GameScene.Add(heap);
                }
            }
            else
                if (heap.HeapType == Heap.Type.LockedChest || heap.HeapType == Heap.Type.CrystalChest)
                {
                    int n;
                    do
                    {
                        n = cell + NEIGHBOURS8[pdsharp.utils.Random.Int(8)];
                    }
                    while (!passable[n] && !avoid[n]);

                    return Drop(item, n);
                }

            heap.Drop(item);

            if (Dungeon.Level != null)
                Press(cell, null);

            return heap;
        }

        public virtual Plant Plant(Plant.Seed seed, int pos)
        {
            var plant = plants[pos];
            if (plant != null)
                plant.Wither();

            plant = seed.Couch(pos);
            plants.Add(pos, plant);

            GameScene.Add(plant);

            return plant;
        }

        public virtual void Uproot(int pos)
        {
            plants.Remove(pos);
        }

        public virtual int PitCell()
        {
            return RandomRespawnCell();
        }

        public virtual void Press(int cell, actors.Character ch)
        {
            if (pit[cell] && ch == Dungeon.Hero)
            {
                Chasm.HeroFall(cell);
                return;
            }

            var trap = false;

            switch (map[cell])
            {

                case Terrain.SECRET_TOXIC_TRAP:
                    GLog.Information(TXT_HIDDEN_PLATE_CLICKS);
                    goto case Terrain.TOXIC_TRAP;
                case Terrain.TOXIC_TRAP:
                    trap = true;
                    ToxicTrap.Trigger(cell, ch);
                    break;

                case Terrain.SECRET_FIRE_TRAP:
                    GLog.Information(TXT_HIDDEN_PLATE_CLICKS);
                    goto case Terrain.FIRE_TRAP;
                case Terrain.FIRE_TRAP:
                    trap = true;
                    FireTrap.Trigger(cell, ch);
                    break;

                case Terrain.SECRET_PARALYTIC_TRAP:
                    GLog.Information(TXT_HIDDEN_PLATE_CLICKS);
                    goto case Terrain.PARALYTIC_TRAP;
                case Terrain.PARALYTIC_TRAP:
                    trap = true;
                    ParalyticTrap.Trigger(cell, ch);
                    break;

                case Terrain.SECRET_POISON_TRAP:
                    GLog.Information(TXT_HIDDEN_PLATE_CLICKS);
                    goto case Terrain.POISON_TRAP;
                case Terrain.POISON_TRAP:
                    trap = true;
                    PoisonTrap.Trigger(cell, ch);
                    break;

                case Terrain.SECRET_ALARM_TRAP:
                    GLog.Information(TXT_HIDDEN_PLATE_CLICKS);
                    goto case Terrain.ALARM_TRAP;
                case Terrain.ALARM_TRAP:
                    trap = true;
                    AlarmTrap.Trigger(cell, ch);
                    break;

                case Terrain.SECRET_LIGHTNING_TRAP:
                    GLog.Information(TXT_HIDDEN_PLATE_CLICKS);
                    goto case Terrain.LIGHTNING_TRAP;
                case Terrain.LIGHTNING_TRAP:
                    trap = true;
                    LightningTrap.Trigger(cell, ch);
                    break;

                case Terrain.SECRET_GRIPPING_TRAP:
                    GLog.Information(TXT_HIDDEN_PLATE_CLICKS);
                    goto case Terrain.GRIPPING_TRAP;
                case Terrain.GRIPPING_TRAP:
                    trap = true;
                    GrippingTrap.Trigger(cell, ch);
                    break;

                case Terrain.SECRET_SUMMONING_TRAP:
                    GLog.Information(TXT_HIDDEN_PLATE_CLICKS);
                    goto case Terrain.SUMMONING_TRAP;
                case Terrain.SUMMONING_TRAP:
                    trap = true;
                    SummoningTrap.Trigger(cell, ch);
                    break;

                case Terrain.HIGH_GRASS:
                    HighGrass.Trample(this, cell, ch);
                    break;

                case Terrain.WELL:
                    WellWater.AffectCell(cell);
                    break;

                case Terrain.ALCHEMY:
                    if (ch == null)
                        Alchemy.Transmute(cell);
                    break;

                case Terrain.DOOR:
                    Door.Enter(cell);
                    break;
            }

            if (trap)
            {
                Sample.Instance.Play(Assets.SND_TRAP);

                if (ch == Dungeon.Hero)
                    Dungeon.Hero.Interrupt();

                Set(cell, Terrain.INACTIVE_TRAP);
                GameScene.UpdateMap(cell);
            }

            var plant = plants[cell];
            if (plant != null)
                plant.Activate(ch);
        }

        public virtual void MobPress(Mob mob)
        {
            int cell = mob.pos;

            if (pit[cell] && !mob.Flying)
            {
                Chasm.MobFall(mob);
                return;
            }

            bool trap = true;
            switch (map[cell])
            {

                case Terrain.TOXIC_TRAP:
                    ToxicTrap.Trigger(cell, mob);
                    break;

                case Terrain.FIRE_TRAP:
                    FireTrap.Trigger(cell, mob);
                    break;

                case Terrain.PARALYTIC_TRAP:
                    ParalyticTrap.Trigger(cell, mob);
                    break;

                case Terrain.POISON_TRAP:
                    PoisonTrap.Trigger(cell, mob);
                    break;

                case Terrain.ALARM_TRAP:
                    AlarmTrap.Trigger(cell, mob);
                    break;

                case Terrain.LIGHTNING_TRAP:
                    LightningTrap.Trigger(cell, mob);
                    break;

                case Terrain.GRIPPING_TRAP:
                    GrippingTrap.Trigger(cell, mob);
                    break;

                case Terrain.SUMMONING_TRAP:
                    SummoningTrap.Trigger(cell, mob);
                    break;

                case Terrain.DOOR:
                    Door.Enter(cell);

                    goto default;
                default:
                    trap = false;
                    break;
            }

            if (trap)
            {
                if (Dungeon.Visible[cell])
                    Sample.Instance.Play(Assets.SND_TRAP);

                Set(cell, Terrain.INACTIVE_TRAP);
                GameScene.UpdateMap(cell);
            }

            var plant = plants[cell];
            if (plant != null)
                plant.Activate(mob);
        }

        public virtual bool[] UpdateFieldOfView(actors.Character c)
        {
            var cx = c.pos % Width;
            var cy = c.pos / Width;

            bool sighted = c.Buff<Blindness>() == null && c.Buff<Shadows>() == null && c.IsAlive;
            if (sighted)
                ShadowCaster.CastShadow(cx, cy, fieldOfView, c.viewDistance);
            else
                Arrays.Fill(fieldOfView, false);

            var sense = 1;
            if (c.IsAlive)
                sense = c.Buffs<MindVision>().Select(b => b.Distance).Concat(new[] { sense }).Max();

            if ((sighted && sense > 1) || !sighted)
            {
                var ax = Math.Max(0, cx - sense);
                var bx = Math.Min(cx + sense, Width - 1);
                var ay = Math.Max(0, cy - sense);
                var by = Math.Min(cy + sense, Height - 1);

                var len = bx - ax + 1;
                var pos = ax + ay * Width;
                for (var y = ay; y <= by; y++, pos += Width)
                    Arrays.Fill(fieldOfView, pos, pos + len, true);

                for (var i = 0; i < Length; i++)
                    fieldOfView[i] &= discoverable[i];
            }

            if (!c.IsAlive)
                return fieldOfView;

            if (c.Buff<MindVision>() != null)
            {
                foreach (var mob in mobs)
                {
                    var p = mob.pos;
                    fieldOfView[p] = true;
                    fieldOfView[p + 1] = true;
                    fieldOfView[p - 1] = true;
                    fieldOfView[p + Width + 1] = true;
                    fieldOfView[p + Width - 1] = true;
                    fieldOfView[p - Width + 1] = true;
                    fieldOfView[p - Width - 1] = true;
                    fieldOfView[p + Width] = true;
                    fieldOfView[p - Width] = true;
                }
            }
            else
                if (c == Dungeon.Hero && ((Hero)c).heroClass == HeroClass.Huntress)
                {
                    foreach (var mob in mobs)
                    {
                        var p = mob.pos;
                        if (Distance(c.pos, p) != 2)
                            continue;

                        fieldOfView[p] = true;
                        fieldOfView[p + 1] = true;
                        fieldOfView[p - 1] = true;
                        fieldOfView[p + Width + 1] = true;
                        fieldOfView[p + Width - 1] = true;
                        fieldOfView[p - Width + 1] = true;
                        fieldOfView[p - Width - 1] = true;
                        fieldOfView[p + Width] = true;
                        fieldOfView[p - Width] = true;
                    }
                }

            if (c.Buff<Awareness>() != null)
                foreach (var heap in heaps.Values)
                {
                    var p = heap.Pos;
                    fieldOfView[p] = true;
                    fieldOfView[p + 1] = true;
                    fieldOfView[p - 1] = true;
                    fieldOfView[p + Width + 1] = true;
                    fieldOfView[p + Width - 1] = true;
                    fieldOfView[p - Width + 1] = true;
                    fieldOfView[p - Width - 1] = true;
                    fieldOfView[p + Width] = true;
                    fieldOfView[p - Width] = true;
                }

            return fieldOfView;
        }

        public static int Distance(int a, int b)
        {
            var ax = a % Width;
            var ay = a / Width;
            var bx = b % Width;
            var by = b / Width;
            return Math.Max(Math.Abs(ax - bx), Math.Abs(ay - by));
        }

        public static bool Adjacent(int a, int b)
        {
            var diff = Math.Abs(a - b);
            return diff == 1 || diff == Width || diff == Width + 1 || diff == Width - 1;
        }

        public virtual string TileName(int tile)
        {
            if (tile >= Terrain.WATER_TILES)
                return TileName(Terrain.WATER);

            if (tile != Terrain.CHASM && (Terrain.Flags[tile] & Terrain.PIT) != 0)
                return TileName(Terrain.CHASM);

            switch (tile)
            {
                case Terrain.CHASM:
                    return "Chasm";
                case Terrain.EMPTY:
                case Terrain.EMPTY_SP:
                case Terrain.EMPTY_DECO:
                case Terrain.SECRET_TOXIC_TRAP:
                case Terrain.SECRET_FIRE_TRAP:
                case Terrain.SECRET_PARALYTIC_TRAP:
                case Terrain.SECRET_POISON_TRAP:
                case Terrain.SECRET_ALARM_TRAP:
                case Terrain.SECRET_LIGHTNING_TRAP:
                    return "Floor";
                case Terrain.GRASS:
                    return "Grass";
                case Terrain.WATER:
                    return "Water";
                case Terrain.WALL:
                case Terrain.WALL_DECO:
                case Terrain.SECRET_DOOR:
                    return "Wall";
                case Terrain.DOOR:
                    return "Closed door";
                case Terrain.OPEN_DOOR:
                    return "Open door";
                case Terrain.ENTRANCE:
                    return "Depth entrance";
                case Terrain.EXIT:
                    return "Depth exit";
                case Terrain.EMBERS:
                    return "Embers";
                case Terrain.LOCKED_DOOR:
                    return "Locked door";
                case Terrain.PEDESTAL:
                    return "Pedestal";
                case Terrain.BARRICADE:
                    return "Barricade";
                case Terrain.HIGH_GRASS:
                    return "High grass";
                case Terrain.LOCKED_EXIT:
                    return "Locked depth exit";
                case Terrain.UNLOCKED_EXIT:
                    return "Unlocked depth exit";
                case Terrain.SIGN:
                    return "Sign";
                case Terrain.WELL:
                    return "Well";
                case Terrain.EMPTY_WELL:
                    return "Empty well";
                case Terrain.STATUE:
                case Terrain.STATUE_SP:
                    return "Statue";
                case Terrain.TOXIC_TRAP:
                    return "Toxic gas trap";
                case Terrain.FIRE_TRAP:
                    return "Fire trap";
                case Terrain.PARALYTIC_TRAP:
                    return "Paralytic gas trap";
                case Terrain.POISON_TRAP:
                    return "Poison dart trap";
                case Terrain.ALARM_TRAP:
                    return "Alarm trap";
                case Terrain.LIGHTNING_TRAP:
                    return "Lightning trap";
                case Terrain.GRIPPING_TRAP:
                    return "Gripping trap";
                case Terrain.SUMMONING_TRAP:
                    return "Summoning trap";
                case Terrain.INACTIVE_TRAP:
                    return "Triggered trap";
                case Terrain.BOOKSHELF:
                    return "Bookshelf";
                case Terrain.ALCHEMY:
                    return "Alchemy pot";
                default:
                    return "???";
            }
        }

        public virtual string TileDesc(int tile)
        {
            switch (tile)
            {
                case Terrain.CHASM:
                    return "You can't see the bottom.";
                case Terrain.WATER:
                    return "In case of burning step into the water to extinguish the fire.";
                case Terrain.ENTRANCE:
                    return "Stairs lead up to the upper depth.";
                case Terrain.EXIT:
                case Terrain.UNLOCKED_EXIT:
                    return "Stairs lead down to the lower depth.";
                case Terrain.EMBERS:
                    return "Embers cover the floor.";
                case Terrain.HIGH_GRASS:
                    return "Dense vegetation blocks the view.";
                case Terrain.LOCKED_DOOR:
                    return "This door is locked, you need a matching key to unlock it.";
                case Terrain.LOCKED_EXIT:
                    return "Heavy bars block the stairs leading down.";
                case Terrain.BARRICADE:
                    return "The wooden barricade is firmly set but has dried over the years. Might it burn?";
                case Terrain.SIGN:
                    return "You can't read the bitmapText from here.";
                case Terrain.TOXIC_TRAP:
                case Terrain.FIRE_TRAP:
                case Terrain.PARALYTIC_TRAP:
                case Terrain.POISON_TRAP:
                case Terrain.ALARM_TRAP:
                case Terrain.LIGHTNING_TRAP:
                case Terrain.GRIPPING_TRAP:
                case Terrain.SUMMONING_TRAP:
                    return "Stepping onto a hidden pressure plate will Activate the trap.";
                case Terrain.INACTIVE_TRAP:
                    return "The trap has been triggered before and it's not dangerous anymore.";
                case Terrain.STATUE:
                case Terrain.STATUE_SP:
                    return "Someone wanted to adorn this place, but failed, obviously.";
                case Terrain.ALCHEMY:
                    return "Drop some seeds here to cook a potion.";
                case Terrain.EMPTY_WELL:
                    return "The well has run dry.";
                default:
                    if (tile >= Terrain.WATER_TILES)
                        return TileDesc(Terrain.WATER);

                    if ((Terrain.Flags[tile] & Terrain.PIT) != 0)
                        return TileDesc(Terrain.CHASM);

                    return "";
            }
        }
    }

    public class RespawnerActor : Actor
    {
        private readonly Level _level;

        public RespawnerActor(Level level)
        {
            _level = level;
        }

        protected override bool Act()
        {
            if (_level.mobs.Count < _level.NMobs())
            {
                var mob = Bestiary.Mutable(Dungeon.Depth);
                mob.State = mob.WANDERING;
                mob.pos = _level.RandomRespawnCell();
                if (Dungeon.Hero.IsAlive && mob.pos != -1)
                {
                    GameScene.Add(mob);
                    if (Statistics.AmuletObtained)
                        mob.Beckon(Dungeon.Hero.pos);
                }
            }

            Spend(Dungeon.NightMode || Statistics.AmuletObtained ? Level.TimeToRespawn / 2 : Level.TimeToRespawn);

            return true;
        }
    }
}