using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
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
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.EldritchSpear)
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
            .SetGuiPresentation(Category.Invocation, SpellDefinitions.Invisibility)
            .SetRequirements(15)
            .SetGrantedSpell(SpellDefinitions.Invisibility)
            .AddToDB();
    }

    internal static InvocationDefinition BuildUndyingServitude()
    {
        const string NAME = "InvocationUndyingServitude";

        var spell = DatabaseRepository.GetDatabase<SpellDefinition>()
            .GetElement("CreateDeadRisenSkeleton_Enforcer");

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(
                GuiPresentationBuilder.CreateTitleKey(NAME, Category.Invocation),
                Gui.Format(GuiPresentationBuilder.CreateDescriptionKey(NAME, Category.Invocation), spell.FormatTitle()),
                spell
            )
            .SetRequirements(5)
            .SetGrantedSpell(spell, false, true)
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
            .SetGuiPresentation(Category.Invocation, spellTrickstersEscape)
            .SetRequirements(7)
            .SetGrantedSpell(spellTrickstersEscape, false, true)
            .AddToDB();
    }

    internal static InvocationDefinition BuildEldritchMind()
    {
        const string NAME = "InvocationEldritchMind";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.EldritchSpear)
            .SetGrantedFeature(
                FeatureDefinitionMagicAffinityBuilder
                    .Create("MagicAffinityInvocationEldritchMind")
                    .SetGuiPresentation(NAME, Category.Invocation)
                    .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildGraspingBlast()
    {
        const string NAME = "InvocationGraspingBlast";

        var powerInvocationGraspingBlast = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerInvocationRepellingBlast, "PowerInvocationGraspingBlast")
            .SetGuiPresentation(NAME, Category.Invocation)
            .AddToDB();

        powerInvocationGraspingBlast.EffectDescription.effectForms.SetRange(EffectFormBuilder.Create()
            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
            .Build());

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast)
            .SetGrantedFeature(powerInvocationGraspingBlast)
            .AddToDB();
    }

    internal static InvocationDefinition BuildHinderingBlast()
    {
        const string NAME = "InvocationHinderingBlast";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast)
            .SetGrantedFeature(FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamage{NAME}")
                .SetGuiPresentationNoContent(true)
                .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamagesTarget)
                .SetRequiredSpecificSpell(SpellDefinitions.EldritchBlast)
                .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add,
                    ConditionDefinitions.ConditionHindered_By_Frost)
                .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildGiftOfTheEverLivingOnes()
    {
        const string NAME = "InvocationGiftOfTheEverLivingOnes";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.EldritchSpear)
            .SetGrantedFeature(FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope)
            .AddToDB();
    }

    internal static InvocationDefinition BuildGiftOfTheProtectors()
    {
        const string NAME = "InvocationGiftOfTheProtectors";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, DamageAffinityHalfOrcRelentlessEndurance)
            .SetRequirements(9, pact: FeatureSetPactTome)
            .SetGrantedFeature(
                FeatureDefinitionDamageAffinityBuilder
                    .Create(
                        DamageAffinityHalfOrcRelentlessEndurance,
                        "DamageAffinityInvocationGiftOfTheProtectorsRelentlessEndurance")
                    .SetGuiPresentation(NAME, Category.Invocation)
                    .AddToDB())
            .AddToDB();
    }

    //TODO: how can I make this be 1 fixed used per long rest?
    internal static InvocationDefinition BuildBondOfTheTalisman()
    {
        const string NAME = "InvocationBondOfTheTalisman";

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.OneWithShadowsTurnInvisible, $"ActionDefinition{NAME}")
            //TODO: need a better icon here
            .SetGuiPresentation(NAME, Category.Invocation, DatabaseHelper.ActionDefinitions.Sunbeam)
            .SetActionId(ExtraActionId.BondOfTheTalismanTeleport)
            .OverrideClassName("UsePower")
            .SetActionScope(ActionDefinitions.ActionScope.All)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetFormType(ActionDefinitions.ActionFormType.Large)
            .SetActivatedPower(FeatureDefinitionPowerBuilder
                .Create(FeatureDefinitionPowers.PowerSorakShadowEscape, $"Power{NAME}")
                //.SetUsesFixed(RuleDefinitions.ActivationTime.Action, RuleDefinitions.RechargeRate.LongRest)
                .SetGuiPresentationNoContent(true)
                .AddToDB())
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.OneWithShadows, NAME)
            .SetGuiPresentation(Category.Invocation, FeatureDefinitionPowers.PowerSorakShadowEscape)
            .SetRequirements(12)
            .SetGrantedFeature(
                FeatureDefinitionActionAffinityBuilder
                    .Create(FeatureDefinitionActionAffinitys.ActionAffinityInvocationOneWithShadowsTurnInvisible,
                        $"ActionAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Invocation)
                    .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.BondOfTheTalismanTeleport)
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildAspectOfTheMoon()
    {
        const string NAME = "InvocationAspectOfTheMoon";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation,
                FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest)
            .SetGrantedFeature(
                FeatureDefinitionFeatureSetBuilder
                    .Create("FeatureSetInvocationAspectOfTheMoon")
                    .SetGuiPresentation(NAME, Category.Invocation)
                    .SetCustomSubFeatures(
                        FeatureDefinitionCampAffinityBuilder
                            .Create(
                                FeatureDefinitionCampAffinitys.CampAffinityElfTrance,
                                "CampAffinityInvocationAspectOfTheMoonTrance")
                            .SetGuiPresentation(NAME, Category.Invocation)
                            .AddToDB(),
                        FeatureDefinitionCampAffinityBuilder
                            .Create(
                                FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest,
                                "CampAffinityInvocationAspectOfTheMoonRest")
                            .SetGuiPresentation(NAME, Category.Invocation)
                            .AddToDB())
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildImprovedPactWeapon()
    {
        const string NAME = "InvocationImprovedPactWeapon";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray)
            .SetRequirements(pact: FeatureSetPactBlade)
            .SetGrantedFeature(
                FeatureDefinitionFeatureSetBuilder
                    .Create("FeatureSetInvocationImprovedPactWeapon")
                    .SetGuiPresentation(NAME, Category.Invocation)
                    .SetCustomSubFeatures(
                        FeatureDefinitionAttackModifierBuilder
                            .Create(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
                                "AttackModifierInvocationImprovedPactWeaponPlus1")
                            .AddToDB(),
                        FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray)
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildSuperiorPactWeapon()
    {
        const string NAME = "InvocationSuperiorPactWeapon";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray)
            .SetRequirements(9, pact: FeatureSetPactBlade)
            .SetGrantedFeature(
                FeatureDefinitionFeatureSetBuilder
                    .Create("FeatureSetInvocationSuperiorPactWeapon")
                    .SetGuiPresentation(NAME, Category.Invocation)
                    .SetCustomSubFeatures(
                        FeatureDefinitionAttackModifierBuilder
                            .Create(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
                                "AttackModifierInvocationSuperiorPactWeaponPlus2")
                            .SetAttackRollModifier(2)
                            .AddToDB(),
                        FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray)
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildUltimatePactWeapon()
    {
        const string NAME = "InvocationUltimatePactWeapon";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray)
            .SetRequirements(15, pact: FeatureSetPactBlade)
            .SetGrantedFeature(
                FeatureDefinitionFeatureSetBuilder
                    .Create("FeatureSetInvocationUltimatePactWeapon")
                    .SetGuiPresentation(NAME, Category.Invocation)
                    .SetCustomSubFeatures(
                        FeatureDefinitionAttackModifierBuilder
                            .Create(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
                                "AttackModifierInvocationUltimatePactWeaponPlus3")
                            .SetAttackRollModifier(3)
                            .AddToDB(),
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
