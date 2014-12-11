using System.Collections.Generic;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using pdsharp.utils;
using sharpdungeon.actors;

namespace sharpdungeon.items.rings
{
    public class Ring : EquipableItem
    {
        private const float TIME_TO_EQUIP = 1f;

        private const string TXT_IDENTIFY = "you are now familiar enough with your {0} to identify it. It is {1}.";

        protected internal Buff _buff;

        private static readonly System.Type[] rings = { 
                                                   typeof(RingOfMending), 
                                                   typeof(RingOfDetection), 
                                                   typeof(RingOfShadows),  
                                                   typeof(RingOfPower),  
                                                   typeof(RingOfHerbalism), 
                                                   typeof(RingOfAccuracy),  
                                                   typeof(RingOfEvasion), 
                                                   typeof(RingOfSatiety),  
                                                   typeof(RingOfHaste), 
                                                   typeof(RingOfHaggler),  
                                                   typeof(RingOfElements),  
                                                   typeof(RingOfThorns)
                                               };

        private static readonly string[] gems = { "diamond", "opal", "garnet", "ruby", "amethyst", "topaz", "onyx", "tourmaline", "emerald", "sapphire", "quartz", "agate" };
        private static readonly int?[] images = { ItemSpriteSheet.RING_DIAMOND, ItemSpriteSheet.RING_OPAL, ItemSpriteSheet.RING_GARNET, ItemSpriteSheet.RING_RUBY, ItemSpriteSheet.RING_AMETHYST, ItemSpriteSheet.RING_TOPAZ, ItemSpriteSheet.RING_ONYX, ItemSpriteSheet.RING_TOURMALINE, ItemSpriteSheet.RING_EMERALD, ItemSpriteSheet.RING_SAPPHIRE, ItemSpriteSheet.RING_QUARTZ, ItemSpriteSheet.RING_AGATE };

        private static ItemStatusHandler _handler;

        private string gem;

        private int ticksToKnow = 200;


        public static void InitGems()
        {
            _handler = new ItemStatusHandler(rings, gems, images);
        }

        public static void Save(Bundle bundle)
        {
            _handler.Save(bundle);
        }

        public static void Restore(Bundle bundle)
        {
            _handler = new ItemStatusHandler(rings, gems, images, bundle);
        }

        public Ring()
        {
            SyncGem();
        }

        public virtual void SyncGem()
        {
            if (_handler == null)
                return;

            image = _handler.Image(GetType()) ?? 0;
            gem = _handler.Label(GetType());
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(IsEquipped(hero) ? AcUnequip : AcEquip);
            return actions;
        }

        public override bool DoEquip(Hero hero)
        {
            if (hero.Belongings.Ring1 != null && hero.Belongings.Ring2 != null)
            {
                GLog.Warning("you can only wear 2 rings at a time");
                return false;

            }
            if (hero.Belongings.Ring1 == null)
                hero.Belongings.Ring1 = this;
            else
                hero.Belongings.Ring2 = this;

            Detach(hero.Belongings.Backpack);

            Activate(hero);

            cursedKnown = true;
            if (cursed)
            {
                EquipCursed(hero);
                GLog.Negative("your " + this + " tightens around your finger painfully");
            }

            hero.SpendAndNext(TIME_TO_EQUIP);
            return true;
        }

        public virtual void Activate(Character ch)
        {
            var buff = Buff();
            buff.AttachTo(ch);
        }

        public override bool DoUnequip(Hero hero, bool collect, bool single)
        {
            if (!base.DoUnequip(hero, collect, single)) 
                return false;

            if (hero.Belongings.Ring1 == this)
                hero.Belongings.Ring1 = null;
            else
                hero.Belongings.Ring2 = null;

            hero.Remove(_buff);
            _buff = null;

            return true;
        }

        public override bool IsEquipped(Hero hero)
        {
            return hero.Belongings.Ring1 == this || hero.Belongings.Ring2 == this;
        }

        public override Item Upgrade()
        {
            base.Upgrade();

            if (_buff == null) 
                return this;

            var owner = _buff.Target;
            
            _buff.Detach();
            
            if ((_buff = Buff()) != null)
                _buff.AttachTo(owner);

            return this;
        }

        public virtual bool IsKnown
        {
            get
            {
                return _handler.IsKnown(GetType());
            }
        }

        protected internal virtual void SetKnown()
        {
            if (!IsKnown)
                _handler.Know(GetType());

            Badge.ValidateAllRingsIdentified();
        }

        public override string Name
        {
            get { return IsKnown ? name : gem + " ring"; }
        }

        public override string Desc()
        {
            return "This metal band is adorned with a large " + gem + " gem " + "that glitters in the darkness. Who knows what effect it has when worn?";
        }

        public override string Info()
        {
            if (IsEquipped(Dungeon.Hero))
                return Desc() + "\\Negative\\Negative" + "The " + Name + " is on your finger" + (cursed ? ", and because it is cursed, you are powerless to Remove it." : ".");

            if (cursed && cursedKnown)
                return Desc() + "\\Negative\nYou can feel a malevolent magic lurking within the " + Name + ".";

            return Desc();
        }

        public override bool Identified
        {
            get
            {
                return base.Identified && IsKnown;
            }
        }

        public override Item Identify()
        {
            SetKnown();
            return base.Identify();
        }

        public override Item Random()
        {
            level = pdsharp.utils.Random.Int(1, 3);
            if (pdsharp.utils.Random.Float() < 0.3f)
            {
                level = -level;
                cursed = true;
            }
            return this;
        }

        public static bool AllKnown()
        {
            return _handler.Known().Count == rings.Length - 2;
        }

        public override int Price()
        {
            var price = 80;
            if (cursed && cursedKnown)
                price /= 2;
            if (levelKnown)
            {
                if (level > 0)
                    price *= (level + 1);
                else if (level < 0)
                    price /= (1 - level);
            }
            
            if (price < 1)
                price = 1;
            
            return price;
        }

        protected internal virtual RingBuff Buff()
        {
            return null;
        }

        public class RingBuff : Buff
        {
            private readonly Ring _ring;

            private const string TxtKnown = "This is a {0}";

            public int Level;
            public RingBuff(Ring ring)
            {
                _ring = ring;
                Level = _ring.level;
            }

            public override bool AttachTo(Character target)
            {
                if (!(target is Hero) || ((Hero) target).heroClass != HeroClass.Rogue || _ring.IsKnown)
                    return base.AttachTo(target);
                
                _ring.SetKnown();

                GLog.Information(TxtKnown, _ring.Name);
                Badge.ValidateItemLevelAquired(_ring);

                return base.AttachTo(target);
            }

            protected override bool Act()
            {
                if (!_ring.Identified && --_ring.ticksToKnow <= 0)
                {
                    var gemName = _ring.Name;
                    _ring.Identify();
                    GLog.Warning(TXT_IDENTIFY, gemName, _ring.ToString());
                    Badge.ValidateItemLevelAquired(_ring);
                }

                Spend(Tick);

                return true;
            }
        }
    }
}