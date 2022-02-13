using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
    public class ConditionDefinitionBuilder<TDefinition> : BaseDefinitionBuilder<TDefinition> where TDefinition : ConditionDefinition
    {
        // Specific to PathOfTheLight
        private ConditionDefinitionBuilder(string name, string guid, Action<TDefinition> modifyDefinition) : base(name, guid)
        {
            Definition
                .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                .SetAllowMultipleInstances(false)
                .SetDurationType(RuleDefinitions.DurationType.Minute)
                .SetDurationParameter(1)
                .SetConditionStartParticleReference(new AssetReference())
                .SetConditionParticleReference(new AssetReference())
                .SetConditionEndParticleReference(new AssetReference())
                .SetCharacterShaderReference(new AssetReference());

            modifyDefinition?.Invoke(Definition);
        }

        protected ConditionDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        private ConditionDefinitionBuilder(TDefinition original, string name, Guid guidNamespace)
            : base(original, name, guidNamespace)
        {
        }

        public ConditionDefinitionBuilder<TDefinition> SetAmountOrigin(ConditionDefinition.OriginOfAmount value)
        {
            Definition.SetAmountOrigin(value);
            return this;
        }

        public static ConditionDefinitionBuilder<TDefinition> Create(TDefinition original, string name, Guid guidNamespace)
        {
            return new ConditionDefinitionBuilder<TDefinition>(original, name, guidNamespace);
        }


        // Specific to PathOfTheLight
        public static TDefinition Build(string name, string guid, Action<TDefinition> modifyDefinition = null)
        {
            return new ConditionDefinitionBuilder<TDefinition>(name, guid, modifyDefinition).AddToDB();
        }
    }

    public class ConditionDefinitionBuilder : ConditionDefinitionBuilder<ConditionDefinition>
    {

        // TODO: refactor/remove
        // Specific to SpellShield and LifeTransmuter
        public ConditionDefinitionBuilder(string name, string guid, IEnumerable<FeatureDefinition> conditionFeatures, RuleDefinitions.DurationType durationType,
            int durationParameter, bool silent) : base(name, guid)
        {
            Definition.Features.AddRange(conditionFeatures);
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
        }
    }
}
