using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.missiles
{
    public class Dart : MissileWeapon
    {
        public Dart() : this(1)
        {
            name = "dart";
            image = ItemSpriteSheet.DART;

            Min = 1;
            Max = 4;
        }

        public Dart(int number)
        {
            quantity = number;
        }

        public override string Desc()
        {
            return "These simple metal spikes are weighted to fly true and " + "sting their prey with a flick of the wrist.";
        }

        public override Item Random()
        {
            quantity = pdsharp.utils.Random.Int(5, 15);
            return this;
        }

        public override int Price()
        {
            return Quantity() * 2;
        }
    }
}