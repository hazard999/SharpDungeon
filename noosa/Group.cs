using System;
using System.Collections.Generic;
using System.Linq;

namespace pdsharp.noosa
{
    public class Group : Gizmo
    {
        protected internal List<Gizmo> Members;

        public Group()
        {
            Members = new List<Gizmo>();
        }

        public override void Destroy()
        {
            foreach (var g in Members.Where(g => g != null).ToList())
                g.Destroy();

            Members.Clear();
            Members = null;
        }

        public override void Update()
        {
            foreach (var g in Members.Where(g => g != null && g.Exists && g.Active).ToList())
                g.Update();
        }

        public override void Draw()
        {
            foreach (var g in Members.Where(g => g != null && g.Exists && g.Visible).ToList())
                g.Draw();
        }

        public override void Kill()
        {
            // A killed group keeps all its members,
            // but they Get killed too
            foreach (var g in Members.Where(g => g != null && g.Exists).ToList())
                g.Kill();

            base.Kill();
        }

        public virtual int IndexOf(Gizmo g)
        {
            return Members.IndexOf(g);
        }

        public virtual Gizmo Add(Gizmo g)
        {
            if (g.Parent == this)
                return g;

            if (g.Parent != null)
                g.Parent.Remove(g);

            // Trying to find an empty space for a new member
            for (var i = 0; i < Members.Count; i++)
            {
                if (Members[i] != null)
                    continue;

                Members[i] = g;
                g.Parent = this;
                return g;
            }

            Members.Add(g);
            g.Parent = this;

            return g;
        }

        public virtual Gizmo AddToBack(Gizmo g)
        {
            if (g.Parent == this)
            {
                SendToBack(g);
                return g;
            }

            if (g.Parent != null)
                g.Parent.Remove(g);

            if (Members[0] == null)
            {
                Members[0] = g;
                g.Parent = this;
                return g;
            }

            Members.Insert(0, g);
            g.Parent = this;

            return g;
        }

        public virtual Gizmo Recycle<T>(T c) where T : Gizmo, new()
        {
            var g = GetFirstAvailable(c);

            if (g != null)
                return g;

            if (c == null)
                return null;

            try
            {
                return Add(new T());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }

            return null;
        }

        public virtual T Recycle<T>() where T : Gizmo, new()
        {
            var g = GetFirstAvailable<T>();

            if (g != null)
                return (T)g;
            
            try
            {
                return (T)Add(new T());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }

            return null;
        }

        public virtual Gizmo Recycle(Type c)
        {
            var g = GetFirstAvailable(c);

            if (g != null)
                return g;

            if (c == null)
                return null;

            try
            {
                return Add((Gizmo)Activator.CreateInstance(c));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }

            return null;
        }

        // Fast removal - replacing with null
        public virtual Gizmo Erase(Gizmo g)
        {
            var index = Members.IndexOf(g);
            if (index == -1)
                return null;

            Members[index] = null;
            g.Parent = null;
            return g;
        }

        // Real removal
        public virtual Gizmo Remove(Gizmo g)
        {
            if (!Members.Remove(g))
                return null;

            g.Parent = null;
            return g;
        }

        public virtual Gizmo Replace(Gizmo oldOne, Gizmo newOne)
        {
            var index = Members.IndexOf(oldOne);
            if (index == -1)
                return null;

            Members[index] = newOne;
            newOne.Parent = this;
            oldOne.Parent = null;
            return newOne;
        }

        public virtual Gizmo GetFirstAvailable<T1>() where T1 : Gizmo
        {
            return Members.FirstOrDefault(g => g != null && !g.Exists && g.GetType() == typeof(T1));
        }

        public virtual Gizmo GetFirstAvailable<T1>(T1 c) where T1 : Gizmo
        {
            return Members.FirstOrDefault(g => g != null && !g.Exists && (c == null || g.GetType() == typeof(T1)));
        }
        
        public virtual Gizmo GetFirstAvailable(Type c) 
        {
            return Members.FirstOrDefault(g => g != null && !g.Exists && (c == null || g.GetType() == c));
        }

        public virtual int CountLiving()
        {
            return Members.Count(g => g != null && g.Exists && g.Alive);
        }

        public virtual int CountDead()
        {
            return Members.Count(g => g != null && !g.Alive);
        }

        public virtual Gizmo Random()
        {
            if (Members.Count > 0)
                return Members[(int)(new Random(1).NextDouble() * Members.Count)];

            return null;
        }

        public virtual void Clear()
        {
            foreach (var g in Members.Where(g => g != null))
                g.Parent = null;

            Members.Clear();
        }

        public virtual Gizmo BringToFront(Gizmo g)
        {
            if (!Members.Contains(g))
                return null;

            Members.Remove(g);
            Members.Add(g);
            return g;
        }

        public virtual Gizmo SendToBack(Gizmo g)
        {
            if (!Members.Contains(g))
                return null;

            Members.Remove(g);
            Members.Insert(0, g);
            return g;
        }
    }
}