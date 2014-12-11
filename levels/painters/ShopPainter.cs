using System.Collections.Generic;
using System.Linq;
using pdsharp.utils;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items;
using sharpdungeon.items.armor;
using sharpdungeon.items.bags;
using sharpdungeon.items.food;
using sharpdungeon.items.potions;
using sharpdungeon.items.weapon.melee;
using sharpdungeon.items.scrolls;

namespace sharpdungeon.levels.painters
{
    public class ShopPainter : Painter
    {
        private static int _pasWidth;
        private static int _pasHeight;

        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY_SP);

            _pasWidth = room.Width() - 2;
            _pasHeight = room.Height() - 2;
            var per = _pasWidth * 2 + _pasHeight * 2;

            var range = Range();

            var pos = XyToPoint(room, room.Entrance()) + (per - range.Length) / 2;
            foreach (var item in range)
            {
                var xy = PointToXy(room, (pos + per) % per);
                var cell = xy.X + xy.Y * Level.Width;

                if (level.heaps[cell] != null)
                {
                    do
                    {
                        cell = room.Random();
                    } 
                    while (level.heaps[cell] != null);
                }

                level.Drop(item, cell).HeapType = Heap.Type.ForSale;

                pos++;
            }

            PlaceShopkeeper(level, room);

            foreach (var door in room.Connected.Values)
                door.Set(Room.Door.DoorType.REGULAR);
        }

        private static Item[] Range()
        {
            var items = new List<Item>();

            switch (Dungeon.Depth)
            {
                case 6:
                    Item item;
                    if (Random.Int(2) == 0)
                        item = new Quarterstaff();
                    else
                        item = new Spear();
                    items.Add(item.Identify());
                    items.Add(new LeatherArmor().Identify());
                    items.Add(new SeedPouch());
                    items.Add(new Weightstone());
                    break;

                case 11:
                    Item item1;
                    if (Random.Int(2) == 0)
                        item1 = new Sword();
                    else
                        item1 = new Mace();
                    items.Add(item1.Identify());
                    items.Add(new MailArmor().Identify());
                    items.Add(new ScrollHolder());
                    items.Add(new Weightstone());
                    break;

                case 16:
                    Item item3;
                    if (Random.Int(2) == 0)
                        item3 = new Longsword();
                    else
                        item3 = new BattleAxe();
                    items.Add(item3.Identify());
                    items.Add(new ScaleArmor().Identify());
                    items.Add(new WandHolster());
                    items.Add(new Weightstone());
                    break;

                case 21:
                    switch (Random.Int(3))
                    {
                        case 0:
                            items.Add(new Glaive().Identify());
                            break;
                        case 1:
                            items.Add(new WarHammer().Identify());
                            break;
                        case 2:
                            items.Add(new PlateArmor().Identify());
                            break;
                    }
                    items.Add(new Torch());
                    items.Add(new Torch());
                    break;
            }

            items.Add(new PotionOfHealing());
            for (var i = 0; i < 3; i++)
                items.Add(Generator.Random(Generator.Category.POTION));

            items.Add(new ScrollOfIdentify());
            items.Add(new ScrollOfRemoveCurse());
            items.Add(new ScrollOfMagicMapping());
            items.Add(Generator.Random(Generator.Category.SCROLL));

            items.Add(new OverpricedRation());
            items.Add(new OverpricedRation());

            items.Add(new Ankh());

            var range = items.ToArray();
            Random.Shuffle(range);

            return range;
        }

        private static void PlaceShopkeeper(Level level, Room room)
        {
            int pos;
            do
            {
                pos = room.Random();
            }
            while (level.heaps[pos] != null);

            var shopkeeper = level is LastShopLevel ? new ImpShopkeeper() : new Shopkeeper();
            shopkeeper.pos = pos;
            level.mobs.Add(shopkeeper);

            if (!(level is LastShopLevel))
                return;

            foreach (var p in Level.NEIGHBOURS9.Select(neighbour => shopkeeper.pos + neighbour).Where(p => level.map[p] == Terrain.EMPTY_SP))
                level.map[p] = Terrain.WATER;
        }

        private static int XyToPoint(Room room, Point xy)
        {
            if (xy.Y == room.Top)
                return (xy.X - room.Left - 1);
            
            if (xy.X == room.Right)
                return (xy.Y - room.Top - 1) + _pasWidth;

            if (xy.Y == room.Bottom)
                return (room.Right - xy.X - 1) + _pasWidth + _pasHeight;
            
            if (xy.Y == room.Top + 1)
                return 0;
            
            return (room.Bottom - xy.Y - 1) + _pasWidth*2 + _pasHeight;
        }

        private static Point PointToXy(Room room, int p)
        {
            if (p < _pasWidth)
                return new Point(room.Left + 1 + p, room.Top + 1);
            
            if (p < _pasWidth + _pasHeight)
                return new Point(room.Right - 1, room.Top + 1 + (p - _pasWidth));
            
            if (p < _pasWidth*2 + _pasHeight)
                return new Point(room.Right - 1 - (p - (_pasWidth + _pasHeight)), room.Bottom - 1);
            
            return new Point(room.Left + 1, room.Bottom - 1 - (p - (_pasWidth*2 + _pasHeight)));
        }
    }
}