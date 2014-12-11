using Java.Nio;

namespace pdsharp.glwrap
{


    public class Quad
    {
        const int ByteCount = 16 * sizeof(float);

        // 0---1
        // | \ |
        // 3---2
        public static readonly short[] VALUES = { 0, 1, 2, 0, 2, 3 };

        public static int SIZE
        {
            get { return VALUES.Length; }
        }

        private static ShortBuffer indices;
        private static int indexSize = 0;

        public static FloatBuffer Create()
        {
            
            return ByteBuffer.AllocateDirect(ByteCount).Order(ByteOrder.NativeOrder()).AsFloatBuffer();
        }

        public static FloatBuffer CreateSet(int size)
        {
            return ByteBuffer.AllocateDirect(size * ByteCount).Order(ByteOrder.NativeOrder()).AsFloatBuffer();
        }

        public static ShortBuffer GetIndices(int size)
        {
            if (size > indexSize)
            {
                // TODO: Optimize it!

                indexSize = size;
                var localSize = size * SIZE * sizeof(short);
                indices = ByteBuffer.AllocateDirect(localSize).Order(ByteOrder.NativeOrder()).AsShortBuffer();

                var values = new short[size * 6];
                var pos = 0;
                var limit = size * 4;
                for (var ofs = 0; ofs < limit; ofs += 4)
                {
                    values[pos++] = (short)(ofs + 0);
                    values[pos++] = (short)(ofs + 1);
                    values[pos++] = (short)(ofs + 2);
                    values[pos++] = (short)(ofs + 0);
                    values[pos++] = (short)(ofs + 2);
                    values[pos++] = (short)(ofs + 3);
                }

                indices.Put(values);
                indices.Position(0);
            }

            return indices;
        }

        public static void Fill(float[] v, float x1, float x2, float y1, float y2, float u1, float u2, float v1, float v2)
        {

            v[0] = x1;
            v[1] = y1;
            v[2] = u1;
            v[3] = v1;

            v[4] = x2;
            v[5] = y1;
            v[6] = u2;
            v[7] = v1;

            v[8] = x2;
            v[9] = y2;
            v[10] = u2;
            v[11] = v2;

            v[12] = x1;
            v[13] = y2;
            v[14] = u1;
            v[15] = v2;
        }

        public static void FillXy(float[] v, float x1, float x2, float y1, float y2)
        {

            v[0] = x1;
            v[1] = y1;

            v[4] = x2;
            v[5] = y1;

            v[8] = x2;
            v[9] = y2;

            v[12] = x1;
            v[13] = y2;
        }

        public static void fillUV(float[] v, float u1, float u2, float v1, float v2)
        {

            v[2] = u1;
            v[3] = v1;

            v[6] = u2;
            v[7] = v1;

            v[10] = u2;
            v[11] = v2;

            v[14] = u1;
            v[15] = v2;
        }
    }

}