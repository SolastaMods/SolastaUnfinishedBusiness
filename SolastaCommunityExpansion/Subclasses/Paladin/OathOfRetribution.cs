using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using UnityEngine;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Subclasses.Paladin;

internal class OathOfRetribution : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("f5efd735-ff95-4256-ad17-dde585aeb5f3");
    private readonly CharacterSubclassDefinition Subclass;

    internal OathOfRetribution()
    {
        var paladinOathOfRetributionAutoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("PaladinOathOfRetributionAutoPreparedSpells", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Paladin)
            .SetPreparedSpellGroups(
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(3, SpellDefinitions.Bane, SpellDefinitions.HuntersMark),
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(5, SpellDefinitions.HoldPerson,
                    SpellDefinitions.MistyStep),
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(6, SpellDefinitions.Haste,
                    SpellDefinitions.ProtectionFromEnergy))
            .AddToDB();

        var conditionFrightenedZealousAccusation = ConditionDefinitionBuilder
            .Create("ConditionFrightenedZealousAccusation", SubclassNamespace)
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionFrightened.GuiPresentation.SpriteReference)
            .Configure(RuleDefinitions.DurationType.Minute, 1, false,
                FeatureDefinitionCombatAffinitys.CombatAffinityFrightened,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetSpecialInterruptions(
                RuleDefinitions.ConditionInterruption.Damaged,
                RuleDefinitions.ConditionInterruption.DamagedByFriendly
            )
            .AddToDB();

        var presentationBuilder3 = new GuiPresentationBuilder(
            "Feature/&PowerOathOfRetributionZealousAccusationTitle",
            "Feature/&PowerOathOfRetributionZealousAccusationDescription",
            FeatureDefinitionPowers.PowerDomainLawHolyRetribution
                .GuiPresentation.SpriteReference);

        var feature2 = BuildActionConditionPowerPaladinCD1(1, RuleDefinitions.UsesDetermination.Fixed,
            "Wisdom", RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.ChannelDivinity,
            RuleDefinitions.RangeType.Distance, 12, RuleDefinitions.TargetType.Individuals,
            ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.Minute, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn, "Wisdom", "Wisdom", 15,
            new List<SaveAffinityBySenseDescription>
            {
                BuildSaveAffinityBySense(SenseMode.Type.Darkvision,
                    RuleDefinitions.AdvantageType.Disadvantage),
                BuildSaveAffinityBySense(SenseMode.Type.SuperiorDarkvision,
                    RuleDefinitions.AdvantageType.Disadvantage)
            }, conditionFrightenedZealousAccusation, "PowerOathOfRetributionZealousAccusation",
            presentationBuilder3.Build());

        var conditionTrueStrikeZealousCondemnation = ConditionDefinitionBuilder
            .Create("ConditionTrueStrikeZealousCondemnation", SubclassNamespace)
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionGuided.GuiPresentation.SpriteReference)
            .Configure(RuleDefinitions.DurationType.Minute, 1, false,
                FeatureDefinitionCombatAffinitys.CombatAffinityTrueStrike)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetSpecialInterruptions(
                RuleDefinitions.ConditionInterruption.None
            )
            .AddToDB();

        var presentationBuilder5 = new GuiPresentationBuilder(
            "Feature/&PowerOathOfRetributionZealousCondemnationTitle",
            "Feature/&PowerOathOfRetributionZealousCondemnationDescription",
            FeatureDefinitionPowers.PowerOathOfTirmarSmiteTheHidden
                .GuiPresentation.SpriteReference);

        var feature3 = BuildActionConditionPowerPaladinCD2(1, RuleDefinitions.UsesDetermination.Fixed,
            "Wisdom", RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ChannelDivinity,
            RuleDefinitions.RangeType.Distance, 2, RuleDefinitions.TargetType.Individuals,
            ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.Minute, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn, conditionTrueStrikeZealousCondemnation,
            "PowerOathOfRetributionZealousCondemnation",
            presentationBuilder5.Build());

        var conditionBonusRushTenaciousPursuit = ConditionDefinitionBuilder
            .Create("ConditionBonusRushTenaciousPursuit", SubclassNamespace)
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionGuided.GuiPresentation.SpriteReference)
            .Configure(RuleDefinitions.DurationType.Round, 1, false,
                FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging,
                FeatureDefinitionActionAffinitys.ActionAffinityExpeditiousRetreat,
                FeatureDefinitionAdditionalActions.AdditionalActionExpeditiousRetreat)
            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(
                RuleDefinitions.ConditionInterruption.None
            )
            .AddToDB();

        var presentationBuilder7 = new GuiPresentationBuilder(
            "Feature/&PowerOathOfRetributionTenaciousPursuitTitle",
            "Feature/&PowerOathOfRetributionTenaciousPursuitDescription",
            FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike
                .GuiPresentation.SpriteReference);

        var feature4 = BuildBonusMoveAfterHitPower(RuleDefinitions.ActivationTime.OnAttackHit,
            RuleDefinitions.RechargeRate.LongRest, 1, "Wisdom", 5, "Charisma",
            "Feature/&PowerOathOfRetributionTenaciousPursuitShortTitle", RuleDefinitions.TargetType.Self,
            ActionDefinitions.ItemSelectionType.Equiped, RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self,
            RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn,
            conditionBonusRushTenaciousPursuit,
            "PowerOathOfRetributionTenaciousPursuit", presentationBuilder7.Build());

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PaladinOathOfRetribution", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.DomainBattle.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                paladinOathOfRetributionAutoPreparedSpells,
                feature2,
                feature3)
            .AddFeaturesAtLevel(7, feature4)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoicePaladinSacredOaths;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    private static SaveAffinityBySenseDescription BuildSaveAffinityBySense(
        SenseMode.Type senseType,
        RuleDefinitions.AdvantageType advantageType)
    {
        var root = new SaveAffinityBySenseDescription {senseType = senseType, advantageType = advantageType};

        return root;
    }

    private static FeatureDefinitionPower BuildActionConditionPowerPaladinCD1(
        int usesPerRecharge,
        RuleDefinitions.UsesDetermination usesDetermination,
        string usesAbilityScoreName,
        RuleDefinitions.ActivationTime activationTime,
        int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        RuleDefinitions.RangeType rangeType,
        int rangeParameter,
        RuleDefinitions.TargetType targetType,
        ActionDefinitions.ItemSelectionType itemSelectionType,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType endOfEffect,
        string savethrowtype,
        string savethrowDA,
        int savethrowDC,
        List<SaveAffinityBySenseDescription> saveAffinityBySenseDescriptions,
        ConditionDefinition condition1,
        string name,
        GuiPresentation guiPresentation)
    {
        var particleParameters = new EffectParticleParameters();

        particleParameters.Copy(SpellDefinitions.Fear.EffectDescription.EffectParticleParameters);

        var root3 = new ConditionForm
        {
            operation = ConditionForm.ConditionOperation.Add,
            conditionDefinition = condition1,
            conditionDefinitionName = condition1.Name
        };
        var root2 = new EffectForm
        {
            FormType = EffectForm.EffectFormType.Condition,
            formType = (EffectForm.EffectFormType)2,
            createdByCharacter = true,
            conditionForm = root3
        };
        var root1 = new EffectDescription
        {
            targetSide = RuleDefinitions.Side.Enemy,
            createdByCharacter = true,
            rangeType = rangeType,
            rangeParameter = rangeParameter,
            targetType = targetType,
            itemSelectionType = itemSelectionType,
            canBePlacedOnCharacter = true,
            durationType = durationType,
            durationParameter = durationParameter,
            endOfEffect = endOfEffect,
            hasSavingThrow = true,
            savingThrowAbility = savethrowtype,
            savingThrowDifficultyAbility = savethrowDA,
            fixedSavingThrowDifficultyClass = savethrowDC,
            savingThrowAffinitiesBySense = saveAffinityBySenseDescriptions,
            effectForms = new List<EffectForm> {root2},
            effectAdvancement = new EffectAdvancement(),
            effectParticleParameters = particleParameters
        };
        var instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();

        instance.fixedUsesPerRecharge = usesPerRecharge;
        instance.usesDetermination = usesDetermination;
        instance.activationTime = activationTime;
        instance.costPerUse = costPerUse;
        instance.rechargeRate = recharge;
        instance.usesAbilityScoreName = usesAbilityScoreName;
        instance.uniqueInstance = true;
        instance.effectDescription = root1;
        instance.name = name;
        instance.guiPresentation = guiPresentation;
        instance.guid = GuidHelper.Create(SubclassNamespace, name).ToString();

        DatabaseRepository.GetDatabase<FeatureDefinition>().Add(instance);

        return instance;
    }

    private static FeatureDefinitionPower BuildActionConditionPowerPaladinCD2(
        int usesPerRecharge,
        RuleDefinitions.UsesDetermination usesDetermination,
        string usesAbilityScoreName,
        RuleDefinitions.ActivationTime activationTime,
        int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        RuleDefinitions.RangeType rangeType,
        int rangeParameter,
        RuleDefinitions.TargetType targetType,
        ActionDefinitions.ItemSelectionType itemSelectionType,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType endOfEffect,
        ConditionDefinition condition1,
        string name,
        GuiPresentation guiPresentation)
    {
        var particleParameters = new EffectParticleParameters();

        particleParameters.Copy(SpellDefinitions.TrueStrike.EffectDescription.EffectParticleParameters);

        var root3 = new ConditionForm
        {
            operation = ConditionForm.ConditionOperation.Add,
            conditionDefinition = condition1,
            conditionDefinitionName = condition1.Name
        };
        var root2 = new EffectForm
        {
            FormType = EffectForm.EffectFormType.Condition,
            formType = (EffectForm.EffectFormType)2,
            createdByCharacter = true,
            conditionForm = root3
        };
        var root1 = new EffectDescription
        {
            targetSide = RuleDefinitions.Side.Enemy,
            createdByCharacter = true,
            rangeType = rangeType,
            rangeParameter = rangeParameter,
            targetType = targetType,
            itemSelectionType = itemSelectionType,
            canBePlacedOnCharacter = true,
            durationType = durationType,
            durationParameter = durationParameter,
            endOfEffect = endOfEffect,
            effectForms = new List<EffectForm> {root2},
            effectAdvancement = new EffectAdvancement(),
            effectParticleParameters = particleParameters
        };
        var instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();

        instance.fixedUsesPerRecharge = usesPerRecharge;
        instance.usesDetermination = usesDetermination;
        instance.activationTime = activationTime;
        instance.costPerUse = costPerUse;
        instance.rechargeRate = recharge;
        instance.usesAbilityScoreName = usesAbilityScoreName;
        instance.uniqueInstance = true;
        instance.effectDescription = root1;
        instance.name = name;
        instance.guiPresentation = guiPresentation;
        instance.guid = GuidHelper.Create(SubclassNamespace, name).ToString();

        DatabaseRepository.GetDatabase<FeatureDefinition>().Add(instance);

        return instance;
    }

    private static FeatureDefinitionPower BuildBonusMoveAfterHitPower(
        RuleDefinitions.ActivationTime activationTime,
        RuleDefinitions.RechargeRate rechargerate,
        int costPerUse,
        string usesAbilityScoreName,
        int fixedusesPerRecharge,
        string abilityScoreName,
        string shorttitle,
        RuleDefinitions.TargetType targetType,
        ActionDefinitions.ItemSelectionType itemSelectionType,
        RuleDefinitions.Side targetside,
        RuleDefinitions.RangeType rangetype,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType turnoccurence,
        ConditionDefinition condition,
        string name,
        GuiPresentation guiPresentation)
    {
        var particleParameters = new EffectParticleParameters();

        particleParameters.Copy(FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription
            .EffectParticleParameters);

        var root3 = new ConditionForm
        {
            operation = ConditionForm.ConditionOperation.Add,
            conditionDefinition = condition,
            conditionDefinitionName = condition.Name,
            applyToSelf = true
        };
        var root2 = new EffectForm
        {
            FormType = EffectForm.EffectFormType.Condition, createdByCharacter = true, conditionForm = root3
        };
        var root1 = new EffectDescription
        {
            targetType = targetType,
            itemSelectionType = itemSelectionType,
            targetSide = targetside,
            createdByCharacter = true,
            rangeType = rangetype,
            canBePlacedOnCharacter = true,
            durationType = durationType,
            durationParameter = durationParameter,
            endOfEffect = turnoccurence,
            effectAdvancement = new EffectAdvancement {incrementMultiplier = 1},
            effectForms = new List<EffectForm> {root2},
            effectParticleParameters = particleParameters
        };

        var instance = ScriptableObject.CreateInstance<FeatureDefinitionPower>();

        instance.activationTime = activationTime;
        instance.rechargeRate = rechargerate;
        instance.costPerUse = costPerUse;
        instance.usesAbilityScoreName = usesAbilityScoreName;
        instance.fixedUsesPerRecharge = fixedusesPerRecharge;
        instance.abilityScore = abilityScoreName;
        instance.showCasting = true;
        instance.uniqueInstance = true;
        instance.shortTitleOverride = shorttitle;
        instance.effectDescription = root1;
        instance.name = name;
        instance.guiPresentation = guiPresentation;
        instance.guid = GuidHelper.Create(SubclassNamespace, name).ToString();

        DatabaseRepository.GetDatabase<FeatureDefinition>().Add(instance);

        return instance;
    }
}
