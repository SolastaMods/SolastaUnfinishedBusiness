using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

namespace SolastaUnfinishedBusiness.Invocations;

internal static class InvocationsBuilders
{
    internal const string EldritchSmiteTag = "EldritchSmite";

    internal static InvocationDefinition BuildEldritchSmite()
    {
        return InvocationDefinitionBuilder
            .Create("InvocationEldritchSmite")
            .SetGuiPresentation(Category.Feature, InvocationDefinitions.EldritchSpear)
            .SetRequirements(5, pact: FeatureSetPactBlade)
            .SetGrantedFeature(FeatureDefinitionAdditionalDamageBuilder
                .Create("AdditionalDamageInvocationEldritchSmite")
                .SetGuiPresentationNoContent()
                .SetCustomSubFeatures(WarlockHolder.Instance)
                .SetNotificationTag(EldritchSmiteTag)
                .SetDamageDice(RuleDefinitions.DieType.D8, 0)
                .SetSpecificDamageType(RuleDefinitions.DamageTypeForce)
                .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot)
                .SetAttackModeOnly()
                .SetImpactParticleReference(SpellDefinitions.EldritchBlast)
                .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.OncePerTurn)
                .SetAdvancement(RuleDefinitions.AdditionalDamageAdvancement.SlotLevel, 2)
                .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildShroudOfShadow()
    {
        const string NAME = "InvocationShroudOfShadow";

        // cast Invisibility at will
        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility)
            .SetRequirements(15)
            .SetGrantedSpell(SpellDefinitions.Invisibility)
            .AddToDB();
    }

    internal static InvocationDefinition BuildTrickstersEscape()
    {
        const string NAME = "InvocationTrickstersEscape";

        var spellTrickstersEscape = SpellDefinitionBuilder
            .Create(SpellDefinitions.FreedomOfMovement, "TrickstersEscape")
            .AddToDB();

        spellTrickstersEscape.EffectDescription.TargetType = RuleDefinitions.TargetType.Self;

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature, spellTrickstersEscape)
            .SetRequirements(7)
            .SetGrantedSpell(spellTrickstersEscape, false, true)
            .AddToDB();
    }

    internal static InvocationDefinition BuildEldritchMind()
    {
        const string NAME = "InvocationEldritchMind";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature, InvocationDefinitions.EldritchSpear)
            .SetGrantedFeature(
                FeatureDefinitionMagicAffinityBuilder
                    .Create("MagicAffinityInvocationEldritchMind")
                    .SetGuiPresentation(NAME, Category.Feature)
                    .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildGiftOfTheEverLivingOnes()
    {
        const string NAME = "InvocationGiftOfTheEverLivingOnes";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature, InvocationDefinitions.EldritchSpear)
            .SetGrantedFeature(FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope)
            .AddToDB();
    }

    internal static InvocationDefinition BuildGiftOfTheProtectors()
    {
        const string NAME = "InvocationGiftOfTheProtectors";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature, DamageAffinityHalfOrcRelentlessEndurance)
            .SetRequirements(9, pact: FeatureSetPactTome)
            .SetGrantedFeature(
                FeatureDefinitionDamageAffinityBuilder
                    .Create(
                        DamageAffinityHalfOrcRelentlessEndurance,
                        "DamageAffinityInvocationGiftOfTheProtectorsRelentlessEndurance")
                    .SetGuiPresentation(NAME, Category.Feature)
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildBondOfTheTalisman()
    {
        const string NAME = "InvocationBondOfTheTalisman";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerSorakShadowEscape)
            .SetRequirements(12)
            .SetGrantedFeature(
                FeatureDefinitionPowerBuilder
                    .Create(
                        FeatureDefinitionPowers.PowerSorakShadowEscape,
                        "PowerInvocationBondOfTheTalismanShadowEscape")
                    .SetGuiPresentation(NAME, Category.Feature)
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildAspectOfTheMoon()
    {
        const string NAME = "InvocationAspectOfTheMoon";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature, FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest)
            .SetGrantedFeature(
                FeatureDefinitionFeatureSetBuilder
                    .Create("FeatureSetInvocationAspectOfTheMoon")
                    .SetGuiPresentation(NAME, Category.Feature)
                    .SetCustomSubFeatures(
                        FeatureDefinitionCampAffinityBuilder
                            .Create(
                                FeatureDefinitionCampAffinitys.CampAffinityElfTrance,
                                "CampAffinityInvocationAspectOfTheMoonTrance")
                            .SetGuiPresentation(NAME, Category.Feature)
                            .AddToDB(),
                        FeatureDefinitionCampAffinityBuilder
                            .Create(
                                FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest,
                                "CampAffinityInvocationAspectOfTheMoonRest")
                            .SetGuiPresentation(NAME, Category.Feature)
                            .AddToDB())
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildImprovedPactWeapon()
    {
        const string NAME = "InvocationImprovedPactWeapon";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature, FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray)
            .SetRequirements(pact: FeatureSetPactBlade)
            .SetGrantedFeature(
                FeatureDefinitionFeatureSetBuilder
                    .Create("FeatureSetInvocationImprovedPactWeapon")
                    .SetGuiPresentation(NAME, Category.Feature)
                    .SetCustomSubFeatures(
                        FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
                        FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray)
                    .AddToDB())
            .AddToDB();
    }

    private class WarlockHolder : IClassHoldingFeature
    {
        private WarlockHolder()
        {
        }

        public static IClassHoldingFeature Instance { get; } = new WarlockHolder();

        public CharacterClassDefinition Class => CharacterClassDefinitions.Warlock;
    }
}
