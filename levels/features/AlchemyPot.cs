using sharpdungeon.actors.hero;
using sharpdungeon.items;
using sharpdungeon.scenes;
using sharpdungeon.windows;

namespace sharpdungeon.levels.features
{
    public class AlchemyPot
    {
        private const string TxtSelectSeed = "Select a seed to throw";

        private static Hero _hero;
        private static int _pos;

        public static void Operate(Hero hero, int pos)
        {
            _hero = hero;
            _pos = pos;

            GameScene.SelectItem(new AlchemyPotItemSelector(), WndBag.Mode.SEED, TxtSelectSeed);
        }

        public class AlchemyPotItemSelector : WndBag.Listener
        {
            public void OnSelect(Item item)
            {
                if (item != null)
                    item.Cast(_hero, _pos);
            }
        }
    }
}