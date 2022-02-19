using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
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

        public static EffectProxyDefinitionBuilder Create(string name, string guid)
        {
            return new EffectProxyDefinitionBuilder(name, guid);
        }

        public static EffectProxyDefinitionBuilder Create(string name, Guid namespaceGuid)
        {
            return new EffectProxyDefinitionBuilder(name, namespaceGuid);
        }

        public static EffectProxyDefinitionBuilder Create(EffectProxyDefinition original, string name, string guid)
        {
            return new EffectProxyDefinitionBuilder(original, name, guid);
        }

        public static EffectProxyDefinitionBuilder Create(EffectProxyDefinition original, string name, Guid namespaceGuid)
        {
            return new EffectProxyDefinitionBuilder(original, name, namespaceGuid);
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

        public EffectProxyDefinitionBuilder AddAdditionalFeatures(params FeatureDefinition[] features)
        {
            return AddAdditionalFeatures(features.AsEnumerable());
        }

        public EffectProxyDefinitionBuilder AddAdditionalFeatures(IEnumerable<FeatureDefinition> features)
        {
            Definition.AddAdditionalFeatures(features);
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
