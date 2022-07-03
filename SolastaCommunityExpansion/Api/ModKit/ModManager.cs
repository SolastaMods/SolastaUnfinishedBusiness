using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityModManagerNet;

namespace SolastaCommunityExpansion.Api.ModKit;

public interface IModEventHandler
{
    int Priority { get; }

    void HandleModEnable();
}

public sealed class ModManager<TCore, TSettings>
    where TCore : class, new()
    where TSettings : UnityModManager.ModSettings, new()
{
    #region Toggle

    public void Enable([NotNull] UnityModManager.ModEntry modEntry, Assembly assembly)
    {
        _logger = modEntry.Logger;

        if (Enabled)
        {
            return;
        }

        using ProcessLogger process = new(_logger);

        try
        {
            process.Log("Enabling.");
            process.Log("Loading settings.");
            modEntry.OnSaveGUI += HandleSaveGUI;
            Settings = UnityModManager.ModSettings.Load<TSettings>(modEntry);
            Core = new TCore();

            var types = assembly.GetTypes();

            if (!Patched)
            {
                Harmony harmonyInstance = new(modEntry.Info.Id);
                foreach (var type in types)
                {
                    var harmonyMethods = HarmonyMethodExtensions.GetFromType(type);
                    if (harmonyMethods == null || !harmonyMethods.Any())
                    {
                        continue;
                    }

                    process.Log($"Patching: {type.FullName}");
                    try
                    {
                        var patchProcessor = harmonyInstance.CreateClassProcessor(type);
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

            process.Log("Registering events.");
            _eventHandlers = types.Where(type => type != typeof(TCore) &&
                                                 !type.IsInterface && !type.IsAbstract &&
                                                 typeof(IModEventHandler).IsAssignableFrom(type))
                .Select(type => Activator.CreateInstance(type, true) as IModEventHandler).ToList();
            if (Core is IModEventHandler core)
            {
                _eventHandlers.Add(core);
            }

            _eventHandlers.Sort((x, y) => x.Priority - y.Priority);

            process.Log("Raising events: OnEnable()");

            foreach (var t in _eventHandlers)
            {
                t.HandleModEnable();
            }
        }
        catch (Exception e)
        {
            Main.Error(e);
            throw;
        }

        process.Log("Enabled.");
    }

    #endregion

    #region Settings

    private void HandleSaveGUI(UnityModManager.ModEntry modEntry)
    {
        UnityModManager.ModSettings.Save(Settings, modEntry);
    }

    #endregion

    private sealed class ProcessLogger : IDisposable
    {
        private readonly UnityModManager.ModEntry.ModLogger _logger;
        private readonly Stopwatch _stopWatch = new();

        public ProcessLogger(UnityModManager.ModEntry.ModLogger logger)
        {
            _logger = logger;
            _stopWatch.Start();
        }

        public void Dispose()
        {
            _stopWatch.Stop();
        }

        [Conditional("DEBUG")]
        public void Log(string status)
        {
            _logger.Log($"[{_stopWatch.Elapsed:ss\\.ff}] {status}");
        }
    }

    #region Fields & Properties

    private UnityModManager.ModEntry.ModLogger _logger;
    private List<IModEventHandler> _eventHandlers;

    private TCore Core { get; set; }

    public TSettings Settings { get; private set; }

    private bool Enabled { get; set; }

    private bool Patched { get; set; }

    #endregion
}
