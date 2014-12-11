using System;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.items;
using sharpdungeon.levels;
using sharpdungeon.scenes;

namespace sharpdungeon.actors.blobs
{
    public class WellWater : Blob
    {
        protected internal int Pos;

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);

            for (var i = 0; i < Length; i++)
            {
                if (Cur[i] <= 0)
                    continue;

                Pos = i;
                break;
            }
        }

        protected internal override void Evolve()
        {
            Volume = Off[Pos] = Cur[Pos];

            if (!Dungeon.Visible[Pos])
                return;

            if (this is WaterOfAwareness)
                Journal.Add(Journal.Feature.WELL_OF_AWARENESS);
            else if (this is WaterOfHealth)
                Journal.Add(Journal.Feature.WELL_OF_HEALTH);
            else if (this is WaterOfTransmutation)
                Journal.Add(Journal.Feature.WELL_OF_TRANSMUTATION);
        }

        protected internal virtual bool Affect()
        {
            Heap heap;

            if (Pos == Dungeon.Hero.pos && AffectHero(Dungeon.Hero))
            {
                Volume = Off[Pos] = Cur[Pos] = 0;
                return true;
            }

            if ((heap = Dungeon.Level.heaps[Pos]) != null)
            {
                var oldItem = heap.Peek();
                var newItem = AffectItem(oldItem);

                if (newItem != null)
                {
                    if (newItem == oldItem)
                    {

                    }
                    else if (oldItem.Quantity() > 1)
                    {
                        oldItem.Quantity(oldItem.Quantity() - 1);
                        heap.Drop(newItem);
                    }
                    else
                        heap.Replace(oldItem, newItem);

                    heap.Sprite.Link();
                    Volume = Off[Pos] = Cur[Pos] = 0;

                    return true;
                }
                
                int newPlace;
                do
                {
                    newPlace = Pos + Level.NEIGHBOURS8[pdsharp.utils.Random.Int(8)];
                } while (!Level.passable[newPlace] && !Level.avoid[newPlace]);
                Dungeon.Level.Drop(heap.PickUp(), newPlace).Sprite.Drop(Pos);

                return false;
            }
            
            return false;
        }

        protected internal virtual bool AffectHero(Hero hero)
        {
            return false;
        }

        protected internal virtual Item AffectItem(Item item)
        {
            return null;
        }

        public override void Seed(int cell, int amount)
        {
            Cur[Pos] = 0;
            Pos = cell;
            Volume = Cur[Pos] = amount;
        }

        public static void AffectCell(int cell)
        {
            Type[] waters = { typeof(WaterOfHealth), typeof(WaterOfAwareness), typeof(WaterOfTransmutation) };
            
            foreach (var waterClass in waters)
            {
                var water = (WellWater)Dungeon.Level.Blobs[waterClass];
                
                if (water == null || water.Volume <= 0 || water.Pos != cell || !water.Affect()) 
                    continue;

                Level.Set(cell, Terrain.EMPTY_WELL);
                GameScene.UpdateMap(cell);

                return;
            }
        }
    }
}