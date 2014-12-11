using pdsharp.noosa;
using pdsharp.noosa.audio;
using sharpdungeon.ui;
using System;

namespace sharpdungeon.scenes
{
    public class BadgesScene : PixelScene
    {
        private const string TxtTitle = "Your Badges";

        public override void Create()
        {
            base.Create();

            Music.Instance.Play(Assets.THEME, true);
            Music.Instance.Volume(1f);

            uiCamera.Visible = false;

            var w = Camera.Main.CameraWidth;
            var h = Camera.Main.CameraHeight;

            var archs = new Archs();
            archs.SetSize(w, h);
            Add(archs);

            var pw = Math.Min(160, w - 6);
            var ph = h - 30;

            var panel = Chrome.Get(Chrome.Type.WINDOW);
            panel.Size(pw, ph);
            panel.X = (w - pw) / 2;
            panel.Y = (h - ph) / 2;
            Add(panel);

            var title = CreateText(TxtTitle, 9);
            title.Hardlight(Window.TitleColor);
            title.Measure();
            title.X = Align((w - title.Width) / 2);
            title.Y = Align((panel.Y - title.BaseLine()) / 2);
            Add(title);

            Badge.LoadGlobal();

            ScrollPane list = new BadgesList(true);
            Add(list);

            list.SetRect(panel.X + panel.MarginLeft(), panel.Y + panel.MarginTop(), panel.InnerWidth(), panel.InnerHeight());

            var btnExit = new ExitButton();
            btnExit.SetPos(Camera.Main.CameraWidth - btnExit.Width, 0);
            Add(btnExit);

            FadeIn();
        }

        protected override void OnBackPressed()
        {
            PixelDungeon.SwitchNoFade<TitleScene>();
        }
    }
}