using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Shielded : Brute
    {
        public Shielded()
        {
            Name = "shielded brute";
            SpriteClass = typeof(ShieldedSprite);
            defenseSkill = 20;
        }

        public override int Dr()
        {
            return 10;
        }

        public override string DefenseVerb()
        {
            return "blocked";
        }

        public override void Die(object cause)
        {
            base.Die(cause);
            Badge.ValidateRare(this);
        }
    }
}