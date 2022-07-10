using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
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

namespace SolastaCommunityExpansion.Feats;

internal static class AcehighFeats
{
    public static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        feats.Add(DeadeyeFeatBuilder.DeadeyeFeat);
        feats.Add(BuildPowerAttackFeat());
    }

    private static FeatDefinition BuildPowerAttackFeat()
    {
        var concentrationProvider = new StopPowerConcentrationProvider("PowerAttack",
            "Tooltip/&PowerAttackConcentration", CustomIcons.CreateAssetReferenceSprite("PowerAttackConcentrationIcon",
                Resources.PowerAttackConcentrationIcon, 64, 64));

        var triggerCondition = ConditionDefinitionBuilder
            .Create("PowerAttackTriggerCondition", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(RuleDefinitions.DurationType.Permanent)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("PowerAttackTriggerFeature", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(concentrationProvider)
                .AddToDB())
            .AddToDB();

        var powerAttackCondition = ConditionDefinitionBuilder
            .Create("PowerAttackCondition", "c125b7b9-e668-4c6f-a742-63c065ad2292")
            .SetGuiPresentation("PowerAttack", Category.Feature,
                DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle.GuiPresentation.SpriteReference)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAllowMultipleInstances(false)
            .SetFeatures(PowerAttackOneHandedAttackModifierBuilder.PowerAttackAttackModifier)
            .SetDuration(RuleDefinitions.DurationType.Round, 1)
            .AddToDB();

        var powerAttackPower = FeatureDefinitionPowerBuilder
            .Create("PowerAttack", "0a3e6a7d-4628-4189-b91d-d7146d774bb6")
            .SetGuiPresentation("PowerAttackFeat", Category.Feat,
                CustomIcons.CreateAssetReferenceSprite("PowerAttackIcon", Resources.PowerAttackIcon, 128, 64))
            .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
            .SetUsesFixed(1)
            .SetCostPerUse(0)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                    RuleDefinitions.TargetType.Self)
                .SetDurationData(RuleDefinitions.DurationType.Permanent)
                .SetEffectForms(
                    new EffectFormBuilder()
                        .SetConditionForm(triggerCondition, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    new EffectFormBuilder()
                        .SetConditionForm(powerAttackCondition, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        var turnOffPowerAttackPower = FeatureDefinitionPowerBuilder
            // Reusing old two-handed power id - we need to keep this id anyway, so old characters won't crash
            .Create("PowerAttackTwoHanded", "b45b8467-7caa-428e-b4b5-ba3c4a153f07")
            .SetGuiPresentationNoContent(true)
            .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
            .SetUsesFixed(1)
            .SetCostPerUse(0)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                    RuleDefinitions.TargetType.Self)
                .SetDurationData(RuleDefinitions.DurationType.Round, 0, false)
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

        concentrationProvider.StopPower = turnOffPowerAttackPower;

        return FeatDefinitionBuilder
            .Create("PowerAttackFeat", "88f1fb27-66af-49c6-b038-a38142b1083e")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerAttackPower,
                turnOffPowerAttackPower
            )
            .AddToDB();
    }

    private sealed class DeadeyeIgnoreDefenderBuilder : FeatureDefinitionCombatAffinityBuilder
    {
        private const string DeadeyeIgnoreDefenderName = "DeadeyeIgnoreDefender";
        private const string DeadeyeIgnoreDefenderGuid = "38940e1f-fc62-4a1a-aebe-b4cb7064050d";

        public static readonly FeatureDefinition DeadeyeIgnoreDefender
            = CreateAndAddToDB(DeadeyeIgnoreDefenderName, DeadeyeIgnoreDefenderGuid);

        private DeadeyeIgnoreDefenderBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&DeadeyeTitle";
            Definition.GuiPresentation.Description = "Feature/&DeadeyeDescription";

            Definition.ignoreCover = true;
            Definition.SetCustomSubFeatures(new BumpWeaponAttackRangeToMax(WeaponValidators.AlwaysValid));
        }

        private static FeatureDefinition CreateAndAddToDB(string name, string guid)
        {
            return new DeadeyeIgnoreDefenderBuilder(name, guid).AddToDB();
        }
    }

    private sealed class DeadeyeAttackModifierBuilder : FeatureDefinitionBuilder
    {
        private const string DeadeyeAttackModifierName = "DeadeyeAttackModifier";
        private const string DeadeyeAttackModifierGuid = "473f6ab6-af46-4717-b55e-ff9e31d909e2";

        public static readonly FeatureDefinition DeadeyeAttackModifier
            = CreateAndAddToDB(DeadeyeAttackModifierName, DeadeyeAttackModifierGuid);

        private DeadeyeAttackModifierBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&DeadeyeTitle";
            Definition.GuiPresentation.Description = "Feature/&DeadeyeDescription";

            Definition.SetCustomSubFeatures(new ModifyDeadeyeAttackPower());
        }

        private static FeatureDefinition CreateAndAddToDB(string name, string guid)
        {
            return new DeadeyeAttackModifierBuilder(name, guid).AddToDB();
        }
    }

    private sealed class DeadeyeConditionBuilder : ConditionDefinitionBuilder
    {
        private const string DeadeyeConditionName = "DeadeyeCondition";
        private const string DeadeyeConditionNameGuid = "a0d24e21-3469-43af-ad63-729552120314";

        public static readonly ConditionDefinition DeadeyeCondition =
            CreateAndAddToDB(DeadeyeConditionName, DeadeyeConditionNameGuid);

        private DeadeyeConditionBuilder(string name, string guid) : base(
            DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&DeadeyeTitle";
            Definition.GuiPresentation.Description = "Feature/&DeadeyeDescription";

            Definition.allowMultipleInstances = false;
            Definition.Features.Clear();
            Definition.Features.Add(DeadeyeAttackModifierBuilder.DeadeyeAttackModifier);

            Definition.durationType = RuleDefinitions.DurationType.Round;
            Definition.durationParameter = 0;
            Definition.CancellingConditions.Clear();
            Definition.CancellingConditions.Add(Definition);
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new DeadeyeConditionBuilder(name, guid).AddToDB();
        }
    }

    private sealed class DeadeyeFeatBuilder : FeatDefinitionBuilder
    {
        private const string DeadeyeFeatName = "DeadeyeFeat";
        private const string DeadeyeFeatNameGuid = "d2ca939a-465e-4e43-8e9b-6469177e1839";

        public static readonly FeatDefinition DeadeyeFeat =
            CreateAndAddToDB(DeadeyeFeatName, DeadeyeFeatNameGuid);

        private DeadeyeFeatBuilder(string name, string guid) : base(
            DatabaseHelper.FeatDefinitions.FollowUpStrike, name, guid)
        {
            var concentrationProvider = new StopPowerConcentrationProvider("Deadeye", "Tooltip/&DeadeyeConcentration",
                CustomIcons.CreateAssetReferenceSprite("DeadeyeConcentrationIcon",
                    Resources.DeadeyeConcentrationIcon, 64, 64));

            var triggerCondition = ConditionDefinitionBuilder
                .Create("DeadeyeTriggerCondition", CENamespaceGuid)
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetDuration(RuleDefinitions.DurationType.Permanent)
                .SetFeatures(
                    FeatureDefinitionBuilder
                        .Create("DeadeyeTriggerFeature", CENamespaceGuid)
                        .SetGuiPresentationNoContent(true)
                        .SetCustomSubFeatures(concentrationProvider)
                        .AddToDB())
                .AddToDB();

            var turnOnPower = FeatureDefinitionPowerBuilder
                .Create("Deadeye", "aa2cc094-0bf9-4e72-ac2c-50e99e680ca1")
                .SetGuiPresentation("DeadeyeFeat", Category.Feat,
                    CustomIcons.CreateAssetReferenceSprite("DeadeyeIcon",
                        Resources.DeadeyeIcon, 128, 64))
                .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
                .SetUsesFixed(1)
                .SetCostPerUse(0)
                .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                        RuleDefinitions.TargetType.Self)
                    .SetDurationData(RuleDefinitions.DurationType.Permanent)
                    .SetEffectForms(
                        new EffectFormBuilder()
                            .SetConditionForm(triggerCondition, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        new EffectFormBuilder()
                            .SetConditionForm(DeadeyeConditionBuilder.DeadeyeCondition,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
                .AddToDB();

            var turnOffPower = FeatureDefinitionPowerBuilder
                .Create("TurnOffDeadeyePower", CENamespaceGuid)
                .SetGuiPresentationNoContent(true)
                .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
                .SetUsesFixed(1)
                .SetCostPerUse(0)
                .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                        RuleDefinitions.TargetType.Self)
                    .SetDurationData(RuleDefinitions.DurationType.Round, 0, false)
                    .SetEffectForms(
                        new EffectFormBuilder()
                            .SetConditionForm(triggerCondition, ConditionForm.ConditionOperation.Remove)
                            .Build(),
                        new EffectFormBuilder()
                            .SetConditionForm(DeadeyeConditionBuilder.DeadeyeCondition,
                                ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
                .AddToDB();

            concentrationProvider.StopPower = turnOffPower;

            Definition.GuiPresentation.Title = "Feat/&DeadeyeFeatTitle";
            Definition.GuiPresentation.Description = "Feat/&DeadeyeFeatDescription";

            Definition.Features.Clear();
            Definition.Features.Add(turnOnPower);
            Definition.Features.Add(turnOffPower);
            Definition.Features.Add(DeadeyeIgnoreDefenderBuilder.DeadeyeIgnoreDefender);
            Definition.minimalAbilityScorePrerequisite = false;
        }

        private static FeatDefinition CreateAndAddToDB(string name, string guid)
        {
            return new DeadeyeFeatBuilder(name, guid).AddToDB();
        }
    }

    private sealed class ModifyDeadeyeAttackPower : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode,
            RulesetItem weapon)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (attackMode is not ( {Reach: false, Ranged: true}))
            {
                return;
            }

            const int TO_HIT = -5;
            const int TO_DAMAGE = 10;

            attackMode.ToHitBonus += TO_HIT;
            attackMode.ToHitBonusTrends.Add(new RuleDefinitions.TrendInfo(TO_HIT,
                RuleDefinitions.FeatureSourceType.Power, "Deadeye", null));

            damage.BonusDamage += TO_DAMAGE;
            damage.DamageBonusTrends.Add(new RuleDefinitions.TrendInfo(TO_DAMAGE,
                RuleDefinitions.FeatureSourceType.Power, "Deadeye", null));
        }
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

    private sealed class StopPowerConcentrationProvider : ICusomConcentrationProvider
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
                TargetCharacters = {locationCharacter},
                ActionModifiers = {new ActionModifier()},
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
            attackMode.ToHitBonusTrends.Add(new RuleDefinitions.TrendInfo(TO_HIT,
                RuleDefinitions.FeatureSourceType.Power, "PowerAttack", null));

            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(new RuleDefinitions.TrendInfo(toDamage,
                RuleDefinitions.FeatureSourceType.Power, "PowerAttack", null));
        }
    }
}
