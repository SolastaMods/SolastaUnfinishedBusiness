using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
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
            var lifeTransmuterAffinity = FeatureDefinitionMagicAffinityBuilder
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
            FeatureDefinitionPower transmuteForce = FeatureDefinitionPowerPoolBuilder
                .Create("AttributeModiferTransmuterHealingPool", SubclassNamespace)
                .Configure(2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, RuleDefinitions.RechargeRate.LongRest)
                .SetGuiPresentation("HealingPoolLifeTransmuterList", Category.Subclass)
                .AddToDB();

            // Make a power that grants darkvision
            ConditionDefinition superiorDarkvision = BuildCondition(RuleDefinitions.DurationType.UntilLongRest,
                    1, "ConditionPowerTransmuteDarkvision", FeatureDefinitionSenses.SenseSuperiorDarkvision)
                .SetGuiPresentation("PowerTransmuteDarkvision", Category.Subclass, ConditionDefinitions.ConditionDarkvision.GuiPresentation.SpriteReference)
                .AddToDB();

            FeatureDefinitionPowerSharedPool powerDarkvision = BuildActionTransmuteConditionPower(transmuteForce, RuleDefinitions.RechargeRate.LongRest,
                    RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                    RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.UntilLongRest, 1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, superiorDarkvision, "PowerTransmuteDarkvision")
                .SetGuiPresentation(Category.Subclass, FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference)
                .AddToDB();

            ConditionDefinition poisonResistance = BuildCondition(
                RuleDefinitions.DurationType.UntilLongRest,
                    1, "ConditionPowerTransmutePoison",
                    FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance,
                    FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance,
                    FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                    FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                    FeatureDefinitionDamageAffinitys.DamageAffinityThunderResistance,
                    FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                    FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance)
                .SetGuiPresentation("ConditionTransmutePoison", Category.Subclass, ConditionDefinitions.ConditionProtectedFromPoison.GuiPresentation.SpriteReference)
                .AddToDB();

            // Make a power that gives resistance to an elemental damage
            FeatureDefinitionPowerSharedPool powerPoison = BuildActionTransmuteConditionPower(
                    transmuteForce, RuleDefinitions.RechargeRate.LongRest,
                    RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                    RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.UntilLongRest, 1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, poisonResistance, "PowerTransmutePoison")
                .SetGuiPresentation("PowerTransmuteElemnetalResistance", Category.Subclass, FeatureDefinitionPowers.PowerDomainElementalFireBurst.GuiPresentation.SpriteReference)
                .AddToDB();

            // Make a power that gives proficiency to constitution saves
            ConditionDefinition constitutionProf = BuildCondition(RuleDefinitions.DurationType.UntilLongRest,
                    1, "ConditionPowerTransmuteConstitution", FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfArun)
                .SetGuiPresentation("ConditionTransmuteConstitutionSave", Category.Subclass, ConditionDefinitions.ConditionBearsEndurance.GuiPresentation.SpriteReference)
                .AddToDB();

            FeatureDefinitionPowerSharedPool powerConstitution = BuildActionTransmuteConditionPower(transmuteForce, RuleDefinitions.RechargeRate.LongRest,
                    RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                    RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.UntilLongRest, 1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, constitutionProf, "PowerTransmuteConstitution")
                .SetGuiPresentation(Category.Subclass, FeatureDefinitionPowers.PowerPaladinAuraOfCourage.GuiPresentation.SpriteReference)
                .AddToDB();

            FeatureDefinitionPowerPoolModifier transmuteForceExtra = FeatureDefinitionPowerPoolModifierBuilder
                .Create("AttributeModiferTransmuterHealingPoolExtra", SubclassNamespace)
                .Configure(2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, transmuteForce)
                .SetGuiPresentation("HealingPoolLifeTransmuterBonus", Category.Subclass)
                .AddToDB();

            FeatureDefinitionPowerSharedPool powerFly = BuildActionTransmuteConditionPower(transmuteForce, RuleDefinitions.RechargeRate.LongRest,
                    RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RangeType.Touch, 2,
                    RuleDefinitions.TargetType.Individuals, ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.UntilLongRest, 1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn, AttributeDefinitions.Intelligence, ConditionDefinitions.ConditionFlying, "PowerTransmuteFly")
                .SetGuiPresentation(Category.Subclass, SpellDefinitions.Fly.GuiPresentation.SpriteReference)
                .AddToDB();

            FeatureDefinitionPowerSharedPool powerHeal = FeatureDefinitionPowerSharedPoolBuilder
                .Create("PowerTransmuteHeal", SubclassNamespace)
                .Configure(transmuteForce, RuleDefinitions.RechargeRate.LongRest, RuleDefinitions.ActivationTime.BonusAction,
                    1, false, false, AttributeDefinitions.Intelligence,
                    SpellDefinitions.MassHealingWord.EffectDescription, false /* unique instance */)
                .SetGuiPresentation(Category.Subclass, SpellDefinitions.MassHealingWord.GuiPresentation.SpriteReference)
                .AddToDB();

            FeatureDefinitionPowerSharedPool powerRevive = FeatureDefinitionPowerSharedPoolBuilder
                .Create("PowerTransmuteRevive", SubclassNamespace)
                .Configure(transmuteForce, RuleDefinitions.RechargeRate.LongRest,
                    RuleDefinitions.ActivationTime.BonusAction, 1, false, false, AttributeDefinitions.Intelligence,
                    SpellDefinitions.Revivify.EffectDescription, false /* unique instance */)
                .SetGuiPresentation(Category.Subclass, SpellDefinitions.Revivify.GuiPresentation.SpriteReference)
                .AddToDB();

            FeatureDefinitionPowerPoolModifier transmuteForceExtraBonus = FeatureDefinitionPowerPoolModifierBuilder
                .Create("AttributeModiferTransmuterHealingPoolBonus", SubclassNamespace)
                .Configure(4, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, transmuteForce)
                .SetGuiPresentation("HealingPoolLifeTransmuterBonusExtra", Category.Subclass)
                .AddToDB();

            Subclass = CharacterSubclassDefinitionBuilder
                .Create("LifeTransmuter", SubclassNamespace)
                .SetGuiPresentation("TraditionLifeTransmuter", Category.Subclass, RoguishDarkweaver.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(lifeTransmuterAffinity, 2)
                .AddFeatureAtLevel(transmuteForce, 6)
                .AddFeatureAtLevel(powerDarkvision, 6)
                .AddFeatureAtLevel(powerPoison, 6)
                .AddFeatureAtLevel(powerConstitution, 6)
                .AddFeatureAtLevel(transmuteForceExtra, 10)
                .AddFeatureAtLevel(powerFly, 10)
                .AddFeatureAtLevel(powerHeal, 10)
                .AddFeatureAtLevel(powerRevive, 10)
                .AddFeatureAtLevel(transmuteForceExtraBonus, 14)
                .AddFeatureAtLevel(FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance, 14)
                .AddToDB();
        }

        private static ConditionDefinitionBuilder BuildCondition(RuleDefinitions.DurationType durationType, int durationParameter,
            string name, params FeatureDefinition[] conditionFeatures)
        {
            return ConditionDefinitionBuilder
                .Create(name, SubclassNamespace)
                .SetFeatures(conditionFeatures)
                .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                .SetAllowMultipleInstances(false)
                .SetDuration(durationType, durationParameter, false); // No validation due to existing configuration
        }

        private static FeatureDefinitionPowerSharedPoolBuilder BuildActionTransmuteConditionPower(FeatureDefinitionPower poolPower,
            RuleDefinitions.RechargeRate recharge, RuleDefinitions.ActivationTime activationTime, int costPerUse,
            RuleDefinitions.RangeType rangeType, int rangeParameter, RuleDefinitions.TargetType targetType,
            ActionDefinitions.ItemSelectionType itemSelectionType, RuleDefinitions.DurationType durationType, int durationParameter,
            RuleDefinitions.TurnOccurenceType endOfEffect, string abilityScore, ConditionDefinition condition,
            string name)
        {
            var effectForm = EffectFormBuilder
                .Create()
                .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, false, false)
                .CreatedByCharacter()
                .Build();

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, 1, 0, itemSelectionType)
                .SetCreatedByCharacter()
                .SetDurationData(durationType, durationParameter, endOfEffect)
                .AddEffectForm(effectForm)
                .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1, 0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None)
                .SetParticleEffectParameters(SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters.Copy())
                .Build();

            return FeatureDefinitionPowerSharedPoolBuilder
                .Create(name, SubclassNamespace)
                .Configure(poolPower, recharge, activationTime, costPerUse, false, false, abilityScore, effectDescription, false /* unique instance */);
        }
    }
}
