using pdsharp.noosa;
using sharpdungeon.effects;
using sharpdungeon.ui;

namespace sharpdungeon.scenes
{
    public class AmuletScene : PixelScene
    {
        private const string TxtExit = "Let's call it a day";
        private const string TxtStay = "I'm not done yet";

        private const int WIDTH = 120;
        private const int BtnHeight = 18;
        private const float SmallGap = 2;
        private const float LargeGap = 8;

        private const string Txt = "You finally hold it in your hands, the Amulet of Yendor. Using its power " + "you can take over the world or bring peace and prosperity to people or whatever. " + "Anyway, your life will change forever and this game will end here. " + "Or you can stay a mere mortal a little longer.";

        public static bool NoText = false;

        private Image _amulet;

        public override void Create()
        {
            base.Create();

            BitmapTextMultiline text = null;
            if (!NoText)
            {
                text = CreateMultiline(Txt, 8);
                text.MaxWidth = WIDTH;
                text.Measure();
                Add(text);
            }

            _amulet = new Image(Assets.AMULET);
            Add(_amulet);

            var btnExit = new RedButton(TxtExit);
            btnExit.ClickAction = button =>
            {
                Dungeon.Win(ResultDescriptions.WIN);
                Dungeon.DeleteGame(Dungeon.Hero.heroClass, true);
                if (NoText)
                    Game.SwitchScene<TitleScene>();
                else 
                    Game.SwitchScene<RankingsScene>();
            };
            btnExit.SetSize(WIDTH, BtnHeight);
            Add(btnExit);

            var btnStay = new RedButton(TxtStay);
            btnStay.ClickAction = button => OnBackPressed();
            btnStay.SetSize(WIDTH, BtnHeight);
            Add(btnStay);

            float height;
            if (NoText)
            {
                height = _amulet.Height + LargeGap + btnExit.Height + SmallGap + btnStay.Height;

                _amulet.X = Align((Camera.Main.CameraWidth - _amulet.Width) / 2);
                _amulet.Y = Align((Camera.Main.CameraHeight - height) / 2);

                btnExit.SetPos((Camera.Main.CameraWidth - btnExit.Width) / 2, _amulet.Y + _amulet.Height + LargeGap);
                btnStay.SetPos(btnExit.Left(), btnExit.Bottom() + SmallGap);
            }
            else
            {
                height = _amulet.Height + LargeGap + text.Height + LargeGap + btnExit.Height + SmallGap + btnStay.Height;

                _amulet.X = Align((Camera.Main.CameraWidth - _amulet.Width) / 2);
                _amulet.Y = Align((Camera.Main.CameraHeight - height) / 2);

                text.X = Align((Camera.Main.CameraWidth - text.Width) / 2);
                text.Y = _amulet.Y + _amulet.Height + LargeGap;

                btnExit.SetPos((Camera.Main.CameraWidth - btnExit.Width) / 2, text.Y + text.Height + LargeGap);
                btnStay.SetPos(btnExit.Left(), btnExit.Bottom() + SmallGap);
            }

            new Flare(8, 48).Color(0xFFDDBB, true).Show(_amulet, 0).AngularSpeed = +30;

            FadeIn();
        }

        protected override void OnBackPressed()
        {
            InterlevelScene.mode = InterlevelScene.Mode.CONTINUE;
            Game.SwitchScene<InterlevelScene>();
        }

        private float _timer;

        public override void Update()
        {
            base.Update();

            if (!((_timer -= Game.Elapsed) < 0))
                return;

            _timer = pdsharp.utils.Random.Float(0.5f, 5f);

            var star = Recycle<Speck>();
            star.Reset(0, _amulet.X + 10.5f, _amulet.Y + 5.5f, Speck.DISCOVER);
            Add(star);
        }
    }
}