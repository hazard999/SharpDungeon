using sharpdungeon.effects;
using sharpdungeon.items.weapon;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfWeaponUpgrade : InventoryScroll
    {
        private const string TxtLooksBetter = "your {0} certainly looks better now";

        public ScrollOfWeaponUpgrade()
        {
            name = "Scroll of Weapon Upgrade";
            InventoryTitle = "Select a weapon to Upgrade";
            Mode = WndBag.Mode.WEAPON;
        }

        protected internal override void OnItemSelected(Item item)
        {
            var weapon = (Weapon)item;

            ScrollOfRemoveCurse.Uncurse(Dungeon.Hero, weapon);
            weapon.Upgrade(true);

            GLog.Positive(TxtLooksBetter, weapon.Name);

            Badge.ValidateItemLevelAquired(weapon);

            CurUser.Sprite.Emitter().Start(Speck.Factory(Speck.UP), 0.2f, 3);
        }

        public override string Desc()
        {
            return "This scroll will Upgrade a melee weapon, improving its quality. In contrast to a regular Scroll of Upgrade, " +
                "this specialized version will never destroy an enchantment on a weapon. On the contrary, it is able to imbue " +
                "an unenchanted weapon with a Random enchantment.";
        }
    }
}