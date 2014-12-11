using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.missiles
{
    public class Boomerang : MissileWeapon
    {
        public Boomerang()
        {
            name = "boomerang";
            image = ItemSpriteSheet.BOOMERANG;

            Str = 10;

            Min = 1;
            Max = 4;

            Stackable = false;
        }

        public override bool Upgradable
        {
            get { return true; }
        }

        public override Item Upgrade()
        {
            return Upgrade(false);
        }

        public override Item Upgrade(bool enchant)
        {
            Min += 1;
            Max += 2;

            base.Upgrade(enchant);

            UpdateQuickslot();

            return this;
        }

        public override Item Degrade()
        {
            Min -= 1;
            Max -= 2;
            return base.Degrade();
        }

        public override Weapon Enchant(Enchantment ench)
        {
            while (ench is Piercing || ench is Swing)
                ench = Enchantment.Random();

            return base.Enchant(ench);
        }

        public override void Proc(Character attacker, Character defender, int damage)
        {
            base.Proc(attacker, defender, damage);

            var hero = attacker as Hero;
            if (hero != null && hero.RangedWeapon == this)
                CircleBack(defender.pos, hero);
        }

        protected internal override void Miss(int cell)
        {
            CircleBack(cell, CurUser);
        }

        private void CircleBack(int from, Hero owner)
        {
            CurUser.Sprite.Parent.Recycle<MissileSprite>().Reset(from, CurUser.pos, curItem, null);

            if (_throwEquiped)
            {
                owner.Belongings.Weapon = this;
                owner.Spend(-TimeToEquip);
            }
            else
                if (!Collect(CurUser.Belongings.Backpack))
                    Dungeon.Level.Drop(this, owner.pos).Sprite.Drop();
        }

        private bool _throwEquiped;

        public override void Cast(Hero user, int dst)
        {
            _throwEquiped = IsEquipped(user);
            base.Cast(user, dst);
        }

        public override string Desc()
        {
            return "Thrown to the enemy this flat curved wooden missile will return to the hands of its thrower.";
        }
    }

}