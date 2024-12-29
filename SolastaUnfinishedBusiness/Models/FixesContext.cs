using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using TA.AI;
using UnityEngine;
using static AttributeDefinitions;
using static EquipmentDefinitions;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalActions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDieRollModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class FixesContext
{
    internal static readonly DecisionDefinition DecisionMoveAfraid =
        DatabaseRepository.GetDatabase<DecisionDefinition>().GetElement("Move_Afraid");

    internal static readonly FeatureDefinitionFeatureSet FeatureSetMonkPatientDefense =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetMonkPatientDefense")
            .SetGuiPresentation("PatientDefense", Category.Feature)
            .SetFeatureSet(PowerMonkPatientDefense)
            .AddToDB();

    internal static void Load()
    {
        InitMagicAffinitiesAndCastSpells();
        FixMinorMagicEffectsIssues();
    }

    internal static void LateLoad()
    {
        // fix condition UI
        FeatureDefinitionCombatAffinitys.CombatAffinityForeknowledge.GuiPresentation.Description = Gui.NoLocalization;

        // fix demonic influence duration and combat log (conditions with ForcedBehavior should have special duration)
        ConditionDefinitions.ConditionUnderDemonicInfluence.specialDuration = true;
        ConditionDefinitions.ConditionUnderDemonicInfluence.durationType = DurationType.Hour;
        ConditionDefinitions.ConditionUnderDemonicInfluence.durationParameter = 1;
        ConditionDefinitions.ConditionUnderDemonicInfluence.possessive = true;

        AddAdditionalActionTitles();
        FixMonkPatientDefenseToAFeatureSet();
        ExtendCharmImmunityToDemonicInfluence();
        FixAdditionalDamageRestrictions();
        FixAdditionalDamageRogueSneakAttack();
        FixArmorClassOnLegendaryArmors();
        FixAttackBuffsAffectingSpellDamage();
        FixBlackDragonLegendaryActions();
        FixColorTables();
        FixCriticalThresholdModifiers();
        FixDecisionMoveAfraid();
        FixDivineBlade();
        FixDragonBreathPowerSavingAttribute();
        FixEagerForBattleTexts();
        FixFightingStyleArchery();
        FixGorillaWildShapeRocksToUnlimited();
        FixGreatWeaponFightingStyle();
        FixLanguagesPointPoolsToIncludeAllLanguages();
        FixMartialArtsProgression();
        FixMartialCommanderCoordinatedDefense();
        FixMeleeRetaliationWithReach();
        FixMountaineerBonusShoveRestrictions();
        FixMummyDreadfulGlareSavingAttribute();
        FixPowerDragonbornBreathWeaponDiceProgression();
        FixRecklessAttackForReachWeaponsAndPathOfTheYeoman();
        FixRestPowerVisibility();
        FixSavingThrowAffinityConditionRaging();
        FixSavingThrowAffinityManaPainterAbsorption();
        FixSorcererChildRiftRiftWalk();
        FixSmitesAndStrikesDiceProgression();
        FixStunningStrikeForAnyMonkWeapon();
        FixTwinnedMetamagic();
        FixUncannyDodgeForRoguishDuelist();
        FixPaladinAurasDisplayOnActionBar();
        ReportDashing();
        FixSpikeGrowthAffectingAir();
        NoTwinnedBladeCantrips();
        FixStaffOfFireToGetFireResistance();

        // fix Dazzled attribute modifier UI previously displaying Daaaaal on attribute modifier
        AttributeModifierDazzled.GuiPresentation.title = "Feature/&AttributeModifierDazzledTitle";
        AttributeModifierDazzled.GuiPresentation.description = Gui.EmptyContent;

        // remove null features from conditions
        foreach (var condition in DatabaseRepository.GetDatabase<ConditionDefinition>())
        {
            condition.Features.RemoveAll(x => !x);
        }

        // avoid breaking mod if anyone changes settings file manually
        Main.Settings.OverridePartySize = Math.Min(Main.Settings.OverridePartySize, ToolsContext.MaxPartySize);
    }

    private static void FixMonkPatientDefenseToAFeatureSet()
    {
        Monk.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PowerMonkPatientDefense);
        Monk.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetMonkPatientDefense, 2));
    }

    private static void NoTwinnedBladeCantrips()
    {
        MetamagicOptionDefinitions.MetamagicTwinnedSpell.AddCustomSubFeatures(NoTwinned.Validator);
    }

    private static void FixStaffOfFireToGetFireResistance()
    {
        ItemDefinitions.StaffOfFire.StaticProperties.Add(
            new ItemPropertyDescription(ItemDefinitions.RingFeatherFalling.StaticProperties[0])
            {
                appliesOnItemOnly = false,
                type = ItemPropertyDescription.PropertyType.Feature,
                featureDefinition = DamageAffinityFireResistance,
                conditionDefinition = null,
                knowledgeAffinity = KnowledgeAffinity.ActiveAndHidden
            });
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

        var allFeatures = classesFeatures.Concat(subclassesFeatures).Concat(racesFeatures).ToArray();
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
            CastSpellTiefling.slotsPerLevels.Add(
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                {
                    Level = level, Slots = CastSpellTiefling.slotsPerLevels[15].slots
                });

            CastSpellTiefling.KnownCantrips[level] = 1;

            // Gnome
            CastSpellGnomeShadow.slotsPerLevels.Add(
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                {
                    Level = level, Slots = CastSpellGnomeShadow.slotsPerLevels[15].slots
                });

            CastSpellGnomeShadow.KnownCantrips[level] = 1;

            // Tradition Light
            CastSpellTraditionLight.slotsPerLevels.Add(
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                {
                    Level = level, Slots = CastSpellTraditionLight.slotsPerLevels[15].slots
                });

            CastSpellTraditionLight.KnownCantrips[level] = 2;

            // Warlock
            CastSpellWarlock.slotsPerLevels[level - 1].slots =
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

            CastSpellWarlock.KnownCantrips[level - 1] = 4;
        }
    }

    private static void FixSavingThrowAffinityConditionRaging()
    {
        SavingThrowAffinityConditionRaging.AffinityGroups[0].savingThrowContext = (SavingThrowContext)10;
    }

    private static void FixSavingThrowAffinityManaPainterAbsorption()
    {
        SavingThrowAffinityManaPainterAbsorption.AffinityGroups.Clear();
    }

    private static void FixSorcererChildRiftRiftWalk()
    {
        SorcerousChildRift.FeatureUnlocks.RemoveAll(x => x.Level == 14);
        SorcerousChildRift.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetSorcererChildRiftRiftwalk, 14));
        FeatureSetSorcererChildRiftRiftwalk.GuiPresentation = PowerSorcererChildRiftRiftwalk.GuiPresentation;
    }

    private static void FixGreatWeaponFightingStyle()
    {
        var conditionGreatWeapon = ConditionDefinitionBuilder
            .Create("ConditionGreatWeapon")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(DieRollModifierFightingStyleGreatWeapon)
            .AddToDB();

        DieRollModifierFightingStyleGreatWeapon.AddCustomSubFeatures(new AllowRerollDiceGreatWeapon());
        FightingStyleDefinitions.GreatWeapon.AddCustomSubFeatures(new CustomBehaviorGreatWeapon(conditionGreatWeapon));
    }

    private static void FixLanguagesPointPoolsToIncludeAllLanguages()
    {
        var dlcLanguages = new List<string> { "Language_Abyssal", "Language_Gnomish", "Language_Infernal" };

        PointPoolBackgroundLanguageChoice_one.RestrictedChoices.AddRange(dlcLanguages);
        PointPoolBackgroundLanguageChoice_two.RestrictedChoices.AddRange(dlcLanguages);
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
        PowerDragonBreath_Acid.EffectDescription.savingThrowAbility = Dexterity;
        PowerDragonBreath_Acid_Spectral_DLC3.EffectDescription.savingThrowAbility = Dexterity;
        PowerDragonBreath_Fire.EffectDescription.savingThrowAbility = Dexterity;
        PowerDragonBreath_YoungGreen_Poison.EffectDescription.savingThrowAbility = Constitution;
    }

    private static void FixBlackDragonLegendaryActions()
    {
        BlackDragon_MasterOfNecromancy.LegendaryActionOptions.SetRange(GoldDragon_AerElai.LegendaryActionOptions);
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
        Power_Mummy_DreadfulGlare.EffectDescription.savingThrowAbility = Wisdom;
        Power_MummyLord_DreadfulGlare.EffectDescription.savingThrowAbility = Wisdom;
    }

    private static void FixAdditionalDamageRestrictions()
    {
        //BUGFIX: Some vanilla additional damage definitions have incorrect attributes
        AdditionalDamageDomainLifeDivineStrike.attackModeOnly = true;
        AdditionalDamageDomainLifeDivineStrike.requiredProperty = RestrictedContextRequiredProperty.Weapon;

        AdditionalDamageDomainMischiefDivineStrike.attackModeOnly = true;
        AdditionalDamageDomainMischiefDivineStrike.requiredProperty = RestrictedContextRequiredProperty.Weapon;

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

        AdditionalDamageRangerSwiftBladeBattleFocus.attackModeOnly = true;
        AdditionalDamageRangerSwiftBladeBattleFocus.requiredProperty = RestrictedContextRequiredProperty.MeleeWeapon;
    }

    private static void FixAttackBuffsAffectingSpellDamage()
    {
        //BUGFIX: fix Branding Smite applying bonus damage to spells
        AdditionalDamageBrandingSmite.attackModeOnly = true;

        //BUGFIX: fix Divine Favor applying bonus damage to spells
        AdditionalDamageDivineFavor.attackModeOnly = true;
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

    private static void FixDecisionMoveAfraid()
    {
        //BUGFIX: allow actors to move a bit far on move_afraid by lowering consideration weight a bit
        DecisionMoveAfraid.Decision.scorer.Scorer.WeightedConsiderations[3].weight = 0.95f;
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
        ProficiencyDruidWeapon.proficiencies.Add(ConjuredWeaponTypeName);
        ProficiencySorcererWeapon.proficiencies.Add(ConjuredWeaponTypeName);
        ProficiencyWizardWeapon.proficiencies.Add(ConjuredWeaponTypeName);
    }

    private static void FixFightingStyleArchery()
    {
        //BEHAVIOR: allow darts, lightning launcher or hand crossbows benefit from Archery Fighting Style
        AttackModifierFightingStyleArchery.AddCustomSubFeatures(
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
            AttackModifierMonkMartialArtsImprovedDamage,
            AttackModifierMonkMartialArtsUnarmedStrikeBonus,
            AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonus,
            AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedom
        };

        foreach (var feature in features)
        {
            feature.AddCustomSubFeatures(provider);
        }
    }

    private static void FixMartialCommanderCoordinatedDefense()
    {
        ActionAffinityMartialCommanderCoordinatedDefense.AddCustomSubFeatures(
            new ValidateDefinitionApplication(ValidatorsCharacter.HasAttacked));
    }

    private static void FixMinorMagicEffectsIssues()
    {
        // fix issues with bad targeting
        Darkness.EffectDescription.targetFilteringMethod = TargetFilteringMethod.AllCharacterAndGadgets;
        Daylight.EffectDescription.targetFilteringMethod = TargetFilteringMethod.AllCharacterAndGadgets;
        WindWall.EffectDescription.targetFilteringMethod = TargetFilteringMethod.AllCharacterAndGadgets;

        // not exactly spells but these are all Darkness variations for enemies
        PowerDefilerDarkness.EffectDescription.targetFilteringMethod =
            TargetFilteringMethod.AllCharacterAndGadgets;
        PowerSorakWordOfDarkness.EffectDescription.targetFilteringMethod =
            TargetFilteringMethod.AllCharacterAndGadgets;
        // well :-)
        PowerDomainSunIndomitableLight.EffectDescription.targetFilteringMethod =
            TargetFilteringMethod.AllCharacterAndGadgets;

        // fix Resurrection
        Resurrection.EffectDescription.EffectForms[0].ReviveForm.maxSecondsSinceDeath = 864000;

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
            SavingThrowAffinityConditionBlinded);

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
        WallOfFire.EffectDescription.savingThrowAbility = Dexterity;
        WallOfFire.EffectDescription.EffectForms[0].hasSavingThrow = true;
        WallOfFire.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.HalfDamage;

        WallOfFireLine.EffectDescription.hasSavingThrow = true;
        WallOfFireLine.EffectDescription.savingThrowAbility = Dexterity;
        WallOfFireLine.EffectDescription.EffectForms[0].hasSavingThrow = true;
        WallOfFireLine.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.HalfDamage;

        WallOfFireRing_Inner.EffectDescription.hasSavingThrow = true;
        WallOfFireRing_Inner.EffectDescription.savingThrowAbility = Dexterity;
        WallOfFireRing_Inner.EffectDescription.EffectForms[0].hasSavingThrow = true;
        WallOfFireRing_Inner.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.HalfDamage;

        WallOfFireRing_Outer.EffectDescription.hasSavingThrow = true;
        WallOfFireRing_Outer.EffectDescription.savingThrowAbility = Dexterity;
        WallOfFireRing_Outer.EffectDescription.EffectForms[0].hasSavingThrow = true;
        WallOfFireRing_Outer.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.HalfDamage;

        //BUGFIX: Insect Plague should have a CON saving throw
        InsectPlague.EffectDescription.hasSavingThrow = true;
        InsectPlague.EffectDescription.savingThrowAbility = Constitution;
        InsectPlague.EffectDescription.EffectForms[0].hasSavingThrow = true;
        InsectPlague.EffectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.HalfDamage;

        //BUGFIX: Ray of Enfeeblement should be recurrent on activation and turn start
        RayOfEnfeeblement.EffectDescription.HasSavingThrow = false;
        RayOfEnfeeblement.EffectDescription.RangeType = RangeType.RangeHit;
        RayOfEnfeeblement.EffectDescription.EffectForms[0].canSaveToCancel = true;
        RayOfEnfeeblement.EffectDescription.recurrentEffect =
            RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart;

        //BUGFIX: Sorcerers should have Fire Shield at 4 and Insect Plague at level 5
        SpellListSorcerer.SpellsByLevel.FirstOrDefault(x => x.Level == 4)!.Spells.Add(FireShield);
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

    private static void FixMeleeRetaliationWithReach()
    {
        // max possible reach in game is 15 ft
        DamageAffinityPatronTreePiercingBranch.retaliateRangeCells = 3;
        DamageAffinityPatronTreePiercingBranchOneWithTheTree.retaliateRangeCells = 3;
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

    private static void FixRestPowerVisibility()
    {
        PowerCircleLandNaturalRecovery.AddCustomSubFeatures(ModifyPowerVisibility.Hidden);
        PowerMarksmanRecycler.AddCustomSubFeatures(ModifyPowerVisibility.Hidden);
        PowerSorcererManaPainterTap.AddCustomSubFeatures(ModifyPowerVisibility.Hidden);
        PowerWizardArcaneRecovery.AddCustomSubFeatures(ModifyPowerVisibility.Hidden);
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
        PowerMonkStunningStrike.activationTime = ActivationTime.NoCost;
        PowerMonkStunningStrike.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
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

                    // handle Wither and Bloom special case
                    if (spell.Name != "WitherAndBloom" &&
                        (effectDescription.TargetType is not (TargetType.Individuals or TargetType.IndividualsUnique) ||
                         spell.ComputeTargetParameter() <= 1))
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

    private static void FixPaladinAurasDisplayOnActionBar()
    {
        foreach (var power in DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
                     .Where(x =>
                         x.ActivationTime is ActivationTime.Permanent or ActivationTime.PermanentUnlessIncapacitated &&
                         (x.Name.StartsWith("PowerDomain") ||
                          x.Name.StartsWith("PowerOath") ||
                          x.Name.StartsWith("PowerPaladin"))))
        {
            power.AddCustomSubFeatures(ModifyPowerVisibility.Hidden);
        }
    }

    private static void ReportDashing()
    {
        Report(ConditionDefinitions.ConditionDashing);
        Report(ConditionDefinitions.ConditionDashingAdditional);
        Report(ConditionDefinitions.ConditionDashingAdditionalSwiftBlade);
        Report(ConditionDefinitions.ConditionDashingBonus);
        Report(ConditionDefinitions.ConditionDashingBonusAdditional);
        Report(ConditionDefinitions.ConditionDashingBonusStepOfTheWind);
        Report(ConditionDefinitions.ConditionDashingBonusSwiftBlade);
        Report(ConditionDefinitions.ConditionDashingBonusSwiftSteps);
        Report(ConditionDefinitions.ConditionDashingExpeditiousRetreat);
        Report(ConditionDefinitions.ConditionDashingExpeditiousRetreatSwiftBlade);
        Report(ConditionDefinitions.ConditionDashingSwiftBlade);
        return;

        static void Report(ConditionDefinition condition)
        {
            condition.GuiPresentation.Title = "Screen/&DashModeTitle";
            condition.GuiPresentation.hidden = false;
            condition.silentWhenAdded = false;
        }
    }

    private static void FixSpikeGrowthAffectingAir()
    {
        SpikeGrowth.EffectDescription.affectOnlyGround = true;
    }

    private static void FixAdditionalDamageRogueSneakAttack()
    {
        AdditionalDamageRogueSneakAttack.AddCustomSubFeatures(
            new ModifyAdditionalDamageRogueSneakAttack(
                "AdditionalDamageRogueSneakAttack",
                "AdditionalDamageRoguishHoodlumNonFinesseSneakAttack",
                "AdditionalDamageRoguishDuelistDaringDuel",
                "AdditionalDamageRoguishUmbralStalkerDeadlyShadows"));
    }

    private static void FixCriticalThresholdModifiers()
    {
        //Changes Champion's Improved Critical to set crit threshold to 19, instead of forcing if worse - fixes stacking with feats
        AttributeModifierMartialChampionImprovedCritical.modifierOperation = AttributeModifierOperation.Set;

        //Changes Champion's Superior Critical to set crit threshold to 18, instead of forcing if worse - fixes stacking with feats
        AttributeModifierMartialChampionSuperiorCritical.modifierOperation = AttributeModifierOperation.Set;

        //Changes critical threshold of Sudden Death dagger to set
        AttributeModifierCriticalThresholdDLC3_Dwarven_Weapon_DaggerPlus3.modifierOperation =
            AttributeModifierOperation.Set;
        AttributeModifierCriticalThresholdDLC3_Dwarven_Weapon_DaggerPlus3.modifierValue = 18;
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
        AdditionalActionHasted.GuiPresentation.Title = Haste.GuiPresentation.Title;
        AdditionalActionSurgedMain.GuiPresentation.Title =
            DatabaseHelper.ActionDefinitions.ActionSurge.GuiPresentation.Title;
    }

    internal sealed class NoTwinned
    {
        public static readonly ValidateMetamagicApplication Validator =
            (RulesetCharacter _, RulesetEffectSpell spell, MetamagicOptionDefinition _, ref bool result,
                ref string failure) =>
            {
                if (!spell.SpellDefinition.HasSubFeatureOfType<NoTwinned>())
                {
                    return;
                }

                result = false;
                failure = "Failure/&FailureFlagInvalidSingleTarget";
            };

        public static NoTwinned Mark { get; } = new();
    }

    private sealed class AllowRerollDiceGreatWeapon : IAllowRerollDice
    {
        public bool IsValid(RulesetActor rulesetActor, bool attackModeDamage, DamageForm damageForm)
        {
            return attackModeDamage;
        }
    }

    private sealed class CustomBehaviorGreatWeapon(ConditionDefinition conditionGreatWeapon)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
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
            var rulesetAttacker = attacker.RulesetCharacter;

            // is not Two-Handed and is not Versatile with free hand
            if (!ValidatorsWeapon.IsTwoHanded(attackMode) &&
                (!ValidatorsCharacter.HasFreeHandConsiderGrapple(rulesetAttacker) ||
                 !ValidatorsWeapon.HasAnyWeaponTag(
                     attackMode.SourceDefinition as ItemDefinition, TagsDefinitions.WeaponTagVersatile)))
            {
                yield break;
            }

            // allow damage die reroll from this point on
            rulesetAttacker.InflictCondition(
                conditionGreatWeapon.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionGreatWeapon.Name,
                0,
                0,
                0);
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker.TryGetConditionOfCategoryAndType(
                    TagEffect, conditionGreatWeapon.Name, out var activeCondition))
            {
                rulesetAttacker.RemoveCondition(activeCondition);
            }

            yield break;
        }
    }

    private sealed class PhysicalAttackFinishedByMeStunningStrike : IPhysicalAttackFinishedByMe,
        IPowerOrSpellFinishedByMe
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
            var usablePower = PowerProvider.Get(PowerMonkStunningStrike, rulesetAttacker);

            if (rulesetAttacker.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            // ensure Zen Archer Hail of Arrows won't trigger stunning strike without ki points
            if (attacker.UsedSpecialFeatures.ContainsKey(WayOfZenArchery.HailOfArrowsAttack))
            {
                var hasTab =
                    attacker.UsedSpecialFeatures.TryGetValue(WayOfZenArchery.HailOfArrowsAttacksTab, out var attacks);

                if (hasTab)
                {
                    attacker.UsedSpecialFeatures[WayOfZenArchery.HailOfArrowsAttacksTab] += 1;
                }
                else
                {
                    attacker.UsedSpecialFeatures.Add(WayOfZenArchery.HailOfArrowsAttacksTab, 1);
                }

                if (rulesetAttacker.RemainingKiPoints - attacks <= 0)
                {
                    yield break;
                }
            }

            var wayOfZenArcheryLevels = rulesetAttacker.GetSubclassLevel(Monk, WayOfZenArchery.Name);

            // Zen Archery get stunning strike with bows at 6
            if (!ValidatorsWeapon.IsMeleeOrUnarmed(attackMode) &&
                (wayOfZenArcheryLevels < WayOfZenArchery.StunningStrikeWithBowAllowedLevel ||
                 !ValidatorsCharacter.HasBowWithoutArmor(rulesetAttacker)))
            {
                yield break;
            }

            if (!attacker.IsActionOnGoing(ActionDefinitions.Id.StunningStrikeToggle))
            {
                yield break;
            }

            // support Stunning Strike 2024 behavior
            var stunningMark = Tabletop2024Context.ConditionStunningStrikeMark.Name;

            if (Main.Settings.EnableMonkStunningStrike2024 &&
                rulesetAttacker.HasConditionOfCategoryAndType(TagEffect, stunningMark))
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                stunningMark,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                stunningMark,
                0,
                0,
                0);

            attacker.MyExecuteActionSpendPower(usablePower, defender);
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action.SaveOutcome == RollOutcome.Failure)
            {
                action.ActingCharacter.RulesetCharacter.ToggledPowersOn
                    .Remove(PowerMonkStunningStrike.AutoActivationPowerTag);
            }

            yield break;
        }
    }

    //
    // CONSOLIDATED SNEAK ATTACK DAMAGE FORM MODIFIER
    //

    private sealed class ModifyAdditionalDamageRogueSneakAttack(params string[] additionalDamages)
        : IModifyAdditionalDamage
    {
        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm damageForm)
        {
            if (!additionalDamages.Contains(featureDefinitionAdditionalDamage.Name))
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
            var conditions = new List<RulesetCondition>();

            rulesetAttacker.GetAllConditionsOfType(conditions, Tabletop2024Context.ConditionReduceSneakDice.Name);

            foreach (var activeCondition in conditions)
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
                        TagEffect, RoguishSlayer.ConditionChainOfExecutionBeneficialName,
                        out var activeCondition) &&
                    activeCondition.SourceGuid == rulesetAttacker.Guid)
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

            // ReSharper disable once InvertIf
            // handle umbral stalker gloomblade feature
            if (rulesetAttacker.GetSubclassLevel(Rogue, RoguishUmbralStalker.Name) > 0 &&
                rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.GloomBladeToggle))
            {
                damageForm.DamageType = DamageTypeNecrotic;
            }
        }
    }
}
