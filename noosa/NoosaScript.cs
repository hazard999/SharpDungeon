using Android.Opengl;
using Java.Nio;
using pdsharp.glwrap;
using pdsharp.glscripts;

namespace pdsharp.noosa
{
    public class NoosaScript : Script
    {
        public Uniform UCamera;
        public Uniform UModel;
        public Uniform UTex;
        public Uniform UColorM;
        public Uniform UColorA;
        public Attribute AXy;
        public Attribute AUv;

        private Camera _lastCamera;

        public NoosaScript()
        {
            Compile(Shader());

            UCamera = Uniform("UCamera");
            UModel = Uniform("UModel");
            UTex = Uniform("UTex");
            UColorM = Uniform("UColorM");
            UColorA = Uniform("UColorA");
            AXy = Attribute("AXy");
            AUv = Attribute("AUv");

        }

        public override void Use()
        {
            base.Use();

            AXy.Enable();
            AUv.Enable();
        }

        public virtual void DrawElements(FloatBuffer vertices, ShortBuffer indices, int size)
        {
            vertices.Position(0);
            AXy.VertexPointer(2, 4, vertices);

            vertices.Position(2);
            AUv.VertexPointer(2, 4, vertices);

            GLES20.GlDrawElements(GLES20.GlTriangles, size, GLES20.GlUnsignedShort, indices);
        }

        public virtual void DrawQuad(FloatBuffer vertices)
        {
            vertices.Position(0);
            AXy.VertexPointer(2, 4, vertices);

            vertices.Position(2);
            AUv.VertexPointer(2, 4, vertices);

            GLES20.GlDrawElements(GLES20.GlTriangles, Quad.SIZE, GLES20.GlUnsignedShort, Quad.GetIndices(1));
        }

        public virtual void DrawQuadSet(FloatBuffer vertices, int size)
        {
            if (size == 0)
                return;

            vertices.Position(0);
            AXy.VertexPointer(2, 4, vertices);

            vertices.Position(2);
            AUv.VertexPointer(2, 4, vertices);

            GLES20.GlDrawElements(GLES20.GlTriangles, Quad.SIZE * size, GLES20.GlUnsignedShort, Quad.GetIndices(size));
        }

        public virtual void Lighting(float rm, float gm, float bm, float am, float ra, float ga, float ba, float aa)
        {
            UColorM.value4f(rm, gm, bm, am);
            UColorA.value4f(ra, ga, ba, aa);
        }

        public virtual void ResetCamera()
        {
            _lastCamera = null;
        }

        public virtual void Camera(Camera camera)
        {
            if (camera == null)
                camera = noosa.Camera.Main;

            if (camera == _lastCamera)
                return;

            _lastCamera = camera;
            UCamera.valueM4(camera.Matrix);

            GLES20.GlScissor(camera.X, Game.Height - (int)camera.ScreenHeight - camera.Y, (int)camera.ScreenWidth, (int)camera.ScreenHeight);
        }

        public static NoosaScript Get()
        {
            return Use<NoosaScript>();
        }


        protected internal virtual string Shader()
        {
            return SHADER;
        }

        private const string SHADER = "uniform mat4 UCamera;" + 
            "uniform mat4 UModel;" + 
            "attribute vec4 AXy;" + 
            "attribute vec2 AUv;" +
            "varying vec2 vUV;" + 
            "void main() {" + 
            "  gl_Position = UCamera * UModel * AXy;" + 
            "  vUV = AUv;" + 
            "}" + 
            "//\n" + 
            "precision mediump float;" + 
            "varying vec2 vUV;" + 
            "uniform sampler2D UTex;" + 
            "uniform vec4 UColorM;" + 
            "uniform vec4 UColorA;" + 
            "void main() {" + 
            "  gl_FragColor = texture2D( UTex, vUV ) * UColorM + UColorA;" + 
            "}";
    }
}