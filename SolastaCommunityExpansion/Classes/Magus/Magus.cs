using System;
using System.Collections.Generic;
using AwesomeTechnologies.VegetationSystem;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Level20;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using static EquipmentDefinitions;
using static SolastaCommunityExpansion.Builders.EquipmentOptionsBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SkillDefinitions;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Classes.Magus;

public static class Magus
{
    public static CharacterClassDefinition ClassMagus { get; private set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyArmor { get; set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyWeapon { get; set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyTool { get; set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencySavingThrow { get; set; }

    private static FeatureDefinitionPointPool FeatureDefinitionSkillPoints { get; set; }

    private static FeatureDefinitionCastSpell FeatureDefinitionClassMagusCastSpell { get; set; }

    public static FeatureDefinitionPower ArcanaPool { get; private set; }

    public static FeatureDefinitionFeatureSetCustom ArcaneArt { get; private set; }

    private static void BuildEquipment(CharacterClassDefinitionBuilder classMagusBuilder)
    {
        classMagusBuilder
            .AddEquipmentRow(
                Column(
                    Option(DatabaseHelper.ItemDefinitions.Longbow, OptionWeaponMartialRangedChoice, 1),
                    Option(DatabaseHelper.ItemDefinitions.Arrow, OptionAmmoPack, 1)
                ),
                Column(Option(DatabaseHelper.ItemDefinitions.Rapier, OptionWeaponMartialMeleeChoice, 1)))
            .AddEquipmentRow(
                Column(Option(DatabaseHelper.ItemDefinitions.ScholarPack, OptionStarterPack, 1)),
                Column(Option(DatabaseHelper.ItemDefinitions.DungeoneerPack, OptionStarterPack, 1)))
            .AddEquipmentRow(
                Column(
                    Option(DatabaseHelper.ItemDefinitions.Leather, OptionArmor, 1),
                    Option(DatabaseHelper.ItemDefinitions.ComponentPouch, OptionFocus, 1),
                    Option(DatabaseHelper.ItemDefinitions.Shortsword, OptionWeaponMartialMeleeChoice, 2)
                ));
    }

    private static void BuildProficiencies()
    {
        static FeatureDefinitionProficiency BuildProficiency(string name, ProficiencyType type,
            params string[] proficiencies)
        {
            return FeatureDefinitionProficiencyBuilder
                .Create(name, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetProficiencies(type, proficiencies)
                .AddToDB();
        }

        FeatureDefinitionProficiencyArmor =
            BuildProficiency("ClassMagusArmorProficiency", ProficiencyType.Armor,
                LightArmorCategory);

        FeatureDefinitionProficiencyWeapon =
            BuildProficiency("ClassMagusWeaponProficiency", ProficiencyType.Weapon,
                MartialWeaponCategory);

        FeatureDefinitionProficiencyTool =
            BuildProficiency("ClassMagusToolsProficiency", ProficiencyType.Tool,
                DatabaseHelper.ToolTypeDefinitions.EnchantingToolType.Name,
                DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType.Name);

        FeatureDefinitionProficiencySavingThrow =
            BuildProficiency("ClassMagusSavingThrowProficiency", ProficiencyType.SavingThrow,
                AttributeDefinitions.Dexterity, AttributeDefinitions.Wisdom);

        FeatureDefinitionSkillPoints = FeatureDefinitionPointPoolBuilder
            .Create("ClassMagusSkillProficiency", DefinitionBuilder.CENamespaceGuid)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 4)
            .OnlyUniqueChoices()
            .RestrictChoices(
                SkillDefinitions.Acrobatics,
                SkillDefinitions.Athletics,
                SkillDefinitions.Persuasion,
                SkillDefinitions.Arcana,
                SkillDefinitions.Deception,
                SkillDefinitions.History,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Perception,
                SkillDefinitions.Nature,
                SkillDefinitions.Religion)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }

    private static void BuildSpells()
    {
        /*FeatureDefinitionClassMagusCastSpell = FeatureDefinitionCastSpellBuilder
            .Create("ClassMagusCastSpell", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusSpellcasting", Category.Class)
            .SetKnownCantrips(4, 4, 4, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6)
            .SetKnownSpells(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 13, 13, 14, 14, 15, 15, 15, 15)
            .SetSlotsPerLevel(MagusSpells.MagusCastingSlot)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellCastingLevel(9)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSpellList(MagusSpells.MagusSpellList)
            .SetReplacedSpells(SpellsHelper.FullCasterReplacedSpells)
            .AddToDB();*/
        
        
        FeatureDefinitionClassMagusCastSpell = FeatureDefinitionCastSpellBuilder
            .Create("ClassMagusCastSpell", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusSpellcasting", Category.Class)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellList(MagusSpells.MagusSpellList)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.WholeList)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.Prepared)
            .SetSpellPreparationCount(RuleDefinitions.SpellPreparationCount.AbilityBonusPlusHalfLevel)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
            .SetSpellCastingLevel(1)
            .SetKnownCantrips(2, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
            .SetSlotsPerLevel(1, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
            .AddToDB();
    }

    private static void BuildArcaneArt()
    {
        var validators = new List<object>();
        if (validators == null)
        {
            throw new ArgumentNullException(nameof(validators));
        }

        ArcanaPool = FeatureDefinitionPowerBuilder
            .Create("ClassMagusArcanePool", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power, "ClassMagusArcanaPool", DatabaseHelper.SpellDefinitions.ProduceFlame.guiPresentation.spriteReference)
            .SetUsesFixed(5)
            .SetActivationTime(ActivationTime.OnAttackHit)
            .SetRechargeRate(RechargeRate.LongRest)
            .AddToDB();

        // rupture strike: damage on moving
        var ruptureStrike = BuildRuptureStrike(ArcanaPool);

        // nullify strike: disable resistance and immunity to damage for one round
        var nullifyStrike = BuildNullifyStrike(ArcanaPool);

        // terror strike: frighten the target
        var terrorStrike = BuildTerrorStrike(ArcanaPool);

        // exile strike: 4d10 force damage (save for half) and banish creature for 1d4 round (save to negate)
        var exileStrike = BuildExileStrike(ArcanaPool);
        
        // eldritch predator
        // goes invisible and paralyze enemies within 6 cells
        // both invisible and paralyzed condition end after casting a spell or making an attack
        // TODO: make this works with sunlight blade and resonate strike because the spell executed before the attack is made which remove the condition.
        var eldritchPredator = BuildEldritchPredator(ArcanaPool);

        // unfathomable terror: creatures with you can see and within 6 cell will no longer immune to fear
        var unfathomableTerror = BuildUnfathomableHorror(ArcanaPool);

        // grant mage armor add INT modifier to AC
        var magisterArmor = BuildAmorOfTheMagister();
        
        // grant bane and force enemy disadvantage when saving throw against it 
        var greaterBane = BuildGreaterBane();
        
        // frost fang: hopefully cold damage with sweet animation
        var frostFang = BuildFrostFang(ArcanaPool);
        // mind spike
        // soul drinker: regain an arcana focus

        PowerBundleContext.RegisterPowerBundle(ArcanaPool, true, ruptureStrike, nullifyStrike, terrorStrike, exileStrike);

        ArcaneArt = FeatureDefinitionFeatureSetCustomBuilder
            .Create("ClassMagusArcaneArtSetLevel", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature,
                CustomIcons.CreateAssetReferenceSprite("ArcaneArt", Resources.EldritchInvocation, 128, 128))
            .SetRequireClassLevels(true)
            .SetLevelFeatures(3, magisterArmor, ruptureStrike, eldritchPredator, terrorStrike, greaterBane /*corrosiveTouch*/)
            .SetLevelFeatures(7, nullifyStrike, unfathomableTerror/*,burningWound*/ )
            .SetLevelFeatures(11, exileStrike, frostFang)
            .AddToDB();
    }

    private static void BuildProgression(CharacterClassDefinitionBuilder classMagusBuilder)
    {
        var warCaster = BuildWarCaster();

        var extraAttack = BuildExtraAttack();
        
        classMagusBuilder
            .AddFeaturesAtLevel(1,
                FeatureDefinitionProficiencySavingThrow,
                FeatureDefinitionProficiencyArmor,
                FeatureDefinitionProficiencyWeapon,
                FeatureDefinitionProficiencyTool,
                FeatureDefinitionSkillPoints,
                FeatureDefinitionClassMagusCastSpell
            )
            .AddFeaturesAtLevel(2, warCaster)
            .AddFeaturesAtLevel(3, ArcanaPool, ArcaneArt, ArcaneArt)
            .AddFeatureAtLevel(4, DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)
            .AddFeatureAtLevel(5, extraAttack)
            //.AddFeaturesAtLevel(6)
            .AddFeaturesAtLevel(7, ArcaneArt, ArcaneArt)
            .AddFeatureAtLevel(8, DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)
            //.AddFeatureAtLevel(10,)
            .AddFeaturesAtLevel(11, ArcaneArt, ArcaneArt)
            .AddFeatureAtLevel(12, DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
    }

    internal static CharacterClassDefinition BuildMagusClass()
    {
        var magusSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("Magus", Resources.Warlock, 1024, 576);

        var classMagusBuilder = CharacterClassDefinitionBuilder
            .Create("ClassMagus", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Class, magusSpriteReference, 1)
            .AddFeatPreferences(DatabaseHelper.FeatDefinitions.PowerfulCantrip,
                DatabaseHelper.FeatDefinitions.FlawlessConcentration,
                DatabaseHelper.FeatDefinitions.Robust)
            .AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.Violence, 3)
            .AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.Self_Preservation, 3)
            .AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.Normal, 3)
            .AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.GpSpellcaster, 5)
            .AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.GpExplorer, 1)
            .AddSkillPreferences(
                Acrobatics,
                Athletics,
                Persuasion,
                Arcana,
                Deception,
                History,
                Intimidation,
                Perception,
                Nature,
                Religion)
            .AddToolPreferences(
                DatabaseHelper.ToolTypeDefinitions.EnchantingToolType,
                DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType)
            .SetAbilityScorePriorities(
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Charisma)
            .SetAnimationId(AnimationDefinitions.ClassAnimationId.Paladin)
            .SetBattleAI(DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeWithBackupRangeDecisions)
            .SetHitDice(DieType.D8)
            .SetIngredientGatheringOdds(DatabaseHelper.CharacterClassDefinitions.Sorcerer.IngredientGatheringOdds)
            .SetPictogram(DatabaseHelper.CharacterClassDefinitions.Wizard.ClassPictogramReference);
        
        ClassMagus = classMagusBuilder.AddToDB();
        
        BuildEquipment(classMagusBuilder);
        BuildProficiencies();
        BuildSpells();
        BuildArcaneArt();
        BuildProgression(classMagusBuilder);

        return ClassMagus;
    }

    #region war_caster
    private static FeatureDefinition BuildWarCaster()
    {
        return FeatureDefinitionProficiencyBuilder
            .Create("ClassMagusWarCaster", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusWarCaster", Category.Class)
            .SetProficiencies(ProficiencyType.Feat, OtherFeats.Warcaster.Name)
            .AddToDB();
    }
    #endregion
    
    #region extra_attack
    private static FeatureDefinition BuildExtraAttack()
    {
        return FeatureDefinitionAttributeModifierBuilder
            .Create("ClassMagusExtraAttack", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack.guiPresentation)
            .SetModifiedAttribute(AttributeDefinitions.AttacksNumber)
            .SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive)
            .SetModifierValue(1)
            .AddToDB();
    }
    #endregion

    #region blood_ritual
    
    #endregion
    #region rupture_strike
    private static FeatureDefinitionPower BuildRuptureStrike(FeatureDefinitionPower sharedPool)
    {
        var condition = ConditionDefinitionBuilder
            .Create("ClassMagusConditionRupture", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Class, "ClassMagusConditionRupture",
                ConditionSlowed.GuiPresentation.SpriteReference)
            .AddToDB();
        condition.specialDuration = true;
        condition.durationParameter = 1;
        condition.durationType = DurationType.Minute;
        
        condition.turnOccurence = (TurnOccurenceType)ExtraTurnOccurenceType.OnMoveEnd;
        condition.RecurrentEffectForms.Add(new EffectForm
        {
            formType = EffectForm.EffectFormType.Damage,
            hasSavingThrow = false,
            canSaveToCancel =  false,
            damageForm = new DamageForm { damageType = DamageTypeNecrotic, diceNumber = 1, dieType = DieType.D4 }
        });

        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMagusArcaneArtRuptureStrike", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusArcaneArtRuptureStrike", null, 0, true)
            .SetSharedPool(sharedPool)
            .SetActivationTime(ActivationTime.NoCost)
            .SetCostPerUse(1)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24,
                        TargetType.Individuals)
                    .SetSavingThrowData(
                        true,
                        false,
                        AttributeDefinitions.Constitution,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Intelligence)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                            .Build()
                    )
                    .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ChillTouch)
                    .Build())
            .AddToDB();
        
        var canUseRuptureStrike = CharacterValidators.HasBeenGrantedFeature(power);
        power.SetCustomSubFeatures(new PowerUseValidity(canUseRuptureStrike, CharacterValidators.NoShield, CharacterValidators.EmptyOffhand));
        
        return power;
    }
    #endregion
    
    #region eldritch_predator
    // TODO: do something here to make creatures that are immune to condition paralyze also not affected by this condition unless they are hit by torment blade.
    private static readonly ConditionDefinition ConditionParalyzedWhenBeingPreyedOn = ConditionDefinitionBuilder
        .Create("ClassMagusConditionParalyzedWhenBeingPreyedOn",
            DefinitionBuilder.CENamespaceGuid)
        .SetParentCondition(DatabaseHelper.ConditionDefinitions.ConditionParalyzed)
        .SetDuration(DurationType.Round, 1)
        .SetGuiPresentation(Category.Class, "ClassMagusConditionParalyzedWhenBeingPreyedOn",
            DatabaseHelper.ConditionDefinitions.ConditionFrightenedFear.guiPresentation.spriteReference)
        .AddToDB();

    private sealed class ConditionDefinitionPreying : ConditionDefinition, INotifyConditionRemoval
    {
        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            var locationCharacterManager =
                ServiceRepository.GetService<IGameLocationCharacterService>();

            foreach (var character in locationCharacterManager.ValidCharacters)
            {
                if (character.rulesetActor.side != Side.Enemy)
                {
                    continue;
                }

                var conditionsToRemoved = new List<RulesetCondition>();
                foreach (var keyValuePair in character.RulesetCharacter.conditionsByCategory)
                {
                    foreach (var linkedCondition in keyValuePair.Value)
                    {
                        if (linkedCondition.ConditionDefinition.Name == ConditionParalyzedWhenBeingPreyedOn.Name &&
                            (long)rulesetCondition.SourceGuid == (long)removedFrom.guid)
                        {
                            conditionsToRemoved.Add(linkedCondition);
                        }
                    }
                }

                foreach (var condition in conditionsToRemoved)
                {
                    character.rulesetActor.RemoveCondition(condition);
                }
            }
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
        }
    }

    private sealed class ConditionDefinitionPreyingBuilder
        : ConditionDefinitionBuilder<ConditionDefinitionPreying, ConditionDefinitionPreyingBuilder>
    {
        internal ConditionDefinitionPreyingBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
        }
    }

    private static FeatureDefinitionPower BuildEldritchPredator(FeatureDefinitionPower sharedPool)
    {
        var preyingCondition = ConditionDefinitionPreyingBuilder
            .Create("ClassMagusConditionPreying", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusConditionPreying", Category.Class,
                DatabaseHelper.ConditionDefinitions.ConditionInvisible.guiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(
                DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityInvisible,
                DatabaseHelper.FeatureDefinitionPerceptionAffinitys.PerceptionAffinityConditionInvisible)
            .AddToDB();
        preyingCondition.characterShaderReference =
            DatabaseHelper.ConditionDefinitions.ConditionInvisible.characterShaderReference;
        preyingCondition.specialInterruptions.Clear();
        preyingCondition.specialInterruptions.AddRange(ConditionInterruption.AttacksAndDamages,
            ConditionInterruption.CastSpellExecuted);

        var preyingConditionForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            ConditionForm = new ConditionForm
            {
                forceOnSelf = true,
                ConditionDefinition = preyingCondition,
                applyToSelf = true,
                operation = ConditionForm.ConditionOperation.Add
            }
        };

        var paralyzedWhenBeingPreyedOn = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            hasSavingThrow = true,
            canSaveToCancel = true,
            saveOccurence = TurnOccurenceType.StartOfTurn,
            savingThrowAffinity = EffectSavingThrowType.Negates,
            ConditionForm = new ConditionForm
            {
                ConditionDefinition = ConditionParalyzedWhenBeingPreyedOn,
                operation = ConditionForm.ConditionOperation.Add
            }
        };

        var effect = EffectDescriptionBuilder
            .Create(DatabaseHelper.SpellDefinitions.GreaterInvisibility.EffectDescription)
            .DeepCopy()
            .ClearEffectForms()
            .SetTargetingData(Side.Enemy, RangeType.Self, 0,
                TargetType.InLineOfSightWithinDistance, 6, 6)
            .SetSavingThrowData(true, false, AttributeDefinitions.Wisdom, false,
                EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence)
            .SetDurationData(DurationType.Round, 0, false)
            .SetEffectForms(
                paralyzedWhenBeingPreyedOn,
                preyingConditionForm)
            .Build();

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMagusArcaneArtEldritchPredator", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusArcaneArtEldritchPredator")
            .SetSharedPool(ArcanaPool)
            .SetCostPerUse(2)
            .SetEffectDescription(effect)
            .SetActivationTime(ActivationTime.BonusAction)
            .AddToDB();
    }
    #endregion

    #region nullify_strike
    private sealed class ConditionNullify : ConditionDefinition,
        IDisableImmunityAndResistanceToDamageType
    {
        public bool DisableImmunityAndResistanceToDamageType(string damageType)
        {
            return true;
        }
    }

    private sealed class ConditionNullifyBuilder
        : ConditionDefinitionBuilder<ConditionNullify, ConditionNullifyBuilder>
    {
        internal ConditionNullifyBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
        }
    }
    private static FeatureDefinitionPower BuildNullifyStrike(FeatureDefinitionPower sharedPool)
    {
        ConditionNullify condition =
            ConditionNullifyBuilder
                .Create("ClassMagusConditionNullify", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Class, "ClassMagusConditionNullify",
                    DatabaseHelper.ConditionDefinitions.ConditionStunned.guiPresentation.spriteReference)
                .SetConditionType(ConditionType.Detrimental)
                .SetSpecialDuration(true)
                .AddToDB();
        condition.durationParameter = 1;
        condition.durationType = DurationType.Minute;
        
        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMagusArcaneArtNullifyStrike", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusArcaneArtNullifyStrike", null, 0, true)
            .SetSharedPool(sharedPool)
            .SetActivationTime(ActivationTime.NoCost)
            .SetCostPerUse(1)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24,
                        TargetType.Individuals)
                    .SetSavingThrowData(
                        true,
                        false,
                        AttributeDefinitions.Constitution,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Intelligence)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                            .Build()
                    )
                    .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.RayOfEnfeeblement)
                    .Build())
            .AddToDB();
        
        var canUseNullifyStrike = CharacterValidators.HasBeenGrantedFeature(power);
        power.SetCustomSubFeatures(new PowerUseValidity(canUseNullifyStrike, CharacterValidators.NoShield, CharacterValidators.FullyUnarmed));
        
        return power;
    }
    #endregion
    
    #region exile_strike
    private static FeatureDefinitionPower BuildExileStrike(FeatureDefinitionPower sharedPool)
    {
        var condition = ConditionDefinitionBuilder
            .Create("ClassMagusConditionExiled", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Class, "ClassMagusConditionExiled",
                ConditionBanished.GuiPresentation.SpriteReference)
            .SetParentCondition(DatabaseHelper.ConditionDefinitions.ConditionBanished)
            .AddToDB();
        condition.specialDuration = true;
        condition.durationParameter = 2;
        condition.durationParameterDie = DieType.D6;
        condition.durationType = DurationType.Round;
        condition.removedFromTheGame = true;
        condition.conditionParticleReference = ConditionBanished.conditionEndParticleReference;
        
        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMagusArcaneArtExileStrike", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusArcaneArtExileStrike", null, 0, true)
            .SetSharedPool(sharedPool)
            .SetActivationTime(ActivationTime.NoCost)
            .SetCostPerUse(1)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24,
                        TargetType.Individuals)
                    .SetSavingThrowData(
                        true,
                        false,
                        AttributeDefinitions.Charisma,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Intelligence)
                    .SetEffectForms(
                        new EffectForm()
                        {
                            formType = EffectForm.EffectFormType.Damage,
                            hasSavingThrow = true,
                            savingThrowAffinity = EffectSavingThrowType.HalfDamage,
                            damageForm = new DamageForm()
                            {
                                damageType = DamageTypeForce,
                                DiceNumber = 4,
                                DieType = DieType.D10
                            }
                        },
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                            .Build()
                    )
                    .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ChillTouch)
                    .Build())
            .AddToDB();
        
        var canUseExileStrike = CharacterValidators.HasBeenGrantedFeature(power);
        power.SetCustomSubFeatures(new PowerUseValidity(canUseExileStrike, CharacterValidators.NoShield, CharacterValidators.EmptyOffhand));
        
        return power;
    }
    #endregion
    
    #region terror_strike
    private static FeatureDefinitionPower BuildTerrorStrike(FeatureDefinitionPower sharedPool)
    {
        var condition = ConditionDefinitionBuilder
            .Create("ClassMagusConditionTerror", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Class, "ClassMagusConditionTerror",
                DatabaseHelper.ConditionDefinitions.ConditionFrightened.GuiPresentation.SpriteReference)
            .SetParentCondition(DatabaseHelper.ConditionDefinitions.ConditionFrightenedFear)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(DatabaseHelper.ConditionDefinitions.ConditionFrightenedFear.Features.ToArray())
            .AddToDB();
        condition.specialDuration = true;
        condition.durationParameter = 1;
        condition.durationType = DurationType.Minute;
        condition.forceBehavior = true;
        condition.fearSource = true;
        condition.battlePackage = DatabaseHelper.DecisionPackageDefinitions.Fear;
        
        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMagusArcaneArtTerrorStrike", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusArcaneArtTerrorStrike", null, 0, true)
            .SetSharedPool(sharedPool)
            .SetActivationTime(ActivationTime.NoCost)
            .SetCostPerUse(1)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24,
                        TargetType.Individuals)
                    .SetSavingThrowData(
                        true,
                        false,
                        AttributeDefinitions.Wisdom,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Intelligence)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                            .Build()
                    )
                    .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.Fear)
                    .Build())
            .AddToDB();
        
        var canUseTerrorStrike = CharacterValidators.HasBeenGrantedFeature(power);
        power.SetCustomSubFeatures(new PowerUseValidity(canUseTerrorStrike, CharacterValidators.NoShield, CharacterValidators.EmptyOffhand));
        
        return power;
    }
    #endregion
    
    #region unfathomable_horror
    private class ConditionFrightenByUnfathomableHorror : ConditionDefinition,
        IDisableImmunityToCondition
    {
        public bool DisableImmunityToCondition(string conditionName, ulong sourceGuid)
        {
            return conditionName == DatabaseHelper.ConditionDefinitions.ConditionFrightenedFear.Name;
        }
    }

    private class ConditionFrightenByUnfathomableHorrorBuilder
        : ConditionDefinitionBuilder<ConditionFrightenByUnfathomableHorror, ConditionFrightenByUnfathomableHorrorBuilder>
    {
        internal ConditionFrightenByUnfathomableHorrorBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
        }
    }
    private static FeatureDefinitionPower BuildUnfathomableHorror(FeatureDefinitionPower sharedPool)
    {
        var frightenByUnfathomableHorror = ConditionFrightenByUnfathomableHorrorBuilder
            .Create("ClassMagusConditionFrightenByUnfathomableHorror", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusConditionFrightenByUnfathomableHorror", Category.Class,
                DatabaseHelper.ConditionDefinitions.ConditionFrightenedFear.guiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        var effect = EffectDescriptionBuilder
            .Create(DatabaseHelper.SpellDefinitions.GreaterInvisibility.EffectDescription)
            .DeepCopy()
            .ClearEffectForms()
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3, 3)
            .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
            .SetSavingThrowData(true, false, AttributeDefinitions.Wisdom, false,
                EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Intelligence)
            .SetDurationData(DurationType.Permanent)
            .SetEffectForms(
                new EffectForm()
                {
                    FormType  = EffectForm.EffectFormType.Damage,
                    savingThrowAffinity = EffectSavingThrowType.Negates,
                    hasSavingThrow = true,
                    DamageForm = new DamageForm()
                    {
                        DamageType = DamageTypePsychic,
                        DiceNumber = 1,
                        DieType = DieType.D10
                    }
                },
                new EffectForm
                {
                    formType = EffectForm.EffectFormType.Condition,
                    hasSavingThrow = true,
                    savingThrowAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.StartOfTurn,
                    ConditionForm = new ConditionForm
                    {
                        ConditionDefinition = frightenByUnfathomableHorror,
                        operation = ConditionForm.ConditionOperation.Add
                    }
                })
            .Build();

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMagusArcaneArtUnfathomableError", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusArcaneArtUnfathomableError")
            .SetSharedPool(sharedPool)
            .SetEffectDescription(effect)
            .SetActivationTime(ActivationTime.PermanentUnlessIncapacitated)
            .AddToDB();
    }
    
    #endregion
    
    #region amor_of_the_magister
    private static FeatureDefinitionAttributeModifier enhancedMageAmor = FeatureDefinitionAttributeModifierBuilder
        .Create("ClassMagusArmorOfTheMagisterACModifier", DefinitionBuilder.CENamespaceGuid)
        .SetGuiPresentation("ClassMagusArmorOfTheMagisterACModifier", Category.Class)
        .SetSituationalContext(SituationalContext.None)
        .SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive)
        .SetModifierAbilityScore(AttributeDefinitions.Intelligence)
        .SetModifiedAttribute(AttributeDefinitions.ArmorClass)
        .AddToDB();
    private static EffectDescription EnhanceMageAmor(SpellDefinition spell, EffectDescription effect,
        RulesetCharacter caster)
    {
        if (spell != DatabaseHelper.SpellDefinitions.MageArmor)
        {
            return effect;
        }

        var effectForm = effect.GetFirstFormOfType(EffectForm.EffectFormType.Condition);
        FeatureDefinitionAttributeModifier feature = effectForm.conditionForm.conditionDefinition.features[0] as FeatureDefinitionAttributeModifier;
        if (feature != null)
        {
            feature.modifierValue =
                13 + AttributeDefinitions.ComputeAbilityScoreModifier(
                    caster.TryGetAttributeValue(AttributeDefinitions.Intelligence));
        }

        return effect;
    }
    private static FeatureDefinition BuildAmorOfTheMagister()
    {
        //  grant mage amor 
        var bonusSpellList = new List<SpellDefinition>();
        bonusSpellList.Add(DatabaseHelper.SpellDefinitions.MageArmor);
        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("ClassMagusArmorOfTheMagisterBonusSpell", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusArmorOfTheMagisterBonusSpell", Category.Class)
            .SetAutoTag("MagisterArmor")
            .SetSpellcastingClass(ClassMagus)
            .SetPreparedSpellGroups(
                new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
                { 
                    classLevel   = 1,
                    spellsList   = bonusSpellList
                })
            .AddToDB();
            
        // enhance mage armor effect
        var enhancedMageAmor = FeatureDefinitionSpellModifierBuilder
            .Create("ClassMagusArmorOfTheMagisterModifyMageAmorEffect", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusArmorOfTheMagisterModifyMageAmorEffect", Category.Class)
            .SetEffectModifier(EnhanceMageAmor)
            .AddToDB();
        
        return FeatureDefinitionFeatureSetBuilder
            .Create("ClassMagusArcaneArtArmorOfTheMagister", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusArcaneArtArmorOfTheMagister", Category.Class)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(autoPreparedSpells, enhancedMageAmor)
            .AddToDB();
    }
    #endregion
    
    #region greater_ban
    private static FeatureDefinition BuildGreaterBane()
    {
        //  grant bane 
        var bonusSpellList = new List<SpellDefinition>();
        bonusSpellList.Add(DatabaseHelper.SpellDefinitions.Bane);
        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("ClassMagusGreaterBaneBonusSpell", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusGreaterBaneBonusSpell", Category.Class)
            .SetAutoTag("GreaterBane")
            .SetSpellcastingClass(ClassMagus)
            .SetPreparedSpellGroups(
                new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
                { 
                    classLevel   = 1,
                    spellsList   = bonusSpellList
                })
            .AddToDB();
        
        // force disadvantage on enemies saving throw
        var greaterBane = FeatureDefinitionMagicAffinityBuilder
            .Create("ClassMagusGreaterBaneSavingThrowAffinityModifier", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusGreaterBaneSavingThrowAffinityModifier", Category.Class)
            .AddToDB();
        greaterBane.forcedSavingThrowAffinity = AdvantageType.Disadvantage;
        greaterBane.forcedSpellDefinition = DatabaseHelper.SpellDefinitions.Bane;
            
        // greater bane
        return FeatureDefinitionFeatureSetBuilder
            .Create("ClassMagusArcaneArtGreaterBane", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusArcaneArtGreaterBane", Category.Class)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(autoPreparedSpells, greaterBane)
            .AddToDB();
    }
    #endregion

    #region frost_fang

    private static FeatureDefinition BuildFrostFang(FeatureDefinitionPower sharedPool)
    {
        var frostFangAdditionalDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create("ClassMagusAdditionalDamageFrostFang", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusAdditionalDamageFrostFang", Category.Class)
            .SetNotificationTag("FrostFang")
            .SetDamageDice(DieType.D8, 2)
            .SetSpecificDamageType(DamageTypeCold)
            .SetImpactParticleReference(DatabaseHelper.SpellDefinitions.ConeOfCold.effectDescription.effectParticleParameters.impactParticleReference)
            .SetFrequencyLimit(FeatureLimitedUsage.None)
            .AddToDB();

        var frostFangCondition = ConditionDefinitionBuilder
            .Create("ClassMagusConditionFrostFang", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusConditionFrostFang", Category.Class)
            .SetSpecialDuration(true)
            .SetDuration(DurationType.Minute, 1)
            .SetFeatures(frostFangAdditionalDamage)
            .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(frostFangCondition, ConditionForm.ConditionOperation.Add, true, false)
                    .Build()
            )
            .Build();
        effectDescription.effectParticleParameters = DatabaseHelper.MonsterAttackDefinitions
            .Attack_Orc_Grimblade_IceBlade.effectDescription.effectParticleParameters;
        
        return FeatureDefinitionPowerBuilder
            .Create("ClassMagusArcaneArtFrostFang", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassMagusArcaneArtFrostFang", Category.Class)
            //.SetSharedPool(sharedPool)
            .SetActivationTime(ActivationTime.BonusAction)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetEffectDescription(effectDescription)
            .AddToDB();
    }
    #endregion
}
