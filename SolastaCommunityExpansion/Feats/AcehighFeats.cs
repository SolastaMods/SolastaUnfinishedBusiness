using System.Collections.Generic;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.CustomUI;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;
using static FeatureDefinitionSavingThrowAffinity;

namespace SolastaCommunityExpansion.Feats;

internal static class AcehighFeats
{
    public static void CreateFeats(List<FeatDefinition> feats)
    {
        feats.Add(SharpShooterFeatBuilder.SharpShooterFeat);
        feats.Add(PowerAttackFeatBuilder.PowerAttackFeat);
        feats.Add(RecklessFuryFeatBuilder.RecklessFuryFeat);
    }

    private static FeatureDefinition BuildSharpShooterPower()
    {
        return FeatureDefinitionPowerBuilder
            .Create("SharpShooter", "aa2cc094-0bf9-4e72-ac2c-50e99e680ca1")
            .SetGuiPresentation("SharpShooterFeat", Category.Feat,
                CustomIcons.CreateAssetReferenceSprite("SharpShooterIcon",
                    Resources.PowerAttackIcon, 128, 64))
            .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
            .SetUsesFixed(1)
            .SetCostPerUse(0)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                    RuleDefinitions.TargetType.Self)
                .SetDurationData(RuleDefinitions.DurationType.Permanent)
                .SetEffectForms(new EffectFormBuilder()
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create("SharpShooterTriggerCondition", DefinitionBuilder.CENamespaceGuid)
                            .SetGuiPresentationNoContent(true)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .SetDuration(RuleDefinitions.DurationType.Permanent)
                            .SetFeatures(FeatureDefinitionBuilder
                                .Create("SharpShooterTriggerFeature", DefinitionBuilder.CENamespaceGuid)
                                .SetGuiPresentationNoContent(true)
                                .SetCustomSubFeatures(new SharpShooterConcentrationProvider())
                                .AddToDB())
                            .AddToDB(), ConditionForm.ConditionOperation.Add)
                        .Build(),
                    new EffectFormBuilder()
                        .SetConditionForm(SharpShooterConditionBuilder.SharpShooterCondition,
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();
    }

    internal sealed class SharpShooterAttackModifierBuilder : FeatureDefinitionBuilder
    {
        private const string SharpShooterAttackModifierName = "SharpShooterAttackModifier";
        private const string SharpShooterAttackModifierNameGuid = "473f6ab6-af46-4717-b55e-ff9e31d909e2";

        public static readonly FeatureDefinition SharpShooterAttackModifier
            = CreateAndAddToDB(SharpShooterAttackModifierName, SharpShooterAttackModifierNameGuid);

        private SharpShooterAttackModifierBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&SharpShooterTitle";
            Definition.GuiPresentation.Description = "Feature/&SharpShooterDescription";

            Definition.SetCustomSubFeatures(new ModifySharpShooterAttackPower());
        }

        private static FeatureDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SharpShooterAttackModifierBuilder(name, guid).AddToDB();
        }
    }

    internal sealed class SharpShooterConditionBuilder : ConditionDefinitionBuilder
    {
        private const string SharpShooterConditionName = "SharpShooterCondition";
        private const string SharpShooterConditionNameGuid = "a0d24e21-3469-43af-ad63-729552120314";

        public static readonly ConditionDefinition SharpShooterCondition =
            CreateAndAddToDB(SharpShooterConditionName, SharpShooterConditionNameGuid);

        private SharpShooterConditionBuilder(string name, string guid) : base(
            DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&SharpShooterTitle";
            Definition.GuiPresentation.Description = "Feature/&SharpShooterDescription";

            Definition.SetAllowMultipleInstances(false);
            Definition.Features.Clear();
            Definition.Features.Add(SharpShooterAttackModifierBuilder.SharpShooterAttackModifier);

            Definition.SetDurationType(RuleDefinitions.DurationType.Round);
            Definition.SetDurationParameter(0);
            Definition.SetCancellingConditions(Definition);
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SharpShooterConditionBuilder(name, guid).AddToDB();
        }
    }

    internal sealed class SharpShooterFeatBuilder : FeatDefinitionBuilder
    {
        private const string SharpShooterFeatName = "SharpShooterFeat";
        private const string SharpShooterFeatNameGuid = "d2ca939a-465e-4e43-8e9b-6469177e1839";

        public static readonly FeatDefinition SharpShooterFeat =
            CreateAndAddToDB(SharpShooterFeatName, SharpShooterFeatNameGuid);

        private SharpShooterFeatBuilder(string name, string guid) : base(
            DatabaseHelper.FeatDefinitions.FollowUpStrike, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SharpShooterFeatTitle";
            Definition.GuiPresentation.Description = "Feat/&SharpShooterFeatDescription";

            Definition.Features.Clear();
            Definition.Features.Add(BuildSharpShooterPower());
            Definition.SetMinimalAbilityScorePrerequisite(false);
        }

        private static FeatDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SharpShooterFeatBuilder(name, guid).AddToDB();
        }
    }

    public class SharpShooterConcentrationProvider : ICusomConcentrationProvider
    {
        private static AssetReferenceSprite _icon;
        public string Name => "SharpShooter";
        public string Tooltip => "Tooltip/&SharpShooterConcentration";

        public AssetReferenceSprite Icon => _icon ??=
            CustomIcons.CreateAssetReferenceSprite("SharpShooterConcentrationIcon",
                Resources.PowerAttackConcentrationIcon, 64, 64);

        public void Stop(RulesetCharacter character)
        {
            var triggerCondition = "SharpShooterTriggerCondition";
            var attackCondition = SharpShooterConditionBuilder.SharpShooterCondition.Name;
            var toRemove = new List<RulesetCondition>();

            foreach (var pair in character.ConditionsByCategory)
            {
                foreach (var rulesetCondition in pair.Value)
                {
                    if (rulesetCondition.Name == triggerCondition || rulesetCondition.Name == attackCondition)
                    {
                        toRemove.Add(rulesetCondition);
                    }
                }
            }

            foreach (var rulesetCondition in toRemove)
            {
                character.RemoveCondition(rulesetCondition);
            }

            character.AddConditionOfCategory(AttributeDefinitions.TagEffect, RulesetCondition.CreateActiveCondition(
                character.Guid,
                SharpShooterConditionBuilder.SharpShooterCondition, RuleDefinitions.DurationType.Round,
                0,
                RuleDefinitions.TurnOccurenceType.StartOfTurn,
                character.Guid,
                character.CurrentFaction.Name
            ));
        }
    }

    private class ModifySharpShooterAttackPower : IModifyAttackModeForWeapon
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

            if (attackMode is not { Reach: false, Ranged: true, Thrown: false })
            {
                return;
            }

            var proficiency = character.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            var toHit = -proficiency;
            var toDamage = proficiency * 2;

            attackMode.ToHitBonus += toHit;
            attackMode.ToHitBonusTrends.Add(new RuleDefinitions.TrendInfo(toHit,
                RuleDefinitions.FeatureSourceType.Power, "SharpShooter", null));

            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(new RuleDefinitions.TrendInfo(toDamage,
                RuleDefinitions.FeatureSourceType.Power, "SharpShooter", null));
        }
    }

    private static FeatureDefinition BuildPowerAttackPower()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerAttack", "0a3e6a7d-4628-4189-b91d-d7146d774bb6")
            .SetGuiPresentation("PowerAttackFeat", Category.Feat,
                CustomIcons.CreateAssetReferenceSprite("PowerAttackIcon",
                    Resources.PowerAttackIcon, 128, 64))
            .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
            .SetUsesFixed(1)
            .SetCostPerUse(0)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                    RuleDefinitions.TargetType.Self)
                .SetDurationData(RuleDefinitions.DurationType.Permanent)
                .SetEffectForms(new EffectFormBuilder()
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create("PowerAttackTriggerCondition", DefinitionBuilder.CENamespaceGuid)
                            .SetGuiPresentationNoContent(true)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .SetDuration(RuleDefinitions.DurationType.Permanent)
                            .SetFeatures(FeatureDefinitionBuilder
                                .Create("PowerAttackTriggerFeature", DefinitionBuilder.CENamespaceGuid)
                                .SetGuiPresentationNoContent(true)
                                .SetCustomSubFeatures(new PowerAttackConcentrationProvider())
                                .AddToDB())
                            .AddToDB(), ConditionForm.ConditionOperation.Add)
                        .Build(),
                    new EffectFormBuilder()
                        .SetConditionForm(PowerAttackConditionBuilder.PowerAttackCondition,
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();
    }

    internal sealed class PowerAttackTwoHandedPowerBuilder : FeatureDefinitionPowerBuilder
    {
        private const string PowerAttackTwoHandedPowerName = "PowerAttackTwoHanded";
        private const string PowerAttackTwoHandedPowerNameGuid = "b45b8467-7caa-428e-b4b5-ba3c4a153f07";

        public static readonly FeatureDefinitionPower PowerAttackTwoHandedPower =
            CreateAndAddToDB(PowerAttackTwoHandedPowerName, PowerAttackTwoHandedPowerNameGuid);

        private PowerAttackTwoHandedPowerBuilder(string name, string guid) : base(
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade, name, guid)
        {
            Definition.GuiPresentation.Title = Gui.NoLocalization;
            Definition.GuiPresentation.Description = Gui.NoLocalization;
            Definition.GuiPresentation.SetHidden(true);

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.NoCost);
            Definition.SetShortTitleOverride("Feature/&PowerAttackTitle");

            //Create the power attack effect
            var powerAttackEffect = new EffectForm
            {
                ConditionForm = new ConditionForm(), FormType = EffectForm.EffectFormType.Condition
            };
            powerAttackEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            powerAttackEffect.ConditionForm.ConditionDefinition =
                PowerAttackTwoHandedConditionBuilder.PowerAttackTwoHandedCondition;

            //Add to our new effect
            var newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(powerAttackEffect);
            newEffectDescription.HasSavingThrow = false;
            newEffectDescription.DurationType = RuleDefinitions.DurationType.Round;
            newEffectDescription.DurationParameter = 0;
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
            newEffectDescription.SetCanBePlacedOnCharacter(true);

            Definition.SetEffectDescription(newEffectDescription);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new PowerAttackTwoHandedPowerBuilder(name, guid).AddToDB();
        }
    }

    internal sealed class PowerAttackOneHandedAttackModifierBuilder : FeatureDefinitionBuilder
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

    internal sealed class PowerAttackTwoHandedAttackModifierBuilder : FeatureDefinitionAttackModifierBuilder
    {
        private const string PowerAttackTwoHandedAttackModifierName = "PowerAttackTwoHandedAttackModifier";
        private const string PowerAttackTwoHandedAttackModifierNameGuid = "b1b05940-7558-4f03-98d1-01f616b5ae25";

        public static readonly FeatureDefinitionAttackModifier PowerAttackTwoHandedAttackModifier =
            CreateAndAddToDB(PowerAttackTwoHandedAttackModifierName, PowerAttackTwoHandedAttackModifierNameGuid);

        private PowerAttackTwoHandedAttackModifierBuilder(string name, string guid) : base(
            DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierFightingStyleArchery, name, guid)
        {
            Definition.GuiPresentation.Title = Gui.NoLocalization;
            Definition.GuiPresentation.Description = Gui.NoLocalization;
            Definition.GuiPresentation.SetHidden(true);

            //Ideally this would be proficiency but there isn't a nice way to subtract proficiency.
            //To do this properly you could likely make multiple versions of this that get replaced at proficiency level ups but it's a bit of a pain, so going with -3 for now.
            //Originally I made an implemenation that used FeatureDefinitionAdditionalDamage and was going to restrict to melee weapons etc. but really power attack should be avaiable for any build as you choose.
            //The FeatureDefinitionAdditionalDamage was limited in the sense that you couldn't check for things like the 'TwoHanded' or 'Heavy' properties of a weapon so it wasn't worth using really.
            Definition.SetAttackRollModifier(-1);
            Definition.SetDamageRollModifier(2);
        }

        private static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
        {
            return new PowerAttackTwoHandedAttackModifierBuilder(name, guid).AddToDB();
        }
    }

    internal sealed class PowerAttackConditionBuilder : ConditionDefinitionBuilder
    {
        private const string PowerAttackConditionName = "PowerAttackCondition";
        private const string PowerAttackConditionNameGuid = "c125b7b9-e668-4c6f-a742-63c065ad2292";

        public static readonly ConditionDefinition PowerAttackCondition =
            CreateAndAddToDB(PowerAttackConditionName, PowerAttackConditionNameGuid);

        private PowerAttackConditionBuilder(string name, string guid) : base(
            DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&PowerAttackTitle";
            Definition.GuiPresentation.Description = "Feature/&PowerAttackDescription";

            Definition.SetAllowMultipleInstances(false);
            Definition.Features.Clear();
            Definition.Features.Add(PowerAttackOneHandedAttackModifierBuilder.PowerAttackAttackModifier);

            Definition.SetDurationType(RuleDefinitions.DurationType.Round);
            Definition.SetDurationParameter(0);
            Definition.SetCancellingConditions(Definition);
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PowerAttackConditionBuilder(name, guid).AddToDB();
        }
    }

    internal sealed class PowerAttackTwoHandedConditionBuilder : ConditionDefinitionBuilder
    {
        private const string PowerAttackTwoHandedConditionName = "PowerAttackTwoHandedCondition";
        private const string PowerAttackTwoHandedConditionNameGuid = "7d0eecbd-9ad8-4915-a3f7-cfa131001fe6";

        public static readonly ConditionDefinition PowerAttackTwoHandedCondition =
            CreateAndAddToDB(PowerAttackTwoHandedConditionName, PowerAttackTwoHandedConditionNameGuid);

        private PowerAttackTwoHandedConditionBuilder(string name, string guid) : base(
            DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = Gui.NoLocalization;
            Definition.GuiPresentation.Description = Gui.NoLocalization;
            Definition.GuiPresentation.SetHidden(true);

            Definition.SetAllowMultipleInstances(false);
            Definition.Features.Clear();
            Definition.Features.Add(PowerAttackTwoHandedAttackModifierBuilder.PowerAttackTwoHandedAttackModifier);

            Definition.SetDurationType(RuleDefinitions.DurationType.Round);
            Definition.SetDurationParameter(0);
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PowerAttackTwoHandedConditionBuilder(name, guid).AddToDB();
        }
    }

    internal sealed class PowerAttackFeatBuilder : FeatDefinitionBuilder
    {
        private const string PowerAttackFeatName = "PowerAttackFeat";
        private const string PowerAttackFeatNameGuid = "88f1fb27-66af-49c6-b038-a38142b1083e";

        public static readonly FeatDefinition PowerAttackFeat =
            CreateAndAddToDB(PowerAttackFeatName, PowerAttackFeatNameGuid);

        private PowerAttackFeatBuilder(string name, string guid) : base(
            DatabaseHelper.FeatDefinitions.FollowUpStrike, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&PowerAttackFeatTitle";
            Definition.GuiPresentation.Description = "Feat/&PowerAttackFeatDescription";

            Definition.Features.Clear();
            Definition.Features.Add(BuildPowerAttackPower());
            Definition.Features.Add(PowerAttackTwoHandedPowerBuilder.PowerAttackTwoHandedPower);
            Definition.SetMinimalAbilityScorePrerequisite(false);
        }

        private static FeatDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PowerAttackFeatBuilder(name, guid).AddToDB();
        }
    }

    public class PowerAttackConcentrationProvider : ICusomConcentrationProvider
    {
        private static AssetReferenceSprite _icon;
        public string Name => "PowerAttack";
        public string Tooltip => "Tooltip/&PowerAttackConcentration";

        public AssetReferenceSprite Icon => _icon ??=
            CustomIcons.CreateAssetReferenceSprite("PowerAttackConcentrationIcon",
                Resources.PowerAttackConcentrationIcon, 64, 64);

        public void Stop(RulesetCharacter character)
        {
            var triggerCondition = "PowerAttackTriggerCondition";
            var attackCondition = PowerAttackConditionBuilder.PowerAttackCondition.Name;
            var toRemove = new List<RulesetCondition>();
            foreach (var pair in character.ConditionsByCategory)
            {
                foreach (var rulesetCondition in pair.Value)
                {
                    if (rulesetCondition.Name == triggerCondition || rulesetCondition.Name == attackCondition)
                    {
                        toRemove.Add(rulesetCondition);
                    }
                }
            }

            foreach (var rulesetCondition in toRemove)
            {
                character.RemoveCondition(rulesetCondition);
            }

            character.AddConditionOfCategory(AttributeDefinitions.TagEffect, RulesetCondition.CreateActiveCondition(
                character.Guid,
                PowerAttackConditionBuilder.PowerAttackCondition, RuleDefinitions.DurationType.Round,
                0,
                RuleDefinitions.TurnOccurenceType.StartOfTurn,
                character.Guid,
                character.CurrentFaction.Name
            ));
        }
    }

    private class ModifyPowerAttackPower : IModifyAttackModeForWeapon
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

            if (attackMode is not {Reach: true, Ranged: false, Thrown: false})
            {
                return;
            }

            var proficiency = character.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            var toHit = -proficiency;
            var toDamage = proficiency;

            if (attackMode.UseVersatileDamage
                || IsTwoHanded(attackMode.SourceDefinition as ItemDefinition)
                || IsTwoHanded(weapon?.ItemDefinition))
            {
                toDamage *= 2;
            }

            attackMode.ToHitBonus += toHit;
            attackMode.ToHitBonusTrends.Add(new RuleDefinitions.TrendInfo(toHit,
                RuleDefinitions.FeatureSourceType.Power, "PowerAttack", null));

            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(new RuleDefinitions.TrendInfo(toDamage,
                RuleDefinitions.FeatureSourceType.Power, "PowerAttack", null));
        }

        private static bool IsTwoHanded(ItemDefinition weapon)
        {
            if (weapon == null)
            {
                return false;
            }

            var description = weapon.WeaponDescription;
            if (description == null)
            {
                return false;
            }

            return description.WeaponTags.Contains(TagsDefinitions.WeaponTagTwoHanded);
        }
    }

    internal sealed class RecklessFuryFeatBuilder : FeatDefinitionBuilder
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
            Definition.SetMinimalAbilityScorePrerequisite(false);
        }

        private static FeatDefinition CreateAndAddToDB(string name, string guid)
        {
            return new RecklessFuryFeatBuilder(name, guid).AddToDB();
        }
    }

    internal sealed class RagePowerBuilder : FeatureDefinitionPowerBuilder
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

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
            Definition.SetCostPerUse(1);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetShortTitleOverride("Feature/&RagePowerTitle");

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

            Definition.SetEffectDescription(newEffectDescription);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new RagePowerBuilder(name, guid).AddToDB();
        }
    }

    internal sealed class RageFeatConditionBuilder : ConditionDefinitionBuilder
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

            Definition.SetAllowMultipleInstances(false);
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
            Definition.SetDurationType(RuleDefinitions.DurationType.Minute);
            Definition.SetDurationParameter(1);
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new RageFeatConditionBuilder(name, guid).AddToDB();
        }
    }

    internal sealed class RageStrengthSavingThrowAffinityBuilder : FeatureDefinitionSavingThrowAffinityBuilder
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
                    affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage, abilityScoreName = "Strength"
                });
        }

        private static FeatureDefinitionSavingThrowAffinity CreateAndAddToDB(string name, string guid)
        {
            return new RageStrengthSavingThrowAffinityBuilder(name, guid).AddToDB();
        }
    }

    internal sealed class RageDamageBonusAttackModifierBuilder : FeatureDefinitionAttackModifierBuilder
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
            Definition.SetAttackRollModifier(0);
            Definition.SetDamageRollModifier(
                2); //Could find a way to up this at level 9 to match barb but that seems like a lot of work right now :)
        }

        private static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
        {
            return new RageDamageBonusAttackModifierBuilder(name, guid).AddToDB();
        }
    }
}
