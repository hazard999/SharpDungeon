using System.Collections.Generic;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.scenes;
using sharpdungeon.windows;

namespace sharpdungeon.items.bags
{
    public class Bag : Item
    {
        public const string AcOpen = "OPEN";

        public Bag()
        {
            image = 11;
            DefaultAction = AcOpen;
        }

        public Character Owner;

        public List<Item> Items = new List<Item>();

        public int Size = 1;

        public override List<string> Actions(Hero hero)
        {
            List<string> actions = base.Actions(hero);
            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action.Equals(AcOpen))
                GameScene.Show(new WndBag(this, null, WndBag.Mode.ALL, null));
            else
                base.Execute(hero, action);
        }

        public override bool Collect(Bag container)
        {
            if (base.Collect(container))
            {

                Owner = container.Owner;

                foreach (Item item in container.Items.ToArray())
                {
                    if (Grab(item))
                    {
                        item.DetachAll(container);
                        item.Collect(this);
                    }
                }

                Badge.ValidateAllBagsBought(this);

                return true;
            }

            return false;
        }

        protected override void OnDetach()
        {
            Owner = null;
        }

        public override bool Upgradable
        {
            get
            {
                return false;
            }
        }

        public override bool Identified
        {
            get
            {
                return true;
            }
        }

        public virtual void Clear()
        {
            Items.Clear();
        }

        private const string ITEMS = "inventory";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(ITEMS, Items);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            foreach (var item in bundle.GetCollection(ITEMS))
            {
                ((Item)item).Collect(this);
            }
        }

        public virtual bool Contains(Item item)
        {
            foreach (var i in Items)
            {
                if (i == item)
                {
                    return true;
                }

                if (i is Bag && ((Bag)i).Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool Grab(Item item)
        {
            return false;
        }

        //public override IEnumerator<Item> iterator()
        //{

        //}

        //private class ItemIterator : IEnumerator<Item>
        //{

        //    private int index = 0;
        //    private IEnumerator<Item> nested = null;

        //    public override bool hasNext()
        //    {
        //        if (nested != null)
        //        {
        //            return nested.MoveNext() || index < items.size();
        //        }
        //        else
        //        {
        //            return index < items.size();
        //        }
        //    }

        //    public override Item next()
        //    {
        //        if (nested != null && nested.MoveNext())
        //        {

        //            return nested.Current;

        //        }
        //        else
        //        {

        //            nested = null;

        //            Item item = items.Get(index++);
        //            if (item is Bag)
        //            {
        //                nested = ((Bag)item).GetEnumerator();
        //            }

        //            return item;
        //        }
        //    }

        //    public override void Remove()
        //    {
        //        if (nested != null)
        //        {
        //            nested.Remove();
        //        }
        //        else
        //        {
        //            items.Remove(index);
        //        }
        //    }

        //    public void Dispose()
        //    {
        //        throw new System.NotImplementedException();
        //    }

        //    public bool MoveNext()
        //    {
        //        throw new System.NotImplementedException();
        //    }

        //    public void Reset()
        //    {
        //        throw new System.NotImplementedException();
        //    }

        //    public Item Current { Get; private set; }

        //    object IEnumerator.Current
        //    {
        //        Get { return Current; }
        //    }
        //}

        //public IEnumerator<Item> GetEnumerator()
        //{
        //    return new ItemIterator();

        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}
    }

}