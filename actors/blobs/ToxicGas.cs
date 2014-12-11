using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.utils;

namespace sharpdungeon.actors.blobs
{
    public class ToxicGas : Blob, Hero.IDoom
    {
        protected internal override void Evolve()
        {
            base.Evolve();

            var levelDamage = 5 + Dungeon.Depth * 5;

            for (var i = 0; i < Length; i++)
            {
                Character ch;
                if (Cur[i] <= 0 || (ch = FindChar(i)) == null) 
                    continue;

                var damage = (ch.HT + levelDamage) / 40;
                if (pdsharp.utils.Random.Int(40) < (ch.HT + levelDamage)%40)
                    damage++;

                ch.Damage(damage, this);
            }

            var blob = Dungeon.Level.Blobs[typeof(ParalyticGas)];
            
            if (blob == null) 
                return;

            var par = blob.Cur;

            for (var i = 0; i < Length; i++)
            {
                var t = Cur[i];
                var p = par[i];

                if (p >= t)
                {
                    Volume -= t;
                    Cur[i] = 0;
                }
                else
                {
                    blob.Volume -= p;
                    par[i] = 0;
                }
            }
        }

        public override void Use(BlobEmitter emitter)
        {
            base.Use(emitter);

            emitter.Pour(Speck.Factory(Speck.TOXIC), 0.6f);
        }

        public override string TileDesc()
        {
            return "A greenish cloud of toxic gas is swirling here.";
        }

        public void OnDeath()
        {
            Badge.ValidateDeathFromGas();

            Dungeon.Fail(Utils.Format(ResultDescriptions.GAS, Dungeon.Depth));
            GLog.Negative("You died from a toxic gas..");
        }
    }
}