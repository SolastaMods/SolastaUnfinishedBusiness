using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Feats;

public static class EwFeats
{
    public const string SentinelFeat = "FeatSentinel";
    private const string PolearmExpertFeat = "FeatPolearmExpert";
    private const string RangedExpertFeat = "FeatRangedExpert";
    private const string RecklessAttackFeat = "FeatRecklessAttack";

    public static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        feats.Add(BuildSentinel());
        feats.Add(BuildPolearmExpert());
        feats.Add(BuildRangedExpert());
        feats.Add(BuildRecklessAttack());
        feats.Add(BuildPowerAttack());
    }

    private static FeatDefinition BuildSentinel()
    {
        var restrained = ConditionDefinitions.ConditionRestrained;

        var stopMovementCondition = ConditionDefinitionBuilder
            .Create("ConditionSentinelStopMovement", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Condition, Gui.NoLocalization, restrained.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained
            )
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(SentinelFeat, DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionOnAttackHitEffectBuilder
                .Create("OnAttackHitEffectFeatSentinel", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentationNoContent(true)
                .SetOnAttackHitDelegates(null, (attacker, defender, outcome, _, mode, _) =>
                {
                    if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
                    {
                        return;
                    }

                    if (mode is not { ActionType: ActionDefinitions.ActionType.Reaction })
                    {
                        return;
                    }

                    if (mode.AttackTags.Contains(AttacksOfOpportunity.NotAoOTag))
                    {
                        return;
                    }

                    var character = defender.RulesetCharacter;

                    character.AddConditionOfCategory(AttributeDefinitions.TagCombat,
                        RulesetCondition.CreateActiveCondition(character.Guid,
                            stopMovementCondition,
                            DurationType.Round,
                            1,
                            TurnOccurenceType.StartOfTurn,
                            attacker.Guid,
                            string.Empty
                        ));
                })
                .SetCustomSubFeatures(
                    AttacksOfOpportunity.CanIgnoreDisengage,
                    AttacksOfOpportunity.SentinelFeatMarker
                )
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildPolearmExpert()
    {
        return FeatDefinitionBuilder
            .Create(PolearmExpertFeat, DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("PolearmFeatFeature", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new CanMakeAoOOnReachEntered(ValidatorsCharacter.HasPolearm),
                    new AddPolearmFollowupAttack()
                )
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildRangedExpert()
    {
        return FeatDefinitionBuilder
            .Create(RangedExpertFeat, DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatRangedExpertFeature", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new RangedAttackInMeleeDisadvantageRemover(),
                    new AddExtraRangedAttack(IsOneHandedRanged, ActionDefinitions.ActionType.Bonus,
                        ValidatorsCharacter.HasAttacked)
                )
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildRecklessAttack()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create(RecklessAttackFeat, DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("RecklessAttack", Category.Action)
            .SetFeatures(FeatureDefinitionActionAffinitys.ActionAffinityBarbarianRecklessAttack)
            .SetValidators(ValidatorsFeat.ValidateNotClass(CharacterClassDefinitions.Barbarian))
            .AddToDB();
    }

    private static bool IsOneHandedRanged(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
    {
        return ValidatorsWeapon.IsRanged(weapon) && ValidatorsWeapon.IsOneHanded(weapon);
    }

    private static FeatDefinition BuildPowerAttack()
    {
        var concentrationProvider = new StopPowerConcentrationProvider("PowerAttack",
            "Tooltip/&PowerAttackConcentration", CustomIcons.CreateAssetReferenceSprite("PowerAttackConcentrationIcon",
                Resources.PowerAttackConcentrationIcon, 64, 64));

        var triggerCondition = ConditionDefinitionBuilder
            .Create("ConditionPowerAttackTrigger", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(DurationType.Permanent)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("PowerAttackTriggerFeature", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(concentrationProvider)
                .AddToDB())
            .AddToDB();

        var powerAttackCondition = ConditionDefinitionBuilder
            .Create("ConditionPowerAttack", "c125b7b9-e668-4c6f-a742-63c065ad2292")
            .SetGuiPresentation("PowerAttack", Category.Feature,
                ConditionDefinitions.ConditionHeraldOfBattle.GuiPresentation.SpriteReference)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAllowMultipleInstances(false)
            .SetFeatures(PowerAttackOneHandedAttackModifierBuilder.PowerAttackAttackModifier)
            .SetDuration(DurationType.Round, 1)
            .AddToDB();

        var powerAttackPower = FeatureDefinitionPowerBuilder
            .Create("PowerAttack", "0a3e6a7d-4628-4189-b91d-d7146d774bb6")
            .SetGuiPresentation("FeatPowerAttack", Category.Feat,
                CustomIcons.CreateAssetReferenceSprite("PowerAttackIcon", Resources.PowerAttackIcon, 128, 64))
            .SetActivationTime(ActivationTime.NoCost)
            .SetUsesFixed(1)
            .SetCostPerUse(0)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(Side.Ally, RangeType.Self, 1,
                    TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    new EffectFormBuilder()
                        .SetConditionForm(triggerCondition, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    new EffectFormBuilder()
                        .SetConditionForm(powerAttackCondition, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        PowersContext.PowersThatIgnoreInterruptions.Add(powerAttackPower);

        var PowerTurnOffPowerAttackPower = FeatureDefinitionPowerBuilder
            .Create("PowerTurnOffPowerAttack", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent(true)
            .SetActivationTime(ActivationTime.NoCost)
            .SetUsesFixed(1)
            .SetCostPerUse(0)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(Side.Ally, RangeType.Self, 1,
                    TargetType.Self)
                .SetDurationData(DurationType.Round, 0, false)
                .SetEffectForms(
                    new EffectFormBuilder()
                        .SetConditionForm(triggerCondition, ConditionForm.ConditionOperation.Remove)
                        .Build(),
                    new EffectFormBuilder()
                        .SetConditionForm(powerAttackCondition, ConditionForm.ConditionOperation.Remove)
                        .Build()
                )
                .Build())
            .AddToDB();

        PowersContext.PowersThatIgnoreInterruptions.Add(PowerTurnOffPowerAttackPower);
        concentrationProvider.StopPower = PowerTurnOffPowerAttackPower;

        return FeatDefinitionBuilder
            .Create("FeatPowerAttack", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerAttackPower,
                PowerTurnOffPowerAttackPower
            )
            .AddToDB();
    }

    private sealed class PowerAttackOneHandedAttackModifierBuilder : FeatureDefinitionBuilder
    {
        private const string PowerAttackAttackModifierName = "PowerAttackAttackModifier";
        private const string PowerAttackAttackModifierNameGuid = "87286627-3e62-459d-8781-ceac1c3462e6";

        public static readonly FeatureDefinition PowerAttackAttackModifier
            = CreateAndAddToDB(PowerAttackAttackModifierName, PowerAttackAttackModifierNameGuid);

        private PowerAttackOneHandedAttackModifierBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&PowerAttackTitle";
            Definition.GuiPresentation.Description = "Feature/&PowerAttackDescription";

            Definition.SetCustomSubFeatures(new ModifyPowerAttackPower());
        }

        private static FeatureDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PowerAttackOneHandedAttackModifierBuilder(name, guid).AddToDB();
        }
    }

    public sealed class StopPowerConcentrationProvider : ICustomConcentrationProvider
    {
        public FeatureDefinitionPower StopPower;

        public StopPowerConcentrationProvider(string name, string tooltip, AssetReferenceSprite icon)
        {
            Name = name;
            Tooltip = tooltip;
            Icon = icon;
        }

        public string Name { get; }
        public string Tooltip { get; }
        public AssetReferenceSprite Icon { get; }

        public void Stop(RulesetCharacter character)
        {
            if (StopPower == null)
            {
                return;
            }

            var rules = ServiceRepository.GetService<IRulesetImplementationService>();
            var usable = UsablePowersProvider.Get(StopPower, character);
            var locationCharacter = GameLocationCharacter.GetFromActor(character);
            var actionParams = new CharacterActionParams(locationCharacter,
                ActionDefinitions.Id.PowerNoCost)
            {
                SkipAnimationsAndVFX = true,
                TargetCharacters = { locationCharacter },
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = rules.InstantiateEffectPower(character, usable, true)
            };

            ServiceRepository.GetService<ICommandService>()
                .ExecuteAction(actionParams, _ => { }, false);
        }
    }

    private sealed class ModifyPowerAttackPower : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (!ValidatorsWeapon.IsMelee(attackMode))
            {
                return;
            }

            var proficiency = character.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            const int TO_HIT = -3;
            var toDamage = 3 + proficiency;

            attackMode.ToHitBonus += TO_HIT;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(TO_HIT,
                FeatureSourceType.Power, "PowerAttack", null));

            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(new TrendInfo(toDamage,
                FeatureSourceType.Power, "PowerAttack", null));
        }
    }
}
