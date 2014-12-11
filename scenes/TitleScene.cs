using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.ui;
using sharpdungeon.effects;
using sharpdungeon.ui;

namespace sharpdungeon.scenes
{
    public class TitleScene : PixelScene
    {
        private const string TxtPlay = "Play";
        private const string TxtHighscores = "Rankings";
        private const string TxtBadges = "Badges";
        private const string TxtAbout = "About";

        public override void Create()
        {
            base.Create();

            Music.Instance.Play(Assets.THEME, true);
            //Music.Instance.Volume(1f);

            uiCamera.Visible = false;

            var w = Camera.Main.CameraWidth;
            var h = Camera.Main.CameraHeight;

            const float height = 180;

            var archs = new Archs();
            archs.SetSize(w, h);
            Add(archs);

            var title = BannerSprites.Get(BannerSprites.Type.PixelDungeon);
            Add(title);

            title.X = (w - title.Width) / 2;
            title.Y = (h - height) / 2;

            PlaceTorch(title.X + 20, title.Y + 20);
            PlaceTorch(title.X + title.Width - 20, title.Y + 20);

            var btnBadges = new DashboardItem(TxtBadges, 3);
            btnBadges.ClickAction = (button) => PixelDungeon.SwitchNoFade<BadgesScene>();
            btnBadges.SetPos(w / 2 - btnBadges.Width, (h + height) / 2 - DashboardItem.Size);
            Add(btnBadges);

            var btnAbout = new DashboardItem(TxtAbout, 1);
            btnAbout.ClickAction = (button) => PixelDungeon.SwitchNoFade<AboutScene>();
            btnAbout.SetPos(w / 2, (h + height) / 2 - DashboardItem.Size);
            Add(btnAbout);

            var btnPlay = new DashboardItem(TxtPlay, 0);
            btnPlay.ClickAction = (button) => PixelDungeon.SwitchNoFade<StartScene>();
            btnPlay.SetPos(w / 2 - btnPlay.Width, btnAbout.Top() - DashboardItem.Size);
            Add(btnPlay);

            var btnHighscores = new DashboardItem(TxtHighscores, 2);
            btnHighscores.ClickAction = (button) => PixelDungeon.SwitchNoFade<RankingsScene>();
            btnHighscores.SetPos(w / 2, btnPlay.Top());
            Add(btnHighscores);

            var version = new BitmapText("v " + Game.version, font1x);
            version.Measure();
            version.Hardlight(0x888888);
            version.X = w - version.Width;
            version.Y = h - version.Height;
            Add(version);

            var btnPrefs = new PrefsButton();
            btnPrefs.SetPos(0, 0);
            Add(btnPrefs);

            var btnExit = new ExitButton();
            btnExit.SetPos(w - btnExit.Width, 0);
            Add(btnExit);

            FadeIn();
        }

        private void PlaceTorch(float x, float y)
        {
            var fb = new Fireball();
            fb.SetPos(x, y);
            Add(fb);
        }

        private class DashboardItem : Button
        {
            public const float Size = 48;

            private const int ImageSize = 32;

            private Image _image;
            private BitmapText _label;

            public DashboardItem(string text, int index)
            {
                _image.Frame(_image.texture.UvRect(index * ImageSize, 0, (index + 1) * ImageSize, ImageSize));
                _label.Text(text);
                _label.Measure();

                SetSize(Size, Size);
            }

            protected override void CreateChildren()
            {
                base.CreateChildren();

                _image = new Image(Assets.DASHBOARD);
                Add(_image);

                _label = CreateText(9);
                Add(_label);
            }

            protected override void Layout()
            {
                base.Layout();

                _image.X = Align(X + (Width - _image.Width) / 2);
                _image.Y = Align(Y);

                _label.X = Align(X + (Width - _label.Width) / 2);
                _label.Y = Align(_image.Y + _image.Height + 2);
            }

            protected override void OnTouchDown()
            {
                _image.Brightness(1.5f);
                Sample.Instance.Play(Assets.SND_CLICK, 1, 1, 0.8f);
            }

            protected override void OnTouchUp()
            {
                _image.ResetColor();
            }
        }
    }
}