using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Mono.Cecil;

namespace Tools
{
  
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.Runtime.CompilerServices;");
            AssemblyDefinition ass = AssemblyDefinition.ReadAssembly(typeof(On.HeroController).Assembly.Location);
            List<TypeDefinition> ts = ass.MainModule.Types.ToList();
            foreach (var v in AssemblyDefinition.ReadAssembly(typeof(On.HutongGames.PlayMaker.Fsm).Assembly.Location)
                .MainModule.Types) ts.Add(v);
            foreach (var v in ts)
            {
                if (v.FullName.Contains('<') || v.FullName.Contains('>') || v.IsNested || v.IsNotPublic) continue;
                string n = v.FullName;
                if (v.HasGenericParameters)
                {
                    string[] s = v.FullName.Split('`');
                    int c = int.Parse(s[1]);
                    n = s[0] + "<" + new string(',', c - 1) + ">";
                }
                sb.Append("[assembly: TypeForwardedTo(typeof(");
                sb.Append(n.Replace("+","."));
                sb.Append("))]\n");
            }
            File.WriteAllText("Import.cs", sb.ToString());
        }
    }
}
