using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;

public static class InnovationWeapon
{
    private const string SteelDefenderTag = "SteelDefender";

    public static CharacterSubclassDefinition Build()
    {
        return CharacterSubclassDefinitionBuilder
            .Create("InnovationWeapon")
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.OathOfJugement)
            .AddFeaturesAtLevel(3, BuildBattleReady(), BuildAutoPreparedSpells(), BuildSteelDefenderFeatureSet())
            .AddFeaturesAtLevel(5, BuildExtraAttack())
            .AddToDB();
    }

    private static FeatureDefinition BuildBattleReady()
    {
        return FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyInnovationWeaponBattleReady")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
            .SetCustomSubFeatures(new CanUseAttributeForWeapon(AttributeDefinitions.Intelligence,
                ValidatorsWeapon.IsMagic))
            .AddToDB();
    }

    private static FeatureDefinition BuildAutoPreparedSpells()
    {
        return FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsInnovationWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(InventorClass.Class)
            .SetAutoTag("InventorWeaponsmith")
            .AddPreparedSpellGroup(3, Heroism, Shield)
            .AddPreparedSpellGroup(5, BrandingSmite, SpiritualWeapon)
            .AddPreparedSpellGroup(9, RemoveCurse, BeaconOfHope)
            .AddPreparedSpellGroup(13, FireShield, DeathWard)
            .AddPreparedSpellGroup(17, MassCureWounds, WallOfForce)
            .AddToDB();
    }

    private static FeatureDefinition BuildExtraAttack()
    {
        return FeatureDefinitionAttributeModifierBuilder
            .Create("ProficiencyInnovationWeaponExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.ForceIfBetter, AttributeDefinitions.AttacksNumber, 2)
            .AddToDB();
    }

    private static FeatureDefinition BuildSteelDefenderFeatureSet()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInnovationWeaponSteelDefender")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                BuildSteelDefenderPower(),
                BuildSteelDefenderAffinity()
            )
            .AddToDB();
    }

    private static FeatureDefinition BuildSteelDefenderPower()
    {
        var defender = BuildSteelDefenderMonster();

        return FeatureDefinitionPowerBuilder
            .Create("PowerInnovationWeaponSummonSteelDefender")
            .SetGuiPresentation(Category.Feature,
                CustomIcons.CreateAssetReferenceSprite("SteelDefenderPower", Resources.SteelDefenderPower, 256, 128))
            .SetUsesFixed(1)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetUniqueInstance()
            .SetCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Permanent)
                .SetTargetingData(Side.Ally, RangeType.Distance, 3, TargetType.Position)
                .AddEffectForm(EffectFormBuilder.Create()
                    .SetSummonCreatureForm(1, defender.Name)
                    .Build())
                .SetParticleEffectParameters(ConjureElementalAir)
                .Build())
            .AddToDB();
    }

    private static FeatureDefinition BuildSteelDefenderAffinity()
    {
        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeInnovationWeaponSummonSteelDefenderHP")
            .SetGuiPresentationNoContent()
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.HitPoints, 0)
            .AddToDB();

        var toHit = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierInnovationWeaponSummonSteelDefenderToHit")
            .SetGuiPresentationNoContent()
            .SetAttackRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var toDamage = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierInnovationWeaponSummonSteelDefenderDamage")
            .SetGuiPresentationNoContent()
            .SetDamageRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var savingThrows = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("AttributeInnovationWeaponSummonSteelDefenderSaves")
            .SetGuiPresentationNoContent()
            .UseControllerSavingThrows()
            .AddToDB();

        var skills = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AttributeInnovationWeaponSummonSteelDefenderSkills")
            .SetGuiPresentationNoContent()
            .UseControllerAbilityChecks()
            .AddToDB();

        return FeatureDefinitionSummoningAffinityBuilder
            .Create("SummoningAffinityInnovationWeaponSummonSteelDefender")
            .SetGuiPresentationNoContent()
            .SetRequiredMonsterTag(SteelDefenderTag)
            .SetAddedConditions(
                //Generic bonuses
                ConditionDefinitionBuilder
                    .Create("ConditionInnovationWeaponSummonSteelDefenderGeneric")
                    .SetGuiPresentationNoContent()
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellAttack)
                    .SetFeatures(savingThrows, skills)
                    .AddToDB(),
                //Bonuses from Inventor's spell attack
                ConditionDefinitionBuilder
                    .Create("ConditionInnovationWeaponSummonSteelDefenderSpellAttack")
                    .SetGuiPresentationNoContent()
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellAttack)
                    .SetFeatures(toHit)
                    .AddToDB(),
                //Bonuses from Inventor's Proficiency Bonus
                ConditionDefinitionBuilder
                    .Create("ConditionInnovationWeaponSummonSteelDefenderProficiencyBonus")
                    .SetGuiPresentationNoContent()
                    .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                    .SetFeatures(toDamage)
                    .AddToDB(),
                //Bonuses from Inventor's level
                ConditionDefinitionBuilder
                    .Create("ConditionInnovationWeaponSummonSteelDefenderLevel")
                    .SetGuiPresentationNoContent()
                    .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel)
                    //Set damage type to class name so `ExtraOriginOfAmount.SourceClassLevel` would know what class to use
                    .SetAdditionalDamageWhenHit(damageType: InventorClass.ClassName, active: false)
                    .SetFeatures(hpBonus, hpBonus, hpBonus, hpBonus, hpBonus) // 5 HP per level
                    .AddToDB(),
                //Bonuses from Inventor's INT
                ConditionDefinitionBuilder
                    .Create("ConditionInnovationWeaponSummonSteelDefenderIntelligence")
                    .SetGuiPresentationNoContent()
                    .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceAbilityBonus)
                    //Set damage type to class name so `ExtraOriginOfAmount.SourceAbilityBonus` would know what class to use
                    .SetAdditionalDamageWhenHit(damageType: AttributeDefinitions.Intelligence, active: false)
                    .SetFeatures(hpBonus) // 1 hp per INT mod
                    .AddToDB()
            )
            .AddToDB();
    }

    private static MonsterDefinition BuildSteelDefenderMonster()
    {
        var rend = MonsterAttackDefinitionBuilder
            .Create("MonsterAttackSteelDefender")
            //TODO: add gui presentation
            .SetToHitBonus(0)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetEffectForms(new EffectFormBuilder()
                    .SetDamageForm(dieType: DieType.D8, diceNumber: 1, bonusDamage: 0, damageType: DamageTypeForce)
                    .Build())
                .Build()
            )
            .AddToDB();

        var monster = MonsterDefinitionBuilder
            .Create("MonsterInnovationWeaponSteelDefender")
            .SetGuiPresentation(Category.Monster,
                CustomIcons.CreateAssetReferenceSprite("SteelDefenderMonster", Resources.SteelDefenderMonster, 160,
                    240))
            .HideFromDungeonEditor()
            .SetAbilityScores(14, 12, 14, 4, 10, 6)
            .SetSkillScores( //currently setup to use Inventor's skills
                (SkillDefinitions.Athletics, 2), //TODO: add Inventor's PB to the skill
                (SkillDefinitions.Perception, 0) //TODO: add Inventor's PB to the skill
            )
            .SetSavingThrowScores( //currently setup to use Inventor's saves
                (AttributeDefinitions.Dexterity, 1), //TODO: add Inventor's PB to the save
                (AttributeDefinitions.Constitution, 2) //TODO: add Inventor's PB to the save
            )
            .SetStandardHitPoints(2)
            .SetHitPointsBonus(2) //doesn't seem to be used anywhere
            .SetHitDice(DieType.D8, 1) //TODO: setup to 1 die per inventor level
            .SetArmorClass(15, EquipmentDefinitions.EmptyMonsterArmor) //natural armor
            .SetAttackIterations((1, rend))
            //.SetGroupAttacks(true)
            .SetFeatures(
                FeatureDefinitionMoveModes.MoveModeMove8,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision12,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,

                //TODO: add repair power
                //TODO: add `Deflect Attack` - reaction to impose disadvantage on attack against ally in 5ft
                //TODO: add Surprised immunity
                //TODO: make it only have reaction and dodge unless summoner used bonus action to grant full actions
                FeatureDefinitionBuilder.Create("FeatureInnovationWeaponSteelDefenderInitiative")
                    .SetGuiPresentationNoContent()
                    .SetCustomSubFeatures(ForceInitiativeToSummoner.Mark)
                    .AddToDB(),
                FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity
            )
            .SetCreatureTags(SteelDefenderTag)
            .SetDefaultFaction(FactionDefinitions.Party)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultBattleDecisionPackage(DecisionPackageDefinitions.DefaultMeleeWithBackupRangeDecisions)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetCharacterFamily(CharacterFamilyDefinitions.Construct)
            .SetAlignment(AlignmentDefinitions.Neutral)
            // .SetLegendaryCreature(false)
            .NoExperienceGain()
            .SetChallengeRating(0)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .SetMonsterPresentation(MonsterPresentationBuilder.Create()
                .SetPrefab(EffectProxyDefinitions.ProxyArcaneSword.prefabReference)
                .SetModelScale(0.5f)
                .SetHasMonsterPortraitBackground(true)
                .SetCanGeneratePortrait(true)
                //portrait properties don't seem to work
                // .SetPortraitFOV(20)
                // .SetPortraitCameraFollowOffset(y: -0.75f)
                .Build())
            .AddToDB();

        return monster;
    }
}
