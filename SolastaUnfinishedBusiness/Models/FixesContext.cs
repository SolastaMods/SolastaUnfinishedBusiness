using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Subclasses;
using UnityEngine;
using static EquipmentDefinitions;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Models;

internal static class FixesContext
{
    internal static void Load()
    {
        InitMagicAffinitiesAndCastSpells();
    }

    internal static void LateLoad()
    {
        FixAdditionalDamageRestrictions();
        FixAttackBuffsAffectingSpellDamage();
        FixColorTables();
        FixDivineBlade();
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
        FixCriticalThresholdModifiers();
        FixEagerForBattleTexts();
        AddAdditionalActionTitles();
        FixRageActionSpending();
        FixGrantBardicInspirationForActionSwitchingFeature();
        FixPowerDragonbornBreathWeaponDiceProgression();
        FixDragonBreathPowerSavingAttribute();
        FixBlackDragonLegendaryActions();
        FixMummyDreadfulGlareSavingAttribute();
        FixArmorClassOnLegendaryArmors();
        ExtendCharmImmunityToDemonicInfluence();
        FixSavingThrowAffinityManaPainterAbsorption();

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

            if (spellListDefinition == null)
            {
                continue;
            }

            var spellsByLevel = spellListDefinition.SpellsByLevel;

            while (spellsByLevel.Count < 10)
            {
                spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = spellsByLevel.Count, Spells = new List<SpellDefinition>()
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

            if (spellListDefinition == null)
            {
                continue;
            }

            var spellsByLevel = spellListDefinition.SpellsByLevel;

            while (spellsByLevel.Count < 10)
            {
                spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = spellsByLevel.Count, Spells = new List<SpellDefinition>()
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
            FeatureDefinitionCastSpells.CastSpellWarlock.slotsPerLevels[level - 1].slots = new List<int>
            {
                0,
                0,
                0,
                0,
                4,
                0,
                0,
                0,
                0
            };

            FeatureDefinitionCastSpells.CastSpellWarlock.KnownCantrips[level - 1] = 4;
        }
    }

    private static void FixSavingThrowAffinityManaPainterAbsorption()
    {
        FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityManaPainterAbsorption.AffinityGroups.Clear();
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
                         .Where(x => x.FeatureDefinition != null &&
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
        AdditionalDamageBrandingSmite.AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack);

        //BUGFIX: fix Divine Favor applying bonus damage to spells
        AdditionalDamageDivineFavor.AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack);
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
            new ValidateContextInsteadOfRestrictedProperty((_, _, _, item, _, _, _) => (OperationType.Set,
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
            .AddCustomSubFeatures(new ValidateDefinitionApplication(ValidatorsCharacter.HasShield));
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
        FeatureDefinitionPowers.PowerMonkStunningStrike.activationTime = ActivationTime.NoCost;
        FeatureDefinitionPowers.PowerMonkStunningStrike.GuiPresentation.hidden = true;
        FeatureDefinitionPowers.PowerMonkStunningStrike.AddCustomSubFeatures(
            new PhysicalAttackFinishedByMeStunningStrike());
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
        ActionAffinityUncannyDodge.AddCustomSubFeatures(new ValidateDefinitionApplication(
            character => character.GetSubclassLevel(Rogue, RoguishDuelist.Name) < 13 ||
                         character.HasConditionOfType(RoguishDuelist.ConditionReflexiveParry)));
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
        //Main Action
        FeatureDefinitionAdditionalActions.AdditionalActionHasted.GuiPresentation.Title
            = Haste.GuiPresentation.Title;
        FeatureDefinitionAdditionalActions.AdditionalActionSurgedMain.GuiPresentation.Title
            = DatabaseHelper.ActionDefinitions.ActionSurge.GuiPresentation.Title;

        //Bonus Action
        // FeatureDefinitionAdditionalActions.AdditionalActionExpeditiousRetreat.GuiPresentation.Title
        //     = ExpeditiousRetreat.GuiPresentation.Title;
    }

    private static void FixRageActionSpending()
    {
        //TA's implementation of Rage Start spends Bonus Action twice - not a big problem in vanilla, but breaks action switching code
        //use our custom rage start class that doesn't have this issue
        DatabaseHelper.ActionDefinitions.RageStart.classNameOverride = "CombatRageStart";
    }

    private static void FixGrantBardicInspirationForActionSwitchingFeature()
    {
        DatabaseHelper.ActionDefinitions.GrantBardicInspiration.classNameOverride = "UsePower";
    }

    private sealed class PhysicalAttackFinishedByMeStunningStrike : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            if (!attacker.IsActionOnGoing(ActionDefinitions.Id.StunningStrikeToggle))
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;
            var wayOfTheDistantHandLevels = rulesetAttacker.GetSubclassLevel(Monk, "WayOfTheDistantHand");

            if (!ValidatorsWeapon.IsMelee(attackMode) && wayOfTheDistantHandLevels < 11)
            {
                yield break;
            }

            var actionParams = action.ActionParams.Clone();
            var usablePower =
                UsablePowersProvider.Get(FeatureDefinitionPowers.PowerMonkStunningStrike, rulesetAttacker);

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                //CHECK: no need for AddAsActivePowerToSource
                .InstantiateEffectPower(rulesetAttacker, usablePower, false);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            actionService.ExecuteAction(actionParams, null, true);
        }
    }
}
