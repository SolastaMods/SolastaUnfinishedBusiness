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
            public FeatureDefinitionPowerBuilder(string name, string guid, int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
                string usesAbilityScoreName, RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
                bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
                EffectDescription effectDescription, GuiPresentation guiPresentation) : base(name, guid)
            {
                Configure(usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse,
                    recharge, proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription);

                Definition.SetGuiPresentation(guiPresentation);
            }

            public FeatureDefinitionPowerBuilder(string name, string guid, int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
                string usesAbilityScoreName, RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
                bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
                EffectDescription effectDescription, GuiPresentation guiPresentation, FeatureDefinitionPower overridenPower) :
                this(name, guid, usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse, recharge,
                    proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription, guiPresentation)
            {
                Definition.SetOverriddenPower(overridenPower);
            }
        }

        public class FeatureDefinitionAttackModifierBuilder : Builders.Features.FeatureDefinitionAttackModifierBuilder
        {
            public FeatureDefinitionAttackModifierBuilder(string name, string guid, RuleDefinitions.AttackModifierMethod attackRollModifierMethod,
                int attackRollModifier, string attackRollAbilityScore, RuleDefinitions.AttackModifierMethod damageRollModifierMethod,
                int damageRollModifier, string damageRollAbilityScore, bool canAddAbilityBonusToSecondary, string additionalAttackTag,
                GuiPresentation guiPresentation) : base(name, guid)
            {
                Configure(attackRollModifierMethod, attackRollModifier, attackRollAbilityScore, damageRollModifierMethod,
                    damageRollModifier, damageRollAbilityScore, canAddAbilityBonusToSecondary, additionalAttackTag);

                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionAttributeModifierBuilder : Builders.Features.FeatureDefinitionAttributeModifierBuilder
        {
            public FeatureDefinitionAttributeModifierBuilder(string name, string guid, AttributeModifierOperation modifierType,
                string attribute, int amount, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetModifierType2(modifierType);
                Definition.SetModifiedAttribute(attribute);
                Definition.SetModifierValue(amount);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionMagicAffinityBuilder : Builders.Features.FeatureDefinitionMagicAffinityBuilder
        {
            public FeatureDefinitionMagicAffinityBuilder(string name, string guid, RuleDefinitions.ConcentrationAffinity concentrationAffinity,
                int threshold, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetConcentrationAffinity(concentrationAffinity);
                if (threshold > 0)
                {
                    Definition.SetOverConcentrationThreshold(threshold);
                }
                Definition.SetGuiPresentation(guiPresentation);
            }

            public FeatureDefinitionMagicAffinityBuilder(string name, string guid, int levelBonus,
                GuiPresentation guiPresentation, params string[] spellNames) :
                this(name, guid, levelBonus, guiPresentation, spellNames.AsEnumerable())
            {
            }

            public FeatureDefinitionMagicAffinityBuilder(string name, string guid, int levelBonus,
                GuiPresentation guiPresentation, IEnumerable<string> spellNames) : base(name, guid)
            {
                Definition.SetUsesWarList(true);
                Definition.SetWarListSlotBonus(levelBonus);
                Definition.WarListSpells.AddRange(spellNames);
                Definition.SetGuiPresentation(guiPresentation);
            }

            public FeatureDefinitionMagicAffinityBuilder(string name, string guid, int attackModifier,
                int dcModifier, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetSpellAttackModifier(attackModifier);
                Definition.SetSaveDCModifier(dcModifier);
                Definition.SetGuiPresentation(guiPresentation);
            }

            public FeatureDefinitionMagicAffinityBuilder(string name, string guid, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetSomaticWithWeaponOrShield(true);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class ConditionDefinitionBuilder : Builders.ConditionDefinitionBuilder
        {
            public ConditionDefinitionBuilder(string name, string guid, RuleDefinitions.DurationType durationType, int durationParameter,
                bool silent, GuiPresentation guiPresentation, params FeatureDefinition[] conditionFeatures) :
                    this(name, guid, durationType, durationParameter, silent, guiPresentation, conditionFeatures.AsEnumerable())
            {
            }

            public ConditionDefinitionBuilder(string name, string guid, RuleDefinitions.DurationType durationType, int durationParameter,
                bool silent, GuiPresentation guiPresentation, IEnumerable<FeatureDefinition> conditionFeatures) : base(name, guid)
            {
                Configure(durationType, durationParameter, silent, conditionFeatures);

                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionSavingThrowAffinityBuilder : Builders.Features.FeatureDefinitionSavingThrowAffinityBuilder
        {
            public FeatureDefinitionSavingThrowAffinityBuilder(string name, string guid, IEnumerable<string> abilityScores,
                RuleDefinitions.CharacterSavingThrowAffinity affinityType,
                FeatureDefinitionSavingThrowAffinity.ModifierType modifierType, int diceNumber, RuleDefinitions.DieType dieType,
                bool againstMagic, GuiPresentation guiPresentation) : base(name, guid)
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
            // TODO: convert tuples to ()
            public FeatureDefinitionAbilityCheckAffinityBuilder(string name, string guid, IEnumerable<Tuple<string, string>> abilityProficiencyPairs,
                int diceNumber, RuleDefinitions.DieType dieType, RuleDefinitions.CharacterAbilityCheckAffinity affinityType,
                GuiPresentation guiPresentation) : base(name, guid)
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
            public FeatureDefinitionCraftingAffinityBuilder(string name, string guid, IEnumerable<ToolTypeDefinition> toolTypes,
                float durationMultiplier, bool doubleProficiencyBonus, GuiPresentation guiPresentation) : base(name, guid)
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
            public RestActivityDefinitionBuilder(string name, string guid, RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType,
                RestActivityDefinition.ActivityCondition condition, string functor, string stringParameter, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetRestStage(restStage);
                Definition.SetRestType(restType);
                Definition.SetCondition(condition);
                Definition.SetFunctor(functor);
                Definition.SetStringParameter(stringParameter);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionMovementAffinityBuilder : Builders.Features.FeatureDefinitionMovementAffinityBuilder
        {
            public FeatureDefinitionMovementAffinityBuilder(string name, string guid, bool addBase,
                int speedAdd, float speedMult, GuiPresentation guiPresentation) : base(name, guid)
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
            public FeatureDefinitionHealingModifierBuilder(string name, string guid, int healingBonusDiceNumber, RuleDefinitions.DieType healingBonusDiceType,
            RuleDefinitions.LevelSourceType addLevel, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetHealingBonusDiceNumber(healingBonusDiceNumber);
                Definition.SetHealingBonusDiceType(healingBonusDiceType);
                Definition.SetAddLevel(addLevel);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionBonusCantripsBuilder : Builders.Features.FeatureDefinitionBonusCantripsBuilder
        {
            public FeatureDefinitionBonusCantripsBuilder(string name, string guid, IEnumerable<SpellDefinition> cantrips, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.BonusCantrips.AddRange(cantrips);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionAdditionalDamageBuilder : Builders.Features.FeatureDefinitionAdditionalDamageBuilder
        {
            public FeatureDefinitionAdditionalDamageBuilder(string name, string guid, string notificationTag, RuleDefinitions.FeatureLimitedUsage limitedUsage,
                RuleDefinitions.AdditionalDamageValueDetermination damageValueDetermination,
                RuleDefinitions.AdditionalDamageTriggerCondition triggerCondition, RuleDefinitions.AdditionalDamageRequiredProperty requiredProperty,
                bool attackModeOnly, RuleDefinitions.DieType damageDieType, int damageDiceNumber, RuleDefinitions.AdditionalDamageType additionalDamageType,
                string specificDamageType, RuleDefinitions.AdditionalDamageAdvancement damageAdvancement, IEnumerable<DiceByRank> diceByRankTable,
                bool hasSavingThrow, string savingThrowAbility, int savingThrowDC, RuleDefinitions.EffectSavingThrowType damageSaveAffinity,
                IEnumerable<ConditionOperationDescription> conditionOperations,
            GuiPresentation guiPresentation) : base(name, guid)
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
                .SetProficiencies(type,  proficiencies);
        }

        public static FeatureDefinitionAttributeModifier BuildAttributeModifier(AttributeModifierOperation modifierType,
            string attribute, int amount, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionAttributeModifierBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                modifierType, attribute, amount, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityHeightenedList(IEnumerable<string> spellNames, int levelBonus, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                levelBonus, guiPresentation, spellNames).AddToDB();
        }

        public static ConditionDefinition BuildCondition(IEnumerable<FeatureDefinition> conditionFeatures, RuleDefinitions.DurationType durationType,
            int durationParameter, bool silent, string name, GuiPresentation guiPresentation)
        {
            return new ConditionDefinitionBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                durationType, durationParameter, silent, guiPresentation, conditionFeatures).AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityModifiers(int attackModifier, int dcModifier, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                attackModifier, dcModifier, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionPower BuildSpellFormPower(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
            RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge, string name, GuiPresentation guiPresentation)
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

            return new FeatureDefinitionPowerBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                usesPerRecharge, usesDetermination, AttributeDefinitions.Intelligence, activationTime, costPerUse, recharge, false, false, AttributeDefinitions.Intelligence,
                effectDescriptionBuilder.Build(), guiPresentation).AddToDB();
        }

        public static RestActivityDefinition BuildRestActivity(RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType, RestActivityDefinition.ActivityCondition condition,
            string functor, string stringParameter, string name, GuiPresentation guiPresentation)
        {
            return new RestActivityDefinitionBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                restStage, restType, condition, functor, stringParameter, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionAttackModifier BuildAttackModifier(RuleDefinitions.AttackModifierMethod attackRollModifierMethod,
            int attackRollModifier, string attackRollAbilityScore, RuleDefinitions.AttackModifierMethod damageRollModifierMethod,
            int damageRollModifier, string damageRollAbilityScore, bool canAddAbilityBonusToSecondary, string additionalAttackTag,
            string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionAttackModifierBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                attackRollModifierMethod, attackRollModifier, attackRollAbilityScore, damageRollModifierMethod, damageRollModifier, damageRollAbilityScore,
                canAddAbilityBonusToSecondary, additionalAttackTag, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionMovementAffinity BuildMovementAffinity(bool addBase, int speedAdd, float speedMult, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionMovementAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                addBase, speedAdd, speedMult, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionHealingModifier BuildHealingModifier(int healingBonusDiceNumber, RuleDefinitions.DieType healingBonusDiceType,
            RuleDefinitions.LevelSourceType addLevel, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionHealingModifierBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                healingBonusDiceNumber, healingBonusDiceType, addLevel, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionBonusCantrips BuildBonusCantrips(IEnumerable<SpellDefinition> cantrips, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionBonusCantripsBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                cantrips, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionSavingThrowAffinity BuildSavingThrowAffinity(IEnumerable<string> abilityScores,
            RuleDefinitions.CharacterSavingThrowAffinity affinityType,
            FeatureDefinitionSavingThrowAffinity.ModifierType modifierType, int diceNumber, RuleDefinitions.DieType dieType,
            bool againstMagic, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionSavingThrowAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                abilityScores, affinityType, modifierType, diceNumber, dieType, againstMagic, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionAbilityCheckAffinity BuildAbilityAffinity(IEnumerable<Tuple<string, string>> abilityProficiencyPairs,
            int diceNumber, RuleDefinitions.DieType dieType, RuleDefinitions.CharacterAbilityCheckAffinity affinityType,
            string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionAbilityCheckAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                abilityProficiencyPairs, diceNumber, dieType, affinityType, guiPresentation).AddToDB();
        }

        public static FeatureDefinitionEquipmentAffinity BuildEquipmentAffinity(float carryingCapacityMultiplier, float additionalCarryingCapacity, string name, GuiPresentation guiPresentation)
        {
            return Builders.Features.FeatureDefinitionEquipmentAffinityBuilder
                .Create(name, TinkererClass.GuidNamespace)
                .SetGuiPresentation(guiPresentation)
                .SetCarryingCapacityMultiplier(carryingCapacityMultiplier, additionalCarryingCapacity)
                .AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityConcentration(RuleDefinitions.ConcentrationAffinity concentrationAffinity,
            int threshold, string name, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                concentrationAffinity, threshold, guiPresentation).AddToDB();
        }

        public class FeatureDefinitionFeatureSetBuilder : Builders.Features.FeatureDefinitionFeatureSetBuilder
        {
            public FeatureDefinitionFeatureSetBuilder(string name, string guid, IEnumerable<FeatureDefinition> featureSet,
                FeatureDefinitionFeatureSet.FeatureSetMode mode, int defaultSelection, bool uniqueChoices,
                bool enumerateInDescription, GuiPresentation guiPresentation) : base(name, guid)
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
