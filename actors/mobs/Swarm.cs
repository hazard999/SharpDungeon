using System.Linq;
using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.items.potions;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.levels;
using sharpdungeon.levels.features;

namespace sharpdungeon.actors.mobs
{
    public class Swarm : Mob
    {
        public Swarm()
        {
            Name = "swarm of flies";
            SpriteClass = typeof(SwarmSprite);

            HP = HT = 80;
            defenseSkill = 5;

            MaxLvl = 10;

            Flying = true;
        }

        private const float SplitDelay = 1f;

        internal int Generation = 0;

        private const string GENERATION = "generation";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(GENERATION, Generation);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            Generation = bundle.GetInt(GENERATION);
        }

        public override int DamageRoll()
        {
            return Random.NormalIntRange(1, 4);
        }

        public override int DefenseProc(Character enemy, int damage)
        {
            if (HP < damage + 2) 
                return damage;

            var passable = Level.passable;

            int[] neighbours = { pos + 1, pos - 1, pos + Level.Width, pos - Level.Width };
            var candidates = neighbours.Where(n => passable[n] && FindChar(n) == null).Select(n => n).ToList();

            if (candidates.Count <= 0) 
                return damage;

            Swarm clone = Split();
            clone.HP = (HP - damage) / 2;
            clone.pos = Random.Element(candidates);
            clone.State = clone.HUNTING;

            if (Dungeon.Level.map[clone.pos] == Terrain.DOOR)
                Door.Enter(clone.pos);

            GameScene.Add(clone, SplitDelay);
            AddDelayed(new Pushing(clone, pos, clone.pos), -1);

            HP -= clone.HP;

            return damage;
        }

        public override int AttackSkill(Character target)
        {
            return 12;
        }

        public override string DefenseVerb()
        {
            return "evaded";
        }

        private Swarm Split()
        {
            var clone = new Swarm();
            clone.Generation = Generation + 1;
            
            if (Buff<Burning>() != null)
                buffs.Buff.Affect<Burning>(clone).Reignite(clone);

            if (Buff<Poison>() != null)
                buffs.Buff.Affect<Poison>(clone).Set(2);

            return clone;
        }

        protected internal override void DropLoot()
        {
            if (Random.Int(5*(Generation + 1)) == 0)
                Dungeon.Level.Drop(new PotionOfHealing(), pos).Sprite.Drop();
        }

        public override string Description()
        {
            return "The deadly swarm of flies buzzes angrily. Every non-magical DoAttack " + "will split it into two smaller but equally dangerous swarms.";
        }
    }
}