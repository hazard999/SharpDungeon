using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.missiles
{
    public class IncendiaryDart : MissileWeapon
    {
        public IncendiaryDart()
            : this(1)
        {
            name = "incendiary dart";
            image = ItemSpriteSheet.INCENDIARY_DART;

            Str = 12;

            Min = 1;
            Max = 2;
        }

        public IncendiaryDart(int number)
        {
            quantity = number;
        }

        protected override void OnThrow(int cell)
        {
            var enemy = Actor.FindChar(cell);
            if (enemy == null || enemy == CurUser)
            {
                if (Level.flamable[cell])
                    GameScene.Add(Blob.Seed(cell, 4, typeof(Fire)));
                else
                    base.OnThrow(cell);
            }
            else
            {
                if (!CurUser.Shoot(enemy, this))
                    Dungeon.Level.Drop(this, cell).Sprite.Drop();
            }
        }

        public override void Proc(Character attacker, Character defender, int damage)
        {
            Buff.Affect<Burning>(defender).Reignite(defender);
            base.Proc(attacker, defender, damage);
        }

        public override string Desc()
        {
            return "The spike on each of these darts is designed to pin it to its target " + "while the unstable compounds strapped to its length burst into brilliant flames.";
        }

        public override Item Random()
        {
            quantity = pdsharp.utils.Random.Int(3, 6);
            return this;
        }

        public override int Price()
        {
            return 10 * Quantity();
        }
    }
}