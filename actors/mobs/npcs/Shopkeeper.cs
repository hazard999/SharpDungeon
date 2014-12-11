using System.Linq;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.windows;

namespace sharpdungeon.actors.mobs.npcs
{
    public class Shopkeeper : NPC
    {
        public Shopkeeper()
        {
            Name = "shopkeeper";
            SpriteClass = typeof(ShopkeeperSprite);
            _itemSelector = new ShopkeeperListener(this);
        }

        protected override bool Act()
        {
            ThrowItem();

            Sprite.TurnTo(pos, Dungeon.Hero.pos);
            Spend(Tick);
            return true;
        }

        public override void Damage(int dmg, object src)
        {
            Flee();
        }

        public override void Add(Buff buff)
        {
            Flee();
        }

        protected internal virtual void Flee()
        {
            foreach (var heap in Dungeon.Level.heaps.Values.Where(heap => heap.HeapType == Heap.Type.ForSale))
            {
                CellEmitter.Get(heap.Pos).Burst(ElmoParticle.Factory, 4);
                heap.Destroy();
            }

            Destroy();

            Sprite.KillAndErase();
            CellEmitter.Get(pos).Burst(ElmoParticle.Factory, 6);
        }

        public override bool Reset()
        {
            return true;
        }

        public override string Description()
        {
            return "This stout guy looks more appropriate for a trade district in some large city " +
                "than for a dungeon. His prices explain why he prefers to do business here.";
        }

        public WndBag Sell()
        {
            return GameScene.SelectItem(_itemSelector, WndBag.Mode.FOR_SALE, "Select an item to sell");
        }

        private readonly WndBag.Listener _itemSelector;

        public override void Interact()
        {
            Sell();
        }
    }

    internal class ShopkeeperListener : WndBag.Listener
    {
        private readonly Shopkeeper _keeper;

        public ShopkeeperListener(Shopkeeper keeper)
        {
            _keeper = keeper;
        }

        public void OnSelect(Item item)
        {
            if (item == null)
                return;

            var parentWnd = _keeper.Sell();
            GameScene.Show(new WndTradeItem(_keeper, item, parentWnd));
        }
    }
}