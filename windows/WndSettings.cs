using Android.OS;
using pdsharp.noosa.audio;
using pdsharp.noosa.ui;
using sharpdungeon.scenes;
using sharpdungeon.ui;
using CheckBox = sharpdungeon.ui.CheckBox;

namespace sharpdungeon.windows
{
    public class WndSettings : Window
    {
        private const string TxtZoomIn = "+";
        private const string TxtZoomOut = "-";
        private const string TxtZoomDefault = "Default Zoom";

        private const string TxtScaleUp = "Scale up UI";
        private const string TxtImmersive = "Immersive mode";

        private const string TxtMusic = "Music";

        private const string TxtSound = "Sound FX";

        private const string TxtBrightness = "Brightness";

        private const string TxtSwitchPort = "Switch to portrait";
        private const string TxtSwitchLand = "Switch to landscape";

        private const int WIDTH = 112;
        private const int BtnHeight = 20;
        private const int Gap = 2;

        private readonly RedButton _btnZoomOut;
        private readonly RedButton _btnZoomIn;

        public WndSettings(bool inGame)
        {
            CheckBox btnImmersive = null;

            if (inGame)
            {
                var w = BtnHeight;

                _btnZoomOut = new RedButton(TxtZoomOut);
                _btnZoomOut.ClickAction = button => Zoom(pdsharp.noosa.Camera.Main.Zoom - 1);
                Add(_btnZoomOut.SetRect(0, 0, w, BtnHeight));

                _btnZoomIn = new RedButton(TxtZoomIn);
                _btnZoomIn.ClickAction = button => Zoom(pdsharp.noosa.Camera.Main.Zoom + 1);
                Add(_btnZoomIn.SetRect(WIDTH - w, 0, w, BtnHeight));

                var btnZoomDefault = new RedButton(TxtZoomDefault);
                btnZoomDefault.ClickAction = button => Zoom(PixelScene.defaultZoom);
                btnZoomDefault.SetRect(_btnZoomOut.Right(), 0, WIDTH - _btnZoomIn.Width - _btnZoomOut.Width, BtnHeight);
                Add(btnZoomDefault);

            }
            else
            {
                var btnScaleUp = new CheckBox(TxtScaleUp);
                btnScaleUp.ClickAction = ScaleUpClick;
                btnScaleUp.SetRect(0, 0, WIDTH, BtnHeight);
                btnScaleUp.SetChecked(PixelDungeon.ScaleUp());
                Add(btnScaleUp);

                btnImmersive = new CheckBox(TxtImmersive);
                btnImmersive.ClickAction = ImmersiveClick;
                btnImmersive.SetRect(0, btnScaleUp.Bottom() + Gap, WIDTH, BtnHeight);
                btnImmersive.SetChecked(PixelDungeon.Immersed());
                btnImmersive.Enable(Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat);
                Add(btnImmersive);

            }

            var btnMusic = new CheckBox(TxtMusic);
            btnMusic.ClickAction = button =>
            {
                var checkBox = button as CheckBox;
                PixelDungeon.Music(checkBox != null && checkBox.Checked());
            };
            btnMusic.SetRect(0, (btnImmersive != null ? btnImmersive.Bottom() : BtnHeight) + Gap, WIDTH, BtnHeight);
            btnMusic.SetChecked(PixelDungeon.Music());
            Add(btnMusic);

            var btnSound = new CheckBox(TxtSound);
            btnSound.ClickAction = SoundClick;
            btnSound.SetRect(0, btnMusic.Bottom() + Gap, WIDTH, BtnHeight);
            btnSound.SetChecked(PixelDungeon.SoundFx());
            Add(btnSound);

            if (!inGame)
            {
                var btnOrientation = new RedButton(OrientationText());
                btnOrientation.ClickAction = OrientationClick;
                btnOrientation.SetRect(0, btnSound.Bottom() + Gap, WIDTH, BtnHeight);
                Add(btnOrientation);

                Resize(WIDTH, (int)btnOrientation.Bottom());
            }
            else
            {
                var btnBrightness = new CheckBox(TxtBrightness);
                btnBrightness.ClickAction = button =>
                {
                    var checkBox = button as CheckBox;
                    PixelDungeon.Brightness(checkBox != null && checkBox.Checked());
                };
                btnBrightness.SetRect(0, btnSound.Bottom() + Gap, WIDTH, BtnHeight);
                btnBrightness.SetChecked(PixelDungeon.Brightness());
                Add(btnBrightness);

                Resize(WIDTH, (int)btnBrightness.Bottom());
            }
        }

        private void SoundClick(Button obj)
        {
            var checkBox = obj as CheckBox;
            PixelDungeon.SoundFx(checkBox != null && checkBox.Checked());
            Sample.Instance.Play(Assets.SND_CLICK);
        }

        private void OrientationClick(Button obj)
        {
            PixelDungeon.Landscape(!PixelDungeon.Landscape());
        }

        private void ScaleUpClick(Button obj)
        {
            var checkBox = obj as CheckBox;
            PixelDungeon.ScaleUp(checkBox != null && checkBox.Checked());
        }

        private void ImmersiveClick(Button button)
        {
            var checkBox = button as CheckBox;
            PixelDungeon.Immerse(checkBox != null && checkBox.Checked());
        }

        private void Zoom(float value)
        {
            pdsharp.noosa.Camera.Main.ZoomTo(value);
            PixelDungeon.Zoom((int)(value - PixelScene.defaultZoom));

            UpdateEnabled();
        }

        private void UpdateEnabled()
        {
            var zoom = pdsharp.noosa.Camera.Main.Zoom;
            _btnZoomIn.Enable(zoom < PixelScene.maxZoom);
            _btnZoomOut.Enable(zoom > PixelScene.minZoom);
        }

        public string OrientationText()
        {
            return PixelDungeon.Landscape() ? TxtSwitchPort : TxtSwitchLand;
        }
    }
}