// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.DataViewer;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.ModKit.UI;

namespace SolastaUnfinishedBusiness.Displays;

public static class PartyEditor
{
    private static ToggleChoice _selectedToggle = ToggleChoice.None;
    private static int _selectedCharacterIndex;
    private static RulesetCharacter _selectedCharacter;
    private static (string, string) _nameEditState = (null, null);

#if DEBUG
    private static ICharacterPoolService PoolService => ServiceRepository.GetService<ICharacterPoolService>();
#endif

    internal static void OnGUI()
    {
        Label("Experimental Preview:".Localized().Orange().Bold() + " " + "PartyEditorMessage".Localized().Green());

        var characters = GetCharacterList();

        if (characters == null)
        {
            Label("PartyEditorUnavailable".Localized().Yellow().Bold());
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
                    if (ch is not RulesetCharacterHero hero)
                    {
                        continue;
                    }

                    var changed = false;
                    var level = ch.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);

                    using (HorizontalScope())
                    {
                        var name = hero.Name + " " + hero.SurName;

                        if (EditableLabel(ref name, ref _nameEditState, 200,
                                n => n.Orange().Bold(),
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

                    if (ch == _selectedCharacter && _selectedToggle == ToggleChoice.Skills)
                    {
                        var skills = BlueprintDisplay.GetBlueprints()?.OfType<SkillDefinition>()
                            .Cast<BaseDefinition>();
                        var tools = BlueprintDisplay.GetBlueprints()?.OfType<ToolTypeDefinition>()
                            .Cast<BaseDefinition>();

                        if (skills != null && tools != null)
                        {
                            var available = skills.Union(tools);
                            var baseDefinitions = available as BaseDefinition[] ?? available.ToArray();
                            var skillsDict = baseDefinitions.ToDictionary(e => e.Name, e => e);
                            var expertisesHash = hero.TrainedExpertises.ToHashSet();
                            var currentSkills = hero.TrainedSkills.Cast<BaseDefinition>();
                            var currentTools = hero.TrainedToolTypes.Cast<BaseDefinition>();
                            var current = currentSkills.Union(currentTools).ToList();

                            Browser<RulesetCharacterHero, BaseDefinition, BaseDefinition>.OnGUI(
                                _selectedToggle.ToString(), ref changed,
                                hero,
                                current,
                                baseDefinitions,
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
                                (chr, item) =>
                                {
                                    // Increment
                                    if (!skillsDict.ContainsKey(item.Name))
                                    {
                                        return () =>
                                        {
                                            switch (item)
                                            {
                                                case SkillDefinition skill:
                                                    chr.TrainedSkills.Add(skill);
                                                    break;
                                                case ToolTypeDefinition tool:
                                                    chr.TrainedToolTypes.Add(tool);
                                                    break;
                                            }
                                        };
                                    }

                                    if (expertisesHash.Contains(item.Name))
                                    {
                                        return null;
                                    }

                                    return () => chr.TrainedExpertises.Add(item.Name);
                                },
                                (chr, item) =>
                                {
                                    // Decrement
                                    if (!skillsDict.ContainsKey(item.Name))
                                    {
                                        return null;
                                    }

                                    if (expertisesHash.Contains(item.Name))
                                    {
                                        return () => chr.TrainedExpertises.Remove(item.Name);
                                    }

                                    return () =>
                                    {
                                        switch (item)
                                        {
                                            case SkillDefinition skill:
                                                chr.TrainedSkills.Remove(skill);
                                                break;
                                            case ToolTypeDefinition tool:
                                                chr.TrainedToolTypes.Remove(tool);
                                                break;
                                        }

                                        chr.TrainedExpertises.Remove(item.Name);
                                    };
                                },
                                (chr, def) =>
                                {
                                    return def switch
                                    {
                                        // Add
                                        SkillDefinition skill when !chr.TrainedSkills.Contains(skill) => () =>
                                            chr.TrainedSkills.Add(skill),
                                        ToolTypeDefinition tool when !chr.TrainedToolTypes.Contains(tool) => () =>
                                            chr.TrainedToolTypes.Add(tool),
                                        _ => null
                                    };
                                },
                                (chr, def) =>
                                {
                                    return def switch
                                    {
                                        // Remove
                                        SkillDefinition skill when chr.TrainedSkills.Contains(skill) => () =>
                                        {
                                            chr.TrainedSkills.Remove(skill);
                                            chr.TrainedExpertises.Remove(skill.Name);
                                        },
                                        ToolTypeDefinition tool when chr.TrainedToolTypes.Contains(tool) => () =>
                                        {
                                            chr.TrainedToolTypes.Remove(tool);
                                            chr.TrainedExpertises.Remove(tool.Name);
                                        },
                                        _ => null
                                    };
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
                                _selectedToggle.ToString(), ref changed,
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
                                (chr, feat) => !chr.TrainedFeats.Contains(feat)
                                    ? () =>
                                    {
                                        chr.TrainFeats([feat]);

                                        LevelUpContext.RecursiveGrantCustomFeatures(
                                            chr, AttributeDefinitions.TagFeat, feat.Features);
                                    }
                                    : null,
                                (chr, feat) => chr.TrainedFeats.Contains(feat)
                                    ? () =>
                                    {
                                        chr.TrainedFeats.Remove(feat);

                                        LevelUpContext.RecursiveRemoveCustomFeatures(
                                            chr, AttributeDefinitions.TagFeat, feat.Features);
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
                        var available = BlueprintDisplay.GetBlueprints()?.OfType<InvocationDefinition>();
                        //.Where(x => x is not InvocationDefinitionCustom);

                        if (available != null)
                        {
                            Browser<RulesetCharacterHero, InvocationDefinition, InvocationDefinition>.OnGUI(
                                _selectedToggle.ToString(), ref changed,
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
                                (chr, def) => !chr.TrainedInvocations.Contains(def)
                                    ? () =>
                                    {
                                        chr.TrainInvocations([def]);
                                        LevelUpContext.RecursiveGrantCustomFeatures(
                                            chr, null, [def.grantedFeature]);
                                    }
                                    : null,
                                (chr, def) => chr.TrainedInvocations.Contains(def)
                                    ? () =>
                                    {
                                        chr.TrainedInvocations.Remove(def);
                                        if (def.grantedFeature is FeatureDefinitionPower power)
                                        {
                                            chr.usablePowers.RemoveAll(x => x.PowerDefinition == power);
                                        }

                                        LevelUpContext.RecursiveRemoveCustomFeatures(
                                            chr, null, [def.grantedFeature]);
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

                    if (_selectedCharacter != GetSelectedCharacter())
                    {
                        _selectedCharacterIndex = GetCharacterList().IndexOf(_selectedCharacter);
                    }
                }
            }
        }
    }

    private static void OnDetailToggle(ToggleChoice choice, RulesetCharacter ch)
    {
        var show = ch == _selectedCharacter && _selectedToggle == choice;

        if (!DisclosureToggle(choice.ToString(), ref show, 125))
        {
            return;
        }

        if (show)
        {
            _selectedCharacter = ch;
            _selectedToggle = choice;
        }
        else
        {
            _selectedToggle = ToggleChoice.None;
        }
    }

    private static List<RulesetCharacter> GetCharacterList()
    {
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

#if DEBUG
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
#endif

    private enum ToggleChoice
    {
        Skills,
        Feats,
        Invocations,

        // Spells,
        // Classes,
        Stats,
        None
    }

#if DEBUG
    private static List<RulesetCharacter> _characterPool;

    private static List<RulesetCharacter> CharacterPool
    {
        get
        {
            if (_characterPool == null)
            {
                RefreshPool();
            }

            return _characterPool;
        }
        // set => _characterPool = value;
    }
#endif
}
