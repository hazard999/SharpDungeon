using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items.food;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Crab : Mob
    {
        public Crab()
        {
            Name = "sewer crab";
            SpriteClass = typeof(CrabSprite);

            HP = HT = 15;
            defenseSkill = 5;
            baseSpeed = 2f;

            Exp = 3;
            MaxLvl = 9;

            loot = new MysteryMeat();
            lootChance = 0.167f;
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(3, 6);
        }

        public override int AttackSkill(Character target)
        {
            return 12;
        }

        public override int Dr()
        {
            return 4;
        }

        public override string DefenseVerb()
        {
            return "parried";
        }

        public override void Die(object cause)
        {
            Ghost.Quest.Process(pos);
            base.Die(cause);
        }

        public override string Description()
        {
            return "These huge crabs are at the top of the food chain in the sewers. " + "They are extremely fast and their thick exoskeleton can withstand " + "heavy blows.";
        }
    }
}