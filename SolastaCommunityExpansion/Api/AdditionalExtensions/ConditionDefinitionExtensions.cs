using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaModApi.Extensions
{
    public static partial class ConditionDefinitionExtensions
    {
#if false
        public static ConditionDefinition ClearParticleReferences(this ConditionDefinition definition)
        {
            var assetReference = new AssetReference();

            definition.SetConditionStartParticleReference(assetReference);
            definition.SetConditionParticleReference(assetReference);
            definition.SetConditionEndParticleReference(assetReference);
            definition.SetCharacterShaderReference(assetReference);

            return definition;
        }
#endif

        public static ConditionDefinition SetEmptyParticleReferencesWhereNull(this ConditionDefinition definition)
        {
            var assetReference = new AssetReference();

            if (definition.conditionStartParticleReference == null)
            {
                definition.SetConditionStartParticleReference(assetReference);
            }

            if (definition.conditionParticleReference == null)
            {
                definition.SetConditionParticleReference(assetReference);
            }

            if (definition.conditionEndParticleReference == null)
            {
                definition.SetConditionEndParticleReference(assetReference);
            }

            if (definition.characterShaderReference == null)
            {
                definition.SetCharacterShaderReference(assetReference);
            }

            return definition;
        }
    }
}
