using System.Linq;
using Java.Lang;
using Java.Util.Regex;
using pdsharp.input;
using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.scenes;
using System;

namespace sharpdungeon.ui
{
    public class Window : Group, IListener<Key>
    {
        protected internal int Width;
        protected internal int Height;

        protected internal TouchArea Blocker;
        protected internal NinePatch Chrome;

        public const int TitleColor = 0xFFFF44;

        public Window()
            : this(0, 0, sharpdungeon.Chrome.Get(sharpdungeon.Chrome.Type.WINDOW))
        {
        }

        public Window(int width, int height)
            : this(width, height, sharpdungeon.Chrome.Get(sharpdungeon.Chrome.Type.WINDOW))
        {
        }

        public Window(int width, int height, NinePatch chrome)
        {
            Blocker = new TouchArea(0, 0, PixelScene.uiCamera.CameraWidth, PixelScene.uiCamera.CameraHeight);
            Blocker.ClickAction = BlockerClickAction;
            Blocker.Camera = PixelScene.uiCamera;
            Add(Blocker);

            Chrome = chrome;

            Width = width;
            Height = height;

            chrome.X = -chrome.MarginLeft();
            chrome.Y = -chrome.MarginTop();
            chrome.Size(width - chrome.X + chrome.MarginRight(), height - chrome.Y + chrome.MarginBottom());
            Add(chrome);

            Camera = new Camera(0, 0, (int)chrome.Width, (int)chrome.Height, PixelScene.defaultZoom);
            Camera.X = (int)(Game.Width - Camera.CameraWidth * Camera.Zoom) / 2;
            Camera.Y = (int)(Game.Height - Camera.CameraHeight * Camera.Zoom) / 2;
            Camera.Scroll.Set(chrome.X, chrome.Y);
            Camera.Add(Camera);

            Keys.Event.Add(this);
        }

        private void BlockerClickAction(Touch touch)
        {
            if (!Chrome.OverlapsScreenPoint((int)touch.Current.X, (int)touch.Current.Y))
                OnBackPressed();
        }

        public virtual void Resize(int w, int h)
        {
            Width = w;
            Height = h;

            Chrome.Size(Width + Chrome.MarginHor(), Height + Chrome.MarginVer());

            Camera.Resize((int)Chrome.Width, (int)Chrome.Height);
            Camera.X = (int)(Game.Width - Camera.ScreenWidth) / 2;
            Camera.Y = (int)(Game.Height - Camera.ScreenHeight) / 2;
        }

        public virtual void Hide()
        {
            Parent.Erase(this);
            Destroy();
        }

        public override void Destroy()
        {
            base.Destroy();

            Camera.Remove(Camera);
            Keys.Event.Remove(this);
        }

        public void OnSignal(Key key)
        {
            if (key.Pressed)
            {
                switch (key.Code)
                {
                    case Keys.Back:
                        OnBackPressed();
                        break;
                    case Keys.Menu:
                        OnMenuPressed();
                        break;
                }
            }

            Keys.Event.Cancel();
        }

        public Action BackPressedAction { get; set; }


        public virtual void OnBackPressed()
        {
            Hide();

            if (BackPressedAction != null)
                BackPressedAction();
        }

        public virtual void OnMenuPressed()
        {
        }

        protected internal class Highlighter
        {
            private static readonly Pattern HIGHLIGHTER = Pattern.Compile("_(.*?)_");
            private static readonly Pattern Stripper = Pattern.Compile("[\\Negative]");

            public string Text;

            public bool[] Mask;

            public Highlighter(string text)
            {
                var stripped = Stripper.Matcher(text).ReplaceAll("");
                Mask = new bool[stripped.Length];

                var m = HIGHLIGHTER.Matcher(stripped);

                var pos = 0;
                var lastMatch = 0;

                while (m.Find())
                {
                    pos += (m.Start() - lastMatch);
                    var groupLen = m.Group(1).Length;

                    for (var i = pos; i < pos + groupLen; i++)
                        Mask[i] = true;

                    pos += groupLen;
                    lastMatch = m.End();
                }

                m.Reset(text);
                var sb = new StringBuffer();
                while (m.Find())
                    m.AppendReplacement(sb, m.Group(1));

                m.AppendTail(sb);

                Text = sb.ToString();
            }

            public virtual bool[] Inverted()
            {
                var result = new bool[Mask.Length];

                for (var i = 0; i < result.Length; i++)
                    result[i] = !Mask[i];

                return result;
            }

            public virtual bool IsHighlighted
            {
                get { return Mask.Any(t => t); }
            }
        }
    }
}