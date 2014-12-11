using System;
using System.Linq;
using System.Collections.Generic;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items.food;
using sharpdungeon.items.scrolls;
using sharpdungeon.plants;
using sharpdungeon.sprites;

namespace sharpdungeon.items
{
    public class Heap : Bundlable
    {
        private const int SeedsToPotion = 3;

        public enum Type
        {
            Heap,
            ForSale,
            Chest,
            LockedChest,
            CrystalChest,
            Tomb,
            Skeleton
        }

        public Type HeapType = Type.Heap;

        public int Pos;

        public ItemSprite Sprite;

        protected internal List<Item> Items = new List<Item>();

        public virtual int Image()
        {
            switch (HeapType)
            {
                case Type.Heap:
                case Type.ForSale:
                    return Size() > 0 ? Items.First().Image : 0;
                case Type.Chest:
                    return ItemSpriteSheet.CHEST;
                case Type.LockedChest:
                    return ItemSpriteSheet.LOCKED_CHEST;
                case Type.CrystalChest:
                    return ItemSpriteSheet.CRYSTAL_CHEST;
                case Type.Tomb:
                    return ItemSpriteSheet.TOMB;
                case Type.Skeleton:
                    return ItemSpriteSheet.BONES;
                default:
                    return 0;
            }
        }

        public virtual ItemSprite.Glowing Glowing()
        {
            return (HeapType == Type.Heap || HeapType == Type.ForSale) && Items.Count > 0 ? Items.First().Glowing() : null;
        }

        public virtual void Open(Hero hero)
        {
            switch (HeapType)
            {
                case Type.Tomb:
                    Wraith.SpawnAround(hero.pos);
                    break;
                case Type.Skeleton:
                    CellEmitter.Center(Pos).Start(Speck.Factory(Speck.RATTLE), 0.1f, 3);
                    if (Items.Any(item => item.cursed))
                    {
                        if (Wraith.SpawnAt(Pos) == null)
                        {
                            hero.Sprite.Emitter().Burst(ShadowParticle.Curse, 6);
                            hero.Damage(hero.HP / 2, this);
                        }
                        Sample.Instance.Play(Assets.SND_CURSED);
                    }
                    break;
            }

            HeapType = Type.Heap;
            Sprite.Link();
            Sprite.Drop();
        }

        public virtual int Size()
        {
            return Items.Count;
        }

        public virtual Item PickUp()
        {
            var item = Items[0];
            Items.RemoveAt(0);
            if (Items.Count == 0)
                Destroy();
            else
                if (Sprite != null)
                    Sprite.View(Image(), Glowing());

            return item;
        }

        public virtual Item Peek()
        {
            return Items.First();
        }

        public virtual void Drop(Item item)
        {
            if (item.Stackable)
            {
                var c = item.GetType();
                foreach (var i in Items.Where(i => i.GetType() == c))
                {
                    i.quantity += item.quantity;
                    item = i;
                    break;
                }
                
                Items.Remove(item);
            }

            if (item is Dewdrop)
                Items.Add(item);
            else
                Items.Insert(0, item);

            if (Sprite != null)
                Sprite.View(Image(), Glowing());
        }

        public virtual void Replace(Item a, Item b)
        {
            var index = Items.IndexOf(a);
            if (index == -1)
                return;
            Items.RemoveAt(index);
            Items.Insert(index, b);
        }

        public virtual void Burn()
        {
            if (HeapType != Type.Heap)
                return;

            var burnt = false;
            var evaporated = false;

            foreach (var item in Items)
            {
                if (item is Scroll)
                {
                    Items.Remove(item);
                    burnt = true;
                }
                else if (item is Dewdrop)
                {
                    Items.Remove(item);
                    evaporated = true;
                }
                else if (item is MysteryMeat)
                {
                    Replace(item, ChargrilledMeat.Cook((MysteryMeat)item));
                    burnt = true;
                }
            }

            if (!burnt && !evaporated) 
                return;

            if (Dungeon.Visible[Pos])
            {
                if (burnt)
                    BurnFx(Pos);
                else
                    EvaporateFx(Pos);
            }

            if (IsEmpty)
                Destroy();
            else
                if (Sprite != null)
                    Sprite.View(Image(), Glowing());
        }

        public virtual void Freeze()
        {
            if (HeapType != Type.Heap)
                return;

            var frozen = false;
            
            foreach (var item in Items.OfType<MysteryMeat>())
            {
                Replace(item, FrozenCarpaccio.Cook(item));
                frozen = true;
            }

            if (!frozen)
                return;

            if (IsEmpty)
                Destroy();
            else
                if (Sprite != null)
                    Sprite.View(Image(), Glowing());
        }

        public virtual Item Transmute()
        {
            CellEmitter.Get(Pos).Burst(Speck.Factory(Speck.BUBBLE), 3);
            Splash.At(Pos, new Android.Graphics.Color(0xFFFFFF), 3);

            var chances = new float[Items.Count];
            var count = 0;

            var index = 0;
            foreach (var item in Items)
            {
                if (item is Plant.Seed)
                {
                    count += item.quantity;
                    chances[index++] = item.quantity;
                }
                else
                {
                    count = 0;
                    break;
                }
            }

            if (count < SeedsToPotion) 
                return null;

            CellEmitter.Get(Pos).Burst(Speck.Factory(Speck.WOOL), 6);
            Sample.Instance.Play(Assets.SND_PUFF);

            if (pdsharp.utils.Random.Int(count) == 0)
            {
                CellEmitter.Center(Pos).Burst(Speck.Factory(Speck.EVOKE), 3);

                Destroy();

                Statistics.PotionsCooked++;
                Badge.ValidatePotionsCooked();

                return Generator.Random(Generator.Category.POTION);

            }

            var proto = (Plant.Seed)Items[pdsharp.utils.Random.Chances(chances)];
                
            var itemClass = proto.AlchemyClass;

            Destroy();

            Statistics.PotionsCooked++;
            Badge.ValidatePotionsCooked();

            if (itemClass == null)
                return Generator.Random(Generator.Category.POTION);

            try
            {
                return (Item)Activator.CreateInstance(itemClass);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void BurnFx(int pos)
        {
            CellEmitter.Get(pos).Burst(ElmoParticle.Factory, 6);
            Sample.Instance.Play(Assets.SND_BURNING);
        }

        public static void EvaporateFx(int pos)
        {
            CellEmitter.Get(pos).Burst(Speck.Factory(Speck.STEAM), 5);
        }

        public virtual bool IsEmpty
        {
            get
            {
                return Items == null || Items.Count == 0;
            }
        }

        public virtual void Destroy()
        {
            Dungeon.Level.heaps.Remove(Pos);
            
            if (Sprite != null)
                Sprite.Kill();

            Items.Clear();
            Items = null;
        }

        private const string POS = "pos";
        private const string TYPE = "type";
        private const string ITEMS = "items";

        public void RestoreFromBundle(Bundle bundle)
        {
            Pos = bundle.GetInt(POS);
            HeapType = (Type)Enum.Parse(typeof(Type), bundle.GetString(TYPE));

            Items = new List<Item>(bundle.GetCollection(ITEMS).OfType<Item>());
        }

        public void StoreInBundle(Bundle bundle)
        {
            bundle.Put(POS, Pos);
            bundle.Put(TYPE, HeapType.ToString());
            bundle.Put(ITEMS, Items);
        }

    }

}