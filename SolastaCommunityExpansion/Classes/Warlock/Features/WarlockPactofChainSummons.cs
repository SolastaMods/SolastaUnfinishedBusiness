using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Features;

internal static class WarlockPactOfTheChainSummons
{
    private static FeatureDefinition _help;
    private static FeatureDefinitionPower PactofChainFamiliarInvisibilityPower { get; set; }
    private static FeatureDefinitionPower PactofChainFamiliarScarePower { get; set; }

    private static FeatureDefinition Help
    {
        get
        {
            if (_help == null && DatabaseRepository.GetDatabase<FeatureDefinition>()
                    .TryGetElement("HelpAction", out var help))
            {
                _help = help;
            }

            return _help;
        }
    }

    public static void buildPactofChainFamiliarInvisibilityPower()
    {
        var invisibilty = DatabaseHelper.SpellDefinitions.Invisibility;
        var effectDescription = new EffectDescription();
        effectDescription.Copy(invisibilty.EffectDescription);

        PactofChainFamiliarInvisibilityPower = FeatureDefinitionPowerBuilder
            .Create("PactofChainFamiliarInvisibilityPower", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power,
                DatabaseHelper.SpellDefinitions.Invisibility.GuiPresentation.SpriteReference)
            .SetUsesFixed(1)
            .SetActivation(ActivationTime.Action, 0)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Permanent)
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                .SetEffectForms(new EffectFormBuilder()
                    .SetConditionForm(ConditionDefinitionBuilder
                            .Create(DatabaseHelper.ConditionDefinitions.ConditionInvisible,
                                "PactofChainFamiliarInvisibilityCondition", DefinitionBuilder.CENamespaceGuid)
                            .SetGuiPresentation(DatabaseHelper.ConditionDefinitions.ConditionInvisible
                                .GuiPresentation)
                            .SetConditionType(ConditionType.Beneficial)
                            .SetDuration(DurationType.Permanent)
                            .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.CastSpell,
                                ConditionInterruption.UsePower, ConditionInterruption.Damaged)
                            .SetInterruptionDamageThreshold(1)
                            .AddToDB(), ConditionForm.ConditionOperation.Add
                    )
                    .Build()
                )
                .Build())
            .AddToDB();
    }

    public static void buildPactofChainFamiliarScarePower()
    {
        var fear = DatabaseHelper.SpellDefinitions.Fear;

        PactofChainFamiliarScarePower = FeatureDefinitionPowerBuilder
            .Create("PactofChainFamiliarScarePower", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power, fear.GuiPresentation.SpriteReference)
            .SetUsesFixed(1)
            .SetActivation(ActivationTime.Action, 1)
            .SetRechargeRate(RechargeRate.Dawn)
            .SetEffectDescription(new EffectDescriptionBuilder(fear.EffectDescription)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Wisdom,
                    true,
                    EffectDifficultyClassComputation.FixedValue,
                    AttributeDefinitions.Wisdom
                )
                .SetTargetingData(Side.Enemy, RangeType.Distance, 4, TargetType.Individuals)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .CanSaveToCancel(TurnOccurenceType.EndOfTurn)
                    .SetConditionForm(DatabaseHelper.ConditionDefinitions.ConditionFrightenedFear,
                        ConditionForm.ConditionOperation.Add)
                    .Build()
                )
                .Build())
            .AddToDB();
    }

    // public static FeatureDefinition buildSummoningAffinity()
    // {
    //     var acConditionDefinition = ConditionDefinitionBuilder
    //         .Create(DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondAC, "ConditionWarlockFamiliarAC",
    //             DefinitionBuilder.CENamespaceGuid)
    //         .SetGuiPresentationNoContent()
    //         .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
    //         .AddToDB();
    //
    //     var stConditionDefinition = ConditionDefinitionBuilder
    //         .Create(DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondSavingThrows,
    //             "ConditionWarlockFamiliarST", DefinitionBuilder.CENamespaceGuid)
    //         .SetGuiPresentationNoContent()
    //         .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
    //         .AddToDB();
    //      
    //      var damageConditionDefinition = ConditionDefinitionBuilder
    //          .Create(DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondMeleeDamage,
    //              "ConditionWarlockFamiliarDamage", DefinitionBuilder.CENamespaceGuid)
    //          .SetGuiPresentationNoContent()
    //          .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
    //          .AddToDB();
    //
    //      var hitConditionDefinition = ConditionDefinitionBuilder
    //          .Create(DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondMeleeAttack,
    //              "ConditionWarlockFamiliarHit", DefinitionBuilder.CENamespaceGuid)
    //          .SetGuiPresentationNoContent()
    //          .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceSpellAttack)
    //          .AddToDB();
    //
    //      var hpConditionDefinition = ConditionDefinitionBuilder
    //          .Create(DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondHP, "ConditionWarlockFamiliarHP",
    //              DefinitionBuilder.CENamespaceGuid)
    //          .SetGuiPresentationNoContent()
    //          .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel)
    //          .SetAllowMultipleInstances(true)
    //          .AddToDB();
    //
    //      var summoningAffinity = FeatureDefinitionSummoningAffinityBuilder
    //          .Create(DatabaseHelper.FeatureDefinitionSummoningAffinitys.SummoningAffinityKindredSpiritBond,
    //              "SummoningAffinityWarlockFamiliar", DefinitionBuilder.CENamespaceGuid)
    //          .ClearEffectForms()
    //          .SetRequiredMonsterTag("WarlockFamiliar")
    //          .SetAddedConditions(
    //              acConditionDefinition, stConditionDefinition, damageConditionDefinition, hitConditionDefinition,
    //              hpConditionDefinition, hpConditionDefinition)
    //          .AddToDB();
    //
    //      return summoningAffinity;
    // }

    public static MonsterDefinition buildCustomPseudodragon()
    {
        var baseMonster = DatabaseHelper.MonsterDefinitions.Young_GreenDragon;

        var biteAttack = MonsterAttackDefinitionBuilder
            .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Wolf_Bite, "AttackWarlockDragonBite",
                DefinitionBuilder.CENamespaceGuid)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetToHitBonus(4)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetEffectForms(new EffectFormBuilder()
                    .SetDamageForm(dieType: DieType.D4, diceNumber: 1, bonusDamage: 2,
                        damageType: DamageTypePiercing)
                    .Build()
                )
                .Build()
            )
            .AddToDB();

        var stingAttack = MonsterAttackDefinitionBuilder
            .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Badlands_Spider_Bite, "AttackWarlockDragonSting",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.MonsterAttack)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetToHitBonus(4)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Constitution,
                    false, EffectDifficultyClassComputation.FixedValue,
                    null,
                    11
                )
                .SetEffectForms(new EffectFormBuilder()
                        .SetDamageForm(dieType: DieType.D4, diceNumber: 1, bonusDamage: 2,
                            damageType: DamageTypePiercing)
                        .Build(),
                    new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(
                            DatabaseHelper.ConditionDefinitions.ConditionPoisoned,
                            ConditionForm.ConditionOperation.Add
                        )
                        .Build()
                )
                .Build()
            )
            .AddToDB();


        var monster = MonsterDefinitionBuilder
            .Create(baseMonster, "PactOfChainDragon", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("PactOfChainCustomPseudodragon", Category.Monster,
                baseMonster.GuiPresentation.SpriteReference)
            .SetFeatures(
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove2,
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                DatabaseHelper.FeatureDefinitionSenses.SenseBlindSight2,

                //DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity
                DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance,
                DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenSight,
                DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenHearing
            )
            .SetAttackIterations(stingAttack, biteAttack)
            .SetSkillScores(
                (DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                (DatabaseHelper.SkillDefinitions.Stealth.Name, 4)
            )
            .SetArmorClass(13)
            .SetAbilityScores(6, 15, 13, 10, 12, 10)
            .SetStandardHitPoints(7)
            .SetHitDice(DieType.D4, 2)
            .SetHitPointsBonus(2)
            .SetSavingThrowScores()
            .SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions)
            .SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Tiny)
            .SetAlignment(DatabaseHelper.AlignmentDefinitions.NeutralGood.Name)
            .SetCharacterFamily(DatabaseHelper.CharacterFamilyDefinitions.Dragon.name)
            .SetChallengeRating(0)
            .SetLegendaryCreature(false)
            .SetDroppedLootDefinition(null)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction("Party")
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .SetInDungeonEditor(false)
            .SetModelScale(0.1f)
            .SetCreatureTags("WarlockFamiliar")
            .SetNoExperienceGain(false)
            .SetHasPhantomDistortion(true)
            .SetForceNoFlyAnimation(true)
            .SetGroupAttacks(true)
            .AddToDB();


        monster.MonsterPresentation.hasPrefabVariants = false;
        monster.MonsterPresentation.MonsterPresentationDefinitions.Empty();
        monster.MonsterPresentation.useCustomMaterials = true;
        monster.MonsterPresentation.customMaterials = DatabaseHelper.MonsterPresentationDefinitions
            .Young_Green_Dragon_Presentation.CustomMaterials;
        monster.MonsterPresentation.hasMonsterPortraitBackground = true;
        monster.MonsterPresentation.canGeneratePortrait = true;

        if (Help) { monster.Features.Add(Help); }

        return monster;
    }

    public static MonsterDefinition BuildCustomSprite()
    {
        var baseMonster = DatabaseHelper.MonsterDefinitions.Dryad;

        var swordAttack = MonsterAttackDefinitionBuilder
            .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Veteran_Longsword, "AttackWarlockSpriteSword",
                DefinitionBuilder.CENamespaceGuid)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetProximity(AttackProximity.Melee)
            .SetToHitBonus(2)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetEffectForms(new EffectFormBuilder()
                    .SetDamageForm(dieType: DieType.D1, diceNumber: 1, bonusDamage: 0,
                        damageType: DamageTypeSlashing)
                    .Build()
                )
                .Build()
            )
            .AddToDB();

        var bowAttack = MonsterAttackDefinitionBuilder
            .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Goblin_ShortBow, "AttackWarlockSpriteBow",
                DefinitionBuilder.CENamespaceGuid)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetProximity(AttackProximity.Range)
            .SetToHitBonus(6)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Constitution,
                    false, EffectDifficultyClassComputation.FixedValue,
                    null
                )
                .SetEffectForms(new EffectFormBuilder()
                        .SetDamageForm(dieType: DieType.D1, diceNumber: 1, bonusDamage: 0,
                            damageType: DamageTypePiercing)
                        .Build(),
                    new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(
                            DatabaseHelper.ConditionDefinitions.ConditionPoisoned,
                            ConditionForm.ConditionOperation.Add
                        )
                        .Build()
                )
                .Build()
            )
            .AddToDB();


        var monster = MonsterDefinitionBuilder
            .Create(baseMonster, "PactOfChainCustomSprite", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Monster, baseMonster.GuiPresentation.SpriteReference)
            .SetFeatures(
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly8,
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove2,
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                PactofChainFamiliarInvisibilityPower
            )
            .SetAttackIterations(bowAttack, swordAttack)
            .SetSkillScores(
                (DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                (DatabaseHelper.SkillDefinitions.Stealth.Name, 8)
            )
            .SetArmorClass(15)
            .SetArmor(DatabaseHelper.ArmorTypeDefinitions.LeatherType.Name)
            .SetAbilityScores(3, 18, 10, 14, 12, 11)
            .SetStandardHitPoints(2)
            .SetHitDice(DieType.D4, 1)
            .SetHitPointsBonus(2)
            .SetSavingThrowScores()
            .SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DryadCombatDecisions)
            .SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Tiny)
            .SetAlignment(DatabaseHelper.AlignmentDefinitions.NeutralGood.Name)
            .SetCharacterFamily(DatabaseHelper.CharacterFamilyDefinitions.Fey.name)
            .SetChallengeRating(0.25f)
            .SetLegendaryCreature(false)
            .SetDroppedLootDefinition(null)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction("Party")
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .SetInDungeonEditor(false)
            .SetModelScale(0.4f)
            .SetCreatureTags("WarlockFamiliar")
            .SetNoExperienceGain(false)
            .SetHasPhantomDistortion(true)
            .SetForceNoFlyAnimation(true)
            .SetGroupAttacks(false)
            .AddToDB();


        monster.MonsterPresentation.hasPrefabVariants = false;
        monster.MonsterPresentation.MonsterPresentationDefinitions.Empty();
        monster.MonsterPresentation.useCustomMaterials = true;
        // monster.MonsterPresentation.customMaterials = (DatabaseHelper.MonsterPresentationDefinitions
        //     .Young_Green_Dragon_Presentation.CustomMaterials);
        monster.MonsterPresentation.hasMonsterPortraitBackground = true;
        monster.MonsterPresentation.canGeneratePortrait = true;

        if (Help) { monster.Features.Add(Help); }

        // monster.MonsterPresentation.SetOverrideCharacterShaderColors(true);
        // monster.MonsterPresentation.SetFirstCharacterShaderColor(DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.firstCharacterShaderColor);
        // monster.MonsterPresentation.SetSecondCharacterShaderColor(DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.secondCharacterShaderColor);
        //
        // // monster.CreatureTags.Clear();
        // monster.MonsterPresentation.MonsterPresentationDefinitions.Empty();
        // monster.MonsterPresentation.monsterPresentationDefinitions = (DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.MonsterPresentationDefinitions);
        // monster.MonsterPresentation.useCustomMaterials = (true);
        // // //  monster.MonsterPresentation.customMaterials = (DatabaseHelper.MonsterPresentationDefinitions.Silver_Dragon_Presentation.customMaterials);
        // //
        // monster.MonsterPresentation.maleModelScale = (0.4f);
        // monster.MonsterPresentation.femaleModelScale = (0.4f);
        // monster.MonsterPresentation.malePrefabReference = (DatabaseHelper.MonsterDefinitions.Dryad.MonsterPresentation.malePrefabReference);
        // monster.MonsterPresentation.femalePrefabReference = (DatabaseHelper.MonsterDefinitions.Dryad.MonsterPresentation.malePrefabReference);

        return monster;
    }

    public static MonsterDefinition BuildCustomImp()
    {
        var baseMonster = DatabaseHelper.MonsterDefinitions.Goblin;

        var stingAttack = MonsterAttackDefinitionBuilder
            .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Badlands_Spider_Bite, "AttackWarlockImpSting",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.MonsterAttack)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetToHitBonus(5)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Constitution,
                    false, EffectDifficultyClassComputation.FixedValue,
                    null,
                    11
                )
                .SetEffectForms(new EffectFormBuilder()
                        .SetDamageForm(dieType: DieType.D4, diceNumber: 1, bonusDamage: 3,
                            damageType: DamageTypePiercing)
                        .Build(),
                    new EffectFormBuilder()
                        .SetDamageForm(dieType: DieType.D6, diceNumber: 3, damageType: DamageTypePoison)
                        .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                        .Build()
                )
                .Build()
            )
            .AddToDB();


        var monster = MonsterDefinitionBuilder
            .Create(baseMonster, "PactOfChainCustomImp", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Monster, baseMonster.GuiPresentation.SpriteReference)
            .SetFeatures(
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly8,
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove4,
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision24,
                //Todo: add devil's sight - magical darkness doesn't affect vision
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceExceptSilver,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceExceptSilver,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceExceptSilver,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance,

                //TODO: can we implement shapechange for monsters at all?
                PactofChainFamiliarInvisibilityPower
            )
            .SetAttackIterations(stingAttack)
            .SetSkillScores(
                (DatabaseHelper.SkillDefinitions.Deception.Name, 4),
                (DatabaseHelper.SkillDefinitions.Insight.Name, 3),
                (DatabaseHelper.SkillDefinitions.Persuasion.Name, 4),
                (DatabaseHelper.SkillDefinitions.Stealth.Name, 5)
            )
            .SetArmorClass(13)
            .SetAbilityScores(6, 17, 13, 11, 12, 14)
            .SetStandardHitPoints(10)
            .SetHitDice(DieType.D4, 3)
            .SetHitPointsBonus(3)
            .SetSavingThrowScores()
            .SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DryadCombatDecisions)
            .SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Tiny)
            .SetAlignment(DatabaseHelper.AlignmentDefinitions.LawfulEvil.Name)
            .SetCharacterFamily(DatabaseHelper.CharacterFamilyDefinitions.Fiend.name)
            .SetChallengeRating(1f)
            .SetLegendaryCreature(false)
            .SetDroppedLootDefinition(null)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction("Party")
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.Reference)
            .SetInDungeonEditor(false)
            .SetModelScale(0.4f)
            .SetCreatureTags("WarlockFamiliar")
            .SetNoExperienceGain(false)
            .SetHasPhantomDistortion(true)
            .SetForceNoFlyAnimation(true)
            .SetGroupAttacks(false)
            .AddToDB();

        monster.MonsterPresentation.MonsterPresentationDefinitions.Empty();
        monster.MonsterPresentation.monsterPresentationDefinitions = DatabaseHelper.MonsterDefinitions.Goblin
            .MonsterPresentation.MonsterPresentationDefinitions;
        monster.MonsterPresentation.useCustomMaterials = true;
        monster.MonsterPresentation.customMaterials = DatabaseHelper.MonsterPresentationDefinitions
            .Orc_Female_Archer_RedScar.CustomMaterials;

        monster.MonsterPresentation.maleModelScale = 0.4f;
        monster.MonsterPresentation.femaleModelScale = 0.4f;
        monster.MonsterPresentation.malePrefabReference =
            DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.malePrefabReference;
        monster.MonsterPresentation.femalePrefabReference =
            DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.femalePrefabReference;

        monster.MonsterPresentation.hasPrefabVariants = false;

        if (Help) { monster.Features.Add(Help); }

        return monster;
    }

    public static MonsterDefinition buildCustomQuasit()
    {
        var baseMonster = DatabaseHelper.MonsterDefinitions.Goblin;

        var clawAttack = MonsterAttackDefinitionBuilder
            .Create(DatabaseHelper.MonsterAttackDefinitions.Attack_Zealot_Claw, "AttackWarlockImpClaw",
                DefinitionBuilder.CENamespaceGuid)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetToHitBonus(4)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Constitution,
                    false, EffectDifficultyClassComputation.FixedValue,
                    null
                )
                .SetEffectForms(new EffectFormBuilder()
                        .SetDamageForm(dieType: DieType.D4, diceNumber: 1, bonusDamage: 3,
                            damageType: DamageTypePiercing)
                        .Build(),
                    new EffectFormBuilder()
                        .SetDamageForm(dieType: DieType.D4, diceNumber: 2, damageType: DamageTypePoison)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build(),
                    new EffectFormBuilder()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .CanSaveToCancel(TurnOccurenceType.EndOfTurn)
                        .SetConditionForm(
                            DatabaseHelper.ConditionDefinitions.ConditionPoisoned, //should be a minute duration
                            ConditionForm.ConditionOperation.Add
                        )
                        .Build()
                )
                .Build()
            )
            .AddToDB();

        var monster = MonsterDefinitionBuilder
            .Create(baseMonster, "PactOfChainCustomQuasit", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Monster, baseMonster.GuiPresentation.SpriteReference)
            .SetFeatures(
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8,
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision24,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinitySpellResistance,

                //TODO: can we implement shapechange for monsters at all?
                PactofChainFamiliarInvisibilityPower,
                PactofChainFamiliarScarePower
            )
            .SetAttackIterations(clawAttack)
            .SetSkillScores(
                (DatabaseHelper.SkillDefinitions.Stealth.Name, 5)
            )
            .SetArmorClass(13)
            .SetAbilityScores(5, 17, 10, 7, 10, 10)
            .SetStandardHitPoints(7)
            .SetHitDice(DieType.D4, 3)
            .SetHitPointsBonus(0)
            .SetSavingThrowScores()
            .SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.GoblinCombatDecisions)
            .SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Tiny)
            .SetAlignment(DatabaseHelper.AlignmentDefinitions.ChaoticEvil.Name)
            .SetCharacterFamily(DatabaseHelper.CharacterFamilyDefinitions.Fiend.name)
            .SetChallengeRating(1f)
            .SetLegendaryCreature(false)
            .SetDroppedLootDefinition(null)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction("Party")
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.Reference)
            .SetInDungeonEditor(false)
            .SetModelScale(0.4f)
            .SetCreatureTags("WarlockFamiliar")
            .SetNoExperienceGain(false)
            .SetHasPhantomDistortion(true)
            .SetForceNoFlyAnimation(true)
            .SetGroupAttacks(false)
            .AddToDB();

        monster.MonsterPresentation.hasPrefabVariants = false;

        monster.MonsterPresentation.MonsterPresentationDefinitions.Empty();
        monster.MonsterPresentation.monsterPresentationDefinitions = DatabaseHelper.MonsterDefinitions.Goblin
            .MonsterPresentation.MonsterPresentationDefinitions;
        monster.MonsterPresentation.useCustomMaterials = true;
        monster.MonsterPresentation.customMaterials = DatabaseHelper.MonsterPresentationDefinitions
            .Orc_Male_Chieftain_BladeFang.CustomMaterials;

        monster.MonsterPresentation.maleModelScale = 0.55f;
        monster.MonsterPresentation.femaleModelScale = 0.55f;
        monster.MonsterPresentation.malePrefabReference =
            DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.malePrefabReference;
        monster.MonsterPresentation.femalePrefabReference =
            DatabaseHelper.MonsterDefinitions.Goblin.MonsterPresentation.malePrefabReference;

        if (Help) { monster.Features.Add(Help); }

        return monster;
    }
}
