using HarmonyLib;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    internal class FeatureHelpers
    {

        // TODO Most of theese builders should likely get moved/merged with the CE builders.
        public class FeatureDefinitionPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            public FeatureDefinitionPowerBuilder(string name, string guid, int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
                string usesAbilityScoreName, RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
                bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
                EffectDescription effectDescription, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetFixedUsesPerRecharge(usesPerRecharge);
                Definition.SetUsesDetermination(usesDetermination);
                Definition.SetUsesAbilityScoreName(usesAbilityScoreName);
                Definition.SetActivationTime(activationTime);
                Definition.SetCostPerUse(costPerUse);
                Definition.SetRechargeRate(recharge);
                Definition.SetProficiencyBonusToAttack(proficiencyBonusToAttack);
                Definition.SetAbilityScoreBonusToAttack(abilityScoreBonusToAttack);
                Definition.SetAbilityScore(abilityScore);
                Definition.SetEffectDescription(effectDescription);
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

        public class FeatureDefinitionProficiencyBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
        {
            public FeatureDefinitionProficiencyBuilder(string name, string guid, RuleDefinitions.ProficiencyType type,
            List<string> proficiencies, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetProficiencyType(type);
                foreach (string item in proficiencies)
                {
                    Definition.Proficiencies.Add(item);
                }
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionAttackModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttackModifier>
        {
            public FeatureDefinitionAttackModifierBuilder(string name, string guid, RuleDefinitions.AttackModifierMethod attackRollModifierMethod,
            int attackRollModifier, string attackRollAbilityScore, RuleDefinitions.AttackModifierMethod damageRollModifierMethod,
            int damageRollModifier, string damageRollAbilityScore, bool canAddAbilityBonusToSecondary, string additionalAttackTag,
            GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetAttackRollModifierMethod(attackRollModifierMethod);
                Definition.SetAttackRollModifier(attackRollModifier);
                Definition.SetAttackRollAbilityScore(attackRollAbilityScore);
                Definition.SetDamageRollModifierMethod(damageRollModifierMethod);
                Definition.SetDamageRollModifier(damageRollModifier);
                Definition.SetDamageRollAbilityScore(damageRollAbilityScore);
                Definition.SetCanAddAbilityBonusToSecondary(canAddAbilityBonusToSecondary);
                Definition.SetAdditionalAttackTag(additionalAttackTag);

                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionAttributeModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
        {
            public FeatureDefinitionAttributeModifierBuilder(string name, string guid, FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
            string attribute, int amount, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetModifierType2(modifierType);
                Definition.SetModifiedAttribute(attribute);
                Definition.SetModifierValue(amount);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionMagicAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionMagicAffinity>
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

            public FeatureDefinitionMagicAffinityBuilder(string name, string guid, List<string> spellNames,
                int levelBonus, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetUsesWarList(true);
                Definition.SetWarListSlotBonus(levelBonus);
                foreach (string spell in spellNames)
                {
                    Definition.WarListSpells.Add(spell);
                }
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

        public class FeatureDefinitionPointPoolBuilder : BaseDefinitionBuilder<FeatureDefinitionPointPool>
        {
            public FeatureDefinitionPointPoolBuilder(string name, string guid, HeroDefinitions.PointsPoolType poolType, int poolAmount,
            List<string> choices, bool uniqueChoices, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetPoolType(poolType);
                Definition.SetPoolAmount(poolAmount);
                foreach (string item in choices)
                {
                    Definition.RestrictedChoices.Add(item);
                }
                Definition.SetUniqueChoices(uniqueChoices);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class ConditionDefinitionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            public ConditionDefinitionBuilder(string name, string guid, List<FeatureDefinition> conditionFeatures, RuleDefinitions.DurationType durationType,
            int durationParameter, bool silent, GuiPresentation guiPresentation) : base(name, guid)
            {
                foreach (FeatureDefinition feature in conditionFeatures)
                {
                    Definition.Features.Add(feature);
                }
                Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
                Definition.SetAllowMultipleInstances(false);
                Definition.SetDurationType(durationType);
                Definition.SetDurationParameter(durationParameter);
                Definition.SetConditionStartParticleReference(new AssetReference());
                Definition.SetConditionParticleReference(new AssetReference());
                Definition.SetConditionEndParticleReference(new AssetReference());
                Definition.SetCharacterShaderReference(new AssetReference());
                if (silent)
                {
                    Definition.SetSilentWhenAdded(true);
                    Definition.SetSilentWhenRemoved(true);
                }
                Traverse.Create(Definition).Field("recurrentEffectForms").SetValue(new List<EffectForm>());
                Traverse.Create(Definition).Field("cancellingConditions").SetValue(new List<ConditionDefinition>());
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionSavingThrowAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionSavingThrowAffinity>
        {
            public FeatureDefinitionSavingThrowAffinityBuilder(string name, string guid, List<string> abilityScores,
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
                        group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolAbjuration.Name);
                        group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolConjuration.Name);
                        group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolDivination.Name);
                        group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEnchantment.Name);
                        group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation.Name);
                        group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolIllusion.Name);
                        group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolNecromancy.Name);
                        group.restrictedSchools.Add(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation.Name);
                    }
                    Definition.AffinityGroups.Add(group);
                }
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionAbilityCheckAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
        {
            public FeatureDefinitionAbilityCheckAffinityBuilder(string name, string guid, List<Tuple<string, string>> abilityProficiencyPairs,
            int diceNumber, RuleDefinitions.DieType dieType, RuleDefinitions.CharacterAbilityCheckAffinity affinityType,
            GuiPresentation guiPresentation) : base(name, guid)
            {
                foreach (Tuple<string, string> abilityProficiency in abilityProficiencyPairs)
                {
                    FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup group = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
                    {
                        abilityScoreName = abilityProficiency.Item1
                    };
                    if (!String.IsNullOrEmpty(abilityProficiency.Item2))
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

        public class FeatureDefinitionCraftingAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionCraftingAffinity>
        {
            public FeatureDefinitionCraftingAffinityBuilder(string name, string guid, List<ToolTypeDefinition> toolTypes,
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

        public class FeatureDefinitionAutoPreparedSpellsBuilder : BaseDefinitionBuilder<FeatureDefinitionAutoPreparedSpells>
        {
            public FeatureDefinitionAutoPreparedSpellsBuilder(string name, string guid, List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
            CharacterClassDefinition characterclass, GuiPresentation guiPresentation) : base(name, guid)
            {
                Traverse.Create(Definition).Field("autoPreparedSpellsGroups").SetValue(new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>());
                foreach (FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup groups in autospelllists)
                {
                    Definition.AutoPreparedSpellsGroups.Add(groups);
                }
                Definition.SetSpellcastingClass(characterclass);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class RestActivityDefinitionBuilder : BaseDefinitionBuilder<RestActivityDefinition>
        {
            public RestActivityDefinitionBuilder(string name, string guid, RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType, RestActivityDefinition.ActivityCondition condition,
            string functor, string stringParameter, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetRestStage(restStage);
                Definition.SetRestType(restType);
                Definition.SetCondition(condition);
                Definition.SetFunctor(functor);
                Definition.SetStringParameter(stringParameter);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionMovementAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionMovementAffinity>
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

        public class FeatureDefinitionHealingModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionHealingModifier>
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

        public class FeatureDefinitionBonusCantripsBuilder : BaseDefinitionBuilder<FeatureDefinitionBonusCantrips>
        {
            public FeatureDefinitionBonusCantripsBuilder(string name, string guid, List<SpellDefinition> cantrips, GuiPresentation guiPresentation) : base(name, guid)
            {

                foreach (SpellDefinition cantrip in cantrips)
                {
                    Definition.BonusCantrips.Add(cantrip);
                }
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionEquipmentAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionEquipmentAffinity>
        {
            public FeatureDefinitionEquipmentAffinityBuilder(string name, string guid, float carryingCapacityMultiplier,
                float additionalCarryingCapacity, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetCarryingCapacityMultiplier(carryingCapacityMultiplier);
                Definition.SetAdditionalCarryingCapacity(additionalCarryingCapacity);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public class FeatureDefinitionAdditionalDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
        {
            public FeatureDefinitionAdditionalDamageBuilder(string name, string guid, string notificationTag, RuleDefinitions.FeatureLimitedUsage limitedUsage,
            RuleDefinitions.AdditionalDamageValueDetermination damageValueDetermination,
            RuleDefinitions.AdditionalDamageTriggerCondition triggerCondition, RuleDefinitions.AdditionalDamageRequiredProperty requiredProperty,
            bool attackModeOnly, RuleDefinitions.DieType damageDieType, int damageDiceNumber, RuleDefinitions.AdditionalDamageType additionalDamageType,
            string specificDamageType, RuleDefinitions.AdditionalDamageAdvancement damageAdvancement, List<DiceByRank> diceByRankTable,
            bool hasSavingThrow, string savingThrowAbility, int savingThrowDC, RuleDefinitions.EffectSavingThrowType damageSaveAffinity,
            List<ConditionOperationDescription> conditionOperations,
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
                Traverse.Create(Definition).Field("diceByRankTable").SetValue(diceByRankTable);
                Definition.SetDamageDieType(damageDieType);

                Definition.SetHasSavingThrow(hasSavingThrow);
                Definition.SetSavingThrowAbility(savingThrowAbility);
                Definition.SetSavingThrowDC(savingThrowDC);
                Definition.SetDamageSaveAffinity(damageSaveAffinity);
                Traverse.Create(Definition).Field("conditionOperations").SetValue(conditionOperations);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        public static FeatureDefinitionAutoPreparedSpells BuildAutoPreparedSpells(List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
            CharacterClassDefinition characterclass, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionAutoPreparedSpellsBuilder builder = new FeatureDefinitionAutoPreparedSpellsBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                autospelllists, characterclass, guiPresentation);
            return builder.AddToDB();
        }


        public static FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup BuildAutoPreparedSpellGroup(int classLevel, List<SpellDefinition> spellnames)
        {
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup spellgroup = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = classLevel,
                SpellsList = new List<SpellDefinition>()
            };
            foreach (SpellDefinition spell in spellnames)
            {
                spellgroup.SpellsList.Add(spell);
            }
            return spellgroup;
        }

        public static FeatureDefinitionProficiency BuildProficiency(RuleDefinitions.ProficiencyType type,
            List<string> proficiencies, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionProficiencyBuilder builder = new FeatureDefinitionProficiencyBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(), type, proficiencies, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionAttributeModifier BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
            string attribute, int amount, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionAttributeModifierBuilder builder = new FeatureDefinitionAttributeModifierBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                modifierType, attribute, amount, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityHeightenedList(List<string> spellNames, int levelBonus, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionMagicAffinityBuilder builder = new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                spellNames, levelBonus, guiPresentation);
            return builder.AddToDB();
        }

        public static ConditionDefinition BuildCondition(List<FeatureDefinition> conditionFeatures, RuleDefinitions.DurationType durationType,
            int durationParameter, bool silent, string name, GuiPresentation guiPresentation)
        {
            ConditionDefinitionBuilder builder = new ConditionDefinitionBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                conditionFeatures, durationType, durationParameter, silent, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityModifiers(int attackModifier, int dcModifier, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionMagicAffinityBuilder builder = new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                attackModifier, dcModifier, guiPresentation);
            return builder.AddToDB();
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


            FeatureDefinitionPowerBuilder builder = new FeatureDefinitionPowerBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                usesPerRecharge, usesDetermination, AttributeDefinitions.Intelligence, activationTime, costPerUse, recharge, false, false, AttributeDefinitions.Intelligence,
                effectDescriptionBuilder.Build(), guiPresentation);
            return builder.AddToDB();
        }

        public static RestActivityDefinition BuildRestActivity(RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType, RestActivityDefinition.ActivityCondition condition,
            string functor, string stringParameter, string name, GuiPresentation guiPresentation)
        {
            RestActivityDefinitionBuilder builder = new RestActivityDefinitionBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                restStage, restType, condition, functor, stringParameter, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionAttackModifier BuildAttackModifier(RuleDefinitions.AttackModifierMethod attackRollModifierMethod,
            int attackRollModifier, string attackRollAbilityScore, RuleDefinitions.AttackModifierMethod damageRollModifierMethod,
            int damageRollModifier, string damageRollAbilityScore, bool canAddAbilityBonusToSecondary, string additionalAttackTag,
            string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionAttackModifierBuilder builder = new FeatureDefinitionAttackModifierBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                attackRollModifierMethod, attackRollModifier, attackRollAbilityScore, damageRollModifierMethod, damageRollModifier, damageRollAbilityScore,
                canAddAbilityBonusToSecondary, additionalAttackTag, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionMovementAffinity BuildMovementAffinity(bool addBase, int speedAdd, float speedMult, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionMovementAffinityBuilder builder = new FeatureDefinitionMovementAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                addBase, speedAdd, speedMult, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionHealingModifier BuildHealingModifier(int healingBonusDiceNumber, RuleDefinitions.DieType healingBonusDiceType,
            RuleDefinitions.LevelSourceType addLevel, string name, GuiPresentation guiPresentation)
        {

            FeatureDefinitionHealingModifierBuilder healingModifier = new FeatureDefinitionHealingModifierBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                healingBonusDiceNumber, healingBonusDiceType, addLevel, guiPresentation);
            return healingModifier.AddToDB();
        }

        public static FeatureDefinitionBonusCantrips BuildBonusCantrips(List<SpellDefinition> cantrips, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionBonusCantripsBuilder bonusCantrips = new FeatureDefinitionBonusCantripsBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                cantrips, guiPresentation);
            return bonusCantrips.AddToDB();
        }

        public static FeatureDefinitionSavingThrowAffinity BuildSavingThrowAffinity(List<string> abilityScores,
            RuleDefinitions.CharacterSavingThrowAffinity affinityType,
            FeatureDefinitionSavingThrowAffinity.ModifierType modifierType, int diceNumber, RuleDefinitions.DieType dieType,
            bool againstMagic, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionSavingThrowAffinityBuilder builder = new FeatureDefinitionSavingThrowAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                abilityScores, affinityType, modifierType, diceNumber, dieType, againstMagic, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionAbilityCheckAffinity BuildAbilityAffinity(List<Tuple<string, string>> abilityProficiencyPairs,
            int diceNumber, RuleDefinitions.DieType dieType, RuleDefinitions.CharacterAbilityCheckAffinity affinityType,
            string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionAbilityCheckAffinityBuilder builder = new FeatureDefinitionAbilityCheckAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                abilityProficiencyPairs, diceNumber, dieType, affinityType, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionPointPool BuildPointPool(HeroDefinitions.PointsPoolType poolType, int poolAmount,
            List<string> choices, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionPointPoolBuilder builder = new FeatureDefinitionPointPoolBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                poolType, poolAmount, choices, false, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionEquipmentAffinity BuildEquipmentAffinity(float carryingCapacityMultiplier, float additionalCarryingCapacity, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionEquipmentAffinityBuilder equipmentAffinity = new FeatureDefinitionEquipmentAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                carryingCapacityMultiplier, additionalCarryingCapacity, guiPresentation);
            return equipmentAffinity.AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityConcentration(RuleDefinitions.ConcentrationAffinity concentrationAffinity, int threshold, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionMagicAffinityBuilder builder = new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(TinkererClass.GuidNamespace, name).ToString(),
                concentrationAffinity, threshold, guiPresentation);
            return builder.AddToDB();
        }

        public static DiceByRank BuildDiceByRank(int rank, int dice)
        {
            DiceByRank diceByRank = new DiceByRank();
            diceByRank.SetField("rank", rank);
            diceByRank.SetField("diceNumber", dice);
            return diceByRank;
        }

        public class FeatureDefinitionFeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
        {
            public FeatureDefinitionFeatureSetBuilder(string name, string guid, List<FeatureDefinition> featureSet,
                FeatureDefinitionFeatureSet.FeatureSetMode mode, int defaultSelection, bool uniqueChoices,
                bool enumerateInDescription, GuiPresentation guiPresentation) : base(name, guid)
            {
                Traverse.Create(Definition).Field("featureSet").SetValue(featureSet);
                Definition.SetMode(mode);
                Definition.SetDefaultSelection(defaultSelection);
                Definition.SetUniqueChoices(uniqueChoices);
                Definition.SetEnumerateInDescription(enumerateInDescription);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }
    }
}
