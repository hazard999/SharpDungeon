using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.ui;
using sharpdungeon.effects;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;
using sharpdungeon.windows;
using System;

namespace sharpdungeon.scenes
{
    public class RankingsScene : PixelScene
    {
        private const string TxtTitle = "Top Rankings";
        private const string TxtTotal = "Total games played: {0}";
        private const string TxtNoGames = "No games have been played yet.";

        private const string TxtNoInfo = "No additional information";

        private const float RowHeight = 30;
        private const float Gap = 4;

        private Archs _archs;

        public override void Create()
        {
            base.Create();

            Music.Instance.Play(Assets.THEME, true);
            Music.Instance.Volume(1f);

            uiCamera.Visible = false;

            var w = Camera.Main.CameraWidth;
            var h = Camera.Main.CameraHeight;

            _archs = new Archs();
            _archs.SetSize(w, h);
            Add(_archs);

            Rankings.Instance.Load();

            if (Rankings.Instance.records.Count > 0)
            {
                var left = (w - Math.Min(160, w)) / 2 + Gap;
                var top = Align((h - RowHeight * Rankings.Instance.records.Count) / 2);

                var title = CreateText(TxtTitle, 9);
                title.Hardlight(Window.TitleColor);
                title.Measure();
                title.X = Align((w - title.Width) / 2);
                title.Y = Align(top - title.Height - Gap);
                Add(title);

                var pos = 0;

                foreach (var rec in Rankings.Instance.records)
                {
                    var row = new RecordButton(pos, pos == Rankings.Instance.lastRecord, rec);
                    row.SetRect(left, top + pos * RowHeight, w - left * 2, RowHeight);
                    Add(row);

                    pos++;
                }

                if (Rankings.Instance.totalNumber >= Rankings.TABLE_SIZE)
                {
                    var total = CreateText(Utils.Format(TxtTotal, Rankings.Instance.totalNumber), 8);
                    total.Hardlight(Window.TitleColor);
                    total.Measure();
                    total.X = Align((w - total.Width) / 2);
                    total.Y = Align(top + pos * RowHeight + Gap);
                    Add(total);
                }
            }
            else
            {
                var title = CreateText(TxtNoGames, 8);
                title.Hardlight(Window.TitleColor);
                title.Measure();
                title.X = Align((w - title.Width) / 2);
                title.Y = Align((h - title.Height) / 2);
                Add(title);
            }

            var btnExit = new ExitButton();
            btnExit.SetPos(Camera.Main.CameraWidth - btnExit.Width, 0);
            Add(btnExit);

            FadeIn();
        }

        protected override void OnBackPressed()
        {
            PixelDungeon.SwitchNoFade<TitleScene>();
        }

        public class RecordButton : Button
        {
            private const float Gap = 4;

            private const int TextWin = 0xFFFF88;
            private const int TextLose = 0xCCCCCC;
            private const int FlareWin = 0x888866;
            private const int FlareLose = 0x666666;

            private readonly Record _rec;

            private ItemSprite _shield;
            private readonly Flare _flare;
            private BitmapText _position;
            private BitmapTextMultiline _desc;
            private Image _classIcon;

            public RecordButton(int pos, bool latest, Record rec)
            {
                _rec = rec;

                if (latest)
                {
                    _flare = new Flare(6, 24);
                    _flare.AngularSpeed = 90;
                    _flare.Color(rec.win ? FlareWin : FlareLose);
                    AddToBack(_flare);
                }

                _position.Text((pos + 1).ToString());
                _position.Measure();

                _desc.Text(rec.info);
                _desc.Measure();

                if (rec.win)
                {
                    _shield.View(ItemSpriteSheet.AMULET, null);
                    _position.Hardlight(TextWin);
                    _desc.Hardlight(TextWin);
                }
                else
                {
                    _position.Hardlight(TextLose);
                    _desc.Hardlight(TextLose);
                }

                _classIcon.Copy(IconsExtensions.Get(rec.heroClass));
            }

            protected override void CreateChildren()
            {
                base.CreateChildren();

                _shield = new ItemSprite(ItemSpriteSheet.TOMB, null);
                Add(_shield);

                _position = new BitmapText(font1x);
                Add(_position);

                _desc = CreateMultiline(9);
                Add(_desc);

                _classIcon = new Image();
                Add(_classIcon);
            }

            protected override void Layout()
            {
                base.Layout();

                _shield.X = X;
                _shield.Y = Y + (Height - _shield.Height) / 2;

                _position.X = Align(_shield.X + (_shield.Width - _position.Width) / 2);
                _position.Y = Align(_shield.Y + (_shield.Height - _position.Height) / 2 + 1);

                if (_flare != null)
                    _flare.Point(_shield.Center());

                _classIcon.X = Align(X + Width - _classIcon.Width);
                _classIcon.Y = _shield.Y;

                _desc.X = _shield.X + _shield.Width + Gap;
                _desc.MaxWidth = (int)(_classIcon.X - _desc.X);
                _desc.Measure();
                _desc.Y = _position.Y + _position.BaseLine() - _desc.BaseLine();
            }

            protected override void OnClick()
            {
                if (_rec.gameFile.Length > 0)
                    Parent.Add(new WndRanking(_rec.gameFile));
                else
                    Parent.Add(new WndError(TxtNoInfo));
            }
        }
    }
}