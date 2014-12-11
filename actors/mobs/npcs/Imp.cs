using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.items;
using sharpdungeon.items.quest;
using sharpdungeon.items.rings;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.actors.mobs.npcs
{
    public class Imp : NPC
    {
        public Imp()
        {
            Name = "ambitious imp";
            SpriteClass = typeof(ImpSprite);
        }

        private const string TxtGolems1 = "Are you an adventurer? I love adventurers! You can always rely on them " + "if something needs to be killed. Am I right? For bounty of course ;)\\Negative" + "In my case this is _golems_ who need to be killed. You see, I'm going to start a " + "little business here, but these stupid golems are bad for business! " + "It's very hard to negotiate with wandering lumps of granite, damn them! " + "So please, kill... let's say _6 of them_ and a reward is yours.";

        private const string TxtMonks1 = "Are you an adventurer? I love adventurers! You can always rely on them " + "if something needs to be killed. Am I right? For bounty of course ;)\\Negative" + "In my case this is _monks_ who need to be killed. You see, I'm going to start a " + "little business here, but these lunatics don't buy anything themselves and " + "will scare away other customers. " + "So please, kill... let's say _8 of them_ and a reward is yours.";

        private const string TxtGolems2 = "How is your golem safari going?";

        private const string TxtMonks2 = "Oh, you are still alive! I knew that your kung-fu is stronger ;) " + "Just don't forget to grab these monks' tokens.";

        private const string TxtCya = "See you, {0}!";
        private const string TxtHey = "Psst, {0}!";

        private bool _seenBefore;

        protected override bool Act()
        {
            if (!Quest.Given && Dungeon.Visible[pos])
            {
                if (!_seenBefore)
                    Yell(Utils.Format(TxtHey, Dungeon.Hero.ClassName()));

                _seenBefore = true;
            }
            else
                _seenBefore = false;

            ThrowItem();

            return base.Act();
        }

        public override int DefenseSkill(Character localEnemy)
        {
            return 1000;
        }

        public override string DefenseVerb()
        {
            return "evaded";
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
            if (Quest.Given)
            {
                var tokens = Dungeon.Hero.Belongings.GetItem<DwarfToken>();
                if (tokens != null && (tokens.Quantity() >= 8 || (!Quest.Alternative && tokens.Quantity() >= 6)))
                    GameScene.Show(new WndImp(this, tokens));
                else
                    Tell(Quest.Alternative ? TxtMonks2 : TxtGolems2, Dungeon.Hero.ClassName());
            }
            else
            {
                Tell(Quest.Alternative ? TxtMonks1 : TxtGolems1);
                Quest.Given = true;
                Quest.IsCompleted = false;

                Journal.Add(Journal.Feature.IMP);
            }
        }

        private void Tell(string format, params object[] args)
        {
            GameScene.Show(new WndQuest(this, Utils.Format(format, args)));
        }

        public virtual void Flee()
        {
            Yell(Utils.Format(TxtCya, Dungeon.Hero.ClassName()));

            Destroy();
            Sprite.DoDie();
        }

        public override string Description()
        {
            return "Imps are lesser demons. They are notable for neither their strength nor their magic talent, " + "but they are quite smart and sociable. Many imps prefer to live among non-demons.";
        }

        public class Quest
        {
            public static bool Alternative { get; private set; }
            public static bool Spawned { get; private set; }
            public static bool Given { get; set; }

            public static Ring reward;

            public static void Reset()
            {
                Spawned = false;

                reward = null;
            }

            private const string NODE = "demon";

            private const string ALTERNATIVE = "alternative";
            private const string SPAWNED = "spawned";
            private const string GIVEN = "given";
            private const string COMPLETED = "completed";
            private const string REWARD = "reward";

            public static void StoreInBundle(Bundle bundle)
            {
                var node = new Bundle();

                node.Put(SPAWNED, Spawned);

                if (Spawned)
                {
                    node.Put(ALTERNATIVE, Alternative);

                    node.Put(GIVEN, Given);
                    node.Put(COMPLETED, IsCompleted);
                    node.Put(REWARD, reward);
                }

                bundle.Put(NODE, node);
            }

            public static void RestoreFromBundle(Bundle bundle)
            {
                var node = bundle.GetBundle(NODE);

                if (node.IsNull || (!(Spawned = node.GetBoolean(SPAWNED)))) 
                    return;

                Alternative = node.GetBoolean(ALTERNATIVE);

                Given = node.GetBoolean(GIVEN);
                IsCompleted = node.GetBoolean(COMPLETED);
                reward = (Ring)node.Get(REWARD);
            }

            public static void Spawn(CityLevel level, Room room)
            {
                if (Spawned || Dungeon.Depth <= 16 || Random.Int(20 - Dungeon.Depth) != 0) 
                    return;
            
                var npc = new Imp();
                do
                {
                    npc.pos = level.RandomRespawnCell();
                } 
                while (npc.pos == -1 || level.heaps[npc.pos] != null);
                
                level.mobs.Add(npc);
                OccupyCell(npc);

                Spawned = true;
                Alternative = Random.Int(2) == 0;

                Given = false;

                do
                {
                    reward = (Ring)Generator.Random(Generator.Category.RING);
                } 
                while (reward.cursed);

                reward.Upgrade(2);
                reward.cursed = true;
            }

            public static void Process(Mob mob)
            {
                if (Spawned && Given && !IsCompleted)
                {
                    if ((Alternative && mob is Monk) || (!Alternative && mob is Golem))
                    {

                        Dungeon.Level.Drop(new DwarfToken(), mob.pos).Sprite.Drop();
                    }
                }
            }

            public static void Complete()
            {
                reward = null;
                IsCompleted = true;

                Journal.Remove(Journal.Feature.IMP);
            }

            public static bool IsCompleted { get; set; }
        }
    }
}