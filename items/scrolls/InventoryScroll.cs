#if !DEBUG
using pdsharp.noosa.audio;
#endif
using sharpdungeon.actors.buffs;
using sharpdungeon.scenes;
using sharpdungeon.windows;

namespace sharpdungeon.items.scrolls
{
    public abstract class InventoryScroll : Scroll
    {
        protected internal string InventoryTitle = "Select an item";
        protected internal WndBag.Mode Mode = WndBag.Mode.ALL;

        private const string TxtWarning = "Do you really want to cancel this scroll usage? It will be consumed anyway.";
        private const string TxtYes = "Yes, I'm positive";
        private const string TxtNo = "No, I changed my mind";

        protected InventoryScroll()
        {
            ItemSelector = new InventoryScrollListener(this);
        }

        protected internal override void DoRead()
        {
            if (!IsKnown)
            {
                SetKnown();
                IdentifiedByUse = true;
            }
            else
                IdentifiedByUse = false;

            GameScene.SelectItem(ItemSelector, Mode, InventoryTitle);
        }

        public void ConfirmCancelation()
        {
            var wndOptions = new WndOptions(Name, TxtWarning, TxtYes, TxtNo);
            wndOptions.SelectAction = index =>
            {
                switch (index)
                {
                    case 0:
                        CurUser.SpendAndNext(TimeToRead);
                        IdentifiedByUse = false;
                        break;
                    case 1:
                        GameScene.SelectItem(ItemSelector, Mode, InventoryTitle);
                        break;
                }
            };
            GameScene.Show(wndOptions);
        }

        protected internal abstract void OnItemSelected(Item item);

        protected internal bool IdentifiedByUse;

        protected static WndBag.Listener ItemSelector;
    }

    public class InventoryScrollListener : WndBag.Listener
    {
        private readonly InventoryScroll _scroll;

        public InventoryScrollListener(InventoryScroll scroll)
        {
            _scroll = scroll;
        }

        public void OnSelect(Item item)
        {
            if (item != null)
            {
                _scroll.OnItemSelected(item);
                _scroll.CurUser.SpendAndNext(Scroll.TimeToRead);
#if !DEBUG
                Sample.Instance.Play(Assets.SND_READ);
#endif
                Invisibility.Dispel();

            }
            else
                if (_scroll.IdentifiedByUse)
                    _scroll.ConfirmCancelation();
                else
                    _scroll.Collect(_scroll.CurUser.Belongings.Backpack);
        }
    }
}