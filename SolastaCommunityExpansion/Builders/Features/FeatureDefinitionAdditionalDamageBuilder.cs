using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAdditionalDamageBuilder : DefinitionBuilder<FeatureDefinitionAdditionalDamage>
    {
        /*
        private FeatureDefinitionAdditionalDamageBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        private FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionAdditionalDamageBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid, Category.None)
        {
        }
        */

        private FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        // TODO: remove this ctor (replace with smaller methods)
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
            Definition.DiceByRankTable.SetRange(diceByRankTable);
            Definition.SetDamageDieType(damageDieType);
            Definition.SetGuiPresentation(guiPresentation);
        }

        // Add other standard Create methods and constructors as required.

        public static FeatureDefinitionAdditionalDamageBuilder Create(FeatureDefinitionAdditionalDamage original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionAdditionalDamageBuilder(original, name, namespaceGuid);
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

        public FeatureDefinitionAdditionalDamageBuilder SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty property)
        {
            Definition.SetRequiredProperty(property);
            return this;
        }
    }
}
