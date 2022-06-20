using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders;

public class EffectProxyDefinitionBuilder : DefinitionBuilder<EffectProxyDefinition, EffectProxyDefinitionBuilder>
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

#if false
    public EffectProxyDefinitionBuilder SetAttack(bool canAttack, RuleDefinitions.ProxyAttackMethod attackMethod,
        RuleDefinitions.DieType damageDie, string damageType)
    {
        Definition.canAttack = canAttack;
        Definition.attackMethod = attackMethod;
        Definition.damageDie = damageDie;
        Definition.damageType = damageType;
        return this;
    }

    public EffectProxyDefinitionBuilder SetLightSource(LightSourceForm lightSourceForm)
    {
        Definition.addLightSource = true;
        Definition.lightSourceForm = lightSourceForm;
        return this;
    }
#endif

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

#if false
    public EffectProxyDefinitionBuilder SetShowWorldLocationFeedbacks()
    {
        Definition.showWorldLocationFeedbacks = true;
        return this;
    }

    public EffectProxyDefinitionBuilder SetAttackParticle(AssetReference attackParticle)
    {
        Definition.attackParticle = attackParticle;
        return this;
    }

    public EffectProxyDefinitionBuilder SetAttackImpactParticle(AssetReference attackImpactParticle)
    {
        Definition.attackImpactParticle = attackImpactParticle;
        return this;
    }

    public EffectProxyDefinitionBuilder SetSoundEffectOnHitDescription(
        SoundEffectOnHitDescription soundEffectOnHitDescription)
    {
        Definition.soundEffectOnHitDescription = soundEffectOnHitDescription;
        return this;
    }
#endif

    public EffectProxyDefinitionBuilder SetCanMove()
    {
        Definition.canMove = true;
        return this;
    }

#if false
    public EffectProxyDefinitionBuilder SetImpactsPlacement()
    {
        Definition.impactsPlacement = true;
        return this;
    }

    public EffectProxyDefinitionBuilder SetModelScale(float modelScale)
    {
        Definition.modelScale = modelScale;
        return this;
    }

    public EffectProxyDefinitionBuilder SetPresentationInformation(bool hasPresentation,
        AssetReference prefabReference, bool isEmptyPresentation)
    {
        Definition.hasPresentation = hasPresentation;
        Definition.prefabReference = prefabReference;
        Definition.isEmptyPresentation = isEmptyPresentation;
        return this;
    }
#endif
}
