using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class EffectProxyDefinitionBuilder : DefinitionBuilder<EffectProxyDefinition, EffectProxyDefinitionBuilder>
{
    protected EffectProxyDefinitionBuilder(string name, string guid) : base(name, guid)
    {
    }

    protected EffectProxyDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected EffectProxyDefinitionBuilder(EffectProxyDefinition original, string name, string guid)
        : base(original, name, guid)
    {
    }

    protected EffectProxyDefinitionBuilder(EffectProxyDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    public EffectProxyDefinitionBuilder SetPortrait(AssetReferenceSprite portraitSpriteReference)
    {
        Definition.hasPortrait = true;
        Definition.portraitSpriteReference = portraitSpriteReference;
        return this;
    }

    public EffectProxyDefinitionBuilder AddAdditionalFeatures(params FeatureDefinition[] features)
    {
        return AddAdditionalFeatures(features.AsEnumerable());
    }

    public EffectProxyDefinitionBuilder AddAdditionalFeatures(IEnumerable<FeatureDefinition> features)
    {
        Definition.AdditionalFeatures.AddRange(features);
        return this;
    }

    public EffectProxyDefinitionBuilder SetIsEmptyPresentation(bool value)
    {
        Definition.isEmptyPresentation = value;
        return this;
    }
    
    public EffectProxyDefinitionBuilder SetCanMove()
    {
        Definition.canMove = true;
        return this;
    }
    
    public EffectProxyDefinitionBuilder SetCanMoveOnCharacters()
    {
        Definition.canMoveOnCharacters = true;
        return this;
    }
    
    public EffectProxyDefinitionBuilder SetActionId(ActionDefinitions.Id actionId)
    {
        Definition.actionId = actionId;
        return this;
    }
    
    public EffectProxyDefinitionBuilder SetAttackMethod(RuleDefinitions.ProxyAttackMethod proxyAttackMethod)
    {
        Definition.attackMethod = proxyAttackMethod;
        return this;
    }
}
