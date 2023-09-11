using System;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class EffectProxyDefinitionBuilder : DefinitionBuilder<EffectProxyDefinition, EffectProxyDefinitionBuilder>
{
    protected EffectProxyDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected EffectProxyDefinitionBuilder(EffectProxyDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    internal EffectProxyDefinitionBuilder SetPortrait(AssetReferenceSprite portraitSpriteReference)
    {
        Definition.hasPortrait = true;
        Definition.portraitSpriteReference = portraitSpriteReference;
        return this;
    }

    internal EffectProxyDefinitionBuilder AddAdditionalFeatures(params FeatureDefinition[] features)
    {
        Definition.AdditionalFeatures.AddRange(features);
        return this;
    }

    internal EffectProxyDefinitionBuilder SetIsEmptyPresentation(bool value)
    {
        Definition.isEmptyPresentation = value;
        return this;
    }

    internal EffectProxyDefinitionBuilder SetCanMove()
    {
        Definition.canMove = true;
        return this;
    }

    internal EffectProxyDefinitionBuilder SetCanMoveOnCharacters()
    {
        Definition.canMoveOnCharacters = true;
        return this;
    }

    internal EffectProxyDefinitionBuilder SetActionId(ActionDefinitions.Id actionId)
    {
        Definition.actionId = actionId;
        return this;
    }

    internal EffectProxyDefinitionBuilder SetAttackMethod(ProxyAttackMethod proxyAttackMethod)
    {
        Definition.attackMethod = proxyAttackMethod;
        return this;
    }
}
