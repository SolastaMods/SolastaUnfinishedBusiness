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

            if (definition.GetField<AssetReference>("conditionStartParticleReference") == null)
            {
                definition.SetConditionStartParticleReference(assetReference);
            }

            if (definition.GetField<AssetReference>("conditionParticleReference") == null)
            {
                definition.SetConditionParticleReference(assetReference);
            }

            if (definition.GetField<AssetReference>("conditionEndParticleReference") == null)
            {
                definition.SetConditionEndParticleReference(assetReference);
            }

            if (definition.GetField<AssetReference>("characterShaderReference") == null)
            {
                definition.SetCharacterShaderReference(assetReference);
            }

            return definition;
        }
    }
}
