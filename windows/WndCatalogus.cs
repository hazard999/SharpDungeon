using System;
using System.Collections.Generic;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.items;
using sharpdungeon.items.potions;
using sharpdungeon.items.scrolls;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndCatalogus : WndTabbed
    {
        private const int WIDTH = 112;
        private const int HEIGHT = 160;

        private const int ItemHeight = 18;

        private const int TabWidth = 50;

        private const string TxtPotions = "Potions";
        private const string TxtScrolls = "Scrolls";
        private const string TxtTitle = "Catalogus";

        private readonly BitmapText _txtTitle;
        private readonly ScrollPane _list;

        private readonly List<ListItem> _items = new List<ListItem>();

        private bool ShowPotions = true;

        public WndCatalogus()
        {
            Resize(WIDTH, HEIGHT);

            _txtTitle = PixelScene.CreateText(TxtTitle, 9);
            _txtTitle.Hardlight(TitleColor);
            _txtTitle.Measure();
            Add(_txtTitle);

            _list = new ScrollPane(new Component());
            _list.ClickAction = ListClick;
            Add(_list);
            _list.SetRect(0, _txtTitle.Height, WIDTH, HEIGHT - _txtTitle.Height);

            var potionTab = new LabeledTab(this, TxtPotions);
            potionTab.SelectAction = PotionTabSelect;
            Tabs.Add(potionTab);

            var scrollTab = new LabeledTab(this, TxtPotions);
            scrollTab.SelectAction = ScrollTabSelect;
            Tabs.Add(potionTab);

            foreach (var tab in Tabs)
            {
                tab.SetSize(TabWidth, TabHeight());
                Add(tab);
            }

            Select(ShowPotions ? 0 : 1);
        }

        private void ScrollTabSelect(Tab obj)
        {
            ShowPotions = !obj.Selected; 
            UpdateList();
        }

        private void PotionTabSelect(Tab obj)
        {
            ShowPotions = obj.Selected; 
            UpdateList();
        }

        private void ListClick(float x, float y)
        {
            foreach (var item in _items)
                item.OnClick(x, y);
        }

        private void UpdateList()
        {
            _txtTitle.Text(Utils.Format(TxtTitle, ShowPotions ? TxtPotions : TxtScrolls));
            _txtTitle.Measure();
            _txtTitle.X = PixelScene.Align(PixelScene.uiCamera, (WIDTH - _txtTitle.Width) / 2);

            _items.Clear();

            var content = _list.Content();
            content.Clear();
            _list.ScrollTo(0, 0);

            float pos = 0;

            HashSet<Type> known;
            if (ShowPotions)
                known = Potion.GetKnown;
            else
                known = Scroll.GetKnown;

            foreach (var itemClass in known)
            {
                var item = new ListItem(itemClass);
                item.SetRect(0, pos, WIDTH, ItemHeight);
                content.Add(item);
                _items.Add(item);

                pos += item.Height;
            }

            HashSet<Type> unknown;
            if (ShowPotions)
                unknown = Potion.GetUnknown;
            else
                unknown = Scroll.GetUnknown;

            foreach (var itemClass in unknown)
            {
                var item = new ListItem(itemClass);
                item.SetRect(0, pos, WIDTH, ItemHeight);
                content.Add(item);
                _items.Add(item);

                pos += item.Height;
            }

            content.SetSize(WIDTH, pos);
        }
    }

    public class ListItem : Component
    {

        private readonly Item _item;
        private readonly bool _identified;

        private ItemSprite _sprite;
        private BitmapText _label;

        public ListItem(Type itemType)
        {

            try
            {
                _item = (Item)Activator.CreateInstance(itemType);
                _identified = _item.Identified;
                if (_identified)
                {
                    _sprite.View(_item.image, null);
                    _label.Text(_item.Name);
                }
                else
                {
                    _sprite.View(127, null);
                    _label.Text(_item.TrueName());
                    _label.Hardlight(0xCCCCCC);
                }
            }
            catch (Exception)
            {
            }
        }

        protected override void CreateChildren()
        {
            _sprite = new ItemSprite();
            Add(_sprite);

            _label = PixelScene.CreateText(8);
            Add(_label);
        }

        protected override void Layout()
        {
            _sprite.Y = PixelScene.Align(Y + (Height - _sprite.Height) / 2);

            _label.X = _sprite.X + _sprite.Width;
            _label.Y = PixelScene.Align(Y + (Height - _label.BaseLine()) / 2);
        }

        public virtual bool OnClick(float x, float y)
        {
            if (!_identified || !Inside(x, y)) 
                return false;

            GameScene.Show(new WndInfoItem(_item));

            return true;
        }
    }
}