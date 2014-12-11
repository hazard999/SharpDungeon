using System.Collections.Generic;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.utils;

namespace sharpdungeon.items.armor
{
    public abstract class ClassArmor : Armor
    {
        private const string TxtLowHealth = "Your health is too low!";
        private const string TxtNotEquipped = "You need to be wearing this armor to use its special power!";

        protected ClassArmor()
            : base(6)
        {
            levelKnown = true;
            cursedKnown = true;
            DefaultAction = Special();
        }

        public static ClassArmor Upgrade(Hero owner, Armor armor)
        {
            ClassArmor classArmor = null;

            if (owner.heroClass == HeroClass.Warrior)
                classArmor = new WarriorArmor();
            if (owner.heroClass == HeroClass.Rogue)
                classArmor = new RogueArmor();
            if (owner.heroClass == HeroClass.Mage)
                classArmor = new MageArmor();
            if (owner.heroClass == HeroClass.Huntress)
                classArmor = new HuntressArmor();

            classArmor.Str = armor.Str;
            classArmor.Dr = armor.Dr;

            classArmor.Inscribe(armor.glyph);

            return classArmor;
        }

        private const string ArmorStr = "STR";
        private const string ArmorDr = "DR";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(ArmorStr, Str);
            bundle.Put(ArmorDr, Dr);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            Str = bundle.GetInt(ArmorStr);
            Dr = bundle.GetInt(ArmorDr);
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            
            if (hero.HP >= 3 && IsEquipped(hero))
                actions.Add(Special());

            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action == Special())
            {
                if (hero.HP < 3)
                    GLog.Warning(TxtLowHealth);
                else if (!IsEquipped(hero))
                    GLog.Warning(TxtNotEquipped);
                else
                {
                    CurUser = hero;
                    DoSpecial();
                }
            }
            else
                base.Execute(hero, action);
        }

        public abstract string Special();
        public abstract void DoSpecial();

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

        public override int Price()
        {
            return 0;
        }

        public override string Desc()
        {
            return "The thing looks awesome!";
        }
    }
}