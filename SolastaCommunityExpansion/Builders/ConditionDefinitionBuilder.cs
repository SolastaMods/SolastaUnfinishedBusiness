using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
    public abstract class ConditionDefinitionBuilder<TDefinition> : BaseDefinitionBuilder<TDefinition> where TDefinition : ConditionDefinition
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

        protected ConditionDefinitionBuilder(TDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public ConditionDefinitionBuilder<TDefinition> SetAmountOrigin(ConditionDefinition.OriginOfAmount value)
        {
            Definition.SetAmountOrigin(value);
            return this;
        }
    }

    public class ConditionDefinitionBuilder : ConditionDefinitionBuilder<ConditionDefinition>
    {
        private ConditionDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        private ConditionDefinitionBuilder(string name, Guid guidNamespace)
            : base(name, guidNamespace)
        {
        }

        private ConditionDefinitionBuilder(ConditionDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        private ConditionDefinitionBuilder(ConditionDefinition original, string name, Guid guidNamespace)
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
