using System;
using System.Collections.Generic;
using pdsharp.glwrap;

namespace pdsharp.glscripts
{
    public class Script : Program
    {
        private static readonly List<Script> all = new List<Script>();

        private static Script curScript;

        private static Type curScriptClass;

        public static T Use<T>() where T : Script
        {
            if (typeof(T) == curScriptClass)
                return (T)curScript;

            var script = all.Find(sc => sc.GetType() == typeof(T));
            if (script == null)
            {
                try
                {
                    script = Activator.CreateInstance<T>();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.StackTrace);
                }
                all.Add(script);
            }

            if (curScript != null)
            {
                curScript.Unuse();
            }

            curScript = script;
            curScriptClass = typeof(T);
            curScript.Use();

            return (T)curScript;
        }

        public static void Reset()
        {
            foreach (var script in all)
                script.Delete();

            all.Clear();

            curScript = null;
            curScriptClass = null;
        }

        public virtual void Compile(string src)
        {
            string[] srcShaders = StringHelperClass.StringSplit(src, "//\n", true);
            Attach(Shader.CreateCompiled(Shader.VERTEX, srcShaders[0]));
            Attach(Shader.CreateCompiled(Shader.FRAGMENT, srcShaders[1]));
            Link();
        }

        public virtual void Unuse()
        {
        }
    }

}