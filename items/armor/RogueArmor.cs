using System.Linq;
using pdsharp.noosa.audio;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.items.wands;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items.armor
{
    public class RogueArmor : ClassArmor, CellSelector.Listener
    {
        private const string TXT_FOV = "You can only jump to an empty location in your field of view";
        private const string TxtNotRogue = "Only rogues can use this armor!";

        private const string AcSpecial = "SMOKE BOMB";

        public RogueArmor()
        {
            name = "rogue garb";
            image = ItemSpriteSheet.ARMOR_ROGUE;

            Teleporter = this;
        }

        public override string Special()
        {
            return AcSpecial;
        }

        public override void DoSpecial()
        {
            GameScene.SelectCell(Teleporter);
        }

        public override bool DoEquip(Hero hero)
        {
            if (hero.heroClass == HeroClass.Rogue)
                return base.DoEquip(hero);

            GLog.Warning(TxtNotRogue);
            return false;
        }

        public override string Desc()
        {
            return "Wearing this dark garb, a rogue can perform a trick, that is called \"smoke bomb\" " + "(though no real explosives are used): he blinds enemies who could see him and jumps aside.";
        }

        protected static CellSelector.Listener Teleporter;
        public void OnSelect(int? target)
        {
            if (target == null)
                return;

            if (!Level.fieldOfView[target.Value] || !(Level.passable[target.Value] || Level.avoid[target.Value]) || Actor.FindChar(target.Value) != null)
            {
                GLog.Warning(TXT_FOV);
                return;
            }

            CurUser.HP -= (CurUser.HP / 3);

            foreach (var mob in Dungeon.Level.mobs.Where(mob => Level.fieldOfView[mob.pos]))
            {
                Buff.Prolong<Blindness>(mob, 2);
                mob.State = mob.WANDERING;
                mob.Sprite.Emitter().Burst(Speck.Factory(Speck.LIGHT), 4);
            }

            WandOfBlink.Appear(CurUser, target.Value);
            CellEmitter.Get(target.Value).Burst(Speck.Factory(Speck.WOOL), 10);
            Sample.Instance.Play(Assets.SND_PUFF);
            Dungeon.Level.Press(target.Value, CurUser);
            Dungeon.Observe();

            CurUser.SpendAndNext(Actor.Tick);
        }

        public string Prompt()
        {
            return "Choose a location to jump to";
        }
    }
}