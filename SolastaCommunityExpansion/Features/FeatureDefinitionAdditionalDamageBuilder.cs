using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionAdditionalDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
    {
        public FeatureDefinitionAdditionalDamageBuilder(string name, string guid,
            string notificationTag, RuleDefinitions.FeatureLimitedUsage limitedUsage,
        RuleDefinitions.AdditionalDamageValueDetermination damageValueDetermination,
        RuleDefinitions.AdditionalDamageTriggerCondition triggerCondition, RuleDefinitions.AdditionalDamageRequiredProperty requiredProperty,
        bool attackModeOnly, RuleDefinitions.DieType damageDieType, int damageDiceNumber, RuleDefinitions.AdditionalDamageType additionalDamageType,
        string specificDamageType, RuleDefinitions.AdditionalDamageAdvancement damageAdvancement, List<DiceByRank> diceByRankTable,
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
            Definition.SetField("diceByRankTable", diceByRankTable);
            Definition.SetDamageDieType(damageDieType);
            Definition.SetGuiPresentation(guiPresentation);
            Definition.SetField("conditionOperations", new List<ConditionOperationDescription>());
            Definition.SetField("familiesWithAdditionalDice", new List<string>());
        }

        public FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage toCopy, string name, string guid,
            GuiPresentation guiPresentation) : base(toCopy, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionAdditionalDamageBuilder SetSpecificDamageType(string damageType)
        {
            Definition.SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.Specific);
            Definition.SetSpecificDamageType(damageType);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder SetDamageDice(RuleDefinitions.DieType dieType, int diceNumber)
        {
            Definition.SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.Die);
            Definition.SetDamageDiceNumber(diceNumber);
            Definition.SetDamageDieType(dieType);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder SetNotificationTag(string tag)
        {
            Definition.SetNotificationTag(tag);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder NoAdvancement()
        {
            Definition.SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.None);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder SetClassAdvancement(List<DiceByRank> diceByRanks)
        {
            Definition.SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.ClassLevel);
            Definition.DiceByRankTable.Clear();
            Definition.DiceByRankTable.AddRange(diceByRanks);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition trigger)
        {
            Definition.SetTriggerCondition(trigger);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder SetNoSave()
        {
            Definition.SetDamageSaveAffinity(RuleDefinitions.EffectSavingThrowType.None);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder SetConditionOperations(List<ConditionOperationDescription> operations)
        {
            Definition.ConditionOperations.Clear();
            Definition.ConditionOperations.AddRange(operations);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder SetTargetCondition(ConditionDefinition requiredCondition, RuleDefinitions.AdditionalDamageTriggerCondition trigger)
        {
            Definition.SetRequiredTargetCondition(requiredCondition);
            Definition.SetTriggerCondition(trigger);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage limit)
        {
            Definition.SetLimitedUsage(limit);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder SetImpactParticleReference(AssetReference asset)
        {
            Definition.SetImpactParticleReference(asset);
            return this;
        }
    }
}
