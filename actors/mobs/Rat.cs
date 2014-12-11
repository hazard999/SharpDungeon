using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Rat : Mob
    {
        public Rat()
        {
            Name = "marsupial rat";
            SpriteClass = typeof(RatSprite);

            HP = HT = 8;
            defenseSkill = 3;

            MaxLvl = 5;
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(1, 5);
        }

        public override int AttackSkill(Character target)
        {
            return 8;
        }

        public override int Dr()
        {
            return 1;
        }

        public override void Die(object cause)
        {
            Ghost.Quest.Process(pos);

            base.Die(cause);
        }

        public override string Description()
        {
            return "Marsupial rats are aggressive, but rather weak denizens " + "of the sewers. They can be dangerous only in big numbers.";
        }
    }

}