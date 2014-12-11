using System.Collections.Generic;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs;
using sharpdungeon.items.weapon.missiles;
using sharpdungeon.levels;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items.armor
{
    public class HuntressArmor : ClassArmor,ICallback
    {
        private const string TxtNoEnemies = "No enemies in sight";
        private const string TxtNotHuntress = "Only huntresses can use this armor!";

        private const string AcSpecial = "SPECTRAL BLADES";

        public HuntressArmor()
        {
            name = "huntress cloak";
            image = ItemSpriteSheet.ARMOR_HUNTRESS;
        }

        private readonly Dictionary<ICallback, Mob> _targets = new Dictionary<ICallback, Mob>();

        public override string Special()
        {
            return AcSpecial;
        }

        public override void DoSpecial()
        {
            var proto = new Shuriken();

            foreach (var mob in Dungeon.Level.mobs)
            {
                if (!Level.fieldOfView[mob.pos]) 
                    continue;

                CurUser.Sprite.Parent.Recycle<MissileSprite>().Reset(CurUser.pos, mob.pos, proto, this);

                _targets.Add(this, mob);
            }

            if (_targets.Count == 0)
            {
                GLog.Warning(TxtNoEnemies);
                return;
            }

            CurUser.HP -= (CurUser.HP / 3);

            CurUser.Sprite.DoZap(CurUser.pos);
            CurUser.Busy();
        }

        public override bool DoEquip(Hero hero)
        {
            if (hero.heroClass == HeroClass.Huntress)
                return base.DoEquip(hero);

            GLog.Warning(TxtNotHuntress);
            return false;
        }

        public override string Desc()
        {
            return "A huntress in such cloak can create a fan of spectral blades. Each of these blades " + "will target a single enemy in the huntress's field of view, inflicting damage depending " + "on her currently equipped melee weapon.";
        }

        public void Call()
        {
            CurUser.Attack(_targets[this]);
            _targets.Remove(this);
            if (_targets.Count == 0)
                CurUser.SpendAndNext(CurUser.AttackDelay());
        }
    }
}