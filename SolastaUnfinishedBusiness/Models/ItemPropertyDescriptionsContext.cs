using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static EquipmentDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;

namespace SolastaUnfinishedBusiness.Models;

internal static class ItemPropertyDescriptionsContext
{
    internal static readonly ItemPropertyDescription WeaponPlus1 = BuildFrom(AttackModifierWeaponPlus1);
    internal static readonly ItemPropertyDescription WeaponPlus1AttackOnly = BuildFrom(AttackModifierWeaponPlus1AT);
    internal static readonly ItemPropertyDescription WeaponPlus2 = BuildFrom(AttackModifierWeaponPlus2);
    internal static readonly ItemPropertyDescription WeaponPlus3 = BuildFrom(AttackModifierWeaponPlus3);

    internal static readonly ItemPropertyDescription ForceImpactVFX =
        BuildFrom(BuildAttackVFXFromSpell(SpellDefinitions.MagicMissile));

    internal static readonly ItemPropertyDescription LightningImpactVFX =
        BuildFrom(BuildAttackVFXFromSpell(SpellDefinitions.LightningBolt));

    internal static readonly ItemPropertyDescription PsychicImpactVFX =
        BuildFrom(BuildAttackVFXFromSpell(SpellDefinitions.Fear));

    internal static readonly ItemPropertyDescription ThunderImpactVFX =
        BuildFrom(BuildAttackVFXFromSpell(SpellDefinitions.Thunderwave));

    internal static readonly ItemPropertyDescription AcidImpactVFX =
        BuildFrom(BuildAttackVFXFromSpell(SpellDefinitions.AcidSplash));

    internal static ItemPropertyDescription BuildFrom(
        FeatureDefinition feature,
        bool appliesOnItemOnly = true,
        KnowledgeAffinity knowledgeAffinity = KnowledgeAffinity.ActiveAndHidden)
    {
        return ItemPropertyDescriptionBuilder.From(feature, appliesOnItemOnly, knowledgeAffinity).Build();
    }

#if false
    internal static ItemPropertyDescription BuildFrom(
        ConditionDefinition condition,
        bool appliesOnItemOnly = true,
        KnowledgeAffinity knowledgeAffinity = KnowledgeAffinity.ActiveAndHidden)
    {
        return ItemPropertyDescriptionBuilder.From(condition, appliesOnItemOnly, knowledgeAffinity).Build();
    }
#endif

    private static FeatureDefinitionAttackModifier BuildAttackVFXFromSpell([NotNull] SpellDefinition spell)
    {
        return BuildAttackVFXFromEffect($"AttackImpact{spell.Name}SpellVFX", spell.EffectDescription);
    }

    private static FeatureDefinitionAttackModifier BuildAttackVFXFromEffect(
        string name,
        [NotNull] EffectDescription effect)
    {
        return FeatureDefinitionAttackModifierBuilder
            .Create(name)
            .SetGuiPresentationNoContent(true)
            .SetImpactParticleReference(effect.EffectParticleParameters.impactParticleReference)
            .AddToDB();
    }
}
