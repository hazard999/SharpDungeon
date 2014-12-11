using System.Collections.Generic;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items
{
    public class Gold : Item
    {
        private const string TXT_COLLECT = "Collect gold coins to spend them later in a shop.";
        private const string TXT_INFO = "A pile of {0} gold coins. " + TXT_COLLECT;
        private const string TXT_INFO_1 = "One gold coin. " + TXT_COLLECT;
        private const string TXT_VALUE = "{0}";

        public Gold()
            : this(1)
        {
            name = "gold";
            image = ItemSpriteSheet.GOLD;
            Stackable = true;
        }

        public Gold(int value)
        {
            this.quantity = value;
        }

        public override List<string> Actions(Hero hero)
        {
            return new List<string>();
        }

        public override bool DoPickUp(Hero hero)
        {
            Dungeon.Gold += quantity;
            Statistics.GoldCollected += quantity;
            Badge.ValidateGoldCollected();

            GameScene.PickUp(this);
            hero.Sprite.ShowStatus(CharSprite.Neutral, TXT_VALUE, quantity);
            hero.SpendAndNext(TimeToPickUp);

            Sample.Instance.Play(Assets.SND_GOLD, 1, 1, pdsharp.utils.Random.Float(0.9f, 1.1f));

            return true;
        }

        public override bool Upgradable
        {
            get
            {
                return false;
            }
        }

        public override bool Identified
        {
            get
            {
                return true;
            }
        }

        public override string Info()
        {
            switch (quantity)
            {
                case 0:
                    return TXT_COLLECT;
                case 1:
                    return TXT_INFO_1;
                default:
                    return Utils.Format(TXT_INFO, quantity);
            }
        }

        public override Item Random()
        {
            quantity = pdsharp.utils.Random.Int(20 + Dungeon.Depth * 10, 40 + Dungeon.Depth * 20);
            return this;
        }

        private const string VALUE = "value";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(VALUE, quantity);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            quantity = bundle.GetInt(VALUE);
        }
    }
}