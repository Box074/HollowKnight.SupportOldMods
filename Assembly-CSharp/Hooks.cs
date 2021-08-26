using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKLab.Hooks;
using UnityEngine;

namespace HKLab
{
    public class ModHooks
    {
        private static ModHooks _instance = null;
        public string ModVersion = Modding.ModHooks.ModVersion;
        public readonly bool IsCurrent = true;
        public readonly List<string> LoadedMods = Modding.ModHooks.GetAllMods().Select(x => x.GetName()).ToList();
        public static readonly char PathSeperator = SystemInfo.operatingSystem.Contains("Windows") ? '\\' : '/';
        public GameVersionData version;
        public ModHooks()
        {
            version = Modding.ModHooks.version;
        }
        public static ModHooks Instance
        {
            get
            {
                if (_instance == null) _instance = new ModHooks();
                return _instance;
            }
        }
        public event HKLab.Hooks.LanguageGetHandler LanguageGetHook
        {
            add => Modding.ModHooks.LanguageGetHook += HooksD.Add(value,
                new Modding.Delegates.LanguageGetProxy(
                    (a, b, c) =>
                    {
                        return value(a, b);
                    }
                    )
                );

            remove => Modding.ModHooks.LanguageGetHook -= HooksD.Remove<Modding.Delegates.LanguageGetProxy>(value);
        }
        public event HKLab.Hooks.CursorHandler CursorHook
        {
            add => Modding.ModHooks.CursorHook += new global::System.Action(value);
            remove => Modding.ModHooks.CursorHook -= new global::System.Action(value);
        }
        public event HKLab.Hooks.ColliderCreateHandler ColliderCreateHook
        {
            add => Modding.ModHooks.ColliderCreateHook += new global::System.Action<UnityEngine.GameObject>(value);
            remove => Modding.ModHooks.ColliderCreateHook -= new global::System.Action<UnityEngine.GameObject>(value);
        }
        public event HKLab.Hooks.GameObjectHandler ObjectPoolSpawnHook
        {
            add => Modding.ModHooks.ObjectPoolSpawnHook += new global::System.Func<UnityEngine.GameObject, UnityEngine.GameObject>(value);
            remove => Modding.ModHooks.ObjectPoolSpawnHook -= new global::System.Func<UnityEngine.GameObject, UnityEngine.GameObject>(value);
        }
        /** public event HKLab.Hooks.GameObjectFsmHandler OnGetEventSenderHook {
         add => Modding.ModHooks.OnGetEventSenderHook += new Modding.GameObjectFsmHandler(value);
         remove => Modding.ModHooks.OnGetEventSenderHook -= new Modding.GameObjectFsmHandler(value); 
}
         **/
        public event HKLab.Hooks.ApplicationQuitHandler ApplicationQuitHook
        {
            add => Modding.ModHooks.ApplicationQuitHook += new global::System.Action(value);
            remove => Modding.ModHooks.ApplicationQuitHook -= new global::System.Action(value);
        }
        /** public event HKLab.Hooks.SetFontHandler SetFontHook {
         add => Modding.ModHooks.SetFontHook += new Modding.SetFontHandler(value);
         remove => Modding.ModHooks.SetFontHook -= new Modding.SetFontHandler(value); 
}
         **//** public event HKLab.Hooks.TextDirectionProxy TextDirectionHook {
         add => Modding.ModHooks.TextDirectionHook += new Modding.TextDirectionProxy(value);
         remove => Modding.ModHooks.TextDirectionHook -= new Modding.TextDirectionProxy(value); 
}
         **/
        public event HKLab.Hooks.HitInstanceHandler HitInstanceHook
        {
            add => Modding.ModHooks.HitInstanceHook += new global::Modding.Delegates.HitInstanceHandler(value);
            remove => Modding.ModHooks.HitInstanceHook -= new global::Modding.Delegates.HitInstanceHandler(value);
        }
        public event HKLab.Hooks.DrawBlackBordersHandler DrawBlackBordersHook
        {
            add => Modding.ModHooks.DrawBlackBordersHook += new global::System.Action<System.Collections.Generic.List<UnityEngine.GameObject>>(value);
            remove => Modding.ModHooks.DrawBlackBordersHook -= new global::System.Action<System.Collections.Generic.List<UnityEngine.GameObject>>(value);
        }
        public event HKLab.Hooks.OnEnableEnemyHandler OnEnableEnemyHook
        {
            add => Modding.ModHooks.OnEnableEnemyHook += new global::Modding.Delegates.OnEnableEnemyHandler(value);
            remove => Modding.ModHooks.OnEnableEnemyHook -= new global::Modding.Delegates.OnEnableEnemyHandler(value);
        }
        /** public event HKLab.Hooks.OnRecieveDeathEventHandler OnRecieveDeathEventHook {
         add => Modding.ModHooks.OnRecieveDeathEventHook += new Modding.OnRecieveDeathEventHandler(value);
         remove => Modding.ModHooks.OnRecieveDeathEventHook -= new Modding.OnRecieveDeathEventHandler(value); 
}
         **/
        public event HKLab.Hooks.OnReceiveDeathEventHandler OnReceiveDeathEventHook
        {
            add => Modding.ModHooks.OnReceiveDeathEventHook += new global::Modding.Delegates.OnReceiveDeathEventHandler(value);
            remove => Modding.ModHooks.OnReceiveDeathEventHook -= new global::Modding.Delegates.OnReceiveDeathEventHandler(value);
        }
        /** public event HKLab.Hooks.OnRecordKillForJournalHandler OnRecordKillForJournalHook {
         add => Modding.ModHooks.OnRecordKillForJournalHook += new Modding.OnRecordKillForJournalHandler(value);
         remove => Modding.ModHooks.OnRecordKillForJournalHook -= new Modding.OnRecordKillForJournalHandler(value); 
}
         **/
        public event HKLab.Hooks.RecordKillForJournalHandler RecordKillForJournalHook
        {
            add => Modding.ModHooks.RecordKillForJournalHook += new global::Modding.Delegates.RecordKillForJournalHandler(value);
            remove => Modding.ModHooks.RecordKillForJournalHook -= new global::Modding.Delegates.RecordKillForJournalHandler(value);
        }
        public event HKLab.Hooks.SetBoolProxy SetPlayerBoolHook
        {
            add => Modding.ModHooks.SetPlayerBoolHook += HooksD.Add(value,
                new Modding.Delegates.SetBoolProxy((a,b)=>
            {
                value(a, b);
                return PlayerData.instance.GetBoolInternal(a);
            }));
            remove => Modding.ModHooks.SetPlayerBoolHook -= HooksD.Remove<Modding.Delegates.SetBoolProxy>(value);
        }
        public event HKLab.Hooks.GetBoolProxy GetPlayerBoolHook
        {
            add => Modding.ModHooks.GetPlayerBoolHook += HooksD.Add(value,
                new Modding.Delegates.GetBoolProxy(
                    (a, b) =>
                    {
                        return value(a);
                    }
                    )
                );

            remove => Modding.ModHooks.GetPlayerBoolHook -= HooksD.Remove<Modding.Delegates.GetBoolProxy>(value);
        }
        public event HKLab.Hooks.SetIntProxy SetPlayerIntHook
        {
            add => Modding.ModHooks.SetPlayerIntHook += HooksD.Add(value,
                new Modding.Delegates.SetIntProxy((a, b) =>
                {
                    value(a, b);
                    return PlayerData.instance.GetIntInternal(a);
                }));
            remove => Modding.ModHooks.SetPlayerIntHook -= HooksD.Remove<Modding.Delegates.SetIntProxy>(value);
        }
        public event HKLab.Hooks.GetIntProxy GetPlayerIntHook
        {
            add => Modding.ModHooks.GetPlayerIntHook += HooksD.Add(value,
                new Modding.Delegates.GetIntProxy(
                    (a, b) =>
                    {
                        return value(a);
                    }
                    )
                );

            remove => Modding.ModHooks.GetPlayerIntHook -= HooksD.Remove<Modding.Delegates.GetIntProxy>(value);
        }
        public event HKLab.Hooks.SetFloatProxy SetPlayerFloatHook
        {
            add => Modding.ModHooks.SetPlayerFloatHook += HooksD.Add(value,
                new Modding.Delegates.SetFloatProxy((a, b) =>
                {
                    value(a, b);
                    return PlayerData.instance.GetFloatInternal(a);
                }));
            remove => Modding.ModHooks.SetPlayerFloatHook -= HooksD.Remove<Modding.Delegates.SetFloatProxy>(value);
        }
        public event HKLab.Hooks.GetFloatProxy GetPlayerFloatHook
        {
            add => Modding.ModHooks.GetPlayerFloatHook += HooksD.Add(value,
                new Modding.Delegates.GetFloatProxy(
                    (a, b) =>
                    {
                        return value(a);
                    }
                    )
                );

            remove => Modding.ModHooks.GetPlayerFloatHook -= HooksD.Remove<Modding.Delegates.GetFloatProxy>(value);
        }
        public event HKLab.Hooks.SetStringProxy SetPlayerStringHook
        {
            add => Modding.ModHooks.SetPlayerStringHook += HooksD.Add(value,
                new Modding.Delegates.SetStringProxy((a, b) =>
                {
                    value(a, b);
                    return PlayerData.instance.GetStringInternal(a);
                }));
            remove => Modding.ModHooks.SetPlayerStringHook -= HooksD.Remove<Modding.Delegates.SetStringProxy>(value);
        }
        public event HKLab.Hooks.GetStringProxy GetPlayerStringHook
        {
            add => Modding.ModHooks.GetPlayerStringHook += HooksD.Add(value,
                new Modding.Delegates.GetStringProxy(
                    (a, b) =>
                    {
                        return value(a);
                    }
                    )
                );

            remove => Modding.ModHooks.GetPlayerStringHook -= HooksD.Remove<Modding.Delegates.GetStringProxy>(value);
        }
        public event HKLab.Hooks.SetVector3Proxy SetPlayerVector3Hook
        {
            add => Modding.ModHooks.SetPlayerVector3Hook += HooksD.Add(value,
                new Modding.Delegates.SetVector3Proxy((a, b) =>
                {
                    value(a, b);
                    return PlayerData.instance.GetVector3Internal(a);
                }));
            remove => Modding.ModHooks.SetPlayerVector3Hook -= HooksD.Remove<Modding.Delegates.SetVector3Proxy>(value);
        }
        public event HKLab.Hooks.GetVector3Proxy GetPlayerVector3Hook
        {
            add => Modding.ModHooks.GetPlayerVector3Hook += HooksD.Add(value,
                new Modding.Delegates.GetVector3Proxy(
                    (a, b) =>
                    {
                        return value(a);
                    }
                    )
                );

            remove => Modding.ModHooks.GetPlayerVector3Hook -= HooksD.Remove<Modding.Delegates.GetVector3Proxy>(value);
        }
        public event HKLab.Hooks.SetVariableProxy SetPlayerVariableHook
        {
            add => Modding.ModHooks.SetPlayerVariableHook += new global::Modding.Delegates.SetVariableProxy(value);
            remove => Modding.ModHooks.SetPlayerVariableHook -= new global::Modding.Delegates.SetVariableProxy(value);
        }
        public event HKLab.Hooks.GetVariableProxy GetPlayerVariableHook
        {
            add => Modding.ModHooks.GetPlayerVariableHook += new global::Modding.Delegates.GetVariableProxy(value);
            remove => Modding.ModHooks.GetPlayerVariableHook -= new global::Modding.Delegates.GetVariableProxy(value);
        }
        /** public event HKLab.Hooks.NewPlayerDataHandler NewPlayerDataHook {
         add => Modding.ModHooks.NewPlayerDataHook += new Modding.NewPlayerDataHandler(value);
         remove => Modding.ModHooks.NewPlayerDataHook -= new Modding.NewPlayerDataHandler(value); 
}
         **/
        public event HKLab.Hooks.BlueHealthHandler BlueHealthHook
        {
            add => Modding.ModHooks.BlueHealthHook += new global::System.Func<System.Int32>(value);
            remove => Modding.ModHooks.BlueHealthHook -= new global::System.Func<System.Int32>(value);
        }
        public event HKLab.Hooks.TakeHealthProxy TakeHealthHook
        {
            add => Modding.ModHooks.TakeHealthHook += new global::Modding.Delegates.TakeHealthProxy(value);
            remove => Modding.ModHooks.TakeHealthHook -= new global::Modding.Delegates.TakeHealthProxy(value);
        }
        public event HKLab.Hooks.TakeDamageProxy TakeDamageHook
        {
            add => Modding.ModHooks.TakeDamageHook += new global::Modding.Delegates.TakeDamageProxy(value);
            remove => Modding.ModHooks.TakeDamageHook -= new global::Modding.Delegates.TakeDamageProxy(value);
        }
        public event HKLab.Hooks.AfterTakeDamageHandler AfterTakeDamageHook
        {
            add => Modding.ModHooks.AfterTakeDamageHook += new global::Modding.Delegates.AfterTakeDamageHandler(value);
            remove => Modding.ModHooks.AfterTakeDamageHook -= new global::Modding.Delegates.AfterTakeDamageHandler(value);
        }
        public event HKLab.Hooks.VoidHandler BeforePlayerDeadHook
        {
            add => Modding.ModHooks.BeforePlayerDeadHook += new global::System.Action(value);
            remove => Modding.ModHooks.BeforePlayerDeadHook -= new global::System.Action(value);
        }
        public event HKLab.Hooks.VoidHandler AfterPlayerDeadHook
        {
            add => Modding.ModHooks.AfterPlayerDeadHook += new global::System.Action(value);
            remove => Modding.ModHooks.AfterPlayerDeadHook -= new global::System.Action(value);
        }
        public event HKLab.Hooks.AttackHandler AttackHook
        {
            add => Modding.ModHooks.AttackHook += new global::System.Action<GlobalEnums.AttackDirection>(value);
            remove => Modding.ModHooks.AttackHook -= new global::System.Action<GlobalEnums.AttackDirection>(value);
        }
        public event HKLab.Hooks.DoAttackHandler DoAttackHook
        {
            add => Modding.ModHooks.DoAttackHook += new global::System.Action(value);
            remove => Modding.ModHooks.DoAttackHook -= new global::System.Action(value);
        }
        public event HKLab.Hooks.AfterAttackHandler AfterAttackHook
        {
            add => Modding.ModHooks.AfterAttackHook += new global::System.Action<GlobalEnums.AttackDirection>(value);
            remove => Modding.ModHooks.AfterAttackHook -= new global::System.Action<GlobalEnums.AttackDirection>(value);
        }
        public event HKLab.Hooks.SlashHitHandler SlashHitHook
        {
            add => Modding.ModHooks.SlashHitHook += new global::Modding.Delegates.SlashHitHandler(value);
            remove => Modding.ModHooks.SlashHitHook -= new global::Modding.Delegates.SlashHitHandler(value);
        }
        public event HKLab.Hooks.CharmUpdateHandler CharmUpdateHook
        {
            add => Modding.ModHooks.CharmUpdateHook += new global::Modding.Delegates.CharmUpdateHandler(value);
            remove => Modding.ModHooks.CharmUpdateHook -= new global::Modding.Delegates.CharmUpdateHandler(value);
        }
        public event HKLab.Hooks.HeroUpdateHandler HeroUpdateHook
        {
            add => Modding.ModHooks.HeroUpdateHook += new global::System.Action(value);
            remove => Modding.ModHooks.HeroUpdateHook -= new global::System.Action(value);
        }
        public event HKLab.Hooks.BeforeAddHealthHandler BeforeAddHealthHook
        {
            add => Modding.ModHooks.BeforeAddHealthHook += new global::System.Func<System.Int32, System.Int32>(value);
            remove => Modding.ModHooks.BeforeAddHealthHook -= new global::System.Func<System.Int32, System.Int32>(value);
        }
        /** public event HKLab.Hooks.BeforeAddHealthHandler _BeforeAddHealthHook {
         add => Modding.ModHooks._BeforeAddHealthHook += new Modding.BeforeAddHealthHandler(value);
         remove => Modding.ModHooks._BeforeAddHealthHook -= new Modding.BeforeAddHealthHandler(value); 
}
         **/
        public event HKLab.Hooks.FocusCostHandler FocusCostHook
        {
            add => Modding.ModHooks.FocusCostHook += new global::System.Func<System.Single>(value);
            remove => Modding.ModHooks.FocusCostHook -= new global::System.Func<System.Single>(value);
        }
        public event HKLab.Hooks.SoulGainHandler SoulGainHook
        {
            add => Modding.ModHooks.SoulGainHook += new global::System.Func<System.Int32, System.Int32>(value);
            remove => Modding.ModHooks.SoulGainHook -= new global::System.Func<System.Int32, System.Int32>(value);
        }
        public event HKLab.Hooks.DashVelocityHandler DashVectorHook
        {
            add => Modding.ModHooks.DashVectorHook += new global::System.Func<UnityEngine.Vector2, UnityEngine.Vector2>(value);
            remove => Modding.ModHooks.DashVectorHook -= new global::System.Func<UnityEngine.Vector2, UnityEngine.Vector2>(value);
        }
        public event HKLab.Hooks.DashPressedHandler DashPressedHook
        {
            add => Modding.ModHooks.DashPressedHook += new global::System.Func<System.Boolean>(value);
            remove => Modding.ModHooks.DashPressedHook -= new global::System.Func<System.Boolean>(value);
        }
        public event HKLab.Hooks.SavegameLoadHandler SavegameLoadHook
        {
            add => Modding.ModHooks.SavegameLoadHook += new global::System.Action<System.Int32>(value);
            remove => Modding.ModHooks.SavegameLoadHook -= new global::System.Action<System.Int32>(value);
        }
        public event HKLab.Hooks.SavegameSaveHandler SavegameSaveHook
        {
            add => Modding.ModHooks.SavegameSaveHook += new global::System.Action<System.Int32>(value);
            remove => Modding.ModHooks.SavegameSaveHook -= new global::System.Action<System.Int32>(value);
        }
        public event HKLab.Hooks.NewGameHandler NewGameHook
        {
            add => Modding.ModHooks.NewGameHook += new global::System.Action(value);
            remove => Modding.ModHooks.NewGameHook -= new global::System.Action(value);
        }
        public event HKLab.Hooks.ClearSaveGameHandler SavegameClearHook
        {
            add => Modding.ModHooks.SavegameClearHook += new global::System.Action<System.Int32>(value);
            remove => Modding.ModHooks.SavegameClearHook -= new global::System.Action<System.Int32>(value);
        }
        public event HKLab.Hooks.AfterSavegameLoadHandler AfterSavegameLoadHook
        {
            add => Modding.ModHooks.AfterSavegameLoadHook += new global::System.Action<SaveGameData>(value);
            remove => Modding.ModHooks.AfterSavegameLoadHook -= new global::System.Action<SaveGameData>(value);
        }
        public event HKLab.Hooks.BeforeSavegameSaveHandler BeforeSavegameSaveHook
        {
            add => Modding.ModHooks.BeforeSavegameSaveHook += new global::System.Action<SaveGameData>(value);
            remove => Modding.ModHooks.BeforeSavegameSaveHook -= new global::System.Action<SaveGameData>(value);
        }
        public event HKLab.Hooks.GetSaveFileNameHandler GetSaveFileNameHook
        {
            add => Modding.ModHooks.GetSaveFileNameHook += new global::System.Func<System.Int32, System.String>(value);
            remove => Modding.ModHooks.GetSaveFileNameHook -= new global::System.Func<System.Int32, System.String>(value);
        }
        public event HKLab.Hooks.AfterClearSaveGameHandler AfterSaveGameClearHook
        {
            add => Modding.ModHooks.AfterSaveGameClearHook += new global::System.Action<System.Int32>(value);
            remove => Modding.ModHooks.AfterSaveGameClearHook -= new global::System.Action<System.Int32>(value);
        }
        public event HKLab.Hooks.SceneChangedHandler SceneChanged
        {
            add => Modding.ModHooks.SceneChanged += new global::System.Action<System.String>(value);
            remove => Modding.ModHooks.SceneChanged -= new global::System.Action<System.String>(value);
        }
        public event HKLab.Hooks.BeforeSceneLoadHandler BeforeSceneLoadHook
        {
            add => Modding.ModHooks.BeforeSceneLoadHook += new global::System.Func<System.String, System.String>(value);
            remove => Modding.ModHooks.BeforeSceneLoadHook -= new global::System.Func<System.String, System.String>(value);
        }

    }
}
