using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using TA;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
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

    private static readonly Dictionary<string, TagsDefinitions.Criticity> Tags = new();

    private static readonly List<MonsterDefinition> MonstersThatEmitLight = new()
    {
        CubeOfLight,
        Fire_Elemental,
        Fire_Jester,
        Fire_Osprey,
        Fire_Spider
    };

    private static readonly FeatureDefinitionActionAffinity ActionAffinityConditionBlind =
        FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityConditionBlind")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(Id.AttackOpportunity)
            .AddToDB();

    private static SpellDefinition ConjureElementalInvisibleStalker { get; set; }

    internal static void LateLoad()
    {
        //SETTING: modify normal vision range
        SenseNormalVision.senseRange = Main.Settings.IncreaseSenseNormalVision;

        AddBleedingToRestoration();
        AllowTargetingSelectionWhenCastingChainLightningSpell();
        ApplyConditionBlindedShouldNotAllowOpportunityAttack();
        ApplySrdWeightToFoodRations();
        BuildConjureElementalInvisibleStalker();
        LoadAfterRestIdentify();
        SwitchAllowClubsToBeThrown();
        SwitchDarknessSpell();
        SwitchDruidAllowMetalArmor();
        SwitchEldritchBlastRange();
        SwitchEnableUpcastConjureElementalAndFey();
        SwitchFilterOnHideousLaughter();
        SwitchFullyControlConjurations();
        SwitchMagicStaffFoci();
        SwitchRecurringEffectOnEntangle();
        SwitchUniversalSylvanArmorAndLightbringer();
        UseCubeOnSleetStorm();
        UseHeightOneCylinderEffect();
        SwitchHastedCasing();
        ActionSwitching.Load();
    }

    internal static void AddLightSourceIfNeeded(GameLocationCharacter gameLocationCharacter)
    {
        if (!Main.Settings.EnableCharactersOnFireToEmitLight)
        {
            return;
        }

        if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
        {
            return;
        }

        if (!MonstersThatEmitLight.Contains(rulesetCharacterMonster.MonsterDefinition))
        {
            return;
        }

        AddLightSource(gameLocationCharacter, rulesetCharacterMonster, "ShouldEmitLightFromMonster");
    }

    internal static void AddLightSourceIfNeeded(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
    {
        if (!Main.Settings.EnableCharactersOnFireToEmitLight)
        {
            return;
        }

        if (rulesetCondition == null || !rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionOnFire.Name))
        {
            return;
        }

        if (rulesetActor is not RulesetCharacter rulesetCharacter)
        {
            return;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (gameLocationCharacter == null)
        {
            return;
        }

        AddLightSource(gameLocationCharacter, rulesetCharacter, "ShouldEmitLightFromCondition");
    }

    private static void AddLightSource(
        GameLocationCharacter gameLocationCharacter,
        RulesetCharacter rulesetCharacter,
        string name)
    {
        var lightSourceForm = Shine.EffectDescription.EffectForms[0].LightSourceForm;

        rulesetCharacter.PersonalLightSource?.Unregister();
        rulesetCharacter.PersonalLightSource = new RulesetLightSource(
            lightSourceForm.Color,
            2,
            4,
            lightSourceForm.GraphicsPrefabAssetGUID,
            LightSourceType.Basic,
            name,
            rulesetCharacter.Guid);

        rulesetCharacter.PersonalLightSource.Register(true);

        ServiceRepository.GetService<IGameLocationVisibilityService>()?
            .AddCharacterLightSource(gameLocationCharacter, rulesetCharacter.PersonalLightSource);
    }

    internal static void RemoveLightSourceIfNeeded(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
    {
        if (rulesetCondition == null ||
            !rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionOnFire.Name))
        {
            return;
        }

        if (rulesetActor is not RulesetCharacter rulesetCharacter ||
            rulesetCharacter.PersonalLightSource == null) // if using extinguish fire light source will come null here
        {
            return;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (gameLocationCharacter == null)
        {
            return;
        }

        ServiceRepository.GetService<IGameLocationVisibilityService>()?
            .RemoveCharacterLightSource(gameLocationCharacter, rulesetCharacter.PersonalLightSource);

        rulesetCharacter.PersonalLightSource.Unregister();
        rulesetCharacter.PersonalLightSource = null;
    }

    internal static void SwitchUniversalSylvanArmorAndLightbringer()
    {
        GreenmageArmor.RequiredAttunementClasses.Clear();
        WizardClothes_Alternate.RequiredAttunementClasses.Clear();

        if (Main.Settings.AllowAnyClassToWearSylvanArmor)
        {
            return;
        }

        var allowedClasses = new[] { Wizard, Sorcerer, Warlock };

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

    internal static void ApplyConditionBlindedShouldNotAllowOpportunityAttack()
    {
        if (Main.Settings.BlindedConditionDontAllowAttackOfOpportunity)
        {
            if (!ConditionDefinitions.ConditionBlinded.Features.Contains(ActionAffinityConditionBlind))
            {
                ConditionDefinitions.ConditionBlinded.Features.Add(ActionAffinityConditionBlind);
            }
        }
        else
        {
            if (ConditionDefinitions.ConditionBlinded.Features.Contains(ActionAffinityConditionBlind))
            {
                ConditionDefinitions.ConditionBlinded.Features.Remove(ActionAffinityConditionBlind);
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
            // This is half fix, half houses rules since it's not completely SRD but better than implemented.
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

    internal static void ApplySrdWeightToFoodRations()
    {
        var foodSrdWeight = Food_Ration;
        var foodForagedSrdWeight = Food_Ration_Foraged;

        if (Main.Settings.UseOfficialFoodRationsWeight)
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
            HideousLaughter.effectDescription.restrictedCreatureFamilies.Add(CharacterFamilyDefinitions.Humanoid.Name);
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

    internal static void SwitchDarknessSpell()
    {
        //BUGFIX: Now that we have better handled sight and advantage elsewhere, darkness should no more give other source of disadvantage on attack
        // and the immunity to condition darkness provided by the condition affinity below prevents one with devil sight and similar abilities, causing other problems
        // so here we remove this immunity
        if (Main.Settings.AttackersWithDarkvisionHaveAdvantageOverDefendersWithout)
        {
            FeatureDefinitionCombatAffinitys.CombatAffinityVeil.myAttackAdvantage = AdvantageType.None;
            FeatureDefinitionConditionAffinitys.ConditionAffinityInvocationDevilsSight.conditionAffinityType =
                ConditionAffinityType.None;
        }
        else
        {
            FeatureDefinitionCombatAffinitys.CombatAffinityVeil.myAttackAdvantage = AdvantageType.Disadvantage;
            FeatureDefinitionConditionAffinitys.ConditionAffinityInvocationDevilsSight.conditionAffinityType =
                ConditionAffinityType.Immunity;
        }
    }

    internal static void SwitchEldritchBlastRange()
    {
        EldritchBlast.effectDescription.rangeParameter = Main.Settings.FixEldritchBlastRange ? 24 : 16;
    }

    internal static void UseHeightOneCylinderEffect()
    {
        // always applicable
        ClearTargetParameter2ForTargetTypeCube();

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

    internal static void SwitchHastedCasing()
    {
        var restrictedActions = FeatureDefinitionAdditionalActions.AdditionalActionHasted.RestrictedActions;
        if (Main.Settings.AllowHasteCasting)
        {
            restrictedActions.TryAdd(Id.CastMain);
        }
        else
        {
            restrictedActions.RemoveAll(id => id == Id.CastMain);
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

        var cfg = GreaterRestoration.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Condition);

        if (cfg == null)
        {
            return;
        }

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

    /// <summary>
    ///     Allow conjurations to fully controlled party members instead of AI controlled.
    /// </summary>
    private static void BuildConjureElementalInvisibleStalker()
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

    internal static void SwitchAllowClubsToBeThrown()
    {
        var db = DatabaseRepository.GetDatabase<ItemDefinition>();

        foreach (var itemDefinition in db
                     .Where(x => x.IsWeapon &&
                                 x.WeaponDescription.WeaponTypeDefinition == WeaponTypeDefinitions.ClubType))
        {
            if (Main.Settings.AllowClubsToBeThrown)
            {
                itemDefinition.WeaponDescription.WeaponTags.Add(TagsDefinitions.WeaponTagThrown);
                itemDefinition.WeaponDescription.maxRange = 10;
            }
            else
            {
                itemDefinition.WeaponDescription.WeaponTags.Remove(TagsDefinitions.WeaponTagThrown);
                itemDefinition.WeaponDescription.maxRange = 5;
            }
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
        ConfigureAdvancement(ConjureMinorElementals);
        ConjureElemental.SubspellsList.Add(ConjureElementalInvisibleStalker);

        // Set advancement at spell level, not sub-spell
        static void ConfigureAdvancement([NotNull] IMagicEffect spell)
        {
            var advancement = spell.EffectDescription.EffectAdvancement;

            advancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;
            advancement.additionalSpellLevelPerIncrement = 1;
        }
    }

    private static void LoadAfterRestIdentify()
    {
        const string AfterRestIdentifyName = "PowerAfterRestIdentify";

        RestActivityDefinitionBuilder
            .Create("RestActivityShortRestIdentify")
            .SetGuiPresentation(AfterRestIdentifyName, Category.Feature)
            .AddCustomSubFeatures(new RestActivityValidationParams(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.None,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                AfterRestIdentifyName)
            .AddToDB();

        RestActivityDefinitionBuilder
            .Create("RestActivityLongRestIdentify")
            .SetGuiPresentation(AfterRestIdentifyName, Category.Feature)
            .AddCustomSubFeatures(new RestActivityValidationParams(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.LongRest,
                RestActivityDefinition.ActivityCondition.None,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                AfterRestIdentifyName)
            .AddToDB();

        var afterRestIdentifyCondition = ConditionDefinitionBuilder
            .Create("AfterRestIdentify")
            .SetGuiPresentation(Category.Condition)
            .AddCustomSubFeatures(OnConditionAddedOrRemovedIdentifyItems.Mark)
            .AddToDB();

        FeatureDefinitionPowerBuilder
            .Create(AfterRestIdentifyName)
            .SetGuiPresentation(Category.Feature, hidden: true)
            .AddCustomSubFeatures(CanIdentifyOnRest.Mark)
            .SetUsesFixed(ActivationTime.Rest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Self,
                        1,
                        TargetType.Self)
                    .SetDurationData(
                        DurationType.Minute,
                        1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                afterRestIdentifyCondition,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();
    }

    internal static bool IsAttackModeInvalid(RulesetCharacter character, RulesetAttackMode mode)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return false;
        }

        return IsHandCrossbowUseInvalid(mode.sourceObject as RulesetItem, hero,
            hero.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand),
            hero.GetItemInSlot(EquipmentDefinitions.SlotTypeOffHand));
    }

    internal static bool IsHandCrossbowUseInvalid(
        RulesetItem item,
        RulesetCharacterHero hero,
        RulesetItem main,
        RulesetItem off)
    {
        if (Main.Settings.IgnoreHandXbowFreeHandRequirements)
        {
            return false;
        }

        if (item == null || hero == null)
        {
            return false;
        }

        Tags.Clear();
        item.FillTags(Tags, hero, true);

        if (!Tags.ContainsKey(TagsDefinitions.WeaponTagAmmunition) ||
            Tags.ContainsKey(TagsDefinitions.WeaponTagTwoHanded))
        {
            return false;
        }

        if (main == item && off != null)
        {
            return true;
        }

        return off == item
               && main != null
               && main.ItemDefinition.WeaponDescription?.WeaponType != WeaponTypeDefinitions.UnarmedStrikeType.Name;
    }

    private sealed class CanIdentifyOnRest : IValidatePowerUse
    {
        private CanIdentifyOnRest()
        {
        }

        public static CanIdentifyOnRest Mark { get; } = new();

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            //does this work properly for wild-shaped heroes?
            if (character is not RulesetCharacterHero hero)
            {
                return false;
            }

            //TODO: show only if has detected unidentified items
            //TODO: show only if anyone in party has identify (optional)
            return Main.Settings.IdentifyAfterRest
                   && hero.HasNonIdentifiedItems();
        }
    }

    private sealed class OnConditionAddedOrRemovedIdentifyItems : IOnConditionAddedOrRemoved
    {
        public static OnConditionAddedOrRemovedIdentifyItems Mark { get; } = new();

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            (target as RulesetCharacterHero)?.AutoIdentifyInventoryItems();
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
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

        if (!items.Any(item => item.ItemDefinition.ItemTags.Contains(spellDefinition.SpecificMaterialComponentTag) &&
                               EquipmentDefinitions.GetApproximateCostInGold(item.ItemDefinition.Costs) *
                               item.StackCount >= spellDefinition.SpecificMaterialComponentCostGp))
        {
            return;
        }

        result = true;
        failure = string.Empty;
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
            // ReSharper disable once InvocationIsSkipped
            Main.Log("Didn't find item.");

            return false;
        }

        // ReSharper disable once InvocationIsSkipped
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
            // ReSharper disable once InvocationIsSkipped
            Main.Log($"Spending stack={itemToUse.StackCountRequired}, cost={itemToUse.TotalCost}");

            rulesetItem.SpendStack(itemToUse.StackCountRequired);
        }
        else
        {
            // ReSharper disable once InvocationIsSkipped
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

        __instance.spellCastEngaged?.Invoke(
            __instance.spellRepertoire, _filteredSubspells[index], __instance.slotLevel);

        __instance.Hide();

        _filteredSubspells.Clear();

        return false;
    }

    /**
     * Patch implementation
     * Replaces calls to masterSpell.SubspellsList getter with custom method that adds extra options for upcasted elementals/fey
     */
    [CanBeNull]
    internal static List<SpellDefinition> SubspellsList([NotNull] SpellDefinition masterSpell, int slotLevel)
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
                })
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

        return _filteredSubspells;
    }
}

internal static class FlankingAndHigherGroundRules
{
    private static readonly Dictionary<(ulong, ulong), bool> FlankingDeterminationCache = new();

    internal static void ClearFlankingDeterminationCache()
    {
        FlankingDeterminationCache.Clear();
    }

    private static IEnumerable<CellFlags.Side> GetEachSide(CellFlags.Side side)
    {
        if ((side & CellFlags.Side.North) > 0)
        {
            yield return CellFlags.Side.North;
        }

        if ((side & CellFlags.Side.South) > 0)
        {
            yield return CellFlags.Side.South;
        }

        if ((side & CellFlags.Side.East) > 0)
        {
            yield return CellFlags.Side.East;
        }

        if ((side & CellFlags.Side.West) > 0)
        {
            yield return CellFlags.Side.West;
        }

        if ((side & CellFlags.Side.Top) > 0)
        {
            yield return CellFlags.Side.Top;
        }

        if ((side & CellFlags.Side.Bottom) > 0)
        {
            yield return CellFlags.Side.Bottom;
        }
    }

    private static IEnumerable<int3> GetPositions(GameLocationCharacter gameLocationCharacter)
    {
        // collect all positions in the character cube surface
        var basePosition = gameLocationCharacter.LocationPosition;
        var maxExtents = gameLocationCharacter.SizeParameters.maxExtent;

        // traverse by horizontal planes as most common use case in battles
        for (var x = 0; x <= maxExtents.x; x++)
        {
            for (var z = 0; z <= maxExtents.z; z++)
            {
                for (var y = 0; y <= maxExtents.y; y++)
                {
                    yield return basePosition + new int3(x, y, z);
                }
            }
        }
    }

    private static bool IsFlanking(GameLocationCharacter attacker, GameLocationCharacter defender)
    {
        if (FlankingDeterminationCache.TryGetValue((attacker.Guid, defender.Guid), out var result))
        {
            return result;
        }

        FlankingDeterminationCache.Add((attacker.Guid, defender.Guid), false);

        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

        if (gameLocationBattleService is not { IsBattleInProgress: true })
        {
            return false;
        }

        var allies = gameLocationBattleService.Battle.AllContenders
            .Where(x =>
                x != attacker &&
                x.Side == attacker.Side &&
                x.CanAct() &&
                gameLocationBattleService.IsWithin1Cell(x, defender))
            .ToList();

        if (allies.Count == 0)
        {
            return false;
        }

        // collect all possible flanking sides from all attacker cells against all enemy cells

        var attackerFlankingSides = new HashSet<CellFlags.Side>();

        foreach (var attackerPosition in GetPositions(attacker))
        {
            foreach (var defenderPosition in GetPositions(defender))
            {
                var attackerDirection = defenderPosition - attackerPosition;
                var attackerSide = CellFlags.DirectionToAllSurfaceSides(attackerDirection);
                var flankingSide = GetEachSide(attackerSide)
                    .Aggregate(CellFlags.Side.None, (current, side) => current | CellFlags.InvertSide(side));

                attackerFlankingSides.Add(flankingSide);
            }
        }

        result = allies
            .Any(ally => GetPositions(ally)
                .Any(allyPosition => GetPositions(defender)
                    .Any(defenderPosition =>
                        attackerFlankingSides.Contains(
                            CellFlags.DirectionToAllSurfaceSides(defenderPosition - allyPosition)))));

        FlankingDeterminationCache[(attacker.Guid, defender.Guid)] = result;

        return result;
    }

    //
    // FLANKING IMPLEMENTATION WITH MATH
    // Uses classes in FlankingMathExtensions
    private static bool IsFlankingWithMath(GameLocationCharacter attacker, GameLocationCharacter defender)
    {
        if (FlankingDeterminationCache.TryGetValue((attacker.Guid, defender.Guid), out var result))
        {
            return result;
        }

        FlankingDeterminationCache.Add((attacker.Guid, defender.Guid), false);

        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

        if (gameLocationBattleService is not { IsBattleInProgress: true })
        {
            return false;
        }

        var attackerCenter = new FlankingMathExtensions.Point3D(attacker.LocationBattleBoundingBox.Center);
        var defenderCube = new FlankingMathExtensions.Cube(
            new FlankingMathExtensions.Point3D(defender.LocationBattleBoundingBox.Min),
            new FlankingMathExtensions.Point3D(defender.LocationBattleBoundingBox.Max + 1));

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var ally in gameLocationBattleService.Battle.AllContenders)
        {
            if (ally == attacker
                || ally == defender
                || ally.IsOppositeSide(attacker.Side)
                || !ally.CanAct()
                || !gameLocationBattleService.IsWithin1Cell(ally, defender))
            {
                continue;
            }

            var allyCenter = new FlankingMathExtensions.Point3D(ally.LocationBattleBoundingBox.Center);
            result = FlankingMathExtensions.LineIntersectsCubeOppositeSides(attackerCenter, allyCenter, defenderCube);

            if (result)
            {
                break;
            }
        }

        FlankingDeterminationCache[(attacker.Guid, defender.Guid)] = result;

        return result;
    }

    internal static void HandleFlanking(BattleDefinitions.AttackEvaluationParams evaluationParams)
    {
        if (!Main.Settings.UseOfficialFlankingRules)
        {
            return;
        }

        if (!Main.Settings.UseOfficialFlankingRulesAlsoForRanged && evaluationParams.attackProximity is
                not (BattleDefinitions.AttackProximity.PhysicalReach or BattleDefinitions.AttackProximity.MagicReach))
        {
            return;
        }

        var attacker = evaluationParams.attacker;
        var defender = evaluationParams.defender;
        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

        if (!Main.Settings.UseOfficialFlankingRulesAlsoForReach &&
            (gameLocationBattleService == null || !gameLocationBattleService.IsWithin1Cell(attacker, defender)))
        {
            return;
        }

        if (Main.Settings.UseMathFlankingRules)
        {
            if (!IsFlankingWithMath(attacker, defender))
            {
                return;
            }
        }
        else
        {
            if (!IsFlanking(attacker, defender))
            {
                return;
            }
        }

        var actionModifier = evaluationParams.attackModifier;

        if (Main.Settings.UseOfficialFlankingRulesButAddAttackModifier)
        {
            actionModifier.attackRollModifier += 1;
            actionModifier.attackToHitTrends.Add(
                new TrendInfo(1, FeatureSourceType.Unknown, "Feedback/&FlankingAttack", null));
        }
        else
        {
            actionModifier.AttackAdvantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.Unknown, "Feedback/&FlankingAttack", null));
        }
    }

    internal static void HandleHigherGround(BattleDefinitions.AttackEvaluationParams evaluationParams)
    {
        if (!Main.Settings.EnableHigherGroundRules)
        {
            return;
        }

        var attacker = evaluationParams.attacker;
        var defender = evaluationParams.defender;

        if (attacker.LocationPosition.y <= defender.LocationPosition.y)
        {
            return;
        }

        var actionModifier = evaluationParams.attackModifier;

        actionModifier.attackRollModifier += 1;
        actionModifier.attackToHitTrends.Add(
            new TrendInfo(1, FeatureSourceType.Unknown, "Feedback/&HigherGroundAttack", null));
    }
}
