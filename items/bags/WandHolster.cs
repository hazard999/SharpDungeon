using System.Linq;
using sharpdungeon.items.wands;
using sharpdungeon.sprites;

namespace sharpdungeon.items.bags
{
    public class WandHolster : Bag
    {
        public WandHolster()
        {
            name = "wand holster";
            image = ItemSpriteSheet.HOLSTER;

            Size = 12;
        }

        public override bool Grab(Item item)
        {
            return item is Wand;
        }

        public override bool Collect(Bag container)
        {
            if (!base.Collect(container))
                return false;

            if (Owner == null)
                return true;

            foreach (var item in Items.OfType<Wand>())
                item.Charge(Owner);

            return true;
        }

        protected override void OnDetach()
        {
            foreach (var item in Items.OfType<Wand>())
                item.StopCharging();
        }

        public override int Price()
        {
            return 50;
        }

        public override string Info()
        {
            return "This slim holder is made of leather of some exotic animal. " + "It allows to compactly carry up to " + Size + " wands.";
        }
    }
}