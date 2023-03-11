using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit.Utility;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit;
internal static partial class UI
{
    internal static class Browser<T, Item, Def> // for many things the item will be the definition
    {
        public static IEnumerable<Def> filteredDefitions = null;
        private static string prevCallerKey = "";
        private static string searchText = "";
        private static int searchLimit = 100;
        public static int matchCount = 0;
        private static bool showAll = false;

        internal static void OnGUI(string callerKey,
                                    T target,
                                    List<Item> current,
                                    IEnumerable<Def> available,
                                    Func<Item, Def> definition,
                                    Func<Def, string> searchAndSortKey,
                                    Func<Def, string> title,
                                    Func<Def, string> description = null,
                                    Func<Item, string> value = null,
                                    Action<T, Item, string> setValue = null,
                                    // The following functors take an target and item and will return an action to perform it if it can, null otehrwise
                                    Func<T, Item, Action> incrementValue = null,
                                    Func<T, Item, Action> decrementValue = null,
                                    Func<T, Def, Action> addItem = null,
                                    Func<T, Item, Action> removeItem = null
                )
        {
            var searchChanged = false;
//            var refreshTree = false;
            if (callerKey != prevCallerKey) { searchChanged = true; showAll = false; }
            prevCallerKey = callerKey;
            using (HorizontalScope())
            {
                100.Space();
                ActionTextField(ref searchText, "searhText", null, () => { searchChanged = true; }, Width(320));
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
            var remainingWidth = ummWidth;
            var currentDict = current.ToDictionary(c => definition(c), c => c);
            List<Def> definitions = null;
            if (showAll)
            {
                UpdateSearchResults(searchText, available, searchAndSortKey);
                definitions = filteredDefitions.ToList();
            }
            else
            {
                definitions = currentDict.Keys.ToList();
            }
            var terms = searchText.Split(' ').Select(s => s.ToLower()).ToHashSet();

            var sorted = definitions.OrderBy(def => title(def));
            matchCount = 0;
            Div(100);
            foreach (var def in sorted)
            {
                var name = title(def);
                var nameLower = name.ToLower();
                if (name != null && name.Length > 0 && (searchText.Length == 0 || terms.All(term => nameLower.Contains(term))))
                {
                    Item item = default(Item);
                    currentDict.TryGetValue(def, out item);
                    OnRowGUI(target, def, item, title, description, value, setValue, incrementValue, decrementValue, addItem, removeItem);
                }
            }
        }
        internal static void OnRowGUI(
                                    T target,
                                    Def definition,
                                    Item item,
                                    Func<Def, string> title,
                                    Func<Def, string> description = null,
                                    Func<Item, string> value = null,
                                    Action<T, Item, string> setValue = null,
                                    Func<T, Item, Action> incrementValue = null,
                                    Func<T, Item, Action> decrementValue = null,
                                    Func<T, Def, Action> addItem = null,
                                    Func<T, Item, Action> removeItem = null
                                   )
        {
            var remWidth = ummWidth;
            matchCount++;
            using (HorizontalScope())
            {
                Space(100); remWidth -= 100;
                var titleWidth = ((int)(ummWidth / (IsWide ? 3.0f : 4.0f))).Point();
                string text = title(definition);
                if (item != null)
                    text = text.Cyan().Bold();
                Label(text, Width(titleWidth));
                remWidth -= titleWidth;
                Space(10); remWidth -= 10;
                if (item != null && value != null && value(item) is string stringValue)
                {
                    if (decrementValue != null && decrementValue(target, item) is Action decrementer)
                        ActionButton("<", decrementer, 60.Width());
                    else
                        63.Space();
                    Space(10f);
                    Label($"{stringValue}".Orange().Bold(), Width(100));
                    if (incrementValue != null && incrementValue(target, item) is Action incrementer)
                        ActionButton(">", incrementer, 60.Width());
                    else
                        63.Space();
                }
                UI.Space(30);
                if (addItem != null && addItem(target, definition) is Action add)
                    ActionButton("Add".Localized(), add, 150.Width());
               Space(10); remWidth -= 10;
                if (item != null && removeItem != null && removeItem(target, item) is Action remove)
                    ActionButton("Remove".Localized(), remove, 175.Width());
                remWidth -= 178;
                Space(20); remWidth -= 20;
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
        internal static void UpdateSearchResults(string searchText, IEnumerable<Def> definitions, Func<Def, string> searchAndSortKey)
        {
            if (definitions == null) return;
            var terms = searchText.Split(' ').Select(s => s.ToLower()).ToHashSet();
            var filtered = new List<Def>();
            foreach (var def in definitions)
            {
                if (def.GetType().ToString().Contains(searchText)
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
            matchCount = filtered.Count();
            filteredDefitions = filtered.OrderBy(def => searchAndSortKey(def)).Take(searchLimit).ToArray();
        }
    }
}
