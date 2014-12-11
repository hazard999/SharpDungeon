using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfUpgrade : InventoryScroll
    {
        private const string TxtLooksBetter = "your {0} certainly looks better now";

        public ScrollOfUpgrade()
        {
            name = "Scroll of Upgrade";
            InventoryTitle = "Select an item to Upgrade";
            Mode = WndBag.Mode.UPGRADEABLE;
        }

        protected internal override void OnItemSelected(Item item)
        {
            ScrollOfRemoveCurse.Uncurse(Dungeon.Hero, item);
            item.Upgrade();

            GLog.Positive(TxtLooksBetter, item.Name);

            Badge.ValidateItemLevelAquired(item);

            Upgrade(CurUser);
        }

        public static void Upgrade(Hero hero)
        {
            hero.Sprite.Emitter().Start(Speck.Factory(Speck.UP), 0.2f, 3);
        }

        public override string Desc()
        {
            return "This scroll will Upgrade a single item, improving its quality. A wand will " + "increase in power and in number of charges; a weapon will inflict more damage " + "or find its mark more frequently; a suit of armor will deflect additional blows; " + "the effect of a ring on its wearer will intensify. Weapons and armor will also " + "require less strength to use, and any curses on the item will be lifted.";
        }
    }

}