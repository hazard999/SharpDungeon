using System.Collections.Generic;
using Java.Lang;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.particles;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.ui;
using sharpdungeon.utils;
using pdsharp.noosa.ui;
using sharpdungeon.windows;
using Math = System.Math;

namespace sharpdungeon.scenes
{
    public class StartScene : PixelScene
    {
        private const float BUTTON_HEIGHT = 24;
        private const float GAP = 2;

        private const string TXT_LOAD = "Load Game";
        private const string TXT_NEW = "New Game";

        private const string TXT_ERASE = "Erase current game";
        private const string TXT_DPTH_LVL = "Depth: {0}, Level: {1}";

        private const string TXT_REALLY = "Do you really want to start new game?";
        private const string TXT_WARNING = "Your current game progress will be erased.";
        private const string TXT_YES = "Yes, start new game";
        private const string TXT_NO = "No, return to main menu";

        private const string TXT_UNLOCK = "To unlock this character class, slay the 3rd boss with any other class";

        public const string TXT_WIN_THE_GAME = "To unlock \"Challenges\", win the game with any character class.";

        private const float WIDTH = 116;
        private const float HEIGHT = 220;

        private static readonly Dictionary<HeroClass, ClassShield> Shields = new Dictionary<HeroClass, ClassShield>();

        private GameButton btnLoad;
        private GameButton btnNewGame;

        private bool huntressUnlocked;
        private Group unlock;

        public static HeroClass curClass;

        public override void Create()
        {
            base.Create();

            Badge.LoadGlobal();

            uiCamera.Visible = false;

            var w = Camera.Main.CameraWidth;
            var h = Camera.Main.CameraHeight;

            var left = (w - WIDTH) / 2;
            var top = (h - HEIGHT) / 2;
            var bottom = h - top;

            var archs = new Archs();
            archs.SetSize(w, h);
            Add(archs);

            var title = BannerSprites.Get(BannerSprites.Type.SelectYourHero);
            title.X = Align((w - title.Width) / 2);
            title.Y = top;
            Add(title);

            btnNewGame = new GameButton(TXT_NEW);
            btnNewGame.ClickAction = NewGameClick;
            Add(btnNewGame);

            btnLoad = new GameButton(TXT_LOAD);
            btnLoad.ClickAction = LoadClick;
            Add(btnLoad);

            var classes = new[]
            {
                HeroClass.Warrior, 
                HeroClass.Mage,
                HeroClass.Rogue, 
                HeroClass.Huntress
            };

            const float shieldW = WIDTH / 2;
            var shieldH = Math.Min((bottom - BUTTON_HEIGHT - title.Y - title.Height) / 2, shieldW * 1.2f);
            top = (bottom - BUTTON_HEIGHT + title.Y + title.Height - shieldH * 2) / 2;
            for (var i = 0; i < classes.Length; i++)
            {
                var shield = new ClassShield(classes[i], this);
                shield.SetRect(left + (i % 2) * shieldW, top + (i / 2) * shieldH, shieldW, shieldH);
                Add(shield);

                if (Shields.ContainsKey(classes[i]))
                    Shields[classes[i]] = shield;
                else
                    Shields.Add(classes[i], shield);
            }

            unlock = new Group();
            Add(unlock);

            var challenge = new ChallengeButton();
            challenge.SetPos(w / 2 - challenge.Width / 2, top + shieldH - challenge.Height / 2);
            Add(challenge);

            if (!(huntressUnlocked = Badge.IsUnlocked(Badge.BOSS_SLAIN_3)))
            {
                var text = CreateMultiline(TXT_UNLOCK, 9);
                text.MaxWidth = (int)WIDTH;
                text.Measure();

                float pos = (bottom - BUTTON_HEIGHT) + (BUTTON_HEIGHT - text.Height) / 2;
                foreach (var line in new LineSplitter(text.Font, text.Scale, text.Text()).Split())
                {
                    line.Measure();
                    line.Hardlight(0xFFFF00);
                    line.X = Align(left + WIDTH / 2 - line.Width / 2);
                    line.Y = Align(pos);
                    unlock.Add(line);

                    pos += line.Height;
                }
            }

            var btnExit = new ExitButton();
            btnExit.SetPos(Camera.Main.CameraWidth - btnExit.Width, 0);
            Add(btnExit);

            curClass = null;
            UpdateClass(HeroClass.Values()[PixelDungeon.LastClass()]);

            FadeIn();
        }

        private void LoadClick(Button obj)
        {
            InterlevelScene.mode = InterlevelScene.Mode.CONTINUE;
            Game.SwitchScene<InterlevelScene>();
        }

        private void NewGameClick(Button obj)
        {
            if (GamesInProgress.Check(curClass) != null)
            {
                var progressWnd = new WndOptions(TXT_REALLY, TXT_WARNING, TXT_YES, TXT_NO);
                progressWnd.SelectAction = (index) =>
                {
                    if (index == 0)
                        StartNewGame();
                };
                Add(progressWnd);
            }
            else
                StartNewGame();
        }

        public void UpdateClass(HeroClass cl)
        {
            if (curClass == cl)
            {
                Add(new WndClass(cl));
                return;
            }

            if (curClass != null)
                Shields[curClass].Highlight(false);

            Shields[curClass = cl].Highlight(true);

            if (cl != HeroClass.Huntress || huntressUnlocked)
            {
                unlock.Visible = false;

                var buttonPos = (Camera.Main.CameraHeight + HEIGHT) / 2 - BUTTON_HEIGHT;

                var left = (Camera.Main.CameraWidth - WIDTH) / 2;

                var info = GamesInProgress.Check(curClass);
                if (info != null)
                {
                    btnLoad.Visible = true;
                    btnLoad.Secondary(Utils.Format(TXT_DPTH_LVL, info.Depth, info.Level));
                    btnNewGame.Visible = true;
                    btnNewGame.Secondary(TXT_ERASE);

                    var w = (WIDTH - GAP) / 2;

                    btnLoad.SetRect(left, buttonPos, w, BUTTON_HEIGHT);
                    btnNewGame.SetRect(btnLoad.Right() + GAP, buttonPos, w, BUTTON_HEIGHT);
                }
                else
                {
                    btnLoad.Visible = false;

                    btnNewGame.Visible = true;
                    btnNewGame.Secondary(null);
                    btnNewGame.SetRect(left, buttonPos, WIDTH, BUTTON_HEIGHT);
                }

            }
            else
            {
                unlock.Visible = true;
                btnLoad.Visible = false;
                btnNewGame.Visible = false;
            }
        }

        private void StartNewGame()
        {
            Dungeon.Hero = null;
            InterlevelScene.mode = InterlevelScene.Mode.DESCEND;

            if (PixelDungeon.Intro())
            {
                PixelDungeon.Intro(false);
                Game.SwitchScene(typeof(IntroScene));
            }
            else
                Game.SwitchScene(typeof(InterlevelScene));
        }

        protected override void OnBackPressed()
        {
            PixelDungeon.SwitchNoFade<TitleScene>();
        }
    }

    public class ChallengeButton : Button
    {
        private Image _image;

        public ChallengeButton()
        {

            _Width = _image.Width;
            _Height = _image.Height;

            _image.Am = Badge.IsUnlocked(Badge.VICTORY) ? 1.0f : 0.5f;
        }

        protected override void CreateChildren()
        {

            base.CreateChildren();

            _image = PixelDungeon.Challenges() > 0 ? Icons.CHALLENGE_ON.Get() : Icons.CHALLENGE_OFF.Get();
            Add(_image);
        }

        protected override void Layout()
        {
            base.Layout();

            _image.X = PixelScene.Align(X);
            _image.Y = PixelScene.Align(Y);
        }

        protected override void OnClick()
        {
            if (Badge.IsUnlocked(Badge.VICTORY))
            {
                //Add(new WndChallenges(PixelDungeon.challenges(), true) { public void onBackPressed() { base.onBackPressed(); image.Copy(Icons.Get(PixelDungeon.challenges() > 0 ? Icons.CHALLENGE_ON :Icons.CHALLENGE_OFF)); }; });
            }
            else
            {
                Add(new WndMessage(StartScene.TXT_WIN_THE_GAME));
            }
        }

        protected override void OnTouchDown()
        {
            Sample.Instance.Play(Assets.SND_CLICK);
        }
    }

    public class GameButton : RedButton
    {
        private const int SecondaryColor = 0xCACFC2;

        private BitmapText _secondary;

        public GameButton(string primary)
            : base(primary)
        {
            _secondary.Text(null);
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _secondary = PixelScene.CreateText(6);
            _secondary.Hardlight(SecondaryColor);
            Add(_secondary);
        }

        protected override void Layout()
        {
            base.Layout();

            if (_secondary.Text().Length > 0)
            {
                InternalText.Y = PixelScene.Align(Y + (Height - InternalText.Height - _secondary.BaseLine()) / 2);

                _secondary.X = PixelScene.Align(X + (Width - _secondary.Width) / 2);
                _secondary.Y = PixelScene.Align(InternalText.Y + InternalText.Height);
            }
            else
            {
                InternalText.Y = PixelScene.Align(Y + (Height - InternalText.BaseLine()) / 2);
            }
        }

        public virtual void Secondary(string text)
        {
            _secondary.Text(text);
            _secondary.Measure();
        }
    }


    public class ClassShield : Button
    {
        private const float MinBrightness = 0.6f;

        public const int WIDTH = 24;
        private const int HEIGHT = 28;
        private const int SCALE = 2;

        private readonly HeroClass _cl;
        private readonly StartScene _startScene;

        private Image _avatar;
        private BitmapText _name;
        private Emitter _emitter;

        private float _brightness;

        public ClassShield(HeroClass cl, StartScene startScene)
        {
            _cl = cl;
            _startScene = startScene;

            _avatar.Frame(((int)cl.Ordinal()) * WIDTH, 0, WIDTH, HEIGHT);
            _avatar.Scale.Set(SCALE);

            _name.Text(cl.Title());
            _name.Measure();

            _brightness = MinBrightness;
            UpdateBrightness();
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _avatar = new Image(Assets.AVATARS);
            Add(_avatar);

            _name = PixelScene.CreateText(9);
            Add(_name);

            _emitter = new Emitter();
            Add(_emitter);
        }

        protected override void Layout()
        {
            base.Layout();

            _avatar.X = PixelScene.Align(X + (Width - _avatar.Width) / 2);
            _avatar.Y = PixelScene.Align(Y + (Height - _avatar.Height - _name.Height) / 2);

            _name.X = PixelScene.Align(X + (Width - _name.Width) / 2);
            _name.Y = _avatar.Y + _avatar.Height + SCALE;

            _emitter.Pos(_avatar.X, _avatar.Y, _avatar.Width, _avatar.Height);
        }

        protected override void OnTouchDown()
        {
            _emitter.Revive();
            _emitter.Start(Speck.Factory(Speck.LIGHT), 0.05f, 7);

            Sample.Instance.Play(Assets.SND_CLICK, 1, 1, 1.2f);

            _startScene.UpdateClass(_cl);
        }

        public override void Update()
        {
            base.Update();

            if (!(_brightness < 1.0f) || !(_brightness > MinBrightness))
                return;

            if ((_brightness -= Game.Elapsed) <= MinBrightness)
                _brightness = MinBrightness;

            UpdateBrightness();
        }

        public virtual void Highlight(bool value)
        {
            if (value)
            {
                _brightness = 1.0f;
                _name.Hardlight(0xCACFC2);
            }
            else
            {
                _brightness = 0.999f;
                _name.Hardlight(0x444444);
            }

            UpdateBrightness();
        }

        private void UpdateBrightness()
        {
            _avatar.Gm = _avatar.Bm = _avatar.Rm = _avatar.Am = _brightness;
        }
    }
}