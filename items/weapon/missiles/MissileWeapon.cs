using System.Collections.Generic;
using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using System.Text;
using sharpdungeon.scenes;
using sharpdungeon.windows;

namespace sharpdungeon.items.weapon.missiles
{
    public class MissileWeapon : Weapon
    {
        private const string TXT_MISSILES = "Factory weapon";
        private const string TXT_YES = "Yes, I know what I'm doing";
        private const string TXT_NO = "No, I changed my mind";
        private const string TXT_R_U_SURE = "Do you really want to equip it as a melee weapon?";

        public MissileWeapon()
        {
            Stackable = true;
            levelKnown = true;
            DefaultAction = AcThrow;
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);

            if (hero.heroClass == HeroClass.Huntress || hero.heroClass == HeroClass.Rogue)
                return actions;

            actions.Remove(AcEquip);
            actions.Remove(AcUnequip);
            return actions;
        }

        protected override void OnThrow(int cell)
        {
            var enemy = Actor.FindChar(cell);
            if (enemy == null || enemy == CurUser)
                base.OnThrow(cell);
            else
            {
                if (!CurUser.Shoot(enemy, this))
                    Miss(cell);
            }
        }

        protected internal virtual void Miss(int cell)
        {
            base.OnThrow(cell);
        }

        public override void Proc(Character attacker, Character defender, int damage)
        {
            base.Proc(attacker, defender, damage);

            var hero = (Hero)attacker;
            if (hero.RangedWeapon != null || !Stackable)
                return;

            if (quantity == 1)
                DoUnequip(hero, false, false);
            else
                Detach(null);
        }

        public override bool DoEquip(Hero hero)
        {
            var wndOptions = new WndOptions(TXT_MISSILES, TXT_R_U_SURE, TXT_YES, TXT_NO);

            wndOptions.SelectAction = (index) =>
            {
                if (index == 0)
                    base.DoEquip(hero);
            };

            GameScene.Show(wndOptions);

            return false;
        }

        public override Item Random()
        {
            return this;
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override bool Identified
        {
            get { return true; }
        }

        public override string Info()
        {
            var info = new StringBuilder(Desc());

            info.Append("\\Negative\nAverage damage of this weapon equals to " + (Min + (Max - Min) / 2) + " points per hit. ");

            if (Dungeon.Hero.Belongings.Backpack.Items.Contains(this))
            {
                if (Str > Dungeon.Hero.STR)
                    info.Append("Because of your inadequate strength the accuracy and speed " + "of your DoAttack with this " + name + " is decreased.");

                if (Str < Dungeon.Hero.STR)
                    info.Append("Because of your excess strength the damage " + "of your DoAttack with this " + name + " is increased.");
            }

            if (IsEquipped(Dungeon.Hero))
                info.Append("\\Negative\nYou hold the " + name + " at the ready.");

            return info.ToString();
        }
    }
}