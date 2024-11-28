using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardEvocation : AbstractSubclass
{
    private const string Name = "WizardEvocation";
    internal const string SpellTag = "Evoker";

    //
    // these lists contain all evocation spells that do damage in a non-vanilla way so they also get bonus
    //

    private static readonly string[] CantripsAdditionalDamages =
    [
        "AdditionalDamageBoomingBlade",
        "AdditionalDamageResonatingStrike", // Green-Flame Blade
        "AdditionalDamageSunlightBlade"
    ];

    private static readonly string[] SpellsAdditionalDamages =
    [
        "AdditionalDamageBanishingSmite",
        "AdditionalDamageBlindingSmite",
        "AdditionalDamageHolyWeapon",
        "AdditionalDamageSearingSmite",
        "AdditionalDamageStaggeringSmite",
        "AdditionalDamageWrathfulSmite"
    ];

    private static readonly string[] SpellsPowerDamages =
    [
        "PowerCrownOfStars",
        "PowerHolyWeapon",
        "PowerThunderousSmite"
    ];

    private static readonly FeatureDefinition FeatureSculptSpells = FeatureDefinitionBuilder
        .Create($"Feature{Name}SculptSpells")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new MagicEffectInitiatedByMeSculptSpells())
        .AddToDB();

    private static readonly FeatureDefinitionMagicAffinity MagicAffinityPotentCantrip =
        FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}PotentCantrip")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnEnemyPotentCantrips())
            .AddToDB();

    private static readonly FeatureDefinitionMagicAffinity MagicAffinitySavant = FeatureDefinitionMagicAffinityBuilder
        .Create($"MagicAffinity{Name}Savant")
        .SetGuiPresentation(Category.Feature)
        .SetSpellLearnAndPrepModifiers(
            0.5f, 0.5f, 0, AdvantageType.None, PreparedSpellsModifier.None)
        .AddCustomSubFeatures(new ModifyScribeCostAndDurationEvocationSavant())
        .AddToDB();

    private static readonly SpellListDefinition SpellListEvoker = SpellListDefinitionBuilder
        .Create(SpellListDefinitions.SpellListWizard, $"SpellList{Name}")
        .AddToDB();

    // no spell tag here as this work correctly with vanilla
    private static readonly FeatureDefinitionPointPool MagicAffinitySavant2024 = FeatureDefinitionPointPoolBuilder
        .Create($"MagicAffinity{Name}Savant2024")
        .SetGuiPresentation(Category.Feature)
        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Spell, 2, SpellListEvoker)
        .AddToDB();

    // need spell tag here to get this offered on level up and
    // let custom behavior at CharacterBuildingManager.FinalizeCharacter grant the spell
    private static readonly FeatureDefinitionPointPool MagicAffinitySavant2024Progression =
        FeatureDefinitionPointPoolBuilder
            .Create($"MagicAffinity{Name}Savant2024Progression")
            .SetGuiPresentationNoContent(true)
            .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Spell, 1, SpellListEvoker, SpellTag)
            .AddToDB();

    private static CharacterSubclassDefinition _subclass;

    public WizardEvocation()
    {
        // LEVEL 02

        // Evocation Savant

        // Sculpt Spells

        // LEVEL 06

        // Potent Cantrip

        MagicAffinityPotentCantrip.forceHalfDamageOnCantrips = true;

        // LEVEL 10

        // Empowered Evocation

        var featureEmpoweredEvocation = FeatureDefinitionBuilder
            .Create($"Feature{Name}EmpoweredEvocation")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnEnemyEmpoweredEvocation())
            .AddToDB();

        // LEVEL 14

        // Over Channel

        var conditionOverChannel = ConditionDefinitionBuilder
            .Create($"Condition{Name}OverChannel")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AllowMultipleInstances()
            .AddToDB();

        var conditionMaxDamage = ConditionDefinitionBuilder
            .Create($"Condition{Name}OverChannelMaxDamage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(new ForceMaxDamageTypeDependentOverChannel())
            .AddToDB();

        var actionAffinityOverChannelToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityOverChannelToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.OverChannelToggle)
            .AddToDB();

        actionAffinityOverChannelToggle.AddCustomSubFeatures(
            new CustomBehaviorOverChannel(actionAffinityOverChannelToggle, conditionOverChannel, conditionMaxDamage));

        var featureSetOverChannel = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}OverChannel")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(actionAffinityOverChannelToggle)
            .AddToDB();

        //
        // Main
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardEvocation, 256))
            .AddFeaturesAtLevel(2, MagicAffinitySavant, FeatureSculptSpells)
            .AddFeaturesAtLevel(6, MagicAffinityPotentCantrip)
            .AddFeaturesAtLevel(10, featureEmpoweredEvocation)
            .AddFeaturesAtLevel(14, featureSetOverChannel)
            .AddToDB();

        _subclass = Subclass;
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void LateLoad()
    {
        SwapSavantAndSavant2024();

        SpellListEvoker.SpellsByLevel.SetRange(
            SpellListDefinitions.SpellListWizard.SpellsByLevel
                .Select(spellByLevel => new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = spellByLevel.Level,
                    Spells = [.. spellByLevel.Spells.Where(x => x.SchoolOfMagic == SchoolEvocation)]
                }));
    }

    internal static void SwapEvocationPotentCantripAndSculptSpell()
    {
        var level = Main.Settings.EnableWizardToLearnSchoolAtLevel3 ? 3 : 2;
        var featureUnlockSculptSpell = _subclass.FeatureUnlocks.FirstOrDefault(x =>
            x.FeatureDefinition == FeatureSculptSpells);
        var featureUnlockMagicAffinityPotentCantrip = _subclass.FeatureUnlocks.FirstOrDefault(x =>
            x.FeatureDefinition == MagicAffinityPotentCantrip);

        if (Main.Settings.SwapEvocationPotentCantripAndSculptSpell)
        {
            featureUnlockSculptSpell!.level = 6;
            featureUnlockMagicAffinityPotentCantrip!.level = level;
        }
        else
        {
            featureUnlockSculptSpell!.level = level;
            featureUnlockMagicAffinityPotentCantrip!.level = 6;
        }

        _subclass.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwapSavantAndSavant2024()
    {
        var level = Main.Settings.EnableWizardToLearnSchoolAtLevel3 ? 3 : 2;

        _subclass.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == MagicAffinitySavant ||
            x.FeatureDefinition == MagicAffinitySavant2024 ||
            x.FeatureDefinition == MagicAffinitySavant2024Progression);

        if (Main.Settings.SwapEvocationSavant)
        {
            _subclass.FeatureUnlocks.Add(new FeatureUnlockByLevel(MagicAffinitySavant2024, level));

            for (var i = 5; i <= 20; i += 2)
            {
                _subclass.FeatureUnlocks.Add(new FeatureUnlockByLevel(MagicAffinitySavant2024Progression, i));
            }
        }
        else
        {
            _subclass.FeatureUnlocks.Add(new FeatureUnlockByLevel(MagicAffinitySavant, level));
        }

        _subclass.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    //
    // Evocation Savant
    //

    private sealed class ModifyScribeCostAndDurationEvocationSavant : IModifyScribeCostAndDuration
    {
        public void ModifyScribeCostMultiplier(
            RulesetCharacter character, SpellDefinition spellDefinition, ref float costMultiplier)
        {
            if (spellDefinition.SchoolOfMagic != SchoolEvocation)
            {
                costMultiplier = 1;
            }
        }

        public void ModifyScribeDurationMultiplier(
            RulesetCharacter character, SpellDefinition spellDefinition, ref float durationMultiplier)
        {
            if (spellDefinition.SchoolOfMagic != SchoolEvocation)
            {
                durationMultiplier = 1;
            }
        }
    }

    //
    // Sculpt Spells
    //

    private sealed class MagicEffectInitiatedByMeSculptSpells : IMagicEffectInitiatedByMe
    {
        public IEnumerator OnMagicEffectInitiatedByMe(
            CharacterAction action,
            RulesetEffect rulesetEffect,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (rulesetEffect.SourceDefinition is not SpellDefinition { SchoolOfMagic: SchoolEvocation } spell ||
                spell.EffectDescription.TargetSide == Side.Ally ||
                spell.EffectDescription.TargetType == TargetType.Self)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var hasAlly = false;

            foreach (var ally in targets
                         .Where(x => !x.IsOppositeSide(attacker.Side) && attacker.CanPerceiveTarget(x))
                         .ToArray())
            {
                hasAlly = true;
                targets.Remove(ally);
            }

            if (hasAlly)
            {
                rulesetAttacker.LogCharacterUsedFeature(FeatureSculptSpells);
            }
        }
    }

    //
    // Potent Cantrips
    //

    private sealed class MagicEffectBeforeHitConfirmedOnEnemyPotentCantrips
        : IMagicEffectBeforeHitConfirmedOnEnemy, IModifyAdditionalDamage
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var isCantrip = rulesetEffect.SourceDefinition is SpellDefinition { SpellLevel: 0 };

            if (!isCantrip ||
                rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit) ||
                (!firstTarget &&
                 rulesetEffect.EffectDescription.TargetType is TargetType.Individuals or TargetType.IndividualsUnique))
            {
                yield break;
            }

            var effectForm = actualEffectForms
                .FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);

            if (effectForm == null)
            {
                yield break;
            }

            var pb = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            effectForm.DamageForm.BonusDamage += pb;
        }

        // handle special blade cantrips use cases
        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm damageForm)
        {
            if (!CantripsAdditionalDamages.Contains(featureDefinitionAdditionalDamage.Name))
            {
                return;
            }

            var pb = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            damageForm.BonusDamage += pb;
        }
    }

    //
    // Empowered Evocation
    //

    private sealed class MagicEffectBeforeHitConfirmedOnEnemyEmpoweredEvocation
        : IMagicEffectBeforeHitConfirmedOnEnemy, IModifyAdditionalDamage
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var isSpell = rulesetEffect.SourceDefinition is SpellDefinition;

            switch (isSpell)
            {
                case false when !SpellsPowerDamages.Contains(rulesetEffect.SourceDefinition.Name):
                case true when rulesetEffect.SchoolOfMagic != SchoolEvocation:
                case true when !firstTarget &&
                               rulesetEffect.EffectDescription.TargetType
                                   is TargetType.Individuals
                                   or TargetType.IndividualsUnique:
                    yield break;
            }

            var effectForm = actualEffectForms
                .FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);

            if (effectForm == null)
            {
                yield break;
            }

            var intelligenceModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence));

            effectForm.DamageForm.BonusDamage += Math.Max(1, intelligenceModifier);
        }

        // handle special blade cantrips and other spells additional damages use cases
        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm damageForm)
        {
            var featureName = featureDefinitionAdditionalDamage.Name;

            if (!CantripsAdditionalDamages.Contains(featureName) && !SpellsAdditionalDamages.Contains(featureName))
            {
                return;
            }

            var intelligenceModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence));

            damageForm.BonusDamage += Math.Max(1, intelligenceModifier);
        }
    }

    //
    // Over Channel
    //

    private sealed class ForceMaxDamageTypeDependentOverChannel : IForceMaxDamageTypeDependent
    {
        public bool IsValid(RulesetActor rulesetActor, DamageForm damageForm)
        {
            return true;
        }
    }

    private sealed class CustomBehaviorOverChannel(
        FeatureDefinition featureOverChannel,
        ConditionDefinition conditionOverChannel,
        ConditionDefinition conditionOverChannelMaxDamage)
        : IMagicEffectBeforeHitConfirmedOnEnemy, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            // only spells between 1st and 5th levels
            if (!firstTarget ||
                !rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.OverChannelToggle) ||
                rulesetEffect.SourceDefinition is not SpellDefinition spellDefinition ||
                spellDefinition.SpellLevel == 0 ||
                spellDefinition.SpellLevel > 5)
            {
                yield break;
            }

            // allow max spell damage on this attack
            EffectHelpers.StartVisualEffect(
                attacker, attacker, PowerFighterActionSurge, EffectHelpers.EffectType.Caster);
            rulesetAttacker.LogCharacterUsedFeature(featureOverChannel);
            rulesetAttacker.InflictCondition(
                conditionOverChannelMaxDamage.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionOverChannelMaxDamage.Name,
                0,
                0,
                0);
        }

        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action, GameLocationCharacter attacker, List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionOverChannelMaxDamage.Name, out var actionCondition))
            {
                yield break;
            }

            rulesetAttacker.RemoveCondition(actionCondition);

            // add one instance of over channel
            rulesetAttacker.InflictCondition(
                conditionOverChannel.Name,
                DurationType.UntilLongRest,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionOverChannel.Name,
                0,
                0,
                0);

            var overChannelInstancesCount =
                rulesetAttacker.AllConditions.Count(x => x.ConditionDefinition == conditionOverChannel);

            // first time used so no self damage
            if (overChannelInstancesCount <= 1)
            {
                yield break;
            }

            const DieType DIE_TYPE = DieType.D12;

            var rulesetEffect = action.ActionParams.RulesetEffect;
            var diceNumber = overChannelInstancesCount * rulesetEffect.EffectLevel;
            var rolls = new List<int>();
            var damage = rulesetAttacker.RollDiceAndSum(DIE_TYPE, RollContext.None, diceNumber, rolls, false);

            EffectHelpers.StartVisualEffect(
                attacker, attacker, PowerPatronFiendDarkOnesOwnLuck, EffectHelpers.EffectType.Effect);
            rulesetAttacker.SustainDamage(
                damage, DamageTypeNecrotic, false, rulesetAttacker.Guid,
                new RollInfo(DIE_TYPE, rolls, 0), out _);
        }
    }
}
