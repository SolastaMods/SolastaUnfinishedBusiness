using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Models;

internal static class MulticlassPatchingContext
{
    private const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly MethodInfo NullMethod = null;

    // these features will be replaced to comply to SRD multiclass rules
    private static readonly Dictionary<FeatureDefinition, FeatureDefinition> FeaturesToReplace = new()
    {
        { ProficiencyBarbarianArmor, ArmorProficiencyMulticlassBuilder.BarbarianArmorProficiencyMulticlass },
        { ProficiencyFighterArmor, ArmorProficiencyMulticlassBuilder.FighterArmorProficiencyMulticlass },
        { ProficiencyPaladinArmor, ArmorProficiencyMulticlassBuilder.PaladinArmorProficiencyMulticlass },
        { PointPoolBardSkillPoints, SkillProficiencyPointPoolSkillsBuilder.PointPoolRogueSkillPointsMulticlass },
        { PointPoolRangerSkillPoints, SkillProficiencyPointPoolSkillsBuilder.PointPoolRangerSkillPointsMulticlass },
        { PointPoolRogueSkillPoints, SkillProficiencyPointPoolSkillsBuilder.PointPoolRogueSkillPointsMulticlass }
    };

    // these features will be removed to comply with SRD multiclass rules
    private static readonly Dictionary<CharacterClassDefinition, List<FeatureDefinition>> FeaturesToExclude = new()
    {
        {
            Barbarian, new List<FeatureDefinition> { PointPoolBarbarianrSkillPoints, ProficiencyBarbarianSavingThrow }
        },
        {
            Cleric,
            new List<FeatureDefinition>
            {
                ProficiencyClericWeapon, PointPoolClericSkillPoints, ProficiencyClericSavingThrow
            }
        },
        { Druid, new List<FeatureDefinition> { PointPoolDruidSkillPoints, ProficiencyDruidSavingThrow } },
        { Fighter, new List<FeatureDefinition> { PointPoolFighterSkillPoints, ProficiencyFighterSavingThrow } },
        { Monk, new List<FeatureDefinition> { PointPoolMonkSkillPoints, ProficiencyMonkSavingThrow } },
        { Paladin, new List<FeatureDefinition> { PointPoolPaladinSkillPoints, ProficiencyPaladinSavingThrow } },
        { Ranger, new List<FeatureDefinition> { ProficiencyRangerSavingThrow } },
        { Rogue, new List<FeatureDefinition> { ProficiencyRogueWeapon, ProficiencyRogueSavingThrow } },
        {
            Sorcerer,
            new List<FeatureDefinition>
            {
                ProficiencySorcererWeapon, PointPoolSorcererSkillPoints, ProficiencySorcererSavingThrow
            }
        },
        {
            Warlock,
            new List<FeatureDefinition>
            {
                ProficiencyWarlockWeapon, PointPoolWarlockSkillPoints, ProficiencyWarlockSavingThrow
            }
        },
        {
            Wizard,
            new List<FeatureDefinition>
            {
                ProficiencyWizardWeapon, PointPoolWizardSkillPoints, ProficiencyWizardSavingThrow
            }
        }
    };

    private static (MethodInfo, HeroContext) FeatureUnlocksContext { get; set; }

    internal static void Load()
    {
        PatchClassLevel();
        PatchEquipmentAssignment();
        PatchFeatureUnlocks();
        AddNonOfficialBlueprintsToFeaturesCollections();
    }

    private static void AddNonOfficialBlueprintsToFeaturesCollections()
    {
        var dbFeatureDefinitionPointPool = DatabaseRepository.GetDatabase<FeatureDefinitionPointPool>();
        var dbFeatureDefinitionProficiency = DatabaseRepository.GetDatabase<FeatureDefinitionProficiency>();

        //
        // add these later as need to wait for these blueprints to be instantiated and not willing to publicise CE
        //

        // FeaturesToExclude.Add(TinkererClass,
        //     new List<FeatureDefinition>
        //     {
        //         dbFeatureDefinitionPointPool.GetElement("PointPoolTinkererSkillPoints"),
        //         dbFeatureDefinitionProficiency.GetElement("ProficiencyWeaponTinkerer"),
        //         dbFeatureDefinitionProficiency.GetElement("ProficiencyTinkererSavingThrow")
        //     });
    }

    //
    // ClassLevel patching support
    //

    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<CodeInstruction> ClassLevelTranspiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var classesAndLevelsMethod = typeof(RulesetCharacterHero).GetMethod("get_ClassesAndLevels");
        var classesHistoryMethod = typeof(RulesetCharacterHero).GetMethod("get_ClassesHistory");
        var getClassLevelMethod = typeof(LevelUpContext).GetMethod("GetSelectedClassLevel");

        var instructionsToBypass = 0;

        foreach (var instruction in instructions)
        {
            if (instructionsToBypass > 0)
            {
                instructionsToBypass--;
            }
            else if (instruction.Calls(classesAndLevelsMethod))
            {
                yield return new CodeInstruction(OpCodes.Call, getClassLevelMethod);

                instructionsToBypass = 2; // bypasses the [] and the classDefinition index
            }
            else if (instruction.Calls(classesHistoryMethod))
            {
                yield return new CodeInstruction(OpCodes.Call, getClassLevelMethod);

                instructionsToBypass = 1; // bypasses the count
            }
            else
            {
                yield return instruction;
            }
        }
    }

    private static void PatchClassLevel()
    {
        var methods = new[]
        {
            // these use ClassesAndLevels[classDefinition]
            typeof(CharacterStageClassSelectionPanel).GetMethod("FillClassFeatures", PrivateBinding),
            typeof(CharacterStageClassSelectionPanel).GetMethod("RefreshCharacter", PrivateBinding),
            // these use ClassesHistory.Count
            typeof(CharacterStageSpellSelectionPanel).GetMethod("Refresh", PrivateBinding),
            typeof(CharacterBuildingManager).GetMethod("AutoAcquireSpells")
        };

        var harmony = new Harmony("SolastaCommunityExpansion");
        var transpiler = typeof(MulticlassPatchingContext).GetMethod("ClassLevelTranspiler");

        try
        {
            foreach (var method in methods)
            {
                harmony.Patch(method, transpiler: new HarmonyMethod(transpiler));
            }
        }
        catch
        {
            Main.Error("cannot fully patch Multiclass selected class levels");
        }
    }

    //
    // Equipment patching support
    //

    // ReSharper disable once UnusedMember.Global
    public static bool ShouldEquipmentBeAssigned([NotNull] CharacterHeroBuildingData heroBuildingData)
    {
        var hero = heroBuildingData.HeroCharacter;
        var isLevelingUp = LevelUpContext.IsLevelingUp(hero);

        return !isLevelingUp;
    }

    private static void PatchEquipmentAssignment()
    {
        var methods = new[]
        {
            typeof(CharacterBuildingManager).GetMethod("BuildWieldedConfigurations"),
            typeof(CharacterBuildingManager).GetMethod("ClearWieldedConfigurations"),
            typeof(CharacterBuildingManager).GetMethod("GrantBaseEquipment"),
            typeof(CharacterBuildingManager).GetMethod("RemoveBaseEquipment"),
            typeof(CharacterBuildingManager).GetMethod("UnassignEquipment")
        };

        var harmony = new Harmony("SolastaCommunityExpansion");
        var prefix = typeof(MulticlassPatchingContext).GetMethod("ShouldEquipmentBeAssigned");

        try
        {
            foreach (var method in methods)
            {
                harmony.Patch(method, new HarmonyMethod(prefix));
            }
        }
        catch
        {
            Main.Error("cannot fully patch Multiclass equipment");
        }
    }

    private static void PatchFeatureUnlocks()
    {
        var patches = new List<(MethodInfo, HeroContext)>
        {
            // CharacterStageClassSelectionPanel
            (
                typeof(CharacterStageClassSelectionPanel).GetMethod("EnumerateActiveFeatures", PrivateBinding) ??
                NullMethod,
                HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageClassSelectionPanel).GetMethod("FillClassFeatures", PrivateBinding) ?? NullMethod,
                HeroContext.StagePanel
            ),

            // CharacterStageDeitySelectionPanel
            (
                typeof(CharacterStageDeitySelectionPanel).GetMethod("OnHigherLevelCb") ?? NullMethod,
                HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageDeitySelectionPanel).GetMethod("EnumerateActiveFeatures", PrivateBinding) ??
                NullMethod,
                HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageDeitySelectionPanel).GetMethod("FillSubclassFeatures", PrivateBinding) ??
                NullMethod,
                HeroContext.StagePanel
            ),
            (typeof(CharacterStageDeitySelectionPanel).GetMethod("EnterStage") ?? NullMethod, HeroContext.StagePanel),

            // CharacterStageLevelGainsPanel
            (
                typeof(CharacterStageLevelGainsPanel).GetMethod("OnHigherLevelClassCb") ?? NullMethod,
                HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageLevelGainsPanel).GetMethod("EnumerateActiveClassFeatures", PrivateBinding) ??
                NullMethod,
                HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageLevelGainsPanel).GetMethod("FillUnlockedClassFeatures", PrivateBinding) ??
                NullMethod,
                HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageLevelGainsPanel).GetMethod("Refresh", PrivateBinding) ?? NullMethod,
                HeroContext.StagePanel
            ),

            // CharacterStageSubclassSelectionPanel
            (
                typeof(CharacterStageSubclassSelectionPanel).GetMethod("OnHigherLevelCb") ?? NullMethod,
                HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageSubclassSelectionPanel).GetMethod("EnumerateActiveFeatures", PrivateBinding) ??
                NullMethod,
                HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageSubclassSelectionPanel).GetMethod("FillSubclassFeatures", PrivateBinding) ??
                NullMethod,
                HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageSubclassSelectionPanel).GetMethod("Refresh", PrivateBinding) ?? NullMethod,
                HeroContext.StagePanel
            ),

            // CharacterBuildingManager
            (
                typeof(CharacterBuildingManager).GetMethod("FinalizeCharacter") ?? NullMethod,
                HeroContext.BuildingManager
            ),

            // ArchetypesPreviewModal
            // (typeof(ArchetypesPreviewModal).GetMethod("Refresh", PrivateBinding), HeroContext.BuildingManager),

            // CharacterInformationPanel
            (
                typeof(CharacterInformationPanel).GetMethod("TryFindChoiceFeature", PrivateBinding) ?? NullMethod,
                HeroContext.InformationPanel
            ),

            // RulesetCharacterHero
            (
                typeof(RulesetCharacterHero).GetMethod("FindClassHoldingFeature") ?? NullMethod,
                HeroContext.CharacterHero
            ),
            (
                typeof(RulesetCharacterHero).GetMethod("LookForFeatureOrigin", PrivateBinding) ?? NullMethod,
                HeroContext.CharacterHero
            )
        };

        var harmony = new Harmony("SolastaCommunityExpansion");
        var transpiler = typeof(MulticlassPatchingContext).GetMethod("FeatureUnlocksTranspiler");

        try
        {
            foreach (var patch in patches)
            {
                FeatureUnlocksContext = patch;

                harmony.Patch(patch.Item1, transpiler: new HarmonyMethod(transpiler));
            }
        }
        catch
        {
            Main.Error("cannot fully patch Multiclass feature unlocks");
        }
    }

    [ItemNotNull]
    private static IEnumerable<CodeInstruction> YieldHero()
    {
        switch (FeatureUnlocksContext.Item2)
        {
            case HeroContext.StagePanel:
                var classType = FeatureUnlocksContext.Item1.DeclaringType;

                if (classType != null)
                {
                    var currentHeroField =
                        classType.GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                }

                break;

            case HeroContext.BuildingManager:
                yield return new CodeInstruction(OpCodes.Ldarg_1);

                break;

            case HeroContext.CharacterHero:
                yield return new CodeInstruction(OpCodes.Ldarg_0);

                break;

            case HeroContext.InformationPanel:
                var inspectedCharacterMethod =
                    typeof(CharacterInformationPanel).GetMethod("get_InspectedCharacter");
                var rulesetCharacterHeroMethod = typeof(GuiCharacter).GetMethod("get_RulesetCharacterHero");

                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, inspectedCharacterMethod);
                yield return new CodeInstruction(OpCodes.Call, rulesetCharacterHeroMethod);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<CodeInstruction> FeatureUnlocksTranspiler(
        [NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
        var classFilteredFeatureUnlocksMethod =
            typeof(MulticlassPatchingContext).GetMethod("ClassFilteredFeatureUnlocks");

        var subclassFeatureUnlocksMethod = typeof(CharacterSubclassDefinition).GetMethod("get_FeatureUnlocks");
        var subclassFilteredFeatureUnlocksMethod =
            typeof(MulticlassPatchingContext).GetMethod("SubclassFilteredFeatureUnlocks");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(classFeatureUnlocksMethod))
            {
                foreach (var inst in YieldHero())
                {
                    yield return inst;
                }

                yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
            }
            else if (instruction.Calls(subclassFeatureUnlocksMethod))
            {
                foreach (var inst in YieldHero())
                {
                    yield return inst;
                }

                yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    // support class filtered feature unlocks
    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<FeatureUnlockByLevel> ClassFilteredFeatureUnlocks(
        CharacterClassDefinition characterClassDefinition, [NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        var firstClass = rulesetCharacterHero.ClassesHistory[0];
        var selectedClass = LevelUpContext.GetSelectedClass(rulesetCharacterHero) ?? characterClassDefinition;
        var selectedSubClass = LevelUpContext.GetSelectedSubclass(rulesetCharacterHero);
        var filteredFeatureUnlockByLevels = selectedClass.FeatureUnlocks.ToList();

        //
        // supports a better MC UI offering
        //
        if (LevelUpContext.IsLevelingUp(rulesetCharacterHero)
            && LevelUpContext.IsClassSelectionStage(rulesetCharacterHero)
            && selectedSubClass != null)
        {
            filteredFeatureUnlockByLevels.AddRange(SubclassFilteredFeatureUnlocks(selectedSubClass,
                rulesetCharacterHero));
        }

        if (!LevelUpContext.IsMulticlass(rulesetCharacterHero) || firstClass == selectedClass)
        {
            return characterClassDefinition.FeatureUnlocks;
        }

        // remove any extra attacks except on classes that get them at 11 (fighter and swiftblade)
        var attacksNumber = rulesetCharacterHero.GetAttribute(AttributeDefinitions.AttacksNumber).CurrentValue;

        if (attacksNumber > 1 && LevelUpContext.GetSelectedClassLevel(rulesetCharacterHero) < 11)
        {
            filteredFeatureUnlockByLevels.RemoveAll(x =>
                x.FeatureDefinition is FeatureDefinitionAttributeModifier
                {
                    ModifiedAttribute: AttributeDefinitions.AttacksNumber
                });
        }

        foreach (var featureNameToReplace in from featureNameToReplace in FeaturesToReplace
                 let count = filteredFeatureUnlockByLevels.RemoveAll(
                     x => x.FeatureDefinition == featureNameToReplace.Key)
                 where count > 0
                 select featureNameToReplace)
        {
            filteredFeatureUnlockByLevels.Add(new FeatureUnlockByLevel(featureNameToReplace.Value, 1));
        }

        // exclude features per mc rules
        FeaturesToExclude.TryGetValue(selectedClass, out var featureNamesToExclude);

        if (featureNamesToExclude != null)
        {
            filteredFeatureUnlockByLevels.RemoveAll(x => featureNamesToExclude.Contains(x.FeatureDefinition));
        }

        // sort back results
        filteredFeatureUnlockByLevels.Sort((a, b) =>
        {
            var result = a.Level.CompareTo(b.Level);

            if (result == 0)
            {
                result = String.Compare(a.FeatureDefinition.FormatTitle(), b.FeatureDefinition.FormatTitle(),
                    StringComparison.CurrentCultureIgnoreCase);
            }

            return result;
        });

        return filteredFeatureUnlockByLevels;
    }

    // support subclass filtered feature unlocks
    // ReSharper disable once MemberCanBePrivate.Global
    public static IEnumerable<FeatureUnlockByLevel> SubclassFilteredFeatureUnlocks(
        [NotNull] CharacterSubclassDefinition characterSublassDefinition, RulesetCharacterHero rulesetCharacterHero)
    {
        if (!LevelUpContext.IsMulticlass(rulesetCharacterHero))
        {
            return characterSublassDefinition.FeatureUnlocks;
        }

        var filteredFeatureUnlockByLevels = characterSublassDefinition.FeatureUnlocks.ToList();

        // remove any extra attacks except on classes that get them at 11 (fighter and swiftBlade)
        var attacksNumber = rulesetCharacterHero.GetAttribute(AttributeDefinitions.AttacksNumber).CurrentValue;

        if (attacksNumber > 1 && LevelUpContext.GetSelectedClassLevel(rulesetCharacterHero) < 11)
        {
            filteredFeatureUnlockByLevels.RemoveAll(x =>
                x.FeatureDefinition is FeatureDefinitionAttributeModifier
                {
                    ModifiedAttribute: AttributeDefinitions.AttacksNumber
                });
        }

        return filteredFeatureUnlockByLevels;
    }

    internal static int SpellCastingLevel(RulesetSpellRepertoire repertoire, RulesetEffectSpell rulesetEffect)
    {
        return SpellCastingLevel(repertoire, rulesetEffect.Caster, rulesetEffect.SpellDefinition);
    }

    internal static int SpellCastingLevel(RulesetSpellRepertoire repertoire,
        CharacterActionCastSpell action)
    {
        return SpellCastingLevel(repertoire, action.ActingCharacter.RulesetActor, action.ActiveSpell.SpellDefinition);
    }

    private static int SpellCastingLevel(RulesetSpellRepertoire repertoire, RulesetActor caster, SpellDefinition spell)
    {
        if (caster is RulesetCharacterHero hero && spell.SpellLevel == 0)
        {
            return hero.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
        }

        return repertoire.SpellCastingLevel;
    }

    //
    // FeatureUnlocks patching support
    //

    private enum HeroContext
    {
        BuildingManager,
        CharacterHero,
        StagePanel,
        InformationPanel
    }
}
