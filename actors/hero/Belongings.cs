using System.Collections;
using System.Collections.Generic;
using System.Linq;
using pdsharp.utils;
using sharpdungeon.items;
using sharpdungeon.items.armor;
using sharpdungeon.items.bags;
using sharpdungeon.items.keys;
using sharpdungeon.items.rings;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.wands;

namespace sharpdungeon.actors.hero
{
    public class Belongings : List<Item>
    {
        public const int BackpackSize = 19;

        private readonly Hero _owner;

        public Bag Backpack;

        public KindOfWeapon Weapon = null;
        public Armor Armor = null;
        public Ring Ring1 = null;
        public Ring Ring2 = null;

        public Belongings(Hero owner)
        {
            _owner = owner;

            Backpack = new Bag
            {
                Name = "backpack",
                Size = BackpackSize
            };

            Backpack.Owner = owner;
        }

        private const string WEAPON = "weapon";
        private const string ARMOR = "armor";
        private const string RING1 = "ring1";
        private const string RING2 = "ring2";

        public virtual void StoreInBundle(Bundle bundle)
        {
            Backpack.StoreInBundle(bundle);

            bundle.Put(WEAPON, Weapon);
            bundle.Put(ARMOR, Armor);
            bundle.Put(RING1, Ring1);
            bundle.Put(RING2, Ring2);
        }

        public virtual void RestoreFromBundle(Bundle bundle)
        {
            Backpack.Clear();
            Backpack.RestoreFromBundle(bundle);

            Weapon = (KindOfWeapon)bundle.Get(WEAPON);
            if (Weapon != null)
                Weapon.Activate(_owner);

            Armor = (Armor)bundle.Get(ARMOR);

            Ring1 = (Ring)bundle.Get(RING1);
            if (Ring1 != null)
                Ring1.Activate(_owner);

            Ring2 = (Ring)bundle.Get(RING2);
            if (Ring2 != null)
                Ring2.Activate(_owner);
        }

        public virtual Item GetItem(Item item)
        {
            return this.FirstOrDefault(i => i == item);
        }

        public virtual T GetItem<T>() where T : Item
        {
            return this.OfType<T>().FirstOrDefault();
        }

        public virtual T GetKey<T>(int depth) where T : Key
        {
            return Backpack.Items.OfType<T>().FirstOrDefault(key => key.depth == depth);
        }

        public virtual void CountIronKeys()
        {
            IronKey.CurDepthQuantity = Backpack.Items.OfType<IronKey>().Count(item => item.depth == Dungeon.Depth);
        }

        public virtual void Identify()
        {
            foreach (var item in this)
                item.Identify();
        }

        public virtual void Observe()
        {
            if (Weapon != null)
            {
                Weapon.Identify();
                Badge.ValidateItemLevelAquired(Weapon);
            }

            if (Armor != null)
            {
                Armor.Identify();
                Badge.ValidateItemLevelAquired(Armor);
            }

            if (Ring1 != null)
            {
                Ring1.Identify();
                Badge.ValidateItemLevelAquired(Ring1);
            }

            if (Ring2 != null)
            {
                Ring2.Identify();
                Badge.ValidateItemLevelAquired(Ring2);
            }

            foreach (var item in Backpack.Items)
                item.cursedKnown = true;
        }

        public virtual void UncurseEquipped()
        {
            ScrollOfRemoveCurse.Uncurse(_owner, Armor, Weapon, Ring1, Ring2);
        }

        public virtual Item RandomUnequipped()
        {
            return Random.Element(Backpack.Items);
        }

        public virtual void Resurrect(int depth)
        {
            foreach (var item in Backpack.Items.ToArray())
            {
                var key = item as Key;
                if (key != null)
                {
                    if (key.depth == depth)
                        key.DetachAll(Backpack);
                }
                else
                    if (!item.IsEquipped(_owner))
                        item.DetachAll(Backpack);
            }

            if (Weapon != null)
            {
                Weapon.cursed = false;
                Weapon.Activate(_owner);
            }

            if (Armor != null)
                Armor.cursed = false;

            if (Ring1 != null)
            {
                Ring1.cursed = false;
                Ring1.Activate(_owner);
            }

            if (Ring2 != null)
            {
                Ring2.cursed = false;
                Ring2.Activate(_owner);
            }
        }

        public virtual int Charge(bool full)
        {
            var count = 0;

            foreach (var wand in this.OfType<Wand>().Where(wand => wand.CurrrentCharges < wand.MaxCharges))
            {
                wand.CurrrentCharges = full ? wand.MaxCharges : wand.CurrrentCharges + 1;
                count++;

                wand.UpdateQuickslot();
            }

            return count;
        }

        public virtual int Discharge()
        {
            var count = 0;

            foreach (var wand in this.OfType<Wand>().Where(wand => wand.CurrrentCharges > 0))
            {
                wand.CurrrentCharges--;
                count++;

                wand.UpdateQuickslot();
            }

            return count;
        }

        // private class ItemIterator : IEnumerator<Item>
        //{

        //    private int index = 0;

        //    //JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class Instance members within a nested class:
        //    private IEnumerator<Item> backpackIterator = Backpack.GetEnumerator();

        //    //JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class Instance members within a nested class:
        //    private Item[] equipped = { Weapon, Armor, Ring1, Ring2 };
        //    private int backpackIndex = equipped.Length;

        //    public override bool hasNext()
        //    {

        //        for (int i = index; i < backpackIndex; i++)
        //        {
        //            if (equipped[i] != null)
        //            {
        //                return true;
        //            }
        //        }

        //        return backpackIterator.MoveNext();
        //    }

        //    public override Item Next()
        //    {

        //        while (index < backpackIndex)
        //        {
        //            Item item = equipped[index++];
        //            if (item != null)
        //            {
        //                return item;
        //            }
        //        }

        //        return backpackIterator.Current;
        //    }

        //    public override void remove()
        //    {
        //        switch (index)
        //        {
        //            case 0:
        //                //JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class Instance members within a nested class:
        //                equipped[0] = Weapon = null;
        //                break;
        //            case 1:
        //                //JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class Instance members within a nested class:
        //                equipped[1] = Armor = null;
        //                break;
        //            case 2:
        //                //JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class Instance members within a nested class:
        //                equipped[2] = Ring1 = null;
        //                break;
        //            case 3:
        //                //JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class Instance members within a nested class:
        //                equipped[3] = Ring2 = null;
        //                break;
        //            default:
        //                backpackIterator.Remove();
        //                break;
        //        }
        //    }

        //    public Item Current
        //    {
        //        Get { throw new System.NotImplementedException(); }
        //    }

        //    public void Dispose()
        //    {
        //        throw new System.NotImplementedException();
        //    }

        //    object IEnumerator.Current
        //    {
        //        Get { throw new System.NotImplementedException(); }
        //    }

        //    public bool MoveNext()
        //    {
        //        throw new System.NotImplementedException();
        //    }

        //    public void Reset()
        //    {
        //        throw new System.NotImplementedException();
        //    }
        //}
        //public IEnumerator<Item> GetEnumerator()
        //{
        //    return new ItemEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return new ItemEnumerator();
        //}
    }

    public class ItemEnumerator : IEnumerator<Item>
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public Item Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}