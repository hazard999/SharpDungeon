using pdsharp.noosa;
using sharpdungeon.actors.hero;
using sharpdungeon.items;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
    public class WndResurrect : Window
    {
        private const string TxtMessage = "You died, but you were given another chance to win this dungeon. Will you take it?";
        private const string TxtYes = "Yes, I will fight!";
        private const string TxtNo = "No, I give up";

        private const int WIDTH = 120;
        private const int BtnHeight = 18;
        private const float Gap = 2;

        public static WndResurrect Instance;
        public static object CauseOfDeath;

        public WndResurrect(Ankh ankh, object causeOfDeath)
        {
            Instance = this;
            CauseOfDeath = causeOfDeath;

            var titlebar = new IconTitle();
            titlebar.Icon(new ItemSprite(ankh.image, null));
            titlebar.Label(ankh.Name);
            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            var message = PixelScene.CreateMultiline(TxtMessage, 6);
            message.MaxWidth = WIDTH;
            message.Measure();
            message.Y = titlebar.Bottom() + Gap;
            Add(message);


            var btnYes = new RedButton(TxtYes);
            btnYes.ClickAction = button =>
            {
                Hide();
                Statistics.AnkhsUsed++;
                InterlevelScene.mode = InterlevelScene.Mode.RESURRECT;
                Game.SwitchScene<InterlevelScene>();
            };
            btnYes.SetRect(0, message.Y + message.Height + Gap, WIDTH, BtnHeight);
            Add(btnYes);

            var btnNo = new RedButton(TxtNo);
            btnNo.ClickAction = button =>
            {
                Hide();
                Rankings.Instance.Submit(false);
                Hero.ReallyDie(causeOfDeath);
            };
            btnNo.SetRect(0, btnYes.Bottom() + Gap, WIDTH, BtnHeight);
            Add(btnNo);


            Resize(WIDTH, (int)btnNo.Bottom());
        }

        public override void Destroy()
        {
            base.Destroy();
            Instance = null;
        }

        public override void OnBackPressed()
        {
        }
    }
}