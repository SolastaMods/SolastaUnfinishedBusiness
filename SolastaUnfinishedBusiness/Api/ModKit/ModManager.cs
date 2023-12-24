using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal interface IModEventHandler
{
    int Priority { get; }

    void HandleModEnable();
}

internal sealed class ModManager<TCore, TSettings>
    where TCore : class, new()
    where TSettings : UnityModManager.ModSettings, new()
{
    #region Settings

    private void HandleSaveGUI(UnityModManager.ModEntry modEntry)
    {
        UnityModManager.ModSettings.Save(Settings, modEntry);
    }

    #endregion

    #region Toggle

    private Harmony _harmonyInstance;

    internal void Enable([NotNull] UnityModManager.ModEntry modEntry, Assembly assembly)
    {
        if (Enabled)
        {
            return;
        }

        try
        {
            if (!LoadedOnce)
            {
                modEntry.OnSaveGUI += HandleSaveGUI;
                Settings = UnityModManager.ModSettings.Load<TSettings>(modEntry);
                Core = new TCore();
            }

            var types = assembly.GetTypes();

            if (!Patched)
            {
                _harmonyInstance ??= new Harmony(modEntry.Info.Id);

                foreach (var type in types)
                {
                    var harmonyMethods = HarmonyMethodExtensions.GetFromType(type);
                    if (harmonyMethods == null || harmonyMethods.Count == 0)
                    {
                        continue;
                    }

                    try
                    {
                        var patchProcessor = _harmonyInstance.CreateClassProcessor(type);
                        patchProcessor.Patch();
                    }
                    catch (Exception e)
                    {
                        Main.Error(e);
                    }
                }

                Patched = true;
            }

            Enabled = true;

            if (!LoadedOnce)
            {
                _eventHandlers = types.Where(type => type != typeof(TCore) &&
                                                     !type.IsInterface && !type.IsAbstract &&
                                                     typeof(IModEventHandler).IsAssignableFrom(type))
                    .Select(type => Activator.CreateInstance(type, true) as IModEventHandler).ToList();
                if (Core is IModEventHandler core)
                {
                    _eventHandlers.Add(core);
                }

                _eventHandlers.Sort((x, y) => x.Priority - y.Priority);

                foreach (var t in _eventHandlers)
                {
                    t.HandleModEnable();
                }
            }

            LoadedOnce = true;
        }
        catch (Exception e)
        {
            Main.Error(e);
            throw;
        }
    }

#if DEBUG
    internal void Unload()
    {
        _harmonyInstance.UnpatchAll();
        Enabled = false;
        Patched = false;
    }
#endif

    #endregion

    #region Fields & Properties

    private List<IModEventHandler> _eventHandlers;

    private TCore Core { get; set; }

    internal TSettings Settings { get; set; }

    private bool Enabled { get; set; }

    private bool Patched { get; set; }
    private bool LoadedOnce { get; set; }

    #endregion
}
