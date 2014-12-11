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
    public class WndImp : Window
    {
        private readonly Imp _imp;
        private readonly DwarfToken _tokens;

        private const string TxtMessage = "Oh yes! You are my hero!\\Negative" +
            "Regarding your reward, I don't have cash with me right now, but I have something better for you. " +
            "This is my family heirloom ring: my granddad took it off a dead paladin's finger.";
        private const string TxtReward = "Take the ring";

        private const int WIDTH = 120;
        private const int BtnHeight = 18;
        private const float Gap = 2;

        public WndImp(Imp imp, DwarfToken tokens)
        {
            _imp = imp;
            _tokens = tokens;
            var titlebar = new IconTitle();
            titlebar.Icon(new ItemSprite(tokens.Image, null));
            titlebar.Label(Utils.Capitalize(tokens.Name));
            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            BitmapTextMultiline message = PixelScene.CreateMultiline(TxtMessage, 6);
            message.MaxWidth = WIDTH;
            message.Measure();
            message.Y = titlebar.Bottom() + Gap;
            Add(message);

            var btnReward = new RedButton(TxtReward);
            btnReward.ClickAction = RewardClickAction;
            btnReward.SetRect(0, message.Y + message.Height + Gap, WIDTH, BtnHeight);
            Add(btnReward);

            Resize(WIDTH, (int)btnReward.Bottom());
        }

        private void RewardClickAction(Button button)
        {
            TakeReward(_imp, _tokens, Imp.Quest.reward);
        }

        private void TakeReward(Imp imp, DwarfToken tokens, Item reward)
        {
            Hide();

            tokens.DetachAll(Dungeon.Hero.Belongings.Backpack);

            reward.Identify();
            if (reward.DoPickUp(Dungeon.Hero))
                GLog.Information(Hero.TxtYouNowHave, reward.Name);
            else
                Dungeon.Level.Drop(reward, imp.pos).Sprite.Drop();

            imp.Flee();

            Imp.Quest.Complete();
        }
    }
}