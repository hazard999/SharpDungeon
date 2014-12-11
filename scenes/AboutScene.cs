using pdsharp.noosa;
using sharpdungeon.effects;
using sharpdungeon.ui;
using System;
using Android.Content;
using pdsharp.input;

namespace sharpdungeon.scenes
{
    public class AboutScene : PixelScene
    {
        private const string Txt = "Code & graphics: Watabou\\Negative" + "Music: Cube_Code\\Negative\\Negative" + "This game is inspired by Brian Walker's Brogue. " + "Try it on Windows, Mac OS or Linux - it's awesome! ;)\\Negative\\Negative" + "Please visit official website for additional info:";

        private const string Lnk = "pixeldungeon.watabou.ru";

        public override void Create()
        {
            base.Create();

            var text = CreateMultiline(Txt, 8);
            text.MaxWidth = Math.Min(Camera.Main.CameraWidth, 120);
            text.Measure();
            Add(text);

            text.X = Align((Camera.Main.CameraWidth - text.Width) / 2);
            text.Y = Align((Camera.Main.CameraHeight - text.Height) / 2);

            var link = CreateMultiline(Lnk, 8);
            link.MaxWidth = Math.Min(Camera.Main.CameraWidth, 120);
            link.Measure();
            link.Hardlight(Window.TitleColor);
            Add(link);

            link.X = text.X;
            link.Y = text.Y + text.Height;
            
            var hotArea = new TouchArea(link);
            hotArea.ClickAction = HotAreaClickAction;
            Add(hotArea);

            var wata = Icons.WATA.Get();
            wata.X = Align(text.X + (text.Width - wata.Width) / 2);
            wata.Y = text.Y - wata.Height - 8;
            Add(wata);

            new Flare(7, 64).Color(0x112233, true).Show(wata, 0).AngularSpeed = +20;

            var archs = new Archs();
            archs.SetSize(Camera.Main.CameraWidth, Camera.Main.CameraHeight);
            AddToBack(archs);

            var btnExit = new ExitButton();
            btnExit.SetPos(Camera.Main.CameraWidth - btnExit.Width, 0);
            Add(btnExit);

            FadeIn();
        }

        private void HotAreaClickAction(Touch obj)
        {
            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://" + Lnk));
            Game.Instance.StartActivity(intent);
        }

        protected override void OnBackPressed()
        {
            PixelDungeon.SwitchNoFade<TitleScene>();
        }
    }
}