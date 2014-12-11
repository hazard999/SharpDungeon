using Android.Content;
using Java.IO;
using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.items;
using sharpdungeon.items.rings;

namespace sharpdungeon
{
    public class Bones
    {
        private const string BonesFile = "bones.dat";

        private const string Level = "Level";
        private const string Item = "item";

        private static int _depth = -1;
        private static Item _item;

        public static void Leave()
        {
            _item = null;
            switch (Random.Int(4))
            {
                case 0:
                    _item = Dungeon.Hero.Belongings.Weapon;
                    break;
                case 1:
                    _item = Dungeon.Hero.Belongings.Armor;
                    break;
                case 2:
                    _item = Dungeon.Hero.Belongings.Ring1;
                    break;
                case 3:
                    _item = Dungeon.Hero.Belongings.Ring2;
                    break;
            }

            if (_item == null)
            {
                if (Dungeon.Gold > 0)
                    _item = new Gold(Random.IntRange(1, Dungeon.Gold));
                else
                    _item = new Gold(1);
            }

            _depth = Dungeon.Depth;

            var bundle = new Bundle();
            bundle.Put(Level, _depth);
            bundle.Put(Item, _item);

            try
            {
                var output = Game.Instance.OpenFileOutput(BonesFile, FileCreationMode.Private);
                Bundle.Write(bundle, output);
                output.Close();
            }
            catch (IOException)
            {

            }
        }

        public static Item Get()
        {
#if Console
            return null;
#endif
            if (_depth == -1)
            {
                try
                {
                    var input = Game.Instance.OpenFileInput(BonesFile);
                    var bundle = Bundle.Read(input);
                    input.Close();

                    _depth = bundle.GetInt(Level);
                    _item = (Item)bundle.Get(Item);

                    return Get();
                }
                catch (IOException)
                {
                    return null;
                }
            }

            if (_depth != Dungeon.Depth) 
                return null;

            Game.Instance.DeleteFile(BonesFile);
            _depth = 0;

            if (!_item.Stackable)
            {
                _item.cursed = true;
                _item.cursedKnown = true;
                if (_item.Upgradable)
                {
                    var lvl = (Dungeon.Depth - 1) * 3 / 5 + 1;
                    
                    if (lvl < _item.level)
                        _item.Degrade(_item.level - lvl);

                    _item.levelKnown = false;
                }
            }

            var ring = _item as Ring;
            if (ring != null)
                ring.SyncGem();

            return _item;
        }
    }
}