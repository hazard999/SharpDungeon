using System.Collections.Generic;
using System.Linq;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using sharpdungeon.effects;
using sharpdungeon.scenes;
using sharpdungeon.windows;
using pdsharp.noosa.ui;

namespace sharpdungeon.ui
{
    public class BadgesList : ScrollPane
    {
        private readonly List<ListItem> _items = new List<ListItem>();

        public BadgesList(bool global) : base(new Component())
        {
            foreach (var item in from badge in Badge.Filtered(global) where badge.Image != -1 select new ListItem(badge))
            {
                Content().Add(item);
                _items.Add(item);
            }
        }

        protected override void Layout()
        {
            base.Layout();

            float pos = 0;

            var size = _items.Count;
            for (var i=0; i < size; i++)
            {
                _items[i].SetRect(0, pos, Width, ListItem.HEIGHT);
                pos += ListItem.HEIGHT;
            }

            Content().SetSize(Width, pos);
        }

        public override void OnClick(float x, float y)
        {
            var size = _items.Count;
            
            for (var i = 0; i < size; i++)
                if (_items[i].OnClick(x, y))
                    break;
        }

        private class ListItem : Component
        {
            public const float HEIGHT = 20;

            private readonly Badge _badge;

            private Image _icon;
            private BitmapText _label;

            public ListItem(Badge badge)
            {
                this._badge = badge;
                _icon.Copy(BadgeBanner.Image(badge.Image));
                _label.Text(badge.Description);
            }

            protected override void CreateChildren()
            {
                _icon = new Image();
                Add(_icon);

                _label = PixelScene.CreateText(6);
                Add(_label);
            }

            protected override void Layout()
            {
                _icon.X = X;
                _icon.Y = PixelScene.Align(Y + (Height - _icon.Height) / 2);

                _label.X = _icon.X + _icon.Width + 2;
                _label.Y = PixelScene.Align(Y + (Height - _label.BaseLine()) / 2);
            }

            public virtual bool OnClick(float x, float y)
            {
                if (!Inside(x, y)) 
                    return false;

                Sample.Instance.Play(Assets.SND_CLICK, 0.7f, 0.7f, 1.2f);
                Game.Scene.Add(new WndBadge(_badge));
                return true;
            }
        }
    }
}