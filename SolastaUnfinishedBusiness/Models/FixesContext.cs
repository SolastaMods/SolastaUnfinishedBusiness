using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine;
using static EquipmentDefinitions;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Models;

internal static class FixesContext
{
    internal static void Load()
    {
        InitMagicAffinitiesAndCastSpells();
        FixMinorMagicEffectsIssues();
    }

    internal static void LateLoad()
    {
        AddAdditionalActionTitles();
        ExtendCharmImmunityToDemonicInfluence();
        FixAdditionalDamageRestrictions();
        FixAdditionalDamageRogueSneakAttack();
        FixArmorClassOnLegendaryArmors();
        FixAttackBuffsAffectingSpellDamage();
        FixBlackDragonLegendaryActions();
        FixColorTables();
        FixCriticalThresholdModifiers();
        FixDivineBlade();
        FixDragonBreathPowerSavingAttribute();
        FixEagerForBattleTexts();
        FixFightingStyleArchery();
        FixGorillaWildShapeRocksToUnlimited();
        FixLanguagesPointPoolsToIncludeAllLanguages();
        FixMartialArtsProgression();
        FixMountaineerBonusShoveRestrictions();
        FixMummyDreadfulGlareSavingAttribute();
        FixPowerDragonbornBreathWeaponDiceProgression();
        FixRecklessAttackForReachWeaponsAndPathOfTheYeoman();
        FixSavingThrowAffinityManaPainterAbsorption();
        FixSmitesAndStrikesDiceProgression();
        FixStunningStrikeForAnyMonkWeapon();
        FixTwinnedMetamagic();
        FixUncannyDodgeForRoguishDuelist();

        // fix condition UI
        FeatureDefinitionCombatAffinitys.CombatAffinityForeknowledge.GuiPresentation.Description = Gui.NoLocalization;

        Main.Settings.OverridePartySize = Math.Min(Main.Settings.OverridePartySize, ToolsContext.MaxPartySize);
    }

    private static void InitMagicAffinitiesAndCastSpells()
    {
        // required to avoid issues on how game calculates caster / spell levels and some trace error messages
        // that might affect multiplayer sessions, prevent level up from 19 to 20 and prevent some MC scenarios

        var classesFeatures = DatabaseRepository.GetDatabase<CharacterClassDefinition>()
            .SelectMany(a => a.FeatureUnlocks)
            .Select(b => b.FeatureDefinition);

        var subclassesFeatures = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
            .SelectMany(a => a.FeatureUnlocks)
            .Select(b => b.FeatureDefinition);

        var racesFeatures = DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
            .SelectMany(a => a.FeatureUnlocks)
            .Select(b => b.FeatureDefinition);

        var allFeatures = classesFeatures.Concat(subclassesFeatures).Concat(racesFeatures).ToList();
        var castSpellDefinitions = allFeatures.OfType<FeatureDefinitionCastSpell>();
        var magicAffinityDefinitions = allFeatures.OfType<FeatureDefinitionMagicAffinity>();

        foreach (var magicAffinityDefinition in magicAffinityDefinitions)
        {
            var spellListDefinition = magicAffinityDefinition.ExtendedSpellList;

            if (!spellListDefinition)
            {
                continue;
            }

            var spellsByLevel = spellListDefinition.SpellsByLevel;

            while (spellsByLevel.Count < 10)
            {
                spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = spellsByLevel.Count, Spells = []
                });
            }
        }

        foreach (var castSpellDefinition in castSpellDefinitions)
        {
            while (castSpellDefinition.KnownCantrips.Count < Level20Context.ModMaxLevel + 1)
            {
                castSpellDefinition.KnownCantrips.Add(0);
            }

            while (castSpellDefinition.KnownSpells.Count < Level20Context.ModMaxLevel + 1)
            {
                castSpellDefinition.KnownSpells.Add(0);
            }

            while (castSpellDefinition.ReplacedSpells.Count < Level20Context.ModMaxLevel + 1)
            {
                castSpellDefinition.ReplacedSpells.Add(0);
            }

            while (castSpellDefinition.ScribedSpells.Count < Level20Context.ModMaxLevel + 1)
            {
                castSpellDefinition.ScribedSpells.Add(0);
            }

            var spellListDefinition = castSpellDefinition.SpellListDefinition;

            if (!spellListDefinition)
            {
                continue;
            }

            var spellsByLevel = spellListDefinition.SpellsByLevel;

            while (spellsByLevel.Count < 10)
            {
                spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = spellsByLevel.Count, Spells = []
                });
            }
        }

        // fixes known cantrips and slots for some incomplete cast spell features
        for (var level = 17; level <= 20; level++)
        {
            // Tiefling
            FeatureDefinitionCastSpells.CastSpellTiefling.slotsPerLevels.Add(
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                {
                    Level = level, Slots = FeatureDefinitionCastSpells.CastSpellTiefling.slotsPerLevels[15].slots
                });

            FeatureDefinitionCastSpells.CastSpellTiefling.KnownCantrips[level] = 1;

            // Gnome
            FeatureDefinitionCastSpells.CastSpellGnomeShadow.slotsPerLevels.Add(
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                {
                    Level = level, Slots = FeatureDefinitionCastSpells.CastSpellGnomeShadow.slotsPerLevels[15].slots
                });

            FeatureDefinitionCastSpells.CastSpellGnomeShadow.KnownCantrips[level] = 1;

            // Tradition Light
            FeatureDefinitionCastSpells.CastSpellTraditionLight.slotsPerLevels.Add(
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                {
                    Level = level,
                    Slots = FeatureDefinitionCastSpells.CastSpellTraditionLight.slotsPerLevels[15].slots
                });

            FeatureDefinitionCastSpells.CastSpellTraditionLight.KnownCantrips[level] = 2;

            // Warlock
            FeatureDefinitionCastSpells.CastSpellWarlock.slotsPerLevels[level - 1].slots =
            [
                0,
                0,
                0,
                0,
                4,
                0,
                0,
                0,
                0
            ];

            FeatureDefinitionCastSpells.CastSpellWarlock.KnownCantrips[level - 1] = 4;
        }
    }

    private static void FixSavingThrowAffinityManaPainterAbsorption()
    {
        FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityManaPainterAbsorption.AffinityGroups.Clear();
    }

    private static void FixLanguagesPointPoolsToIncludeAllLanguages()
    {
        var dlcLanguages = new List<string> { "Language_Abyssal", "Language_Gnomish", "Language_Infernal" };

        FeatureDefinitionPointPools.PointPoolBackgroundLanguageChoice_one.RestrictedChoices.AddRange(dlcLanguages);
        FeatureDefinitionPointPools.PointPoolBackgroundLanguageChoice_two.RestrictedChoices.AddRange(dlcLanguages);
    }

    private static void ExtendCharmImmunityToDemonicInfluence()
    {
        ConditionDefinitions.ConditionUnderDemonicInfluence.parentCondition = ConditionDefinitions.ConditionCharmed;
    }

    private static void FixPowerDragonbornBreathWeaponDiceProgression()
    {
        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
            .Where(x => x.Name.StartsWith("PowerDragonbornBreathWeapon"));

        foreach (var power in powers)
        {
            power.EffectDescription.EffectForms[0].diceByLevelTable =
                DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (6, 1), (11, 2), (16, 3));
        }
    }

    private static void FixDragonBreathPowerSavingAttribute()
    {
        FeatureDefinitionPowers.PowerDragonBreath_Acid.EffectDescription.savingThrowAbility =
            AttributeDefinitions.Dexterity;

        FeatureDefinitionPowers.PowerDragonBreath_Acid_Spectral_DLC3.EffectDescription.savingThrowAbility =
            AttributeDefinitions.Dexterity;

        FeatureDefinitionPowers.PowerDragonBreath_Fire.EffectDescription.savingThrowAbility =
            AttributeDefinitions.Dexterity;

        FeatureDefinitionPowers.PowerDragonBreath_YoungGreen_Poison.EffectDescription.savingThrowAbility =
            AttributeDefinitions.Constitution;
    }

    private static void FixBlackDragonLegendaryActions()
    {
        MonsterDefinitions.BlackDragon_MasterOfNecromancy.LegendaryActionOptions.SetRange(
            MonsterDefinitions.GoldDragon_AerElai.LegendaryActionOptions);
    }

    private static void FixArmorClassOnLegendaryArmors()
    {
        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
        {
            foreach (var staticProperty in item.StaticProperties
                         .Where(x => x.FeatureDefinition &&
                                     x.FeatureDefinition.Name.StartsWith("AttributeModifierArmor")))
            {
                staticProperty.knowledgeAffinity = KnowledgeAffinity.ActiveAndVisible;
            }
        }
    }

    private static void FixMummyDreadfulGlareSavingAttribute()
    {
        FeatureDefinitionPowers.Power_Mummy_DreadfulGlare.EffectDescription.savingThrowAbility =
            AttributeDefinitions.Wisdom;

        FeatureDefinitionPowers.Power_MummyLord_DreadfulGlare.EffectDescription.savingThrowAbility =
            AttributeDefinitions.Wisdom;
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
        AdditionalDamagePaladinDivineSmite.AddCustomSubFeatures(
            ValidateContextInsteadOfRestrictedProperty.Or(
                OperationType.Set,
                ValidatorsRestrictedContext.IsMeleeWeaponAttack,
                ValidatorsRestrictedContext.IsOathOfDemonHunter,
                ValidatorsRestrictedContext.IsOathOfThunder
            ));

        AdditionalDamagePaladinImprovedDivineSmite.attackModeOnly = true;
        AdditionalDamagePaladinImprovedDivineSmite.requiredProperty = RestrictedContextRequiredProperty.Weapon;
        AdditionalDamagePaladinImprovedDivineSmite.AddCustomSubFeatures(
            ValidateContextInsteadOfRestrictedProperty.Or(
                OperationType.Set,
                ValidatorsRestrictedContext.IsMeleeWeaponAttack,
                ValidatorsRestrictedContext.IsOathOfDemonHunter,
                ValidatorsRestrictedContext.IsOathOfThunder
            ));

        AdditionalDamageBrandingSmite.attackModeOnly = true;
        AdditionalDamageBrandingSmite.requiredProperty = RestrictedContextRequiredProperty.MeleeWeapon;

        AdditionalDamageRangerSwiftBladeBattleFocus.attackModeOnly = true;
        AdditionalDamageRangerSwiftBladeBattleFocus.requiredProperty = RestrictedContextRequiredProperty.MeleeWeapon;
    }

    private static void FixAttackBuffsAffectingSpellDamage()
    {
        //BUGFIX: fix Branding Smite applying bonus damage to spells
        AdditionalDamageBrandingSmite.AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack);

        //BUGFIX: fix Divine Favor applying bonus damage to spells
        AdditionalDamageDivineFavor.AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack);
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

    private static void FixDivineBlade()
    {
        //BUGFIX: allows clerics to actually wield divine blade
        const string ConjuredWeaponTypeName = "ConjuredWeaponType";

        WeaponTypeDefinitionBuilder
            .Create(WeaponTypeDefinitions.LongswordType, ConjuredWeaponTypeName)
            .SetGuiPresentation(Category.Item, GuiPresentationBuilder.EmptyString)
            .SetWeaponCategory(WeaponCategoryDefinitions.SimpleWeaponCategory)
            .AddToDB();

        ItemDefinitions.DivineBladeWeapon.weaponDefinition.weaponType = ConjuredWeaponTypeName;

        //BUGFIX: extend the fix to Flame Blade
        ItemDefinitions.FlameBlade.weaponDefinition.weaponType = ConjuredWeaponTypeName;

        //BUGFIX: allows classes without simple weapon proficiency to wield divine blade
        FeatureDefinitionProficiencys.ProficiencyDruidWeapon.proficiencies.Add(ConjuredWeaponTypeName);
        FeatureDefinitionProficiencys.ProficiencySorcererWeapon.proficiencies.Add(ConjuredWeaponTypeName);
        FeatureDefinitionProficiencys.ProficiencyWizardWeapon.proficiencies.Add(ConjuredWeaponTypeName);
    }

    private static void FixFightingStyleArchery()
    {
        //BEHAVIOR: allow darts, lightning launcher or hand crossbows benefit from Archery Fighting Style
        FeatureDefinitionAttackModifiers.AttackModifierFightingStyleArchery.AddCustomSubFeatures(
            new ValidateContextInsteadOfRestrictedProperty((_, _, _, _, _, mode, _) => (OperationType.Set,
                ValidatorsWeapon.IsOfWeaponType(
                    CustomWeaponsContext.HandXbowWeaponType,
                    CustomWeaponsContext.LightningLauncherType,
                    WeaponTypeDefinitions.LongbowType,
                    WeaponTypeDefinitions.ShortbowType,
                    WeaponTypeDefinitions.HeavyCrossbowType,
                    WeaponTypeDefinitions.LightCrossbowType,
                    WeaponTypeDefinitions.DartType)(mode, null, null))));
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
        var provider = new ModifyProviderRankByClassLevel(Monk);
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

    private static void FixMinorMagicEffectsIssues()
    {
        // fix Vampiric Touch
        VampiricTouch.EffectDescription.rangeParameter = 1;

        // fix Banishment
        var conditionBanishedByBanishment = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBanished, "ConditionBanishedByBanishment")
            .SetParentCondition(ConditionDefinitions.ConditionBanished)
            .SetFeatures()
            .AddToDB();

        Banishment.EffectDescription.EffectForms[0] = EffectFormBuilder.ConditionForm(conditionBanishedByBanishment);
        conditionBanishedByBanishment.permanentlyRemovedIfExtraPlanar = true;
        ConditionDefinitions.ConditionBanished.permanentlyRemovedIfExtraPlanar = false;

        // fix touch powers with range parameter greater than zero
        foreach (var power in DatabaseRepository.GetDatabase<SpellDefinition>()
                     .Where(x => x.EffectDescription.RangeType == RangeType.Touch))
        {
            power.EffectDescription.rangeParameter = 0;
        }

        // fix touch spells with range parameter greater than zero
        foreach (var spell in DatabaseRepository.GetDatabase<SpellDefinition>()
                     .Where(x => x.EffectDescription.RangeType == RangeType.Touch))
        {
            spell.EffectDescription.rangeParameter = 0;
        }

        // fix raise dead spells adding a buff instead of debuff after raising from dead
        foreach (var affinityGroup in DatabaseRepository.GetDatabase<FeatureDefinitionSavingThrowAffinity>()
                     .Where(x => x.Name.Contains("ConditionBackFromDead"))
                     .SelectMany(x => x.AffinityGroups))
        {
            affinityGroup.savingThrowModifierType = FeatureDefinitionSavingThrowAffinity.ModifierType.RemoveDice;
        }

        foreach (var abilityCheckAffinity in DatabaseRepository.GetDatabase<FeatureDefinitionAbilityCheckAffinity>()
                     .Where(x => x.Name.Contains("ConditionBackFromDead"))
                     .SelectMany(x => x.AffinityGroups))
        {
            abilityCheckAffinity.abilityCheckGroupOperation = AbilityCheckGroupOperation.SubstractDie;
        }

        ConditionDefinitions.ConditionBlinded.Features.Remove(
            FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionBlinded);

        CloudKill.EffectDescription.EffectForms.TryAdd(
            EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionHeavilyObscured));

        CloudKill.EffectDescription.recurrentEffect =
            RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnEnd;

        Entangle.EffectDescription.EffectForms.Add(
            EffectFormBuilder.TopologyForm(TopologyForm.Type.DangerousZone, false));

        IncendiaryCloud.EffectDescription.EffectForms.TryAdd(
            EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionHeavilyObscured));

        SleetStorm.EffectDescription.recurrentEffect =
            RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnEnd;

        //BUGFIX: add an effect to Counterspell
        Counterspell.EffectDescription.effectParticleParameters =
            DreadfulOmen.EffectDescription.effectParticleParameters;

        //BUGFIX: Chill Touch and Ray of Frost should have no saving throw
        ChillTouch.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.None;
        RayOfFrost.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.None;

        //BUGFIX: Wall of Fire should have a DEX saving throw
        WallOfFire.EffectDescription.hasSavingThrow = true;
        WallOfFire.EffectDescription.savingThrowAbility = AttributeDefinitions.Dexterity;
        WallOfFire.EffectDescription.EffectForms[0].hasSavingThrow = true;
        WallOfFire.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.HalfDamage;

        WallOfFireLine.EffectDescription.hasSavingThrow = true;
        WallOfFireLine.EffectDescription.savingThrowAbility = AttributeDefinitions.Dexterity;
        WallOfFireLine.EffectDescription.EffectForms[0].hasSavingThrow = true;
        WallOfFireLine.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.HalfDamage;

        WallOfFireRing_Inner.EffectDescription.hasSavingThrow = true;
        WallOfFireRing_Inner.EffectDescription.savingThrowAbility = AttributeDefinitions.Dexterity;
        WallOfFireRing_Inner.EffectDescription.EffectForms[0].hasSavingThrow = true;
        WallOfFireRing_Inner.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.HalfDamage;

        WallOfFireRing_Outer.EffectDescription.hasSavingThrow = true;
        WallOfFireRing_Outer.EffectDescription.savingThrowAbility = AttributeDefinitions.Dexterity;
        WallOfFireRing_Outer.EffectDescription.EffectForms[0].hasSavingThrow = true;
        WallOfFireRing_Outer.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.HalfDamage;

        //BUGFIX: Insect Plague should have a CON saving throw
        InsectPlague.EffectDescription.hasSavingThrow = true;
        InsectPlague.EffectDescription.savingThrowAbility = AttributeDefinitions.Constitution;
        InsectPlague.EffectDescription.EffectForms[0].hasSavingThrow = true;
        InsectPlague.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.HalfDamage;

        //BUGFIX: Ray of Enfeeblement should be recurrent on activation and turn start
        RayOfEnfeeblement.EffectDescription.HasSavingThrow = false;
        RayOfEnfeeblement.EffectDescription.RangeType = RangeType.RangeHit;
        RayOfEnfeeblement.EffectDescription.EffectForms[0].canSaveToCancel = true;
        RayOfEnfeeblement.EffectDescription.recurrentEffect =
            RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart;

        //BUGFIX: Sorcerers should have Insect Plague at level 5
        SpellListSorcerer.SpellsByLevel.FirstOrDefault(x => x.Level == 5)!.Spells.Add(InsectPlague);

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
            .AddCustomSubFeatures(new ValidateDefinitionApplication(ValidatorsCharacter.HasShield));
    }

    private static void FixRecklessAttackForReachWeaponsAndPathOfTheYeoman()
    {
        //BEHAVIOR: Makes `Reckless` context check if main hand weapon is melee/unarmed within reach or long bow if Path of the Yeoman
        //instead of if character is next to target
        FeatureDefinitionCombatAffinitys.CombatAffinityReckless.situationalContext = (SituationalContext)
            ExtraSituationalContext.AttackerWithMeleeOrUnarmedAndTargetWithinReachOrYeomanWithLongbow;
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
        FeatureDefinitionPowers.PowerMonkStunningStrike.activationTime = ActivationTime.NoCost;
        FeatureDefinitionPowers.PowerMonkStunningStrike.GuiPresentation.hidden = true;
        FeatureDefinitionPowers.PowerMonkStunningStrike.AddCustomSubFeatures(
            new PhysicalAttackFinishedByMeStunningStrike());
    }

    private static void FixTwinnedMetamagic()
    {
        //BUGFIX: fix vanilla twinned spells offering not accounting for target parameter progression
        MetamagicOptionDefinitions.MetamagicTwinnedSpell.AddCustomSubFeatures(
            new ValidateMetamagicApplication(
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
        ActionAffinityUncannyDodge.AddCustomSubFeatures(new ValidateDefinitionApplication(
            character => character.GetSubclassLevel(Rogue, RoguishDuelist.Name) < 13 ||
                         character.HasConditionOfType(RoguishDuelist.ConditionReflexiveParryName)));
    }

    private static void FixAdditionalDamageRogueSneakAttack()
    {
        AdditionalDamageRogueSneakAttack.AddCustomSubFeatures(
            new ModifyAdditionalDamageRogueSneakAttack(AdditionalDamageRogueSneakAttack));
    }

    private static void FixCriticalThresholdModifiers()
    {
        //Changes Champion's Improved Critical to set crit threshold to 19, instead of forcing if worse - fixes stacking with feats
        var modifier = AttributeModifierMartialChampionImprovedCritical;
        modifier.modifierOperation = AttributeModifierOperation.Set;

        //Changes Champion's Superior Critical to set crit threshold to 18, instead of forcing if worse - fixes stacking with feats
        modifier = AttributeModifierMartialChampionSuperiorCritical;
        modifier.modifierOperation = AttributeModifierOperation.Set;

        //Changes critical threshold of Sudden Death dagger to set
        modifier = AttributeModifierCriticalThresholdDLC3_Dwarven_Weapon_DaggerPlus3;

        //v1.5.92 set it to ForceIfWorse 19 to fix stacking issues. in UB we fixed those original issues, so making it to SET to not break stacking
        modifier.modifierOperation = AttributeModifierOperation.Set;
        modifier.modifierValue = 18;
    }

    private static void FixEagerForBattleTexts()
    {
        var feat = FeatDefinitions.EagerForBattle.GuiPresentation;
        var feature = FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle.GuiPresentation;

        feature.title = feat.title;

        var parts = Gui.Localize(feat.description).Split('\n');

        //last line of feat description
        feature.description = parts[parts.Length - 1].Trim();
    }

    private static void AddAdditionalActionTitles()
    {
        FeatureDefinitionAdditionalActions.AdditionalActionHasted.GuiPresentation.Title
            = Haste.GuiPresentation.Title;
        FeatureDefinitionAdditionalActions.AdditionalActionSurgedMain.GuiPresentation.Title
            = DatabaseHelper.ActionDefinitions.ActionSurge.GuiPresentation.Title;
    }

    private sealed class PhysicalAttackFinishedByMeStunningStrike : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            if (rulesetAttacker.GetRemainingPowerUses(FeatureDefinitionPowers.PowerMonkStunningStrike) == 0)
            {
                yield break;
            }

            // ensure Zen Archer Hail of Arrows won't trigger stunning strike without ki points
            if (attackMode.AttackTags.Contains(WayOfZenArchery.HailOfArrows))
            {
                var hasTab = attacker.UsedSpecialFeatures.TryGetValue(WayOfZenArchery.HailOfArrows, out var attacks);

                if (hasTab)
                {
                    attacker.UsedSpecialFeatures[WayOfZenArchery.HailOfArrows] += 1;
                }
                else
                {
                    attacker.UsedSpecialFeatures.Add(WayOfZenArchery.HailOfArrows, 1);
                }

                if (rulesetAttacker.RemainingKiPoints - attacks <= 0)
                {
                    yield break;
                }
            }

            if (!attacker.IsActionOnGoing(ActionDefinitions.Id.StunningStrikeToggle))
            {
                yield break;
            }

            var wayOfZenArcheryLevels = rulesetAttacker.GetSubclassLevel(Monk, WayOfZenArchery.Name);

            // Zen Archery get stunning strike with bows at 6 and Distant Hand with bows at 11
            if (!ValidatorsWeapon.IsMelee(attackMode) &&
                (wayOfZenArcheryLevels < WayOfZenArchery.StunningStrikeWithBowAllowedLevel ||
                 !ValidatorsCharacter.HasBowWithoutArmor(rulesetAttacker)))
            {
                yield break;
            }

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(FeatureDefinitionPowers.PowerMonkStunningStrike, rulesetAttacker);
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }

    //
    // CONSOLIDATED SNEAK ATTACK DAMAGE FORM MODIFIER
    //

    private sealed class ModifyAdditionalDamageRogueSneakAttack(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionAdditionalDamage additionalDamage) : IModifyAdditionalDamage
    {
        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm damageForm)
        {
            if (featureDefinitionAdditionalDamage != additionalDamage)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter.GetOriginalHero();

            if (rulesetAttacker == null)
            {
                return;
            }

            // handle close quarters feat
            ClassFeats.HandleCloseQuarters(attacker, rulesetAttacker, defender, ref damageForm);

            // handle rogue cunning strike feature
            if (rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, CharacterContext.ConditionReduceSneakDice.Name,
                    out var activeCondition))
            {
                var newDiceNumber = Math.Max(damageForm.diceNumber - activeCondition.amount, 0);

                var title = Gui.Format("Reaction/&ReactionSpendPowerBundlePowerRogueCunningStrikeTitle");
                var description = Gui.Format("Reaction/&ReactionSpendPowerBundlePowerRogueCunningStrikeDescription");

                rulesetAttacker.LogCharacterActivatesAbility(
                    title, "Feedback/&ChangeSneakDiceNumber",
                    tooltipContent: description, indent: true,
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.AbilityInfo, damageForm.diceNumber.ToString()),
                        (ConsoleStyleDuplet.ParameterType.Negative, newDiceNumber.ToString())
                    ]);

                damageForm.diceNumber = newDiceNumber;
            }

            // handle arcane scoundrel distracting ambush feature
            var arcaneScoundrelLevels = rulesetAttacker.GetSubclassLevel(Rogue, RoguishArcaneScoundrel.Name);

            if (arcaneScoundrelLevels >= RoguishArcaneScoundrel.DistractingAmbushLevel)
            {
                RoguishArcaneScoundrel.InflictConditionDistractingAmbush(rulesetAttacker, defender.RulesetCharacter);
            }

            // handle slayer chain of execution features
            var slayerLevels = rulesetAttacker.GetSubclassLevel(Rogue, RoguishSlayer.Name);

            if (slayerLevels >= RoguishSlayer.ChainOfExecutionLevel)
            {
                RoguishSlayer.InflictConditionChainOfExecution(rulesetAttacker, defender.RulesetCharacter);

                if (rulesetAttacker.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, RoguishSlayer.ConditionChainOfExecutionBeneficialName,
                        out activeCondition))
                {
                    var newDiceNumber = damageForm.DiceNumber + slayerLevels switch
                    {
                        >= 17 => 5,
                        >= 13 => 4,
                        _ => 3
                    };

                    var title = Gui.Format("Feature/&FeatureRoguishSlayerChainOfExecutionTitle");
                    var description = Gui.Format("Feature/&FeatureRoguishSlayerChainOfExecutionDescription");

                    rulesetAttacker.LogCharacterActivatesAbility(
                        title, "Feedback/&ChangeSneakDiceNumber",
                        tooltipContent: description, indent: true,
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.AbilityInfo, damageForm.diceNumber.ToString()),
                            (ConsoleStyleDuplet.ParameterType.Positive, newDiceNumber.ToString())
                        ]);

                    damageForm.DiceNumber = newDiceNumber;
                    rulesetAttacker.RemoveCondition(activeCondition);
                }
            }

            // handle umbral stalker gloomblade feature
            // ReSharper disable once InvertIf
            if (rulesetAttacker.GetSubclassLevel(Rogue, RoguishUmbralStalker.Name) > 0 &&
                ValidatorsWeapon.IsMelee(attackMode) &&
                rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.GloomBladeToggle))
            {
                var title = Gui.Format("Feature/&ActionAffinityGloomBladeToggleTitle");
                var description = Gui.Format("Feature/&ActionAffinityGloomBladeToggleDescription");

                rulesetAttacker.LogCharacterActivatesAbility(
                    title, "Feedback/&ChangeSneakDiceDamageType",
                    tooltipContent: description, indent: true,
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.Localize("Tooltip/&TagDamageNecroticTitle"))
                    ]);

                damageForm.DamageType = DamageTypeNecrotic;
            }
        }
    }
}
