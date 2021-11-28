using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Features
{
    public class ConditionDefinitionBuilder<TDefinition> : BaseDefinitionBuilder<TDefinition> where TDefinition : ConditionDefinition
    {
        public ConditionDefinitionBuilder(string name, string guid, Action<TDefinition> modifyDefinition = null) : base(name, guid)
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

            if (modifyDefinition != null)
            {
                modifyDefinition(Definition);
            }
        }

        public static TDefinition Build(string name, string guid, Action<TDefinition> modifyDefinition = null)
        {
            var conditionDefinitionBuilder = new ConditionDefinitionBuilder<TDefinition>(name, guid, modifyDefinition);

            return conditionDefinitionBuilder.AddToDB();
        }
    }

    public class ConditionDefinitionBuilder : ConditionDefinitionBuilder<ConditionDefinition>
    {
        public ConditionDefinitionBuilder(string name, string guid, Action<ConditionDefinition> modifyDefinition = null) : base(name, guid, modifyDefinition)
        {
        }

        public ConditionDefinitionBuilder(string name, string guid, List<FeatureDefinition> conditionFeatures, RuleDefinitions.DurationType durationType,
        int durationParameter, bool silent, GuiPresentation guiPresentation) : base(name, guid)
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
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
