using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Gnoll : Mob
    {
        public Gnoll()
        {
            Name = "gnoll scout";
            SpriteClass = typeof(GnollSprite);

            HP = HT = 12;
            defenseSkill = 4;

            Exp = 2;
            MaxLvl = 8;

            loot = new Gold();
            lootChance = 0.5f;
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(2, 5);
        }

        public override int AttackSkill(Character target)
        {
            return 11;
        }

        public override int Dr()
        {
            return 2;
        }

        public override void Die(object cause)
        {
            Ghost.Quest.Process(pos);
            base.Die(cause);
        }

        public override string Description()
        {
            return "Gnolls are hyena-like humanoids. They dwell in sewers and dungeons, venturing up to raid the surface from time to time. " + "Gnoll scouts are regular members of their pack, they are not as strong as brutes and not as intelligent as shamans.";
        }
    }
}