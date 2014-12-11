using Java.IO;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.scenes;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
    public class WndGame : Window
    {
        private const string TxtSettings = "Settings";
        private const string TxtChalleges = "Challenges";
        private const string TxtRankings = "Rankings";
        private const string TxtStart = "Start New Game";
        private const string TxtMenu = "Main Menu";
        private const string TxtExit = "Exit Pixel Dungeon";
        private const string TxtReturn = "Return to Game";

        private const int WIDTH = 120;
        private const int BtnHeight = 20;
        private const int Gap = 2;

        private int _pos;

        public WndGame()
        {
            var btnSetting = new RedButton(TxtSettings);
            btnSetting.ClickAction = button =>
            {
                Hide();
                GameScene.Show(new WndSettings(true));
            };
            AddButton(btnSetting);

            if (Dungeon.Challenges > 0)
            {
                var btnChallenges = new RedButton(TxtChalleges);
                btnChallenges.ClickAction = button =>
                {
                    Hide();
                    GameScene.Show(new WndChallenges(Dungeon.Challenges, false));
                };
                AddButton(btnChallenges);
            }

            if (!Dungeon.Hero.IsAlive)
            {
                var btnStart = new RedButton(TxtStart);
                btnStart.ClickAction = StartAction;
                AddButton(btnStart);
                btnStart.Icon(Dungeon.Hero.heroClass.Get());

                var btnRankings = new RedButton(TxtRankings);
                btnRankings.ClickAction = RankingsAction;

                AddButton(btnRankings);
            }

            var btnMenu = new RedButton(TxtMenu);
            btnMenu.ClickAction = button =>
            {
                try
                {
                    Dungeon.SaveAll();
                }
                catch (IOException)
                {
                }
                Game.SwitchScene<TitleScene>();
            };

            AddButton(btnMenu);

            var btnExit = new RedButton(TxtExit);
            btnExit.ClickAction = button => Game.Instance.Finish();
            AddButton(btnExit);

            var btnReturn = new RedButton(TxtReturn);
            btnReturn.ClickAction = button => Hide();
            AddButton(btnReturn);

            Resize(WIDTH, _pos);
        }

        private void RankingsAction(Button obj)
        {
            InterlevelScene.mode = InterlevelScene.Mode.DESCEND; 
            Game.SwitchScene<RankingsScene>();
        }

        private void StartAction(Button obj)
        {
            Dungeon.Hero = null;
            PixelDungeon.Challenges(Dungeon.Challenges);
            InterlevelScene.mode = InterlevelScene.Mode.DESCEND;
            InterlevelScene.noStory = true;
            Game.SwitchScene<InterlevelScene>();
        }

        private void AddButton(RedButton btn)
        {
            Add(btn);
            btn.SetRect(0, _pos > 0 ? _pos += Gap : 0, WIDTH, BtnHeight);
            _pos += BtnHeight;
        }
    }
}