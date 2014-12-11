using System;
using System.Collections.Generic;
using System.Linq;
using pdsharp.utils;
using sharpdungeon.levels.painters;

namespace sharpdungeon.levels
{
    public enum RoomType
    {
        NULL,
        STANDARD,
        ENTRANCE,
        WEAK_FLOOR,
        MAGIC_WELL,
        CRYPT,
        POOL,
        GARDEN,
        LIBRARY,
        ARMORY,
        TREASURY,
        TRAPS,
        STORAGE,
        STATUE,
        LABORATORY,
        VAULT,
        EXIT,
        BOSS_EXIT,
        TUNNEL,
        PASSAGE,
        SHOP,
        BLACKSMITH,
        RAT_KING,
        PIT
    }

    public class Room : Rect, Node, Bundlable
    {
        public List<Room> Neigbours = new List<Room>();
        public Dictionary<Room, Door> Connected = new Dictionary<Room, Door>();

        public int distance;
        public int price = 1;

        public static Dictionary<RoomType, Type> RoomTypePainters = new Dictionary<RoomType, Type>
        {
            {RoomType.NULL, null},
            {RoomType.STANDARD, typeof(StandardPainter)},
            {RoomType.ENTRANCE, typeof(EntrancePainter)},
            {RoomType.EXIT, typeof(ExitPainter)},
            {RoomType.BOSS_EXIT, typeof(BossExitPainter)},
            {RoomType.TUNNEL, typeof(TunnelPainter)},
            {RoomType.PASSAGE, typeof(PassagePainter)},
            {RoomType.SHOP, typeof(ShopPainter)},
            {RoomType.BLACKSMITH, typeof(BlacksmithPainter)},
            {RoomType.TREASURY, typeof(TreasuryPainter)},
            {RoomType.ARMORY, typeof(ArmoryPainter)},
            {RoomType.LIBRARY, typeof(LibraryPainter)},
            {RoomType.LABORATORY, typeof(LaboratoryPainter)},
            {RoomType.VAULT, typeof(VaultPainter)},
            {RoomType.TRAPS, typeof(TrapsPainter)},
            {RoomType.STORAGE, typeof(StoragePainter)},
            {RoomType.MAGIC_WELL, typeof(MagicWellPainter)},
            {RoomType.GARDEN, typeof(GardenPainter)},
            {RoomType.CRYPT, typeof(CryptPainter)},
            {RoomType.STATUE, typeof(StatuePainter)},
            {RoomType.POOL, typeof(PoolPainter)},
            {RoomType.RAT_KING, typeof(RatKingPainter)},
            {RoomType.WEAK_FLOOR, typeof(WeakFloorPainter)},
            {RoomType.PIT, typeof(PitPainter)}
        };

        public void Paint(Level level, Room room)
        {
            var painter = Activator.CreateInstance(RoomTypePainters[type]) as Painter;

            if (painter != null)
                painter.Paint(level, room);
        }

        public static List<RoomType> SPECIALS = new List<RoomType>
        {
RoomType.WEAK_FLOOR, RoomType.MAGIC_WELL, RoomType.CRYPT, RoomType.POOL, RoomType.GARDEN, RoomType.LIBRARY,
                RoomType.ARMORY, RoomType.TREASURY, RoomType.TRAPS, RoomType.STORAGE, RoomType.STATUE, RoomType.LABORATORY, RoomType.VAULT
        };

        public RoomType type = RoomType.NULL;

        public virtual int Random()
        {
            return Random(0);
        }

        public virtual int Random(int m)
        {
            var x = pdsharp.utils.Random.Int(Left + 1 + m, Right - m);
            var y = pdsharp.utils.Random.Int(Top + 1 + m, Bottom - m);
            return x + y * Level.Width;
        }

        public virtual void AddNeigbour(Room other)
        {
            var i = Intersect(other);

            if ((i.Width() != 0 || i.Height() < 3) && (i.Height() != 0 || i.Width() < 3))
                return;

            Neigbours.Add(other);
            other.Neigbours.Add(this);
        }

        public virtual void Connect(Room room)
        {
            if (Connected.ContainsKey(room))
                return;

            Connected.Add(room, null);
            room.Connected.Add(this, null);
        }

        public virtual Door Entrance()
        {
            return Connected.Values.First();
        }

        public virtual bool Inside(int p)
        {
            var x = p % Level.Width;
            var y = p / Level.Width;
            return x > Left && y > Top && x < Right && y < Bottom;
        }

        public virtual Point Center()
        {
            return new Point((Left + Right) / 2 + (((Right - Left) & 1) == 1 ? pdsharp.utils.Random.Int(2) : 0), (Top + Bottom) / 2 + (((Bottom - Top) & 1) == 1 ? pdsharp.utils.Random.Int(2) : 0));
        }

        // **** Graph.Node interface ****

        public int Distance()
        {
            return distance;
        }

        public void Distance(int value)
        {
            distance = value;
        }

        public int Price()
        {
            return price;
        }

        public void Price(int value)
        {
            price = value;
        }

        ICollection<Node> Node.Edges()
        {
            return Neigbours.OfType<Node>().ToList();
        }

        public ICollection<Room> Edges()
        {
            return Neigbours;
        }

        public void StoreInBundle(Bundle bundle)
        {
            bundle.Put("left", Left);
            bundle.Put("top", Top);
            bundle.Put("right", Right);
            bundle.Put("bottom", Bottom);
            bundle.Put("type", type.ToString());
        }

        public void RestoreFromBundle(Bundle bundle)
        {
            Left = bundle.GetInt("left");
            Top = bundle.GetInt("top");
            Right = bundle.GetInt("right");
            Bottom = bundle.GetInt("bottom");
            type = (RoomType)Enum.Parse(typeof(RoomType), bundle.GetString("type"));
        }

        public static void ShuffleTypes()
        {
            var size = SPECIALS.Count;
            for (var i = 0; i < size - 1; i++)
            {
                var j = pdsharp.utils.Random.Int(i, size);
                if (j == i)
                    continue;
                var t = SPECIALS[i];
                SPECIALS[i] = SPECIALS[j];
                SPECIALS[j] = t;
            }
        }

        public static void UseType(RoomType type)
        {
            if (SPECIALS.Remove(type))
                SPECIALS.Add(type);
        }

        private const string ROOMS = "rooms";

        public static void RestoreRoomsFromBundle(Bundle bundle)
        {
            if (bundle.Contains(ROOMS))
            {
                SPECIALS.Clear();
                foreach (var type in bundle.GetStringArray(ROOMS))
                    SPECIALS.Add((RoomType)Enum.Parse(typeof(RoomType), type));
            }
            else
                ShuffleTypes();
        }

        public static void StoreRoomsInBundle(Bundle bundle)
        {
            var array = new string[SPECIALS.Count];
            for (var i = 0; i < array.Length; i++)
                array[i] = SPECIALS[i].ToString();
            bundle.Put(ROOMS, array);
        }

        public class Door : Point
        {
            public enum DoorType
            {
                EMPTY,
                TUNNEL,
                REGULAR,
                UNLOCKED,
                HIDDEN,
                BARRICADE,
                LOCKED
            }

            private DoorType _type = DoorType.EMPTY;

            public Door(int x, int y)
                : base(x, y)
            {
            }

            public DoorType Type
            {
                get { return _type; }
                set { _type = value; }
            }

            public virtual void Set(DoorType type)
            {
                if (type.CompareTo(_type) > 0)
                    _type = type;
            }
        }
    }
}