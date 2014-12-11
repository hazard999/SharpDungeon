using System.Collections.Generic;
using Android.Graphics;
using pdsharp.noosa.audio;
using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.items.scrolls;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.items.wands
{
    public class WandOfMagicMissile : Wand
    {
        public const string AcDisenchant = "DISENCHANT";

        private const string TxtSelectWand = "Select a wand to Upgrade";

        public const string TxtDisenchanted = "you disenchanted the Wand of Magic Factory and used its essence to Upgrade your {0}";

        public const float TimeToDisenchant = 2f;

        public bool DisenchantEquipped;

        public WandOfMagicMissile()
        {
            name = "Wand of Magic Factory";
            image = ItemSpriteSheet.WAND_MAGIC_MISSILE;
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);

            if (Level > 0)
                actions.Add(AcDisenchant);

            return actions;
        }

        protected internal override void OnZap(int cell)
        {
            var ch = Actor.FindChar(cell);
            if (ch == null)
                return;

            var localLevel = Level;

            ch.Damage(pdsharp.utils.Random.Int(1, 6 + localLevel * 2), this);

            ch.Sprite.Burst(Color.Argb(0xFF, 0x99, 0xCC, 0xFF), localLevel / 2 + 2);

            if (ch != CurUser || ch.IsAlive)
                return;

            Dungeon.Fail(Utils.Format(ResultDescriptions.WAND, name, Dungeon.Depth));
            GLog.Negative("You killed yourself with your own Wand of Magic Factory...");
        }

        public override void Execute(Hero hero, string action)
        {
            if (action.Equals(AcDisenchant))
            {
                if (hero.Belongings.Weapon == this)
                {
                    DisenchantEquipped = true;
                    hero.Belongings.Weapon = null;
                    UpdateQuickslot();
                }
                else
                {
                    DisenchantEquipped = false;
                    Detach(hero.Belongings.Backpack);
                }

                CurUser = hero;
                GameScene.SelectItem(itemSelector, WndBag.Mode.WAND, TxtSelectWand);
            }
            else
                base.Execute(hero, action);
        }

        protected internal override bool IsKnown
        {
            get { return true; }
        }

        public override void SetKnown()
        {
        }

        protected internal override int InitialCharges()
        {
            return 3;
        }

        public override string Desc()
        {
            return "This wand launches missiles of pure magical energy, dealing moderate damage to a target creature.";
        }

        private WndBag.Listener itemSelector = new WandOfMagicMissileListener();

    }

    internal class WandOfMagicMissileListener : WndBag.Listener
    {
        public void OnSelect(Item item)
        {
            if (item != null)
            {
                Sample.Instance.Play(Assets.SND_EVOKE);
                ScrollOfUpgrade.Upgrade(item.CurUser);
                Item.Evoke(item.CurUser);

                GLog.Warning(WandOfMagicMissile.TxtDisenchanted, item.Name);

                item.Upgrade();
                item.CurUser.SpendAndNext(WandOfMagicMissile.TimeToDisenchant);

                Badge.ValidateItemLevelAquired(item);

            }
            else
            {
                //TODO: WandOfMagicMissile
                var wand = item as WandOfMagicMissile;
                
                if (wand != null)
                    if (wand.DisenchantEquipped)
                    {
                        item.CurUser.Belongings.Weapon = item as KindOfWeapon;
                        item.UpdateQuickslot();
                    }
                    else
                    {
                        item.Collect(item.CurUser.Belongings.Backpack);
                    }
            }
        }
    }
}