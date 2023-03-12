using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit.Utility;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    public static class Browser<T, TItem, TDef> // for many things the item will be the definition
    {
        private static IEnumerable<TDef> _filteredDefinitions;
        private static string prevCallerKey = "";
        private static string searchText = "";
        private static int searchLimit = 100;
        private static int matchCount;
        private static bool showAll;

        public static void OnGUI(string callerKey,
            T target,
            IEnumerable<TItem> current,
            IEnumerable<TDef> available,
            Func<TItem, TDef> definition,
            Func<TDef, string> searchAndSortKey,
            Func<TDef, string> title,
            Func<TDef, string> description = null,
            Func<TItem, string> value = null,
            Action<T, TItem, string> setValue = null,
            // The following functors take an target and item and will return an action to perform it if it can, null otherwise
            Func<T, TItem, Action> incrementValue = null,
            Func<T, TItem, Action> decrementValue = null,
            Func<T, TDef, Action> addItem = null,
            Func<T, TItem, Action> removeItem = null
        )
        {
            var searchChanged = false;
//            var refreshTree = false;
            if (callerKey != prevCallerKey)
            {
                searchChanged = true;
                showAll = false;
            }

            prevCallerKey = callerKey;
            using (HorizontalScope())
            {
                100.Space();
                ActionTextField(ref searchText, "searchText", null, () => { searchChanged = true; }, Width(320));
                25.Space();
                Label("Limit", ExpandWidth(false));
                ActionIntTextField(ref searchLimit, "searchLimit", null, () => { searchChanged = true; }, Width(175));
                if (searchLimit > 1000) { searchLimit = 1000; }

                25.Space();
                searchChanged |= DisclosureToggle("Show All".Orange().Bold(), ref showAll);
            }

            using (HorizontalScope())
            {
                Space(100);
                ActionButton("Search", () => { searchChanged = true; }, AutoWidth());
                Space(25);
                if (matchCount > 0 && searchText.Length > 0)
                {
                    var matchesText = "Matches: ".Green().Bold() + $"{matchCount}".Orange().Bold();
                    if (matchCount > searchLimit) { matchesText += " => ".Cyan() + $"{searchLimit}".Cyan().Bold(); }

                    Label(matchesText, ExpandWidth(false));
                }
            }

            var currentDict = current.ToDictionary(definition, c => c);
            List<TDef> definitions;
            if (showAll)
            {
                UpdateSearchResults(searchText, available, searchAndSortKey);
                definitions = _filteredDefinitions.ToList();
            }
            else
            {
                definitions = currentDict.Keys.ToList();
            }

            var terms = searchText.Split(' ').Select(s => s.ToLower()).ToHashSet();

            var sorted = definitions.OrderBy(title);
            matchCount = 0;
            Div(100);
            foreach (var def in sorted)
            {
                var name = title(def);
                var nameLower = name.ToLower();
                if (name is not { Length: > 0 } ||
                    (searchText.Length != 0 && !terms.All(term => nameLower.Contains(term))))
                {
                    continue;
                }

                currentDict.TryGetValue(def, out var item);
                OnRowGUI(target, def, item, title, description, value, setValue, incrementValue, decrementValue,
                    addItem, removeItem);
            }
        }

        [UsedImplicitly]
        public static void OnRowGUI(
            T target,
            TDef definition,
            TItem item,
            Func<TDef, string> title,
            Func<TDef, string> description = null,
            Func<TItem, string> value = null,
            Action<T, TItem, string> setValue = null,
            Func<T, TItem, Action> incrementValue = null,
            Func<T, TItem, Action> decrementValue = null,
            Func<T, TDef, Action> addItem = null,
            Func<T, TItem, Action> removeItem = null
        )
        {
            var remWidth = UmmWidth;
            matchCount++;
            using (HorizontalScope())
            {
                Space(100);
                remWidth -= 100;
                var titleWidth = ((int)(UmmWidth / (IsWide ? 3.0f : 4.0f))).Point();
                var text = title(definition);
                if (item != null)
                {
                    text = text.Cyan().Bold();
                }

                Label(text, Width(titleWidth));
                remWidth -= titleWidth;
                Space(10);
                remWidth -= 10;
                if (item != null && value?.Invoke(item) is { } stringValue)
                {
                    if (decrementValue?.Invoke(target, item) is { } decrementAction)
                    {
                        ActionButton("<", decrementAction, 60.Width());
                    }
                    else
                    {
                        63.Space();
                    }

                    Space(10f);
                    Label($"{stringValue}".Orange().Bold(), Width(100));
                    if (incrementValue?.Invoke(target, item) is { } incrementer)
                    {
                        ActionButton(">", incrementer, 60.Width());
                    }
                    else
                    {
                        63.Space();
                    }
                }

                30.Space();
                if (addItem?.Invoke(target, definition) is { } add)
                {
                    ActionButton("Add".Localized(), add, 150.Width());
                }

                Space(10);
                remWidth -= 10;
                if (item != null && removeItem?.Invoke(target, item) is { } remove)
                {
                    ActionButton("Remove".Localized(), remove, 175.Width());
                }

                remWidth -= 178;
                Space(20);
                remWidth -= 20;
                using (VerticalScope())
                {
                    if (description != null)
                    {
                        Label(description(definition).StripHTML().Green(), AutoWidth());
                    }
                }
            }

            Div(100);
        }

        [UsedImplicitly]
        public static void UpdateSearchResults(string searchTextParam, IEnumerable<TDef> definitions,
            Func<TDef, string> searchAndSortKey)
        {
            if (definitions == null)
            {
                return;
            }

            var terms = searchTextParam.Split(' ').Select(s => s.ToLower()).ToHashSet();
            var filtered = new List<TDef>();
            foreach (var def in definitions)
            {
                if (def.GetType().ToString().Contains(searchTextParam)
                   )
                {
                    filtered.Add(def);
                }
                else
                {
                    var name = searchAndSortKey(def).ToLower();
                    if (terms.All(term => name.Matches(term)))
                    {
                        filtered.Add(def);
                    }
                }
            }

            matchCount = filtered.Count;
            _filteredDefinitions = filtered.OrderBy(searchAndSortKey).Take(searchLimit).ToArray();
        }
    }
}
