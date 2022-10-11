using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class SrdAndHouseRulesContext
{
    internal const int DefaultVisionRange = 16;
    internal const int MaxVisionRange = 120;

    internal static void Load()
    {
        AllowTargetingSelectionWhenCastingChainLightningSpell();
        ApplyConditionBlindedShouldNotAllowOpportunityAttack();
        ApplyAcNonStackingRules();
        ApplySrdWeightToFoodRations();
        ConjurationsContext.BuildConjureElementalInvisibleStalker();
        SenseNormalVision.senseRange = Main.Settings.IncreaseSenseNormalVision;
    }

    internal static void LateLoad()
    {
        FixDivineSmiteRestrictions();
        FixDivineSmiteDiceNumberWhenUsingHighLevelSlots();
        FixMountaineerBonusShoveRestrictions();
        FixRecklessAttackForReachWeapons();
        MinorFixes();
        AddBleedingToRestoration();
        SwitchFilterOnHideousLaughter();
        SwitchRecurringEffectOnEntangle();
        UseCubeOnSleetStorm();
        UseHeightOneCylinderEffect();
        SwitchUniversalSylvanArmorAndLightbringer();
        SwitchDruidAllowMetalArmor();
        SwitchMagicStaffFoci();
        ConjurationsContext.SwitchEnableUpcastConjureElementalAndFey();
        ConjurationsContext.SwitchFullyControlConjurations();
    }


    internal static void SwitchUniversalSylvanArmorAndLightbringer()
    {
        GreenmageArmor.RequiredAttunementClasses.Clear();
        WizardClothes_Alternate.RequiredAttunementClasses.Clear();

        if (Main.Settings.AllowAnyClassToWearSylvanArmor)
        {
            return;
        }

        var allowedClasses = new[] { Sorcerer, Warlock, Wizard };

        GreenmageArmor.RequiredAttunementClasses.AddRange(allowedClasses);
        WizardClothes_Alternate.RequiredAttunementClasses.AddRange(allowedClasses);
    }

    internal static void SwitchDruidAllowMetalArmor()
    {
        var active = Main.Settings.AllowDruidToWearMetalArmor;

        if (active)
        {
            FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Clear();
        }
        else
        {
            if (!FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Contains(
                    TagsDefinitions.ItemTagMetal))
            {
                FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Add(
                    TagsDefinitions.ItemTagMetal);
            }
        }
    }

    internal static void SwitchMagicStaffFoci()
    {
        if (!Main.Settings.MakeAllMagicStaveArcaneFoci)
        {
            return;
        }

        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>()
                     .Where(x => x.IsWeapon) // WeaponDescription could be null
                     .Where(x => x.WeaponDescription.WeaponType == EquipmentDefinitions.WeaponTypeQuarterstaff)
                     .Where(x => x.Magical && !x.Name.Contains("OfHealing")))
        {
            item.IsFocusItem = true;
            item.FocusItemDescription.focusType = EquipmentDefinitions.FocusType.Arcane;
        }
    }

    /**
     * Makes Divine Smite trigger only from melee attacks.
     * This wasn't relevant until we changed how SpendSpellSlot trigger works.
     */
    private static void FixDivineSmiteRestrictions()
    {
        FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite.attackModeOnly = true;
        FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite.requiredProperty =
            RestrictedContextRequiredProperty.MeleeWeapon;
    }

    /**
     * Makes Divine Smite use correct number of dice when spending slot level 5+.
     * Base game has config only up to level 4 slots, which leads to it using 1 die if level 5+ slot is spent.
     */
    private static void FixDivineSmiteDiceNumberWhenUsingHighLevelSlots()
    {
        FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite.diceByRankTable =
            DiceByRankBuilder.BuildDiceByRankTable();
    }

    /**
     * Makes Mountaineer's `Shield Push` bonus shove work only with shield equipped.
     * This wasn't relevant until we removed forced shield check in the `GameLocationCharacter.GetActionStatus`.
     */
    private static void FixMountaineerBonusShoveRestrictions()
    {
        ActionAffinityMountaineerShieldCharge
            .SetCustomSubFeatures(new ValidatorDefinitionApplication(ValidatorsCharacter.HasShield));
    }

    /**
     * Makes `Reckless` context check if main hand weapon is melee, instead of if character is next to target.
     * Required for it to work on reach weapons.
     */
    private static void FixRecklessAttackForReachWeapons()
    {
        FeatureDefinitionCombatAffinitys.CombatAffinityReckless
            .situationalContext = (SituationalContext)ExtraSituationalContext.MainWeaponIsMelee;
    }

    internal static void ApplyConditionBlindedShouldNotAllowOpportunityAttack()
    {
        // Use the shocked condition affinity which has the desired effect
        if (Main.Settings.BlindedConditionDontAllowAttackOfOpportunity)
        {
            if (!ConditionDefinitions.ConditionBlinded.Features.Contains(ActionAffinityConditionShocked))
            {
                ConditionDefinitions.ConditionBlinded.Features.Add(ActionAffinityConditionShocked);
            }
        }
        else
        {
            if (ConditionDefinitions.ConditionBlinded.Features.Contains(ActionAffinityConditionShocked))
            {
                ConditionDefinitions.ConditionBlinded.Features.Remove(ActionAffinityConditionShocked);
            }
        }
    }

    /// <summary>
    ///     Allow the user to select targets when using 'Chain Lightning'.
    /// </summary>
    internal static void AllowTargetingSelectionWhenCastingChainLightningSpell()
    {
        var spell = ChainLightning.EffectDescription;

        if (Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell)
        {
            // This is half bug-fix, half houses rules since it's not completely SRD but better than implemented.
            // Spell should arc from target (range 150ft) onto upto 3 extra selectable targets (range 30ft from first).
            // Fix by allowing 4 selectable targets.
            spell.targetType = TargetType.IndividualsUnique;
            spell.targetParameter = 4;
            spell.effectAdvancement.additionalTargetsPerIncrement = 1;
        }
        else
        {
            spell.targetType = TargetType.ArcFromIndividual;
            spell.targetParameter = 3;
            spell.effectAdvancement.additionalTargetsPerIncrement = 0;
        }
    }

    private static void ApplyAcNonStackingRules()
    {
        FeatureDefinitionAttributeModifiers.AttributeModifierBarbarianUnarmoredDefense
            .SetCustomSubFeatures(ExclusiveAcBonus.MarkUnarmoredDefense);
        FeatureDefinitionAttributeModifiers.AttributeModifierMonkUnarmoredDefense
            .SetCustomSubFeatures(ExclusiveAcBonus.MarkUnarmoredDefense);
        FeatureDefinitionAttributeModifiers.AttributeModifierSorcererDraconicResilienceAC
            .SetCustomSubFeatures(ExclusiveAcBonus.MarkLikeArmor);

        //Mostly for wild-shaped AC stacking, since unarmored defenses would not be valid anyway under mage armor
        FeatureDefinitionAttributeModifiers.AttributeModifierMageArmor
            .SetCustomSubFeatures(ExclusiveAcBonus.MarkLikeArmor);
    }

    internal static void ApplySrdWeightToFoodRations()
    {
        var foodSrdWeight = Food_Ration;
        var foodForagedSrdWeight = Food_Ration_Foraged;

        if (Main.Settings.ApplySrdWeightToFoodRations)
        {
            foodSrdWeight.weight = 2.0f;
            foodForagedSrdWeight.weight = 2.0f;
        }
        else
        {
            foodSrdWeight.weight = 3.0f;
            foodForagedSrdWeight.weight = 3.0f;
        }
    }

    internal static void SwitchFilterOnHideousLaughter()
    {
        HideousLaughter.effectDescription.restrictedCreatureFamilies.Clear();

        if (!Main.Settings.RemoveHumanoidFilterOnHideousLaughter)
        {
            HideousLaughter.effectDescription.restrictedCreatureFamilies.Add("Humanoid");
        }
    }

    internal static void SwitchRecurringEffectOnEntangle()
    {
        if (Main.Settings.RemoveRecurringEffectOnEntangle)
        {
            // Remove recurring effect on Entangle (as per SRD, any creature is only affected at cast time)
            Entangle.effectDescription.recurrentEffect = RecurrentEffect.OnActivation;
        }
        else
        {
            Entangle.effectDescription.recurrentEffect =
                RecurrentEffect.OnActivation | RecurrentEffect.OnTurnEnd | RecurrentEffect.OnEnter;
        }
    }

    private static void MinorFixes()
    {
        // Shows Concentration tag in UI
        BladeBarrier.requiresConcentration = true;

        //
        // BUGFIX: spells durations
        //

        // Use our logic to calculate duration for DominatePerson/Beast/Monster
        DominateBeast.EffectDescription.EffectAdvancement.alteredDuration =
            (AdvancementDuration)ExtraAdvancementDuration.DominateBeast;
        DominatePerson.EffectDescription.EffectAdvancement.alteredDuration =
            (AdvancementDuration)ExtraAdvancementDuration.DominatePerson;

        // Stops upcasting assigning non-SRD durations
        ClearAlteredDuration(ProtectionFromEnergy);
        ClearAlteredDuration(ProtectionFromEnergyAcid);
        ClearAlteredDuration(ProtectionFromEnergyCold);
        ClearAlteredDuration(ProtectionFromEnergyFire);
        ClearAlteredDuration(ProtectionFromEnergyLightning);
        ClearAlteredDuration(ProtectionFromEnergyThunder);
        ClearAlteredDuration(ProtectionFromPoison);

        static void ClearAlteredDuration([NotNull] IMagicEffect spell)
        {
            spell.EffectDescription.EffectAdvancement.alteredDuration = AdvancementDuration.None;
        }
    }

    internal static void UseCubeOnSleetStorm()
    {
        var sleetStormEffect = SleetStorm.EffectDescription;

        if (Main.Settings.ChangeSleetStormToCube)
        {
            // Set to Cube side 8, default height
            sleetStormEffect.targetType = TargetType.Cube;
            sleetStormEffect.targetParameter = 8;
            sleetStormEffect.targetParameter2 = 0;
        }
        else
        {
            // Restore to cylinder radius 4, height 3
            sleetStormEffect.targetType = TargetType.Cylinder;
            sleetStormEffect.targetParameter = 4;
            sleetStormEffect.targetParameter2 = 3;
        }
    }

    internal static void UseHeightOneCylinderEffect()
    {
        // always applicable
        ClearTargetParameter2ForTargetTypeCube();

        ///////////////////////////////////////////////////////////
        // Change SpikeGrowth to be height 1 round cylinder/sphere
        var spikeGrowthEffect = SpikeGrowth.EffectDescription;
        spikeGrowthEffect.targetParameter = 4;

        if (Main.Settings.UseHeightOneCylinderEffect)
        {
            // Set to Cylinder radius 4, height 1
            spikeGrowthEffect.targetType = TargetType.Cylinder;
            spikeGrowthEffect.targetParameter2 = 1;
        }
        else
        {
            // Restore default of Sphere radius 4
            spikeGrowthEffect.targetType = TargetType.Sphere;
            spikeGrowthEffect.targetParameter2 = 0;
        }

        // Spells with TargetType.Cube and defaults values of (tp, tp2)
        // Note that tp2 should be 0 for Cube and is ignored in game.
        // BlackTentacles: (4, 2)
        // Entangle: (4, 1)
        // FaerieFire: (4, 2)
        // FlamingSphere: (3, 2) <- a flaming sphere is a cube?
        // Grease: (2, 2)
        // HypnoticPattern: (6, 2)
        // Slow: (8, 2)
        // Thunderwave: (3, 2)


        ///////////////////////////////////////////////////////////
        // Change Black Tentacles, Entangle, Grease to be height 1 square cylinder/cube
        if (Main.Settings.UseHeightOneCylinderEffect)
        {
            // Setting height switches to square cylinder (if originally cube)
            SetHeight(BlackTentacles, 1);
            SetHeight(Entangle, 1);
            SetHeight(Grease, 1);
        }
        else
        {
            // Setting height to 0 restores original behaviour
            SetHeight(BlackTentacles, 0);
            SetHeight(Entangle, 0);
            SetHeight(Grease, 0);
        }

        static void SetHeight([NotNull] IMagicEffect spellDefinition, int height)
        {
            spellDefinition.EffectDescription.targetParameter2 = height;
        }

        static void ClearTargetParameter2ForTargetTypeCube()
        {
            foreach (var sd in DatabaseRepository
                         .GetDatabase<SpellDefinition>()
                         .Where(sd =>
                             sd.EffectDescription.TargetType is TargetType.Cube
                                 or TargetType.CubeWithOffset))
            {
                // TargetParameter2 is not used by TargetType.Cube but has random values assigned.
                // We are going to use it to create a square cylinder with height so set to zero for all spells with TargetType.Cube.
                sd.EffectDescription.targetParameter2 = 0;
            }
        }
    }

    internal static void AddBleedingToRestoration()
    {
        var cf = LesserRestoration.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Condition);

        if (cf != null)
        {
            if (Main.Settings.AddBleedingToLesserRestoration)
            {
                cf.ConditionForm.ConditionsList.TryAdd(ConditionBleeding);
            }
            else
            {
                cf.ConditionForm.ConditionsList.Remove(ConditionBleeding);
            }
        }
        else
        {
            Main.Error("Unable to find form of type Condition in LesserRestoration");
        }

        var cfg = GreaterRestoration.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Condition);

        if (cfg != null)
        {
            // NOTE: using the same setting as for Lesser Restoration for compatibility
            if (Main.Settings.AddBleedingToLesserRestoration)
            {
                cfg.ConditionForm.ConditionsList.TryAdd(ConditionBleeding);
            }
            else
            {
                cfg.ConditionForm.ConditionsList.Remove(ConditionBleeding);
            }
        }
        else
        {
            Main.Error("Unable to find form of type Condition in GreaterRestoration");
        }
    }
}

internal static class ConjurationsContext
{
    private const string InvisibleStalkerSubspellName = "ConjureElementalInvisibleStalker";

    internal static readonly HashSet<MonsterDefinition> ConjuredMonsters = new()
    {
        // Conjure animals (3)
        ConjuredOneBeastTiger_Drake,
        ConjuredTwoBeast_Direwolf,
        ConjuredFourBeast_BadlandsSpider,
        ConjuredEightBeast_Wolf,

        // Conjure minor elemental (4)
        SkarnGhoul, // CR 2
        WindSnake, // CR 2
        Fire_Jester, // CR 1

        // Conjure woodland beings (4) - not implemented

        // Conjure elemental (5)
        Air_Elemental, // CR 5
        Fire_Elemental, // CR 5
        Earth_Elemental, // CR 5

        InvisibleStalker, // CR 6

        // Conjure fey (6)
        FeyGiantApe, // CR 6
        FeyGiant_Eagle, // CR 5
        FeyBear, // CR 4
        Green_Hag, // CR 3
        FeyWolf, // CR 2
        FeyDriad // CR 1
    };

    private static SpellDefinition ConjureElementalInvisibleStalker { get; set; }

    /// <summary>
    ///     Allow conjurations to fully controlled party members instead of AI controlled.
    /// </summary>
    internal static void BuildConjureElementalInvisibleStalker()
    {
        ConjureElementalInvisibleStalker = SpellDefinitionBuilder
            .Create(ConjureElementalFire, InvisibleStalkerSubspellName)
            .SetOrUpdateGuiPresentation("Spell/&ConjureElementalInvisibleStalkerTitle",
                "Spell/&ConjureElementalDescription")
            .AddToDB();

        var summonForm = ConjureElementalInvisibleStalker
            .EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Summon)?.SummonForm;

        if (summonForm != null)
        {
            summonForm.monsterDefinitionName = InvisibleStalker.Name;
        }
    }

    internal static void SwitchFullyControlConjurations()
    {
        foreach (var conjuredMonster in ConjuredMonsters)
        {
            conjuredMonster.fullyControlledWhenAllied = Main.Settings.FullyControlConjurations;
        }
    }

    internal static void SwitchEnableUpcastConjureElementalAndFey()
    {
        if (!Main.Settings.EnableUpcastConjureElementalAndFey)
        {
            ConjureElemental.SubspellsList.Remove(ConjureElementalInvisibleStalker);

            return;
        }

        ConfigureAdvancement(ConjureFey);
        ConfigureAdvancement(ConjureElemental);
        ConjureElemental.SubspellsList.Add(ConjureElementalInvisibleStalker);
        ConfigureAdvancement(ConjureMinorElementals);

        // Set advancement at spell level, not sub-spell
        static void ConfigureAdvancement([NotNull] IMagicEffect spell)
        {
            var advancement = spell.EffectDescription.EffectAdvancement;

            advancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;
            advancement.additionalSpellLevelPerIncrement = 1;
        }
    }
}

internal static class ArmorClassStacking
{
    //replaces call to `RulesetAttributeModifier.BuildAttributeModifier` with custom method that calls base on e and adds extra tags when necessary
    internal static IEnumerable<CodeInstruction> AddCustomTagsToModifierBuilderInCharacter(
        IEnumerable<CodeInstruction> instructions)
    {
        var method =
            new Func<AttributeModifierOperation, float, string, string, RulesetAttributeModifier>(
                RulesetAttributeModifier.BuildAttributeModifier).Method;

        var custom =
            new Func<AttributeModifierOperation, float, string, string, FeatureDefinitionAttributeModifier,
                RulesetAttributeModifier>(CustomBuildAttributeModifier).Method;

        foreach (var code in instructions)
        {
            if (code.Calls(method))
            {
                yield return new CodeInstruction(OpCodes.Ldloc_1);
                yield return new CodeInstruction(OpCodes.Call, custom);
            }
            else
            {
                yield return code;
            }
        }
    }

    internal static IEnumerable<CodeInstruction> AddCustomTagsToModifierBuilderInFeature(
        IEnumerable<CodeInstruction> instructions)
    {
        var method =
            new Func<AttributeModifierOperation, float, string, string, RulesetAttributeModifier>(
                RulesetAttributeModifier.BuildAttributeModifier).Method;

        var custom =
            new Func<AttributeModifierOperation, float, string, string, FeatureDefinitionAttributeModifier,
                RulesetAttributeModifier>(CustomBuildAttributeModifier).Method;

        foreach (var code in instructions)
        {
            if (code.Calls(method))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, custom);
            }
            else
            {
                yield return code;
            }
        }
    }

    private static RulesetAttributeModifier CustomBuildAttributeModifier(
        AttributeModifierOperation operationType,
        float modifierValue,
        string tag,
        string sourceAbility,
        FeatureDefinitionAttributeModifier feature)
    {
        var modifier =
            RulesetAttributeModifier.BuildAttributeModifier(operationType, modifierValue, tag, sourceAbility);

        var exclusive = feature.GetFirstSubFeatureOfType<ExclusiveAcBonus>();
        if (exclusive != null)
        {
            modifier.Tags.TryAdd(exclusive.Tag);
        }

        return modifier;
    }

    // Replaces calls to `RulesetAttributeModifier.SortAttributeModifiersList` with custom method
    // that removes inactive exclusive modifiers, and then calls `RulesetAttributeModifier.SortAttributeModifiersList`
    internal static IEnumerable<CodeInstruction> UnstackAcTranspile(IEnumerable<CodeInstruction> instructions)
    {
        var sort = new Action<List<RulesetAttributeModifier>>(RulesetAttributeModifier.SortAttributeModifiersList)
            .Method;

        var unstack = new Action<List<RulesetAttributeModifier>, RulesetCharacter>(UnstackAc).Method;

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(sort))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, unstack);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    private static void UnstackAc(List<RulesetAttributeModifier> modifiers, RulesetCharacter character)
    {
        //AC formula is `Value + DEX`
        var armor = new List<RulesetAttributeModifier>();
        //AC formula is `Value`
        var natural = new List<RulesetAttributeModifier>();
        //AC Formula is `10 + DEX + Value`
        var unarmored = new List<RulesetAttributeModifier>();

        //Collect all different AC formulas into lists
        foreach (var modifier in modifiers)
        {
            switch (modifier.operation)
            {
                case AttributeModifierOperation.Set
                    when modifier.Tags.Contains(ExclusiveAcBonus.TagLikeArmor):
                {
                    armor.Add(modifier);
                    break;
                }
                case AttributeModifierOperation.Set
                    when modifier.Tags.Contains(ExclusiveAcBonus.TagNaturalArmor):
                {
                    natural.Add(modifier);
                    break;
                }
                case AttributeModifierOperation.AddAbilityScoreBonus
                    when modifier.Tags.Contains(ExclusiveAcBonus.TagUnarmoredDefense):
                {
                    unarmored.Add(modifier);
                    break;
                }
            }
        }

        //setup for getting top formula of each type
        var topFormulas = new List<(float, RulesetAttributeModifier, string)>();
        var dexterity = AttributeDefinitions.ComputeAbilityScoreModifier(
            character.TryGetAttributeValue(AttributeDefinitions.Dexterity));

        void TryAddFormula(List<RulesetAttributeModifier> mods, float baseStat, string tag)
        {
            if (mods.Count <= 0)
            {
                return;
            }

            mods.Sort((left, right) => -left.value.CompareTo(right.value));
            topFormulas.Add((baseStat + mods[0].Value, mods[0], tag));
        }

        //get top formula of each type
        TryAddFormula(armor, dexterity, ExclusiveAcBonus.TagLikeArmor);
        TryAddFormula(natural, 0, ExclusiveAcBonus.TagNaturalArmor);
        TryAddFormula(unarmored, 10 + dexterity, ExclusiveAcBonus.TagUnarmoredDefense);

        //remove all modifiers corresponding to formulas
        modifiers.RemoveAll(m => armor.Contains(m));
        modifiers.RemoveAll(m => natural.Contains(m));
        modifiers.RemoveAll(m => unarmored.Contains(m));

        if (topFormulas.Count > 0)
        {
            //sort modifiers so that biggest is first
            topFormulas.Sort((left, right) => -left.Item1.CompareTo(right.Item1));

            var topFormula = topFormulas[0];

            //return top AC formula back into modifiers
            modifiers.Add(topFormula.Item2);

            //if this is Natural armor, we need to dump dex so it won't apply
            if (topFormula.Item3 == ExclusiveAcBonus.TagNaturalArmor)
            {
                var dexMod = modifiers.Find(m =>
                    m.Operation == AttributeModifierOperation.AddAbilityScoreBonus
                    && m.SourceAbility == AttributeDefinitions.Dexterity);

                if (dexMod != null)
                {
                    dexMod.Value = 0;
                }
            }
        }

        //sort modifiers
        RulesetAttributeModifier.SortAttributeModifiersList(modifiers);
    }

    internal static IEnumerable<CodeInstruction> AddAcTrendsToMonsterAcRefreshTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var sort = new Action<
            List<RulesetAttributeModifier>
        >(RulesetAttributeModifier.SortAttributeModifiersList).Method;

        var unstack = new Action<
            List<RulesetAttributeModifier>,
            RulesetCharacterMonster
        >(ProcessWildShapeAc).Method;

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(sort))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, unstack);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    private static void ProcessWildShapeAc(List<RulesetAttributeModifier> modifiers, RulesetCharacterMonster monster)
    {
        var ac = monster.GetAttribute(AttributeDefinitions.ArmorClass);

        MulticlassWildshapeContext.RefreshWildShapeAcFeatures(monster, ac);
        MulticlassWildshapeContext.UpdateWildShapeAcTrends(modifiers, monster, ac);
        UnstackAc(modifiers, monster); // also sorts modifiers
    }
}

/// <summary>
///     Allow spells that require consumption of a material component (e.g. a gem of value >= 1000gp) use a stack
///     of lesser value components (e.g. 4 x 300gp diamonds).
///     Note that this implementation will only work with identical components - e.g. 'all diamonds', it won't consider
///     combining
///     different types of items with the tag 'gem'.
/// </summary>
internal static class StackedMaterialComponent
{
    internal static void IsComponentMaterialValid(
        RulesetCharacter character,
        SpellDefinition spellDefinition,
        ref string failure,
        ref bool result)
    {
        if (!Main.Settings.AllowStackedMaterialComponent)
        {
            return;
        }

        if (result)
        {
            return;
        }

        // Repeats the last section of the original method but adds 'approximateCostInGold * item.StackCount'
        var items = new List<RulesetItem>();

        character.CharacterInventory.EnumerateAllItems(items);

        if ((from item in items
                let approximateCostInGold = EquipmentDefinitions.GetApproximateCostInGold(item.ItemDefinition.Costs)
                where item.ItemDefinition.ItemTags.Contains(spellDefinition.SpecificMaterialComponentTag) &&
                      approximateCostInGold * item.StackCount >= spellDefinition.SpecificMaterialComponentCostGp
                select item).Any())
        {
            result = true;
            failure = string.Empty;
        }
    }

    //Modify original code to spend enough of a stack to meet component cost
    internal static bool SpendSpellMaterialComponentAsNeeded(RulesetCharacter character, RulesetEffectSpell activeSpell)
    {
        if (!Main.Settings.AllowStackedMaterialComponent)
        {
            return true;
        }

        var spell = activeSpell.SpellDefinition;
        if (spell.MaterialComponentType != MaterialComponentType.Specific
            || !spell.SpecificMaterialComponentConsumed
            || string.IsNullOrEmpty(spell.SpecificMaterialComponentTag)
            || spell.SpecificMaterialComponentCostGp <= 0
            || character.CharacterInventory == null)
        {
            return false;
        }

        var items = new List<RulesetItem>();

        character.CharacterInventory.EnumerateAllItems(items);

        var itemToUse = items
            .Where(item => item.ItemDefinition.ItemTags.Contains(spell.SpecificMaterialComponentTag))
            .Select(item => new
            {
                RulesetItem = item,
                // Note original code is "int cost = rulesetItem2.ItemDefinition.Costs[1];" which doesn't agree with IsComponentMaterialValid which
                // uses GetApproximateCostInGold
                Cost = EquipmentDefinitions.GetApproximateCostInGold(item.ItemDefinition.Costs)
            })
            .Select(item => new
            {
                item.RulesetItem,
                item.Cost,
                StackCountRequired = (int)Math.Ceiling(spell.SpecificMaterialComponentCostGp / (double)item.Cost)
            })
            .Where(item => item.StackCountRequired <= item.RulesetItem.StackCount)
            .Select(item => new
            {
                item.RulesetItem,
                item.Cost,
                item.StackCountRequired,
                TotalCost = item.StackCountRequired * item.Cost
            })
            .Where(item => item.TotalCost >= activeSpell.SpellDefinition.SpecificMaterialComponentCostGp)
            .OrderBy(item => item.TotalCost) // min total cost used
            .ThenBy(item => item.StackCountRequired) // min items used
            .FirstOrDefault();

        if (itemToUse == null)
        {
            Main.Log("Didn't find item.");

            return false;
        }

        Main.Log($"Spending stack={itemToUse.StackCountRequired}, cost={itemToUse.TotalCost}");

        var componentConsumed = character.SpellComponentConsumed;

        if (componentConsumed != null)
        {
            for (var i = 0; i < itemToUse.StackCountRequired; i++)
            {
                componentConsumed(character, spell, itemToUse.RulesetItem);
            }
        }

        var rulesetItem = itemToUse.RulesetItem;

        if (rulesetItem.ItemDefinition.CanBeStacked && rulesetItem.StackCount > 1 &&
            itemToUse.StackCountRequired < rulesetItem.StackCount)
        {
            Main.Log($"Spending stack={itemToUse.StackCountRequired}, cost={itemToUse.TotalCost}");

            rulesetItem.SpendStack(itemToUse.StackCountRequired);
        }
        else
        {
            Main.Log("Destroy item");

            character.CharacterInventory.DestroyItem(rulesetItem);
        }

        return false;
    }
}

internal static class UpcastConjureElementalAndFey
{
    private static List<SpellDefinition> _filteredSubspells;

    /**
     * Patch implementation
     * Replaces subspell activation with custom code for upcasted elementals/fey
     */
    internal static bool CheckSubSpellActivated(SubspellSelectionModal __instance, int index)
    {
        if (!Main.Settings.EnableUpcastConjureElementalAndFey || _filteredSubspells is not { Count: > 0 })
        {
            return true;
        }

        if (_filteredSubspells.Count <= index)
        {
            return true;
        }

        __instance.spellCastEngaged?.Invoke(__instance.spellRepertoire, _filteredSubspells[index],
            __instance.slotLevel);

        __instance.Hide();

        _filteredSubspells.Clear();

        return false;
    }

    /**
     * Patch implementation
     * Replaces calls to masterSpell.SubspellsList getter with custom method that adds extra options for upcasted elementals/fey
     */
    internal static IEnumerable<CodeInstruction> ReplaceSubSpellList(IEnumerable<CodeInstruction> instructions)
    {
        var subspellsListMethod = typeof(SpellDefinition).GetMethod("get_SubspellsList");
        var getSpellList = new Func<SpellDefinition, int, List<SpellDefinition>>(SubspellsList);

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(subspellsListMethod))
            {
                yield return new CodeInstruction(OpCodes.Ldarg, 5); // slotLevel
                yield return new CodeInstruction(OpCodes.Call, getSpellList.Method);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    [CanBeNull]
    private static List<SpellDefinition> SubspellsList([NotNull] SpellDefinition masterSpell, int slotLevel)
    {
        var subspellsList = masterSpell.SubspellsList;
        var mySlotLevel = masterSpell.Name == ConjureElemental.Name
                          || masterSpell.Name == ConjureFey.Name
            ? slotLevel
            : -1;

        if (!Main.Settings.EnableUpcastConjureElementalAndFey || mySlotLevel < 0 || subspellsList == null ||
            subspellsList.Count == 0)
        {
            return subspellsList;
        }

        var subspellsGroupedAndFilteredByCR = subspellsList
            .Select(s =>
                new
                {
                    SpellDefinition = s,
                    s.EffectDescription
                        .GetFirstFormOfType(EffectForm.EffectFormType.Summon)
                        .SummonForm
                        .MonsterDefinitionName
                }
            )
            .Select(s => new
            {
                s.SpellDefinition,
                s.MonsterDefinitionName,
                ChallengeRating =
                    TryGetDefinition<MonsterDefinition>(s.MonsterDefinitionName, out var monsterDefinition)
                        ? monsterDefinition.ChallengeRating
                        : int.MaxValue
            })
            .GroupBy(s => s.ChallengeRating)
            .Select(g => new
            {
                ChallengeRating = g.Key,
                SpellDefinitions = g.Select(s => s.SpellDefinition)
                    .OrderBy(s => Gui.Localize(s.GuiPresentation.Title))
            })
            .Where(s => s.ChallengeRating <= mySlotLevel)
            .OrderByDescending(s => s.ChallengeRating)
            .ToList();

        var allOrMostPowerful = Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey
            ? subspellsGroupedAndFilteredByCR.Take(1).ToList()
            : subspellsGroupedAndFilteredByCR;

        _filteredSubspells = allOrMostPowerful.SelectMany(s => s.SpellDefinitions).ToList();

        _filteredSubspells.ForEach(s => Main.Log($"{Gui.Localize(s.GuiPresentation.Title)}"));

        return _filteredSubspells;
    }
}
