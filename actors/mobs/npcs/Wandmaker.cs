using System.Linq;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.items.bags;
using sharpdungeon.items.potions;
using sharpdungeon.items.quest;
using sharpdungeon.items.wands;
using sharpdungeon.levels;
using sharpdungeon.plants;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.actors.mobs.npcs
{
    public class Wandmaker : NPC
    {
        public Wandmaker()
        {
            Name = "old wandmaker";
            SpriteClass = typeof(WandmakerSprite);
        }

        private const string TxtBerry1 = "Oh, what a pleasant surprise to meet a decent person in such place! I came here for a rare ingredient - " + "a _Rotberry seed_. Being a magic user, I'm quite able to defend myself against local monsters, " + "but I'm getting lost in no time, it's very embarrassing. Probably you could help me? I would be " + "happy to pay for your service with one of my best wands.";

        private const string TxtDust1 = "Oh, what a pleasant surprise to meet a decent person in such place! I came here for a rare ingredient - " + "_corpse dust_. It can be gathered from skeletal remains and there is an ample number of them in the dungeon. " + "Being a magic user, I'm quite able to defend myself against local monsters, but I'm getting lost in no time, " + "it's very embarrassing. Probably you could help me? I would be happy to pay for your service with one of my best wands.";

        private const string TxtBerry2 = "Any luck with a Rotberry seed, {0}? No? Don't worry, I'm not in a hurry.";

        private const string TxtDust2 = "Any luck with corpse dust, {0}? Bone piles are the most obvious places to look.";

        protected override bool Act()
        {
            ThrowItem();
            return base.Act();
        }

        public override int DefenseSkill(Character localEnemy)
        {
            return 1000;
        }

        public override string DefenseVerb()
        {
            return "absorbed";
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
            Sprite.TurnTo(pos, Dungeon.Hero.pos);
            if (Quest.IsGiven)
            {
                Item item;
                if (Quest.Alternative)
                    item = Dungeon.Hero.Belongings.GetItem<CorpseDust>();
                else
                    item = Dungeon.Hero.Belongings.GetItem<Rotberry.Seed>();

                if (item != null)
                    GameScene.Show(new WndWandmaker(this, item));
                else
                    Tell(Quest.Alternative ? TxtDust2 : TxtBerry2, Dungeon.Hero.ClassName());
            }
            else
            {
                Tell(Quest.Alternative ? TxtDust1 : TxtBerry1);
                Quest.IsGiven = true;

                Quest.PlaceItem();

                Journal.Add(Journal.Feature.WANDMAKER);
            }
        }

        private void Tell(string format, params object[] args)
        {
            GameScene.Show(new WndQuest(this, Utils.Format(format, args)));
        }

        public override string Description()
        {
            return "This old but hale gentleman wears a slightly confused " + "expression. He is protected by a magic shield.";
        }

        public class Quest
        {
            public static bool Spawned { get; private set; }

            public static bool Alternative { get; private set; }

            public static bool IsGiven { get; set; }

            public static Wand Wand1;
            public static Wand Wand2;

            public static void Reset()
            {
                Spawned = false;

                Wand1 = null;
                Wand2 = null;
            }

            private const string Node = "wandmaker";

            private const string SPAWNED = "spawned";
            private const string ALTERNATIVE = "alternative";
            private const string GIVEN = "given";
            private const string WAND1 = "wand1";
            private const string WAND2 = "wand2";

            public static void StoreInBundle(Bundle bundle)
            {
                var node = new Bundle();

                node.Put(SPAWNED, Spawned);

                if (Spawned)
                {
                    node.Put(ALTERNATIVE, Alternative);

                    node.Put(GIVEN, IsGiven);

                    node.Put(WAND1, Wand1);
                    node.Put(WAND2, Wand2);
                }

                bundle.Put(Node, node);
            }

            public static void RestoreFromBundle(Bundle bundle)
            {
                var node = bundle.GetBundle(Node);

                if (!node.IsNull && (Spawned = node.GetBoolean(SPAWNED)))
                {
                    Alternative = node.GetBoolean(ALTERNATIVE);

                    IsGiven = node.GetBoolean(GIVEN);

                    Wand1 = (Wand) node.Get(WAND1);
                    Wand2 = (Wand) node.Get(WAND2);
                }
                else
                    Reset();
            }

            public static void Spawn(PrisonLevel level, Room room)
            {
                if (Spawned || Dungeon.Depth <= 6 || Random.Int(10 - Dungeon.Depth) != 0)
                    return;

                var npc = new Wandmaker();
                do
                {
                    npc.pos = room.Random();
                }
                while (level.map[npc.pos] == Terrain.ENTRANCE || level.map[npc.pos] == Terrain.SIGN);

                level.mobs.Add(npc);
                OccupyCell(npc);

                Spawned = true;
                Alternative = Random.Int(2) == 0;

                IsGiven = false;

                switch (Random.Int(5))
                {
                    case 0:
                        Wand1 = new WandOfAvalanche();
                        break;
                    case 1:
                        Wand1 = new WandOfDisintegration();
                        break;
                    case 2:
                        Wand1 = new WandOfFirebolt();
                        break;
                    case 3:
                        Wand1 = new WandOfLightning();
                        break;
                    case 4:
                        Wand1 = new WandOfPoison();
                        break;
                }
                Wand1.Random().Upgrade();

                switch (Random.Int(5))
                {
                    case 0:
                        Wand2 = new WandOfAmok();
                        break;
                    case 1:
                        Wand2 = new WandOfBlink();
                        break;
                    case 2:
                        Wand2 = new WandOfRegrowth();
                        break;
                    case 3:
                        Wand2 = new WandOfSlowness();
                        break;
                    case 4:
                        Wand2 = new WandOfTelekinesis();
                        break;
                }
                Wand2.Random().Upgrade();
            }

            public static void PlaceItem()
            {
                if (Alternative)
                {
                    var candidates = Dungeon.Level.heaps.Values.Where(heap => heap.HeapType == Heap.Type.Skeleton && !Dungeon.Visible[heap.Pos]).ToList();

                    if (candidates.Count > 0)
                        Random.Element(candidates).Drop(new CorpseDust());
                    else
                    {
                        var pos = Dungeon.Level.RandomRespawnCell();

                        while (Dungeon.Level.heaps[pos] != null)
                            pos = Dungeon.Level.RandomRespawnCell();


                        var heap = Dungeon.Level.Drop(new CorpseDust(), pos);
                        heap.HeapType = Heap.Type.Skeleton;
                        heap.Sprite.Link();
                    }
                }
                else
                {

                    var shrubPos = Dungeon.Level.RandomRespawnCell();
                    while (Dungeon.Level.heaps[shrubPos] != null)
                        shrubPos = Dungeon.Level.RandomRespawnCell();

                    Dungeon.Level.Plant(new Rotberry.Seed(), shrubPos);
                }
            }

            public static void Complete()
            {
                Wand1 = null;
                Wand2 = null;

                Journal.Remove(Journal.Feature.WANDMAKER);
            }
        }

        public class Rotberry : Plant
        {
            private const string TxtDesc = "Berries of this shrub taste like sweet, sweet death.";

            public Rotberry()
            {
                Image = 7;
                PlantName = "Rotberry";
            }

            public override void Activate(Character ch)
            {
                base.Activate(ch);

                GameScene.Add(Blob.Seed(Pos, 100, typeof(ToxicGas)));

                Dungeon.Level.Drop(new Seed(), Pos).Sprite.Drop();

                if (ch != null)
                    buffs.Buff.Prolong<Roots>(ch, Tick * 3);
            }

            public override string Desc()
            {
                return TxtDesc;
            }

            public new class Seed : Plant.Seed
            {
                public Seed()
                {
                    plantName = "Rotberry";

                    name = "seed of " + plantName;
                    image = ItemSpriteSheet.SEED_ROTBERRY;

                    PlantClass = typeof(Rotberry);
                    AlchemyClass = typeof(PotionOfStrength);
                }

                public override bool Collect(Bag container)
                {
                    if (!base.Collect(container)) 
                        return false;

                    if (Dungeon.Level == null) 
                        return true;

                    foreach (var mob in Dungeon.Level.mobs)
                        mob.Beckon(Dungeon.Hero.pos);

                    GLog.Warning("The seed emits a roar that echoes throughout the dungeon!");
                    CellEmitter.Center(Dungeon.Hero.pos).Start(Speck.Factory(Speck.SCREAM), 0.3f, 3);
                    Sample.Instance.Play(Assets.SND_CHALLENGE);

                    return true;
                }

                public override string Desc()
                {
                    return TxtDesc;
                }
            }
        }
    }
}