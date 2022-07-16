using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static FeatureDefinitionAttributeModifier;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer;

internal static class FeatureHelpers
{
    public static FeatureDefinitionProficiencyBuilder BuildProficiency(string name,
        RuleDefinitions.ProficiencyType type, params string[] proficiencies)
    {
        return FeatureDefinitionProficiencyBuilder
            .Create(name, TinkererClass.GuidNamespace)
            .SetProficiencies(type, proficiencies);
    }

    public static FeatureDefinitionAttributeModifier BuildAttributeModifier(string name,
        AttributeModifierOperation modifierType, string attribute, int amount, GuiPresentation guiPresentation)
    {
        return new FeatureDefinitionAttributeModifierBuilder(name, TinkererClass.GuidNamespace,
            modifierType, attribute, amount, guiPresentation).AddToDB();
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinityHeightenedList(IEnumerable<string> spellNames,
        int levelBonus, string name, GuiPresentation guiPresentation)
    {
        return new FeatureDefinitionMagicAffinityBuilder(name, TinkererClass.GuidNamespace, levelBonus,
            guiPresentation, spellNames).AddToDB();
    }

    public static ConditionDefinition BuildCondition(string name, RuleDefinitions.DurationType durationType,
        int durationParameter, bool silent, GuiPresentation guiPresentation,
        params FeatureDefinition[] conditionFeatures)
    {
        return ConditionDefinitionBuilder
            .Create(name, TinkererClass.GuidNamespace)
            .SetGuiPresentation(guiPresentation)
            .Configure(durationType, durationParameter, silent, conditionFeatures)
            .AddToDB();
    }

    public static FeatureDefinitionMagicAffinity BuildMagicAffinityModifiers(string name, int attackModifier,
        int dcModifier, GuiPresentation guiPresentation)
    {
        return FeatureDefinitionMagicAffinityBuilder
            .Create(name, TinkererClass.GuidNamespace)
            .SetCastingModifiers(
                attackModifier, RuleDefinitions.SpellParamsModifierType.FlatValue,
                dcModifier, RuleDefinitions.SpellParamsModifierType.FlatValue,
                false, false, false)
            .AddToDB();
    }

    public static FeatureDefinitionPowerBuilder BuildSpellFormPower(string name, int usesPerRecharge,
        RuleDefinitions.UsesDetermination usesDetermination, RuleDefinitions.ActivationTime activationTime,
        int costPerUse, RuleDefinitions.RechargeRate recharge)
    {
        var effectDescriptionBuilder = new EffectDescriptionBuilder();
        effectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Self, 0, 0, 0,
            0);
        effectDescriptionBuilder.SetCreatedByCharacter();

        var effectFormBuilder = new EffectFormBuilder();
        effectFormBuilder.SetSpellForm(9);
        effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());
        effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None);

        var particleParams = new EffectParticleParameters();
        particleParams.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.EffectDescription
            .EffectParticleParameters);
        effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

        return new FeatureDefinitionPowerBuilder(name, TinkererClass.GuidNamespace,
            usesPerRecharge, usesDetermination, AttributeDefinitions.Intelligence, activationTime, costPerUse,
            recharge, false, false, AttributeDefinitions.Intelligence,
            effectDescriptionBuilder.Build());
    }

    public static RestActivityDefinitionBuilder BuildRestActivity(string name, RestDefinitions.RestStage restStage,
        RuleDefinitions.RestType restType,
        RestActivityDefinition.ActivityCondition condition, string functor, string stringParameter)
    {
        return new RestActivityDefinitionBuilder(name, TinkererClass.GuidNamespace, restStage, restType, condition,
            functor, stringParameter);
    }

    public static FeatureDefinitionAttackModifier BuildAttackModifier(string name,
        RuleDefinitions.AttackModifierMethod attackRollModifierMethod, int attackRollModifier,
        string attackRollAbilityScore,
        RuleDefinitions.AttackModifierMethod damageRollModifierMethod, int damageRollModifier,
        string damageRollAbilityScore, bool canAddAbilityBonusToSecondary,
        string additionalAttackTag, GuiPresentation guiPresentation)
    {
        return FeatureDefinitionAttackModifierBuilder.Create(name, TinkererClass.GuidNamespace)
            .SetGuiPresentation(guiPresentation)
            .Configure(
                attackRollModifierMethod, attackRollModifier, attackRollAbilityScore, damageRollModifierMethod,
                damageRollModifier, damageRollAbilityScore, canAddAbilityBonusToSecondary, additionalAttackTag)
            .AddToDB();
    }

    public static FeatureDefinitionMovementAffinity BuildMovementAffinity(string name, bool addBase, int speedAdd,
        float speedMult, GuiPresentation guiPresentation)
    {
        return new FeatureDefinitionMovementAffinityBuilder(name, TinkererClass.GuidNamespace,
            addBase, speedAdd, speedMult, guiPresentation).AddToDB();
    }

    public static FeatureDefinitionHealingModifier BuildHealingModifier(string name, int healingBonusDiceNumber,
        RuleDefinitions.DieType healingBonusDiceType, RuleDefinitions.LevelSourceType addLevel,
        GuiPresentation guiPresentation)
    {
        return new FeatureDefinitionHealingModifierBuilder(name, TinkererClass.GuidNamespace,
            healingBonusDiceNumber, healingBonusDiceType, addLevel, guiPresentation).AddToDB();
    }

    public static FeatureDefinitionSavingThrowAffinity BuildSavingThrowAffinity(string name,
        IEnumerable<string> abilityScores,
        RuleDefinitions.CharacterSavingThrowAffinity affinityType,
        FeatureDefinitionSavingThrowAffinity.ModifierType modifierType, int diceNumber,
        RuleDefinitions.DieType dieType, bool againstMagic, GuiPresentation guiPresentation)
    {
        return new FeatureDefinitionSavingThrowAffinityBuilder(name, TinkererClass.GuidNamespace,
                abilityScores, affinityType, modifierType, diceNumber, dieType, againstMagic, guiPresentation)
            .AddToDB();
    }

    public static FeatureDefinitionAbilityCheckAffinity BuildAbilityAffinity(string name,
        IEnumerable<Tuple<string, string>> abilityProficiencyPairs, int diceNumber, RuleDefinitions.DieType dieType,
        RuleDefinitions.CharacterAbilityCheckAffinity affinityType, GuiPresentation guiPresentation)
    {
        return new FeatureDefinitionAbilityCheckAffinityBuilder(name, TinkererClass.GuidNamespace,
            abilityProficiencyPairs, diceNumber, dieType, affinityType, guiPresentation).AddToDB();
    }

    // TODO Most of theese builders should likely get moved/merged with the CE builders.
    public sealed class FeatureDefinitionPowerBuilder : Builders.Features.FeatureDefinitionPowerBuilder
    {
        public FeatureDefinitionPowerBuilder(string name, Guid guidNamespace, int usesPerRecharge,
            RuleDefinitions.UsesDetermination usesDetermination,
            string usesAbilityScoreName, RuleDefinitions.ActivationTime activationTime, int costPerUse,
            RuleDefinitions.RechargeRate recharge,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
            EffectDescription effectDescription) : base(name, guidNamespace)
        {
            Configure(usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse,
                recharge, proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription);
        }

        public FeatureDefinitionPowerBuilder(string name, Guid guidNamespace, int usesPerRecharge,
            RuleDefinitions.UsesDetermination usesDetermination,
            string usesAbilityScoreName, RuleDefinitions.ActivationTime activationTime, int costPerUse,
            RuleDefinitions.RechargeRate recharge,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
            EffectDescription effectDescription, FeatureDefinitionPower overridenPower) :
            this(name, guidNamespace, usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime,
                costPerUse, recharge,
                proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription)
        {
            Definition.overriddenPower = overridenPower;
        }
    }

    private sealed class
        FeatureDefinitionAttributeModifierBuilder : Builders.Features.FeatureDefinitionAttributeModifierBuilder
    {
        public FeatureDefinitionAttributeModifierBuilder(string name, Guid guidNamespace,
            AttributeModifierOperation modifierType,
            string attribute, int amount, GuiPresentation guiPresentation) : base(name, guidNamespace)
        {
            Definition.modifierType2 = modifierType;
            Definition.modifiedAttribute = attribute;
            Definition.modifierValue = amount;
            Definition.guiPresentation = guiPresentation;
        }
    }

    public sealed class FeatureDefinitionMagicAffinityBuilder : Builders.Features.FeatureDefinitionMagicAffinityBuilder
    {
        public FeatureDefinitionMagicAffinityBuilder(string name, Guid guidNamespace, int levelBonus,
            GuiPresentation guiPresentation, params string[] spellNames) :
            this(name, guidNamespace, levelBonus, guiPresentation, spellNames.AsEnumerable())
        {
        }

        public FeatureDefinitionMagicAffinityBuilder(string name, Guid guidNamespace, int levelBonus,
            GuiPresentation guiPresentation, IEnumerable<string> spellNames) : base(name, guidNamespace)
        {
            Definition.usesWarList = true;
            Definition.warListSlotBonus = levelBonus;
            Definition.WarListSpells.AddRange(spellNames);
            Definition.guiPresentation = guiPresentation;
        }

        public FeatureDefinitionMagicAffinityBuilder(string name, Guid guidNamespace, int attackModifier,
            int dcModifier, GuiPresentation guiPresentation) : base(name, guidNamespace)
        {
            Definition.spellAttackModifier = attackModifier;
            Definition.saveDCModifier = dcModifier;
            Definition.guiPresentation = guiPresentation;
        }

        public FeatureDefinitionMagicAffinityBuilder(string name, Guid guidNamesapce,
            GuiPresentation guiPresentation) : base(name, guidNamesapce)
        {
            Definition.somaticWithWeaponOrShield = true;
            Definition.guiPresentation = guiPresentation;
        }
    }

    private sealed class
        FeatureDefinitionSavingThrowAffinityBuilder : Builders.Features.FeatureDefinitionSavingThrowAffinityBuilder
    {
        public FeatureDefinitionSavingThrowAffinityBuilder(string name, Guid guidNamespace,
            IEnumerable<string> abilityScores,
            RuleDefinitions.CharacterSavingThrowAffinity affinityType,
            FeatureDefinitionSavingThrowAffinity.ModifierType modifierType, int diceNumber,
            RuleDefinitions.DieType dieType,
            bool againstMagic, GuiPresentation guiPresentation) : base(name, guidNamespace)
        {
            foreach (var ability in abilityScores)
            {
                var group = new FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup
                {
                    savingThrowModifierType = modifierType,
                    savingThrowModifierDiceNumber = diceNumber,
                    savingThrowModifierDieType = dieType,
                    abilityScoreName = ability,
                    affinity = affinityType
                };
                if (againstMagic)
                {
                    group.restrictedSchools.AddRange(
                        SchoolAbjuration.Name,
                        SchoolConjuration.Name,
                        SchoolDivination.Name,
                        SchoolEnchantment.Name,
                        SchoolEvocation.Name,
                        SchoolIllusion.Name,
                        SchoolNecromancy.Name,
                        SchoolTransmutation.Name);
                }

                Definition.AffinityGroups.Add(group);
            }

            Definition.guiPresentation = guiPresentation;
        }
    }

    private sealed class
        FeatureDefinitionAbilityCheckAffinityBuilder : Builders.Features.FeatureDefinitionAbilityCheckAffinityBuilder
    {
        public FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid guidNameapce,
            IEnumerable<Tuple<string, string>> abilityProficiencyPairs,
            int diceNumber, RuleDefinitions.DieType dieType,
            RuleDefinitions.CharacterAbilityCheckAffinity affinityType,
            GuiPresentation guiPresentation) : base(name, guidNameapce)
        {
            foreach (var abilityProficiency in abilityProficiencyPairs)
            {
                var group = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
                {
                    abilityScoreName = abilityProficiency.Item1
                };
                if (!string.IsNullOrEmpty(abilityProficiency.Item2))
                {
                    group.proficiencyName = abilityProficiency.Item2;
                }

                group.affinity = affinityType;
                group.abilityCheckModifierDiceNumber = diceNumber;
                group.abilityCheckModifierDieType = dieType;
                Definition.AffinityGroups.Add(group);
            }

            Definition.guiPresentation = guiPresentation;
        }
    }

    public sealed class
        FeatureDefinitionCraftingAffinityBuilder : Builders.Features.FeatureDefinitionCraftingAffinityBuilder
    {
        public FeatureDefinitionCraftingAffinityBuilder(string name, Guid guidNamespace,
            IEnumerable<ToolTypeDefinition> toolTypes,
            float durationMultiplier, bool doubleProficiencyBonus, GuiPresentation guiPresentation) : base(name,
            guidNamespace)
        {
            foreach (var tool in toolTypes)
            {
                var group = new FeatureDefinitionCraftingAffinity.CraftingAffinityGroup
                {
                    tooltype = tool,
                    durationMultiplier = durationMultiplier,
                    doubleProficiencyBonus = doubleProficiencyBonus
                };
                Definition.AffinityGroups.Add(group);
            }

            Definition.guiPresentation = guiPresentation;
        }
    }

    public sealed class RestActivityDefinitionBuilder : Builders.RestActivityDefinitionBuilder
    {
        public RestActivityDefinitionBuilder(string name, Guid guidNamespace, RestDefinitions.RestStage restStage,
            RuleDefinitions.RestType restType,
            RestActivityDefinition.ActivityCondition condition, string functor, string stringParameter) : base(name,
            guidNamespace)
        {
            Definition.restStage = restStage;
            Definition.restType = restType;
            Definition.condition = condition;
            Definition.functor = functor;
            Definition.stringParameter = stringParameter;
        }
    }

    private sealed class
        FeatureDefinitionMovementAffinityBuilder : Builders.Features.FeatureDefinitionMovementAffinityBuilder
    {
        public FeatureDefinitionMovementAffinityBuilder(string name, Guid guidNamespace, bool addBase,
            int speedAdd, float speedMult, GuiPresentation guiPresentation) : base(name, guidNamespace)
        {
            Definition.appliesToAllModes = true;
            Definition.baseSpeedMultiplicativeModifier = speedMult;
            Definition.baseSpeedAdditiveModifier = speedAdd;
            Definition.speedAddBase = addBase;
            Definition.guiPresentation = guiPresentation;
        }
    }

    private sealed class
        FeatureDefinitionHealingModifierBuilder : Builders.Features.FeatureDefinitionHealingModifierBuilder
    {
        public FeatureDefinitionHealingModifierBuilder(string name, Guid guidNamespace, int healingBonusDiceNumber,
            RuleDefinitions.DieType healingBonusDiceType,
            RuleDefinitions.LevelSourceType addLevel, GuiPresentation guiPresentation) : base(name, guidNamespace)
        {
            Definition.healingBonusDiceNumber = healingBonusDiceNumber;
            Definition.healingBonusDiceType = healingBonusDiceType;
            Definition.addLevel = addLevel;
            Definition.guiPresentation = guiPresentation;
        }
    }

    public sealed class
        FeatureDefinitionAdditionalDamageBuilder : Builders.Features.FeatureDefinitionAdditionalDamageBuilder
    {
        public FeatureDefinitionAdditionalDamageBuilder(string name, Guid guidNamesapce, string notificationTag,
            RuleDefinitions.FeatureLimitedUsage limitedUsage,
            RuleDefinitions.AdditionalDamageValueDetermination damageValueDetermination,
            RuleDefinitions.AdditionalDamageTriggerCondition triggerCondition,
            RuleDefinitions.AdditionalDamageRequiredProperty requiredProperty,
            bool attackModeOnly, RuleDefinitions.DieType damageDieType, int damageDiceNumber,
            RuleDefinitions.AdditionalDamageType additionalDamageType,
            string specificDamageType, RuleDefinitions.AdditionalDamageAdvancement damageAdvancement,
            IEnumerable<DiceByRank> diceByRankTable,
            bool hasSavingThrow, string savingThrowAbility, int savingThrowDc,
            RuleDefinitions.EffectSavingThrowType damageSaveAffinity,
            IEnumerable<ConditionOperationDescription> conditionOperations,
            GuiPresentation guiPresentation) : base(name, guidNamesapce)
        {
            Definition.notificationTag = notificationTag;
            Definition.limitedUsage = limitedUsage;
            Definition.damageValueDetermination = damageValueDetermination;
            Definition.triggerCondition = triggerCondition;
            Definition.requiredProperty = requiredProperty;
            Definition.attackModeOnly = attackModeOnly;
            Definition.damageDieType = damageDieType;
            Definition.damageDiceNumber = damageDiceNumber;
            Definition.additionalDamageType = additionalDamageType;
            Definition.specificDamageType = specificDamageType;
            Definition.damageAdvancement = damageAdvancement;
            Definition.DiceByRankTable.SetRange(diceByRankTable);
            Definition.damageDieType = damageDieType;
            Definition.hasSavingThrow = hasSavingThrow;
            Definition.savingThrowAbility = savingThrowAbility;
            Definition.savingThrowDC = savingThrowDc;
            Definition.damageSaveAffinity = damageSaveAffinity;
            Definition.ConditionOperations.SetRange(conditionOperations);
            Definition.guiPresentation = guiPresentation;
        }
    }

    public class FeatureDefinitionFeatureSetBuilder : Builders.Features.FeatureDefinitionFeatureSetBuilder
    {
        public FeatureDefinitionFeatureSetBuilder(string name, Guid guidNamespace,
            IEnumerable<FeatureDefinition> featureSet,
            FeatureDefinitionFeatureSet.FeatureSetMode mode, int defaultSelection, bool uniqueChoices,
            bool enumerateInDescription, GuiPresentation guiPresentation) : base(name, guidNamespace)
        {
            Definition.FeatureSet.SetRange(featureSet);
            Definition.mode = mode;
            Definition.defaultSelection = defaultSelection;
            Definition.uniqueChoices = uniqueChoices;
            Definition.enumerateInDescription = enumerateInDescription;
            Definition.guiPresentation = guiPresentation;
        }
    }
}
