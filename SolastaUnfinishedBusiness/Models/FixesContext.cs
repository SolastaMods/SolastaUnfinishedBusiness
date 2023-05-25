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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class FixesContext
{
    internal static void LateLoad()
    {
        // REQUIRED FIXES
        FixColorTables();
        FixAttackBuffsAffectingSpellDamage();
        FixSmitesAndStrikesDiceProgression();
        FixDivineStrikeRestrictions();
        FixDivineSmiteRestrictions();
        FixFightingStyleArchery();
        FixGorillaWildShapeRocksToUnlimited();
        FixMartialArtsProgression();
        FixMeleeHitEffectsRange();
        FixMinorSpellIssues();
        FixMissingWildShapeTagOnSomeForms();
        FixMountaineerBonusShoveRestrictions();
        FixRecklessAttackForReachWeapons();
        FixStunningStrikeForAnyMonkWeapon();
        FixTwinnedMetamagic();
        FixWildshapeGroupAttacks();

        // avoid folks tweaking max party size directly on settings as we don't need to stress cloud servers
        Main.Settings.OverridePartySize = Math.Min(Main.Settings.OverridePartySize, ToolsContext.MaxPartySize);

        //BUGFIX: this official condition doesn't have sprites or description
        ConditionDefinitions.ConditionConjuredItemLink.silentWhenAdded = true;
        ConditionDefinitions.ConditionConjuredItemLink.silentWhenRemoved = true;
        ConditionDefinitions.ConditionConjuredItemLink.GuiPresentation.hidden = true;

        //BEHAVIOR: Allow Duelist higher level feature to interact correctly with Uncanny Dodge
        static IsCharacterValidHandler IsActionAffinityUncannyDodgeValid(params string[] conditions)
        {
            // this allows Reflexive Party to trigger without Uncanny Dodge which can be triggered after that
            return character => character.GetSubclassLevel(Rogue, RoguishDuelist.Name) < 13 ||
                                conditions.Any(character.HasConditionOfType);
        }

        ActionAffinityUncannyDodge.SetCustomSubFeatures(new ValidatorsDefinitionApplication(
            IsActionAffinityUncannyDodgeValid(RoguishDuelist.ConditionReflexiveParry)));
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

    /**
     * Makes Divine Strike trigger only from melee attacks.
     */
    private static void FixDivineStrikeRestrictions()
    {
        FeatureDefinitionAdditionalDamages.AdditionalDamageDomainLifeDivineStrike.attackModeOnly = true;
        FeatureDefinitionAdditionalDamages.AdditionalDamageDomainLifeDivineStrike.requiredProperty =
            RestrictedContextRequiredProperty.MeleeWeapon;

        FeatureDefinitionAdditionalDamages.AdditionalDamageDomainMischiefDivineStrike.attackModeOnly = true;
        FeatureDefinitionAdditionalDamages.AdditionalDamageDomainMischiefDivineStrike.requiredProperty =
            RestrictedContextRequiredProperty.MeleeWeapon;
    }

    /**
     * Makes Divine Smite trigger only from melee attacks.
     * This wasn't relevant until we changed how SpendSpellSlot trigger works.
     */
    private static void FixDivineSmiteRestrictions()
    {
        FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite.attackModeOnly = true;
        FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite.requiredProperty =
            RestrictedContextRequiredProperty.MeleeWeapon;

        FeatureDefinitionAdditionalDamages.AdditionalDamageBrandingSmite.attackModeOnly = true;
        FeatureDefinitionAdditionalDamages.AdditionalDamageBrandingSmite.requiredProperty =
            RestrictedContextRequiredProperty.MeleeWeapon;
    }

    /**
     * Makes Divine Smite use correct number of dice when spending slot level 5+.
     * Base game has config only up to level 4 slots, which leads to it using 1 die if level 5+ slot is spent.
     */
    private static void FixSmitesAndStrikesDiceProgression()
    {
        FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite.diceByRankTable =
            DiceByRankBuilder.BuildDiceByRankTable(2);

        FeatureDefinitionAdditionalDamages.AdditionalDamageBrandingSmite.diceByRankTable =
            DiceByRankBuilder.BuildDiceByRankTable(2);

        FeatureDefinitionAdditionalDamages.AdditionalDamageDomainLifeDivineStrike.diceByRankTable =
            DiceByRankBuilder.BuildDiceByRankTable(0, 1, 7);

        FeatureDefinitionAdditionalDamages.AdditionalDamageDomainMischiefDivineStrike.diceByRankTable =
            DiceByRankBuilder.BuildDiceByRankTable(0, 1, 7);
    }

    /**
     * Ensures any spell or power effect in game that uses MeleeHit has a correct range of 1.
     * Otherwise our AttackEvaluationParams.FillForMagicReachAttack will use incorrect data.
     */
    private static void FixMeleeHitEffectsRange()
    {
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

    /**
     * Makes Mountaineer's `Shield Push` bonus shove work only with shield equipped.
     * This wasn't relevant until we removed forced shield check in the `GameLocationCharacter.GetActionStatus`.
     */
    private static void FixMountaineerBonusShoveRestrictions()
    {
        ActionAffinityMountaineerShieldCharge
            .SetCustomSubFeatures(new ValidatorsDefinitionApplication(ValidatorsCharacter.HasShield));
    }

    /**
     * Makes `Reckless` context check if main hand weapon is melee, instead of if character is next to target.
     * Required for it to work on reach weapons.
     */
    private static void FixRecklessAttackForReachWeapons()
    {
        FeatureDefinitionCombatAffinitys.CombatAffinityReckless.situationalContext =
            (SituationalContext)ExtraSituationalContext.MainWeaponIsMeleeOrUnarmedOrYeomanWithLongbow;
    }

    /**
     * Makes `Stunning Strike` context check if any monk weapon instead on OnAttackMeleeHitAuto
     * Required for it to work with monk weapon specialization and/or way of distant hand.
     */
    private static void FixStunningStrikeForAnyMonkWeapon()
    {
        FeatureDefinitionPowers.PowerMonkStunningStrike.activationTime = ActivationTime.OnAttackHitAuto;
    }

    private static void FixMinorSpellIssues()
    {
        //BUGFIX: add an effect to Counterspell
        Counterspell.EffectDescription.effectParticleParameters =
            DreadfulOmen.EffectDescription.effectParticleParameters;

        //BUGFIX: Chill Touch and Ray of Frost should have not saving throw
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

    private static void FixMartialArtsProgression()
    {
        //Fixes die progression of Monk's Martial Arts to use Monk level, not character level
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

    private static void FixAttackBuffsAffectingSpellDamage()
    {
        //BUGFIX: fix Branding Smite applying bonus damage to spells
        FeatureDefinitionAdditionalDamages.AdditionalDamageBrandingSmite
            .AddCustomSubFeatures(ValidatorsRestrictedContext.WeaponAttack);

        //BUGFIX: fix Divine Favor applying bonus damage to spells
        FeatureDefinitionAdditionalDamages.AdditionalDamageDivineFavor
            .AddCustomSubFeatures(ValidatorsRestrictedContext.WeaponAttack);
    }

    private static void FixMissingWildShapeTagOnSomeForms()
    {
        //BUGFIX: fix some Wild Shape forms missing proper tag, making wild shape action button visible while wild-shaped
        var wildShape = FeatureDefinitionPowers.PowerDruidWildShape;

        foreach (var option in wildShape.EffectDescription.FindFirstShapeChangeForm().ShapeOptions)
        {
            option.substituteMonster.CreatureTags.TryAdd(TagsDefinitions.CreatureTagWildShape);
        }
    }

    private static void FixGorillaWildShapeRocksToUnlimited()
    {
        //CHANGE: makes Wildshape Gorilla form having unlimited rock toss attacks 
        MonsterAttackDefinitions.Attack_Wildshape_Ape_Toss_Rock.limitedUse = false;
        MonsterAttackDefinitions.Attack_Wildshape_Ape_Toss_Rock.maxUses = -1;
    }

    private static void FixWildshapeGroupAttacks()
    {
        var monsters = new List<MonsterDefinition>
        {
            WildShapeApe,
            WildshapeBlackBear,
            WildShapeBrownBear,
            WildshapeDeepSpider,
            WildShapeGiant_Eagle
        };

        foreach (var monster in monsters)
        {
            monster.groupAttacks = false;

            foreach (var attackIterations in monster.AttackIterations)
            {
                attackIterations.number = 2;
            }
        }
    }

    // allow darts, lightning launcher or hand crossbows benefit from Archery Fighting Style
    private static void FixFightingStyleArchery()
    {
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
}
