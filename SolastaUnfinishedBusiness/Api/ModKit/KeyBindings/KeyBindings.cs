using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.ModKit.Utility;
using static UnityModManagerNet.UnityModManager;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    public static IEnumerable<string> Conflicts(this KeyBind keyBind)
    {
        return KeyBindings.conflicts.GetValueOrDefault(keyBind.BindCode, new List<string>())
            .Where(id => id != keyBind.ID);
    }

    public static class KeyBindings
    {
        private static ModEntry modEntry;
        private static SerializableDictionary<string, KeyBind> bindings;
        private static readonly Dictionary<string, Action> actions = new();
        internal static Dictionary<string, List<string>> conflicts = new();
        internal static bool BindingsDidChange;

        private static KeyBind lastTriggered;

        public static bool IsActive(string identifier)
        {
            return GetBinding(identifier).IsActive;
        }

        public static Action GetAction(string identifier)
        {
            return actions.GetValueOrDefault(identifier);
        }

        public static void RegisterAction(string identifier, Action action)
        {
            actions[identifier] = action;
        }

        internal static KeyBind GetBinding(string identifier)
        {
            BindingsDidChange = true;
            return bindings.GetValueOrDefault(identifier, new KeyBind(identifier));
        }

        internal static void SetBinding(string identifier, KeyBind binding)
        {
            bindings[identifier] = binding;
            modEntry.SaveSettings("bindings.json", bindings);
            BindingsDidChange = true;
        }

        public static void UpdateConflicts()
        {
            conflicts.Clear();
            foreach (var binding in bindings)
            {
                var keyBind = binding.Value;
                if (keyBind.IsEmpty)
                {
                    continue;
                }

                var identifier = binding.Key;
                var bindCode = keyBind.ToString();
                var conflict = conflicts.GetValueOrDefault(bindCode, new List<string>());
                conflict.Add(identifier);
                conflicts[bindCode] = conflict;
            }

            conflicts = conflicts.Filter(kvp => kvp.Value.Count > 1);
            //Logger.Log($"conflicts: {String.Join(", ", conflicts.Select(kvp => $"{kvp.Key.orange()} : {kvp.Value.Count}".cyan())).yellow()}");
        }

        public static void OnLoad(ModEntry modEntryIn)
        {
            modEntry ??= modEntryIn;

            if (bindings != null)
            {
                return;
            }

            modEntryIn.LoadSettings("bindings.json", ref bindings);
            BindingsDidChange = true;
        }

        public static void OnGUI()
        {
            if (!BindingsDidChange)
            {
                return;
            }

            UpdateConflicts();
            BindingsDidChange = false;
        }

        public static void OnUpdate()
        {
            if (lastTriggered is { IsActive: false })
                //if (debugKeyBind)
                //    Logger.Log($"    lastTriggered: {lastTriggered} - IsActive: {lastTriggered.IsActive}");
            {
                //if (debugKeyBind)
                //    Logger.Log($"    lastTriggered: {lastTriggered} - Finished".green());
                lastTriggered = null;
            }

            //if (debugKeyBind)
            //    Logger.Log($"looking for {Event.current.keyCode}");
            foreach (var item in bindings)
            {
                var identifier = item.Key;
                var binding = item.Value;
                var active = binding.IsActive;
                //if (debugKeyBind)
                //    Logger.Log($"    checking: {binding.ToString()} - IsActive: {(active ? "True".cyan() : "False")} action: {actions.ContainsKey(identifier)}");
                if (!active || !actions.ContainsKey(identifier))
                {
                    continue;
                }

                //if (debugKeyBind)
                //    Logger.Log($"    binding: {binding.ToString()} - lastTriggered: {lastTriggered}");
                if (lastTriggered != null && binding == lastTriggered)
                {
                    continue;
                }

                //if (debugKeyBind)
                //    Logger.Log($"    firing action: {identifier}".cyan());
                actions.TryGetValue(identifier, out var action);
                action();
                lastTriggered = binding;
            }
        }
    }
}
