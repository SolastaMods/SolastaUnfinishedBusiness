using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
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
        Definition.isEmptyPresentation = false;
        Definition.portraitSpriteReference = portraitSpriteReference;
        return this;
    }

    internal EffectProxyDefinitionBuilder SetActionId(
        ExtraActionId actionId, ExtraActionId freeActionId = (ExtraActionId)ActionDefinitions.Id.NoAction)
    {
        Definition.actionId = (ActionDefinitions.Id)actionId;
        Definition.freeActionId = (ActionDefinitions.Id)freeActionId;
        return this;
    }

    internal EffectProxyDefinitionBuilder SetAttackMethod(
        ProxyAttackMethod proxyAttackMethod,
        string damageType = DamageTypeRadiant,
        DieType damageDie = DieType.D8,
        int damageDieNum = 1,
        bool addAbilityToDamage = false)
    {
        Definition.attackMethod = proxyAttackMethod;
        Definition.damageDie = damageDie;
        Definition.damageDieNum = damageDieNum;
        Definition.damageType = damageType;
        Definition.addAbilityToDamage = addAbilityToDamage;
        return this;
    }

    internal EffectProxyDefinitionBuilder SetAdditionalFeatures(params FeatureDefinition[] features)
    {
        Definition.AdditionalFeatures.SetRange(features);
        return this;
    }

    internal EffectProxyDefinitionBuilder SetCanMove(bool canMove = true, bool canMoveOnCharacters = true)
    {
        Definition.canMove = canMove;
        Definition.canMoveOnCharacters = canMoveOnCharacters;
        return this;
    }
}
