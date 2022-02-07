using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
    public class EffectProxyDefinitionBuilder : BaseDefinitionBuilder<EffectProxyDefinition>
    {
        public EffectProxyDefinitionBuilder(string name, string guid) : base(name, guid)
        {
        }

        public EffectProxyDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        public EffectProxyDefinitionBuilder(EffectProxyDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public EffectProxyDefinitionBuilder(EffectProxyDefinition original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        public EffectProxyDefinitionBuilder SetAttack(bool canAttack, RuleDefinitions.ProxyAttackMethod attackMethod,
            RuleDefinitions.DieType damageDie, string damageType)
        {
            Definition.SetCanAttack(canAttack);
            Definition.SetAttackMethod(attackMethod);
            Definition.SetDamageDie(damageDie);
            Definition.SetDamageType(damageType);
            return this;
        }

        public EffectProxyDefinitionBuilder SetLightSource(LightSourceForm lightSourceForm)
        {
            Definition.SetAddLightSource(true);
            Definition.SetLightSourceForm(lightSourceForm);
            return this;
        }

        public EffectProxyDefinitionBuilder SetPortrait(AssetReferenceSprite portraitSpriteReference)
        {
            Definition.SetHasPortrait(true);
            Definition.SetPortraitSpriteReference(portraitSpriteReference);
            return this;
        }

        public EffectProxyDefinitionBuilder AddAdditionalFeature(FeatureDefinition feature)
        {
            Definition.AdditionalFeatures.Add(feature);
            return this;
        }

        public EffectProxyDefinitionBuilder SetShowWorldLocationFeedbacks()
        {
            Definition.SetShowWorldLocationFeedbacks(true);
            return this;
        }

        public EffectProxyDefinitionBuilder SetAttackParticle(AssetReference attackParticle)
        {
            Definition.SetAttackParticle(attackParticle);
            return this;
        }

        public EffectProxyDefinitionBuilder SetAttackImpactParticle(AssetReference attackImpactParticle)
        {
            Definition.SetAttackImpactParticle(attackImpactParticle);
            return this;
        }

        public EffectProxyDefinitionBuilder SetSoundEffectOnHitDescription(SoundEffectOnHitDescription soundEffectOnHitDescription)
        {
            Definition.SetSoundEffectOnHitDescription(soundEffectOnHitDescription);
            return this;
        }

        public EffectProxyDefinitionBuilder SetCanMove()
        {
            Definition.SetCanMove(true);
            return this;
        }

        public EffectProxyDefinitionBuilder SetImpactsPlacement()
        {
            Definition.SetImpactsPlacement(true);
            return this;
        }

        public EffectProxyDefinitionBuilder SetModelScale(float modelScale)
        {
            Definition.SetModelScale(modelScale);
            return this;
        }

        public EffectProxyDefinitionBuilder SetPresentationInformation(bool hasPresentation, AssetReference prefabReference, bool isEmptyPresentation)
        {
            Definition.SetHasPresentation(hasPresentation);
            Definition.SetPrefabReference(prefabReference);
            Definition.SetIsEmptyPresentation(isEmptyPresentation);
            return this;
        }
    }
}
