using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.items.bags;
using sharpdungeon.items.weapon.missiles;
using sharpdungeon.mechanics;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.items
{
    public class Item : Bundlable, IComparable<Item>
    {
        public Item()
        {
            Thrower = new ItemCellSelector(this);
        }
        private const string TxtPackFull = "Your pack is too full for the {0}";

        private const string TxtToString = "{0}";
        private const string TxtToStringX = "{0} x{1}";
        private const string TxtToStringLvl = "{0}{1}";
        private const string TxtToStringLvlX = "{0}{1} x{2}";

        protected internal const float TimeToThrow = 1.0f;
        protected internal const float TimeToPickUp = 1.0f;
        protected internal const float TimeToDrop = 0.5f;

        public const string AcDrop = "DROP";
        public const string AcThrow = "THROW";

        public string DefaultAction;

        protected string name = "smth";
        protected internal int image = 0;

        public bool Stackable = false;
        protected internal int quantity = 1;

        public int level = 0;
        public bool levelKnown = false;

        public bool cursed;
        public bool cursedKnown;

        // Unique items persist through revival
        public bool unique = false;

        public virtual List<string> Actions(Hero hero)
        {
            var actions = new List<string>();
            actions.Add(AcDrop);
            actions.Add(AcThrow);
            return actions;
        }

        public virtual bool DoPickUp(Hero hero)
        {
            if (!Collect(hero.Belongings.Backpack)) return false;
            GameScene.PickUp(this);
            Sample.Instance.Play(Assets.SND_ITEM);
            hero.SpendAndNext(TimeToPickUp);
            return true;
        }

        public virtual void DoDrop(Hero hero)
        {
            hero.SpendAndNext(TimeToDrop);
            Dungeon.Level.Drop(DetachAll(hero.Belongings.Backpack), hero.pos).Sprite.Drop(hero.pos);
        }

        public virtual void DoThrow(Hero hero)
        {
            GameScene.SelectCell(Thrower);
        }

        public virtual void Execute(Hero hero, string action)
        {

            CurUser = hero;
            curItem = this;

            if (action.Equals(AcDrop))
            {

                DoDrop(hero);

            }
            else if (action.Equals(AcThrow))
            {

                DoThrow(hero);

            }
        }

        public virtual void Execute(Hero hero)
        {
            Execute(hero, DefaultAction);
        }

        protected virtual void OnThrow(int cell)
        {
            var heap = Dungeon.Level.Drop(this, cell);
            if (!heap.IsEmpty)
                heap.Sprite.Drop(cell);
        }

        public virtual bool Collect(Bag container)
        {
            var items = container.Items;

            if (items.Contains(this))
                return true;

            foreach (var item in items.Where(item => item is Bag && ((Bag)item).Grab(this)))
                return Collect((Bag)item);

            if (Stackable)
            {
                var c = GetType();
                foreach (var item in items.Where(item => item.GetType() == c))
                {
                    item.quantity += quantity;
                    item.UpdateQuickslot();
                    return true;
                }
            }

            if (items.Count < container.Size)
            {
                if (Dungeon.Hero != null && Dungeon.Hero.IsAlive)
                    Badge.ValidateItemLevelAquired(this);

                items.Add(this);
                QuickSlot.Refresh();
                items.Sort();

                return true;
            }

            GLog.Negative(TxtPackFull, Name);
            return false;
        }

        public bool Collect()
        {
            return Collect(Dungeon.Hero.Belongings.Backpack);
        }

        public Item Detach(Bag container)
        {
            if (quantity <= 0)
                return null;

            if (quantity == 1)
                return DetachAll(container);

            quantity--;
            UpdateQuickslot();

            try
            {
                var detached = (Item)Activator.CreateInstance(GetType());
                detached.OnDetach();
                return detached;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Item DetachAll(Bag container)
        {
            foreach (var item in container.Items)
            {
                if (item == this)
                {
                    container.Items.Remove(this);
                    item.OnDetach();
                    QuickSlot.Refresh();
                    return this;
                }

                var bag1 = item as Bag;
                if (bag1 == null)
                    continue;

                var bag = bag1;
                if (bag.Contains(this))
                    return DetachAll(bag);
            }

            return this;
        }

        protected virtual void OnDetach()
        {
        }

        public virtual Item Upgrade()
        {
            cursed = false;
            cursedKnown = true;
            level++;

            return this;
        }

        public Item Upgrade(int n)
        {
            for (var i = 0; i < n; i++)
                Upgrade();

            return this;
        }

        public virtual Item Degrade()
        {
            level--;

            return this;
        }

        public Item Degrade(int n)
        {
            for (var i = 0; i < n; i++)
                Degrade();

            return this;
        }

        public int VisiblyUpgraded()
        {
            return levelKnown ? level : 0;
        }

        public bool VisiblyCursed()
        {
            return cursed && cursedKnown;
        }

        public virtual bool Upgradable
        {
            get { return true; }
        }

        public virtual bool Identified
        {
            get { return levelKnown && cursedKnown; }
        }

        public virtual bool IsEquipped(Hero hero)
        {
            return false;
        }

        public virtual Item Identify()
        {
            levelKnown = true;
            cursedKnown = true;

            return this;
        }

        public static void Evoke(Hero hero)
        {
            hero.Sprite.Emitter().Burst(Speck.Factory(Speck.EVOKE), 5);
        }

        //public void Evoke(Hero hero)
        //{
        //    hero.Sprite.Emitter().Burst(Speck.Factory(Speck.EVOKE), 5);
        //}

        public int CompareTo(Item other)
        {
            return Generator.Category.Order(this) - Generator.Category.Order(other);
        }

        public override string ToString()
        {
            if (levelKnown && level != 0)
            {
                if (Quantity() > 1)
                    return Utils.Format(TxtToStringLvlX, Name, level, quantity);

                return Utils.Format(TxtToStringLvl, Name, level);
            }

            if (quantity > 1)
                return Utils.Format(TxtToStringX, Name, quantity);

            return Utils.Format(TxtToString, Name);
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string TrueName()
        {
            return name;
        }

        public int Image
        {
            get { return image; }
        }

        public virtual ItemSprite.Glowing Glowing()
        {
            return null;
        }

        public virtual string Info()
        {
            return Desc();
        }

        public virtual string Desc()
        {
            return "";
        }

        public int Quantity()
        {
            return quantity;
        }

        public void Quantity(int value)
        {
            quantity = value;
        }

        public virtual int Price()
        {
            return 0;
        }

        public static Item Virtual(Item cl)
        {
            try
            {
                var item = (Item)Activator.CreateInstance(cl.GetType());
                item.quantity = 0;
                return item;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual Item Random()
        {
            return this;
        }

        public virtual string Status()
        {
            return Quantity() != 1 ? Quantity().ToString(CultureInfo.InvariantCulture) : null;
        }

        public void UpdateQuickslot()
        {
            //TODO: CHECK UpdateQuickslot
            if (Stackable && Dungeon.Quickslot == this)
                QuickSlot.Refresh();
        }

        private const string QUANTITY = "quantity";
        private const string LEVEL = "Level";
        private const string LEVEL_KNOWN = "levelKnown";
        private const string CURSED = "cursed";
        private const string CURSED_KNOWN = "cursedKnown";
        private const string QUICKSLOT = "quickslot";

        public virtual void StoreInBundle(Bundle bundle)
        {
            bundle.Put(QUANTITY, quantity);
            bundle.Put(LEVEL, level);
            bundle.Put(LEVEL_KNOWN, levelKnown);
            bundle.Put(CURSED, cursed);
            bundle.Put(CURSED_KNOWN, cursedKnown);

            if (this == Dungeon.Quickslot)
                bundle.Put(QUICKSLOT, true);
        }

        public virtual void RestoreFromBundle(Bundle bundle)
        {
            quantity = bundle.GetInt(QUANTITY);
            levelKnown = bundle.GetBoolean(LEVEL_KNOWN);
            cursedKnown = bundle.GetBoolean(CURSED_KNOWN);

            var level = bundle.GetInt(LEVEL);
            if (level > 0)
                Upgrade(level);
            else if (level < 0)
                Degrade(-level);

            cursed = bundle.GetBoolean(CURSED);

            if (bundle.GetBoolean(QUICKSLOT))
                Dungeon.Quickslot = this;
        }

        public virtual void Cast(Hero user, int dst)
        {
            var cell = Ballistica.Cast(user.pos, dst, false, true);
            user.Sprite.DoZap(cell);
            user.Busy();

            var enemy = Actor.FindChar(cell);
            QuickSlot.Target(this, enemy);

            var delay = TimeToThrow;
            if (this is MissileWeapon)
            {
                // FIXME
                delay *= ((MissileWeapon)this).SpeedFactor(user);
                if (enemy != null && enemy.Buff<SnipersMark>() != null)
                    delay *= 0.5f;
            }

            float finalDelay = delay;

            //((MissileSprite)user.sprite.parent.Recycle(typeof(MissileSprite))).Reset(user.pos, cell, this, new ICallback() { public void call() { Item.detach(user.belongings.backpack).onThrow(cell); user.spendAndNext(finalDelay); } });
        }

        public Hero CurUser;
        protected static Item curItem = null;
        protected static CellSelector.Listener Thrower;
    }

    public class ItemCellSelector : CellSelector.Listener
    {
        private readonly Item _item;

        public ItemCellSelector(Item item)
        {
            _item = item;
        }

        public void OnSelect(int? target)
        {
            if (target != null)
                _item.Cast(_item.CurUser, target.Value);
            ;
        }

        public string Prompt()
        {
            return "Choose direction of throw";
        }
    }
}