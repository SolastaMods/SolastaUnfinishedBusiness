using System.Collections.Generic;
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
using static FeatureDefinitionSavingThrowAffinity;

namespace SolastaCommunityExpansion.Feats;

internal static class AcehighFeats
{
    public static void CreateFeats(List<FeatDefinition> feats)
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

        concentrationProvider.stopPower = turnOffPowerAttackPower;

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

            concentrationProvider.stopPower = turnOffPower;

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

    private class ModifyDeadeyeAttackPower : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
        {
            if (attackMode == null)
            {
                return;
            }

            var damage = attackMode.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (attackMode is not {Reach: false, Ranged: true, Thrown: false})
            {
                return;
            }

            var toHit = -5;
            var toDamage = 10;

            attackMode.ToHitBonus += toHit;
            attackMode.ToHitBonusTrends.Add(new RuleDefinitions.TrendInfo(toHit,
                RuleDefinitions.FeatureSourceType.Power, "Deadeye", null));

            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(new RuleDefinitions.TrendInfo(toDamage,
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
        public FeatureDefinitionPower stopPower;

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
            if (stopPower == null)
            {
                return;
            }

            var rules = ServiceRepository.GetService<IRulesetImplementationService>();
            var usable = UsablePowersProvider.Get(stopPower, character);
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
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
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

    // Leaving this to not break existing characters
    private sealed class RecklessFuryFeatBuilder : FeatDefinitionBuilder
    {
        private const string RecklessFuryFeatName = "RecklessFuryFeat";
        private const string RecklessFuryFeatNameGuid = "78c5fd76-e25b-499d-896f-3eaf84c711d8";

        public static readonly FeatDefinition RecklessFuryFeat =
            CreateAndAddToDB(RecklessFuryFeatName, RecklessFuryFeatNameGuid);

        private RecklessFuryFeatBuilder(string name, string guid) : base(
            DatabaseHelper.FeatDefinitions.FollowUpStrike, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&RecklessFuryFeatTitle";
            Definition.GuiPresentation.Description = "Feat/&RecklessFuryFeatDescription";

            Definition.Features.Clear();
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionPowers.PowerReckless);
            Definition.Features.Add(RagePowerBuilder.RagePower);
            Definition.minimalAbilityScorePrerequisite = false;
        }

        private static FeatDefinition CreateAndAddToDB(string name, string guid)
        {
            return new RecklessFuryFeatBuilder(name, guid).AddToDB();
        }
    }

    private sealed class RagePowerBuilder : FeatureDefinitionPowerBuilder
    {
        private const string RagePowerName = "AHRagePower";
        private const string RagePowerNameGuid = "a46c1722-7825-4a81-bca1-392b51cd7d97";

        public static readonly FeatureDefinitionPower
            RagePower = CreateAndAddToDB(RagePowerName, RagePowerNameGuid);

        private RagePowerBuilder(string name, string guid) : base(
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalFireBurst, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&RagePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&RagePowerDescription";

            Definition.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
            Definition.activationTime = RuleDefinitions.ActivationTime.BonusAction;
            Definition.costPerUse = 1;
            Definition.fixedUsesPerRecharge = 1;
            Definition.shortTitleOverride = "Feature/&RagePowerTitle";

            //Create the power attack effect
            var rageEffect = new EffectForm
            {
                ConditionForm = new ConditionForm
                {
                    Operation = ConditionForm.ConditionOperation.Add,
                    ConditionDefinition = RageFeatConditionBuilder.RageFeatCondition
                },
                FormType = EffectForm.EffectFormType.Condition
            };

            //Add to our new effect
            var newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(rageEffect);
            newEffectDescription.HasSavingThrow = false;
            newEffectDescription.DurationType = RuleDefinitions.DurationType.Minute;
            newEffectDescription.DurationParameter = 1;
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
            newEffectDescription.SetCanBePlacedOnCharacter(true);

            Definition.effectDescription = newEffectDescription;
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new RagePowerBuilder(name, guid).AddToDB();
        }
    }

    private sealed class RageFeatConditionBuilder : ConditionDefinitionBuilder
    {
        private const string RageFeatConditionName = "AHRageFeatCondition";
        private const string RageFeatConditionNameGuid = "2f34fb85-6a5d-4a4e-871b-026872bc24b8";

        public static readonly ConditionDefinition RageFeatCondition =
            CreateAndAddToDB(RageFeatConditionName, RageFeatConditionNameGuid);

        private RageFeatConditionBuilder(string name, string guid) : base(
            DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&RageFeatConditionTitle";
            Definition.GuiPresentation.Description = "Feature/&RageFeatConditionDescription";

            Definition.allowMultipleInstances = false;
            Definition.Features.Clear();
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys
                .DamageAffinityBludgeoningResistance);
            Definition.Features.Add(
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance);
            Definition.Features.Add(
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys
                .AbilityCheckAffinityConditionBullsStrength);
            Definition.Features.Add(RageStrengthSavingThrowAffinityBuilder.RageStrengthSavingThrowAffinity);
            Definition.Features.Add(RageDamageBonusAttackModifierBuilder.RageDamageBonusAttackModifier);
            Definition.durationType = RuleDefinitions.DurationType.Minute;
            Definition.durationParameter = 1;
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new RageFeatConditionBuilder(name, guid).AddToDB();
        }
    }

    private sealed class RageStrengthSavingThrowAffinityBuilder : FeatureDefinitionSavingThrowAffinityBuilder
    {
        private const string RageStrengthSavingThrowAffinityName = "AHRageStrengthSavingThrowAffinity";
        private const string RageStrengthSavingThrowAffinityNameGuid = "17d26173-7353-4087-a295-96e1ec2e6cd4";

        public static readonly FeatureDefinitionSavingThrowAffinity RageStrengthSavingThrowAffinity =
            CreateAndAddToDB(RageStrengthSavingThrowAffinityName, RageStrengthSavingThrowAffinityNameGuid);

        private RageStrengthSavingThrowAffinityBuilder(string name, string guid) : base(
            DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfArun, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&RageStrengthSavingThrowAffinityTitle";
            Definition.GuiPresentation.Description = "Feature/&RageStrengthSavingThrowAffinityDescription";

            Definition.AffinityGroups.Clear();

            Definition.AffinityGroups.Add(
                new SavingThrowAffinityGroup
                {
                    affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
                    abilityScoreName = AttributeDefinitions.Strength
                });
        }

        private static FeatureDefinitionSavingThrowAffinity CreateAndAddToDB(string name, string guid)
        {
            return new RageStrengthSavingThrowAffinityBuilder(name, guid).AddToDB();
        }
    }

    private sealed class RageDamageBonusAttackModifierBuilder : FeatureDefinitionAttackModifierBuilder
    {
        private const string RageDamageBonusAttackModifierName = "AHRageDamageBonusAttackModifier";
        private const string RageDamageBonusAttackModifierNameGuid = "7bc1a47e-9519-4a37-a89a-10bcfa83e48a";

        public static readonly FeatureDefinitionAttackModifier RageDamageBonusAttackModifier =
            CreateAndAddToDB(RageDamageBonusAttackModifierName, RageDamageBonusAttackModifierNameGuid);

        private RageDamageBonusAttackModifierBuilder(string name, string guid) : base(
            DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierFightingStyleArchery, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&RageDamageBonusAttackModifierTitle";
            Definition.GuiPresentation.Description = "Feature/&RageDamageBonusAttackModifierDescription";

            //Currently works with ranged weapons, in the end it's fine.
            Definition.attackRollModifier = 0;
            Definition.damageRollModifier =
                2; //Could find a way to up this at level 9 to match barb but that seems like a lot of work right now :)
        }

        private static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
        {
            return new RageDamageBonusAttackModifierBuilder(name, guid).AddToDB();
        }
    }
}
