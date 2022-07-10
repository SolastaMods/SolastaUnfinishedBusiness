using System;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpells;
using static SolastaCommunityExpansion.Classes.Witch.Witch;
using static SolastaCommunityExpansion.Models.SpellsContext;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaCommunityExpansion.Spells;

internal static class BazouSpells
{
    private static readonly Guid BazouSpellsBaseGuid = new("91384db5-6659-4384-bf2c-3a41160343f4");

    private static SpellDefinition _eldritchOrb;

    private static SpellDefinition _findFamiliar;

    private static SpellDefinition _frenzy;

    private static SpellDefinition _minorLifesteal;

    private static SpellDefinition _petalStorm;

    private static SpellDefinition _protectThreshold;
    [NotNull] internal static SpellDefinition EldritchOrb => _eldritchOrb ??= BuildEldritchOrb();
    [NotNull] internal static SpellDefinition FindFamiliar => _findFamiliar ??= BuildFindFamiliar();
    [NotNull] internal static SpellDefinition Frenzy => _frenzy ??= BuildFrenzy();
    [NotNull] internal static SpellDefinition MinorLifesteal => _minorLifesteal ??= BuildMinorLifesteal();
    [NotNull] internal static SpellDefinition PetalStorm => _petalStorm ??= BuildPetalStorm();
    [NotNull] internal static SpellDefinition ProtectThreshold => _protectThreshold ??= BuildProtectThreshold();

    // don't need since spells are created when first referenced/used
    /*        internal static void AddToDB()
            {
                _ = EldritchOrb;
                _ = FindFamiliar;
                _ = Frenzy;
                _ = MinorLifesteal;
                _ = PetalStorm;
                _ = ProtectThreshold;
            }
    */
    internal static void Register()
    {
        RegisterSpell(EldritchOrb, 1, WitchSpellList, WarlockSpellList);
        RegisterSpell(FindFamiliar, 1, WitchSpellList, SpellListWizard, WarlockSpellList);
        RegisterSpell(Frenzy, 1, WitchSpellList, WarlockSpellList, SpellListWizard, SpellListSorcerer);
        RegisterSpell(MinorLifesteal, 1, WitchSpellList, SpellListWizard);
        RegisterSpell(PetalStorm, 1, WitchSpellList, SpellListDruid);
        RegisterSpell(ProtectThreshold, 1, WitchSpellList, SpellListCleric, SpellListDruid, SpellListPaladin);
    }

    [NotNull]
    private static SpellDefinition BuildEldritchOrb()
    {
        var spell = SpellDefinitionBuilder
            .Create(MagicMissile, "EldritchOrb", BazouSpellsBaseGuid)
            .SetGuiPresentation(Category.Spell, Shine.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetMaterialComponent(RuleDefinitions.MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(0)
            .AddToDB();

        // Not sure if I prefer copying and editing existing effect description
        // or creating one from scratch through API
        spell.EffectDescription
            .SetRangeType(RuleDefinitions.RangeType.Distance)
            .SetRangeParameter(12)
            .SetDurationType(RuleDefinitions.DurationType.Instantaneous)
            .SetTargetType(RuleDefinitions.TargetType.Sphere)
            .SetTargetParameter(1)
            .SetHasSavingThrow(false)
            .SetSavingThrowAbility(AttributeDefinitions.Dexterity)
            .SetCanBeDispersed(true);

        spell.EffectDescription.EffectAdvancement
            .additionalDicePerIncrement = 1;

        spell.EffectDescription.EffectAdvancement
            .incrementMultiplier = 5;

        spell.EffectDescription.EffectAdvancement
            .effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.CasterLevelTable;

        // Changing to a single damage effect with d4, as I am unsure how to implement 2 different effectDescriptions within the same spell
        // First one should be single target attack roll, d8 damage
        // Second one should be adjacent aoe to first target, half of damage of first effect, no damage on saving throw negates
        var effectForm = spell.EffectDescription.EffectForms[0];
        effectForm.hasSavingThrow = false;
        effectForm.levelMultiplier = 1;
        // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
        //effectForm.AlterationForm.SetMaximumIncrease(2).SetValueIncrease(2);
        effectForm.DamageForm.diceNumber = 1;
        effectForm.DamageForm.dieType = RuleDefinitions.DieType.D4;
        effectForm.DamageForm.damageType = RuleDefinitions.DamageTypeForce;

        // Not sure if I prefer copying and editing existing effect forms
        // or creating one from scratch through API
        //            var effectForm = new EffectFormBuilder().Build();

        //            effectForm.Copy(spell.EffectDescription.EffectForms[0]);
        //            effectForm.hasSavingThrow =(true);
        //            effectForm.savingThrowAffinity =(RuleDefinitions.EffectSavingThrowType.Negates);
        //            effectForm.DamageForm.SetDieType(RuleDefinitions.DieType.D4);

        //            spell.EffectDescription.EffectForms.Add(effectForm);

        return spell;
    }

    [NotNull]
    private static SpellDefinition BuildFindFamiliar()
    {
        var familiarMonsterBuilder = MonsterDefinitionBuilder
            .Create(Eagle_Matriarch, "Owl", BazouSpellsBaseGuid)
            .SetGuiPresentation("OwlFamiliar", Category.Monster, Eagle_Matriarch.GuiPresentation.SpriteReference)
            .SetFeatures(
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision24,
                FeatureDefinitionMoveModes.MoveModeMove2,
                FeatureDefinitionMoveModes.MoveModeFly12,
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenSight,
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenHearing,
                FeatureDefinitionCombatAffinitys.CombatAffinityFlyby,
                FeatureDefinitionMovementAffinitys.MovementAffinityNoClimb,
                FeatureDefinitionMovementAffinitys.MovementAffinityNoSpecialMoves,
                FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity)
            .ClearAttackIterations()
            .SetSkillScores(
                (DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                (DatabaseHelper.SkillDefinitions.Stealth.Name, 3)
            )
            .SetArmorClass(11)
            .SetAbilityScores(3, 13, 8, 2, 12, 7)
            .SetHitDiceNumber(1)
            .SetHitDiceType(RuleDefinitions.DieType.D4)
            .SetHitPointsBonus(-1)
            .SetStandardHitPoints(1)
            .SetSizeDefinition(CharacterSizeDefinitions.Tiny)
            .SetAlignment(AlignmentDefinitions.Neutral.Name)
            .SetCharacterFamily(CharacterFamilyDefinitions.Fey.name)
            .SetChallengeRating(0)
            .SetDroppedLootDefinition(null)
            .SetDefaultBattleDecisionPackage(DecisionPackageDefinitions
                .DefaultSupportCasterWithBackupAttacksDecisions)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction("Party")
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);

        if (DatabaseRepository.GetDatabase<FeatureDefinition>().TryGetElement("HelpAction", out var help))
        {
            familiarMonsterBuilder.AddFeatures(help);
        }

        var familiarMonster = familiarMonsterBuilder.AddToDB();

        var spell = SpellDefinitionBuilder.Create(Fireball, "FindFamiliar", BazouSpellsBaseGuid)
            .SetGuiPresentation(Category.Spell, AnimalFriendship.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Specific)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(1)
            .SetCastingTime(RuleDefinitions.ActivationTime.Hours1)
            // BUG: Unable to have 70 minutes ritual casting time... if set to 10 minutes, it really only takes 10 minutes, instead of 70
            .SetRitualCasting(RuleDefinitions.ActivationTime.Minute10)
            .AddToDB();

        spell.uniqueInstance = true;

        spell.EffectDescription.Copy(ConjureAnimalsOneBeast.EffectDescription);
        spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        spell.EffectDescription.SetRangeParameter(2);
        spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Permanent);
        spell.EffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
        spell.EffectDescription.EffectForms.Clear();

        var summonForm = new SummonForm {monsterDefinitionName = familiarMonster.name, decisionPackage = null};

        var effectForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Summon, createdByCharacter = true, summonForm = summonForm
        };

        spell.EffectDescription.EffectForms.Add(effectForm);

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, spell);

        return spell;
    }

    [NotNull]
    private static SpellDefinition BuildFrenzy()
    {
        var spell = SpellDefinitionBuilder
            .Create(Confusion, "Frenzy", BazouSpellsBaseGuid)
            .SetGuiPresentation(Category.Spell, Confusion.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(6)
            .SetRequiresConcentration(true)
            .AddToDB();

        // Not sure if I prefer copying and editing existing effect description
        // or creating one from scratch through API
        spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        spell.EffectDescription.SetRangeParameter(24);
        spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
        spell.EffectDescription.SetDurationParameter(1);
        spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
        spell.EffectDescription.SetTargetParameter(4);
        spell.EffectDescription.hasSavingThrow = true;
        spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);

        var conditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionConfused, "ConditionFrenzied", BazouSpellsBaseGuid)
            .SetOrUpdateGuiPresentation("Frenzied", Category.Condition)
            .AddToDB();

        // Some methods are missing like SetField or Copy
        var actionAffinity = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinityConditionConfused, "ActionAffinityConditionFrenzied", BazouSpellsBaseGuid)
            .AddToDB();

        actionAffinity.RandomBehaviourOptions.Clear();

        var behaviorMode = new BehaviorModeDescription
        {
            behaviour =
                RuleDefinitions.RandomBehaviour
                    .ConditionDuringTurn, // It will not make the affected creature move towards another creature... :(
            // This condition seems to only attack a creature adjacent to where it is.
            condition = ConditionConfusedAttack,
            weight = 10
        };

        actionAffinity.RandomBehaviourOptions.Add(behaviorMode);
        conditionDefinition.Features.SetRange(actionAffinity);

        spell.EffectDescription.EffectForms[0].ConditionForm.conditionDefinition = conditionDefinition;

        return spell;
    }

    [NotNull]
    private static SpellDefinition BuildMinorLifesteal()
    {
        var spell = SpellDefinitionBuilder
            .Create(VampiricTouch, "MinorLifesteal", BazouSpellsBaseGuid)
            .SetGuiPresentation(Category.Spell, VampiricTouch.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetMaterialComponent(RuleDefinitions.MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(false)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .AddToDB();

        spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        spell.EffectDescription.SetRangeParameter(12);
        spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Instantaneous);
        spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
        spell.EffectDescription.hasSavingThrow = true;
        spell.EffectDescription.SetHalfDamageOnAMiss(false);
        spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Constitution);
        spell.EffectDescription.EffectAdvancement.additionalDicePerIncrement = 1;
        spell.EffectDescription.EffectAdvancement.incrementMultiplier = 5;
        spell.EffectDescription.EffectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod
            .CasterLevelTable;

        spell.EffectDescription.EffectForms[1].hasSavingThrow = true;
        spell.EffectDescription.EffectForms[1]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
        spell.EffectDescription.EffectForms[1].DamageForm.diceNumber = 1;
        spell.EffectDescription.EffectForms[1].DamageForm.dieType = RuleDefinitions.DieType.D4;
        spell.EffectDescription.EffectForms[1].DamageForm.damageType = RuleDefinitions.DamageTypeNecrotic;
        spell.EffectDescription.EffectForms[1].DamageForm
            .healFromInflictedDamage = RuleDefinitions.HealFromInflictedDamage.Full;
        spell.EffectDescription.EffectForms[1].levelMultiplier = 1;
        // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
        //spell.EffectDescription.EffectForms[1].AlterationForm.SetMaximumIncrease(2);
        //spell.EffectDescription.EffectForms[1].AlterationForm.SetValueIncrease(2);

        return spell;
    }

    [NotNull]
    private static SpellDefinition BuildPetalStorm()
    {
        var spell = SpellDefinitionBuilder
            .Create(InsectPlague, "PetalStorm", BazouSpellsBaseGuid)
            .SetGuiPresentation(Category.Spell, WindWall.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(2)
            .SetRequiresConcentration(true)
            .AddToDB();

        // Not sure if I prefer copying and editing existing effect description
        // or creating one from scratch through API
        spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        spell.EffectDescription.SetRangeParameter(12);
        spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
        spell.EffectDescription.SetDurationParameter(1);
        spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cube);
        spell.EffectDescription.SetTargetParameter(3);
        spell.EffectDescription.hasSavingThrow = true;
        spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Strength);
        spell.EffectDescription.SetRecurrentEffect((RuleDefinitions.RecurrentEffect)20);
        spell.EffectDescription.EffectAdvancement.additionalDicePerIncrement = 2;
        spell.EffectDescription.EffectAdvancement.incrementMultiplier = 1;
        spell.EffectDescription.EffectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod
            .PerAdditionalSlotLevel;

        spell.EffectDescription.EffectForms[0].hasSavingThrow = true;
        spell.EffectDescription.EffectForms[0]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
        spell.EffectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        spell.EffectDescription.EffectForms[0].DamageForm.dieType = RuleDefinitions.DieType.D4;
        spell.EffectDescription.EffectForms[0].DamageForm.damageType = RuleDefinitions.DamageTypeSlashing;
        spell.EffectDescription.EffectForms[0].levelMultiplier = 1;
        // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
        //spell.EffectDescription.EffectForms[0].AlterationForm.SetMaximumIncrease(2);
        //spell.EffectDescription.EffectForms[0].AlterationForm.SetValueIncrease(2);

        var effectProxyDefinition = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyInsectPlague, "ProxyPetalStorm", BazouSpellsBaseGuid)
            .SetGuiPresentation("PetalStorm", Category.Spell, WindWall.GuiPresentation.SpriteReference)
            .SetCanMove()
            .SetPortrait(WindWall.GuiPresentation.SpriteReference)
            .AddAdditionalFeatures(FeatureDefinitionMoveModes.MoveModeMove6)
            .AddToDB();

        effectProxyDefinition.actionId = ActionDefinitions.Id.ProxyFlamingSphere;
        effectProxyDefinition.attackMethod = RuleDefinitions.ProxyAttackMethod.ReproduceDamageForms;
        effectProxyDefinition.canMoveOnCharacters = true;
        effectProxyDefinition.isEmptyPresentation = false;

        spell.EffectDescription.EffectForms[2].SummonForm.effectProxyDefinitionName = effectProxyDefinition.Name;

        return spell;
    }

    [NotNull]
    private static SpellDefinition BuildProtectThreshold()
    {
        var spell = SpellDefinitionBuilder
            .Create(SpikeGrowth, "ProtectThreshold", BazouSpellsBaseGuid)
            .SetGuiPresentation(Category.Spell, Bane.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(2)
            .SetRequiresConcentration(false)
            .SetRitualCasting(RuleDefinitions.ActivationTime.Minute10).AddToDB();

        // Not sure if I prefer copying and editing existing effect description
        // or creating one from scratch through API
        spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        spell.EffectDescription.SetRangeParameter(1);
        spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
        spell.EffectDescription.SetDurationParameter(10);
        spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
        spell.EffectDescription.SetTargetParameter(0);
        spell.EffectDescription.hasSavingThrow = true;
        spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);
        spell.EffectDescription.SetRecurrentEffect(RuleDefinitions.RecurrentEffect.OnEnter);
        spell.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation
            .AbilityScoreAndProficiency);

        spell.EffectDescription.EffectAdvancement.additionalDicePerIncrement = 1;
        spell.EffectDescription.EffectAdvancement.incrementMultiplier = 1;
        spell.EffectDescription.EffectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod
            .PerAdditionalSlotLevel;

        spell.EffectDescription.EffectForms[1].hasSavingThrow = true;
        spell.EffectDescription.EffectForms[1]
            .savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
        spell.EffectDescription.EffectForms[1].DamageForm.diceNumber = 4;
        spell.EffectDescription.EffectForms[1].DamageForm.dieType = RuleDefinitions.DieType.D6;
        spell.EffectDescription.EffectForms[1].DamageForm.damageType = RuleDefinitions.DamageTypePsychic;
        spell.EffectDescription.EffectForms[1].levelMultiplier = 1;
        // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
        //spell.EffectDescription.EffectForms[1].AlterationForm.SetMaximumIncrease(2);
        //spell.EffectDescription.EffectForms[1].AlterationForm.SetValueIncrease(2);

        const string PROXY_PROTECT_THRESHOLD = "ProxyProtectThreshold";

        EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxySpikeGrowth, PROXY_PROTECT_THRESHOLD, BazouSpellsBaseGuid)
            .SetOrUpdateGuiPresentation("ProtectThreshold", Category.Spell)
            .AddToDB();

        spell.EffectDescription.EffectForms[0].SummonForm.effectProxyDefinitionName = PROXY_PROTECT_THRESHOLD;

        return spell;
    }
}
