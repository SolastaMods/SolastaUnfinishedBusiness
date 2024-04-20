using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Models;

internal static class MulticlassContext
{
    internal const int DefaultClasses = 3;
    internal const int MaxClasses = 6;

    private const string ArmorTrainingDescription = "Feature/&ArmorTrainingShortDescription";

    private const string SkillGainChoicesDescription = "Feature/&SkillGainChoicesPluralDescription";

    private const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

    private static readonly FeatureDefinitionProficiency ProficiencyBarbarianArmorMulticlass =
        FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBarbarianArmorMulticlass")
            .SetGuiPresentation("Feature/&BarbarianArmorProficiencyTitle", ArmorTrainingDescription)
            .SetProficiencies(ProficiencyType.Armor,
                EquipmentDefinitions.ShieldCategory)
            .AddToDB();

    private static readonly FeatureDefinitionProficiency ProficiencyFighterArmorMulticlass =
        FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFighterArmorMulticlass")
            .SetGuiPresentation("Feature/&FighterArmorProficiencyTitle", ArmorTrainingDescription)
            .SetProficiencies(ProficiencyType.Armor,
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory)
            .AddToDB();

    private static readonly FeatureDefinitionProficiency ProficiencyPaladinArmorMulticlass =
        FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyPaladinArmorMulticlass")
            .SetGuiPresentation("Feature/&PaladinArmorProficiencyTitle", ArmorTrainingDescription)
            .SetProficiencies(ProficiencyType.Armor,
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory)
            .AddToDB();

    private static readonly FeatureDefinitionPointPool PointPoolBardSkillPointsMulticlass =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolBardSkillPointsMulticlass")
            .SetGuiPresentation("Feature/&BardSkillPointsTitle", SkillGainChoicesDescription)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.Acrobatics,
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Arcana,
                SkillDefinitions.Athletics,
                SkillDefinitions.Deception,
                SkillDefinitions.History,
                SkillDefinitions.Insight,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Investigation,
                SkillDefinitions.Medecine,
                SkillDefinitions.Nature,
                SkillDefinitions.Perception,
                SkillDefinitions.Performance,
                SkillDefinitions.Persuasion,
                SkillDefinitions.Religion,
                SkillDefinitions.SleightOfHand,
                SkillDefinitions.Stealth,
                SkillDefinitions.Survival)
            .AddToDB();

    private static readonly FeatureDefinitionPointPool PointPoolRangerSkillPointsMulticlass =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolRangerSkillPointsMulticlass")
            .SetGuiPresentation("Feature/&RangerSkillsTitle", SkillGainChoicesDescription)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Athletics,
                SkillDefinitions.Insight,
                SkillDefinitions.Investigation,
                SkillDefinitions.Nature,
                SkillDefinitions.Perception,
                SkillDefinitions.Survival,
                SkillDefinitions.Stealth)
            .AddToDB();

    private static readonly FeatureDefinitionPointPool PointPoolRogueSkillPointsMulticlass =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolRogueSkillPointsMulticlass")
            .SetGuiPresentation("Feature/&RogueSkillPointsTitle", SkillGainChoicesDescription)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.Acrobatics,
                SkillDefinitions.Athletics,
                SkillDefinitions.Deception,
                SkillDefinitions.Insight,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Investigation,
                SkillDefinitions.Perception,
                SkillDefinitions.Performance,
                SkillDefinitions.Persuasion,
                SkillDefinitions.SleightOfHand,
                SkillDefinitions.Stealth)
            .AddToDB();

    private static readonly MethodInfo NullMethod = null;

    // these features will be replaced to comply to SRD multiclass rules
    private static readonly Dictionary<FeatureDefinition, FeatureDefinition> FeaturesToReplace = new()
    {
        { ProficiencyBarbarianArmor, ProficiencyBarbarianArmorMulticlass },
        { ProficiencyFighterArmor, ProficiencyFighterArmorMulticlass },
        { ProficiencyPaladinArmor, ProficiencyPaladinArmorMulticlass },
        { PointPoolBardSkillPoints, PointPoolBardSkillPointsMulticlass },
        { PointPoolRangerSkillPoints, PointPoolRangerSkillPointsMulticlass },
        { PointPoolRogueSkillPoints, PointPoolRogueSkillPointsMulticlass }
    };

    // these features will be removed to comply with SRD multiclass rules
    private static readonly Dictionary<CharacterClassDefinition, List<FeatureDefinition>> FeaturesToExclude = new()
    {
        { Barbarian, [PointPoolBarbarianrSkillPoints, ProficiencyBarbarianSavingThrow] },
        { Bard, [ProficiencyBardWeapon, ProficiencyBardSavingThrow] },
        { Cleric, [ProficiencyClericWeapon, PointPoolClericSkillPoints, ProficiencyClericSavingThrow] },
        { Druid, [PointPoolDruidSkillPoints, ProficiencyDruidSavingThrow] },
        { Fighter, [PointPoolFighterSkillPoints, ProficiencyFighterSavingThrow] },
        { Monk, [PointPoolMonkSkillPoints, ProficiencyMonkSavingThrow] },
        { Paladin, [PointPoolPaladinSkillPoints, ProficiencyPaladinSavingThrow] },
        { Ranger, [ProficiencyRangerSavingThrow] },
        { Rogue, [ProficiencyRogueWeapon, ProficiencyRogueSavingThrow] },
        { Sorcerer, [ProficiencySorcererWeapon, PointPoolSorcererSkillPoints, ProficiencySorcererSavingThrow] },
        { Warlock, [PointPoolWarlockSkillPoints, ProficiencyWarlockSavingThrow] },
        { Wizard, [ProficiencyWizardWeapon, PointPoolWizardSkillPoints, ProficiencyWizardSavingThrow] }
    };

    private static (MethodInfo, HeroContext) FeatureUnlocksContext { get; set; }

    internal static void LateLoad()
    {
        FixExtraAttacksScenarios();
        FixDragonbornBreathPowers();
        AddNonOfficialBlueprintsToFeaturesCollections();
        PatchClassLevel();
        PatchEquipmentAssignment();
        PatchFeatureUnlocks();
    }

    private static void FixExtraAttacksScenarios()
    {
        // make all extra attacks use Force If Better
        foreach (var featureDefinitionAttributeModifier in DatabaseRepository
                     .GetDatabase<FeatureDefinitionAttributeModifier>()
                     .Where(x => x.modifiedAttribute == AttributeDefinitions.AttacksNumber &&
                                 x.modifierOperation == AttributeModifierOperation.Additive))
        {
            featureDefinitionAttributeModifier.modifierValue = 2;
            featureDefinitionAttributeModifier.modifierOperation = AttributeModifierOperation.ForceIfBetter;
        }

        // fix use cases when certain classes / subs get a 3rd attack
        var attributeModifierExtraAttackForce3 = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierFighterExtraAttack, "AttributeModifierExtraAttackForce3")
            .SetGuiPresentationNoContent(true)
            .SetModifier(AttributeModifierOperation.ForceIfBetter, AttributeDefinitions.AttacksNumber, 3)
            .AddToDB();

        Fighter.FeatureUnlocks.Add(new FeatureUnlockByLevel(attributeModifierExtraAttackForce3, 11));
        RangerSwiftBlade.FeatureUnlocks.Add(new FeatureUnlockByLevel(attributeModifierExtraAttackForce3, 11));
        RangerMarksman.FeatureUnlocks.Add(new FeatureUnlockByLevel(attributeModifierExtraAttackForce3, 15));

        // fix Fighter use case at level 20
        var attributeModifierExtraAttackForce4 = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierFighterExtraAttack, "AttributeModifierExtraAttackForce4")
            .SetGuiPresentationNoContent(true)
            .SetModifier(AttributeModifierOperation.ForceIfBetter, AttributeDefinitions.AttacksNumber, 4)
            .AddToDB();

        Fighter.FeatureUnlocks.Add(new FeatureUnlockByLevel(attributeModifierExtraAttackForce4, 20));
    }

    private static void FixDragonbornBreathPowers()
    {
        var dragonbornBreathPowers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
            .Where(x => x.name.StartsWith("PowerDragonbornBreath"));

        foreach (var power in dragonbornBreathPowers)
        {
            power.EffectDescription.effectForms[0].levelType = LevelSourceType.CharacterLevel;
        }
    }

    private static void AddNonOfficialBlueprintsToFeaturesCollections()
    {
        const string INVENTOR_NAME = InventorClass.ClassName;

        if (DatabaseHelper.TryGetDefinition<CharacterClassDefinition>(INVENTOR_NAME, out var inventorClass))
        {
            FeaturesToExclude.Add(inventorClass,
            [
                DatabaseHelper.GetDefinition<FeatureDefinitionPointPool>("PointPoolInventorSkills"),
                DatabaseHelper.GetDefinition<FeatureDefinitionProficiency>("ProficiencyInventorSavingThrow")
            ]);
        }
    }

    //
    // ClassLevel patching support
    //

    private static IEnumerable<CodeInstruction> ClassesAndLevelsTranspiler(
        [NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var classesAndLevelsMethod = typeof(RulesetCharacterHero).GetMethod("get_ClassesAndLevels");
        var getClassLevelMethod = new Func<RulesetCharacterHero, int>(LevelUpContext.GetSelectedClassLevel).Method;

        return instructions.ReplaceCall(classesAndLevelsMethod,
            -1,
            2,
            "MulticlassContext.ClassesAndLevels",
            new CodeInstruction(OpCodes.Call, getClassLevelMethod));
    }

    private static IEnumerable<CodeInstruction> ClassesHistoryTranspiler(
        [NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var classesHistoryMethod = typeof(RulesetCharacterHero).GetMethod("get_ClassesHistory");
        var getClassLevelMethod = new Func<RulesetCharacterHero, int>(LevelUpContext.GetSelectedClassLevel).Method;

        return instructions.ReplaceCall(classesHistoryMethod,
            -1,
            1,
            "MulticlassContext.ClassesHistory",
            new CodeInstruction(OpCodes.Call, getClassLevelMethod));
    }

    private static void PatchClassLevel()
    {
        var classesAndLevelsMethods = new[]
        {
            typeof(CharacterStageClassSelectionPanel).GetMethod("FillClassFeatures", PrivateBinding),
            typeof(CharacterStageClassSelectionPanel).GetMethod("RefreshCharacter", PrivateBinding)
        };

        var classesHistoryMethods = new[]
        {
            typeof(CharacterStageSpellSelectionPanel).GetMethod("Refresh", PrivateBinding),
            typeof(CharacterBuildingManager).GetMethod("AutoAcquireSpells")
        };

        var harmony = new Harmony("SolastaUnfinishedBusiness");
        var classesAndLevelsTranspiler =
            new Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>(
                ClassesAndLevelsTranspiler).Method;
        var classesHistoryTranspiler =
            new Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>(
                ClassesHistoryTranspiler).Method;

        foreach (var method in classesAndLevelsMethods)
        {
            try
            {
                harmony.Patch(method, transpiler: new HarmonyMethod(classesAndLevelsTranspiler));
            }
            catch
            {
                Main.Error(
                    $"Failed to apply ClassesAndLevelsTranspiler patch to {method.DeclaringType}.{method.Name}");
            }
        }

        foreach (var method in classesHistoryMethods)
        {
            try
            {
                harmony.Patch(method, transpiler: new HarmonyMethod(classesHistoryTranspiler));
            }
            catch
            {
                Main.Error($"Failed to apply ClassesHistoryTranspiler patch to {method.DeclaringType}.{method.Name}");
            }
        }
    }

    //
    // Equipment patching support
    //

    private static bool ShouldEquipmentBeAssigned([NotNull] CharacterHeroBuildingData heroBuildingData)
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

        var harmony = new Harmony("SolastaUnfinishedBusiness");
        var prefix = new Func<CharacterHeroBuildingData, bool>(ShouldEquipmentBeAssigned).Method;

        foreach (var method in methods)
        {
            try
            {
                harmony.Patch(method, new HarmonyMethod(prefix));
            }
            catch
            {
                Main.Error($"Failed to apply ShouldEquipmentBeAssigned patch to {method.DeclaringType}.{method.Name}");
            }
        }
    }

    private static void PatchFeatureUnlocks()
    {
        var patches = new[]
        {
            // CharacterStageClassSelectionPanel
            (
                typeof(CharacterStageClassSelectionPanel).GetMethod("EnumerateActiveFeatures", PrivateBinding) ??
                NullMethod, HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageClassSelectionPanel).GetMethod("FillClassFeatures", PrivateBinding) ??
                NullMethod, HeroContext.StagePanel
            ),
            // CharacterStageLevelGainsPanel
            (
                typeof(CharacterStageLevelGainsPanel).GetMethod("OnHigherLevelClassCb") ??
                NullMethod, HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageLevelGainsPanel).GetMethod("EnumerateActiveClassFeatures", PrivateBinding) ??
                NullMethod, HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageLevelGainsPanel).GetMethod("FillUnlockedClassFeatures", PrivateBinding) ??
                NullMethod, HeroContext.StagePanel
            ),
            (
                typeof(CharacterStageLevelGainsPanel).GetMethod("Refresh", PrivateBinding) ??
                NullMethod, HeroContext.StagePanel
            ),

            // CharacterBuildingManager
            (
                typeof(CharacterBuildingManager).GetMethod("FinalizeCharacter") ??
                NullMethod, HeroContext.BuildingManager
            ),

            // CharacterInformationPanel
            (
                typeof(CharacterInformationPanel).GetMethod("TryFindChoiceFeature", PrivateBinding) ??
                NullMethod, HeroContext.InformationPanel)
        };

        var harmony = new Harmony("SolastaUnfinishedBusiness");
        var transpiler = new Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>
            (FeatureUnlocksTranspiler).Method;

        foreach (var patch in patches)
        {
            FeatureUnlocksContext = patch;
            var method = FeatureUnlocksContext.Item1;

            try
            {
                harmony.Patch(method, transpiler: new HarmonyMethod(transpiler));
            }
            catch
            {
                Main.Error($"Failed to apply FeatureUnlocksTranspiler patch to {method.DeclaringType}.{method.Name}");
            }
        }
    }

    private static IEnumerable<CodeInstruction> FeatureUnlocksTranspiler(
        [NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
        var classFilteredFeatureUnlocksMethod =
            new Func<CharacterClassDefinition, RulesetCharacterHero, IEnumerable<FeatureUnlockByLevel>>(
                ClassFilteredFeatureUnlocks).Method;

        switch (FeatureUnlocksContext.Item2)
        {
            case HeroContext.StagePanel:
                var classType = FeatureUnlocksContext.Item1.DeclaringType;
                var currentHeroField =
                    classType!.GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

                return instructions.ReplaceCalls(classFeatureUnlocksMethod,
                    "MulticlassContext.FeatureUnlocksTranspiler.StagePanel",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, currentHeroField),
                    new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod));

            case HeroContext.BuildingManager:
                return instructions.ReplaceCalls(classFeatureUnlocksMethod,
                    "MulticlassContext.FeatureUnlocksTranspiler.BuildingManager",
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod));

            case HeroContext.InformationPanel:
                var inspectedCharacterMethod =
                    typeof(CharacterInformationPanel).GetMethod("get_InspectedCharacter");
                var rulesetCharacterHeroMethod = typeof(GuiCharacter).GetMethod("get_RulesetCharacterHero");

                return instructions.ReplaceCalls(classFeatureUnlocksMethod,
                    "MulticlassContext.FeatureUnlocksTranspiler.InformationPanel",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, inspectedCharacterMethod),
                    new CodeInstruction(OpCodes.Call, rulesetCharacterHeroMethod),
                    new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod));

            default:
                throw new ArgumentOutOfRangeException(nameof(instructions));
        }
    }

    // support class filtered feature unlocks
    // ReSharper disable once SuggestBaseTypeForParameter
    private static List<FeatureUnlockByLevel> ClassFilteredFeatureUnlocks(
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
            && selectedSubClass)
        {
            filteredFeatureUnlockByLevels.AddRange(selectedSubClass.FeatureUnlocks);
        }

        // don't mess up with very first class taken
        if (!LevelUpContext.IsMulticlass(rulesetCharacterHero) || firstClass == selectedClass)
        {
            return characterClassDefinition.FeatureUnlocks;
        }

        // replace features per mc rules
        foreach (var featureNameToReplace in FeaturesToReplace
                     .Where(featureNameToReplace => filteredFeatureUnlockByLevels
                         .RemoveAll(x => x.FeatureDefinition == featureNameToReplace.Key) > 0))
        {
            filteredFeatureUnlockByLevels.Add(new FeatureUnlockByLevel(featureNameToReplace.Value, 1));
        }

        // exclude features per mc rules
        if (FeaturesToExclude.TryGetValue(selectedClass, out var featureNamesToExclude))
        {
            filteredFeatureUnlockByLevels.RemoveAll(x => featureNamesToExclude.Contains(x.FeatureDefinition));
        }

        // sort back results
        filteredFeatureUnlockByLevels.Sort(Sorting.CompareFeatureUnlock);

        return filteredFeatureUnlockByLevels;
    }

    internal static int SpellCastingLevel(RulesetSpellRepertoire repertoire, RulesetEffectSpell rulesetEffect)
    {
        return SpellCastingLevel(repertoire, rulesetEffect.Caster, rulesetEffect.SpellDefinition);
    }

    internal static int SpellCastingLevel(RulesetSpellRepertoire repertoire, CharacterActionCastSpell action)
    {
        return SpellCastingLevel(repertoire, action.ActingCharacter.RulesetActor, action.ActiveSpell.SpellDefinition);
    }

    private static int SpellCastingLevel(RulesetSpellRepertoire repertoire, ISerializable caster, SpellDefinition spell)
    {
        if (caster is RulesetCharacterHero hero && spell.SpellLevel == 0)
        {
            return hero.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
        }

        return repertoire.SpellCastingLevel;
    }

    //
    // FeatureUnlocks patching support
    //

    private enum HeroContext
    {
        BuildingManager,
        StagePanel,
        InformationPanel
    }
}
