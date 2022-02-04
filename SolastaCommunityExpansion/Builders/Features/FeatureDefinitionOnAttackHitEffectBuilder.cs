using System.Collections.Generic;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionOnAttackHitEffectBuilder : BaseDefinitionBuilder<FeatureDefinitionOnAttackHitEffect>
    {
        public FeatureDefinitionOnAttackHitEffectBuilder(string name, string guid,
            OnAttackHitDelegate onHit, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetOnAttackHitDelegate(onHit);
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
