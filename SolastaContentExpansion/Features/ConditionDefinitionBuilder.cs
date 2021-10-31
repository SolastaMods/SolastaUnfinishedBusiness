using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace SolastaContentExpansion.Features
{
    public class ConditionDefinitionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        public ConditionDefinitionBuilder(string name, string guid, List<FeatureDefinition> conditionFeatures, RuleDefinitions.DurationType durationType,
        int durationParameter, bool silent, GuiPresentation guiPresentation) : base(name, guid)
        {
            foreach (FeatureDefinition feature in conditionFeatures)
            {
                Definition.Features.Add(feature);
            }
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
            Traverse.Create(Definition).Field("recurrentEffectForms").SetValue(new List<EffectForm>());
            Traverse.Create(Definition).Field("cancellingConditions").SetValue(new List<ConditionDefinition>());
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
