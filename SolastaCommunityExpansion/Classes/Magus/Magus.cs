using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
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

    public static FeatureDefinitionPower ArcaneFocus { get; private set; }

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
        FeatureDefinitionClassMagusCastSpell = FeatureDefinitionCastSpellBuilder
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
            .AddToDB();
    }

    private static void BuildArcaneArt()
    {
        ArcaneFocus = FeatureDefinitionPowerBuilder
            .Create("ClassMagusArcaneFocus", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power)
            .SetFixedUsesPerRecharge(4)
            .SetActivationTime(ActivationTime.NoCost)
            .SetRechargeRate(RechargeRate.AtWill)
            .AddToDB();

        // rupture: damage on moving
        var rupture = BuildRupture(ArcaneFocus);

        // eldritch predator
        // goes invisible and paralyze enemies within 6 cells
        // both invisible and paralyzed condition end after casting a spell or making an attack
        // or using power
        var eldritchPredator = BuildEldritchPredator(ArcaneFocus);

        // torment blade
        // disable resistance and immunity to damage for one round
        var tormentBlade = BuildTormentBlade();

        // broken courage
        // menace present
        // mind spike
        // exile
        // immunity disruption: fire, ice, acid, thunder, lighting, necro
        // soul drinker: regain an arcana focus

        PowerBundleContext.RegisterPowerBundle(ArcaneFocus, true, rupture);

        ArcaneArt = FeatureDefinitionFeatureSetCustomBuilder
            .Create("ClassMagusArcaneArtSetLevel", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature,
                CustomIcons.CreateAssetReferenceSprite("ArcaneArt", Resources.EldritchInvocation, 128, 128))
            .SetRequireClassLevels(true)
            .SetLevelFeatures(2, rupture, eldritchPredator, tormentBlade)
            .AddToDB();
    }

    private static void BuildProgression(CharacterClassDefinitionBuilder classMagusBuilder)
    {
        // No subclass for now
        /*var subclassChoices = FeatureDefinitionSubclassChoiceBuilder
            .Create("ClassWarlockSubclassChoice", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassWarlockPatron", Category.Subclass)
            .SetSubclassSuffix("Patron")
            .SetFilterByDeity(false)
            .SetSubclasses(
                AHWarlockSubclassSoulBladePact.Build(),
                DHWarlockSubclassAncientForestPatron.Build(),
                DHWarlockSubclassElementalPatron.Build(),
                DHWarlockSubclassMoonLitPatron.Build()
            )
            .AddToDB();*/

        classMagusBuilder
            .AddFeaturesAtLevel(1,
                FeatureDefinitionProficiencySavingThrow,
                FeatureDefinitionProficiencyArmor,
                FeatureDefinitionProficiencyWeapon,
                FeatureDefinitionProficiencyTool,
                FeatureDefinitionSkillPoints
            )
            .AddFeaturesAtLevel(2, FeatureDefinitionClassMagusCastSpell, ArcaneFocus, ArcaneArt, ArcaneArt, ArcaneArt)
            .AddFeatureAtLevel(4, DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)
            .AddFeatureAtLevel(8, DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)
            .AddFeatureAtLevel(12, DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
    }

    internal static CharacterClassDefinition BuildMagusClass()
    {
        var warlockSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("Magus", Resources.Warlock, 1024, 576);

        var classMagusBuilder = CharacterClassDefinitionBuilder
            .Create("ClassMagus", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Class, warlockSpriteReference, 1)
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
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Charisma)
            .SetAnimationId(AnimationDefinitions.ClassAnimationId.Paladin)
            .SetBattleAI(DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeWithBackupRangeDecisions)
            .SetHitDice(DieType.D8)
            .SetIngredientGatheringOdds(DatabaseHelper.CharacterClassDefinitions.Sorcerer.IngredientGatheringOdds)
            .SetPictogram(DatabaseHelper.CharacterClassDefinitions.Wizard.ClassPictogramReference);

        BuildEquipment(classMagusBuilder);
        BuildProficiencies();
        BuildSpells();
        BuildArcaneArt();
        BuildProgression(classMagusBuilder);

        ClassMagus = classMagusBuilder.AddToDB();

        return ClassMagus;
    }

    #region rupture
    private static FeatureDefinitionPower BuildRupture(FeatureDefinitionPower sharedPool)
    {
        var condition = ConditionDefinitionBuilder
            .Create("ClassMagusConditionRupture", DefinitionBuilder.CENamespaceGuid)
            .Configure(DurationType.Round, 1, true)
            .SetGuiPresentation(Category.Class, "ClassMagusConditionRupture",
                ConditionSlowed.GuiPresentation.SpriteReference)
            .AddToDB();
        
        condition.turnOccurence = (TurnOccurenceType)ExtraTurnOccurenceType.OnMoveEnd;
        condition.RecurrentEffectForms.Add(new EffectForm
        {
            formType = EffectForm.EffectFormType.Damage,
            damageForm = new DamageForm { damageType = DamageTypeNecrotic, diceNumber = 1, dieType = DieType.D4 }
        });

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMagusArcaneArtRupture", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusArcaneArtRupture")
            .SetSharedPool(sharedPool)
            .SetActivationTime(ActivationTime.OnAttackHit)
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
    }
    #endregion
    
    #region eldritch_predator
    // TODO: do something here to make creatures that are immune to condition paralyze also not affected by this condition unless they are hit by torment blade.
    private static readonly ConditionDefinition conditionParalyzedWhenBeingPreyedOn = ConditionDefinitionBuilder
        .Create(DatabaseHelper.ConditionDefinitions.ConditionParalyzed, "ClassMagusConditionParalyzedWhenBeingPreyedOn",
            DefinitionBuilder.CENamespaceGuid)
        .DeepCopy()
        .SetCustomSubFeatures()
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
                        if (linkedCondition.ConditionDefinition.Name == conditionParalyzedWhenBeingPreyedOn.Name &&
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

        var paralyzedByWhenDeathLurkingAroundConditionForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            hasSavingThrow = true,
            canSaveToCancel = true,
            saveOccurence = TurnOccurenceType.StartOfTurn,
            savingThrowAffinity = EffectSavingThrowType.Negates,
            ConditionForm = new ConditionForm
            {
                ConditionDefinition = conditionParalyzedWhenBeingPreyedOn,
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
                paralyzedByWhenDeathLurkingAroundConditionForm,
                preyingConditionForm)
            .Build();

        return FeatureDefinitionPowerBuilder
            .Create("ClassMagusArcaneArtEldritchPredator", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusArcaneArtEldritchPredator")
            .SetRechargeRate(RechargeRate.AtWill)
            .SetEffectDescription(effect)
            .SetActivationTime(ActivationTime.BonusAction)
            .AddToDB();
    }

    #endregion

    #region torment_blade
    private sealed class ConditionTormentBladeDisruption : ConditionDefinition,
        IDisableImmunityAndResistanceToDamageType
    {
        public bool DisableImmunityAndResistanceToDamageType(string damageType)
        {
            return true;
        }
    }

    private sealed class ConditionTormentBladeDisruptionBuilder
        : ConditionDefinitionBuilder<ConditionTormentBladeDisruption, ConditionTormentBladeDisruptionBuilder>
    {
        internal ConditionTormentBladeDisruptionBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
        }
    }
    private static FeatureDefinitionPower BuildTormentBlade()
    {
        ConditionTormentBladeDisruption conditionDisruptedByTormentBlade =
            ConditionTormentBladeDisruptionBuilder
                .Create("ClassMagusConditionDisruptedByTormentBlade", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Class, "ClassMagusConditionDisruptedByTormentBlade",
                    DatabaseHelper.ConditionDefinitions.ConditionStunned.guiPresentation.spriteReference)
                .SetConditionType(ConditionType.Detrimental)
                .SetSpecialDuration(true)
                .SetTerminateWhenRemoved(true)
                .AddToDB();
        conditionDisruptedByTormentBlade.durationParameterDie = DieType.D1;
        conditionDisruptedByTormentBlade.durationParameter = 1;
        conditionDisruptedByTormentBlade.durationType = DurationType.Minute;
        
        FeatureDefinitionAdditionalDamage additionalDamageTormentBlade =
            FeatureDefinitionAdditionalDamageBuilder
                .Create("ClassMagusAdditionalDamageTormentBlade", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Class, "ClassMagusAdditionalDamageTormentBlade")
                
                .Configure("TormentBlade", FeatureLimitedUsage.None,
                    AdditionalDamageValueDetermination.None,
                    AdditionalDamageTriggerCondition.AlwaysActive
                    , AdditionalDamageRequiredProperty.MeleeWeapon, true, DieType.D1, 0,
                    AdditionalDamageType.SameAsBaseDamage, DamageTypeNecrotic,
                    AdditionalDamageAdvancement.None, new List<DiceByRank>())
                .AddToDB();
        additionalDamageTormentBlade.hasSavingThrow = true;
        additionalDamageTormentBlade.savingThrowAbility = AttributeDefinitions.Constitution;
        additionalDamageTormentBlade.damageSaveAffinity = EffectSavingThrowType.Negates;
        additionalDamageTormentBlade.dcComputation = EffectDifficultyClassComputation.SpellCastingFeature;
        additionalDamageTormentBlade.conditionOperations.Add(new ConditionOperationDescription()
        {
            conditionDefinition =  conditionDisruptedByTormentBlade,
            conditionName =  conditionDisruptedByTormentBlade.Name,
            operation = ConditionOperationDescription.ConditionOperation.Add,
        });

        ConditionDefinition classMagusConditionTormentBlade =
            ConditionDefinitionBuilder
                .Create("ClassMagusConditionTormentBlade", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Class ,"ClassMagusConditionTormentBlade", DatabaseHelper.ConditionDefinitions.ConditionRaging.guiPresentation.spriteReference)
                .AddFeatures(additionalDamageTormentBlade)
                .SetConditionParticleReference(DatabaseHelper.ConditionDefinitions.ConditionRaging.conditionParticleReference)
                .AddToDB();

        var effectForm = EffectFormBuilder
            .Create()
            .CreatedByCharacter()
            .SetConditionForm(classMagusConditionTormentBlade, ConditionForm.ConditionOperation.Add, true, true)
            .Build();

        var effect = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Round, 10, TurnOccurenceType.EndOfTurn)
            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
            .SetCreatedByCharacter()
            .AddEffectForm(effectForm)
            .Build();

        return FeatureDefinitionPowerBuilder
            .Create("ClassMagusArcaneArtBaneBlade", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusArcaneArtBaneBlade")
            .SetRechargeRate(RechargeRate.AtWill)
            .SetEffectDescription(effect)
            .SetActivationTime(ActivationTime.BonusAction)
            .AddToDB();
    }

    #endregion
}
