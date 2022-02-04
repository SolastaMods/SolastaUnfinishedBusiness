using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
    public class ConditionDefinitionBuilder<TDefinition> : BaseDefinitionBuilder<TDefinition> where TDefinition : ConditionDefinition
    {
        public ConditionDefinitionBuilder(string name, string guid, Action<TDefinition> modifyDefinition) : base(name, guid)
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

            Definition.SetField("recurrentEffectForms", new List<EffectForm>());
            Definition.SetField("cancellingConditions", new List<ConditionDefinition>());

            modifyDefinition?.Invoke(Definition);
        }

        public ConditionDefinitionBuilder(string name, string guid) : base(name, guid)
        {
        }

        public ConditionDefinitionBuilder(TDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public ConditionDefinitionBuilder(TDefinition original, string name, Guid guidNamespace, string category = null)
            : base(original, name, guidNamespace, category)
        {
        }

        public ConditionDefinitionBuilder<TDefinition> SetAmountOrigin(ConditionDefinition.OriginOfAmount value)
        {
            Definition.SetAmountOrigin(value);
            return this;
        }

        public static TDefinition Build(string name, string guid, Action<TDefinition> modifyDefinition = null)
        {
            return new ConditionDefinitionBuilder<TDefinition>(name, guid, modifyDefinition).AddToDB();
        }
    }

    public class ConditionDefinitionBuilder : ConditionDefinitionBuilder<ConditionDefinition>
    {
        // TODO: additional ctors

        public ConditionDefinitionBuilder(string name, string guid, Action<ConditionDefinition> modifyDefinition = null) : base(name, guid, modifyDefinition)
        {
        }

        // TODO: refactor/remove
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
            Definition.SetField("recurrentEffectForms", new List<EffectForm>());
            Definition.SetField("cancellingConditions", new List<ConditionDefinition>());
        }
    }
}
