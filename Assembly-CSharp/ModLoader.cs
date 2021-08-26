using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using MonoMod;
using Modding;
using UnityEngine;

namespace HKLab
{
    [MonoModPatch("Modding.ModLoader")]
    static class ModLoader
    {
		public static bool isLoading = false;
        public static extern IEnumerator orig_LoadModsInit(GameObject coroutineHolder);
        public static IEnumerator LoadModsInit(GameObject coroutineHolder)
        {
            new Thread(HKLab.ReflectionHelper.PreloadCommonTypes);
            try
            {
                OldModLoader.Init();
            }catch(Exception e)
            {
                Modding.Logger.LogError(e);
            }
			isLoading = true;
            yield return orig_LoadModsInit(coroutineHolder);
			isLoading = false;
        }
        private static extern void orig_AddModInstance(Type ty, ModLoader.ModInstance mod);
        private static void AddModInstance(Type ty, ModLoader.ModInstance mod)
        {
            if (mod.Error.HasValue)
            {
                if (mod.Error.Value == ModErrorState.Construct)
                {
                    return;
                }
            }
            orig_AddModInstance(ty, mod);
        }
        [MonoModIgnore]
        public class ModInstance
        {
            public ModInstance()
            {
            }

            public IMod Mod;

            public string Name;

            public ModLoader.ModErrorState? Error;

            public bool Enabled;
        }
        [MonoModIgnore]
        public enum ModErrorState
        {
            Construct,
            Initialize,
            Unload
        }
    }
}
