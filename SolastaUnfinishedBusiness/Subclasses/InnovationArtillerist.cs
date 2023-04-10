using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static ActionDefinitions;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

public static class InnovationArtillerist
{
    private const string Name = "InnovationArtillerist";
    private const string CreatureTag = "EldritchCannon";
    private const string ConditionCommandEldritchCannon = $"Condition{Name}CommandEldritchCannon";
    private const string PowerSummonEldritchCannon = $"Power{Name}SummonEldritchCannon";
    private const string EldritchCannon = "EldritchCannon";
    private const string ExplosiveCannon = "ExplosiveCannon";
    private const string FortifiedPosition = "FortifiedPosition";
    private const string Flamethrower = "Flamethrower";
    private const string ForceBallista = "ForceBallista";
    private const string Protector = "Protector";
    private const string ArcaneFirearm = "ArcaneFirearm";

    // 
    // Common cannon features
    //

    private static readonly FeatureDefinitionActionAffinity ActionAffinityEldritchCannon =
        FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}{EldritchCannon}")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(
                Id.AttackMain, Id.AttackOff, Id.AttackFree, Id.AttackReadied, Id.AttackOpportunity, Id.Ready,
                Id.PowerMain, Id.PowerBonus, Id.PowerReaction, Id.SpendPower, Id.Shove, Id.ShoveBonus, Id.ShoveFree)
            .SetCustomSubFeatures(new SummonerHasConditionOrKOd())
            .AddToDB();

    private static readonly FeatureDefinitionConditionAffinity ConditionAffinityEldritchCannon =
        FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{Name}{EldritchCannon}")
            .SetGuiPresentationNoContent(true)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .SetConditionType(DatabaseHelper.ConditionDefinitions.ConditionSurprised)
            .SetCustomSubFeatures(ForceInitiativeToSummoner.Mark)
            .AddToDB();

    private static readonly FeatureDefinitionMoveMode MoveModeEldritchCannon =
        FeatureDefinitionMoveModeBuilder
            .Create($"MoveMode{Name}{EldritchCannon}")
            .SetGuiPresentationNoContent(true)
            .SetMode(MoveMode.Walk, 3)
            .AddToDB();

    public static CharacterSubclassDefinition Build()
    {
        var eldritchCannonSprite = Sprites.GetSprite(EldritchCannon, Resources.PowerEldritchCannon, 256, 128);

        // LEVEL 03

        // Common

        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}{EldritchCannon}HitPoints")
            .SetGuiPresentationNoContent(true)
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.HitPoints)
            .AddToDB();

        var summoningAffinityEldritchCannon = FeatureDefinitionSummoningAffinityBuilder
            .Create($"SummoningAffinity{Name}{EldritchCannon}")
            .SetGuiPresentationNoContent(true)
            .SetRequiredMonsterTag(CreatureTag)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}{EldritchCannon}CopyCharacterLevel")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCopyAttributeFromSummoner,
                        AttributeDefinitions.CharacterLevel)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}{EldritchCannon}CopyProficiencyBonus")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCopyAttributeFromSummoner,
                        AttributeDefinitions.ProficiencyBonus)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}{EldritchCannon}HitPoints")
                    .SetGuiPresentationNoContent()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceClassLevel)
                    .SetFeatures(hpBonus, hpBonus, hpBonus, hpBonus, hpBonus)
                    .AddToDB())
            .AddToDB();

        // Cannon Powers

        var powerFlamethrower = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{Flamethrower}")
            .SetGuiPresentation(Category.Feature, PowerDomainElementalFireBurst)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(BurningHands)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.FixedValue, AttributeDefinitions.Intelligence, 15)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeFire, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.CharacterLevel, 2, 1, 6, 3)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetAlterationForm(AlterationForm.Type.LightUp)
                            .Build())
                    .Build())
            .AddToDB();

        var powerForceBallista = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ForceBallista}")
            .SetGuiPresentation(Category.Feature, PowerMarksmanRecycler)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(EldritchBlast)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.CharacterLevel, 2, 1, 6, 3)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                            .Build())
                    .Build())
            .AddToDB();

        var powerProtector = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{Protector}")
            .SetGuiPresentation(Category.Feature, PowerTraditionCourtMageSpellShield)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(EldritchBlast)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 2)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(0, DieType.D8, 2)
                            .SetDiceAdvancement(LevelSourceType.CharacterLevel, 2, 1, 6, 3)
                            .Build())
                    .Build())
            .AddToDB();

        // Eldritch Cannon

        var powerEldritchCannonPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{EldritchCannon}")
            .SetGuiPresentation($"Power{Name}{EldritchCannon}", Category.Feature, eldritchCannonSprite)
            .SetUsesFixed(ActivationTime.Action)
            // only adding this for UI reasons
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 1, TargetType.Position)
                    .Build())
            .AddToDB();

        var powerCannonFlamethrower03 = BuildFlamethrowerPower(powerEldritchCannonPool, 3, powerFlamethrower);
        var powerCannonForceBallista03 = BuildForceBallistaPower(powerEldritchCannonPool, 3, powerForceBallista);
        var powerCannonProtector03 = BuildProtectorPower(powerEldritchCannonPool, 3, powerProtector);

        var featureSetEldritchCannon = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{EldritchCannon}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildCommandEldritchCannon(),
                summoningAffinityEldritchCannon,
                powerEldritchCannonPool,
                powerCannonFlamethrower03,
                powerCannonForceBallista03,
                powerCannonProtector03)
            .AddToDB();

        // LEVEL 05

        // Detonate

        var powerDetonate = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EldritchDetonation")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Sphere, 4)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeForce, 3, DieType.D8)
                            .Build())
                    .Build())
            .AddToDB();

        // Arcane Firearm

        var additionalDamageArcaneFirearm = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}{ArcaneFirearm}")
            .SetGuiPresentation($"FeatureSet{Name}{ArcaneFirearm}", Category.Feature)
            .SetNotificationTag(ArcaneFirearm)
            .SetRequiredProperty(RestrictedContextRequiredProperty.SpellWithAttackRoll)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.SpellDamagesTarget)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetDamageDice(DieType.D8, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 10, 5)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddToDB();

        var featureSetArcaneFirearm = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{ArcaneFirearm}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                additionalDamageArcaneFirearm,
                MagicAffinitySpellBladeIntoTheFray,
                powerDetonate)
            .AddToDB();

        // LEVEL 09

        // Explosive Cannon

        var powerExplosiveCannonPool = FeatureDefinitionPowerBuilder
            .Create(powerEldritchCannonPool, $"Power{Name}{ExplosiveCannon}")
            .SetOverriddenPower(powerEldritchCannonPool)
            .AddToDB();

        var powerCannonFlamethrower09 = BuildFlamethrowerPower(powerExplosiveCannonPool, 9, powerFlamethrower);
        var powerCannonForceBallista09 = BuildForceBallistaPower(powerExplosiveCannonPool, 9, powerForceBallista);
        var powerCannonProtector09 = BuildProtectorPower(powerExplosiveCannonPool, 9, powerProtector);

        var featureSetExplosiveCannon = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{ExplosiveCannon}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerExplosiveCannonPool,
                powerCannonFlamethrower09,
                powerCannonForceBallista09,
                powerCannonProtector09)
            .AddToDB();

        // LEVEL 15

        var powerFortifiedPositionPool = FeatureDefinitionPowerBuilder
            .Create(powerEldritchCannonPool, $"Power{Name}{FortifiedPosition}")
            .SetOverriddenPower(powerEldritchCannonPool)
            .AddToDB();

        var powerCannonFlamethrower15 = BuildFlamethrowerPower(powerFortifiedPositionPool, 15, powerFlamethrower);
        var powerCannonForceBallista15 = BuildForceBallistaPower(powerFortifiedPositionPool, 15, powerForceBallista);
        var powerCannonProtector15 = BuildProtectorPower(powerFortifiedPositionPool, 15, powerProtector);

        var featureSetFortifiedPosition = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{FortifiedPosition}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerFortifiedPositionPool,
                powerCannonFlamethrower15,
                powerCannonForceBallista15,
                powerCannonProtector15)
            .AddToDB();

        //
        // MAIN
        //

        PowerBundle.RegisterPowerBundle(powerEldritchCannonPool, true,
            powerCannonFlamethrower03,
            powerCannonForceBallista03,
            powerCannonProtector03);

        PowerBundle.RegisterPowerBundle(powerExplosiveCannonPool, true,
            powerCannonFlamethrower09,
            powerCannonForceBallista09,
            powerCannonProtector09);

        PowerBundle.RegisterPowerBundle(powerFortifiedPositionPool, true,
            powerCannonFlamethrower15,
            powerCannonForceBallista15,
            powerCannonProtector15);

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.ArtilleristCannon,
            powerCannonFlamethrower03,
            powerCannonForceBallista03,
            powerCannonProtector03,
            powerCannonFlamethrower09,
            powerCannonForceBallista09,
            powerCannonProtector09,
            powerCannonFlamethrower15,
            powerCannonForceBallista15,
            powerCannonProtector15);

        return CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.InventorArtillerist, 256))
            .AddFeaturesAtLevel(3, featureSetEldritchCannon, BuildAutoPreparedSpells())
            .AddFeaturesAtLevel(5, featureSetArcaneFirearm)
            .AddFeaturesAtLevel(9, featureSetExplosiveCannon)
            .AddFeaturesAtLevel(15, featureSetFortifiedPosition)
            .AddToDB();
    }

    private static FeatureDefinition BuildAutoPreparedSpells()
    {
        return FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(InventorClass.Class)
            .SetAutoTag("InventorArtillerist")
            .AddPreparedSpellGroup(3, Shield, Thunderwave)
            .AddPreparedSpellGroup(5, ScorchingRay, Shatter)
            .AddPreparedSpellGroup(9, Fireball, WindWall)
            .AddPreparedSpellGroup(13, IceStorm, WallOfFire)
            .AddPreparedSpellGroup(17, ConeOfCold, WallOfForce)
            .AddToDB();
    }

    private static MonsterDefinition BuildEldritchCannonMonster(
        string cannonName,
        MonsterDefinition monsterDefinition,
        int level,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var monsterName = $"{Name}{cannonName}";
        var presentationName = $"Power{Name}SummonEldritchCannon{cannonName}";

        var monster = MonsterDefinitionBuilder
            .Create(monsterDefinition, monsterName + level)
            .SetGuiPresentation(presentationName, Category.Feature, monsterDefinition)
            .SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Small)
            .NoExperienceGain()
            .SetArmorClass(18)
            .SetChallengeRating(0)
            .SetStandardHitPoints(1)
            .SetHitDice(DieType.D6, level)
            .SetAbilityScores(10, 10, 10, 10, 10, 10)
            .SetDefaultFaction(DatabaseHelper.FactionDefinitions.Party)
            .SetCharacterFamily(DatabaseHelper.CharacterFamilyDefinitions.Construct)
            .SetCreatureTags(CreatureTag)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.Full)
            .SetFullyControlledWhenAllied(true)
            .SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None)
            .ClearAttackIterations()
            .SetFeatures(
                ActionAffinityEldritchCannon,
                ConditionAffinityEldritchCannon,
                MoveModeEldritchCannon,
                ConditionAffinityPoisonImmunity,
                DamageAffinityPoisonImmunity,
                DamageAffinityPsychicImmunity,
                SenseNormalVision,
                MovementAffinityNoSpecialMoves,
                MovementAffinitySpiderClimb)
            .AddFeatures(monsterAdditionalFeatures)
            .AddToDB();

        monster.guiPresentation.description = GuiPresentationBuilder.EmptyString;

        return monster;
    }

    private static FeatureDefinitionPower BuildFlamethrowerPower(FeatureDefinitionPower sharedPoolPower, int level,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        return BuildEldritchCannonPower(
            Flamethrower,
            sharedPoolPower,
            DatabaseHelper.MonsterDefinitions.Fire_Spider,
            level,
            monsterAdditionalFeatures);
    }

    private static FeatureDefinitionPower BuildForceBallistaPower(FeatureDefinitionPower sharedPoolPower, int level,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        return BuildEldritchCannonPower(
            ForceBallista,
            sharedPoolPower,
            DatabaseHelper.MonsterDefinitions.PhaseSpider,
            level,
            monsterAdditionalFeatures);
    }

    private static FeatureDefinitionPower BuildProtectorPower(FeatureDefinitionPower sharedPoolPower, int level,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        return BuildEldritchCannonPower(
            Protector,
            sharedPoolPower,
            DatabaseHelper.MonsterDefinitions.SpectralSpider,
            level,
            monsterAdditionalFeatures);
    }

    private static FeatureDefinitionPower BuildEldritchCannonPower(
        string powerName,
        FeatureDefinitionPower sharedPoolPower,
        MonsterDefinition monsterDefinition,
        int level,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        const string DESCRIPTION = $"Feature/&Power{Name}{EldritchCannon}Description";

        var name = PowerSummonEldritchCannon + powerName;
        var monster = BuildEldritchCannonMonster(powerName, monsterDefinition, level, monsterAdditionalFeatures);

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(name + level)
            .SetGuiPresentation(name, Category.Feature, DESCRIPTION, hidden: true)
            .SetSharedPool(ActivationTime.Action, sharedPoolPower)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Permanent)
                .SetTargetingData(Side.Ally, RangeType.Distance, 1, TargetType.Position)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetSummonCreatureForm(1, monster.Name)
                        .Build())
                .SetParticleEffectParameters(ConjureGoblinoids)
                .Build())
            .SetUniqueInstance()
            .SetCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always)
            .AddToDB();
    }

    private static FeatureDefinition BuildCommandEldritchCannon()
    {
        var condition = ConditionDefinitionBuilder
            .Create(ConditionCommandEldritchCannon)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var powerWildMasterSpiritBeastCommand = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{EldritchCannon}Command")
            .SetGuiPresentation(Category.Feature, Command)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(new ShowInCombatWhenHasCannon())
            .AddToDB();

        powerWildMasterSpiritBeastCommand.AddCustomSubFeatures(
            new ApplyOnTurnEnd(condition, powerWildMasterSpiritBeastCommand));

        return powerWildMasterSpiritBeastCommand;
    }

    private class SummonerHasConditionOrKOd : IDefinitionApplicationValidator, ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            // if commanded allow anything
            if (IsCommanded(locationCharacter.RulesetCharacter))
            {
                return;
            }

            // if not commanded it cannot move
            locationCharacter.usedTacticalMoves = locationCharacter.MaxTacticalMoves;

            // or use powers so force the dodge action
            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(new CharacterActionParams(locationCharacter, Id.Dodge), null, false);
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character)
        {
            //Apply limits if not commanded
            return !IsCommanded(character);
        }

        private static bool IsCommanded(RulesetCharacter character)
        {
            //can act freely outside of battle
            if (Gui.Battle == null)
            {
                return true;
            }

            var summoner = character.GetMySummoner()?.RulesetCharacter;

            //shouldn't happen, but consider being commanded in this case
            if (summoner == null)
            {
                return true;
            }

            //can act if summoner is KO
            return summoner.IsUnconscious ||
                   //can act if summoner commanded
                   summoner.HasConditionOfType(ConditionCommandEldritchCannon);
        }
    }

    private class ApplyOnTurnEnd : ICharacterTurnEndListener
    {
        private readonly ConditionDefinition condition;
        private readonly FeatureDefinitionPower power;

        public ApplyOnTurnEnd(ConditionDefinition condition, FeatureDefinitionPower power)
        {
            this.condition = condition;
            this.power = power;
        }

        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
            var status = locationCharacter.GetActionStatus(Id.PowerBonus, ActionScope.Battle);

            if (status != ActionStatus.Available)
            {
                return;
            }

            var character = locationCharacter.RulesetCharacter;
            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                character.Guid,
                condition,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                character.Guid,
                character.CurrentFaction.Name);

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
            GameConsoleHelper.LogCharacterUsedPower(character, power);
        }
    }

    private class ShowInCombatWhenHasCannon : IPowerUseValidity
    {
        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            return ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress &&
                   character.powersUsedByMe.Any(p => p.sourceDefinition.Name.StartsWith(PowerSummonEldritchCannon));
        }
    }
}
