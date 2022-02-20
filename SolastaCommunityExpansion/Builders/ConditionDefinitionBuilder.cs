using System;
using System.Collections.Generic;
using SolastaModApi.Diagnostics;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
    /// <summary>
    /// Abstract ConditionDefinitionBuilder that allows creating builders for custom ConditionDefinition types.
    /// </summary>
    /// <typeparam name="TDefinition"></typeparam>
    /// <typeparam name="TBuilder"></typeparam>
    public abstract class ConditionDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : ConditionDefinition
        where TBuilder : ConditionDefinitionBuilder<TDefinition, TBuilder>
    {
        private void ClearParticleReferences()
        {
            var assetReference = new AssetReference();

            Definition
                .SetConditionStartParticleReference(assetReference)
                .SetConditionParticleReference(assetReference)
                .SetConditionEndParticleReference(assetReference)
                .SetCharacterShaderReference(assetReference);
        }

        protected ConditionDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
            ClearParticleReferences();
        }

        protected ConditionDefinitionBuilder(string name, Guid guidNamespace)
            : base(name, guidNamespace)
        {
            ClearParticleReferences();
        }

        protected ConditionDefinitionBuilder(TDefinition original, string name, Guid guidNamespace)
            : base(original, name, guidNamespace)
        {
        }

        protected ConditionDefinitionBuilder(TDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

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

        public TBuilder ClearRecurrentEffectForms()
        {
            Definition.ClearRecurrentEffectForms();
            return This();
        }

        // TODO: factor out validation code
        // rename to match names of similar method in EffectDescriptionBuilder (and elsewhere)
        public TBuilder SetDuration(RuleDefinitions.DurationType type, int? duration = null)
        {
            switch (type)
            {
                case RuleDefinitions.DurationType.Round:
                case RuleDefinitions.DurationType.Minute:
                case RuleDefinitions.DurationType.Hour:
                case RuleDefinitions.DurationType.Day:
                    if (duration == null)
                    {
                        throw new ArgumentNullException(nameof(duration), $"A duration value is required for duration type {type}.");
                    }
                    Definition.SetDurationParameter(duration.Value);
                    break;
                default:
                    if (duration != null)
                    {
                        throw new SolastaModApiException($"A duration value is not expected for duration type {type}");
                    }
                    Definition.SetDurationParameter(0);
                    break;
            }

            Definition.SetDurationType(type);

            return (TBuilder)this;
        }

        // TODO: add more methods as required (and that aren't delegating property setters to add value)
    }

    /// <summary>
    /// Concrete ConditionDefinitionBuilder that allows building ConditionDefinition.
    /// </summary>
    public class ConditionDefinitionBuilder :
        ConditionDefinitionBuilder<ConditionDefinition, ConditionDefinitionBuilder>
    {
        protected ConditionDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        protected ConditionDefinitionBuilder(string name, Guid guidNamespace)
            : base(name, guidNamespace)
        {
        }

        protected ConditionDefinitionBuilder(ConditionDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        protected ConditionDefinitionBuilder(ConditionDefinition original, string name, Guid guidNamespace)
            : base(original, name, guidNamespace)
        {
        }

        public static ConditionDefinitionBuilder Create(string name, string guid)
        {
            return new ConditionDefinitionBuilder(name, guid);
        }

        public static ConditionDefinitionBuilder Create(string name, Guid guidNamespace)
        {
            return new ConditionDefinitionBuilder(name, guidNamespace);
        }

        public static ConditionDefinitionBuilder Create(ConditionDefinition original, string name, Guid guidNamespace)
        {
            return new ConditionDefinitionBuilder(original, name, guidNamespace);
        }

        public static ConditionDefinitionBuilder Create(ConditionDefinition original, string name, string guid)
        {
            return new ConditionDefinitionBuilder(original, name, guid);
        }
    }
}
