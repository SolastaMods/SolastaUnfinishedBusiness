using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard
{
    internal class LifeTransmuter : AbstractSubclass
    {
        private static Guid SubclassNamespace = new("81cdcf44-5f04-4aea-8232-b22a1c264065");
        private readonly CharacterSubclassDefinition Subclass;

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal LifeTransmuter()
        {
            var LifeTransmuterAffinity = FeatureDefinitionMagicAffinityBuilder
                .Create("MagicAffinityLifeTransmuterHeightened", SubclassNamespace)
                .SetGuiPresentation("MagicAffinityLifeTransmuterList", Category.Subclass)
                .SetWarList(2,
                    SpellDefinitions.FalseLife.Name, // necromancy
                    SpellDefinitions.MagicWeapon.Name, // transmutation
                    SpellDefinitions.Blindness.Name, // necromancy
                    SpellDefinitions.Fly.Name, // transmutation
                    SpellDefinitions.BestowCurse.Name, // necromancy
                    SpellDefinitions.VampiricTouch.Name, // necromancy
                    SpellDefinitions.Blight.Name, // necromancy
                    SpellDefinitions.CloudKill.Name) // conjuration)
                .AddToDB();

            // Add tranmsuter stone like abilities.
            GuiPresentationBuilder LifeTransmuterDivintyGui = new GuiPresentationBuilder(
                "Subclass/&HealingPoolLifeTransmuterListTitle",
                "Subclass/&HealingPoolLifeTransmuterListDescription");

            FeatureDefinitionPower TransmuteForce = new FeatureDefinitionPowerPoolBuilder("AttributeModiferTransmuterHealingPool",
                GuidHelper.Create(SubclassNamespace, "AttributeModiferTransmuterHealingPool").ToString(),
                2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, RuleDefinitions.RechargeRate.LongRest,
                LifeTransmuterDivintyGui.Build()).AddToDB();

            // Make a power that grants darkvision
            GuiPresentationBuilder TransmuteDarkvision = new GuiPresentationBuilder(
                "Subclass/&PowerTransmuteDarkvisionTitle",
                "Subclass/&PowerTransmuteDarkvisionDescription");
            TransmuteDarkvision.SetSpriteReference(FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference);
            GuiPresentationBuilder ConditionDarkvisionGui = new GuiPresentationBuilder(
                "Subclass/&PowerTransmuteDarkvisionTitle",
                "Subclass/&PowerTransmuteDarkvisionDescription");
            ConditionDarkvisionGui.SetSpriteReference(ConditionDefinitions.ConditionDarkvision.GuiPresentation.SpriteReference);
            ConditionDefinition superiorDarkvision = BuildCondition(new List<FeatureDefinition>() { FeatureDefinitionSenses.SenseSuperiorDarkvision },
                RuleDefinitions.DurationType.UntilLongRest, 1, "ConditionPowerTransmuteDarkvision", ConditionDarkvisionGui.Build());
            FeatureDefinitionPowerSharedPool PowerDarkvision = BuildActionTransmuteConditionPower(TransmuteForce, RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, superiorDarkvision, "PowerTransmuteDarkvision", TransmuteDarkvision.Build());

            // Make a power that gives resistance to an elemental damage
            GuiPresentationBuilder TransmutePoison = new GuiPresentationBuilder(
                "Subclass/&PowerTransmuteElemnetalResistanceTitle",
                "Subclass/&PowerTransmuteElementalResistanceDescription");
            TransmutePoison.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalFireBurst.GuiPresentation.SpriteReference);
            GuiPresentationBuilder ConditionPoisonResistance = new GuiPresentationBuilder(
                "Subclass/&ConditionTransmutePoisonTitle",
                "Subclass/&ConditionTransmutePoisonDescription");
            ConditionPoisonResistance.SetSpriteReference(ConditionDefinitions.ConditionProtectedFromPoison.GuiPresentation.SpriteReference);
            ConditionDefinition PoisonResistance = BuildCondition(new List<FeatureDefinition>() {
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityThunderResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance,
            },
                RuleDefinitions.DurationType.UntilLongRest, 1, "ConditionPowerTransmutePoison", ConditionPoisonResistance.Build());
            FeatureDefinitionPowerSharedPool PowerPoison = BuildActionTransmuteConditionPower(TransmuteForce, RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, PoisonResistance, "PowerTransmutePoison", TransmutePoison.Build());
            // Make a power that gives proficiency to constitution saves
            GuiPresentationBuilder TransmuteConstitution = new GuiPresentationBuilder(
                "Subclass/&PowerTransmuteConstitutionTitle",
                "Subclass/&PowerTransmuteConstitutionDescription");
            GuiPresentationBuilder ConstitutionSaveGui = new GuiPresentationBuilder(
                "Subclass/&ConditionTransmuteConstitutionSaveTitle",
                "Subclass/&ConditionTransmuteConstitutionSaveDescription");
            ConstitutionSaveGui.SetSpriteReference(ConditionDefinitions.ConditionBearsEndurance.GuiPresentation.SpriteReference);
            ConditionDefinition ConstitutionProf = BuildCondition(new List<FeatureDefinition>() { FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfArun },
                RuleDefinitions.DurationType.UntilLongRest, 1, "ConditionPowerTransmuteConstitution", ConstitutionSaveGui.Build());
            TransmuteConstitution.SetSpriteReference(FeatureDefinitionPowers.PowerPaladinAuraOfCourage.GuiPresentation.SpriteReference);
            FeatureDefinitionPowerSharedPool PowerConstitution = BuildActionTransmuteConditionPower(TransmuteForce, RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, ConstitutionProf, "PowerTransmuteConstitution", TransmuteConstitution.Build());

            GuiPresentationBuilder LifeTransmuterExtraPoolGui = new GuiPresentationBuilder(
                "Subclass/&HealingPoolLifeTransmuterBonusTitle",
                "Subclass/&HealingPoolLifeTransmuterBonusDescription");
            FeatureDefinitionPowerPoolModifier TransmuteForceExtra = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferTransmuterHealingPoolExtra",
                GuidHelper.Create(SubclassNamespace, "AttributeModiferTransmuterHealingPoolExtra").ToString(),
                2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, TransmuteForce, LifeTransmuterExtraPoolGui.Build()).AddToDB();

            GuiPresentationBuilder TransmuteFly = new GuiPresentationBuilder(
                "Subclass/&PowerTransmuteFlyTitle",
                "Subclass/&PowerTransmuteFlyDescription");
            TransmuteFly.SetSpriteReference(SpellDefinitions.Fly.GuiPresentation.SpriteReference);
            FeatureDefinitionPowerSharedPool PowerFly = BuildActionTransmuteConditionPower(TransmuteForce, RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, ConditionDefinitions.ConditionFlying, "PowerTransmuteFly", TransmuteFly.Build());

            GuiPresentationBuilder TransmuteHeal = new GuiPresentationBuilder(
                "Subclass/&PowerTransmuteHealTitle",
                "Subclass/&PowerTransmuteHealDescription");
            TransmuteHeal.SetSpriteReference(SpellDefinitions.MassHealingWord.GuiPresentation.SpriteReference);
            FeatureDefinitionPowerSharedPool PowerHeal = new FeatureDefinitionPowerSharedPoolBuilder("PowerTransmuteHeal", GuidHelper.Create(SubclassNamespace, "PowerTransmuteHeal").ToString(),
                TransmuteForce, RuleDefinitions.RechargeRate.LongRest, RuleDefinitions.ActivationTime.BonusAction, 1, false, false, AttributeDefinitions.Intelligence,
                SpellDefinitions.MassHealingWord.EffectDescription, TransmuteHeal.Build(), false /* unique instance */).AddToDB();

            GuiPresentationBuilder TransmuteRevive = new GuiPresentationBuilder(
                "Subclass/&PowerTransmuteReviveTitle",
                "Subclass/&PowerTransmuteReviveDescription");
            TransmuteRevive.SetSpriteReference(SpellDefinitions.Revivify.GuiPresentation.SpriteReference);
            FeatureDefinitionPowerSharedPool PowerRevive = new FeatureDefinitionPowerSharedPoolBuilder("PowerTransmuteRevive", GuidHelper.Create(SubclassNamespace, "PowerTransmuteRevive").ToString(),
                TransmuteForce, RuleDefinitions.RechargeRate.LongRest, RuleDefinitions.ActivationTime.BonusAction, 1, false, false, AttributeDefinitions.Intelligence,
                SpellDefinitions.Revivify.EffectDescription, TransmuteRevive.Build(), false /* unique instance */).AddToDB();

            GuiPresentationBuilder LifeTransmuterExtraPoolBonusGui = new GuiPresentationBuilder(
                "Subclass/&HealingPoolLifeTransmuterBonusExtraTitle",
                "Subclass/&HealingPoolLifeTransmuterBonusExtraDescription");
            FeatureDefinitionPowerPoolModifier TransmuteForceExtraBonus = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferTransmuterHealingPoolBonus",
                GuidHelper.Create(SubclassNamespace, "AttributeModiferTransmuterHealingPoolBonus").ToString(),
                4, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, TransmuteForce, LifeTransmuterExtraPoolBonusGui.Build()).AddToDB();

            Subclass = CharacterSubclassDefinitionBuilder
                .Create("LifeTransmuter", SubclassNamespace)
                .SetGuiPresentation("TraditionLifeTransmuter", Category.Subclass, RoguishDarkweaver.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(LifeTransmuterAffinity, 2)
                .AddFeatureAtLevel(TransmuteForce, 6)
                .AddFeatureAtLevel(PowerDarkvision, 6)
                .AddFeatureAtLevel(PowerPoison, 6)
                .AddFeatureAtLevel(PowerConstitution, 6)
                .AddFeatureAtLevel(TransmuteForceExtra, 10)
                .AddFeatureAtLevel(PowerFly, 10)
                .AddFeatureAtLevel(PowerHeal, 10)
                .AddFeatureAtLevel(PowerRevive, 10)
                .AddFeatureAtLevel(TransmuteForceExtraBonus, 14)
                .AddFeatureAtLevel(FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance, 14).AddToDB();
        }

        private static ConditionDefinition BuildCondition(IEnumerable<FeatureDefinition> conditionFeatures, RuleDefinitions.DurationType durationType,
            int durationParameter, string name, GuiPresentation guiPresentation)
        {
            return ConditionDefinitionBuilder
                .Create(name, SubclassNamespace)
                .SetGuiPresentation(guiPresentation)
                .SetFeatures(conditionFeatures)
                .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                .SetAllowMultipleInstances(false)
                .SetDuration(durationType, durationParameter, false) // No validation due to existing configuration
                .AddToDB();
        }

        private static FeatureDefinitionPowerSharedPool BuildActionTransmuteConditionPower(FeatureDefinitionPower poolPower,
            RuleDefinitions.RechargeRate recharge, RuleDefinitions.ActivationTime activationTime, int costPerUse,
            RuleDefinitions.RangeType rangeType, int rangeParameter, RuleDefinitions.TargetType targetType,
            ActionDefinitions.ItemSelectionType itemSelectionType, RuleDefinitions.DurationType durationType, int durationParameter,
            RuleDefinitions.TurnOccurenceType endOfEffect, string abilityScore, ConditionDefinition condition,
            string name, GuiPresentation guiPresentation)
        {
            EffectDescriptionBuilder effectDescriptionBuilder = new EffectDescriptionBuilder();
            effectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, 1, 0, itemSelectionType);
            effectDescriptionBuilder.SetCreatedByCharacter();
            effectDescriptionBuilder.SetDurationData(durationType, durationParameter, endOfEffect);

            EffectFormBuilder effectFormBuilder = new EffectFormBuilder();
            effectFormBuilder.SetConditionForm(condition, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>());

            effectFormBuilder.CreatedByCharacter();

            effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());
            effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectParticleParameters particleParams = new EffectParticleParameters();
            particleParams.Copy(SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters);
            effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

            FeatureDefinitionPowerSharedPoolBuilder builder = new FeatureDefinitionPowerSharedPoolBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(),
                poolPower, recharge, activationTime, costPerUse, false, false, abilityScore,
                effectDescriptionBuilder.Build(), guiPresentation, false /* unique instance */);

            return builder.AddToDB();
        }
    }
}
