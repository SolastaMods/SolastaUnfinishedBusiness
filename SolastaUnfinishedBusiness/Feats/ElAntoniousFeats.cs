using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalActions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ElAntoniousFeats
{
    public static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        feats.Add(FeatDualFlurryBuilder.FeatDualFlurry);
        feats.Add(FeatTorchbearerBuilder.FeatTorchbearer);
    }
}

internal sealed class FeatDualFlurryBuilder : FeatDefinitionBuilder
{
    private const string FeatDualFlurryName = "FeatDualFlurry";
    private const string FeatDualFlurryNameGuid = "03C523EB-91B9-4F1B-A697-804D1BC2D6DD";

    internal static readonly FeatDefinition FeatDualFlurry =
        CreateAndAddToDB(FeatDualFlurryName, FeatDualFlurryNameGuid);

    private FeatDualFlurryBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.Ambidextrous,
        name, guid)
    {
        Definition.Features.SetRange(BuildFeatureDualFlurry());
        Definition.minimalAbilityScorePrerequisite = false;
    }

    private static FeatDefinition CreateAndAddToDB(string name, string guid)
    {
        return new FeatDualFlurryBuilder(name, guid)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    private static FeatureDefinition BuildFeatureDualFlurry()
    {
        return FeatureDefinitionOnAttackDamageEffectBuilder
            .Create("OnAttackDamageEffectFeatDualFlurry", CENamespaceGuid)
            .SetGuiPresentation("FeatDualFlurry", Category.Feat)
            .SetOnAttackDamageDelegates(null, AfterOnAttackDamage)
            .AddToDB();
    }

    private static void AfterOnAttackDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier attackModifier,
        [CanBeNull] RulesetAttackMode attackMode,
        bool rangedAttack,
        RuleDefinitions.AdvantageType advantageType,
        List<EffectForm> actualEffectForms,
        RulesetEffect rulesetEffect,
        bool criticalHit,
        bool firstTarget)
    {
        if (rangedAttack || attackMode == null)
        {
            return;
        }

        var condition = attacker.RulesetCharacter.HasConditionOfType(ConditionDualFlurryApplyBuilder.ConditionDualFlurryApply.Name)
            ? ConditionDualFlurryGrantBuilder.ConditionDualFlurryGrant
            : ConditionDualFlurryApplyBuilder.ConditionDualFlurryApply;

        var rulesetCondition = RulesetCondition.CreateActiveCondition(
            attacker.RulesetCharacter.Guid,
            condition, RuleDefinitions.DurationType.Round, 0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            attacker.RulesetCharacter.Guid,
            attacker.RulesetCharacter.CurrentFaction.Name);

        attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
    }
}

internal sealed class ConditionDualFlurryApplyBuilder : ConditionDefinitionBuilder
{
    internal static readonly ConditionDefinition ConditionDualFlurryApply =
        CreateAndAddToDB(
            "ConditionDualFlurryApply",
            GuidHelper.Create(CENamespaceGuid, "ConditionDualFlurryApply").ToString());
    
    private ConditionDualFlurryApplyBuilder(string name, string guid) : base(
        DatabaseHelper.ConditionDefinitions.ConditionSurged, name, guid)
    {
        Definition.allowMultipleInstances = false;
        Definition.durationParameter = 0;
        Definition.durationType = RuleDefinitions.DurationType.Round;
        Definition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
        Definition.possessive = true;
        Definition.silentWhenAdded = true;
        Definition.silentWhenRemoved = true;
        Definition.conditionType = RuleDefinitions.ConditionType.Beneficial;
        Definition.Features.Clear();
    }

    private static ConditionDefinition CreateAndAddToDB(string name, string guid)
    {
        return new ConditionDualFlurryApplyBuilder(name, guid)
            .SetGuiPresentation(Category.Condition)
            .AddToDB();
    }
}

internal sealed class ConditionDualFlurryGrantBuilder : ConditionDefinitionBuilder
{
    internal static readonly ConditionDefinition ConditionDualFlurryGrant =
        CreateAndAddToDB(
            "ConditionDualFlurryGrant",
            GuidHelper.Create(CENamespaceGuid, "ConditionDualFlurryGrant").ToString());

    private ConditionDualFlurryGrantBuilder(string name, string guid) : base(
        DatabaseHelper.ConditionDefinitions.ConditionSurged, name, guid)
    {
        Definition.GuiPresentation.hidden = true;
        Definition.allowMultipleInstances = false;
        Definition.durationParameter = 0;
        Definition.durationType = RuleDefinitions.DurationType.Round;
        Definition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
        Definition.possessive = true;
        Definition.silentWhenAdded = false;
        Definition.silentWhenRemoved = false;
        Definition.conditionType = RuleDefinitions.ConditionType.Beneficial;
        Definition.Features.Clear();
        Definition.Features.Add(BuildAdditionalActionDualFlurry());
    }

    private static ConditionDefinition CreateAndAddToDB(string name, string guid)
    {
        return new ConditionDualFlurryGrantBuilder(name, guid)
            .SetGuiPresentation(Category.Condition)
            .AddToDB();
    }

    private static FeatureDefinition BuildAdditionalActionDualFlurry()
    {
        return FeatureDefinitionAdditionalActionBuilder
            .Create(AdditionalActionSurgedMain, "AdditionalActionDualFlurry", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, AdditionalActionSurgedMain.GuiPresentation.SpriteReference)
            .SetActionType(ActionDefinitions.ActionType.Bonus)
            .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
            .AddToDB();
    }
}

internal sealed class FeatTorchbearerBuilder : FeatDefinitionBuilder
{
    private const string FeatTorchbearerName = "FeatTorchbearer";
    private const string FeatTorchbearerNameGuid = "03C523EB-91B9-4F1B-A697-804D1BC2D6DD";

    public static readonly FeatDefinition FeatTorchbearer =
        CreateAndAddToDB(FeatTorchbearerName, FeatTorchbearerNameGuid);

    private FeatTorchbearerBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.Ambidextrous,
        name, guid)
    {
        Definition.Features.SetRange(BuildFeatureTorchbearer());
        Definition.minimalAbilityScorePrerequisite = false;
    }

    private static FeatDefinition CreateAndAddToDB(string name, string guid)
    {
        return new FeatTorchbearerBuilder(name, guid)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    private static FeatureDefinition BuildFeatureTorchbearer()
    {
        var burnEffect = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            ConditionForm = new ConditionForm
            {
                Operation = ConditionForm.ConditionOperation.Add,
                ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionOnFire1D4
            }
        };

        var burnDescription = new EffectDescription();

        burnDescription.Copy(DatabaseHelper.SpellDefinitions.Fireball.EffectDescription);
        burnDescription.SetCreatedByCharacter(true);
        burnDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
        burnDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
        burnDescription.SetTargetParameter(1);
        burnDescription.SetRangeType(RuleDefinitions.RangeType.Touch);
        burnDescription.SetDurationType(RuleDefinitions.DurationType.Round);
        burnDescription.SetDurationParameter(3);
        burnDescription.SetCanBePlacedOnCharacter(false);
        burnDescription.SetHasSavingThrow(true);
        burnDescription.SetSavingThrowAbility(AttributeDefinitions.Dexterity);
        burnDescription.SetSavingThrowDifficultyAbility(AttributeDefinitions.Dexterity);
        burnDescription.SetDifficultyClassComputation(
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
        burnDescription.SetSpeedType(RuleDefinitions.SpeedType.Instant);
        burnDescription.EffectForms.Clear();
        burnDescription.EffectForms.Add(burnEffect);

        return FeatureDefinitionPowerBuilder
            .Create("PowerTorchbearer", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetActivation(RuleDefinitions.ActivationTime.BonusAction, 0)
            .SetEffectDescription(burnDescription)
            .SetUsesFixed(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetShowCasting(false)
            .SetCustomSubFeatures(new ValidatorPowerUse(ValidatorsCharacter.OffHandHasLightSource))
            .AddToDB();
    }
}
