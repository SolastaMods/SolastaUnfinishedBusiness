using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders.Features
{
    public abstract class FeatureDefinitionAdditionalDamageBuilder<TDefinition, TBuilder> : FeatureDefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : FeatureDefinitionAdditionalDamage
        where TBuilder : FeatureDefinitionAdditionalDamageBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected FeatureDefinitionAdditionalDamageBuilder(TDefinition original) : base(original)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public TBuilder SetSpecificDamageType(string damageType)
        {
            Definition.SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.Specific);
            Definition.SetSpecificDamageType(damageType);
            return This();
        }

        public TBuilder SetDamageDice(RuleDefinitions.DieType dieType, int diceNumber)
        {
            Definition.SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.Die);
            Definition.SetDamageDiceNumber(diceNumber);
            Definition.SetDamageDieType(dieType);
            return This();
        }

        public TBuilder SetNotificationTag(string tag)
        {
            Definition.SetNotificationTag(tag);
            return This();
        }

        public TBuilder SetNoAdvancement()
        {
            Definition.SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.None);
            return This();
        }

        public TBuilder SetClassAdvancement(params DiceByRank[] diceByRanks)
        {
            return SetClassAdvancement(diceByRanks.AsEnumerable());
        }

        public TBuilder SetClassAdvancement(IEnumerable<DiceByRank> diceByRanks)
        {
            Definition.SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.ClassLevel);
            Definition.DiceByRankTable.SetRange(diceByRanks);
            return This();
        }

        public TBuilder SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition trigger)
        {
            Definition.SetTriggerCondition(trigger);
            return This();
        }

        public TBuilder SetNoSave()
        {
            Definition.SetDamageSaveAffinity(RuleDefinitions.EffectSavingThrowType.None);
            return This();
        }

        public TBuilder SetConditionOperations(params ConditionOperationDescription[] operations)
        {
            return SetConditionOperations(operations.AsEnumerable());
        }

        public TBuilder SetConditionOperations(IEnumerable<ConditionOperationDescription> operations)
        {
            Definition.ConditionOperations.SetRange(operations);
            return This();
        }

        public TBuilder SetTargetCondition(ConditionDefinition requiredCondition, RuleDefinitions.AdditionalDamageTriggerCondition trigger)
        {
            Definition.SetRequiredTargetCondition(requiredCondition);
            Definition.SetTriggerCondition(trigger);
            return This();
        }

        public TBuilder SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage limit)
        {
            Definition.SetLimitedUsage(limit);
            return This();
        }

        public TBuilder SetImpactParticleReference(AssetReference asset)
        {
            Definition.SetImpactParticleReference(asset);
            return This();
        }
    }

    public class FeatureDefinitionAdditionalDamageBuilder : FeatureDefinitionAdditionalDamageBuilder<FeatureDefinitionAdditionalDamage, FeatureDefinitionAdditionalDamageBuilder>
    {
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

        #region Constructors
        protected FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original) : base(original)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        // Add other standard Create methods and constructors as required.

        public static FeatureDefinitionAdditionalDamageBuilder Create(FeatureDefinitionAdditionalDamage original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionAdditionalDamageBuilder(original, name, namespaceGuid);
        }
    }
}
