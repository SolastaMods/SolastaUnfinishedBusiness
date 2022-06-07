using UnityEngine.AddressableAssets;

namespace SolastaModApi.Extensions;

public static class ConditionDefinitionExtensions
{
#if false
        public static ConditionDefinition ClearParticleReferences(this ConditionDefinition definition)
        {
            var assetReference = new AssetReference();

            definition.conditionStartParticleReference = assetReference;
            definition.conditionParticleReference = assetReference;
            definition.conditionEndParticleReference = assetReference;
            definition.characterShaderReference = assetReference;

            return definition;
        }
#endif

    public static ConditionDefinition SetEmptyParticleReferencesWhereNull(this ConditionDefinition definition)
    {
        var assetReference = new AssetReference();

        if (definition.conditionStartParticleReference == null)
        {
            definition.conditionStartParticleReference = assetReference;
        }

        if (definition.conditionParticleReference == null)
        {
            definition.conditionParticleReference = assetReference;
        }

        if (definition.conditionEndParticleReference == null)
        {
            definition.conditionEndParticleReference = assetReference;
        }

        if (definition.characterShaderReference == null)
        {
            definition.characterShaderReference = assetReference;
        }

        return definition;
    }
}
