using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SolastaUnfinishedBusiness.Api.ModKit.Utility.Extensions;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit;
internal static partial class UI
{
    internal class Browser<T, Item, Def> // for many things the item will be the definition
    {
        public static IEnumerable<Def> filteredDefitions = null;
        private static string prevCallerKey = "";
        private static string searchText = "";
        private static int searchLimit = 100;
        public static int matchCount = 0;
        private static bool showAll = false;

        internal List<Action> OnGUI(string callerKey,
                                    T target,
                                    Func<T, List<Item>> current,
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
            List<Action> todo = new();
            var searchChanged = false;
//            var refreshTree = false;
            if (callerKey != prevCallerKey) { searchChanged = true; showAll = false; }
            prevCallerKey = callerKey;
            using (HorizontalScope())
            {
                100.space();
                ActionTextField(ref searchText, "searhText", null, () => { searchChanged = true; }, Width(320));
                25.space();
                Label("Limit", ExpandWidth(false));
                ActionIntTextField(ref searchLimit, "searchLimit", null, () => { searchChanged = true; }, Width(175));
                if (searchLimit > 1000) { searchLimit = 1000; }
                25.space();
                searchChanged |= DisclosureToggle("Show All".orange().bold(), ref showAll);
            }
            using (HorizontalScope())
            {
                Space(100);
                ActionButton("Search", () => { searchChanged = true; }, AutoWidth());
                Space(25);
                if (matchCount > 0 && searchText.Length > 0)
                {
                    var matchesText = "Matches: ".green().bold() + $"{matchCount}".orange().bold();
                    if (matchCount > searchLimit) { matchesText += " => ".cyan() + $"{searchLimit}".cyan().bold(); }
                    Label(matchesText, ExpandWidth(false));
                }
            }
            var remainingWidth = ummWidth;
            if (showAll)
            {
                UpdateSearchResults(searchText, available, searchAndSortKey);
                
            }
            var terms = searchText.Split(' ').Select(s => s.ToLower()).ToHashSet();

            var sorted = current(target).OrderBy((item) => title(definition(item)));
            matchCount = 0;
            Div(100);
            foreach (var item in sorted)
            {
                var name = title(definition(item));
                var nameLower = name.ToLower();
                if (name != null && name.Length > 0 && (searchText.Length == 0 || terms.All(term => nameLower.Contains(term))))
                {

                    OnRowGUI(target, definition(item), item, title, description, value, setValue, incrementValue, decrementValue, addItem, removeItem);
                }
            }
            return todo;
        }
        internal static void OnRowGUI(T target,
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
            if (item == null) return;
            {
                matchCount++;
                using (HorizontalScope())
                {
                    Space(100); remWidth -= 100;
                    var titleWidth = (ummWidth / (IsWide ? 3.0f : 4.0f)) - 100;
                    string text = title(definition).cyan().bold();
                    Label(text, Width(titleWidth));
                    remWidth -= titleWidth;
                    Space(10); remWidth -= 10;
                    if (value != null && value(item) is string stringValue)
                    {
                        if (decrementValue != null && decrementValue(target, item) is Action decrementer)
                            ActionButton("<", decrementer, 60.width());
                        else
                            63.space();
                        Space(10f);
                        Label($"{stringValue}".orange().bold(), Width(30));
                        if (incrementValue != null && incrementValue(target, item) is Action incrementer)
                            ActionButton(">", incrementer, 60.width());
                        else
                            63.space();
                        remWidth -= 166;
                    }
                    UI.Space(30);
                    if (addItem != null && addItem(target, definition) is Action add)
                        ActionButton("@UI/Add".Localized(), add, 150.width());
                    else
                        153.space();
                    remWidth -= 153;
                    Space(10); remWidth -= 10;
                    if (removeItem != null && removeItem(target, item) is Action remove)
                        ActionButton("@UI/Add".Localized(), remove, 175.width());
                    else
                        178.space();
                    remWidth -= 178;
                    Space(20); remWidth -= 20;
                    using (VerticalScope(Width(remWidth - 100)))
                    {
                        if (description != null)
                        {
                            Label(description(definition).StripHTML().green(), Width(remWidth - 100));
                        }
                    }
                }
                Div(100);
            }
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

    internal class Browser<T, Item> : Browser<T, Item, Item> { }
}
