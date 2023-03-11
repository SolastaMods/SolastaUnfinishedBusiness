// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.DataViewer;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.ModKit.UI;

namespace SolastaUnfinishedBusiness.Displays;

public static class PartyEditor
{
    private static ToggleChoice _selectedToggle = ToggleChoice.None;
    private static int _selectedCharacterIndex;
    private static RulesetCharacter _selectedCharacter;
    private static bool _editingFromPool;

    private static (string, string) _nameEditState = (null, null);

    private static List<RulesetCharacter> _characterPool;
    private static ICharacterPoolService PoolService => ServiceRepository.GetService<ICharacterPoolService>();

    public static List<RulesetCharacter> CharacterPool
    {
        get
        {
            if (_characterPool == null)
            {
                RefreshPool();
            }

            return _characterPool;
        }
        set => _characterPool = value;
    }

    internal static void OnGUI()
    {
        Label("Experimental Preview:".Localized().Orange().Bold() + " " +
              "This simple party editor lets you edit characters in a loaded game session. Right now it lets you edit your character's first and last name. More features are coming soon (tm). Please click on the following to report issues:"
                  .Localized().Green());
        LinkButton("https://github.com/SolastaMods/SolastaUnfinishedBusiness/issues",
            "https://github.com/SolastaMods/SolastaUnfinishedBusiness/issues");
        var characters = GetCharacterList();
        if (characters == null)
        {
            Label("****** Party Editor unavailable: Please load a save game ******".Localized()
                .Yellow().Bold());
        }
        else
        {
            var commandService = ServiceRepository.GetService<ICommandService>();
            Space(15);
            HStack("Quickies".Localized(), 2,
                () => ActionButton("Long Rest",
                    () => commandService.StartRest(RuleDefinitions.RestType.LongRest, false),
                    AutoWidth()),
                () => ActionButton("Refresh Powers",
                    () => characters.ForEach(ch =>
                    {
                        ch.GrantPowers();
                        ch.GrantInvocations();
                        ch.GrantFixedSpells();
                    }),
                    AutoWidth()),
                () => { }
            );
            Div();
            Label("Current Party".Localized().Cyan().Bold());
            using (VerticalScope())
            {
                foreach (var ch in characters)
                {
                    if (ch is RulesetCharacterHero hero)
                    {
                        var changed = false;
                        var level = ch.TryGetAttributeValue("CharacterLevel");
                        using (HorizontalScope())
                        {
                            var name = hero.Name + " " + hero.SurName;
                            if (EditableLabel(ref name, ref _nameEditState, 200, n => n.Orange().Bold(),
                                    MinWidth(100),
                                    MaxWidth(600)))
                            {
                                var parts = name.Split();
                                switch (parts.Length)
                                {
                                    case > 1:
                                        hero.Name = parts[0];
                                        hero.SurName = String.Join(" ", parts.Skip(1).ToArray()).Trim();
                                        break;
                                    case 1:
                                        hero.Name = parts[0];
                                        hero.SurName = "";
                                        break;
                                }

                                changed = true;
                            }

                            Space(5);
                            Label((level < 10 ? "   lvl" : "   lv").Green() + $" {level}", Width(90));
                            Space(5);
                            OnDetailToggle(ToggleChoice.Stats, ch);
                            OnDetailToggle(ToggleChoice.Skills, ch);
                            OnDetailToggle(ToggleChoice.Feats, ch);
                            OnDetailToggle(ToggleChoice.Invocations, ch);
                        }

                        if (ch == _selectedCharacter && _selectedToggle == ToggleChoice.Stats)
                        {
                            Div(100, 20, 755);

                            foreach (var attr in ch.Attributes)
                            {
                                var attrName = attr.Key;
                                var attribute = attr.Value;
                                var baseValue = attribute.baseValue;
                                var modifiers = attribute.ActiveModifiers.Where(m => m.Value != 0).Select(m =>
                                        $"{m.Value:+0;-#} {String.Join(" ", m.Tags).TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9').Cyan()}")
                                    .ToArray();
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
                                    Label($"{attribute.currentValue}".Orange().Bold(), Width(50f));
                                    ActionButton(" > ", () =>
                                    {
                                        attribute.baseValue += 1;
                                        changed = true;
                                    }, GUI.skin.box, AutoWidth());
                                    Space(25);
                                    ActionIntTextField(ref baseValue, v =>
                                    {
                                        attribute.baseValue = v;
                                        changed = true;
                                    }, Width(75));
                                    Space(10);
                                    Label($"{modifiersString}");
                                }
                            }
                        }

                        if (ch == _selectedCharacter && BlueprintLoader.Shared.LoadInProgress())
                        {
                            using (HorizontalScope())
                            {
                                GUILayout.Label("Loading: " +
                                                BlueprintLoader.Shared.Progress.ToString("P2").Cyan().Bold());
                            }

                            continue;
                        }
#if false
                        if (ch == _selectedCharacter && _selectedToggle == ToggleChoice.Skills)
                        {
                            var available = BlueprintDisplay.GetBlueprints()?.OfType<SkillDefinition>();
                            if (available != null)
                                Browser<RulesetCharacterHero, SkillDefinition, SkillDefinition>.OnGUI(
                                    _selectedToggle.ToString(),
                                    hero,
                                    hero.TrainedSkills,
                                    available,
                                    (i) => i,
                                    (skill) => skill.Name,
                                    (skill) => skill.Name,
                                    (skill) => skill.FormatDescription(),
                                    null,
                                    null,
                                    null,
                                    null,
                                    (hero, skill) => !hero.TrainedSkills.Contains(skill) ?  () => { 
                                        hero.TrainedSkills.Add(skill); changed = true; 
                                    } : null,
                                    (hero, skill) => hero.TrainedSkills.Contains(skill) ? () => { 
                                        hero.TrainedSkills.Remove(skill); changed = true; 
                                    } : null
                                    );
                        }
#endif
                        if (ch == _selectedCharacter && _selectedToggle == ToggleChoice.Skills)
                        {
                            var skills = BlueprintDisplay.GetBlueprints()?.OfType<SkillDefinition>()
                                .Cast<BaseDefinition>();
                            var tools = BlueprintDisplay.GetBlueprints()?.OfType<ToolTypeDefinition>()
                                .Cast<BaseDefinition>();
                            if (skills != null && tools != null)
                            {
                                var available = skills.Union(tools);
                                var skillsDict = available.ToDictionary(e => e.Name, e => e);
                                var expertisesHash = hero.TrainedExpertises.ToHashSet();
                                var currentSkills = hero.TrainedSkills.Cast<BaseDefinition>();
                                var currentTools = hero.TrainedToolTypes.Cast<BaseDefinition>();
                                var current = currentSkills.Union(currentTools).ToList();
                                Browser<RulesetCharacterHero, BaseDefinition, BaseDefinition>.OnGUI(
                                    _selectedToggle.ToString(),
                                    hero,
                                    current,
                                    available,
                                    item => skillsDict[item.Name],
                                    def => def.Name,
                                    def => def.Name,
                                    def => def.FormatDescription(),
                                    item => skillsDict.ContainsKey(item.Name)
                                        ? expertisesHash.Contains(item.Name)
                                            ? "Expert".Localized()
                                            : "Skilled".Localized()
                                        : "-",
                                    null,
                                    (hero, item) =>
                                    {
                                        // Increment
                                        if (skillsDict.ContainsKey(item.Name))
                                        {
                                            if (expertisesHash.Contains(item.Name))
                                            {
                                                return null;
                                            }

                                            return () => hero.TrainedExpertises.Add(item.Name);
                                        }

                                        return () =>
                                        {
                                            if (item is SkillDefinition skill)
                                            {
                                                hero.TrainedSkills.Add(skill);
                                            }
                                            else if (item is ToolTypeDefinition tool)
                                            {
                                                hero.TrainedToolTypes.Add(tool);
                                            }
                                        };
                                    },
                                    (hero, item) =>
                                    {
                                        // Decrement
                                        if (skillsDict.ContainsKey(item.Name))
                                        {
                                            if (expertisesHash.Contains(item.Name))
                                            {
                                                return () => hero.TrainedExpertises.Remove(item.Name);
                                            }

                                            return () =>
                                            {
                                                if (item is SkillDefinition skill)
                                                {
                                                    hero.TrainedSkills.Remove(skill);
                                                }
                                                else if (item is ToolTypeDefinition tool)
                                                {
                                                    hero.TrainedToolTypes.Remove(tool);
                                                }

                                                hero.TrainedExpertises.Remove(item.Name);
                                            };
                                        }

                                        return null;
                                    },
                                    (hero, def) =>
                                    {
                                        // Add
                                        if (def is SkillDefinition skill && !hero.TrainedSkills.Contains(skill))
                                        {
                                            return () => hero.TrainedSkills.Add(skill);
                                        }

                                        if (def is ToolTypeDefinition tool && !hero.TrainedToolTypes.Contains(tool))
                                        {
                                            return () => hero.TrainedToolTypes.Add(tool);
                                        }

                                        return null;
                                    },
                                    (hero, def) =>
                                    {
                                        // Remove
                                        if (def is SkillDefinition skill && hero.TrainedSkills.Contains(skill))
                                        {
                                            return () =>
                                            {
                                                hero.TrainedSkills.Remove(skill);
                                                hero.TrainedExpertises.Remove(skill.Name);
                                            };
                                        }

                                        if (def is ToolTypeDefinition tool && hero.TrainedToolTypes.Contains(tool))
                                        {
                                            return () =>
                                            {
                                                hero.TrainedToolTypes.Remove(tool);
                                                hero.TrainedExpertises.Remove(tool.Name);
                                            };
                                        }

                                        return null;
                                    }
                                );
                            }
                        }

                        if (ch == _selectedCharacter && _selectedToggle == ToggleChoice.Feats)
                        {
                            var available = BlueprintDisplay.GetBlueprints()?.OfType<FeatDefinition>();
                            if (available != null)
                            {
                                Browser<RulesetCharacterHero, FeatDefinition, FeatDefinition>.OnGUI(
                                    _selectedToggle.ToString(),
                                    hero,
                                    hero.TrainedFeats,
                                    available,
                                    feat => feat,
                                    feat => feat.Name,
                                    feat => feat.Name,
                                    feat => feat.FormatDescription(),
                                    null,
                                    null,
                                    null,
                                    null,
                                    (hero, feat) => !hero.TrainedFeats.Contains(feat)
                                        ? () =>
                                        {
                                            hero.TrainFeats(new List<FeatDefinition> { feat });
                                            changed = true;
                                        }
                                        : null,
                                    (hero, feat) => hero.TrainedFeats.Contains(feat)
                                        ? () =>
                                        {
                                            hero.TrainedFeats.Remove(feat);
                                            changed = true;
                                        }
                                        : null
                                );
                            }

                            if (changed)
                            {
                                hero.GrantPowers();
                            }
                        }

                        if (ch == _selectedCharacter && _selectedToggle == ToggleChoice.Invocations)
                        {
                            var available = BlueprintDisplay.GetBlueprints()?.OfType<InvocationDefinition>()
                                .Where(x => x is not InvocationDefinitionCustom);
                            if (available != null)
                            {
                                Browser<RulesetCharacterHero, InvocationDefinition, InvocationDefinition>.OnGUI(
                                    _selectedToggle.ToString(),
                                    hero,
                                    hero.TrainedInvocations,
                                    available,
                                    item => item,
                                    def => def.Name,
                                    def => def.Name,
                                    def => def.FormatDescription(),
                                    null,
                                    null,
                                    null,
                                    null,
                                    (hero, def) => !hero.TrainedInvocations.Contains(def)
                                        ? () =>
                                        {
                                            hero.TrainInvocations(new List<InvocationDefinition> { def });
                                            changed = true;
                                        }
                                        : null,
                                    (hero, def) => hero.TrainedInvocations.Contains(def)
                                        ? () =>
                                        {
                                            hero.TrainedInvocations.Remove(def);
                                            changed = true;
                                        }
                                        : null
                                );
                            }

                            if (changed)
                            {
                                hero.GrantInvocations();
                            }
                        }

                        if (changed)
                        {
                            ch.RefreshAll();
                        }

                        if (changed && _editingFromPool && ch is RulesetCharacterHero h)
                        {
                            // ReSharper disable once InvocationIsSkipped
                            Main.Log(String.Format("Saving Pool Character: " + h.Name));
                            // ReSharper disable once InvocationIsSkipped
                            Main.Log(PoolService.SaveCharacter(h));
                            // h.RefreshAll();
                            // RefreshPool();
                        }

                        if (_selectedCharacter != GetSelectedCharacter())
                        {
                            _selectedCharacterIndex = GetCharacterList().IndexOf(_selectedCharacter);
                        }
                    }
                }
            }
        }
    }

    internal static void OnDetailToggle(ToggleChoice choice, RulesetCharacter ch)
    {
        var show = ch == _selectedCharacter && _selectedToggle == choice;
        if (DisclosureToggle(choice.ToString(), ref show, 125))
        {
            if (show)
            {
                _selectedCharacter = ch;
                _selectedToggle = choice;
            }
            else { _selectedToggle = ToggleChoice.None; }
        }
    }

    private static List<RulesetCharacter> GetCharacterList()
    {
        _editingFromPool = false;

#pragma warning disable IDE0031
        // don't use ? or ?? or a type deriving from an UnityEngine.Object to avoid bypassing lifetime check
        var chars = Gui.GameCampaign == null
            ? null
            : Gui.GameCampaign.Party?.CharactersList?.Select(ch => ch.RulesetCharacter).ToList();
#if DEBUG
        if (chars != null)
        {
            return chars;
        }

        chars = CharacterPool;
        _editingFromPool = true;
#endif
#pragma warning restore IDE0031
        return chars;
    }

    private static RulesetCharacter GetSelectedCharacter()
    {
        var characterList = GetCharacterList();
        if (characterList == null || characterList.Count == 0)
        {
            return null;
        }

        if (_selectedCharacterIndex >= characterList.Count || _selectedCharacterIndex < 0)
        {
            _selectedCharacterIndex = 0;
        }

        return characterList[_selectedCharacterIndex];
    }

    private static void RefreshPool()
    {
        _characterPool = new List<RulesetCharacter>();
        PoolService.EnumeratePool();

        foreach (var filename in PoolService.Pool.Select(item => item.Key))
        {
            // ReSharper disable once InvocationIsSkipped
            Main.Log("Loading: " + filename);
            PoolService.LoadCharacter(filename, out var h, out _);
#if false
                        Mod.Debug(h.Name + " " + h);
                        PropertyInfo[] infos = h.GetType().GetProperties();
                        Mod.Debug("" +  infos);
                        foreach (PropertyInfo info in infos)
                        {
                            Mod.Debug(String.Format("    {0} : {1}", info.Name, info.GetValue(h, null)?.ToString()) ?? "null");
                        }
#endif
            _characterPool.Add(h);
        }

        // ReSharper disable once InvocationIsSkipped
        Main.Log($"{_characterPool.Count} Characters Loaded");
    }

    internal enum ToggleChoice
    {
        Skills,
        Feats,
        Invocations,
        Classes,
        Stats,
        Spells,
        None
    }
}
