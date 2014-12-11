using System.Collections.Generic;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.ui;
using sharpdungeon.scenes;
using sharpdungeon.ui;
using System;

namespace sharpdungeon.windows
{
    public class WndTabbed : Window
    {
        protected internal List<Tab> Tabs = new List<Tab>();
        protected internal Tab Selected;

        public WndTabbed()
            : base(0, 0, sharpdungeon.Chrome.Get(sharpdungeon.Chrome.Type.TAB_SET))
        {
        }

        protected internal virtual Tab Add(Tab tab)
        {

            tab.SetPos(Tabs.Count == 0 ? -Chrome.MarginLeft() + 1 : Tabs[Tabs.Count - 1].Right(), Height);
            tab.Select(false);
            base.Add(tab);

            Tabs.Add(tab);

            return tab;
        }

        public virtual void Select(int index)
        {
            Select(Tabs[index]);
        }

        public virtual void Select(Tab tab)
        {
            if (tab == Selected)
                return;

            foreach (var t in Tabs)
            {
                if (t == Selected)
                    t.Select(false);
                else
                    if (t == tab)
                        t.Select(true);
            }

            Selected = tab;
        }

        public override void Resize(int w, int h)
        {
            Width = w;
            Height = h;

            Chrome.Size(Width + Chrome.MarginHor(), Height + Chrome.MarginVer());

            Camera.Resize((int)Chrome.Width, Chrome.MarginTop() + Height + TabHeight());
            Camera.X = (int)(Game.Width - Camera.ScreenWidth) / 2;
            Camera.Y = (int)(Game.Height - Camera.ScreenHeight) / 2;

            foreach (var tab in Tabs)
                Remove(tab);

            var tabList = new List<Tab>(Tabs);
            Tabs.Clear();

            foreach (var tab in tabList)
                Add(tab);
        }

        protected internal virtual int TabHeight()
        {
            return 25;
        }

        protected internal virtual void OnClick(Tab tab)
        {
            Select(tab);
        }

        public class Tab : Button
        {
            private readonly WndTabbed _owner;

            public Tab(WndTabbed owner)
            {
                _owner = owner;
            }

            protected internal readonly int Cut = 5;

            protected internal bool Selected;

            protected internal NinePatch Bg;

            protected override void Layout()
            {
                base.Layout();

                if (Bg == null)
                    return;

                Bg.X = X;
                Bg.Y = Y;

                Bg.Size(Width, Height);
            }

            public Action<Tab> SelectAction { get; set; }

            protected internal virtual void Select(bool value)
            {
                Active = !(Selected = value);

                if (Bg != null)
                    Remove(Bg);

                Bg = sharpdungeon.Chrome.Get(Selected ? sharpdungeon.Chrome.Type.TAB_SELECTED : sharpdungeon.Chrome.Type.TAB_UNSELECTED);
                AddToBack(Bg);

                Layout();

                if (SelectAction != null)
                    SelectAction(this);
            }

            protected override void OnClick()
            {
                Sample.Instance.Play(Assets.SND_CLICK, 0.7f, 0.7f, 1.2f);
                _owner.OnClick(this);
            }
        }

        protected internal class LabeledTab : Tab
        {
            private BitmapText _btLabel;

            public LabeledTab(WndTabbed owner, string label)
                : base(owner)
            {
                _btLabel.Text(label);
                _btLabel.Measure();
            }

            protected override void CreateChildren()
            {
                base.CreateChildren();

                _btLabel = PixelScene.CreateText(9);
                
                Add(_btLabel);
            }

            protected override void Layout()
            {
                base.Layout();
                
                _btLabel.X = PixelScene.Align(X + (Width - _btLabel.Width) / 2);
                _btLabel.X = PixelScene.Align(Y + (Height - _btLabel.BaseLine()) / 2) - 1;
                
                if (!Selected)
                    _btLabel.Y -= 2;
            }

            protected internal override void Select(bool value)
            {
                base.Select(value);
                _btLabel.Am = Selected ? 1.0f : 0.6f;
            }
        }
    }
}