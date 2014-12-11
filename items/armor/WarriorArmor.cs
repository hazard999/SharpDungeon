using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.levels;
using sharpdungeon.mechanics;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items.armor
{
    public class WarriorArmor : ClassArmor
    {
        public static int LeapTime = 1;
        public static int ShockTime = 3;

        private const string AcSpecial = "HEROIC LEAP";

        private const string TxtNotWarrior = "Only warriors can use this armor!";

        public WarriorArmor()
        {
            name = "warrior suit of armor";
            image = ItemSpriteSheet.ARMOR_WARRIOR;
            Leaper = new WarriorArmorLeaper(this);
        }

        public override string Special()
        {
            return AcSpecial;
        }

        public override void DoSpecial()
        {
            GameScene.SelectCell(Leaper);
        }

        public override bool DoEquip(Hero hero)
        {
            if (hero.heroClass == HeroClass.Warrior)
                return base.DoEquip(hero);

            GLog.Warning(TxtNotWarrior);
            return false;
        }

        public override string Desc()
        {
            return "While this armor looks heavy, it allows a warrior to perform heroic leap towards " + "a targeted location, slamming down to stun All neighbouring enemies.";
        }

        protected static CellSelector.Listener Leaper;
    }

    public class WarriorArmorLeaper : CellSelector.Listener, ICallback
    {
        private readonly WarriorArmor _warriorArmor;
        private int _dest;

        public WarriorArmorLeaper(WarriorArmor warriorArmor)
        {
            _warriorArmor = warriorArmor;
        }

        public void OnSelect(int? target)
        {
            if (target == null || target == _warriorArmor.CurUser.pos)
                return;

            target = Ballistica.Cast(_warriorArmor.CurUser.pos, target.Value, false, true);
            if (Actor.FindChar(target.Value) != null && target != _warriorArmor.CurUser.pos)
                target = Ballistica.Trace[Ballistica.Distance - 2];

            _warriorArmor.CurUser.HP -= (_warriorArmor.CurUser.HP / 3);
            if (_warriorArmor.CurUser.subClass == HeroSubClass.BERSERKER && _warriorArmor.CurUser.HP <= _warriorArmor.CurUser.HT * Fury.Level)
                Buff.Affect<Fury>(_warriorArmor.CurUser);

            Invisibility.Dispel();

            _dest = target.Value;
            _warriorArmor.CurUser.Busy();
            ((HeroSprite)_warriorArmor.CurUser.Sprite).Jump(_warriorArmor.CurUser.pos, target.Value, this);
        }

        public string Prompt()
        {
            return "Choose direction to leap";
        }

        public void Call()
        {
            _warriorArmor.CurUser.Move(_dest);
            Dungeon.Level.Press(_dest, _warriorArmor.CurUser);
            Dungeon.Observe();
            foreach (var neighbour in Level.NEIGHBOURS8)
            {
                var mob = Actor.FindChar(_warriorArmor.CurUser.pos + neighbour);
                if (mob != null && mob != _warriorArmor.CurUser)
                    Buff.Prolong<Paralysis>(mob, WarriorArmor.ShockTime);
            }
            CellEmitter.Center(_dest).Burst(Speck.Factory(Speck.DUST), 10);
            Camera.Main.Shake(2, 0.5f);
            _warriorArmor.CurUser.SpendAndNext(WarriorArmor.LeapTime);
        }
    }
}