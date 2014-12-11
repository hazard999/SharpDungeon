using System.Collections.Generic;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.items.weapon;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.items.quest
{
    public class Pickaxe : Weapon, ICallback
    {
        public const string AcMine = "MINE";

        public const float TimeToMine = 2;

        private const string TxtNoVein = "There is no dark gold vein near you to mine";

        private static readonly ItemSprite.Glowing Bloody = new ItemSprite.Glowing(0x550000);

        public Pickaxe()
        {
            name = "pickaxe";
            image = ItemSpriteSheet.PICKAXE;

            unique = true;

            DefaultAction = AcMine;

            Str = 14;
            Min = 3;
            Max = 12;
        }

        public bool BloodStained = false;
        private int _pos;

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(AcMine);
            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action == AcMine)
            {
                if (Dungeon.Depth < 11 || Dungeon.Depth > 15)
                {
                    GLog.Warning(TxtNoVein);
                    return;
                }

                foreach (var neighbour in Level.NEIGHBOURS8)
                {
                    _pos = hero.pos + neighbour;

                    if (Dungeon.Level.map[_pos] != Terrain.WALL_DECO)
                        continue;

                    hero.Spend(TimeToMine);
                    hero.Busy();
                    hero.Sprite.DoAttack(_pos, this);
                    return;
                }

                GLog.Warning(TxtNoVein);
            }
            else
                base.Execute(hero, action);
        }

        public override bool Upgradable
        {
            get
            {
                return false;
            }
        }

        public override bool Identified
        {
            get
            {
                return true;
            }
        }

        public override void Proc(Character c, Character defender, int damage)
        {
            if (BloodStained || !(defender is Bat) || (defender.HP > damage))
                return;

            BloodStained = true;
            UpdateQuickslot();
        }

        private const string Bloodstained = "bloodStained";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);

            bundle.Put(Bloodstained, BloodStained);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);

            BloodStained = bundle.GetBoolean(Bloodstained);
        }

        public override ItemSprite.Glowing Glowing()
        {
            return BloodStained ? Bloody : null;
        }

        public override string Info()
        {
            return "This is a large and sturdy tool for breaking rocks. Probably it can be used as a weapon.";
        }

        public void Call()
        {
            CellEmitter.Center(_pos).Burst(Speck.Factory(Speck.STAR), 7);

            Sample.Instance.Play(Assets.SND_EVOKE);

            Level.Set(_pos, Terrain.WALL);
            GameScene.UpdateMap(_pos);
            var gold = new DarkGold();

            if (gold.DoPickUp(Dungeon.Hero))
                GLog.Information(Hero.TxtYouNowHave, gold.Name);
            else
                Dungeon.Level.Drop(gold, Dungeon.Hero.pos).Sprite.Drop();

            var hunger = Dungeon.Hero.Buff<Hunger>();
            if (hunger != null && !hunger.IsStarving)
            {
                hunger.Satisfy(Hunger.Starving / 10 * -1);
                BuffIndicator.RefreshHero();
            }
            Dungeon.Hero.OnOperateComplete();
        }
    }
}