using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.CustomBuilders;

internal static class InvocationsBuilders
{
    internal const string EldritchSmiteTag = "EldritchSmite";

    internal static InvocationDefinition BuildEldritchSmite()
    {
        return InvocationDefinitionBuilder
            .Create("InvocationEldritchSmite")
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.EldritchSpear)
            .SetRequirements(5, pact: FeatureSetPactBlade)
            .SetGrantedFeature(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageInvocationEldritchSmite")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag(EldritchSmiteTag)
                    .SetTriggerCondition(AdditionalDamageTriggerCondition.SpendSpellSlot)
                    .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
                    .SetAttackModeOnly()
                    .SetDamageDice(DieType.D8, 0)
                    .SetSpecificDamageType(DamageTypeForce)
                    .SetAdvancement(AdditionalDamageAdvancement.SlotLevel, 2)
                    .SetImpactParticleReference(EldritchBlast)
                    .AddCustomSubFeatures(
                        WarlockHolder.Instance,
                        new AdditionalEffectFormOnDamageHandler(HandleEldritchSmiteKnockProne))
                    .AddToDB())
            .AddToDB();
    }

    private static IEnumerable<EffectForm> HandleEldritchSmiteKnockProne(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        IAdditionalDamageProvider provider)
    {
        var rulesetDefender = defender.RulesetCharacter;

        if (rulesetDefender is null || rulesetDefender.SizeDefinition.WieldingSize > CreatureSize.Huge)
        {
            return null;
        }

        rulesetDefender.LogCharacterAffectedByCondition(ConditionDefinitions.ConditionProne);
        return new[]
        {
            EffectFormBuilder.Create()
                .SetMotionForm(MotionForm.MotionType.FallProne)
                .Build()
        };
    }


    internal static InvocationDefinition BuildShroudOfShadow()
    {
        const string NAME = "InvocationShroudOfShadow";

        // cast Invisibility at will
        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, Invisibility)
            .SetRequirements(15)
            .SetGrantedSpell(Invisibility)
            .AddToDB();
    }

    internal static InvocationDefinition BuildBreathOfTheNight()
    {
        const string NAME = "InvocationBreathOfTheNight";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, FogCloud)
            .SetGrantedSpell(FogCloud)
            .AddToDB();
    }

    internal static InvocationDefinition BuildVerdantArmor()
    {
        const string NAME = "InvocationVerdantArmor";

        var barkskinNoConcentration = SpellDefinitionBuilder
            .Create(Barkskin, "BarkskinNoConcentration")
            .SetRequiresConcentration(false)
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, barkskinNoConcentration)
            .SetRequirements(5)
            .SetGrantedSpell(barkskinNoConcentration)
            .AddToDB();
    }

    internal static InvocationDefinition BuildCallOfTheBeast()
    {
        const string NAME = "InvocationCallOfTheBeast";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, ConjureAnimals)
            .SetGrantedSpell(ConjureAnimals, true, true)
            .SetRequirements(5)
            .AddToDB();
    }

    internal static InvocationDefinition BuildTenaciousPlague()
    {
        const string NAME = "InvocationTenaciousPlague";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, InsectPlague)
            .SetGrantedSpell(InsectPlague, true, true)
            .SetRequirements(9, pact: FeatureSetPactChain)
            .AddToDB();
    }

    internal static InvocationDefinition BuildUndyingServitude()
    {
        const string NAME = "InvocationUndyingServitude";

        var spell = GetDefinition<SpellDefinition>("CreateDeadRisenSkeleton_Enforcer");

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(
                GuiPresentationBuilder.CreateTitleKey(NAME, Category.Invocation),
                Gui.Format(GuiPresentationBuilder.CreateDescriptionKey(NAME, Category.Invocation), spell.FormatTitle()),
                spell)
            .SetRequirements(5)
            .SetGrantedSpell(spell, false, true)
            .AddToDB();
    }

    internal static InvocationDefinition BuildTrickstersEscape()
    {
        const string NAME = "InvocationTrickstersEscape";

        var spellTrickstersEscape = SpellDefinitionBuilder
            .Create(FreedomOfMovement, "TrickstersEscape")
            .AddToDB();

        spellTrickstersEscape.EffectDescription.targetType = TargetType.Self;

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
                    .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 0)
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

        powerInvocationGraspingBlast.EffectDescription.effectForms.SetRange(
            EffectFormBuilder
                .Create()
                .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                .Build());

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast)
            .SetGrantedFeature(powerInvocationGraspingBlast)
            .SetRequirements(spell: EldritchBlast)
            .AddToDB();
    }

    internal static InvocationDefinition BuildHinderingBlast()
    {
        const string NAME = "InvocationHinderingBlast";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast)
            .SetGrantedFeature(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create($"AdditionalDamage{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .SetTriggerCondition(AdditionalDamageTriggerCondition.SpellDamagesTarget)
                    .SetRequiredSpecificSpell(EldritchBlast)
                    .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add,
                        ConditionDefinitions.ConditionHindered_By_Frost)
                    .AddToDB())
            .SetRequirements(spell: EldritchBlast)
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

    internal static InvocationDefinition BuildBondOfTheTalisman()
    {
        const string NAME = "InvocationBondOfTheTalisman";

        var power = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerSorakShadowEscape, $"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Invocation, FeatureDefinitionPowers.PowerSorakShadowEscape)
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .DelegatedToAction()
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(FeatureDefinitionPowers.PowerSorakShadowEscape)
                    .UseQuickAnimations()
                    .Build())
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create($"ActionDefinition{NAME}")
            .SetGuiPresentation(NAME, Category.Invocation, Sprites.Teleport, 71)
            .SetActionId(ExtraActionId.BondOfTheTalismanTeleport)
            .RequiresAuthorization(false)
            .OverrideClassName("UsePower")
            .SetActionScope(ActionDefinitions.ActionScope.All)
            .SetActionType(ActionDefinitions.ActionType.Bonus)
            .SetFormType(ActionDefinitions.ActionFormType.Small)
            .SetActivatedPower(power)
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.OneWithShadows, NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.EldritchSpear)
            .SetRequirements(12)
            .SetGrantedFeature(power)
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
                    .AddFeatureSet(
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
        return BuildPactWeapon("InvocationImprovedPactWeapon", 5);
    }

    internal static InvocationDefinition BuildSuperiorPactWeapon()
    {
        return BuildPactWeapon("InvocationSuperiorPactWeapon", 9);
    }

    internal static InvocationDefinition BuildUltimatePactWeapon()
    {
        return BuildPactWeapon("InvocationUltimatePactWeapon", 15);
    }

    private static InvocationDefinition BuildPactWeapon(string name, int level)
    {
        return InvocationDefinitionBuilder
            .Create(name)
            .SetGuiPresentation(Category.Invocation, FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray)
            .SetRequirements(level, pact: FeatureSetPactBlade)
            .SetGrantedFeature(
                FeatureDefinitionAttackModifierBuilder
                    .Create($"AttackModifier{name}")
                    .SetGuiPresentation(name, Category.Invocation)
                    .SetAttackRollModifier(1)
                    .SetDamageRollModifier(1)
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildKinesis()
    {
        const string NAME = "InvocationKinesis";

        var spellKinesis = SpellDefinitionBuilder
            .Create(Haste, "Kinesis")
            .AddToDB();

        var effect = spellKinesis.EffectDescription;
        effect.targetFilteringMethod = TargetFilteringMethod.CharacterOnly;
        effect.targetExcludeCaster = true;
        effect.EffectForms.Add(EffectFormBuilder.ConditionForm(
            ConditionDefinitions.ConditionHasted,
            ConditionForm.ConditionOperation.Add, true));

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, spellKinesis)
            .SetRequirements(7)
            .SetGrantedSpell(spellKinesis, false, true)
            .AddToDB();
    }

    internal static InvocationDefinition BuildStasis()
    {
        const string NAME = "InvocationStasis";

        var spellStasis = SpellDefinitionBuilder
            .Create(Slow, "Stasis")
            .SetRequiresConcentration(false)
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, spellStasis)
            .SetRequirements(7)
            .SetGrantedSpell(spellStasis, false, true)
            .AddToDB();
    }

    internal static InvocationDefinition BuildChillingBlast()
    {
        const string NAME = "InvocationChillingBlast";

        var rayOfFrostParticles = RayOfFrost.EffectDescription.EffectParticleParameters;

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetOrUpdateGuiPresentation(Category.Invocation)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeCold,
                            rayOfFrostParticles.casterParticleReference,
                            rayOfFrostParticles.effectParticleReference,
                            rayOfFrostParticles.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildCorrosiveBlast()
    {
        const string NAME = "InvocationCorrosiveBlast";

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetOrUpdateGuiPresentation(Category.Invocation)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeAcid,
                            AcidSplash.EffectDescription.EffectParticleParameters.casterParticleReference,
                            AcidArrow.EffectDescription.EffectParticleParameters.casterParticleReference,
                            AcidArrow.EffectDescription.EffectParticleParameters.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildFieryBlast()
    {
        const string NAME = "InvocationFieryBlast";

        var fireBoltParticles = FireBolt.EffectDescription.EffectParticleParameters;

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetOrUpdateGuiPresentation(Category.Invocation)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeFire,
                            fireBoltParticles.casterParticleReference,
                            fireBoltParticles.effectParticleReference,
                            fireBoltParticles.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildFulminateBlast()
    {
        const string NAME = "InvocationFulminateBlast";

        var lightningBoltParticles = LightningBolt.EffectDescription.EffectParticleParameters;

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetOrUpdateGuiPresentation(Category.Invocation)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeLightning,
                            lightningBoltParticles.casterParticleReference,
                            lightningBoltParticles.effectParticleReference,
                            lightningBoltParticles.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildNecroticBlast()
    {
        const string NAME = "InvocationNecroticBlast";

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetOrUpdateGuiPresentation(Category.Invocation)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeNecrotic,
                            RayOfEnfeeblement.EffectDescription.EffectParticleParameters.casterParticleReference,
                            Disintegrate.EffectDescription.EffectParticleParameters.effectParticleReference,
                            ChillTouch.EffectDescription.EffectParticleParameters.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildPoisonousBlast()
    {
        const string NAME = "InvocationPoisonousBlast";

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetOrUpdateGuiPresentation(Category.Invocation)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypePoison,
                            PoisonSpray.EffectDescription.EffectParticleParameters.casterParticleReference,
                            AcidSplash.EffectDescription.EffectParticleParameters.effectParticleReference,
                            // it's indeed effect here
                            PoisonSpray.EffectDescription.EffectParticleParameters.effectParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildPsychicBlast()
    {
        const string NAME = "InvocationPsychicBlast";

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetOrUpdateGuiPresentation(Category.Invocation)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypePsychic,
                            Blur.EffectDescription.EffectParticleParameters.casterParticleReference,
                            ShadowDagger.EffectDescription.EffectParticleParameters.effectParticleReference,
                            ShadowDagger.EffectDescription.EffectParticleParameters.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildRadiantBlast()
    {
        const string NAME = "InvocationRadiantBlast";

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetOrUpdateGuiPresentation(Category.Invocation)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeRadiant,
                            BrandingSmite.EffectDescription.EffectParticleParameters.casterParticleReference,
                            GuidingBolt.EffectDescription.EffectParticleParameters.effectParticleReference,
                            BrandingSmite.EffectDescription.EffectParticleParameters.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildThunderBlast()
    {
        const string NAME = "InvocationThunderBlast";

        var thunderwaveParticles = Thunderwave.EffectDescription.EffectParticleParameters;

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetOrUpdateGuiPresentation(Category.Invocation)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeThunder,
                            thunderwaveParticles.casterParticleReference,
                            thunderwaveParticles.effectParticleReference,
                            thunderwaveParticles.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildSpectralShield()
    {
        const string NAME = "InvocationSpectralShield";

        var spellSpectralShield = SpellDefinitionBuilder
            .Create(ShieldOfFaith, "SpectralShield")
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, spellSpectralShield)
            .SetRequirements(9)
            .SetGrantedSpell(spellSpectralShield, false, true)
            .AddToDB();
    }

    internal static InvocationDefinition BuildGiftOfTheHunter()
    {
        const string NAME = "InvocationGiftOfTheHunter";

        var spellGiftOfTheHunter = SpellDefinitionBuilder
            .Create(PassWithoutTrace, "GiftOfTheHunter")
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, spellGiftOfTheHunter)
            .SetRequirements(5)
            .SetGrantedSpell(spellGiftOfTheHunter, false, true)
            .AddToDB();
    }


    internal static InvocationDefinition BuildDiscerningGaze()
    {
        const string NAME = "InvocationDiscerningGaze";

        var spellDiscerningGaze = SpellDefinitionBuilder
            .Create(DetectEvilAndGood, "DiscerningGaze")
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, spellDiscerningGaze)
            .SetRequirements(9)
            .SetGrantedSpell(spellDiscerningGaze, false, true)
            .AddToDB();
    }

    internal static InvocationDefinition BuildBreakerAndBanisher()
    {
        const string NAME = "InvocationBreakerAndBanisher";

        var spellBreakerAndBanisher = SpellDefinitionBuilder
            .Create(DispelEvilAndGood, "BreakerAndBanisher")
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, spellBreakerAndBanisher)
            .SetRequirements(9)
            .SetGrantedSpell(spellBreakerAndBanisher, true, true)
            .AddToDB();
    }

    internal static InvocationDefinition BuildAbilitiesOfTheChainMaster()
    {
        const string NAME = "InvocationAbilitiesOfTheChainMaster";

        var conditionAbilitySprite = ConditionDefinitionBuilder
            .Create("ConditionAbilitySprite")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainSprite)
            .AddFeatures(
                FeatureDefinitionAttributeModifiers.AttributeModifierBarkskin,
                FeatureDefinitionCombatAffinitys.CombatAffinityBlurred)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var conditionAbilityImp = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionInvisibleGreater, "ConditionAbilityImp")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainImp)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var conditionAbilityPseudo = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlying12, "ConditionAbilityPseudo")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainPseudodragon)
            .AddFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create(AdditionalDamagePoison_GhoulsCaress, "AdditionalDamagePseudoDragon")
                    .SetSavingThrowData(
                        EffectDifficultyClassComputation.SpellCastingFeature, EffectSavingThrowType.HalfDamage)
                    .SetDamageDice(DieType.D8, 1)
                    .SetNotificationTag("Poison")
                    .AddToDB())
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var conditionAbilityQuasit = ConditionDefinitionBuilder
            .Create("ConditionAbilityQuasit")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainQuasit)
            .AddFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create("AdditionalActionAbilityQuasit")
                    .SetGuiPresentationNoContent(true)
                    .SetActionType(ActionDefinitions.ActionType.Main)
                    .AddToDB(),
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionHasted)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var featureAbilitiesOfTheChainMaster = FeatureDefinitionBuilder
            .Create($"Feature{NAME}")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new AfterActionFinishedByMeAbilitiesChain(
                conditionAbilitySprite, conditionAbilityImp, conditionAbilityQuasit, conditionAbilityPseudo))
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.VoiceChainMaster, NAME)
            .SetOrUpdateGuiPresentation(Category.Invocation)
            .SetRequirements(7, pact: FeatureSetPactChain)
            .SetGrantedFeature(featureAbilitiesOfTheChainMaster)
            .AddToDB();
    }


    /*
     
    Celestial Blessing

        Prerequisites: Celestial Subclass, 9th level

        You can cast Bless as a 1st level spell at will without maintaining concentration. You can use this feature a number of times equal to your charisma modifier. You regain any extended uses after completing a long rest. 

    Ally of Nature

        Prerequisite: 9th level

        You can cast awaken once using a warlock spell slot. You can't do so again until you finish a long rest.
         
    Witching Blade

        Prerequisite: Pact of the Blade

        You can use your Charisma modifier instead of your Strength or Dexterity modifiers for attack and damage rolls made with your pact weapon.

    Witching Plate

        Prerequisite: Pact of the Blade

        As an action, you can conjure a suit of magical armor onto your body that grants you an AC equal to 14 + your Charisma modifier. (edited)
     */

    private sealed class WarlockHolder : IClassHoldingFeature
    {
        private WarlockHolder()
        {
        }

        public static IClassHoldingFeature Instance { get; } = new WarlockHolder();

        public CharacterClassDefinition Class => CharacterClassDefinitions.Warlock;
    }

    private sealed class ModifyEffectDescriptionEldritchBlast : IModifyEffectDescription
    {
        private readonly string _damageType;
        private readonly EffectParticleParameters _effectParticleParameters = new();

        public ModifyEffectDescriptionEldritchBlast(
            string damageType,
            AssetReference caster,
            AssetReference effect,
            AssetReference impact)
        {
            _damageType = damageType;
            _effectParticleParameters.casterParticleReference = caster;
            _effectParticleParameters.effectParticleReference = effect;
            _effectParticleParameters.impactParticleReference = impact;
        }

        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == EldritchBlast;
        }

        public EffectDescription GetEffectDescription(BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var damage = effectDescription.FindFirstDamageForm();

            damage.DamageType = _damageType;
            effectDescription.effectParticleParameters = _effectParticleParameters;

            return effectDescription;
        }
    }

    private sealed class AfterActionFinishedByMeAbilitiesChain : IActionFinishedByMe
    {
        private readonly ConditionDefinition _conditionImpAbility;

        private readonly ConditionDefinition _conditionPseudoAbility;

        private readonly ConditionDefinition _conditionQuasitAbility;
        private readonly ConditionDefinition _conditionSpriteAbility;

        internal AfterActionFinishedByMeAbilitiesChain(ConditionDefinition conditionSpriteAbility,
            ConditionDefinition conditionImpAbility,
            ConditionDefinition conditionQuasitAbility,
            ConditionDefinition conditionPseudoAbility)
        {
            _conditionSpriteAbility = conditionSpriteAbility;
            _conditionImpAbility = conditionImpAbility;
            _conditionQuasitAbility = conditionQuasitAbility;
            _conditionPseudoAbility = conditionPseudoAbility;
        }

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;

            if (actingCharacter.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (action.ActionType != ActionDefinitions.ActionType.Bonus &&
                //action.ActingCharacter.PerceptionState == ActionDefinitions.PerceptionState.OnGuard
                action.ActionDefinition.ActionScope == ActionDefinitions.ActionScope.Battle)
            {
                yield break;
            }

            var rulesetCharacter = actingCharacter.RulesetCharacter;

            foreach (var power in rulesetCharacter.usablePowers
                         .ToList()) // required ToList() to avoid list was changed when Far Step in play
            {
                if (rulesetCharacter.IsPowerActive(power))
                {
                    if (power.PowerDefinition == FeatureDefinitionPowers.PowerPactChainImp &&
                        !rulesetCharacter.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect,
                            _conditionImpAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, _conditionImpAbility);
                    }
                    else if (power.PowerDefinition == FeatureDefinitionPowers.PowerPactChainQuasit &&
                             !rulesetCharacter.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect,
                                 _conditionQuasitAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, _conditionQuasitAbility);
                    }
                    else if (power.PowerDefinition == FeatureDefinitionPowers.PowerPactChainSprite &&
                             !rulesetCharacter.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect,
                                 _conditionSpriteAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, _conditionSpriteAbility);
                    }
                    else if (power.PowerDefinition == FeatureDefinitionPowers.PowerPactChainPseudodragon &&
                             !rulesetCharacter.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect,
                                 _conditionPseudoAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, _conditionPseudoAbility);
                    }
                }
                else
                {
                    if (power.PowerDefinition == FeatureDefinitionPowers.PowerPactChainImp)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            _conditionImpAbility.name);
                    }
                    else if (power.PowerDefinition == FeatureDefinitionPowers.PowerPactChainQuasit)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            _conditionQuasitAbility.name);
                    }
                    else if (power.PowerDefinition == FeatureDefinitionPowers.PowerPactChainSprite)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            _conditionSpriteAbility.name);
                    }
                    else if (power.PowerDefinition == FeatureDefinitionPowers.PowerPactChainPseudodragon)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            _conditionPseudoAbility.name);
                    }
                }
            }
        }

        private static void SetChainBuff(RulesetCharacter rulesetCharacter, BaseDefinition conditionDefinition)
        {
            rulesetCharacter.InflictCondition(
                conditionDefinition.Name,
                DurationType.Minute,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }
}
