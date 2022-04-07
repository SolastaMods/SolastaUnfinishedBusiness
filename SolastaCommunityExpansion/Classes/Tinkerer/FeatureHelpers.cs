using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static FeatureDefinitionAttributeModifier;
using static SolastaModApi.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    internal static class FeatureHelpers
    {
        // TODO Most of theese builders should likely get moved/merged with the CE builders.
        public class FeatureDefinitionPowerBuilder : Builders.Features.FeatureDefinitionPowerBuilder
        {
            public FeatureDefinitionPowerBuilder(string name, Guid guidNamespace, int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
                string usesAbilityScoreName, RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
                bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
                EffectDescription effectDescription) : base(name, guidNamespace)
            {
                Configure(usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse,
                    recharge, proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription);
            }

            public FeatureDefinitionPowerBuilder(string name, Guid guidNamespace, int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
                string usesAbilityScoreName, RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
                bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
                EffectDescription effectDescription, FeatureDefinitionPower overridenPower) :
                this(name, guidNamespace, usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse, recharge,
                    proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription)
            {
                Definition.SetOverriddenPower(overridenPower);
            }
        }

        public class FeatureDefinitionAttackModifierBuilder : Builders.Features.FeatureDefinitionAttackModifierBuilder
        {
            public FeatureDefinitionAttackModifierBuilder(string name, Guid guidNamespace, RuleDefinitions.AttackModifierMethod attackRollModifierMethod,
                int attackRollModifier, string attackRollAbilityScore, RuleDefinitions.AttackModifierMethod damageRollModifierMethod,
                int damageRollModifier, string damageRollAbilityScore, bool canAddAbilityBonusToSecondary, string additionalAttackTag,
                GuiPresentation guiPresentation) : base(name, guidNamespace)
            {
                Configure(attackRollModifierMethod, attackRollModifier, attackRollAbilityScore, damageRollModifierMethod,
                    damageRollModifier, damageRollAbilityScore, canAddAbilityBonusToSecondary, additionalAttackTag);

                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionAttributeModifierBuilder : Builders.Features.FeatureDefinitionAttributeModifierBuilder
        {
            public FeatureDefinitionAttributeModifierBuilder(string name, Guid guidNamespace, AttributeModifierOperation modifierType,
                string attribute, int amount, GuiPresentation guiPresentation) : base(name, guidNamespace)
            {
                Definition.SetModifierType2(modifierType);
                Definition.SetModifiedAttribute(attribute);
                Definition.SetModifierValue(amount);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionMagicAffinityBuilder : Builders.Features.FeatureDefinitionMagicAffinityBuilder
        {
            public FeatureDefinitionMagicAffinityBuilder(string name, Guid guidNamespace, int levelBonus,
                GuiPresentation guiPresentation, params string[] spellNames) :
                this(name, guidNamespace, levelBonus, guiPresentation, spellNames.AsEnumerable())
            {
            }

            public FeatureDefinitionMagicAffinityBuilder(string name, Guid guidNamespace, int levelBonus,
                GuiPresentation guiPresentation, IEnumerable<string> spellNames) : base(name, guidNamespace)
            {
                Definition.SetUsesWarList(true);
                Definition.SetWarListSlotBonus(levelBonus);
                Definition.WarListSpells.AddRange(spellNames);
                Definition.SetGuiPresentation(guiPresentation);
            }

            public FeatureDefinitionMagicAffinityBuilder(string name, Guid guidNamespace, int attackModifier,
                int dcModifier, GuiPresentation guiPresentation) : base(name, guidNamespace)
            {
                Definition.SetSpellAttackModifier(attackModifier);
                Definition.SetSaveDCModifier(dcModifier);
                Definition.SetGuiPresentation(guiPresentation);
            }

            public FeatureDefinitionMagicAffinityBuilder(string name, Guid guidNamesapce, GuiPresentation guiPresentation) : base(name, guidNamesapce)
            {
                Definition.SetSomaticWithWeaponOrShield(true);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class ConditionDefinitionBuilder : Builders.ConditionDefinitionBuilder
        {
            public ConditionDefinitionBuilder(string name, Guid guidNamespace, RuleDefinitions.DurationType durationType, int durationParameter,
                bool silent, GuiPresentation guiPresentation, params FeatureDefinition[] conditionFeatures) : base(name, guidNamespace)
            {
                Configure(durationType, durationParameter, silent, conditionFeatures);

                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionSavingThrowAffinityBuilder : Builders.Features.FeatureDefinitionSavingThrowAffinityBuilder
        {
            public FeatureDefinitionSavingThrowAffinityBuilder(string name, Guid guidNamespace, IEnumerable<string> abilityScores,
                RuleDefinitions.CharacterSavingThrowAffinity affinityType,
                FeatureDefinitionSavingThrowAffinity.ModifierType modifierType, int diceNumber, RuleDefinitions.DieType dieType,
                bool againstMagic, GuiPresentation guiPresentation) : base(name, guidNamespace)
            {
                foreach (string ability in abilityScores)
                {
                    FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup group = new FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup
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
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionAbilityCheckAffinityBuilder : Builders.Features.FeatureDefinitionAbilityCheckAffinityBuilder
        {
            public FeatureDefinitionAbilityCheckAffinityBuilder(string name, Guid guidNameapce, IEnumerable<Tuple<string, string>> abilityProficiencyPairs,
                int diceNumber, RuleDefinitions.DieType dieType, RuleDefinitions.CharacterAbilityCheckAffinity affinityType,
                GuiPresentation guiPresentation) : base(name, guidNameapce)
            {
                foreach (Tuple<string, string> abilityProficiency in abilityProficiencyPairs)
                {
                    FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup group = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
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
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionCraftingAffinityBuilder : Builders.Features.FeatureDefinitionCraftingAffinityBuilder
        {
            public FeatureDefinitionCraftingAffinityBuilder(string name, Guid guidNamespace, IEnumerable<ToolTypeDefinition> toolTypes,
                float durationMultiplier, bool doubleProficiencyBonus, GuiPresentation guiPresentation) : base(name, guidNamespace)
            {
                foreach (ToolTypeDefinition tool in toolTypes)
                {
                    FeatureDefinitionCraftingAffinity.CraftingAffinityGroup group = new FeatureDefinitionCraftingAffinity.CraftingAffinityGroup
                    {
                        tooltype = tool,
                        durationMultiplier = durationMultiplier,
                        doubleProficiencyBonus = doubleProficiencyBonus
                    };
                    Definition.AffinityGroups.Add(group);
                }
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class RestActivityDefinitionBuilder : Builders.RestActivityDefinitionBuilder
        {
            public RestActivityDefinitionBuilder(string name, Guid guidNamespace, RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType,
                RestActivityDefinition.ActivityCondition condition, string functor, string stringParameter) : base(name, guidNamespace)
            {
                Definition.SetRestStage(restStage);
                Definition.SetRestType(restType);
                Definition.SetCondition(condition);
                Definition.SetFunctor(functor);
                Definition.SetStringParameter(stringParameter);
            }
        }

        public class FeatureDefinitionMovementAffinityBuilder : Builders.Features.FeatureDefinitionMovementAffinityBuilder
        {
            public FeatureDefinitionMovementAffinityBuilder(string name, Guid guidNamespace, bool addBase,
                int speedAdd, float speedMult, GuiPresentation guiPresentation) : base(name, guidNamespace)
            {
                Definition.SetAppliesToAllModes(true);
                Definition.SetBaseSpeedMultiplicativeModifier(speedMult);
                Definition.SetBaseSpeedAdditiveModifier(speedAdd);
                Definition.SetSpeedAddBase(addBase);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionHealingModifierBuilder : Builders.Features.FeatureDefinitionHealingModifierBuilder
        {
            public FeatureDefinitionHealingModifierBuilder(string name, Guid guidNamespace, int healingBonusDiceNumber, RuleDefinitions.DieType healingBonusDiceType,
            RuleDefinitions.LevelSourceType addLevel, GuiPresentation guiPresentation) : base(name, guidNamespace)
            {
                Definition.SetHealingBonusDiceNumber(healingBonusDiceNumber);
                Definition.SetHealingBonusDiceType(healingBonusDiceType);
                Definition.SetAddLevel(addLevel);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionAdditionalDamageBuilder : Builders.Features.FeatureDefinitionAdditionalDamageBuilder
        {
            public FeatureDefinitionAdditionalDamageBuilder(string name, Guid guidNamesapce, string notificationTag, RuleDefinitions.FeatureLimitedUsage limitedUsage,
                RuleDefinitions.AdditionalDamageValueDetermination damageValueDetermination,
                RuleDefinitions.AdditionalDamageTriggerCondition triggerCondition, RuleDefinitions.AdditionalDamageRequiredProperty requiredProperty,
                bool attackModeOnly, RuleDefinitions.DieType damageDieType, int damageDiceNumber, RuleDefinitions.AdditionalDamageType additionalDamageType,
                string specificDamageType, RuleDefinitions.AdditionalDamageAdvancement damageAdvancement, IEnumerable<DiceByRank> diceByRankTable,
                bool hasSavingThrow, string savingThrowAbility, int savingThrowDC, RuleDefinitions.EffectSavingThrowType damageSaveAffinity,
                IEnumerable<ConditionOperationDescription> conditionOperations,
            GuiPresentation guiPresentation) : base(name, guidNamesapce)
            {
                Definition.SetNotificationTag(notificationTag);
                Definition.SetLimitedUsage(limitedUsage);
                Definition.SetDamageValueDetermination(damageValueDetermination);
                Definition.SetTriggerCondition(triggerCondition);
                Definition.SetRequiredProperty(requiredProperty);
                Definition.SetAttackModeOnly(attackModeOnly);
                Definition.SetDamageDieType(damageDieType);
                Definition.SetDamageDiceNumber(damageDiceNumber);
                Definition.SetAdditionalDamageType(additionalDamageType);
                Definition.SetSpecificDamageType(specificDamageType);
                Definition.SetDamageAdvancement(damageAdvancement);
                Definition.DiceByRankTable.SetRange(diceByRankTable);
                Definition.SetDamageDieType(damageDieType);
                Definition.SetHasSavingThrow(hasSavingThrow);
                Definition.SetSavingThrowAbility(savingThrowAbility);
                Definition.SetSavingThrowDC(savingThrowDC);
                Definition.SetDamageSaveAffinity(damageSaveAffinity);
                Definition.ConditionOperations.SetRange(conditionOperations);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public static SpellListDefinition.SpellsByLevelDuplet BuildSpellList(int classLevel, params SpellDefinition[] spellnames)
        {
            return BuildSpellList(classLevel, spellnames.AsEnumerable());
        }

        public static SpellListDefinition.SpellsByLevelDuplet BuildSpellList(int classLevel, IEnumerable<SpellDefinition> spellnames)
        {
            return new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = classLevel,
                Spells = new List<SpellDefinition>(spellnames)
            };
        }

        public static FeatureDefinitionProficiencyBuilder BuildProficiency(string name,
            RuleDefinitions.ProficiencyType type, params string[] proficiencies)
        {
            return BuildProficiency(name, type, proficiencies.AsEnumerable());
        }

        public static FeatureDefinitionProficiencyBuilder BuildProficiency(string name,
            RuleDefinitions.ProficiencyType type, IEnumerable<string> proficiencies)
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

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityHeightenedList(IEnumerable<string> spellNames, int levelBonus, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionMagicAffinityBuilder(name, TinkererClass.GuidNamespace, levelBonus, guiPresentation, spellNames).AddToDB();
        }

        public static ConditionDefinition BuildCondition(string name, RuleDefinitions.DurationType durationType,
            int durationParameter, bool silent, GuiPresentation guiPresentation, params FeatureDefinition[] conditionFeatures)
        {
            return new ConditionDefinitionBuilder(name, TinkererClass.GuidNamespace,
                durationType, durationParameter, silent, guiPresentation, conditionFeatures).AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityModifiers(string name, int attackModifier, int dcModifier, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionMagicAffinityBuilder(name, TinkererClass.GuidNamespace,
                attackModifier, dcModifier, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionPowerBuilder BuildSpellFormPower(string name, int usesPerRecharge,
            RuleDefinitions.UsesDetermination usesDetermination, RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge)
        {
            EffectDescriptionBuilder effectDescriptionBuilder = new EffectDescriptionBuilder();
            effectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Self, 0, 0, 0, 0, ActionDefinitions.ItemSelectionType.None);
            effectDescriptionBuilder.SetCreatedByCharacter();

            EffectFormBuilder effectFormBuilder = new EffectFormBuilder();
            effectFormBuilder.SetSpellForm(9);
            effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());
            effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1, 0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectParticleParameters particleParams = new EffectParticleParameters();
            particleParams.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.EffectDescription.EffectParticleParameters);
            effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

            return new FeatureDefinitionPowerBuilder(name, TinkererClass.GuidNamespace,
                usesPerRecharge, usesDetermination, AttributeDefinitions.Intelligence, activationTime, costPerUse, recharge, false, false, AttributeDefinitions.Intelligence,
                effectDescriptionBuilder.Build());
        }

        public static RestActivityDefinitionBuilder BuildRestActivity(string name, RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType,
            RestActivityDefinition.ActivityCondition condition, string functor, string stringParameter)
        {
            return new RestActivityDefinitionBuilder(name, TinkererClass.GuidNamespace, restStage, restType, condition, functor, stringParameter);
        }

        public static FeatureDefinitionAttackModifier BuildAttackModifier(string name,
            RuleDefinitions.AttackModifierMethod attackRollModifierMethod, int attackRollModifier, string attackRollAbilityScore,
            RuleDefinitions.AttackModifierMethod damageRollModifierMethod, int damageRollModifier, string damageRollAbilityScore, bool canAddAbilityBonusToSecondary,
            string additionalAttackTag, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionAttackModifierBuilder(name, TinkererClass.GuidNamespace,
                attackRollModifierMethod, attackRollModifier, attackRollAbilityScore, damageRollModifierMethod, damageRollModifier, damageRollAbilityScore,
                canAddAbilityBonusToSecondary, additionalAttackTag, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionMovementAffinity BuildMovementAffinity(string name, bool addBase, int speedAdd, float speedMult, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionMovementAffinityBuilder(name, TinkererClass.GuidNamespace,
                addBase, speedAdd, speedMult, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionHealingModifier BuildHealingModifier(string name, int healingBonusDiceNumber,
            RuleDefinitions.DieType healingBonusDiceType, RuleDefinitions.LevelSourceType addLevel, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionHealingModifierBuilder(name, TinkererClass.GuidNamespace,
                healingBonusDiceNumber, healingBonusDiceType, addLevel, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionSavingThrowAffinity BuildSavingThrowAffinity(string name,
            IEnumerable<string> abilityScores,
            RuleDefinitions.CharacterSavingThrowAffinity affinityType, FeatureDefinitionSavingThrowAffinity.ModifierType modifierType, int diceNumber,
            RuleDefinitions.DieType dieType, bool againstMagic, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionSavingThrowAffinityBuilder(name, TinkererClass.GuidNamespace,
                abilityScores, affinityType, modifierType, diceNumber, dieType, againstMagic, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionAbilityCheckAffinity BuildAbilityAffinity(string name,
            IEnumerable<Tuple<string, string>> abilityProficiencyPairs, int diceNumber, RuleDefinitions.DieType dieType,
            RuleDefinitions.CharacterAbilityCheckAffinity affinityType, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionAbilityCheckAffinityBuilder(name, TinkererClass.GuidNamespace,
                abilityProficiencyPairs, diceNumber, dieType, affinityType, guiPresentation).AddToDB();
        }

        public class FeatureDefinitionFeatureSetBuilder : Builders.Features.FeatureDefinitionFeatureSetBuilder
        {
            public FeatureDefinitionFeatureSetBuilder(string name, Guid guidNamespace, IEnumerable<FeatureDefinition> featureSet,
                FeatureDefinitionFeatureSet.FeatureSetMode mode, int defaultSelection, bool uniqueChoices,
                bool enumerateInDescription, GuiPresentation guiPresentation) : base(name, guidNamespace)
            {
                Definition.FeatureSet.SetRange(featureSet);
                Definition.SetMode(mode);
                Definition.SetDefaultSelection(defaultSelection);
                Definition.SetUniqueChoices(uniqueChoices);
                Definition.SetEnumerateInDescription(enumerateInDescription);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }
    }
}
