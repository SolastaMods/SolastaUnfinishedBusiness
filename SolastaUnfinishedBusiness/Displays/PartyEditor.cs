// Copyright < 2021 > Narria (github user Cabarius) - License: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Api.ModKit;
using static SolastaUnfinishedBusiness.Api.ModKit.UI;
using UnityEngine;
using static RulesetCharacterHero;
using System.IO;
using SolastaUnfinishedBusiness.Api.Extensions;
using System.Reflection;
using System.Web.ClientServices.Providers;
using System.Web.UI.WebControls;
using System.Runtime.Remoting.Messaging;

namespace SolastaUnfinishedBusiness.Displays {
    public class PartyEditor
    {
        private enum ToggleChoice
        {
            Classes,
            Stats,
            Facts,
            Features,
            Buffs,
            Abilities,
            Spells,
            None,
        };

        private static ToggleChoice selectedToggle = ToggleChoice.None;
        private static int selectedCharacterIndex = 0;
        private static bool editingFromPool = false;
        private static ICharacterPoolService poolService => ServiceRepository.GetService<ICharacterPoolService>();

        private static (string, string) nameEditState = (null, null);
        public static void OnGUI()
        {
            Label("Experimental Preview:".Localized().Orange().bold() + " " + "This simple party editor lets you edit characters in a loaded game session. Right now it lets you edit your character's first and last name. More features are coming soon (tm). Please click on the following to report issues:".Localized().green());
            LinkButton("https://github.com/SolastaMods/SolastaUnfinishedBusiness/issues", "https://github.com/SolastaMods/SolastaUnfinishedBusiness/issues");
            var characters = GetCharacterList();
            if (characters == null)
            {
                Label("****** Party Editor unavailable: Please load a save game ******".Localized().yellow().bold());
            }
            else
            {
                var commandService = ServiceRepository.GetService<ICommandService>();
                Space(15);
                HStack("Quickies".Localized(), 2,
                    () => ActionButton($"Long Rest", () => commandService.StartRest(RuleDefinitions.RestType.LongRest, false),
                    AutoWidth())
                    );
                Div();
                Label("Current Party".Localized().Cyan().Bold());
                using (UI.VerticalScope())
                {
                    var chIndex = 0;
                    foreach (var ch in characters)
                    {
                        var selectedCharacter = GetSelectedCharacter();
                        bool changed = false;
                        var level = ch.TryGetAttributeValue("CharacterLevel");
                        using (UI.HorizontalScope())
                        {
                            var name = ch.Name;
                            if (ch is RulesetCharacterHero hero)
                            {
                                name = hero.Name + " " + hero.SurName;
                                if (EditableLabel(ref name, ref nameEditState, 200, n => n.orange().bold(), MinWidth(100), MaxWidth(600)))
                                {
                                    var parts = name.Split();
                                    if (parts.Length > 1)
                                    {
                                        hero.Name = parts[0];
                                        hero.SurName = String.Join(" ", parts.Skip(1).ToArray()).Trim();
                                    }
                                    if (parts.Length == 1)
                                    {
                                        hero.Name = parts[0];
                                        hero.SurName = "";
                                    };
                                    changed = true;
                                }
                            }
                            else if (EditableLabel(ref name, ref nameEditState, 200, n => n.orange().bold(), MinWidth(100), MaxWidth(600)))
                            {
                                ch.Name = name;
                                changed = true;
                            }
                            Space(5);
                            Label((level < 10 ? "   lvl" : "   lv").green() + $" {level}", Width(90));
                            Space(5);
                            var showStats = ch == selectedCharacter && selectedToggle == ToggleChoice.Stats;
                            if (DisclosureToggle("Stats", ref showStats, 125))
                            {
                                if (showStats) { selectedCharacter = ch; selectedToggle = ToggleChoice.Stats; }
                                else { selectedToggle = ToggleChoice.None; }
                            }

                        }
                        if (ch == selectedCharacter && selectedToggle == ToggleChoice.Stats)
                        {
                            Div(100, 20, 755);

                            foreach (var attr in ch.Attributes)
                            {
                                var attrName = attr.Key;
                                var attribute = attr.Value;
                                var baseValue = attribute.baseValue;
                                var modifiers = attribute.ActiveModifiers.Select(m => $"{m.Value.ToString("+0;-#")} {String.Join(" ",m.Tags).TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9').Cyan()}").ToArray();
                                var modifiersString = String.Join(" ", modifiers);
                                using (HorizontalScope())
                                {
                                    Space(100);
                                    Label(attrName, Width(400f));
                                    Space(25);
                                    ActionButton(" < ", () =>
                                    {
                                        attribute.baseValue -= 1;
                                        changed = true;
                                    }, GUI.skin.box, AutoWidth());
                                    Space(20);
                                    Label($"{attribute.currentValue}".orange().bold(), Width(50f));
                                    ActionButton(" > ", () =>
                                    {
                                        attribute.baseValue += 1;
                                        changed = true;
                                    }, GUI.skin.box, AutoWidth());
                                    Space(25);
                                    ActionIntTextField(ref baseValue, (v) => {
                                        attribute.baseValue = v;
                                        changed = true;
                                    }, Width(75));
                                    Space(10);
                                    Label($"{modifiersString}");
                                }
                            }
                            if (changed)
                                ch.RefreshAll();
                        }
                        if (changed && editingFromPool && ch is RulesetCharacterHero h)
                        {
                            Mod.Debug(String.Format("Saving Pool Character: " + h.Name));
                            Mod.Debug(poolService.SaveCharacter(h));
                            // h.RefreshAll();
                            // RefreshPool();
                        }
                        if (selectedCharacter != GetSelectedCharacter())
                        {
                            selectedCharacterIndex = GetCharacterList().IndexOf(selectedCharacter);
                        }
                        chIndex += 1;
                    }
                }
            }
        }

        public static List<RulesetCharacter> GetCharacterList()
        {
           editingFromPool = false;

#pragma warning disable IDE0031
            // don't use ? or ?? or a type deriving from an UnityEngine.Object to avoid bypassing lifetime check
            var chars = Gui.GameCampaign == null ? null : Gui.GameCampaign.Party.CharactersList.Select((ch) => ch.RulesetCharacter).ToList();
#if DEBUG
        if (chars == null)
            {
                chars = CharacterPool;
                editingFromPool = true;
            }
#endif
#pragma warning restore IDE0031
            return chars;
        }

        private static RulesetCharacter GetSelectedCharacter()
        {
            var characterList = GetCharacterList();
            if (characterList == null || characterList.Count == 0) return null;
            if (selectedCharacterIndex >= characterList.Count) selectedCharacterIndex = 0;
            return characterList[selectedCharacterIndex];
        }

        private static List<RulesetCharacter> characterPool = null;
        public static List<RulesetCharacter> CharacterPool
        {
            get
            {
                if (characterPool == null)
                {
                    RefreshPool();
                }
                return characterPool;
            }
            set { characterPool = value; }
        }
        private static void RefreshPool()
        {
            characterPool = new List<RulesetCharacter>();
            ICharacterPoolService poolService = ServiceRepository.GetService<ICharacterPoolService>();

            poolService.EnumeratePool();
            foreach (var item in poolService.Pool)
            {
                var filename = item.Key;
                Mod.Log("Loading: " + filename);
                var snapshot = item.Value;
                RulesetCharacterHero h;
                RulesetCharacterHero.Snapshot s;
                poolService.LoadCharacter(filename, out h, out s);
#if false
                        Mod.Debug(h.Name + " " + h);
                        PropertyInfo[] infos = h.GetType().GetProperties();
                        Mod.Debug("" +  infos);
                        foreach (PropertyInfo info in infos)
                        {
                            Mod.Debug(String.Format("    {0} : {1}", info.Name, info.GetValue(h, null)?.ToString()) ?? "null");
                        }
#endif
                characterPool.Add(h);
            }

            Mod.Log(string.Format("{0} Characters Loaded", characterPool.Count));
        }
    }
}
