using pdsharp.noosa.audio;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects.particles;
using sharpdungeon.levels;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items.armor
{
    public class MageArmor : ClassArmor
    {
        private const string AcSpecial = "MOLTEN EARTH";

        private const string TxtNotMage = "Only mages can use this armor!";

        public MageArmor()
        {
            name = "mage robe";
            image = ItemSpriteSheet.ARMOR_MAGE;
        }

        public override string Special()
        {
            return AcSpecial;
        }

        public override string Desc()
        {
            return "Wearing this gorgeous robe, a mage can cast a spell of molten earth: All the enemies " + "in his field of view will be set on fire and unable to move at the same time.";
        }

        public override void DoSpecial()
        {
            foreach (var mob in Dungeon.Level.mobs)
            {
                if (!Level.fieldOfView[mob.pos])
                    continue;

                Buff.Affect<Burning>(mob).Reignite(mob);
                Buff.Prolong<Roots>(mob, 3);
            }

            CurUser.HP -= (CurUser.HP / 3);

            CurUser.Spend(Actor.Tick);
            CurUser.Sprite.DoOperate(CurUser.pos);
            CurUser.Busy();

            CurUser.Sprite.CenterEmitter().Start(ElmoParticle.Factory, 0.15f, 4);
            Sample.Instance.Play(Assets.SND_READ);
        }

        public override bool DoEquip(Hero hero)
        {
            if (hero.heroClass == HeroClass.Mage)
                return base.DoEquip(hero);

            GLog.Warning(TxtNotMage);
            return false;
        }
    }
}