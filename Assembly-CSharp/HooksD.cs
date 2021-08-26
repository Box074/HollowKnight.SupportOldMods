using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace HKLab
{
    class HooksD
    {
        static readonly List<(object, Dictionary<MethodInfo, Delegate>)> ds = new();

        public static T Remove<T>(Delegate @delegate) where T : Delegate
        {
            var d = ds.FirstOrDefault(x => x.Item1 == @delegate.Target);
            Dictionary<MethodInfo, Delegate> gd = null;
            if (d == default((object, Dictionary<MethodInfo, Delegate>)))
            {
                return null;
            }
            else
            {
                gd = d.Item2;
            }
            if (gd.TryGetValue(@delegate.Method, out var v2))
            {
                gd.Remove(@delegate.Method);
                return (T)v2;
            }
            return null;
        }
        public static T Add<T>(Delegate @delegate,T a) where T : Delegate
        {
            var d = ds.FirstOrDefault(x => x.Item1 == @delegate.Target);
            Dictionary<MethodInfo, Delegate> gd = null;
            if (d == default((object, Dictionary<MethodInfo, Delegate>)))
            {
                gd = new Dictionary<MethodInfo, Delegate>();
                ds.Add((@delegate.Target, gd));
            }
            else
            {
                gd = d.Item2;
            }
            if (gd.TryGetValue(@delegate.Method, out var v2))
            {
                return (T)v2;
            }
            List<Type> t = new();
            foreach(var v in typeof(T).GetMethod("Invoke").GetParameters())
            {
                t.Add(v.ParameterType);
            }
           
            gd.Add(@delegate.Method, a);
            return a;
        }
    }
}
