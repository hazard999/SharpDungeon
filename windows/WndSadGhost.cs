using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items;
using sharpdungeon.items.quest;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndSadGhost : Window
    {
        private readonly Ghost _ghost;
        private readonly Item _item;
        private const string TxtRose = "Yes! Yes!!! This is it! Please give it to me! " +
            "And you can take one of these items, maybe they " +
            "will be useful to you in your journey...";
        private const string TxtRat = "Yes! The ugly creature is slain and I can finally rest... " +
            "Please take one of these items, maybe they " +
            "will be useful to you in your journey...";
        private const string TxtWeapon = "Ghost's weapon";
        private const string TxtArmor = "Ghost's armor";

        private const int WIDTH = 120;
        private const int BTN_HEIGHT = 18;
        private const float GAP = 2;

        public WndSadGhost(Ghost ghost, Item item)
        {
            _ghost = ghost;
            _item = item;
            var titlebar = new IconTitle();
            titlebar.Icon(new ItemSprite(item.image, null));
            titlebar.Label(Utils.Capitalize(item.Name));
            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            BitmapTextMultiline message = PixelScene.CreateMultiline(item is DriedRose ? TxtRose : TxtRat, 6);
            message.MaxWidth = WIDTH;
            message.Measure();
            message.Y = titlebar.Bottom() + GAP;
            Add(message);

            var btnWeapon = new RedButton(TxtWeapon);
            btnWeapon.ClickAction = WeaponClick;
            btnWeapon.SetRect(0, message.Y + message.Height + GAP, WIDTH, BTN_HEIGHT);
            Add(btnWeapon);

            var btnArmor = new RedButton(TxtArmor);
            btnArmor.ClickAction = ArmorClick;
            btnArmor.SetRect(0, btnWeapon.Bottom() + GAP, WIDTH, BTN_HEIGHT);
            Add(btnArmor);

            Resize(WIDTH, (int)btnArmor.Bottom());
        }

        private void ArmorClick(Button button)
        {
            SelectReward(_ghost, _item, Ghost.Quest.armor);
        }

        private void WeaponClick(Button button)
        {
            SelectReward(_ghost, _item, Ghost.Quest.weapon);
        }

        private void SelectReward(Ghost ghost, Item item, Item reward)
        {
            Hide();

            item.Detach(Dungeon.Hero.Belongings.Backpack);

            if (reward.DoPickUp(Dungeon.Hero))
                GLog.Information(Hero.TxtYouNowHave, reward.Name);
            else
                Dungeon.Level.Drop(reward, ghost.pos).Sprite.Drop();

            ghost.Yell("Farewell, adventurer!");
            ghost.Die(null);

            Ghost.Quest.Complete();
        }
    }
}