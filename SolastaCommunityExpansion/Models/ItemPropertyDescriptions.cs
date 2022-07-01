using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static EquipmentDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;

namespace SolastaCommunityExpansion.Models;

public static class ItemPropertyDescriptions
{
    public static readonly ItemPropertyDescription WeaponPlus1 = BuildFrom(AttackModifierWeaponPlus1);
    public static readonly ItemPropertyDescription WeaponPlus1AttackOnly = BuildFrom(AttackModifierWeaponPlus1AT);

    public static readonly ItemPropertyDescription WeaponPlus2 = BuildFrom(AttackModifierWeaponPlus2);

    public static readonly ItemPropertyDescription WeaponPlus3 = BuildFrom(AttackModifierWeaponPlus3);

    public static readonly ItemPropertyDescription ForceImpactVFX =
        BuildFrom(BuildAttackVFXFromSpell(SpellDefinitions.MagicMissile));

    public static readonly ItemPropertyDescription LightningImpactVFX =
        BuildFrom(BuildAttackVFXFromSpell(SpellDefinitions.LightningBolt));

    public static readonly ItemPropertyDescription PsychicImpactVFX =
        BuildFrom(BuildAttackVFXFromSpell(SpellDefinitions.Fear));

    public static readonly ItemPropertyDescription ThunderImpactVFX =
        BuildFrom(BuildAttackVFXFromSpell(SpellDefinitions.Thunderwave));

    public static readonly ItemPropertyDescription AcidImpactVFX =
        BuildFrom(BuildAttackVFXFromSpell(SpellDefinitions.AcidSplash));

    private static ItemPropertyDescription BuildFrom(
        FeatureDefinition feature,
        bool appliesOnItemOnly = true,
        KnowledgeAffinity knowledgeAffinity = KnowledgeAffinity.InactiveAndHidden)
    {
        return ItemPropertyDescriptionBuilder.From(feature, appliesOnItemOnly, knowledgeAffinity).Build();
    }

    public static ItemPropertyDescription BuildFrom(
        ConditionDefinition conditione,
        bool appliesOnItemOnly = true,
        KnowledgeAffinity knowledgeAffinity = KnowledgeAffinity.ActiveAndHidden)
    {
        return ItemPropertyDescriptionBuilder.From(conditione, appliesOnItemOnly, knowledgeAffinity).Build();
    }


    private static FeatureDefinition BuildAttackVFXFromSpell(SpellDefinition spell)
    {
        return BuildAttackVFXFromEffect($"AttackImpact{spell.Name}SpellVFX", spell.EffectDescription);
    }

    private static FeatureDefinition BuildAttackVFXFromEffect(string name, EffectDescription effect)
    {
        return FeatureDefinitionAttackModifierBuilder
            .Create(name, DefinitionBuilder.CENamespaceGuid)
            .Configure()
            .SetImpactParticleReference(effect.EffectParticleParameters.impactParticleReference)
            .AddToDB();
    }
}
