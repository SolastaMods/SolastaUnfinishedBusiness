using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.CustomUI;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Feats;

public static class EwFeats
{
    public const string SentinelFeat = "FeatSentinel";
    private const string PolearmExpertFeat = "FeatPolearmExpert";
    private const string RangedExpertFeat = "FeatRangedExpert";
    private const string RecklessAttackFeat = "FeatRecklessAttack";
    private static readonly Guid Guid = new("B4ED480F-2D06-4EB1-8732-9A721D80DD1A");

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
            .Create("ConditionSentinelStopMovement", Guid)
            .SetGuiPresentation(Category.Condition, Gui.NoLocalization, restrained.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained
            )
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(SentinelFeat, Guid)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionOnAttackHitEffectBuilder
                .Create("OnAttackHitEffectFeatSentinel", Guid)
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
            .Create(PolearmExpertFeat, Guid)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("PolearmFeatFeature", Guid)
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new CanMakeAoOOnReachEntered(CharacterValidators.HasPolearm),
                    new AddPolearmFollowupAttack()
                )
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildRangedExpert()
    {
        return FeatDefinitionBuilder
            .Create(RangedExpertFeat, Guid)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatRangedExpertFeature", Guid)
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new RangedAttackInMeleeDisadvantageRemover(),
                    new AddExtraRangedAttack(IsOneHandedRanged, ActionDefinitions.ActionType.Bonus,
                        CharacterValidators.HasAttacked)
                )
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildRecklessAttack()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create(RecklessAttackFeat, Guid)
            .SetGuiPresentation("RecklessAttack", Category.Action)
            .SetFeatures(FeatureDefinitionActionAffinitys.ActionAffinityBarbarianRecklessAttack)
            .SetValidators(FeatsValidators.ValidateNotClass(CharacterClassDefinitions.Barbarian))
            .AddToDB();
    }

    private static bool IsOneHandedRanged(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
    {
        return WeaponValidators.IsRanged(weapon) && WeaponValidators.IsOneHanded(weapon);
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

        var turnOffPowerAttackPower = FeatureDefinitionPowerBuilder
            // Reusing old two-handed power id - we need to keep this id anyway, so old characters won't crash
            .Create("PowerAttackTwoHanded", "b45b8467-7caa-428e-b4b5-ba3c4a153f07")
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

        PowersContext.PowersThatIgnoreInterruptions.Add(turnOffPowerAttackPower);
        concentrationProvider.StopPower = turnOffPowerAttackPower;

        return FeatDefinitionBuilder
            .Create("FeatPowerAttack", "88f1fb27-66af-49c6-b038-a38142b1083e")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerAttackPower,
                turnOffPowerAttackPower
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

    public sealed class StopPowerConcentrationProvider : ICusomConcentrationProvider
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
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode,
            RulesetItem weapon)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (!WeaponValidators.IsMelee(weapon))
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
