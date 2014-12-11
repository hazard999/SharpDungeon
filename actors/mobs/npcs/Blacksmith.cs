using System.Collections.Generic;
using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.items;
using sharpdungeon.items.quest;
using sharpdungeon.items.scrolls;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.windows;
using pdsharp.utils;

namespace sharpdungeon.actors.mobs.npcs
{
    public class Blacksmith : NPC
    {
        private const string TXT_GOLD_1 = "Hey human! Wanna be useful, eh? Take dis pickaxe and mine me some _dark gold ore_, _15 pieces_ should be enough. " +
            "What do you mean, how am I gonna pay? You greedy...\\Negative" +
            "Ok, ok, I don't have money to pay, but I can do some smithin' for you. Consider yourself lucky, " + "I'm the only blacksmith around.";
        private const string TXT_BLOOD_1 = "Hey human! Wanna be useful, eh? Take dis pickaxe and _kill a bat_ wit' it, I need its blood on the head. " +
            "What do you mean, how am I gonna pay? You greedy...\\Negative" +
            "Ok, ok, I don't have money to pay, but I can do some smithin' for you. Consider yourself lucky, " + "I'm the only blacksmith around.";
        private const string TXT2 = "Are you kiddin' me? Where is my pickaxe?!";
        private const string TXT3 = "Dark gold ore. 15 pieces. Seriously, is it dat hard?";
        private const string TXT4 = "I said I need bat blood on the pickaxe. Chop chop!";
        private const string TXT_COMPLETED = "Oh, you have returned... Better late dan never.";
        private const string TXT_GET_LOST = "I'm busy. Get lost!";

        private const string TXT_LOOKS_BETTER = "your {0} certainly looks better now";

        public Blacksmith()
        {
            Name = "troll blacksmith";
            SpriteClass = typeof(BlacksmithSprite);
        }

        protected override bool Act()
        {
            ThrowItem();
            return base.Act();
        }

        public override void Interact()
        {
            Sprite.TurnTo(pos, Dungeon.Hero.pos);

            if (!Quest.given)
            {
                var wndQuest = new WndQuest(this, Quest.alternative ? TXT_BLOOD_1 : TXT_GOLD_1);
                wndQuest.BackPressedAction = () =>
                {
                    Quest.given = true; 
                    Quest.completed = false; 
                    var pick = new Pickaxe();
                    if (pick.DoPickUp(Dungeon.Hero))
                        GLog.Information(Hero.TxtYouNowHave, pick.Name);
                    else
                        Dungeon.Level.Drop(pick, Dungeon.Hero.pos).Sprite.Drop();
                };
                GameScene.Show(wndQuest);

                Journal.Add(Journal.Feature.TROLL);
            }
            else
                if (!Quest.completed)
                {
                    if (Quest.alternative)
                    {
                        var pick = Dungeon.Hero.Belongings.GetItem<Pickaxe>();
                        if (pick == null)
                            Tell(TXT2);
                        else
                            if (!pick.BloodStained)
                                Tell(TXT4);
                            else
                            {
                                if (pick.IsEquipped(Dungeon.Hero))
                                    pick.DoUnequip(Dungeon.Hero, false);

                                pick.Detach(Dungeon.Hero.Belongings.Backpack);
                                Tell(TXT_COMPLETED);

                                Quest.completed = true;
                                Quest.reforged = false;
                            }
                    }
                    else
                    {
                        var pick = Dungeon.Hero.Belongings.GetItem<Pickaxe>();
                        var gold = Dungeon.Hero.Belongings.GetItem<DarkGold>();

                        if (pick == null)
                            Tell(TXT2);
                        else
                            if (gold == null || gold.Quantity() < 15)
                                Tell(TXT3);
                            else
                            {
                                if (pick.IsEquipped(Dungeon.Hero))
                                    pick.DoUnequip(Dungeon.Hero, false);

                                pick.Detach(Dungeon.Hero.Belongings.Backpack);
                                gold.DetachAll(Dungeon.Hero.Belongings.Backpack);
                                Tell(TXT_COMPLETED);

                                Quest.completed = true;
                                Quest.reforged = false;
                            }
                    }
                }
                else
                    if (!Quest.reforged)
                        GameScene.Show(new WndBlacksmith(this, Dungeon.Hero));
                    else
                        Tell(TXT_GET_LOST);
        }

        private void Tell(string text)
        {
            GameScene.Show(new WndQuest(this, text));
        }

        public static string Verify(Item item1, Item item2)
        {
            if (item1 == item2)
                return "Select 2 different items, not the same item twice!";

            if (item1.GetType() != item2.GetType())
                return "Select 2 items of the same type!";

            if (!item1.Identified || !item2.Identified)
                return "I need to know what I'm working with, identify them first!";

            if (item1.cursed || item2.cursed)
                return "I don't work with cursed items!";

            if (item1.level < 0 || item2.level < 0)
                return "It's a junk, the quality is too poor!";

            if (!item1.Upgradable || !item2.Upgradable)
                return "I can't reforge these items!";

            return null;
        }

        public void Upgrade(Item item1, Item item2)
        {
            Item first, second;
            if (item2.level > item1.level)
            {
                first = item2;
                second = item1;
            }
            else
            {
                first = item1;
                second = item2;
            }

            Sample.Instance.Play(Assets.SND_EVOKE);
            ScrollOfUpgrade.Upgrade(Dungeon.Hero);
            Item.Evoke(Dungeon.Hero);

            if (first.IsEquipped(Dungeon.Hero))
                ((EquipableItem)first).DoUnequip(Dungeon.Hero, true);

            first.Upgrade();
            GLog.Positive(TXT_LOOKS_BETTER, first.Name);
            Dungeon.Hero.SpendAndNext(2f);
            Badge.ValidateItemLevelAquired(first);

            if (second.IsEquipped(Dungeon.Hero))
                ((EquipableItem)second).DoUnequip(Dungeon.Hero, false);

            second.DetachAll(Dungeon.Hero.Belongings.Backpack);

            Quest.reforged = true;

            Journal.Remove(Journal.Feature.TROLL);
        }

        public override int DefenseSkill(Character localEnemy)
        {
            return 1000;
        }

        public override void Damage(int dmg, object src)
        {
        }

        public override void Add(Buff buff)
        {
        }

        public override bool Reset()
        {
            return true;
        }

        public override string Description()
        {
            return "This troll blacksmith looks like All trolls look: he is tall and lean, and his skin resembles stone " + "in both color and texture. The troll blacksmith is tinkering with unproportionally small tools.";
        }

        public class Quest
        {
            private static bool _spawned;

            public static bool alternative { get; set; }
            public static bool given { get; set; }
            public static bool completed { get; set; }
            public static bool reforged { get; set; }

            public static void Reset()
            {
                _spawned = false;
                given = false;
                completed = false;
                reforged = false;
            }

            private const string Node = "blacksmith";

            private const string Spawned = "spawned";
            private const string Alternative = "alternative";
            private const string Given = "given";
            private const string Completed = "completed";
            private const string Reforged = "reforged";


            public static void StoreInBundle(Bundle bundle)
            {
                var node = new Bundle();

                node.Put(Spawned, _spawned);

                if (_spawned)
                {
                    node.Put(Alternative, alternative);
                    node.Put(Given, given);
                    node.Put(Completed, completed);
                    node.Put(Reforged, reforged);
                }

                bundle.Put(Node, node);
            }

            public static void RestoreFromBundle(Bundle bundle)
            {
                var node = bundle.GetBundle(Node);

                if (!node.IsNull && (_spawned = node.GetBoolean(Spawned)))
                {
                    alternative = node.GetBoolean(Alternative);
                    given = node.GetBoolean(Given);
                    completed = node.GetBoolean(Completed);
                    reforged = node.GetBoolean(Reforged);
                }
                else
                    Reset();
            }

            public static void Spawn(ICollection<Room> rooms)
            {
                if (_spawned || Dungeon.Depth <= 11 || Random.Int(15 - Dungeon.Depth) != 0)
                    return;

                foreach (var r in rooms)
                {
                    if (r.type != RoomType.STANDARD || r.Width() <= 4 || r.Height() <= 4)
                        continue;

                    var blacksmith = r;
                    blacksmith.type = RoomType.BLACKSMITH;

                    _spawned = true;
                    alternative = Random.Int(2) == 0;

                    given = false;

                    break;
                }
            }
        }
    }
}