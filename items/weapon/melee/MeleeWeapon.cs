using System;
using System.Text;
using sharpdungeon.utils;

namespace sharpdungeon.items.weapon.melee
{
    public class MeleeWeapon : Weapon
    {
        private readonly int _tier;

        public MeleeWeapon(int tier, float acu, float dly)
        {
            _tier = tier;

            Acu = acu;
            Dly = dly;

            Str = TypicalStr();

            Min = min();
            Max = max();
        }

        private int min()
        {
            return _tier;
        }

        private int max()
        {
            return (int)((_tier * _tier - _tier + 10) / Acu * Dly);
        }

        public override Item Upgrade()
        {
            return Upgrade(false);
        }

        public override Item Upgrade(bool enchant)
        {
            Str--;
            Min++;
            Max += _tier;

            return base.Upgrade(enchant);
        }

        public virtual Item SafeUpgrade()
        {
            return Upgrade(enchantment != null);
        }

        public override Item Degrade()
        {
            Str++;
            Min--;
            Max -= _tier;
            return base.Degrade();
        }

        public virtual int TypicalStr()
        {
            return 8 + _tier * 2;
        }

        public override string Info()
        {

            const string p = "\\Negative\\Negative";

            var info = new StringBuilder(Desc());

            var quality = levelKnown && level != 0 ? (level > 0 ? "upgraded" : "degraded") : "";
            info.Append(p);
            info.Append("This " + name + " is " + Utils.Indefinite(quality));
            info.Append(" tier-" + _tier + " melee weapon. ");

            if (levelKnown)
                info.Append("Its average damage is " + (Min + (Max - Min) / 2) + " points per hit. ");
            else
            {
                info.Append("Its typical average damage is " + (min() + (max() - min()) / 2) + " points per hit " + "and usually it requires " + TypicalStr() + " points of strength. ");
                if (TypicalStr() > Dungeon.Hero.STR)
                    info.Append("Probably this weapon is too heavy for you. ");
            }

            if (Math.Abs(Dly - 1f) > 0.001)
            {
                info.Append("This is a rather " + (Dly < 1f ? "fast" : "slow"));

                if (Math.Abs(Acu - 1f) > 0.001)
                {
                    if ((Acu > 1f) == (Dly < 1f))
                        info.Append(" and ");
                    else
                        info.Append(" but ");

                    info.Append(Acu > 1f ? "accurate" : "inaccurate");
                }

                info.Append(" weapon. ");
            }
            else
                if (Math.Abs(Acu - 1f) > 0.001)
                    info.Append("This is a rather " + (Acu > 1f ? "accurate" : "inaccurate") + " weapon. ");

            switch (imbue)
            {
                case Imbue.Speed:
                    info.Append("It was balanced to make it faster. ");
                    break;
                case Imbue.Accuracy:
                    info.Append("It was balanced to make it more accurate. ");
                    break;
                case Imbue.None:
                    break;
            }

            if (enchantment != null)
                info.Append("It is enchanted.");

            if (levelKnown && Dungeon.Hero.Belongings.Backpack.Items.Contains(this))
            {
                if (Str > Dungeon.Hero.STR)
                {
                    info.Append(p);
                    info.Append("Because of your inadequate strength the accuracy and speed " + "of your DoAttack with this " + name + " is decreased.");
                }
                if (Str < Dungeon.Hero.STR)
                {
                    info.Append(p);
                    info.Append("Because of your excess strength the damage " + "of your DoAttack with this " + name + " is increased.");
                }
            }

            if (IsEquipped(Dungeon.Hero))
            {
                info.Append(p);
                info.Append("You hold the " + name + " at the ready" + (cursed ? ", and because it is cursed, you are powerless to let go." : "."));
            }
            else
            {
                if (!cursedKnown || !cursed)
                    return info.ToString();

                info.Append(p);
                info.Append("You can feel a malevolent magic lurking within " + name + ".");
            }

            return info.ToString();
        }

        public override int Price()
        {
            float price = 20 * (1 << (_tier - 1));
            if (enchantment != null)
                price *= 1.5f;

            if (cursed && cursedKnown)
                price /= 2;

            if (levelKnown)
            {
                if (level > 0)
                    price *= (level + 1);
                else
                    if (level < 0)
                        price /= (1 - level);
            }
            
            if (price < 1)
                price = 1;

            return (int)price;
        }

        public override Item Random()
        {
            base.Random();

            if (pdsharp.utils.Random.Int(10 + level) == 0)
                Enchant(Enchantment.Random());

            return this;
        }
    }
}