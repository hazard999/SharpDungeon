using System;
using System.Collections.Generic;
using System.Linq;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.mobs;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.items.armor;
using sharpdungeon.items.keys;
using sharpdungeon.items.potions;
using sharpdungeon.items.rings;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.wands;
using sharpdungeon.items.weapon.melee;
using sharpdungeon.items.weapon.missiles;
using sharpdungeon.levels;
using sharpdungeon.levels.features;
using sharpdungeon.plants;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;
using sharpdungeon.windows;
using Random = pdsharp.utils.Random;

namespace sharpdungeon.actors.hero
{
    public class Hero : Character
    {
        private const string TxtLeave = "One does not simply leave Pixel Dungeon.";

        private const string TxtLevelUp = "Level up!";
        private const string TxtNewLevel = "Welcome to Level {0}! Now you are healthier and more focused. " + "It's easier for you to hit enemies and dodge their attacks.";

        public const string TxtYouNowHave = "You now have {0}";

        private const string TxtSomethingElse = "There is something else here";
        private const string TxtLockedChest = "This chest is locked and you don't have matching key";
        private const string TxtLockedDoor = "You don't have a matching key";
        private const string TxtNoticedSmth = "You noticed something";

        private const string TxtWait = "...";
        private const string TxtSearch = "search";

        public const int StartingStr = 10;

        private const float TimeToRest = 1f;
        private const float TimeToSearch = 2f;

        public HeroClass heroClass = HeroClass.Rogue;
        public HeroSubClass subClass = HeroSubClass.NONE;

        private int _attackSkill = 10;
        private int _defenseSkill = 5;

        public bool ready = false;
        public HeroAction curAction = null;
        public HeroAction lastAction = null;

        private Character _enemy;

        public Glyph KillerGlyph = null;

        private Item _theKey;

        public bool RestoreHealth = false;

        public MissileWeapon RangedWeapon = null;
        public Belongings Belongings;

        private int _str;
        public bool Weakened = false;

        public float Awareness;

        public int Lvl = 1;
        public int Exp = 0;

        private List<Mob> _visibleEnemies;

        public Hero()
        {
            Name = "you";

            HP = HT = 20;
            STR = StartingStr;
            Awareness = 0.1f;

            Belongings = new Belongings(this);

            _visibleEnemies = new List<Mob>();
        }

        public virtual int STR
        {
            get { return Weakened ? _str - 2 : _str; }
            set { _str = value; }
        }

        private const string ATTACK = "_attackSkill";
        private const string DEFENSE = "_defenseSkill";
        private const string STRENGTH = "STR";
        private const string LEVEL = "lvl";
        private const string EXPERIENCE = "exp";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);

            heroClass.StoreInBundle(bundle);
            subClass.StoreInBundle(bundle);

            bundle.Put(ATTACK, _attackSkill);
            bundle.Put(DEFENSE, _defenseSkill);

            bundle.Put(STRENGTH, STR);

            bundle.Put(LEVEL, Lvl);
            bundle.Put(EXPERIENCE, Exp);

            Belongings.StoreInBundle(bundle);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);

            heroClass = HeroClass.ReStoreInBundle(bundle);
            subClass = HeroSubClass.RestoreInBundle(bundle);

            _attackSkill = bundle.GetInt(ATTACK);
            _defenseSkill = bundle.GetInt(DEFENSE);

            STR = bundle.GetInt(STRENGTH);
            UpdateAwareness();

            Lvl = bundle.GetInt(LEVEL);
            Exp = bundle.GetInt(EXPERIENCE);

            Belongings.RestoreFromBundle(bundle);
        }

        public static void Preview(GamesInProgress.Info info, Bundle bundle)
        {
            info.Level = bundle.GetInt(LEVEL);
        }

        public virtual string ClassName()
        {
            return subClass == null || subClass == HeroSubClass.NONE ? heroClass.Title() : subClass.Title;
        }

        public virtual void Live()
        {
            buffs.Buff.Affect<Regeneration>(this);
            buffs.Buff.Affect<Hunger>(this);
        }

        public virtual int Tier()
        {
            return Belongings.Armor == null ? 0 : Belongings.Armor.Tier;
        }

        public virtual bool Shoot(Character enemy, MissileWeapon wep)
        {
            RangedWeapon = wep;
            var result = Attack(enemy);
            RangedWeapon = null;

            return result;
        }

        public override int AttackSkill(Character target)
        {
            var bonus = Buffs<RingOfAccuracy.Accuracy>().Cast<Buff>().Sum(buff => ((RingOfAccuracy.Accuracy)buff).Level);
            var accuracy = (bonus == 0) ? 1 : (float)Math.Pow(1.4, bonus);

            if (RangedWeapon != null && Level.Distance(pos, target.pos) == 1)
                accuracy *= 0.5f;

            var wep = RangedWeapon ?? Belongings.Weapon;
            if (wep != null)
                return (int)(_attackSkill * accuracy * wep.AcuracyFactor(this));

            return (int)(_attackSkill * accuracy);
        }

        public override int DefenseSkill(Character localEnemy)
        {
            var bonus = Buffs<RingOfEvasion.Evasion>().Sum(buff => buff.Level);
            var evasion = bonus == 0 ? 1 : (float)Math.Pow(1.2, bonus);
            if (Paralysed)
                evasion /= 2;

            var aEnc = Belongings.Armor != null ? Belongings.Armor.Str - STR : 0;

            if (aEnc > 0)
                return (int)(_defenseSkill * evasion / Math.Pow(1.5, aEnc));

            if (heroClass != HeroClass.Rogue)
                return (int)(_defenseSkill * evasion);

            if (curAction != null && subClass == HeroSubClass.FREERUNNER && !IsStarving)
                evasion *= 2;

            return (int)((_defenseSkill - aEnc) * evasion);
        }

        public override int Dr()
        {
            var dr = Belongings.Armor != null ? Math.Max(Belongings.Armor.Dr, 0) : 0;
            var barkskin = Buff<Barkskin>();

            if (barkskin != null)
                dr += barkskin.Level();

            return dr;
        }

        public override int DamageRoll()
        {
            var wep = RangedWeapon ?? Belongings.Weapon;
            int dmg;

            if (wep != null)
                dmg = wep.DamageRoll(this);
            else
                dmg = STR > 10 ? Random.IntRange(1, STR - 9) : 1;

            return Buff<Fury>() != null ? (int)(dmg * 1.5f) : dmg;
        }

        public override float Speed()
        {
            var aEnc = Belongings.Armor != null ? Belongings.Armor.Str - STR : 0;

            if (aEnc > 0)
                return (float)(base.Speed() * Math.Pow(1.3, -aEnc));

            var speed = base.Speed();

            return ((HeroSprite)Sprite).Sprint(subClass == HeroSubClass.FREERUNNER && !IsStarving) ? 1.6f * speed : speed;
        }

        public virtual float AttackDelay()
        {
            var wep = RangedWeapon ?? Belongings.Weapon;

            if (wep == null)
                return 1f;

            return wep.SpeedFactor(this);
        }

        protected internal override void Spend(float time)
        {
            var hasteLevel = Buffs<RingOfHaste.Haste>().Sum(buff => buff.Level);

            base.Spend(hasteLevel == 0 ? time : (float)(time * Math.Pow(1.1, -hasteLevel)));
        }

        public virtual void SpendAndNext(float time)
        {
            Busy();
            Spend(time);
            Next();
        }

        protected override bool Act()
        {
            base.Act();

            if (Paralysed)
            {
                curAction = null;

                SpendAndNext(Tick);
                return false;
            }

            CheckVisibleMobs();
            AttackIndicator.UpdateState();

            if (curAction == null)
            {
                if (RestoreHealth)
                {
                    if (IsStarving || HP >= HT)
                        RestoreHealth = false;
                    else
                    {
                        Spend(TimeToRest);
                        Next();
                        return false;
                    }
                }

                Ready();
                return false;
            }
            RestoreHealth = false;

            ready = false;

            if (curAction is HeroAction.Move)
                return ActMove((HeroAction.Move)curAction);

            if (curAction is HeroAction.Interact)
                return ActInteract((HeroAction.Interact)curAction);

            if (curAction is HeroAction.Buy)
                return ActBuy((HeroAction.Buy)curAction);

            if (curAction is HeroAction.PickUp)
                return ActPickUp((HeroAction.PickUp)curAction);

            if (curAction is HeroAction.OpenChest)
                return ActOpenChest((HeroAction.OpenChest)curAction);

            if (curAction is HeroAction.Unlock)
                return ActUnlock((HeroAction.Unlock)curAction);

            if (curAction is HeroAction.Descend)
                return ActDescend((HeroAction.Descend)curAction);

            if (curAction is HeroAction.Ascend)
                return ActAscend((HeroAction.Ascend)curAction);

            if (curAction is HeroAction.Attack)
                return ActAttack((HeroAction.Attack)curAction);

            if (curAction is HeroAction.Cook)
                return ActCook((HeroAction.Cook)curAction);

            return false;
        }

        public virtual void Busy()
        {
            ready = false;
        }

        private void Ready()
        {
            Sprite.Idle();
            curAction = null;
            ready = true;

            GameScene.Ready();
        }

        public virtual void Interrupt()
        {
            if (curAction != null && curAction.Dst != pos)
                lastAction = curAction;
            
            curAction = null;
        }

        public virtual void Resume()
        {
            curAction = lastAction;
            lastAction = null;
            Act();
        }

        private bool ActMove(HeroAction.Move action)
        {
            if (GetCloser(action.Dst))
                return true;

            if (Dungeon.Level.map[pos] == Terrain.SIGN)
                GameScene.Show(new WndMessage(Dungeon.Tip()));

            Ready();

            return false;
        }

        private bool ActInteract(HeroAction.Interact action)
        {
            var npc = action.Npc;

            if (Level.Adjacent(pos, npc.pos))
            {
                Ready();
                Sprite.TurnTo(pos, npc.pos);
                npc.Interact();
                return false;
            }

            if (Level.fieldOfView[npc.pos] && GetCloser(npc.pos))
                return true;

            Ready();

            return false;
        }

        private bool ActBuy(HeroAction.Buy action)
        {
            var dst = action.Dst;
            if (pos == dst || Level.Adjacent(pos, dst))
            {
                Ready();

                var heap = Dungeon.Level.heaps[dst];
                if (heap != null && heap.HeapType == Heap.Type.ForSale && heap.Size() == 1)
                    GameScene.Show(new WndTradeItem(heap, true));

                return false;

            }

            if (GetCloser(dst))
                return true;

            Ready();

            return false;
        }

        private bool ActCook(HeroAction.Cook action)
        {
            var dst = action.Dst;
            if (Dungeon.Visible[dst])
            {
                Ready();
                AlchemyPot.Operate(this, dst);
                return false;

            }

            if (GetCloser(dst))
                return true;

            Ready();

            return false;
        }

        private bool ActPickUp(HeroAction.PickUp action)
        {
            var dst = action.Dst;
            if (pos == dst)
            {
                var heap = Dungeon.Level.heaps[pos];
                if (heap != null)
                {
                    var item = heap.PickUp();
                    if (item.DoPickUp(this))
                    {
                        if (!(item is Dewdrop))
                        {
                            if ((item is ScrollOfUpgrade && ((ScrollOfUpgrade)item).IsKnown) ||
                                (item is PotionOfStrength && ((PotionOfStrength)item).IsKnown))
                            {
                                GLog.Positive(TxtYouNowHave, item.Name);
                            }
                            else
                                GLog.Information(TxtYouNowHave, item.Name);
                        }

                        if (!heap.IsEmpty)
                            GLog.Information(TxtSomethingElse);

                        curAction = null;
                    }
                    else
                    {
                        Dungeon.Level.Drop(item, pos).Sprite.Drop();
                        Ready();
                    }
                }
                else
                    Ready();

                return false;
            }

            if (GetCloser(dst))
                return true;

            Ready();

            return false;
        }

        private bool ActOpenChest(HeroAction.OpenChest action)
        {
            var dst = action.Dst;
            if (Level.Adjacent(pos, dst) || pos == dst)
            {
                var heap = Dungeon.Level.heaps[dst];
                if (heap != null &&
                    (heap.HeapType == Heap.Type.Chest || heap.HeapType == Heap.Type.Tomb ||
                     heap.HeapType == Heap.Type.Skeleton || heap.HeapType == Heap.Type.LockedChest ||
                     heap.HeapType == Heap.Type.CrystalChest))
                {
                    _theKey = null;

                    if (heap.HeapType == Heap.Type.LockedChest || heap.HeapType == Heap.Type.CrystalChest)
                    {
                        _theKey = Belongings.GetKey<GoldenKey>(Dungeon.Depth);

                        if (_theKey == null)
                        {
                            GLog.Warning(TxtLockedChest);
                            Ready();
                            return false;
                        }
                    }

                    switch (heap.HeapType)
                    {
                        case Heap.Type.Tomb:
                            Sample.Instance.Play(Assets.SND_TOMB);
                            Camera.Main.Shake(1, 0.5f);
                            break;
                        case Heap.Type.Skeleton:
                            break;
                        default:
                            Sample.Instance.Play(Assets.SND_UNLOCK);
                            break;
                    }

                    Spend(Key.TIME_TO_UNLOCK);
                    Sprite.DoOperate(dst);
                }
                else
                    Ready();

                return false;
            }

            if (GetCloser(dst))
                return true;

            Ready();

            return false;
        }

        private bool ActUnlock(HeroAction.Unlock action)
        {
            var doorCell = action.Dst;
            if (Level.Adjacent(pos, doorCell))
            {
                _theKey = null;
                var door = Dungeon.Level.map[doorCell];

                if (door == Terrain.LOCKED_DOOR)
                    _theKey = Belongings.GetKey<IronKey>(Dungeon.Depth);
                else if (door == Terrain.LOCKED_EXIT)
                    _theKey = Belongings.GetKey<SkeletonKey>(Dungeon.Depth);

                if (_theKey != null)
                {
                    Spend(Key.TIME_TO_UNLOCK);
                    Sprite.DoOperate(doorCell);

                    Sample.Instance.Play(Assets.SND_UNLOCK);

                }
                else
                {
                    GLog.Warning(TxtLockedDoor);
                    Ready();
                }

                return false;
            }

            if (GetCloser(doorCell))
                return true;

            Ready();

            return false;
        }

        private bool ActDescend(HeroAction.Descend action)
        {
            var stairs = action.Dst;
            if (pos == stairs && pos == Dungeon.Level.exit)
            {
                curAction = null;

                var hunger = Buff<Hunger>();
                if (hunger != null && !hunger.IsStarving)
                    hunger.Satisfy(-Hunger.Starving / 10);

                InterlevelScene.mode = InterlevelScene.Mode.DESCEND;
                Game.SwitchScene(typeof(InterlevelScene));

                return false;

            }

            if (GetCloser(stairs))
                return true;

            Ready();
            return false;
        }

        private bool ActAscend(HeroAction.Ascend action)
        {
            var stairs = action.Dst;
            if (pos == stairs && pos == Dungeon.Level.entrance)
            {
                if (Dungeon.Depth == 1)
                {
                    if (Belongings.GetItem<Amulet>() == null)
                    {
                        GameScene.Show(new WndMessage(TxtLeave));
                        Ready();
                    }
                    else
                    {
                        Dungeon.Win(ResultDescriptions.WIN);
                        Dungeon.DeleteGame(Dungeon.Hero.heroClass, true);
                        Game.SwitchScene(typeof(SurfaceScene));
                    }
                }
                else
                {
                    curAction = null;

                    var hunger = Buff<Hunger>();
                    if (hunger != null && !hunger.IsStarving)
                        hunger.Satisfy(-Hunger.Starving / 10);

                    InterlevelScene.mode = InterlevelScene.Mode.ASCEND;
                    Game.SwitchScene(typeof(InterlevelScene));
                }

                return false;
            }

            if (GetCloser(stairs))
                return true;

            Ready();
            return false;
        }

        private bool ActAttack(HeroAction.Attack action)
        {
            _enemy = action.Target;

            if (Level.Adjacent(pos, _enemy.pos) && _enemy.IsAlive && !Pacified)
            {
                Spend(AttackDelay());
                Sprite.DoAttack(_enemy.pos);

                return false;
            }

            if (Level.fieldOfView[_enemy.pos] && GetCloser(_enemy.pos))
                return true;

            Ready();
            return false;
        }

        public virtual void Rest(bool tillHealthy)
        {
            SpendAndNext(TimeToRest);

            if (!tillHealthy)
                Sprite.ShowStatus(CharSprite.Default, TxtWait);

            RestoreHealth = tillHealthy;
        }

        public override int AttackProc(Character enemy, int damage)
        {
            var wep = RangedWeapon ?? Belongings.Weapon;

            if (wep == null)
                return damage;

            wep.Proc(this, enemy, damage);

            switch (subClass.SubClassType)
            {
                case HeroSubClassType.GLADIATOR:
                    if (wep is MeleeWeapon)
                        damage += buffs.Buff.Affect<Combo>(this).Hit(enemy, damage);
                    break;
                case HeroSubClassType.BATTLEMAGE:
                    var wand = wep as Wand;
                    if (wand != null)
                    {
                        if (wand.CurrrentCharges < wand.MaxCharges && damage > 0)
                        {
                            wand.CurrrentCharges++;
                            if (Dungeon.Quickslot == wand)
                                QuickSlot.Refresh();

                            ScrollOfRecharging.Charge(this);
                        }
                        damage += wand.CurrrentCharges;
                    }
                    goto case HeroSubClassType.SNIPER;
                case HeroSubClassType.SNIPER:
                    if (RangedWeapon != null)
                        buffs.Buff.Prolong<SnipersMark>(enemy, AttackDelay()*1.1f);
                    break;
            }

            return damage;
        }

        public override int DefenseProc(Character enemy, int damage)
        {
            var thorns = Buff<RingOfThorns.Thorns>();
            if (thorns != null)
            {
                var dmg = Random.IntRange(0, damage);
                if (dmg > 0)
                    enemy.Damage(dmg, thorns);
            }

            var armor = Buff<Earthroot.Armor>();
            if (armor != null)
                damage = armor.Absorb(damage);

            if (Belongings.Armor != null)
                damage = Belongings.Armor.Proc(enemy, this, damage);

            return damage;
        }

        public override void Damage(int dmg, object src)
        {
            RestoreHealth = false;
            base.Damage(dmg, src);

            if (subClass == HeroSubClass.BERSERKER && 0 < HP && HP <= HT * Fury.Level)
                buffs.Buff.Affect<Fury>(this);
        }

        private void CheckVisibleMobs()
        {
            var visible = new List<Mob>();

            var newMob = false;

            foreach (var m in Dungeon.Level.mobs.Where(m => Level.fieldOfView[m.pos] && m.Hostile))
            {
                visible.Add(m);
                if (!_visibleEnemies.Contains(m))
                    newMob = true;
            }

            if (newMob)
            {
                Interrupt();
                RestoreHealth = false;
            }

            _visibleEnemies = visible;
        }

        public virtual int VisibleEnemies
        {
            get { return _visibleEnemies.Count; }
        }

        public virtual Mob VisibleEnemy(int index)
        {
            return _visibleEnemies[index % _visibleEnemies.Count];
        }

        private bool GetCloser(int target)
        {
            if (Rooted)
                return false;

            var step = -1;

            if (Level.Adjacent(pos, target))
            {
                if (FindChar(target) == null)
                {
                    if (Level.pit[target] && !Flying && !Chasm.JumpConfirmed)
                    {
                        Chasm.HeroJump(this);
                        Interrupt();
                        return false;
                    }

                    if (Level.passable[target] || Level.avoid[target])
                        step = target;
                }
            }
            else
            {
                const int len = Level.Length;
                var p = Level.passable;
                var v = Dungeon.Level.visited;
                var m = Dungeon.Level.mapped;
                var passable = new bool[len];
                for (var i = 0; i < len; i++)
                    passable[i] = p[i] && (v[i] || m[i]);

                step = Dungeon.FindPath(this, pos, target, passable, Level.fieldOfView);
            }

            if (step == -1)
                return false;

            var oldPos = pos;
            Move(step);
            Sprite.Move(oldPos, pos);

            Spend(1 / Speed());

            return true;
        }

        public virtual bool Handle(int cell)
        {
            if (cell == -1)
                return false;

            Character ch;
            Heap heap;
            if (Dungeon.Level.map[cell] == Terrain.ALCHEMY && cell != pos)
            {
                curAction = new HeroAction.Cook(cell);
            }
            else
                if (Level.fieldOfView[cell] && (ch = FindChar(cell)) is Mob)
                {
                    if (ch is NPC)
                    {
                        curAction = new HeroAction.Interact((NPC)ch);
                    }
                    else
                    {
                        curAction = new HeroAction.Attack(ch);
                    }
                }
                else
                    if ((heap = Dungeon.Level.heaps[cell]) != null)
                    {
                        switch (heap.HeapType)
                        {
                            case Heap.Type.Heap:
                                curAction = new HeroAction.PickUp(cell);
                                break;
                            case Heap.Type.ForSale:
                                if (heap.Size() == 1 && heap.Peek().Price() > 0)
                                    curAction = new HeroAction.Buy(cell);
                                else
                                    curAction = new HeroAction.PickUp(cell);
                                break;
                            default:
                                curAction = new HeroAction.OpenChest(cell);
                                break;
                        }

                    }
                    else if (Dungeon.Level.map[cell] == Terrain.LOCKED_DOOR ||
                             Dungeon.Level.map[cell] == Terrain.LOCKED_EXIT)
                        curAction = new HeroAction.Unlock(cell);
                    else if (cell == Dungeon.Level.exit)
                        curAction = new HeroAction.Descend(cell);
                    else if (cell == Dungeon.Level.entrance)
                        curAction = new HeroAction.Ascend(cell);
                    else
                    {
                        curAction = new HeroAction.Move(cell);
                        lastAction = null;
                    }

            return Act();
        }

        public virtual void EarnExp(int exp)
        {
            Exp += exp;

            var levelUp = false;
            while (Exp >= MaxExp())
            {
                Exp -= MaxExp();
                Lvl++;

                HT += 5;
                HP += 5;
                _attackSkill++;
                _defenseSkill++;

                if (Lvl < 10)
                    UpdateAwareness();

                levelUp = true;
            }

            if (levelUp)
            {
                GLog.Positive(TxtNewLevel, Lvl);
                Sprite.ShowStatus(CharSprite.Positive, TxtLevelUp);
                Sample.Instance.Play(Assets.SND_LEVELUP);

                Badge.ValidateLevelReached();
            }

            if (subClass != HeroSubClass.WARLOCK)
                return;

            var value = Math.Min(HT - HP, 1 + (Dungeon.Depth - 1) / 5);
            if (value > 0)
            {
                HP += value;
                Sprite.Emitter().Burst(Speck.Factory(Speck.HEALING), 1);
            }

            Buff<Hunger>().Satisfy(10);
        }

        public virtual int MaxExp()
        {
            return 5 + Lvl * 5;
        }

        internal virtual void UpdateAwareness()
        {
            Awareness = (float)(1 - Math.Pow((heroClass == HeroClass.Rogue ? 0.85 : 0.90), (1 + Math.Min(Lvl, 9)) * 0.5));
        }

        public virtual bool IsStarving
        {
            get { return Buff<Hunger>().IsStarving; }
        }

        public override void Add(Buff buff)
        {
            Actor.Add(buff);

            if (Sprite != null)
            {
                if (buff is Burning)
                {
                    GLog.Warning("You catch fire!");
                    Interrupt();
                }
                else if (buff is Paralysis)
                {
                    GLog.Warning("You are paralysed!");
                    Interrupt();
                }
                else if (buff is Poison)
                {
                    GLog.Warning("You are poisoned!");
                    Interrupt();
                }
                else if (buff is Ooze)
                {
                    GLog.Warning("Caustic ooze eats your flesh. Wash away it!");
                }
                else if (buff is Roots)
                {
                    GLog.Warning("You can't move!");
                }
                else if (buff is Weakness)
                {
                    GLog.Warning("You feel weakened!");
                }
                else if (buff is Blindness)
                {
                    GLog.Warning("You are blinded!");
                }
                else if (buff is Fury)
                {
                    GLog.Warning("You become furious!");
                    Sprite.ShowStatus(CharSprite.Positive, "furious");
                }
                else if (buff is Charm)
                {
                    GLog.Warning("You are charmed!");
                }
                else if (buff is Cripple)
                {
                    GLog.Warning("You are crippled!");
                }
                else if (buff is Bleeding)
                {
                    GLog.Warning("You are bleeding!");
                }
                else if (buff is Vertigo)
                {
                    GLog.Warning("Everything is spinning around you!");
                    Interrupt();
                }

                else if (buff is Light)
                {
                    Sprite.Add(CharSprite.State.Illuminated);
                }
            }

            BuffIndicator.RefreshHero();
        }

        public override void Remove(Buff buff)
        {
            base.Remove(buff);

            if (buff is Light)
                Sprite.Remove(CharSprite.State.Illuminated);

            BuffIndicator.RefreshHero();
        }

        public override int Stealth()
        {
            return base.Stealth() + Buffs<RingOfShadows.Shadows>().Cast<Buff>().Sum(buff => ((RingOfShadows.Shadows)buff).Level);
        }

        public override void Die(object cause)
        {
            curAction = null;

            DewVial.AutoDrink(this);
            if (IsAlive)
            {
                new Flare(8, 32).Color(0xFFFF66, true).Show(Sprite, 2f);
                return;
            }

            FixTime();
            base.Die(cause);

            var ankh = Belongings.GetItem<Ankh>();
            if (ankh == null)
                ReallyDie(cause);
            else
            {
                Dungeon.DeleteGame(Dungeon.Hero.heroClass, false);
                GameScene.Show(new WndResurrect(ankh, cause));
            }
        }

        public static void ReallyDie(object cause)
        {
            const int length = Level.Length;
            var map = Dungeon.Level.map;
            var visited = Dungeon.Level.visited;
            var discoverable = Level.discoverable;

            for (var i = 0; i < length; i++)
            {
                var terr = map[i];

                if (!discoverable[i]) 
                    continue;

                visited[i] = true;
                
                if ((Terrain.Flags[terr] & Terrain.SECRET) == 0) 
                    continue;

                Level.Set(i, Terrain.discover(terr));
                GameScene.UpdateMap(i);
            }

            Bones.Leave();

            Dungeon.Observe();

            Dungeon.Hero.Belongings.Identify();

            GameScene.GameOver();

            var doom = cause as IDoom;
            if (doom != null)
                doom.OnDeath();

            Dungeon.DeleteGame(Dungeon.Hero.heroClass, true);
        }

        public override void Move(int step)
        {
            base.Move(step);

            if (Flying) 
               return;

            if (Level.water[pos])
                Sample.Instance.Play(Assets.SND_WATER, 1, 1, Random.Float(0.8f, 1.25f));
            else
                Sample.Instance.Play(Assets.SND_STEP);
            
            Dungeon.Level.Press(pos, this);
        }

        public override void OnMotionComplete()
        {
            Dungeon.Observe();
            
            Search(false);

            base.OnMotionComplete();
        }

        public override void OnAttackComplete()
        {
            AttackIndicator.Target(_enemy);

            Attack(_enemy);
            curAction = null;

            Invisibility.Dispel();

            base.OnAttackComplete();
        }

        public override void OnOperateComplete()
        {
            var unlock = curAction as HeroAction.Unlock;
            if (unlock != null)
            {
                if (_theKey != null)
                {
                    _theKey.Detach(Belongings.Backpack);
                    _theKey = null;
                }

                var doorCell = unlock.Dst;
                var door = Dungeon.Level.map[doorCell];

                Level.Set(doorCell, door == Terrain.LOCKED_DOOR ? Terrain.DOOR : Terrain.UNLOCKED_EXIT);
                GameScene.UpdateMap(doorCell);
            }
            else
            {
                var chest = curAction as HeroAction.OpenChest;
                if (chest != null)
                {
                    if (_theKey != null)
                    {
                        _theKey.Detach(Belongings.Backpack);
                        _theKey = null;
                    }

                    var heap = Dungeon.Level.heaps[chest.Dst];
                    if (heap.HeapType == Heap.Type.Skeleton)
                        Sample.Instance.Play(Assets.SND_BONES);
                    heap.Open(this);
                }
            }
            curAction = null;

            base.OnOperateComplete();
        }

        public virtual bool Search(bool intentional)
        {
            var smthFound = false;

            var positive = 0;
            var negative = 0;
            
            foreach (var bonus in Buffs<RingOfDetection.Detection>().Select(buff => buff.Level))
            {
                if (bonus > positive)
                    positive = bonus;
                else if (bonus < 0)
                    negative += bonus;
            }
            
            var distance = 1 + positive + negative;

            var level = intentional ? (2 * Awareness - Awareness * Awareness) : Awareness;
            if (distance <= 0)
            {
                level /= 2 - distance;
                distance = 1;
            }

            var cx = pos % Level.Width;
            var cy = pos / Level.Width;
            var ax = cx - distance;
            if (ax < 0)
                ax = 0;
            var bx = cx + distance;
            if (bx >= Level.Width)
                bx = Level.Width - 1;
            var ay = cy - distance;
            if (ay < 0)
                ay = 0;
            var by = cy + distance;
            if (by >= Level.Height)
                by = Level.Height - 1;

            for (var y = ay; y <= by; y++)
                for (int x = ax, p = ax + y*Level.Width; x <= bx; x++, p++)
                {
                    if (!Dungeon.Visible[p])
                        continue;

                    if (intentional)
                        Sprite.Parent.AddToBack(new CheckedCell(p));

                    if (!Level.secret[p] || (!intentional && !(Random.Float() < level)))
                        continue;

                    var oldValue = Dungeon.Level.map[p];

                    GameScene.DiscoverTile(p, oldValue);

                    Level.Set(p, Terrain.discover(oldValue));

                    GameScene.UpdateMap(p);

                    ScrollOfMagicMapping.Discover(p);

                    smthFound = true;
                }


            if (intentional)
            {
                Sprite.ShowStatus(CharSprite.Default, TxtSearch);
                Sprite.DoOperate(pos);
            
                if (smthFound)
                    SpendAndNext(Random.Float() < level ? TimeToSearch : TimeToSearch * 2);
                else
                    SpendAndNext(TimeToSearch);
            }

            if (!smthFound)
                return false;

            GLog.Warning(TxtNoticedSmth);
            Sample.Instance.Play(Assets.SND_SECRET);
            Interrupt();

            return true;
        }

        public virtual void Resurrect(int resetLevel)
        {
            HP = HT;
            Dungeon.Gold = 0;
            Exp = 0;

            Belongings.Resurrect(resetLevel);

            Live();
        }

        public override HashSet<Type> Resistances()
        {
            var r = Buff<RingOfElements.Resistance>();
            return r == null ? base.Resistances() : r.Resistances();
        }

        public override HashSet<Type> Immunities()
        {
            var buff = Buff<GasesImmunity>();
            return buff == null ? base.Immunities() : GasesImmunity.Immunities;
        }

        public interface IDoom
        {
            void OnDeath();
        }
    }
}