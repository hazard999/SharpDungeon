using System.Collections.Generic;
using pdsharp.noosa.audio;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.items.armor;
using sharpdungeon.items.quest;
using sharpdungeon.items.weapon;
using sharpdungeon.items.weapon.missiles;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.windows;
using System;
using pdsharp.utils;

namespace sharpdungeon.actors.mobs.npcs
{
    public class Ghost : NPC
    {
        private const string TxtRose1 = "Hello adventurer... Once I was like you - strong and confident... " + "And now I'm dead... But I can't leave this place... Not until I have my _dried rose_... " + "It's very important to me... Some monster stole it from my body...";

        private const string TxtRose2 = "Please... Help me... Find the rose...";

        private const string TxtRat1 = "Hello adventurer... Once I was like you - strong and confident... " + "And now I'm dead... But I can't leave this place... Not until I have my revenge... " + "Slay the _fetid rat_, that has taken my life...";

        private const string TxtRat2 = "Please... Help me... Slay the abomination...";

        public Ghost()
        {
            Name = "sad ghost";
            SpriteClass = typeof(GhostSprite);

            Flying = true;

            State = WANDERING;
            Sample.Instance.Load(Assets.SND_GHOST);
        }

        public override int DefenseSkill(Character localEnemy)
        {
            return 1000;
        }

        public override string DefenseVerb()
        {
            return "evaded";
        }

        public override float Speed()
        {
            return 0.5f;
        }

        protected internal override Character ChooseEnemy()
        {
            return Dummy;
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

        public override void Interact()
        {
            ((Character)this).Sprite.TurnTo(pos, Dungeon.Hero.pos);

            Sample.Instance.Play(Assets.SND_GHOST);

            if (Quest.given)
            {
                Item item;

                if (Quest.alternative)
                    item = Dungeon.Hero.Belongings.GetItem<RatSkull>();
                else
                    item = Dungeon.Hero.Belongings.GetItem<DriedRose>();

                if (item != null)
                    GameScene.Show(new WndSadGhost(this, item));
                else
                {
                    GameScene.Show(new WndQuest(this, Quest.alternative ? TxtRat2 : TxtRose2));

                    var newPos = -1;
                    for (var i = 0; i < 10; i++)
                    {
                        newPos = Dungeon.Level.RandomRespawnCell();
                        if (newPos != -1)
                            break;
                    }

                    if (newPos == -1)
                        return;

                    FreeCell(pos);

                    CellEmitter.Get(pos).Start(Speck.Factory(Speck.LIGHT), 0.2f, 3);
                    pos = newPos;
                    ((Character)this).Sprite.Place(pos);
                    ((Character)this).Sprite.Visible = Dungeon.Visible[pos];
                }
            }
            else
            {
                GameScene.Show(new WndQuest(this, Quest.alternative ? TxtRat1 : TxtRose1));
                Quest.given = true;

                Journal.Add(Journal.Feature.GHOST);
            }
        }

        public override string Description()
        {
            return "The ghost is barely visible. It looks like a shapeless " + "spot of faint light with a sorrowful face.";
        }

        private static readonly HashSet<Type> IMMUNITIES = new HashSet<Type>();
        static Ghost()
        {
            IMMUNITIES.Add(typeof(Paralysis));
            IMMUNITIES.Add(typeof(Roots));
            IMMUNITIES.Add(typeof(Paralysis));
        }

        public override HashSet<Type> Immunities()
        {
            return IMMUNITIES;
        }

        public class Quest
        {
            public static bool spawned;

            public static bool alternative;

            public static bool given;

            public static bool processed;

            public static int depth;

            public static int left2kill;

            public static Weapon weapon;
            public static Armor armor;

            public static void reset()
            {
                spawned = false;

                weapon = null;
                armor = null;
            }

            private const string NODE = "sadGhost";

            private const string SPAWNED = "spawned";
            private const string ALTERNATIVE = "alternative";
            private const string LEFT2KILL = "left2kill";
            private const string GIVEN = "given";
            private const string PROCESSED = "processed";
            private const string DEPTH = "depth";
            private const string WEAPON = "weapon";
            private const string ARMOR = "armor";

            public static void StoreInBundle(Bundle bundle)
            {
                var node = new Bundle();

                node.Put(SPAWNED, spawned);

                if (spawned)
                {
                    node.Put(ALTERNATIVE, alternative);
                    if (!alternative)
                        node.Put(LEFT2KILL, left2kill);

                    node.Put(GIVEN, given);
                    node.Put(DEPTH, depth);
                    node.Put(PROCESSED, processed);

                    node.Put(WEAPON, weapon);
                    node.Put(ARMOR, armor);
                }

                bundle.Put(NODE, node);
            }

            public static void RestoreFromBundle(Bundle bundle)
            {
                var node = bundle.GetBundle(NODE);

                if (!node.IsNull && (spawned = node.GetBoolean(SPAWNED)))
                {
                    alternative = node.GetBoolean(ALTERNATIVE);
                    if (!alternative)
                        left2kill = node.GetInt(LEFT2KILL);

                    given = node.GetBoolean(GIVEN);
                    depth = node.GetInt(DEPTH);
                    processed = node.GetBoolean(PROCESSED);

                    weapon = (Weapon)node.Get(WEAPON);
                    armor = (Armor)node.Get(ARMOR);
                }
                else
                    reset();
            }

            public static void Spawn(SewerLevel level)
            {
                if (spawned || Dungeon.Depth <= 1 || pdsharp.utils.Random.Int(5 - Dungeon.Depth) != 0)
                    return;

                var ghost = new Ghost();
                do
                {
                    ghost.pos = level.RandomRespawnCell();
                } while (ghost.pos == -1);

                level.mobs.Add(ghost);
                OccupyCell(ghost);

                spawned = true;
                alternative = pdsharp.utils.Random.Int(2) == 0;
                if (!alternative)
                    left2kill = 8;

                given = false;
                processed = false;
                depth = Dungeon.Depth;

                do
                {
                    weapon = (Weapon)Generator.Random(Generator.Category.WEAPON);
                } while (weapon is MissileWeapon);

                if (Dungeon.IsChallenged(Challenges.NO_ARMOR))
                {
                    armor = (Armor)new ClothArmor().Degrade();
                }
                else
                {
                    armor = (Armor)Generator.Random(Generator.Category.ARMOR);
                }

                for (int i = 0; i < 3; i++)
                {
                    Item another;
                    do
                    {
                        another = Generator.Random(Generator.Category.WEAPON);
                    } while (another is MissileWeapon);
                    if (another.level > weapon.level)
                    {
                        weapon = (Weapon)another;
                    }
                    another = Generator.Random(Generator.Category.ARMOR);
                    if (another.level > armor.level)
                    {
                        armor = (Armor)another;
                    }
                }
                weapon.Identify();
                armor.Identify();
            }

            public static void Process(int pos)
            {
                if (spawned && given && !processed && (depth == Dungeon.Depth))
                {
                    if (alternative)
                    {

                        FetidRat rat = new FetidRat();
                        rat.pos = Dungeon.Level.RandomRespawnCell();
                        if (rat.pos != -1)
                        {
                            GameScene.Add(rat);
                            processed = true;
                        }

                    }
                    else
                    {
                        if (pdsharp.utils.Random.Int(left2kill) == 0)
                        {
                            Dungeon.Level.Drop(new DriedRose(), pos).Sprite.Drop();
                            processed = true;
                        }
                        else
                            left2kill--;
                    }
                }
            }

            public static void Complete()
            {
                weapon = null;
                armor = null;

                Journal.Remove(Journal.Feature.GHOST);
            }
        }

        public class FetidRat : Mob
        {
            public FetidRat()
            {
                Name = "fetid rat";
                SpriteClass = typeof(FetidRatSprite);

                HP = HT = 15;
                defenseSkill = 5;

                Exp = 0;
                MaxLvl = 5;

                State = WANDERING;
            }

            public override int DamageRoll()
            {
                return pdsharp.utils.Random.NormalIntRange(2, 6);
            }

            public override int AttackSkill(Character target)
            {
                return 12;
            }

            public override int Dr()
            {
                return 2;
            }

            public override int DefenseProc(Character enemy, int damage)
            {

                GameScene.Add(Blob.Seed(pos, 20, typeof(ParalyticGas)));

                return base.DefenseProc(enemy, damage);
            }

            public override void Die(object cause)
            {
                base.Die(cause);

                Dungeon.Level.Drop(new RatSkull(), pos).Sprite.Drop();
            }

            public override string Description()
            {
                return "This marsupial rat is much larger, than a regular one. It is surrounded by a foul cloud.";
            }

            public static readonly HashSet<Type> IMMUNITIES = new HashSet<Type>();

            public override HashSet<Type> Immunities()
            {
                return IMMUNITIES;
            }
        }
    }
}