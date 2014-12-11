using System;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.items.armor.glyphs;
using sharpdungeon.sprites;

namespace sharpdungeon.items.armor
{
    public abstract class Glyph : Bundlable
    {
        private static readonly Type[] Glyphs = { typeof(Bounce), typeof(Affection), typeof(AntiEntropy), typeof(Multiplicity), typeof(Potential), typeof(Metabolism), typeof(Stench), typeof(Viscosity), typeof(Displacement), typeof(Entanglement) };

        private static readonly float[] Chances = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        public abstract int Proc(Armor armor, Character attacker, Character defender, int damage);

        public virtual string Name()
        {
            return Name("glyph");
        }

        public virtual string Name(string armorName)
        {
            return armorName;
        }

        public virtual void RestoreFromBundle(Bundle bundle)
        {
        }

        public virtual void StoreInBundle(Bundle bundle)
        {
        }

        public virtual ItemSprite.Glowing Glowing()
        {
            return ItemSprite.Glowing.White;
        }

        public virtual bool CheckOwner(Character owner)
        {
            if (!owner.IsAlive && owner is Hero)
            {
                ((Hero)owner).KillerGlyph = this;
                Badge.ValidateDeathFromGlyph();
                return true;
            }

            return false;
        }

        public static Glyph Random()
        {
            try
            {
                return (Glyph)Activator.CreateInstance(Glyphs[pdsharp.utils.Random.Chances(Chances)]);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}