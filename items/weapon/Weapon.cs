using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.items.weapon.missiles;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using System;
using Fire = sharpdungeon.actors.blobs.Fire;

namespace sharpdungeon.items.weapon
{
    public class Weapon : KindOfWeapon
    {
        private const string TxtIdentify = "You are now familiar enough with your {0} to identify it. It is {1}.";
        private const string TxtIncompatible = "Interaction of different types of magic has negated the enchantment on this weapon!";
        private const string TxtToString = "{0} :{1}";

        public int Str = 10;
        public float Acu = 1;
        public float Dly = 1f;

        public enum Imbue
        {
            None,
            Speed,
            Accuracy
        }
        public Imbue imbue = Imbue.None;

        private int _hitsToKnow = 20;

        protected internal Enchantment enchantment;

        public override void Proc(Character attacker, Character defender, int damage)
        {
            if (enchantment != null)
                enchantment.Proc(this, attacker, defender, damage);

            if (levelKnown)
                return;

            if (--_hitsToKnow > 0)
                return;

            levelKnown = true;
            GLog.Information(TxtIdentify, Name, ToString());
            Badge.ValidateItemLevelAquired(this);
        }

        private const string ENCHANTMENT = "enchantment";
        private const string IMBUE = "imbue";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(ENCHANTMENT, enchantment);
            bundle.Put(IMBUE, imbue);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            enchantment = (Enchantment)bundle.Get(ENCHANTMENT);
            imbue = bundle.GetEnum<Imbue>(IMBUE);
        }

        public override float AcuracyFactor(Hero hero)
        {
            var encumbrance = Str - hero.STR;

            if (!(this is MissileWeapon))
                return (encumbrance > 0 ? (float)(Acu / Math.Pow(1.5, encumbrance)) : Acu) * (imbue == Imbue.Accuracy ? 1.5f : 1.0f);

            switch (hero.heroClass.Ordinal())
            {
                case HeroClassType.Warrior:
                    encumbrance += 3;
                    break;
                case HeroClassType.Huntress:
                    encumbrance -= 2;
                    break;
            }

            return (encumbrance > 0 ? (float)(Acu / Math.Pow(1.5, encumbrance)) : Acu) * (imbue == Imbue.Accuracy ? 1.5f : 1.0f);
        }

        public override float SpeedFactor(Hero hero)
        {
            var encumrance = Str - hero.STR;
            if (this is MissileWeapon && hero.heroClass == HeroClass.Huntress)
                encumrance -= 2;

            return (encumrance > 0 ? (float)(Dly * Math.Pow(1.2, encumrance)) : Dly) * (imbue == Imbue.Speed ? 0.6f : 1.0f);
        }

        public override int DamageRoll(Hero hero)
        {
            var damage = base.DamageRoll(hero);

            if ((hero.RangedWeapon != null) != (hero.heroClass == HeroClass.Huntress))
                return damage;

            var exStr = hero.STR - Str;
            if (exStr > 0)
                damage += pdsharp.utils.Random.IntRange(0, exStr);

            return damage;
        }

        public virtual Item Upgrade(bool enchant)
        {
            if (enchantment != null)
            {
                if (enchant || pdsharp.utils.Random.Int(level) <= 0)
                    return base.Upgrade();

                GLog.Warning(TxtIncompatible);
                Enchant(null);
            }
            else
            {
                if (enchant)
                    Enchant(Enchantment.Random());
            }

            return base.Upgrade();
        }

        public override string ToString()
        {
            return levelKnown ? Utils.Format(TxtToString, base.ToString(), Str) : base.ToString();
        }

        public override string Name
        {
            get { return enchantment == null ? base.Name : enchantment.Name(base.Name); }
        }

        public override Item Random()
        {
            if (!(pdsharp.utils.Random.Float() < 0.4))
                return this;

            var n = 1;
            if (pdsharp.utils.Random.Int(3) == 0)
            {
                n++;
                if (pdsharp.utils.Random.Int(3) == 0)
                    n++;
            }

            if (pdsharp.utils.Random.Int(2) == 0)
                Upgrade(true);
            else
            {
                Degrade(n);
                cursed = true;
            }

            return this;
        }

        public virtual Weapon Enchant(Enchantment ench)
        {
            enchantment = ench;
            return this;
        }

        public virtual bool IsEnchanted
        {
            get
            {
                return enchantment != null;
            }
        }

        public override ItemSprite.Glowing Glowing()
        {
            return enchantment != null ? enchantment.Glowing() : null;
        }

        public abstract class Enchantment : Bundlable
        {
            private static readonly Type[] Enchants = { typeof(enchantments.Fire), typeof(Poison), typeof(Death), typeof(Paralysis), typeof(Leech), typeof(Slow), typeof(Swing), typeof(Piercing), typeof(Instability), typeof(Horror), typeof(Luck) };
            private static readonly float[] Chances = { 10, 10, 1, 2, 1, 2, 3, 3, 3, 2, 2 };

            public abstract bool Proc(Weapon weapon, Character attacker, Character defender, int damage);

            public virtual string Name(string weaponName)
            {
                return weaponName;
            }

            public void RestoreFromBundle(Bundle bundle)
            {
            }

            public void StoreInBundle(Bundle bundle)
            {
            }

            public virtual ItemSprite.Glowing Glowing()
            {
                return ItemSprite.Glowing.White;
            }

            public static Enchantment Random()
            {
                try
                {
                    return (Enchantment)Activator.CreateInstance(Enchants[pdsharp.utils.Random.Chances(Chances)]);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}