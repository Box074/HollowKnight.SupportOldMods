using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using JetBrains.Annotations;
using Modding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MonoMod;
using UnityEngine;

namespace HKLab
{
    [MonoModPatch("Modding.ModSavegameData")]
    public class Patch_ModSavegameData
    {
        public Patch_ModSavegameData()
        {
            modData = new Dictionary<string, JToken>();
        }

        public Dictionary<string, string> loadedMods;

        public Dictionary<string, JToken> modData;
    }
    [MonoModPatch("Modding.Mod")]
    public abstract class Patch_Mod : Modding.Mod
    {
        [MonoModReplace]
        internal Type globalSettingsType;
        [MonoModReplace]
        internal Type saveSettingsType;

        public Patch_Mod() : base(null)
        {

        }

        internal virtual Type GlobalSettingsType { get; set; } = null;
        internal virtual Type LocalSettingsType { get; set; } = null;

        [MonoModIgnore]
        public Patch_Mod(string name = null) : base(name)
        {
        }

        private extern void orig_LoadGlobalSettings();

        
        private void LoadGlobalSettings()
        {
            if(this is Mod)
            {
                Log("GlobalSettings Type: " + GlobalSettingsType?.FullName ?? "Null");
                globalSettingsType = GlobalSettingsType;
            }
            orig_LoadGlobalSettings();
        }
        private extern void orig_SaveGlobalSettings();
        private void SaveGlobalSettings()
        {
            if (this is Mod)
            {
                Log("GlobalSettings Type: " + LocalSettingsType?.FullName ?? "Null");
                globalSettingsType = GlobalSettingsType;
            }
            orig_SaveGlobalSettings();
        }
        private extern void orig_SaveLocalSettings(Patch_ModSavegameData data);
        private void SaveLocalSettings(Patch_ModSavegameData data)
        {
            if (this is Mod)
            {
                Log("LocalSettings Type: " + LocalSettingsType?.FullName ?? "Null");
                saveSettingsType = LocalSettingsType;
            }
            orig_SaveLocalSettings(data);
        }
        private extern void orig_LoadLocalSettings(Patch_ModSavegameData data);
        
        private void LoadLocalSettings(Patch_ModSavegameData data)
        {
            if (this is Mod)
            {
                Log("LocalSettings Type: " + LocalSettingsType?.FullName ?? "Null");
                saveSettingsType = LocalSettingsType;
            }
            orig_LoadLocalSettings(data);
        }
    }
    [PublicAPI]
    public abstract class Mod : Patch_Mod , IGlobalSettings<ModSettings> , ILocalSettings<ModSettings>
    {
        public Mod() : this(null)
        {
            
        }
        public Mod(string name) : base(name)
        {
            globalSettingsType = GlobalSettingsType;
            saveSettingsType = LocalSettingsType;
        }
        internal virtual List<(string, string)> preloadNames() => null;
        public readonly string _globalSettingsPath = null;
        public override List<(string, string)> GetPreloadNames() => preloadNames();

        public void OnLoadGlobal(ModSettings s)
        {
            try
            {
                GlobalSettings = s;
            }catch(Exception e)
            {
                LogError(e);
            }
        }

        public ModSettings OnSaveGlobal()
        {
            return GlobalSettings;
        }

        public void OnLoadLocal(ModSettings s)
        {
            try
            {
                SaveSettings = s;
            }catch(Exception e)
            {
                LogError(e);
            }
        }

        public ModSettings OnSaveLocal()
        {
            return SaveSettings;
        }

        public virtual ModSettings GlobalSettings { get; set; } = null;
        public virtual ModSettings SaveSettings { get; set; } = null;
        

        internal override Type GlobalSettingsType
        {
            get
            {
                if (_gsettingsType == null)
                {
                    ModSettings settings = GlobalSettings;
                    if (settings != null)
                    {
                        _gsettingsType = settings.GetType();
                    }
                }
                return _gsettingsType;
            }
        }
        private Type _gsettingsType = null;

        internal override Type LocalSettingsType
        {
            get
            {
                if (_lsettingsType == null)
                {
                    ModSettings settings = SaveSettings;
                    if (settings != null)
                    {
                        _lsettingsType = settings.GetType();
                    }
                }
                return _lsettingsType;
            }
        }
        private Type _lsettingsType = null;

    }
}
