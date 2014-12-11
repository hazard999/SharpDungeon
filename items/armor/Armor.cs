using System;
using System.Collections.Generic;
using System.Text;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.actors;

namespace sharpdungeon.items.armor
{
    public class Armor : EquipableItem
    {
        private const string TxtEquipCursed = "your {0} constricts around you painfully";

        private const string TxtIdentify = "you are now familiar enough with your {0} to identify it. It is {1}.";

        private const string TxtToString = "{0} :{1}";

        private const string TxtIncompatible =
            "Interaction of different types of magic has erased the glyph on this armor!";

        public int Tier;

        public int Str;
        public int Dr;

        private int _hitsToKnow = 10;

        public Glyph glyph;

        public Armor(int tier)
        {
            Tier = tier;

            Str = TypicalStr();
            Dr = TypicalDr();
        }

        private const string GLYPH = "glyph";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(GLYPH, glyph);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            glyph = (Glyph)bundle.Get(GLYPH);
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(IsEquipped(hero) ? AcUnequip : AcEquip);
            return actions;
        }

        public override bool DoEquip(Hero hero)
        {
            Detach(hero.Belongings.Backpack);

            if (hero.Belongings.Armor == null || hero.Belongings.Armor.DoUnequip(hero, true, false))
            {
                hero.Belongings.Armor = this;

                cursedKnown = true;
                if (cursed)
                {
                    EquipCursed(hero);
                    GLog.Negative(TxtEquipCursed, ToString());
                }

                ((HeroSprite)hero.Sprite).UpdateArmor();

                hero.SpendAndNext(2 * TimeToEquip(hero));
                return true;

            }

            Collect(hero.Belongings.Backpack);
            return false;
        }

        protected virtual float TimeToEquip(Hero hero)
        {
            return hero.Speed();
        }

        public override bool DoUnequip(Hero hero, bool collect, bool single)
        {
            if (!base.DoUnequip(hero, collect, single))
                return false;

            hero.Belongings.Armor = null;
            ((HeroSprite)hero.Sprite).UpdateArmor();

            return true;
        }

        public override bool IsEquipped(Hero hero)
        {
            return hero.Belongings.Armor == this;
        }

        public override Item Upgrade()
        {
            return Upgrade(false);
        }

        public virtual Item Upgrade(bool inscribe)
        {
            if (glyph != null)
            {
                if (!inscribe && pdsharp.utils.Random.Int(level) > 0)
                {
                    GLog.Warning(TxtIncompatible);
                    Inscribe(null);
                }
            }
            else
            {
                if (inscribe)
                    Inscribe(Glyph.Random());
            }

            Dr += Tier;
            Str--;

            return base.Upgrade();
        }

        public virtual Item SafeUpgrade()
        {
            return Upgrade(glyph != null);
        }

        public override Item Degrade()
        {
            Dr -= Tier;
            Str++;

            return base.Degrade();
        }

        public virtual int Proc(Character attacker, Character defender, int damage)
        {
            if (glyph != null)
                damage = glyph.Proc(this, attacker, defender, damage);

            if (levelKnown)
                return damage;

            if (--_hitsToKnow > 0)
                return damage;

            levelKnown = true;
            GLog.Warning(TxtIdentify, Name, ToString());
            Badge.ValidateItemLevelAquired(this);

            return damage;
        }

        public override string ToString()
        {
            return levelKnown ? Utils.Format(TxtToString, base.ToString(), Str) : base.ToString();
        }

        public override string Name
        {
            get { return glyph == null ? base.Name : glyph.Name(base.Name); }
        }

        public override string Info()
        {
            var name = Name;
            var info = new StringBuilder(Desc());

            if (levelKnown)
            {
                info.Append("\\Negative\nThis " + name + " provides damage absorption up to " + "" + Math.Max(Dr, 0) +
                            " points per DoAttack. ");

                if (Str > Dungeon.Hero.STR)
                    if (IsEquipped(Dungeon.Hero))
                        info.Append("\\Negative\nBecause of your inadequate strength your " +
                                    "movement speed and defense skill is decreased. ");
                    else
                        info.Append("\\Negative\nBecause of your inadequate strength wearing this armor " +
                                    "will decrease your movement speed and defense skill. ");
            }
            else
            {
                info.Append("\\Negative\nTypical " + name + " provides damage absorption up to " + TypicalDr() +
                            " points per DoAttack " + " and requires " + TypicalStr() + " points of strength. ");
                if (TypicalStr() > Dungeon.Hero.STR)
                    info.Append("Probably this armor is too heavy for you. ");
            }

            if (glyph != null)
                info.Append("It is inscribed.");

            if (IsEquipped(Dungeon.Hero))
                info.Append("\\Negative\nYou are wearing the " + name +
                            (cursed ? ", and because it is cursed, you are powerless to Remove it." : "."));
            else
            {
                if (cursedKnown && cursed)
                    info.Append("\\Negative\nYou can feel a malevolent magic lurking within the " + name + ".");
            }

            return info.ToString();
        }

        public override Item Random()
        {
            if (pdsharp.utils.Random.Float() < 0.4)
            {
                var n = 1;
                if (pdsharp.utils.Random.Int(3) == 0)
                {
                    n++;
                    if (pdsharp.utils.Random.Int(3) == 0)
                        n++;
                }

                if (pdsharp.utils.Random.Int(2) == 0)
                    Upgrade(n);
                else
                {
                    Degrade(n);
                    cursed = true;
                }
            }

            if (pdsharp.utils.Random.Int(10) == 0)
                Inscribe(Glyph.Random());

            return this;
        }

        public virtual int TypicalStr()
        {
            return 7 + Tier * 2;
        }

        public virtual int TypicalDr()
        {
            return Tier * 2;
        }

        public override int Price()
        {
            var price = 10 * (1 << (Tier - 1));
            if (glyph != null)
                price = (int) (price*1.5);
            if (cursed && cursedKnown)
                price /= 2;
            if (levelKnown)
            {
                if (level > 0)
                {
                    price *= (level + 1);
                }
                else if (level < 0)
                {
                    price /= (1 - level);
                }
            }
            if (price < 1)
            {
                price = 1;
            }
            return price;
        }

        public virtual Armor Inscribe(Glyph glyph)
        {
            if (glyph != null && this.glyph == null)
                Dr += Tier;
            else if (glyph == null && this.glyph != null)
                Dr -= Tier;

            this.glyph = glyph;

            return this;
        }

        public virtual bool IsInscribed
        {
            get { return glyph != null; }
        }

        public override ItemSprite.Glowing Glowing()
        {
            return glyph != null ? glyph.Glowing() : null;
        }
    }
}