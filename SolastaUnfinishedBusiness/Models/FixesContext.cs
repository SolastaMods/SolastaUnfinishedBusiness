using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Subclasses;
using UnityEngine;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class FixesContext
{
    internal static void LateLoad()
    {
        FixAdditionalDamageRestrictions();
        FixAttackBuffsAffectingSpellDamage();
        FixColorTables();
        FixFightingStyleArchery();
        FixGorillaWildShapeRocksToUnlimited();
        FixMartialArtsProgression();
        FixMeleeHitEffectsRange();
        FixMinorSpellIssues();
        FixMountaineerBonusShoveRestrictions();
        FixRecklessAttackForReachWeaponsAndPathOfTheYeoman();
        FixSmitesAndStrikesDiceProgression();
        FixStunningStrikeForAnyMonkWeapon();
        FixTwinnedMetamagic();
        FixUncannyDodgeForRoguishDuelist();

        Main.Settings.OverridePartySize = Math.Min(Main.Settings.OverridePartySize, ToolsContext.MaxPartySize);
    }

    private static void FixAdditionalDamageRestrictions()
    {
        //BUGFIX: Some vanilla additional damage definitions have incorrect attributes
        AdditionalDamageDomainLifeDivineStrike.attackModeOnly = true;
        AdditionalDamageDomainLifeDivineStrike.requiredProperty = RestrictedContextRequiredProperty.MeleeWeapon;

        AdditionalDamageDomainMischiefDivineStrike.attackModeOnly = true;
        AdditionalDamageDomainMischiefDivineStrike.requiredProperty = RestrictedContextRequiredProperty.MeleeWeapon;

        AdditionalDamagePaladinDivineSmite.attackModeOnly = true;
        AdditionalDamagePaladinDivineSmite.requiredProperty = RestrictedContextRequiredProperty.Weapon;
        AdditionalDamagePaladinDivineSmite.SetCustomSubFeatures(
            ValidatorsRestrictedContext.MeleeWeaponAttackOrOathOfThunder);

        AdditionalDamagePaladinImprovedDivineSmite.attackModeOnly = true;
        AdditionalDamagePaladinImprovedDivineSmite.requiredProperty = RestrictedContextRequiredProperty.Weapon;
        AdditionalDamagePaladinImprovedDivineSmite.SetCustomSubFeatures(
            ValidatorsRestrictedContext.MeleeWeaponAttackOrOathOfThunder);

        AdditionalDamageBrandingSmite.attackModeOnly = true;
        AdditionalDamageBrandingSmite.requiredProperty = RestrictedContextRequiredProperty.MeleeWeapon;

        AdditionalDamageRangerSwiftBladeBattleFocus.attackModeOnly = true;
        AdditionalDamageRangerSwiftBladeBattleFocus.requiredProperty = RestrictedContextRequiredProperty.MeleeWeapon;
    }

    private static void FixAttackBuffsAffectingSpellDamage()
    {
        //BUGFIX: fix Branding Smite applying bonus damage to spells
        AdditionalDamageBrandingSmite.AddCustomSubFeatures(ValidatorsRestrictedContext.WeaponAttack);

        //BUGFIX: fix Divine Favor applying bonus damage to spells
        AdditionalDamageDivineFavor.AddCustomSubFeatures(ValidatorsRestrictedContext.WeaponAttack);
    }

    private static void FixColorTables()
    {
        //BUGFIX: expand color tables
        for (var i = 21; i < 33; i++)
        {
            Gui.ModifierColors.Add(i, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.CheckModifierColors.Add(i, new Color32(0, 36, 77, byte.MaxValue));
        }
    }

    private static void FixFightingStyleArchery()
    {
        //BEHAVIOR: allow darts, lightning launcher or hand crossbows benefit from Archery Fighting Style
        FeatureDefinitionAttackModifiers.AttackModifierFightingStyleArchery.SetCustomSubFeatures(
            new RestrictedContextValidator((_, _, _, item, _, _, _) => (OperationType.Set,
                ValidatorsWeapon.IsWeaponType(item,
                    CustomWeaponsContext.HandXbowWeaponType,
                    CustomWeaponsContext.LightningLauncherType,
                    WeaponTypeDefinitions.LongbowType,
                    WeaponTypeDefinitions.ShortbowType,
                    WeaponTypeDefinitions.HeavyCrossbowType,
                    WeaponTypeDefinitions.LightCrossbowType,
                    WeaponTypeDefinitions.DartType))));
    }

    private static void FixGorillaWildShapeRocksToUnlimited()
    {
        //BEHAVIOR: makes Wildshape Gorilla form having unlimited rock toss attacks 
        MonsterAttackDefinitions.Attack_Wildshape_Ape_Toss_Rock.limitedUse = false;
        MonsterAttackDefinitions.Attack_Wildshape_Ape_Toss_Rock.maxUses = -1;
    }

    private static void FixMartialArtsProgression()
    {
        //BUGFIX: fixes die progression of Monk's Martial Arts to use Monk level, not character level
        var provider = new RankByClassLevel(Monk);
        var features = new List<FeatureDefinition>
        {
            FeatureDefinitionAttackModifiers.AttackModifierMonkMartialArtsImprovedDamage,
            FeatureDefinitionAttackModifiers.AttackModifierMonkMartialArtsUnarmedStrikeBonus,
            FeatureDefinitionAttackModifiers.AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonus,
            FeatureDefinitionAttackModifiers.AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedom
        };

        foreach (var feature in features)
        {
            feature.AddCustomSubFeatures(provider);
        }
    }

    private static void FixMeleeHitEffectsRange()
    {
        //BEHAVIOR: Ensures any spell or power effect in game that uses MeleeHit has a correct range of 1
        //Otherwise our AttackEvaluationParams.FillForMagicReachAttack will use incorrect data
        foreach (var effectDescription in DatabaseRepository.GetDatabase<SpellDefinition>()
                     .Select(x => x.EffectDescription)
                     .Where(x => x.rangeType == RangeType.MeleeHit))
        {
            effectDescription.rangeParameter = 1;
        }

        foreach (var effectDescription in DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
                     .Select(x => x.EffectDescription)
                     .Where(x => x.rangeType == RangeType.MeleeHit))
        {
            effectDescription.rangeParameter = 1;
        }
    }

    private static void FixMinorSpellIssues()
    {
        //BUGFIX: add an effect to Counterspell
        Counterspell.EffectDescription.effectParticleParameters =
            DreadfulOmen.EffectDescription.effectParticleParameters;

        //BUGFIX: Chill Touch and Ray of Frost should have no saving throw
        ChillTouch.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.None;
        RayOfFrost.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.None;

        //BUGFIX: Shows Concentration tag in UI
        BladeBarrier.requiresConcentration = true;

        //BUGFIX: stops upcasting assigning non-SRD durations
        var spells = new IMagicEffect[]
        {
            ProtectionFromEnergy, ProtectionFromEnergyAcid, ProtectionFromEnergyCold, ProtectionFromEnergyFire,
            ProtectionFromEnergyLightning, ProtectionFromEnergyThunder, ProtectionFromPoison
        };

        foreach (var spell in spells)
        {
            spell.EffectDescription.EffectAdvancement.alteredDuration = AdvancementDuration.None;
        }
    }

    private static void FixMountaineerBonusShoveRestrictions()
    {
        //BEHAVIOR: Makes Mountaineer's `Shield Push` bonus shove work only with shield equipped
        //This wasn't relevant until we removed forced shield check in the `GameLocationCharacter.GetActionStatus`
        ActionAffinityMountaineerShieldCharge
            .SetCustomSubFeatures(new ValidatorsDefinitionApplication(ValidatorsCharacter.HasShield));
    }

    private static void FixRecklessAttackForReachWeaponsAndPathOfTheYeoman()
    {
        //BEHAVIOR: Makes `Reckless` context check if main hand weapon is melee or long bow if Path of the Yeoman
        //instead of if character is next to target
        FeatureDefinitionCombatAffinitys.CombatAffinityReckless.situationalContext =
            (SituationalContext)ExtraSituationalContext.MainWeaponIsMeleeOrUnarmedOrYeomanWithLongbow;
    }

    private static void FixSmitesAndStrikesDiceProgression()
    {
        //BUGFIX: Makes Divine Smite use correct number of dice when spending slot level 5+
        AdditionalDamagePaladinDivineSmite.diceByRankTable = DiceByRankBuilder.BuildDiceByRankTable(2);
        AdditionalDamageBrandingSmite.diceByRankTable = DiceByRankBuilder.BuildDiceByRankTable(2);
        AdditionalDamageDomainLifeDivineStrike.diceByRankTable = DiceByRankBuilder.BuildDiceByRankTable(0, 1, 7);
        AdditionalDamageDomainMischiefDivineStrike.diceByRankTable = DiceByRankBuilder.BuildDiceByRankTable(0, 1, 7);
    }

    private static void FixStunningStrikeForAnyMonkWeapon()
    {
        //BEHAVIOR: Makes `Stunning Strike` context check if any monk weapon instead on OnAttackMeleeHitAuto
        //Required for it to work with monk weapon specialization and/or way of distant hand
        FeatureDefinitionPowers.PowerMonkStunningStrike.activationTime = ActivationTime.OnAttackHitAuto;
    }

    private static void FixTwinnedMetamagic()
    {
        //BUGFIX: fix vanilla twinned spells offering not accounting for target parameter progression
        MetamagicOptionDefinitions.MetamagicTwinnedSpell.AddCustomSubFeatures(
            new MetamagicApplicationValidator(
                (RulesetCharacter _,
                    RulesetEffectSpell spell,
                    MetamagicOptionDefinition _,
                    ref bool result,
                    ref string failure) =>
                {
                    var effectDescription = spell.SpellDefinition.effectDescription;

                    if (effectDescription.TargetType is not (TargetType.Individuals or TargetType.IndividualsUnique)
                        || spell.ComputeTargetParameter() == 1)
                    {
                        return;
                    }

                    failure = FailureFlagInvalidSingleTarget;
                    result = false;
                }));
    }

    private static void FixUncannyDodgeForRoguishDuelist()
    {
        //BEHAVIOR: Allow Duelist higher level feature to interact correctly with Uncanny Dodge
        ActionAffinityUncannyDodge.SetCustomSubFeatures(new ValidatorsDefinitionApplication(
            character => character.GetSubclassLevel(Rogue, RoguishDuelist.Name) < 13 ||
                         character.HasConditionOfType(RoguishDuelist.ConditionReflexiveParry)));
    }
}
