using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses.Builders;

internal static class InvocationsBuilders
{
    internal const string EldritchSmiteTag = "EldritchSmite";

    private static InvocationDefinition _graspingBlast;

    internal static readonly InvocationDefinition EldritchMind = InvocationDefinitionBuilder
        .Create("InvocationEldritchMind")
        .SetGuiPresentation(Category.Invocation, InvocationDefinitions.EldritchSpear)
        .SetGrantedFeature(
            FeatureDefinitionMagicAffinityBuilder
                .Create("MagicAffinityInvocationEldritchMind")
                .SetGuiPresentation("InvocationEldritchMind", Category.Invocation)
                .SetConcentrationModifiers(ConcentrationAffinity.Advantage)
                .AddToDB())
        .AddToDB();

    internal static InvocationDefinition GraspingBlast => _graspingBlast ??= BuildGraspingBlast();

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
                        ClassHolder.Warlock,
                        new AdditionalEffectFormOnDamageHandler(HandleEldritchSmiteKnockProne))
                    .AddToDB())
            .AddToDB();
    }

    [CanBeNull]
    private static EffectForm[] HandleEldritchSmiteKnockProne(
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
        return
        [
            EffectFormBuilder.Create()
                .SetMotionForm(MotionForm.MotionType.FallProne)
                .Build()
        ];
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
            .SetGuiPresentation(Category.Invocation, FogCloud, hidden: true)
            .SetGrantedSpell(FogCloud)
            .AddToDB();
    }

    internal static InvocationDefinition BuildVerdantArmor()
    {
        const string NAME = "InvocationVerdantArmor";

        var spellVerdantArmor = SpellDefinitionBuilder
            .Create(Barkskin, "VerdantArmor")
            .SetRequiresConcentration(false)
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, Barkskin)
            .SetRequirements(5)
            .SetGrantedSpell(spellVerdantArmor)
            .AddToDB();
    }

    internal static InvocationDefinition BuildCallOfTheBeast()
    {
        const string NAME = "InvocationCallOfTheBeast";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, ConjureAnimals)
            .SetGrantedSpell(ConjureAnimals, false, true)
            .SetRequirements(5)
            .AddToDB();
    }

    internal static InvocationDefinition BuildTenaciousPlague()
    {
        const string NAME = "InvocationTenaciousPlague";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, InsectPlague)
            .SetGrantedSpell(InsectPlague, false, true)
            .SetRequirements(9, pact: FeatureSetPactChain)
            .AddToDB();
    }

    internal static InvocationDefinition BuildUndyingServitude()
    {
        const string NAME = "InvocationUndyingServitude";

        var spell = GetDefinition<SpellDefinition>("CreateDeadRisenSkeleton_Enforcer");

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, spell)
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

    private static InvocationDefinition BuildGraspingBlast()
    {
        const string NAME = "InvocationGraspingBlast";

        var powerInvocationGraspingBlast = FeatureDefinitionPowerBuilder
            .Create(PowerInvocationRepellingBlast, "PowerInvocationGraspingBlast")
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
            .Create(PowerSorakShadowEscape, $"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Invocation, PowerSorakShadowEscape)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .DelegatedToAction()
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerSorakShadowEscape)
                    .UseQuickAnimations()
                    .Build())
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create($"ActionDefinition{NAME}")
            .SetGuiPresentation(NAME, Category.Invocation,
                Sprites.GetSprite("Teleport", Resources.Teleport, 24), 71)
            .SetActionId(ExtraActionId.BondOfTheTalismanTeleport)
            .RequiresAuthorization(false)
            .OverrideClassName("UsePower")
            .SetActionScope(ActionScope.All)
            .SetActionType(ActionType.Bonus)
            .SetFormType(ActionFormType.Small)
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

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, Haste)
            .SetRequirements(5)
            .SetGrantedSpell(Haste, false, true)
            .AddToDB();
    }

    internal static InvocationDefinition BuildStasis()
    {
        const string NAME = "InvocationStasis";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, Slow)
            .SetRequirements(5)
            .SetGrantedSpell(Slow, false, true)
            .AddToDB();
    }

    internal static InvocationDefinition BuildChillingBlast()
    {
        const string NAME = "InvocationChillingBlast";

        var rayOfFrostParticles = RayOfFrost.EffectDescription.EffectParticleParameters;

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast, hidden: true)
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
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast, hidden: true)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeAcid,
                            AcidSplash.EffectDescription.EffectParticleParameters.casterParticleReference,
                            AcidArrow.EffectDescription.EffectParticleParameters.effectParticleReference,
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
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast, hidden: true)
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

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast, hidden: true)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeLightning,
                            LightningBolt.EffectDescription.EffectParticleParameters.casterParticleReference,
                            EldritchBlast.EffectDescription.EffectParticleParameters.effectParticleReference,
                            LightningBolt.EffectDescription.EffectParticleParameters.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildNecroticBlast()
    {
        const string NAME = "InvocationNecroticBlast";

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast, hidden: true)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeNecrotic,
                            RayOfEnfeeblement.EffectDescription.EffectParticleParameters.casterParticleReference,
                            Disintegrate.EffectDescription.EffectParticleParameters.effectParticleReference,
                            BurningHands_B.EffectDescription.EffectParticleParameters.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildPoisonousBlast()
    {
        const string NAME = "InvocationPoisonousBlast";

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast, hidden: true)
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
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast, hidden: true)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypePsychic,
                            Blur.EffectDescription.EffectParticleParameters.casterParticleReference,
                            EldritchBlast.EffectDescription.EffectParticleParameters.effectParticleReference,
                            Power_HornOfBlasting.EffectDescription.EffectParticleParameters.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildRadiantBlast()
    {
        const string NAME = "InvocationRadiantBlast";

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast, hidden: true)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeRadiant,
                            BrandingSmite.EffectDescription.EffectParticleParameters.casterParticleReference,
                            GuidingBolt.EffectDescription.EffectParticleParameters.effectParticleReference,
                            PowerTraditionLightBlindingFlash.EffectDescription.EffectParticleParameters
                                .impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildThunderBlast()
    {
        const string NAME = "InvocationThunderBlast";

        return InvocationDefinitionBuilder
            .Create(InvocationDefinitions.RepellingBlast, NAME)
            .SetGuiPresentation(Category.Invocation, InvocationDefinitions.RepellingBlast, hidden: true)
            .SetGrantedFeature(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ModifyEffectDescriptionEldritchBlast(
                            DamageTypeThunder,
                            Thunderwave.EffectDescription.EffectParticleParameters.casterParticleReference,
                            Disintegrate.EffectDescription.EffectParticleParameters.effectParticleReference,
                            Thunderwave.EffectDescription.EffectParticleParameters.impactParticleReference))
                    .AddToDB())
            .AddToDB();
    }

    internal static InvocationDefinition BuildSpectralShield()
    {
        const string NAME = "InvocationSpectralShield";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, ShieldOfFaith)
            .SetRequirements(5)
            .SetGrantedSpell(ShieldOfFaith)
            .AddToDB();
    }

    internal static InvocationDefinition BuildGiftOfTheHunter()
    {
        const string NAME = "InvocationGiftOfTheHunter";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, PassWithoutTrace)
            .SetRequirements(5)
            .SetGrantedSpell(PassWithoutTrace, false, true)
            .AddToDB();
    }


    internal static InvocationDefinition BuildDiscerningGaze()
    {
        const string NAME = "InvocationDiscerningGaze";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, DetectEvilAndGood)
            .SetRequirements(5)
            .SetGrantedSpell(DetectEvilAndGood)
            .AddToDB();
    }

    internal static InvocationDefinition BuildBreakerAndBanisher()
    {
        const string NAME = "InvocationBreakerAndBanisher";

        return InvocationDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, DispelEvilAndGood)
            .SetRequirements(9)
            .SetGrantedSpell(DispelEvilAndGood, false, true)
            .AddToDB();
    }

    #region Burning Hex

    internal static InvocationDefinition BuildBurningHex()
    {
        const string NAME = "InvocationBurningHex";

        var powerInvocationBurningHex = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Invocation, Blindness)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .SetDamageForm(DamageTypeFire)
                            .Build(),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionOnFire))
                    .SetParticleEffectParameters(PowerDomainElementalFireBurst)
                    .SetCasterEffectParameters(PowerPactChainPseudodragon)
                    .Build())
            .AddToDB();

        powerInvocationBurningHex.AddCustomSubFeatures(
            new FilterTargetingCharacterCanApplyHex(powerInvocationBurningHex));

        return InvocationDefinitionWithPrerequisitesBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, FireBolt)
            .SetValidators(ValidateHex)
            .SetGrantedFeature(powerInvocationBurningHex)
            .AddToDB();
    }

    #endregion

    #region Vexing Hex

    internal static InvocationDefinition BuildVexingHex()
    {
        const string NAME = "InvocationVexingHex";

        var powerVexingHex = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Invocation, Blindness)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Cube, 3)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetBonusMode(AddBonusMode.AbilityBonus)
                        .SetDamageForm(DamageTypePsychic)
                        .Build())
                    .SetParticleEffectParameters(PowerSorakDreadLaughter)
                    .SetCasterEffectParameters(PowerPactChainPseudodragon)
                    .Build())
            .AddToDB();

        powerVexingHex.AddCustomSubFeatures(new FilterTargetingCharacterCanApplyHex(powerVexingHex));

        return InvocationDefinitionWithPrerequisitesBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, Blindness)
            .SetRequirements(5)
            .SetValidators(ValidateHex)
            .SetGrantedFeature(powerVexingHex)
            .AddToDB();
    }

    #endregion

    #region Pernicious Cloak

    internal static InvocationDefinition BuildPerniciousCloak()
    {
        const string Name = "InvocationPerniciousCloak";

        var sprite = Sprites.GetSprite($"Power{Name}", Resources.PowerPerniciousCloak, 128);

        var abilityCheckAffinityPerniciousCloak = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}")
            .SetGuiPresentation($"Condition{Name}Self", Category.Condition)
            .BuildAndSetAffinityGroups(
                CharacterAbilityCheckAffinity.Advantage,
                abilityProficiencyPairs: (AttributeDefinitions.Charisma, SkillDefinitions.Intimidation))
            .BuildAndAddAffinityGroups(
                CharacterAbilityCheckAffinity.Disadvantage,
                abilityProficiencyPairs:
                [
                    (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Performance),
                    (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion)
                ])
            .AddToDB();

        var conditionPerniciousCloakSelf = ConditionDefinitionBuilder
            .Create($"Condition{Name}Self")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConjuredCreature)
            .SetConditionType(ConditionType.Neutral)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(abilityCheckAffinityPerniciousCloak)
            .CopyParticleReferences(ConditionDefinitions.ConditionOnAcidPilgrim)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var powerPerniciousCloakDamage = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Damage")
            .SetGuiPresentation(Name, Category.Invocation, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .SetDamageForm(DamageTypePoison)
                            .Build())
                    .SetImpactEffectParameters(AcidArrow)
                    .Build())
            .AddToDB();

        var conditionPerniciousCloak = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        conditionPerniciousCloak.AddCustomSubFeatures(
            new CharacterTurnStartListenerPerniciousCloak(powerPerniciousCloakDamage, conditionPerniciousCloak));

        var powerPerniciousCloak = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Invocation, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.None)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetRecurrentEffect(RecurrentEffect.OnEnter | RecurrentEffect.OnActivation)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionPerniciousCloak),
                        EffectFormBuilder.ConditionForm(conditionPerniciousCloakSelf,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .SetParticleEffectParameters(PowerDomainOblivionMarkOfFate)
                    .SetEffectEffectParameters(new AssetReference())
                    .Build())
            .AddToDB();

        var powerPerniciousCloakRemove = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Remove")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.None)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionPerniciousCloak, ConditionForm.ConditionOperation.Remove),
                        EffectFormBuilder.ConditionForm(
                            conditionPerniciousCloakSelf, ConditionForm.ConditionOperation.Remove))
                    .SetCasterEffectParameters(PowerDomainOblivionMarkOfFate)
                    .Build())
            .AddCustomSubFeatures(
                new CustomBehaviorPerniciousCloakRemove(powerPerniciousCloak, conditionPerniciousCloakSelf))
            .AddToDB();

        conditionPerniciousCloakSelf.Features.Add(powerPerniciousCloakRemove);

        return InvocationDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Invocation, sprite)
            .SetRequirements(5)
            .SetGrantedFeature(powerPerniciousCloak)
            .AddToDB();
    }

    private sealed class CharacterTurnStartListenerPerniciousCloak(
        FeatureDefinitionPower powerPerniciousCloakDamage,
        ConditionDefinition conditionPerniciousCloak) : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter character)
        {
            var rulesetCharacter = character.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionPerniciousCloak.Name, out var activeCondition))
            {
                return;
            }

            var rulesetCaster = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            if (rulesetCaster is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetCharacter == rulesetCaster)
            {
                return;
            }

            var caster = GameLocationCharacter.GetFromActor(rulesetCaster);
            var usablePower = PowerProvider.Get(powerPerniciousCloakDamage, rulesetCaster);

            // pernicious cloak damage is a use at will power
            caster.MyExecuteActionSpendPower(usablePower, character);
        }
    }

    private sealed class CustomBehaviorPerniciousCloakRemove(
        FeatureDefinitionPower powerPerniciousCloak,
        ConditionDefinition conditionPerniciousCloakSelf) : IPowerOrSpellFinishedByMe, IValidatePowerUse
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var rulesetEffectPower = EffectHelpers.GetAllEffectsBySourceGuid(rulesetCharacter.Guid)
                .OfType<RulesetEffectPower>()
                .FirstOrDefault(x => x.PowerDefinition == powerPerniciousCloak);

            if (rulesetEffectPower != null)
            {
                rulesetCharacter.TerminatePower(rulesetEffectPower);
            }

            yield break;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            return character.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionPerniciousCloakSelf.Name);
        }
    }

    #endregion

    #region Chain Master

    internal static InvocationDefinition BuildAbilitiesOfTheChainMaster()
    {
        const string NAME = "InvocationAbilitiesOfTheChainMaster";

        var conditionAbilitySprite = ConditionDefinitionBuilder
            .Create("ConditionAbilitySprite")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainSprite)
            .AddFeatures(
                FeatureDefinitionAttributeModifiers.AttributeModifierBarkskin,
                FeatureDefinitionCombatAffinitys.CombatAffinityBlinded)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var conditionAbilityImp = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionInvisibleGreater, "ConditionAbilityImp")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainImp)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var conditionAbilityPseudo = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlying, "ConditionAbilityPseudo")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainPseudodragon)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetFeatures(
                FeatureDefinitionMoveModes.MoveModeFly12,
                FeatureDefinitionAdditionalDamageBuilder
                    .Create(AdditionalDamagePoison_GhoulsCaress, "AdditionalDamagePseudoDragon")
                    .SetSavingThrowData(
                        EffectDifficultyClassComputation.SpellCastingFeature, EffectSavingThrowType.HalfDamage)
                    .SetDamageDice(DieType.D8, 1)
                    .SetNotificationTag("Poison")
                    .AddToDB())
            .AddToDB();

        conditionAbilityPseudo.ConditionTags.Clear();

        var conditionAbilityQuasit = ConditionDefinitionBuilder
            .Create("ConditionAbilityQuasit")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainQuasit)
            .AddFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create("AdditionalActionAbilityQuasit")
                    .SetGuiPresentationNoContent(true)
                    .SetActionType(ActionType.Main)
                    .SetRestrictedActions(Id.AttackMain)
                    .SetMaxAttacksNumber(1)
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

    private sealed class AfterActionFinishedByMeAbilitiesChain(
        ConditionDefinition conditionSpriteAbility,
        ConditionDefinition conditionImpAbility,
        ConditionDefinition conditionQuasitAbility,
        ConditionDefinition conditionPseudoAbility) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;

            if (action.ActionType != ActionType.Bonus &&
                //action.ActingCharacter.PerceptionState == ActionDefinitions.PerceptionState.OnGuard
                action.ActionDefinition.ActionScope == ActionScope.Battle)
            {
                yield break;
            }

            var rulesetCharacter = actingCharacter.RulesetCharacter;

            foreach (var power in rulesetCharacter.usablePowers
                         .ToArray()) // required ToArray() to avoid list was changed when Far Step in play
            {
                if (rulesetCharacter.IsPowerActive(power))
                {
                    if (power.PowerDefinition == PowerPactChainImp &&
                        !rulesetCharacter.HasConditionOfType(conditionImpAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, conditionImpAbility);
                    }
                    else if (power.PowerDefinition == PowerPactChainQuasit &&
                             !rulesetCharacter.HasConditionOfType(conditionQuasitAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, conditionQuasitAbility);
                    }
                    else if (power.PowerDefinition == PowerPactChainSprite &&
                             !rulesetCharacter.HasConditionOfType(conditionSpriteAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, conditionSpriteAbility);
                    }
                    else if (power.PowerDefinition == PowerPactChainPseudodragon &&
                             !rulesetCharacter.HasConditionOfType(conditionPseudoAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, conditionPseudoAbility);
                    }
                }
                else
                {
                    if (power.PowerDefinition == PowerPactChainImp)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            conditionImpAbility.name);
                    }
                    else if (power.PowerDefinition == PowerPactChainQuasit)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            conditionQuasitAbility.name);
                    }
                    else if (power.PowerDefinition == PowerPactChainSprite)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            conditionSpriteAbility.name);
                    }
                    else if (power.PowerDefinition == PowerPactChainPseudodragon)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            conditionPseudoAbility.name);
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
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionDefinition.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Chilling Hex

    internal static InvocationDefinition BuildChillingHex()
    {
        const string NAME = "InvocationChillingHex";

        var powerInvocationChillingHexDamage = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Damage")
            .SetGuiPresentation(NAME, Category.Invocation, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .SetDamageForm(DamageTypeCold)
                            .Build())
                    .SetImpactEffectParameters(PowerDomainElementalIceLance)
                    .Build())
            .AddToDB();

        var powerInvocationChillingHex = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Invocation, RayOfFrost)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(PowerDomainElementalIceLance)
                    .SetCasterEffectParameters(PowerPactChainPseudodragon)
                    .Build())
            .AddToDB();

        powerInvocationChillingHex.AddCustomSubFeatures(
            new FilterTargetingCharacterCanApplyHex(powerInvocationChillingHex),
            new PowerOrSpellFinishedByMeChillingHex(powerInvocationChillingHexDamage));

        return InvocationDefinitionWithPrerequisitesBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, RayOfFrost)
            .SetValidators(ValidateHex)
            .SetGrantedFeature(powerInvocationChillingHex)
            .AddToDB();
    }

    private sealed class PowerOrSpellFinishedByMeChillingHex(FeatureDefinitionPower powerChillingHexDamage)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var attacker = action.ActingCharacter;
            var defender = action.ActionParams.TargetCharacters[0];
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerChillingHexDamage, rulesetAttacker);
            var targets = Gui.Battle.GetContenders(defender, isOppositeSide: false, withinRange: 1).ToArray();

            // chilling hex damage is a use at will power
            attacker.MyExecuteActionSpendPower(usablePower, targets);
        }
    }

    #endregion

    #region HELPERS

    private sealed class FilterTargetingCharacterCanApplyHex(FeatureDefinitionPower powerHex)
        : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.ActionParams.activeEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != powerHex)
            {
                return true;
            }

            var rulesetCharacter = target.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                return false;
            }

            var isValid = CanApplyHex(rulesetCharacter);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Failure/&MustHaveMaledictionCurseOrHex");
            }

            return isValid;
        }
    }

    private static bool CanApplyHex(RulesetActor rulesetCharacter)
    {
        return rulesetCharacter.HasConditionOfType(ConditionDefinitions.ConditionMalediction.Name) ||
               rulesetCharacter.HasConditionOfTypeOrSubType(ConditionDefinitions.ConditionCursed.Name) ||
               rulesetCharacter.HasConditionOfType(PatronSoulBlade.ConditionHex);
    }

    private static (bool, string) ValidateHex(InvocationDefinition invocationDefinition, RulesetCharacterHero hero)
    {
        var hasMalediction = hero.SpellRepertoires.Any(x => x.HasKnowledgeOfSpell(Malediction));
        var hasBestowCurse = hero.SpellRepertoires.Any(x => x.HasKnowledgeOfSpell(BestowCurse));
        var hasSignIllOmen = hero.TrainedInvocations.Any(x => x == InvocationDefinitions.SignIllOmen)
                             || hero.GetHeroBuildingData().LevelupTrainedInvocations.Any(x =>
                                 x.Value.Any(y => y == InvocationDefinitions.SignIllOmen));
        var isSoulblade = hero.GetSubclassLevel(CharacterClassDefinitions.Warlock, PatronSoulBlade.FullName) > 0;
        var hasHex = hasMalediction || hasBestowCurse || hasSignIllOmen || isSoulblade;

        var guiFormat = Gui.Localize("Failure/&MustHaveMaledictionCurseOrHex");

        return !hasHex ? (false, Gui.Colorize(guiFormat, Gui.ColorFailure)) : (true, guiFormat);
    }

    #endregion

    #region Inexorable Hex

    internal static InvocationDefinition BuildInexorableHex()
    {
        const string Name = "InvocationInexorableHex";

        var powerInexorableHex = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Invocation, PowerMelekTeleport)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build())
                    .SetParticleEffectParameters(PowerMelekTeleport)
                    .Build())
            .AddCustomSubFeatures(
                ValidatorsValidatePowerUse.InCombat,
                new CustomBehaviorInexorableHex())
            .AddToDB();

        return InvocationDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Invocation, PowerMelekTeleport)
            .SetRequirements(7)
            .SetValidators(ValidateHex)
            .SetGrantedFeature(powerInexorableHex)
            .AddToDB();
    }

    private sealed class CustomBehaviorInexorableHex : IFilterTargetingPosition
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            cursorLocationSelectPosition.validPositionsCache.Clear();

            if (Gui.Battle == null)
            {
                yield break;
            }

            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();

            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;

            foreach (var gameLocationCharacter in Gui.Battle
                         .GetContenders(actingCharacter)
                         .Where(x => CanApplyHex(x.RulesetCharacter)))
            {
                var boxInt = new BoxInt(gameLocationCharacter.LocationPosition, int3.zero, int3.zero);

                boxInt.Inflate(1, 0, 1);

                foreach (var position in boxInt.EnumerateAllPositionsWithin())
                {
                    if (!visibilityService.MyIsCellPerceivedByCharacter(position, actingCharacter) ||
                        !positioningService.CanPlaceCharacter(
                            actingCharacter, position, CellHelpers.PlacementMode.Station) ||
                        !positioningService.CanCharacterStayAtPosition_Floor(
                            actingCharacter, position, onlyCheckCellsWithRealGround: true))
                    {
                        continue;
                    }

                    cursorLocationSelectPosition.validPositionsCache.Add(position);

                    if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                    {
                        yield return null;
                    }
                }
            }
        }
    }

    #endregion

    #region Tomb of Frost

    internal static InvocationDefinition BuildTombOfFrost()
    {
        const string Name = "InvocationTombOfFrost";

        var sprite = Sprites.GetSprite($"Power{Name}", Resources.PowerTombOfFrost, 128);

        var conditionTombOfFrost = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionIncapacitated, $"Condition{Name}")
            .SetGuiPresentation(Name, Category.Invocation, ConditionDefinitions.ConditionChilled)
            .SetParentCondition(ConditionDefinitions.ConditionIncapacitated)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(DamageAffinityFireVulnerability)
            .CopyParticleReferences(PowerDomainElementalHeraldOfTheElementsCold)
            .AddToDB();

        conditionTombOfFrost.GuiPresentation.description = Gui.EmptyContent;

        var conditionTombOfFrostLazy = ConditionDefinitionBuilder
            .Create($"Condition{Name}Lazy")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedTombOfFrostLazy(conditionTombOfFrost))
            .AddToDB();

        var powerTombOfFrost = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Invocation, sprite)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionTombOfFrostLazy))
                    .SetParticleEffectParameters(PowerDomainElementalHeraldOfTheElementsCold)
                    .SetCasterEffectParameters(RayOfFrost)
                    .Build())
            .AddToDB();

        powerTombOfFrost.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorTombOfFrost(powerTombOfFrost));

        return InvocationDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Invocation, sprite)
            .SetRequirements(5)
            .SetGrantedFeature(powerTombOfFrost)
            .AddToDB();
    }

    private sealed class CustomBehaviorTombOfFrost(FeatureDefinitionPower powerTombOfFrost)
        : IPhysicalAttackBeforeHitConfirmedOnMe, IMagicEffectBeforeHitConfirmedOnMe
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(battleManager, attacker, defender);
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(battleManager, attacker, defender);
        }

        private IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            var rulesetDefender = defender.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerTombOfFrost, rulesetDefender);

            if (!defender.CanReact() ||
                rulesetDefender.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            yield return defender.MyReactToUsePower(
                Id.PowerReaction,
                usablePower,
                [defender],
                attacker,
                "TombOfFrost",
                reactionValidated: ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                var classLevel = rulesetDefender.GetClassLevel(CharacterClassDefinitions.Warlock);

                rulesetDefender.ReceiveTemporaryHitPoints(
                    classLevel * 10, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetDefender.Guid);
            }
        }
    }

    private sealed class OnConditionAddedOrRemovedTombOfFrostLazy(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionTombOfFrost) : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var glc = GameLocationCharacter.GetFromActor(target);

            EffectHelpers.StartVisualEffect(
                glc, glc, PowerDomainElementalHeraldOfTheElementsCold, EffectHelpers.EffectType.Effect);

            target.InflictCondition(
                conditionTombOfFrost.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                target.Guid,
                target.CurrentFaction.Name,
                1,
                conditionTombOfFrost.Name,
                0,
                0,
                0);
        }
    }

    #endregion
}
