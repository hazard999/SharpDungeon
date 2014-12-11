using System.Collections.Generic;
using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.items
{
    public class KindOfWeapon : EquipableItem
    {
        private const string TxtEquipCursed = "you wince as your grip involuntarily tightens around your {0}";

        protected internal const float TimeToEquip = 1f;

        public int Min = 0;
        public int Max = 1;

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(IsEquipped(hero) ? AcUnequip : AcEquip);
            return actions;
        }

        public override bool IsEquipped(Hero hero)
        {
            return hero.Belongings.Weapon == this;
        }

        public override bool DoEquip(Hero hero)
        {
            DetachAll(hero.Belongings.Backpack);

            if (hero.Belongings.Weapon == null || hero.Belongings.Weapon.DoUnequip(hero, true))
            {

                hero.Belongings.Weapon = this;
                Activate(hero);

                QuickSlot.Refresh();

                cursedKnown = true;
                if (cursed)
                {
                    EquipCursed(hero);
                    GLog.Negative(TxtEquipCursed, Name);
                }

                hero.SpendAndNext(TimeToEquip);
                return true;

            }
            
            Collect(hero.Belongings.Backpack);
            return false;
        }

        public override bool DoUnequip(Hero hero, bool collect, bool single)
        {
            if (!base.DoUnequip(hero, collect, single)) 
                return false;

            hero.Belongings.Weapon = null;
            return true;
        }

        public virtual void Activate(Hero hero)
        {
        }

        public virtual int DamageRoll(Hero owner)
        {
            return pdsharp.utils.Random.NormalIntRange(Min, Max);
        }

        public virtual float AcuracyFactor(Hero hero)
        {
            return 1f;
        }

        public virtual float SpeedFactor(Hero hero)
        {
            return 1f;
        }

        public virtual void Proc(Character attacker, Character defender, int damage)
        {
        }
    }
}