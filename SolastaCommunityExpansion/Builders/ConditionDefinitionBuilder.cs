using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
    public class ConditionDefinitionBuilder<TDefinition> : BaseDefinitionBuilder<TDefinition> where TDefinition : ConditionDefinition
    {
        protected ConditionDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
            var assetReference = new AssetReference();

            Definition
                .SetConditionStartParticleReference(assetReference)
                .SetConditionParticleReference(assetReference)
                .SetConditionEndParticleReference(assetReference)
                .SetCharacterShaderReference(assetReference);
        }

        protected ConditionDefinitionBuilder(string name, Guid guidNamespace)
            : base(name, guidNamespace)
        {
            var assetReference = new AssetReference();

            Definition
                .SetConditionStartParticleReference(assetReference)
                .SetConditionParticleReference(assetReference)
                .SetConditionEndParticleReference(assetReference)
                .SetCharacterShaderReference(assetReference);
        }

        protected ConditionDefinitionBuilder(TDefinition original, string name, Guid guidNamespace)
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

        public static ConditionDefinitionBuilder<TDefinition> Create(string name, Guid guidNamespace)
        {
            return new ConditionDefinitionBuilder<TDefinition>(name, guidNamespace);
        }
    }

    public class ConditionDefinitionBuilder : ConditionDefinitionBuilder<ConditionDefinition>
    {
        public ConditionDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public ConditionDefinitionBuilder(ConditionDefinition original, string name, Guid guidNamespace)
            : base(original, name, guidNamespace)
        {
        }
    }
}
