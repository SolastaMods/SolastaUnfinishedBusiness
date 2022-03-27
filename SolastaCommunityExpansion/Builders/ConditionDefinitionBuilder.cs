using System;
using System.Collections.Generic;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
    [Flags]
    public enum Silent
    {
        None,
        WhenAdded = 1,
        WhenRemoved = 2,
        WhenAddedOrRemoved = WhenAdded | WhenRemoved
    }

    /// <summary>
    /// Abstract ConditionDefinitionBuilder that allows creating builders for custom ConditionDefinition types.
    /// </summary>
    /// <typeparam name="TDefinition"></typeparam>
    /// <typeparam name="TBuilder"></typeparam>
    public abstract class ConditionDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : ConditionDefinition
        where TBuilder : ConditionDefinitionBuilder<TDefinition, TBuilder>
    {
        protected override void Initialise()
        {
            base.Initialise();

            SetEmptyParticleReferencesWhereNull();
        }

        private void SetEmptyParticleReferencesWhereNull()
        {
            Definition.SetEmptyParticleReferencesWhereNull();
        }

        #region Constructors
        protected ConditionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
            SetEmptyParticleReferencesWhereNull();
        }

        protected ConditionDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
            SetEmptyParticleReferencesWhereNull();
        }

        protected ConditionDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
            SetEmptyParticleReferencesWhereNull();
        }

        protected ConditionDefinitionBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected ConditionDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected ConditionDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        protected ConditionDefinitionBuilder(TDefinition original) : base(original)
        {
        }
        #endregion

        // Setters delegating to Definition
        public TBuilder SetAllowMultipleInstances(bool value)
        {
            Definition.SetAllowMultipleInstances(value);
            return This();
        }

        public TBuilder SetAmountOrigin(ConditionDefinition.OriginOfAmount value)
        {
            Definition.SetAmountOrigin(value);
            return This();
        }

        public TBuilder SetConditionType(RuleDefinitions.ConditionType value)
        {
            Definition.SetConditionType(value);
            return This();
        }

        public TBuilder SetTurnOccurence(RuleDefinitions.TurnOccurenceType value)
        {
            Definition.SetTurnOccurence(value);
            return This();
        }

        public TBuilder AddConditionTags(IEnumerable<string> value)
        {
            Definition.AddConditionTags(value);
            return This();
        }

        public TBuilder AddConditionTags(params string[] value)
        {
            Definition.AddConditionTags(value);
            return This();
        }

        public TBuilder AddFeatures(IEnumerable<FeatureDefinition> value)
        {
            Definition.AddFeatures(value);
            return This();
        }

        public TBuilder AddFeatures(params FeatureDefinition[] value)
        {
            Definition.AddFeatures(value);
            return This();
        }

        public TBuilder SetFeatures(IEnumerable<FeatureDefinition> value)
        {
            Definition.SetFeatures(value);
            return This();
        }

        public TBuilder SetFeatures(params FeatureDefinition[] value)
        {
            Definition.SetFeatures(value);
            return This();
        }

        public TBuilder SetAdditionalDamageData(RuleDefinitions.DieType dieType, int numberOfDie, ConditionDefinition.DamageQuantity damageQuantity, bool additionalDamageWhenHit)
        {
            Definition
                .SetAdditionalDamageWhenHit(additionalDamageWhenHit)
                .SetAdditionalDamageDieType(dieType)
                .SetAdditionalDamageDieNumber(numberOfDie)
                .SetAdditionalDamageQuantity(damageQuantity);

            return This();
        }

        public TBuilder SetParentCondition(ConditionDefinition value)
        {
            Definition.SetParentCondition(value);
            return This();
        }

        public TBuilder SetTerminateWhenRemoved(bool value)
        {
            Definition.SetTerminateWhenRemoved(value);
            return This();
        }

        public TBuilder SetSilentWhenAdded(bool value)
        {
            Definition.SetSilentWhenAdded(value);
            return This();
        }

        public TBuilder SetSilentWhenRemoved(bool value)
        {
            Definition.SetSilentWhenRemoved(value);
            return This();
        }

        public TBuilder SetSilent(Silent silent)
        {
            SetSilentWhenRemoved(silent.HasFlag(Silent.WhenRemoved));
            SetSilentWhenAdded(silent.HasFlag(Silent.WhenAdded));
            return This();
        }

        public TBuilder SetSpecialDuration(bool value)
        {
            Definition.SetSpecialDuration(value);
            return This();
        }

        public TBuilder SetPossessive(bool value)
        {
            Definition.SetPossessive(value);
            return This();
        }

        public TBuilder SetSpecialInterruptions(params RuleDefinitions.ConditionInterruption[] value)
        {
            Definition.SetSpecialInterruptions(value);
            return This();
        }

        public TBuilder ClearRecurrentEffectForms()
        {
            Definition.ClearRecurrentEffectForms();
            return This();
        }

        // TODO: rename to match names of similar method in EffectDescriptionBuilder (and elsewhere)
        public TBuilder SetDuration(RuleDefinitions.DurationType type, int duration = 0, bool validate = true)
        {
            if (validate)
            {
                Preconditions.IsValidDuration(type, duration);
            }

            Definition.SetDurationParameter(duration);
            Definition.SetDurationType(type);

            return This();
        }

        public TBuilder Configure(RuleDefinitions.DurationType durationType, int durationParameter,
            bool silent, IEnumerable<FeatureDefinition> conditionFeatures)
        {
            Definition.Features.AddRange(conditionFeatures);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationType(durationType);
            Definition.SetDurationParameter(durationParameter);
            if (silent)
            {
                Definition.SetSilentWhenAdded(true);
                Definition.SetSilentWhenRemoved(true);
            }
            return This();
        }
    }

    /// <summary>
    /// Concrete ConditionDefinitionBuilder that allows building ConditionDefinition.
    /// </summary>
    public class ConditionDefinitionBuilder :
        ConditionDefinitionBuilder<ConditionDefinition, ConditionDefinitionBuilder>
    {
        #region Constructors
        protected ConditionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected ConditionDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected ConditionDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected ConditionDefinitionBuilder(ConditionDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected ConditionDefinitionBuilder(ConditionDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected ConditionDefinitionBuilder(ConditionDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        protected ConditionDefinitionBuilder(ConditionDefinition original) : base(original)
        {
        }
        #endregion
    }
}
