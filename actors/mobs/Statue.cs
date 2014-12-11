using System.Collections.Generic;
using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.items;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.weapon;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.items.weapon.melee;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Statue : Mob
    {
        private Weapon _weapon;

        public Statue()
        {
            Name = "animated statue";
            SpriteClass = typeof(StatueSprite);

            Exp = 0;
            State = PASSIVE;
            do
            {
                _weapon = (Weapon)Generator.Random(Generator.Category.WEAPON);
            } while (!(_weapon is MeleeWeapon) || _weapon.level < 0);

            _weapon.Identify();
            _weapon.Enchant(items.weapon.Weapon.Enchantment.Random());

            HP = HT = 15 + Dungeon.Depth * 5;
            defenseSkill = 4 + Dungeon.Depth;
        }

        private const string Weapon = "weapon";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(Weapon, _weapon);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            _weapon = (Weapon)bundle.Get(Weapon);
        }

        protected override bool Act()
        {
            if (Dungeon.Visible[pos])
                Journal.Add(Journal.Feature.STATUE);

            return base.Act();
        }

        public override int DamageRoll()
        {
            return Random.NormalIntRange(_weapon.Min, _weapon.Max);
        }

        public override int AttackSkill(Character target)
        {
            return (int)((9 + Dungeon.Depth) * _weapon.Acu);
        }

        protected internal override float AttackDelay()
        {
            return _weapon.Dly;
        }

        public override int Dr()
        {
            return Dungeon.Depth;
        }

        public override void Damage(int dmg, object src)
        {
            if (State == PASSIVE)
                State = HUNTING;

            base.Damage(dmg, src);
        }

        public override int AttackProc(Character enemy, int damage)
        {
            _weapon.Proc(this, enemy, damage);
            return damage;
        }

        public override void Beckon(int cell)
        {
        }

        public override void Die(object cause)
        {
            Dungeon.Level.Drop(_weapon, pos).Sprite.Drop();
            base.Die(cause);
        }

        public override void Destroy()
        {
            Journal.Remove(Journal.Feature.STATUE);
            base.Destroy();
        }

        public override bool Reset()
        {
            State = PASSIVE;
            return true;
        }

        public override string Description()
        {
            return "You would think that it's just another ugly statue of this dungeon, but its red glowing eyes give itself away. " + "While the statue itself is made of stone, the _" + _weapon.Name + "_, it's wielding, looks real.";
        }

        private static readonly HashSet<System.Type> RESISTANCES = new HashSet<System.Type>();

        private static readonly HashSet<System.Type> IMMUNITIES = new HashSet<System.Type>();
        static Statue()
        {
            RESISTANCES.Add(typeof(ToxicGas));
            RESISTANCES.Add(typeof(Poison));
            RESISTANCES.Add(typeof(Death));
            RESISTANCES.Add(typeof(ScrollOfPsionicBlast));
            IMMUNITIES.Add(typeof(Leech));
        }

        public override HashSet<System.Type> Resistances()
        {
            return RESISTANCES;
        }

        public override HashSet<System.Type> Immunities()
        {
            return IMMUNITIES;
        }
    }
}