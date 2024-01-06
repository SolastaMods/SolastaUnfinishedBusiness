using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using TA;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
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
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
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

        spellKinesis.EffectDescription.targetFilteringMethod = TargetFilteringMethod.CharacterOnly;

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
                            BurningHands_B.EffectDescription.EffectParticleParameters.impactParticleReference))
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
            .SetOrUpdateGuiPresentation(Category.Invocation)
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

    #region Pernicious Cloak

    internal static InvocationDefinition BuildPerniciousCloak()
    {
        const string Name = "InvocationPerniciousCloak";

        var sprite = Sprites.GetSprite($"Power{Name}", Resources.PowerPerniciousCloak, 128, 128);

        var abilityCheckAffinityPerniciousCloak = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}")
            .SetGuiPresentation($"Condition{Name}Self", Category.Condition)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, DieType.D1, 0,
                (AttributeDefinitions.Charisma, SkillDefinitions.Intimidation))
            .BuildAndAddAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage, DieType.D1, 0,
                (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
                (AttributeDefinitions.Charisma, SkillDefinitions.Performance),
                (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion))
            .AddToDB();

        var conditionPerniciousCloakSelf = ConditionDefinitionBuilder
            .Create($"Condition{Name}Self")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConjuredCreature)
            .SetConditionType(ConditionType.Neutral)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(abilityCheckAffinityPerniciousCloak)
            .AddToDB();

        conditionPerniciousCloakSelf.conditionStartParticleReference =
            ConditionDefinitions.ConditionOnAcidPilgrim.conditionStartParticleReference;
        conditionPerniciousCloakSelf.conditionParticleReference =
            ConditionDefinitions.ConditionOnAcidPilgrim.conditionParticleReference;
        conditionPerniciousCloakSelf.conditionEndParticleReference =
            ConditionDefinitions.ConditionOnAcidPilgrim.conditionEndParticleReference;

        conditionPerniciousCloakSelf.specialDuration = false;

        var conditionPerniciousCloak = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        conditionPerniciousCloak.AddCustomSubFeatures(
            new CharacterTurnStartListenerPerniciousCloak(conditionPerniciousCloak));

        var powerPerniciousCloak = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Invocation, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.None)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilAnyRest)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetRecurrentEffect(RecurrentEffect.OnTurnStart | RecurrentEffect.OnActivation)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionPerniciousCloak),
                        EffectFormBuilder.ConditionForm(conditionPerniciousCloakSelf,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(PowerDomainOblivionMarkOfFate)
                    .Build())
            .AddToDB();

        powerPerniciousCloak.EffectDescription.EffectParticleParameters.effectParticleReference = new AssetReference();

        var powerPerniciousCloakRemove = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Remove")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.None)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                    .SetRecurrentEffect(RecurrentEffect.OnTurnStart | RecurrentEffect.OnActivation)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionPerniciousCloak, ConditionForm.ConditionOperation.Remove),
                        EffectFormBuilder.ConditionForm(
                            conditionPerniciousCloakSelf, ConditionForm.ConditionOperation.Remove))
                    .Build())
            .AddCustomSubFeatures(
                new CustomBehaviorPerniciousCloakRemove(powerPerniciousCloak, conditionPerniciousCloakSelf))
            .AddToDB();

        powerPerniciousCloakRemove.EffectDescription.EffectParticleParameters.casterParticleReference =
            PowerDomainOblivionMarkOfFate.EffectDescription.EffectParticleParameters.casterParticleReference;

        var featureSetPerniciousCloak = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(powerPerniciousCloak, powerPerniciousCloakRemove)
            .AddToDB();

        return InvocationDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Invocation, sprite)
            .SetRequirements(5)
            .SetGrantedFeature(featureSetPerniciousCloak)
            .AddToDB();
    }

    private sealed class CharacterTurnStartListenerPerniciousCloak : ICharacterTurnStartListener
    {
        private readonly ConditionDefinition _conditionPerniciousCloak;

        public CharacterTurnStartListenerPerniciousCloak(ConditionDefinition conditionPerniciousCloak)
        {
            _conditionPerniciousCloak = conditionPerniciousCloak;
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    _conditionPerniciousCloak.Name,
                    out var activeCondition))
            {
                return;
            }

            var caster = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            if (caster is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            if (rulesetCharacter == caster)
            {
                return;
            }

            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                caster.TryGetAttributeValue(AttributeDefinitions.Charisma));

            var damageForm = new DamageForm
            {
                DamageType = DamageTypePoison, DieType = DieType.D1, DiceNumber = 0, BonusDamage = charismaModifier
            };

            var gameLocationCaster = GameLocationCharacter.GetFromActor(caster);

            if (gameLocationCaster != null)
            {
                EffectHelpers.StartVisualEffect(
                    gameLocationCaster, locationCharacter, PoisonSpray, EffectHelpers.EffectType.Effect);
            }

            RulesetActor.InflictDamage(
                charismaModifier,
                damageForm,
                damageForm.DamageType,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetCharacter },
                rulesetCharacter,
                false,
                caster.Guid,
                false,
                [],
                new RollInfo(DieType.D1, [], charismaModifier),
                true,
                out _);
        }
    }

    private sealed class CustomBehaviorPerniciousCloakRemove : IMagicEffectFinishedByMe, IValidatePowerUse
    {
        private readonly ConditionDefinition _conditionPerniciousCloakSelf;
        private readonly FeatureDefinitionPower _powerPerniciousCloak;

        public CustomBehaviorPerniciousCloakRemove(
            FeatureDefinitionPower powerPerniciousCloak,
            ConditionDefinition conditionPerniciousCloakSelf)
        {
            _powerPerniciousCloak = powerPerniciousCloak;
            _conditionPerniciousCloakSelf = conditionPerniciousCloakSelf;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var rulesetEffectPower = EffectHelpers.GetAllEffectsBySourceGuid(rulesetCharacter.Guid)
                .OfType<RulesetEffectPower>()
                .FirstOrDefault(x => x.PowerDefinition == _powerPerniciousCloak);

            if (rulesetEffectPower != null)
            {
                rulesetCharacter.TerminatePower(rulesetEffectPower);
            }

            yield break;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            return character.HasAnyConditionOfType(_conditionPerniciousCloakSelf.Name);
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
                    if (power.PowerDefinition == PowerPactChainImp &&
                        !rulesetCharacter.HasConditionOfType(_conditionImpAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, _conditionImpAbility);
                    }
                    else if (power.PowerDefinition == PowerPactChainQuasit &&
                             !rulesetCharacter.HasConditionOfType(_conditionQuasitAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, _conditionQuasitAbility);
                    }
                    else if (power.PowerDefinition == PowerPactChainSprite &&
                             !rulesetCharacter.HasConditionOfType(_conditionSpriteAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, _conditionSpriteAbility);
                    }
                    else if (power.PowerDefinition == PowerPactChainPseudodragon &&
                             !rulesetCharacter.HasConditionOfType(_conditionPseudoAbility.name))
                    {
                        SetChainBuff(rulesetCharacter, _conditionPseudoAbility);
                    }
                }
                else
                {
                    if (power.PowerDefinition == PowerPactChainImp)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            _conditionImpAbility.name);
                    }
                    else if (power.PowerDefinition == PowerPactChainQuasit)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            _conditionQuasitAbility.name);
                    }
                    else if (power.PowerDefinition == PowerPactChainSprite)
                    {
                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                            _conditionSpriteAbility.name);
                    }
                    else if (power.PowerDefinition == PowerPactChainPseudodragon)
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
                conditionDefinition.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Vexing Hex

    internal static InvocationDefinition BuildVexingHex()
    {
        const string NAME = "InvocationVexingHex";

        var powerInvocationVexingHex = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Invocation, Blindness)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetBonusMode(AddBonusMode.AbilityBonus)
                        .SetDamageForm(DamageTypePsychic)
                        .Build())
                    .SetParticleEffectParameters(PowerSorakDreadLaughter)
                    .Build())
            .AddToDB();

        powerInvocationVexingHex.EffectDescription.EffectParticleParameters.casterParticleReference =
            PowerPactChainPseudodragon.EffectDescription.EffectParticleParameters.casterParticleReference;

        powerInvocationVexingHex.AddCustomSubFeatures(new FilterTargetingCharacterVexingHex(powerInvocationVexingHex));

        return InvocationDefinitionWithPrerequisitesBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Invocation, Blindness)
            .SetRequirements(5)
            .SetValidators(ValidateHex)
            .SetGrantedFeature(powerInvocationVexingHex)
            .AddToDB();
    }

    private sealed class FilterTargetingCharacterVexingHex : IFilterTargetingCharacter, IMagicEffectFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerVexingHex;

        public FilterTargetingCharacterVexingHex(FeatureDefinitionPower powerVexingHex)
        {
            _powerVexingHex = powerVexingHex;
        }

        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower
                || rulesetEffectPower.PowerDefinition != _powerVexingHex)
            {
                return true;
            }

            var rulesetCharacter = target.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                return true;
            }

            var isValid = CanApplyHex(rulesetCharacter);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustHaveMaledictionCurseOrHex");
            }

            return isValid;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var attacker = action.ActingCharacter;
            var defender = action.ActionParams.TargetCharacters[0];

            var rulesetAttacker = attacker.RulesetCharacter;
            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Charisma));

            // apply damage to all targets
            foreach (var rulesetDefender in gameLocationBattleService.Battle.AllContenders
                         .Where(x => x.IsOppositeSide(attacker.Side)
                                     && x != defender
                                     && x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }
                                     && gameLocationBattleService.IsWithin1Cell(defender, x))
                         .ToList() // avoid changing enumerator
                         .Select(targetCharacter => targetCharacter.RulesetCharacter))
            {
                var damageForm = new DamageForm
                {
                    DamageType = DamageTypePsychic,
                    DieType = DieType.D6,
                    DiceNumber = 0,
                    BonusDamage = charismaModifier
                };
                var rolls = new List<int>();
                var damageRoll = rulesetAttacker.RollDamage(
                    damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

                EffectHelpers.StartVisualEffect(attacker, defender, PowerSorakDreadLaughter);
                RulesetActor.InflictDamage(
                    damageRoll,
                    damageForm,
                    damageForm.DamageType,
                    new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                    rulesetDefender,
                    false,
                    attacker.Guid,
                    false,
                    [],
                    new RollInfo(damageForm.DieType, rolls, 0),
                    false,
                    out _);
            }
        }
    }

    #endregion

    #region HELPERS

    private static bool CanApplyHex(RulesetActor rulesetCharacter)
    {
        return rulesetCharacter.HasConditionOfType(ConditionDefinitions.ConditionMalediction.Name)
               || rulesetCharacter.HasConditionOfTypeOrSubType(ConditionDefinitions.ConditionCursed.Name)
               || rulesetCharacter.HasConditionOfType("ConditionPatronSoulbladeHexDefender");
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

        var guiFormat = Gui.Localize("Tooltip/&MustHaveMaledictionCurseOrHex");

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
            .AddCustomSubFeatures(new CustomBehaviorInexorableHex())
            .AddToDB();

        return InvocationDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Invocation, PowerMelekTeleport)
            .SetRequirements(7)
            .SetValidators(ValidateHex)
            .SetGrantedFeature(powerInexorableHex)
            .AddToDB();
    }

    private sealed class CustomBehaviorInexorableHex : IFilterTargetingPosition, IValidatePowerUse
    {
        public void EnumerateValidPositions(
            CursorLocationSelectPosition cursorLocationSelectPosition, List<int3> validPositions)
        {
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var gameLocationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                return;
            }

            var source = cursorLocationSelectPosition.ActionParams.ActingCharacter;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var gameLocationCharacter in gameLocationBattleService.Battle.AllContenders
                         .Where(x => x.IsOppositeSide(source.Side)
                                     && x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }
                                     && CanApplyHex(x.RulesetCharacter))
                         .ToList())
            {
                var boxInt = new BoxInt(
                    gameLocationCharacter.LocationPosition, new int3(-1, -1, -1), new int3(1, 1, 1));

                foreach (var position in boxInt.EnumerateAllPositionsWithin())
                {
                    if (gameLocationPositioningService.CanPlaceCharacter(
                            source, position, CellHelpers.PlacementMode.Station) &&
                        gameLocationPositioningService.CanCharacterStayAtPosition_Floor(
                            source, position, onlyCheckCellsWithRealGround: true))
                    {
                        validPositions.Add(position);
                    }
                }
            }
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            return Gui.Battle != null;
        }
    }

    #endregion

    #region Tomb of Frost

    internal static InvocationDefinition BuildTombOfFrost()
    {
        const string Name = "InvocationTombOfFrost";

        var sprite = Sprites.GetSprite($"Power{Name}", Resources.PowerTombOfFrost, 128, 128);

        var conditionTombOfFrost = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionIncapacitated, $"Condition{Name}")
            .SetGuiPresentation(Name, Category.Invocation, ConditionDefinitions.ConditionChilled)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialDuration()
            .AddFeatures(DamageAffinityFireVulnerability)
            .AddToDB();

        conditionTombOfFrost.conditionStartParticleReference = PowerDomainElementalHeraldOfTheElementsCold
            .EffectDescription.EffectParticleParameters.conditionStartParticleReference;
        conditionTombOfFrost.conditionParticleReference = PowerDomainElementalHeraldOfTheElementsCold
            .EffectDescription.EffectParticleParameters.conditionParticleReference;
        conditionTombOfFrost.conditionEndParticleReference = PowerDomainElementalHeraldOfTheElementsCold
            .EffectDescription.EffectParticleParameters.conditionEndParticleReference;

        conditionTombOfFrost.GuiPresentation.description = Gui.NoLocalization;

        var conditionTombOfFrostLazy = ConditionDefinitionBuilder
            .Create($"Condition{Name}Lazy")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedTombOfFrostLazy(conditionTombOfFrost))
            .AddToDB();

        var powerTombOfFrost = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Invocation, sprite)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.None)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(PowerDomainElementalHeraldOfTheElementsCold)
                    .Build())
            .AddToDB();

        powerTombOfFrost.EffectDescription.EffectParticleParameters.casterParticleReference =
            RayOfFrost.EffectDescription.EffectParticleParameters.casterParticleReference;

        powerTombOfFrost.AddCustomSubFeatures(
            new CustomBehaviorTombOfFrost(powerTombOfFrost, conditionTombOfFrostLazy));

        return InvocationDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Invocation, sprite)
            .SetRequirements(5)
            .SetGrantedFeature(powerTombOfFrost)
            .AddToDB();
    }

    private sealed class CustomBehaviorTombOfFrost : IAttackBeforeHitConfirmedOnMe, IMagicalAttackBeforeHitConfirmedOnMe
    {
        private readonly ConditionDefinition _conditionTombOfFrostLazy;
        private readonly FeatureDefinitionPower _powerTombOfFrost;

        public CustomBehaviorTombOfFrost(
            FeatureDefinitionPower powerTombOfFrost,
            ConditionDefinition conditionTombOfFrostLazy)
        {
            _powerTombOfFrost = powerTombOfFrost;
            _conditionTombOfFrostLazy = conditionTombOfFrostLazy;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect == null)
            {
                yield return HandleReaction(defender);
            }
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnMe(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(defender);
        }

        private IEnumerator HandleReaction(GameLocationCharacter defender)
        {
            var gameLocationBattleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationBattleManager is not { IsBattleInProgress: true } || gameLocationActionManager == null)
            {
                yield break;
            }

            if (!defender.CanReact())
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender.GetRemainingPowerCharges(_powerTombOfFrost) <= 0)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "TombOfFrost"
                };

            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendPower(reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleManager.WaitForReactions(
                defender, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetDefender.UpdateUsageForPower(_powerTombOfFrost, _powerTombOfFrost.CostPerUse);

            var classLevel = rulesetDefender.GetClassLevel(CharacterClassDefinitions.Warlock);
            var tempHitPoints = classLevel * 10;

            var activeCondition = rulesetDefender.InflictCondition(
                _conditionTombOfFrostLazy.Name,
                _conditionTombOfFrostLazy.DurationType,
                _conditionTombOfFrostLazy.DurationParameter,
                _conditionTombOfFrostLazy.TurnOccurence,
                AttributeDefinitions.TagEffect,
                rulesetDefender.Guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                _conditionTombOfFrostLazy.Name,
                0,
                0,
                0);

            rulesetDefender.ReceiveTemporaryHitPoints(
                tempHitPoints, DurationType.Round, 0, TurnOccurenceType.EndOfTurn, activeCondition.Guid);
        }
    }

    private sealed class OnConditionAddedOrRemovedTombOfFrostLazy : IOnConditionAddedOrRemoved
    {
        private readonly ConditionDefinition _conditionTombOfFrost;

        public OnConditionAddedOrRemovedTombOfFrostLazy(ConditionDefinition conditionTombOfFrost)
        {
            _conditionTombOfFrost = conditionTombOfFrost;
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var glc = GameLocationCharacter.GetFromActor(target);

            if (glc != null)
            {
                EffectHelpers.StartVisualEffect(
                    glc, glc, PowerDomainElementalHeraldOfTheElementsCold, EffectHelpers.EffectType.Effect);
            }

            target.InflictCondition(
                _conditionTombOfFrost.Name,
                _conditionTombOfFrost.DurationType,
                _conditionTombOfFrost.DurationParameter,
                _conditionTombOfFrost.TurnOccurence,
                AttributeDefinitions.TagEffect,
                target.Guid,
                target.CurrentFaction.Name,
                1,
                _conditionTombOfFrost.Name,
                0,
                0,
                0);
        }
    }

    #endregion
}
