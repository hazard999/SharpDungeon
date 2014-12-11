using pdsharp.noosa.ui;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items;
using sharpdungeon.items.wands;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndWandmaker : Window
    {
        private readonly Wandmaker _wandmaker;
        private readonly Item _item;
        private const string TxtMessage = "Oh, I see you have succeeded! I do hope it hasn't troubled you too much. " +
            "As I promised, you can choose one of my high quality wands.";
        private const string TxtBattle = "Battle wand";
        private const string TxtNonBattle = "Non-battle wand";

        private const string TxtFarawell = "Good luck in your quest, {0}!";

        private const int WIDTH = 120;
        private const int BtnHeight = 18;
        private const float Gap = 2;

        public WndWandmaker(Wandmaker wandmaker, Item item)
        {
            _wandmaker = wandmaker;
            _item = item;
            var titlebar = new IconTitle();
            titlebar.Icon(new ItemSprite(item.image, null));
            titlebar.Label(Utils.Capitalize(item.Name));
            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            var message = PixelScene.CreateMultiline(TxtMessage, 6);
            message.MaxWidth = WIDTH;
            message.Measure();
            message.Y = titlebar.Bottom() + Gap;
            Add(message);


            var btnBattle = new RedButton(TxtBattle);
            btnBattle.ClickAction = BattleAction;
            btnBattle.SetRect(0, message.Y + message.Height + Gap, WIDTH, BtnHeight);
            Add(btnBattle);

            var btnNonBattle = new RedButton(TxtNonBattle);
            btnNonBattle.ClickAction = NonBattleAction;
            btnNonBattle.SetRect(0, btnBattle.Bottom() + Gap, WIDTH, BtnHeight);
            Add(btnNonBattle);

            Resize(WIDTH, (int)btnNonBattle.Bottom());
        }

        private void NonBattleAction(Button button)
        {
            SelectReward(_wandmaker, _item, Wandmaker.Quest.Wand2);
        }

        private void BattleAction(Button button)
        {
            SelectReward(_wandmaker, _item, Wandmaker.Quest.Wand1);
        }

        private void SelectReward(Wandmaker wandmaker, Item item, Wand reward)
        {
            Hide();

            item.Detach(Dungeon.Hero.Belongings.Backpack);

            reward.Identify();
            if (reward.DoPickUp(Dungeon.Hero))
                GLog.Information(Hero.TxtYouNowHave, reward.Name);
            else
                Dungeon.Level.Drop(reward, wandmaker.pos).Sprite.Drop();

            wandmaker.Yell(Utils.Format(TxtFarawell, Dungeon.Hero.ClassName()));
            wandmaker.Destroy();

            wandmaker.Sprite.DoDie();

            Wandmaker.Quest.Complete();
        }
    }
}