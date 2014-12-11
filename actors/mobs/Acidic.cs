using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Acidic : Scorpio
    {
        public Acidic()
        {
            Name = "acidic scorpio";
            SpriteClass = typeof(AcidicSprite);
        }

        public override int DefenseProc(Character enemy, int damage)
        {
            var dmg = pdsharp.utils.Random.IntRange(0, damage);
            if (dmg > 0)
                enemy.Damage(dmg, this);

            return base.DefenseProc(enemy, damage);
        }

        public override void Die(object cause)
        {
            base.Die(cause);
            Badge.ValidateRare(this);
        }
    }
}