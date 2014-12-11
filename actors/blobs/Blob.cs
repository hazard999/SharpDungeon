using System;
using Java.Util;
using pdsharp.utils;
using sharpdungeon.effects;
using sharpdungeon.utils;

namespace sharpdungeon.actors.blobs
{
    public class Blob : Actor
    {
        public const int Width = levels.Level.Width;
        public const int Height = levels.Level.Height;
        public const int Length = levels.Level.Length;

        public int Volume = 0;

        public int[] Cur;
        protected internal int[] Off;

        public BlobEmitter Emitter;

        protected internal Blob()
        {
            Cur = new int[Length];
            Off = new int[Length];

            Volume = 0;
        }

        private const string CUR = "cur";
        private const string START = "start";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);

            if (Volume <= 0) 
                return;

            int start;
            for (start = 0; start < Length; start++)
            {
                if (Cur[start] > 0)
                {
                    break;
                }
            }
            int end;
            for (end = Length - 1; end > start; end--)
            {
                if (Cur[end] > 0)
                {
                    break;
                }
            }

            bundle.Put(START, start);
            bundle.Put(CUR, Trim(start, end + 1));
        }

        private int[] Trim(int start, int end)
        {
            var len = end - start;
            var copy = new int[len];
            Array.Copy(Cur, start, copy, 0, len);
            return copy;
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);

            var data = bundle.GetIntArray(CUR);
            if (data != null)
            {
                var start = bundle.GetInt(START);
                for (var i = 0; i < data.Length; i++)
                {
                    Cur[i + start] = data[i];
                    Volume += data[i];
                }
            }

            if (!levels.Level.resizingNeeded) 
                return;

            var cur = new int[levels.Level.Length];
            Arrays.Fill(cur, 0);

            var loadedMapSize = levels.Level.loadedMapSize;
            for (var i = 0; i < loadedMapSize; i++)
                Array.Copy(this.Cur, i*loadedMapSize, cur, i*levels.Level.Width, loadedMapSize);

            Cur = cur;
        }

        protected override bool Act()
        {
            Spend(Tick);

            if (Volume <= 0) 
                return true;

            Volume = 0;
            Evolve();

            var tmp = Off;
            Off = Cur;
            Cur = tmp;

            return true;
        }

        public virtual void Use(BlobEmitter emitter)
        {
            Emitter = emitter;
        }

        protected internal virtual void Evolve()
        {
            var notBlocking = BArray.not(levels.Level.solid, null);

            for (var i = 1; i < Height - 1; i++)
            {
                var from = i * Width + 1;
                var to = from + Width - 2;

                for (var pos = from; pos < to; pos++)
                {
                    if (notBlocking[pos])
                    {
                        var count = 1;
                        var sum = Cur[pos];

                        if (notBlocking[pos - 1])
                        {
                            sum += Cur[pos - 1];
                            count++;
                        }

                        if (notBlocking[pos + 1])
                        {
                            sum += Cur[pos + 1];
                            count++;
                        }

                        if (notBlocking[pos - Width])
                        {
                            sum += Cur[pos - Width];
                            count++;
                        }

                        if (notBlocking[pos + Width])
                        {
                            sum += Cur[pos + Width];
                            count++;
                        }

                        var value = sum >= count ? (sum/count) - 1 : 0;
                        Off[pos] = value;

                        Volume += value;
                    }
                    else
                        Off[pos] = 0;
                }
            }
        }

        public virtual void Seed(int cell, int amount)
        {
            Cur[cell] += amount;
            Volume += amount;
        }

        public virtual void Clear(int cell)
        {
            Volume -= Cur[cell];
            Cur[cell] = 0;
        }

        public virtual string TileDesc()
        {
            return null;
        }

        public static Blob Seed(int cell, int amount, Type type)
        {
            try
            {
                var gas = Dungeon.Level.Blobs[type];
                if (gas == null)
                {
                    gas = (Blob)Activator.CreateInstance(type);
                    Dungeon.Level.Blobs.Add(type, gas);
                }

                gas.Seed(cell, amount);

                return gas;
            }
            catch (Exception e)
            {
                PixelDungeon.ReportException(e);
                return null;
            }
        }
    }
}