using System.Collections.Generic;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.items.wands;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.levels;

namespace sharpdungeon.items
{
    public class LloydsBeacon : Item
    {
        private const string TxtPreventing = "Strong magic aura of this place prevents you from using the lloyd's beacon!";

        private const string TxtCreatures = "Psychic aura of neighbouring creatures doesn't allow you to use the lloyd's beacon at this moment.";

        private const string TxtReturn = "The lloyd's beacon is successfully set at your current location, now you can return here anytime.";

        private const string TxtInfo = "Lloyd's beacon is an intricate magic device, that allows you to return to a place you have already been.";

        private const string TxtSet = "\\Negative\nThis beacon was set somewhere on the Level {0} of Pixel Dungeon.";

        public const float TimeToUse = 1;

        public const string AcSet = "SET";
        public const string AcReturn = "RETURN";

        private int _returnDepth = -1;
        private int _returnPos;

        public LloydsBeacon()
        {
            name = "lloyd's beacon";
            image = ItemSpriteSheet.BEACON;

            unique = true;
        }

        private const string DEPTH = "depth";
        private const string POS = "pos";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(DEPTH, _returnDepth);
            if (_returnDepth != -1)
                bundle.Put(POS, _returnPos);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            _returnDepth = bundle.GetInt(DEPTH);
            _returnPos = bundle.GetInt(POS);
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(AcSet);
            if (_returnDepth != -1)
                actions.Add(AcReturn);
            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action == AcSet || action == AcReturn)
            {
                if (Dungeon.BossLevel())
                {
                    hero.Spend(TimeToUse);
                    GLog.Warning(TxtPreventing);
                    return;
                }

                for (var i = 0; i < Level.NEIGHBOURS8.Length; i++)
                {
                    if (Actor.FindChar(hero.pos + Level.NEIGHBOURS8[i]) == null)
                        continue;

                    GLog.Warning(TxtCreatures);
                    return;
                }
            }

            switch (action)
            {
                case AcSet:
                    _returnDepth = Dungeon.Depth;
                    _returnPos = hero.pos;
                    hero.Spend(TimeToUse);
                    hero.Busy();
                    hero.Sprite.DoOperate(hero.pos);
                    Sample.Instance.Play(Assets.SND_BEACON);
                    GLog.Information(TxtReturn);
                    break;
                case AcReturn:
                    if (_returnDepth == Dungeon.Depth)
                    {
                        Reset();
                        WandOfBlink.Appear(hero, _returnPos);
                        Dungeon.Level.Press(_returnPos, hero);
                        Dungeon.Observe();
                    }
                    else
                    {
                        InterlevelScene.mode = InterlevelScene.Mode.RETURN;
                        InterlevelScene.returnDepth = _returnDepth;
                        InterlevelScene.returnPos = _returnPos;
                        Reset();
                        Game.SwitchScene<InterlevelScene>();
                    }
                    break;
                default:
                    base.Execute(hero, action);
                    break;
            }
        }

        public virtual void Reset()
        {
            _returnDepth = -1;
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override bool Identified
        {
            get { return true; }
        }

        private static readonly ItemSprite.Glowing White = new ItemSprite.Glowing(0xFFFFFF);

        public override ItemSprite.Glowing Glowing()
        {
            return _returnDepth != -1 ? White : null;
        }

        public override string Info()
        {
            return TxtInfo + (_returnDepth == -1 ? "" : Utils.Format(TxtSet, _returnDepth));
        }
    }
}