using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAdditionalDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
    {
        public FeatureDefinitionAdditionalDamageBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionAdditionalDamageBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        // TODO: remove this ctor
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

            SetDamageDice(damageDieType, damageDiceNumber);

            // Does this conflict with SetSpecificDamageType below?
            Definition.SetAdditionalDamageType(additionalDamageType);
            Definition.SetSpecificDamageType(specificDamageType);

            Definition.SetDamageAdvancement(damageAdvancement);
            Definition.SetField("diceByRankTable", diceByRankTable);
            Definition.SetDamageDieType(damageDieType);
            Definition.SetGuiPresentation(guiPresentation);
            Definition.SetField("conditionOperations", new List<ConditionOperationDescription>());
            Definition.SetField("familiesWithAdditionalDice", new List<string>());
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

        public FeatureDefinitionAdditionalDamageBuilder SetNoAdvancement()
        {
            Definition.SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.None);
            return this;
        }

        public FeatureDefinitionAdditionalDamageBuilder SetClassAdvancement(params DiceByRank[] diceByRanks)
        {
            return SetClassAdvancement(diceByRanks.AsEnumerable());
        }

        public FeatureDefinitionAdditionalDamageBuilder SetClassAdvancement(IEnumerable<DiceByRank> diceByRanks)
        {
            Definition.SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.ClassLevel);
            Definition.DiceByRankTable.SetRange(diceByRanks);
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

        public FeatureDefinitionAdditionalDamageBuilder SetConditionOperations(params ConditionOperationDescription[] operations)
        {
            return SetConditionOperations(operations.AsEnumerable());
        }

        public FeatureDefinitionAdditionalDamageBuilder SetConditionOperations(IEnumerable<ConditionOperationDescription> operations)
        {
            Definition.ConditionOperations.SetRange(operations);
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
