using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Models.SpellsContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static class BazouSpells
{
    internal static void Register()
    {
        // RegisterSpell(BuildEldritchOrb(), 0, SpellListWarlock);
        RegisterSpell(BuildFindFamiliar(), 0, SpellListWarlock, SpellListWizard);
        // RegisterSpell(BuildFrenzy(), 0, SpellListWarlock, SpellListWizard, SpellListSorcerer);
        RegisterSpell(BuildMinorLifesteal(), 0, SpellListWizard);
        // RegisterSpell(BuildPetalStorm(), 0, SpellListDruid);
        // RegisterSpell(BuildProtectThreshold(), 0, SpellListCleric, SpellListDruid, SpellListPaladin);
    }

    #if false
    [NotNull]
    private static SpellDefinition BuildEldritchOrb()
    {
        var spell = SpellDefinitionBuilder
            .Create(MagicMissile, "EldritchOrb")
            .SetGuiPresentation(Category.Spell, Shine.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(0)
            .AddToDB();

        spell.EffectDescription
            .SetRangeType(RangeType.Distance)
            .SetRangeParameter(12)
            .SetDurationType(DurationType.Instantaneous)
            .SetTargetType(TargetType.Sphere)
            .SetTargetParameter(1)
            .SetHasSavingThrow(false)
            .SetSavingThrowAbility(AttributeDefinitions.Dexterity)
            .SetCanBeDispersed(true);

        spell.EffectDescription.EffectAdvancement
            .additionalDicePerIncrement = 1;

        spell.EffectDescription.EffectAdvancement
            .incrementMultiplier = 5;

        spell.EffectDescription.EffectAdvancement
            .effectIncrementMethod = EffectIncrementMethod.CasterLevelTable;

        // Changing to a single damage effect with d4, as I am unsure how to implement 2 different effectDescriptions within the same spell
        // First one should be single target attack roll, d8 damage
        // Second one should be adjacent aoe to first target, half of damage of first effect, no damage on saving throw negates
        var effectForm = spell.EffectDescription.EffectForms[0];
        effectForm.hasSavingThrow = false;
        effectForm.levelMultiplier = 1;
        // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
        //effectForm.AlterationForm.SetMaximumIncrease(2).SetValueIncrease(2);
        effectForm.DamageForm.diceNumber = 1;
        effectForm.DamageForm.dieType = DieType.D4;
        effectForm.DamageForm.damageType = DamageTypeForce;

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
    #endif
    
    [NotNull]
    private static SpellDefinition BuildFindFamiliar()
    {
        var familiarMonsterBuilder = MonsterDefinitionBuilder
            .Create(Eagle_Matriarch, "Owl")
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
            .SetHitDiceType(DieType.D4)
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

        if (DatabaseRepository.GetDatabase<FeatureDefinition>().TryGetElement("PowerHelp", out var help))
        {
            familiarMonsterBuilder.AddFeatures(help);
        }

        var familiarMonster = familiarMonsterBuilder.AddToDB();

        var spell = SpellDefinitionBuilder.Create(Fireball, "FindFamiliar")
            .SetGuiPresentation(Category.Spell, AnimalFriendship.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(1)
            .SetCastingTime(ActivationTime.Hours1)
            // BUG: Unable to have 70 minutes ritual casting time... if set to 10 minutes, it really only takes 10 minutes, instead of 70
            .SetRitualCasting(ActivationTime.Minute10)
            .AddToDB();

        spell.uniqueInstance = true;

        spell.EffectDescription.Copy(ConjureAnimalsOneBeast.EffectDescription);
        spell.EffectDescription.SetRangeType(RangeType.Distance);
        spell.EffectDescription.SetRangeParameter(2);
        spell.EffectDescription.SetDurationType(DurationType.Permanent);
        spell.EffectDescription.SetTargetSide(Side.Ally);
        spell.EffectDescription.EffectForms.Clear();

        var summonForm = new SummonForm { monsterDefinitionName = familiarMonster.name, decisionPackage = null };

        var effectForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Summon, createdByCharacter = true, summonForm = summonForm
        };

        spell.EffectDescription.EffectForms.Add(effectForm);

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, spell);

        return spell;
    }

    #if false
    [NotNull]
    private static SpellDefinition BuildFrenzy()
    {
        var spell = SpellDefinitionBuilder
            .Create(Confusion, "Frenzy")
            .SetGuiPresentation(Category.Spell, Confusion.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(6)
            .SetRequiresConcentration(true)
            .AddToDB();

        // Not sure if I prefer copying and editing existing effect description
        // or creating one from scratch through API
        spell.EffectDescription.SetRangeType(RangeType.Distance);
        spell.EffectDescription.SetRangeParameter(24);
        spell.EffectDescription.SetDurationType(DurationType.Minute);
        spell.EffectDescription.SetDurationParameter(1);
        spell.EffectDescription.SetTargetType(TargetType.Sphere);
        spell.EffectDescription.SetTargetParameter(4);
        spell.EffectDescription.hasSavingThrow = true;
        spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);

        var conditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionConfused, "ConditionFrenzied")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddToDB();

        // Some methods are missing like SetField or Copy
        var actionAffinity = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinityConditionConfused, "ActionAffinityConditionFrenzied")
            .AddToDB();

        actionAffinity.RandomBehaviourOptions.Clear();

        var behaviorMode = new BehaviorModeDescription
        {
            behaviour =
                RandomBehaviour
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
    #endif

    [NotNull]
    private static SpellDefinition BuildMinorLifesteal()
    {
        var spell = SpellDefinitionBuilder
            .Create("MinorLifesteal")
            .SetGuiPresentation(Category.Spell, VampiricTouch.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(false)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.Individuals)
                .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Construct, CharacterFamilyDefinitions.Undead)
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5,
                    additionalDicePerIncrement: 1)
                .AddEffectForm(new EffectFormBuilder()
                    .SetDamageForm(dieType: DieType.D8, diceNumber: 1, damageType: DamageTypeNecrotic,
                        healFromInflictedDamage: HealFromInflictedDamage.Half)
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .Build())
                .AddEffectForm(new EffectFormBuilder()
                    .SetTempHPForm(dieType: DieType.D4, diceNumber: 1, applyToSelf: true)
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .Build())
                .SetParticleEffectParameters(VampiricTouch.EffectDescription.EffectParticleParameters)
                .Build())
            .AddToDB();
        return spell;
    }

    #if false
    [NotNull]
    private static SpellDefinition BuildPetalStorm()
    {
        var spell = SpellDefinitionBuilder
            .Create(InsectPlague, "PetalStorm")
            .SetGuiPresentation(Category.Spell, WindWall.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(2)
            .SetRequiresConcentration(true)
            .AddToDB();

        // Not sure if I prefer copying and editing existing effect description
        // or creating one from scratch through API
        spell.EffectDescription.SetRangeType(RangeType.Distance);
        spell.EffectDescription.SetRangeParameter(12);
        spell.EffectDescription.SetDurationType(DurationType.Minute);
        spell.EffectDescription.SetDurationParameter(1);
        spell.EffectDescription.SetTargetType(TargetType.Cube);
        spell.EffectDescription.SetTargetParameter(3);
        spell.EffectDescription.hasSavingThrow = true;
        spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Strength);
        spell.EffectDescription.SetRecurrentEffect((RecurrentEffect)20);
        spell.EffectDescription.EffectAdvancement.additionalDicePerIncrement = 2;
        spell.EffectDescription.EffectAdvancement.incrementMultiplier = 1;
        spell.EffectDescription.EffectAdvancement.effectIncrementMethod = EffectIncrementMethod
            .PerAdditionalSlotLevel;

        spell.EffectDescription.EffectForms[0].hasSavingThrow = true;
        spell.EffectDescription.EffectForms[0]
            .savingThrowAffinity = EffectSavingThrowType.Negates;
        spell.EffectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        spell.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D4;
        spell.EffectDescription.EffectForms[0].DamageForm.damageType = DamageTypeSlashing;
        spell.EffectDescription.EffectForms[0].levelMultiplier = 1;
        // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
        //spell.EffectDescription.EffectForms[0].AlterationForm.SetMaximumIncrease(2);
        //spell.EffectDescription.EffectForms[0].AlterationForm.SetValueIncrease(2);

        var effectProxyDefinition = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyInsectPlague, "ProxyPetalStorm")
            .SetGuiPresentation("PetalStorm", Category.Spell, WindWall.GuiPresentation.SpriteReference)
            .SetCanMove()
            .SetPortrait(WindWall.GuiPresentation.SpriteReference)
            .AddAdditionalFeatures(FeatureDefinitionMoveModes.MoveModeMove6)
            .AddToDB();

        effectProxyDefinition.actionId = ActionDefinitions.Id.ProxyFlamingSphere;
        effectProxyDefinition.attackMethod = ProxyAttackMethod.ReproduceDamageForms;
        effectProxyDefinition.canMoveOnCharacters = true;
        effectProxyDefinition.isEmptyPresentation = false;

        spell.EffectDescription.EffectForms[2].SummonForm.effectProxyDefinitionName = effectProxyDefinition.Name;

        return spell;
    }

    [NotNull]
    private static SpellDefinition BuildProtectThreshold()
    {
        var spell = SpellDefinitionBuilder
            .Create(SpikeGrowth, "ProtectThreshold")
            .SetGuiPresentation(Category.Spell, Bane.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetSpellLevel(2)
            .SetRequiresConcentration(false)
            .SetRitualCasting(ActivationTime.Minute10).AddToDB();

        // Not sure if I prefer copying and editing existing effect description
        // or creating one from scratch through API
        spell.EffectDescription.SetRangeType(RangeType.Distance);
        spell.EffectDescription.SetRangeParameter(1);
        spell.EffectDescription.SetDurationType(DurationType.Minute);
        spell.EffectDescription.SetDurationParameter(10);
        spell.EffectDescription.SetTargetType(TargetType.Sphere);
        spell.EffectDescription.SetTargetParameter(0);
        spell.EffectDescription.hasSavingThrow = true;
        spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);
        spell.EffectDescription.SetRecurrentEffect(RecurrentEffect.OnEnter);
        spell.EffectDescription.SetDifficultyClassComputation(EffectDifficultyClassComputation
            .AbilityScoreAndProficiency);

        spell.EffectDescription.EffectAdvancement.additionalDicePerIncrement = 1;
        spell.EffectDescription.EffectAdvancement.incrementMultiplier = 1;
        spell.EffectDescription.EffectAdvancement.effectIncrementMethod = EffectIncrementMethod
            .PerAdditionalSlotLevel;

        spell.EffectDescription.EffectForms[1].hasSavingThrow = true;
        spell.EffectDescription.EffectForms[1]
            .savingThrowAffinity = EffectSavingThrowType.HalfDamage;
        spell.EffectDescription.EffectForms[1].DamageForm.diceNumber = 4;
        spell.EffectDescription.EffectForms[1].DamageForm.dieType = DieType.D6;
        spell.EffectDescription.EffectForms[1].DamageForm.damageType = DamageTypePsychic;
        spell.EffectDescription.EffectForms[1].levelMultiplier = 1;
        // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
        //spell.EffectDescription.EffectForms[1].AlterationForm.SetMaximumIncrease(2);
        //spell.EffectDescription.EffectForms[1].AlterationForm.SetValueIncrease(2);

        const string PROXY_PROTECT_THRESHOLD = "ProxyProtectThreshold";

        EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxySpikeGrowth, PROXY_PROTECT_THRESHOLD)
            .SetOrUpdateGuiPresentation("ProtectThreshold", Category.Spell)
            .AddToDB();

        spell.EffectDescription.EffectForms[0].SummonForm.effectProxyDefinitionName = PROXY_PROTECT_THRESHOLD;

        return spell;
    }
    #endif
}
