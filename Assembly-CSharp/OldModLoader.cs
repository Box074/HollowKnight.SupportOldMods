using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour.HookGen;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft;
using Modding.Delegates;

namespace HKLab
{
    public static class OldModLoader
    {
        static bool _init = false;
        public static string ModPath
        {
            get
            {
                string path = "";
                switch (SystemInfo.operatingSystemFamily)
                {
                    case OperatingSystemFamily.MacOSX:
                        path = Application.dataPath + "/Resources/Data/Managed/Mods/";
                        break;
                    case OperatingSystemFamily.Windows:
                        path = Application.dataPath + "\\Managed\\Mods\\";
                        break;
                    case OperatingSystemFamily.Linux:
                        path = Application.dataPath + "/Managed/Mods/";
                        break;
                }
                return path;
            }
        }

        public static Config Config { get; private set; } = null;

        public static string CachePath
        {
            get
            {
                string p = Path.Combine(ModPath, "oldMods");
                if (!Directory.Exists(p)) Directory.CreateDirectory(p);
                return p;
            }
        }
        public static List<string> tempFile = new();
        public static List<string> HD = new();
        public static List<string> HDN = new();
        public static readonly (string, string, string)[] TypeReplaceList = new (string, string, string)[]
        {
            ("Modding.Mod","HKLab","Mod"),
            ("Modding.ModHooks","HKLab","ModHooks"),
            ("Modding.ReflectionHelper","HKLab","ReflectionHelper")
        };
        public static readonly List<AssemblyDefinition> apis = new();
        public static void Init()
        {
            if (_init) return;
            _init = true;
            try
            {
                foreach (var v in Directory.GetFiles(Path.GetDirectoryName(typeof(Modding.Mod).Assembly.Location), "*.dll"))
                {
                    try
                    {
                        apis.Add(AssemblyDefinition.ReadAssembly(v));
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
            if (File.Exists(Path.Combine(CachePath, "config.json")))
            {
                try
                {
                    Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(CachePath, "config.json")));
                }
                catch (Exception)
                {
                    Modding.Logger.LogWarn("Cannot load config");
                }
            }
            if (Config == null) Config = new();
            Config.OldVersion = Config.Version;
            Config.Version = Config.CurrentVersion;

            List<string> willRe = new();
            foreach (var v in Config.IgnoreOldMods)
            {
                if (v.Value < 1) continue;
                if (v.Value < Config.CurrentVersion) willRe.Add(v.Key);
            }
            foreach (var v in willRe) Config.IgnoreOldMods.Remove(v);

            foreach (var v in typeof(OldModLoader).Assembly.GetTypes().Where(x => x.Namespace == "HKLab.Hooks"))
            {
                HD.Add("Modding." + v.Name);
            }
            foreach(var v in typeof(SetBoolProxy).Assembly.GetTypes().Where(x=>x.Namespace == "Modding.Delegates"))
            {
                HDN.Add("Modding." + v.Name);
            }

            Modding.ModHooks.ApplicationQuitHook += ModHooks_ApplicationQuitHook;

            AllMods();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;
            HookEndpointManager.Add(typeof(Directory).GetMethod("GetFiles", new Type[]{
                typeof(string),typeof(string)
                }), new Func<Func<string, string, string[]>, string, string, string[]>(GetFilesHook));
            File.WriteAllText(Path.Combine(CachePath, "config.json"), JsonConvert.SerializeObject(Config, new JsonSerializerSettings
            {

            }));
        }

        private static void ModHooks_ApplicationQuitHook()
        {
            foreach(var v in tempFile)
            {
                try
                {
                    File.Delete(v);
                }
                catch (Exception)
                {

                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Modding.Logger.LogError(e.ExceptionObject.ToString());
        }
		public static bool inModLoader => ModLoader.isLoading;

        private static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            foreach (var v in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = v.GetType(args.Name);
                if (t != null) return v;
            }
            return null;
        }

        public static string[] GetFilesHook(Func<string, string, string[]> orig, string p, string d)
        {
			if(!inModLoader) return orig(p, d);
			Modding.Logger.Log("Find \""+d+"\" in " + p);
            if (p == CachePath && d == "*.dll")
            {
                List<string> s = new List<string>();
                foreach (var v in orig(CachePath, "*.dll"))
                {
                    if (Path.GetFileName(v).StartsWith("cache_"))
                    {
                        s.Add(v);
                    }
                }
                return s.ToArray();
            }
            else if(d == "*.dll" && Config.TestMode)
            {
                string[] a = orig(p, "*.dll");
                List<string> o = new();
                foreach(var v in a)
                {
                    if (Path.GetFileName(v).StartsWith("cache_")) continue;
                    AssemblyDefinition ass = AssemblyDefinition.ReadAssembly(v);
                    try
                    {
                        ModB62(ass);
                        string op = Path.Combine(p, "cache_" + Path.GetFileName(v));
                        ass.Write(op);
                        tempFile.Add(op);
                        File.SetAttributes(op, FileAttributes.Hidden | FileAttributes.Temporary);
						o.Add(op);
                    }catch(Exception e)
                    {
                        Modding.Logger.LogError(e);
                        o.Add(v);
                    }
                }
				Modding.Logger.Log("Find *.dll(Cache): ");
				foreach(var v in o) Modding.Logger.Log(v);
                return o.ToArray();
            }
			string[] ou = orig(p, d);
			if(d == "*.dll")
			{
				Modding.Logger.Log("Find *.dll: ");
				foreach(var v in ou) Modding.Logger.Log(v);
			}
			return ou;
        }
        public static Type GetType(string name)
        {
            foreach (var v in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = v.GetType(name);
                if (t != null) return t;
            }
            return null;
        }
        private static void AllMods()
        {
            string op = Path.Combine(ModPath, "oldMods");
            if (!Directory.Exists(op))
            {
                Directory.CreateDirectory(op);
            }
            if (Config.OldVersion != Config.CurrentVersion)
            {
                foreach (var v in Directory.GetFiles(op, "cache_*.dll")) File.Delete(v);
            }
            foreach (var v in Directory.GetFiles(op, "*.dll"))
            {
                if (Path.GetFileName(v).StartsWith("cache_"))
                {
                    if (!File.Exists(Path.Combine(op, Path.GetFileName(v).Substring(6))) &&
                        Config.OldVersion == Config.CurrentVersion)
                    {
                        File.Delete(v);
                    }
                    continue;
                }
                if (Config.IgnoreOldMods.ContainsKey(Path.GetFileNameWithoutExtension(v)))
                {
                    Modding.Logger.Log("Ignore: " + v);
                    continue;
                }
                try
                {
                    AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(v);
                    string fp = Path.Combine(CachePath, "cache_" + Path.GetFileNameWithoutExtension(v) + ".dll");

                    Modding.Logger.Log("old mod: " + v);
                    if (File.Exists(fp))
                    {
                        File.SetAttributes(fp, FileAttributes.Hidden);
                        if (File.GetLastWriteTime(fp).ToUniversalTime() >= File.GetLastWriteTime(v).ToUniversalTime()) continue;
                    }
                    Modding.Logger.Log(v + " -> " + fp);
                    try
                    {
                        OldMod(assembly);

                        assembly.Write(fp);
                        File.SetAttributes(fp, FileAttributes.Hidden);
                        Modding.Logger.Log(v + " -> " + fp + "(done)");
                    }
                    catch (Exception e)
                    {
                        Modding.Logger.LogError(e);
                        Modding.Logger.Log(v + " -> " + fp + "(failed)");
                        if (Config.AutoIgnoreBrokenMods)
                        {
                            Config.IgnoreOldMods.Add(Path.GetFileNameWithoutExtension(v), Config.CurrentVersion);
                        }
                    }
                    finally
                    {
                        assembly?.Dispose();
                    }

                }
                catch (Exception e)
                {
                    Modding.Logger.LogError(e);
                    if (Config.AutoIgnoreBrokenMods)
                    {
                        Config.IgnoreOldMods.Add(Path.GetFileNameWithoutExtension(v), Config.CurrentVersion);
                    }
                }
            }
        }

        public static void ModB62(AssemblyDefinition a)
        {
            foreach(var m in a.Modules)
            {
                foreach(var t in m.GetTypeReferences())
                {
                    if (HDN.Contains(t.FullName))
                    {
                        t.Namespace = "Modding.Delegates";
                    }
                }
            }
        }

        public static void OldMod(AssemblyDefinition assembly)
        {
            foreach (var m in assembly.Modules)
            {
                foreach (var t in m.GetTypeReferences())
                {

                    foreach (var s in TypeReplaceList)
                    {
                        if (t.FullName == s.Item1)
                        {
                            Modding.Logger.Log("Replace TypeReferences: " + t.FullName + " -> " + s.Item2 + "." + s.Item3);
                            t.Namespace = s.Item2;
                            t.Name = s.Item3;
                            break;
                        }
                    }
                    if (m.Runtime == TargetRuntime.Net_4_0)
                    {
                        if (HDN.Contains(t.FullName))
                        {
                            t.Namespace = "Modding.Delegates";
                        }
                    }
                    else
                    {
                        try
                        {
                            if (HD.Contains(t.FullName))
                            {
                                t.Namespace = "HKLab.Hooks";
                                continue;
                            }
                            foreach (var v in apis.Select(x => x.MainModule))
                            {
                                string name = t.FullName;
                                TypeDefinition td = v.GetType(name);
                                if (td != null)
                                {
                                    td.Scope = new AssemblyNameReference("A", null);
                                    try
                                    {
                                        Modding.Logger.Log(name + "[" +
                                        m.AssemblyReferences[(int)t.Scope.MetadataToken.RID - 1].FullName + "][" +
                                        t.Scope.MetadataToken.RID + "] -> "
                                        + v.Assembly.Name.FullName);
                                    }
                                    catch (Exception)
                                    {

                                    }
                                    AssemblyNameReference asmName = m.AssemblyReferences.FirstOrDefault(x => x.FullName == v.Assembly.FullName);
                                    if (asmName == null)
                                    {
                                        asmName = AssemblyNameReference.Parse(v.Assembly.FullName);
                                        m.AssemblyReferences.Add(asmName);
                                    }
                                    t.Scope = asmName;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Modding.Logger.LogWarn(ex.ToString());
                        }
                    }
                }
                int i2 = 0;
                foreach (var r in m.AssemblyReferences)
                {
                    Modding.Logger.LogDebug($"AssemblyRef[{i2}]: {r.FullName}");
                    if (r.Name == "mscorlib")
                    {
                        r.Version = typeof(object).Assembly.GetName().Version;
                    }
                    if (r.Name == "PlayMaker")
                    {
                        r.Version = typeof(HutongGames.PlayMaker.Fsm).Assembly.GetName().Version;
                    }
                    i2++;
                }

            }
        }
    }
}
