using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using static ActionDefinitions;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;

public static class InnovationWeapon
{
    private const string SteelDefenderTag = "SteelDefender";
    private const string CommandSteelDefenderCondition = "ConditionInventorWeaponSteelDefenerCommand";
    private const string SummonSteelDefenderPower = "PowerInnovationWeaponSummonSteelDefender";

    public static CharacterSubclassDefinition Build()
    {
        return CharacterSubclassDefinitionBuilder
            .Create("InnovationWeapon")
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.OathOfJugement)
            .AddFeaturesAtLevel(3, BuildBattleReady(), BuildAutoPreparedSpells(), BuildSteelDefenderFeatureSet())
            .AddFeaturesAtLevel(5, BuildExtraAttack())
            .AddFeaturesAtLevel(9, BuildArcaneJolt())
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
                //TODO: add short-rest camping activity to Inventor that would heal Blade by 1d8, Inventor level times per long rest, similar to Hit Die rolling by heroes 
                BuildSteelDefenderPower(),
                BuildCommandSteelDefender(),
                BuildSteelDefenderAffinity()
            )
            .AddToDB();
    }

    private static FeatureDefinition BuildSteelDefenderPower()
    {
        var defender = BuildSteelDefenderMonster();

        return FeatureDefinitionPowerBuilder
            .Create(SummonSteelDefenderPower)
            .SetGuiPresentation(Category.Feature,
                CustomIcons.GetSprite("SteelDefenderPower", Resources.SteelDefenderPower, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 3, TargetType.Position)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetSummonCreatureForm(1, defender.Name)
                        .Build())
                    .SetParticleEffectParameters(ConjureElementalAir)
                    .Build())
            .SetUniqueInstance()
            .SetCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always, ValidatorPowerUse.NotInCombat)
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
                    .SetGuiPresentation(Category.Condition, Gui.NoLocalization)
                    .SetPossessive()
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
            .SetGuiPresentation(Category.Item, Gui.NoLocalization)
            .SetToHitBonus(0)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeForce, 1, DieType.D8)
                    .Build())
                .Build()
            )
            .AddToDB();

        var monster = MonsterDefinitionBuilder
            .Create("MonsterInnovationWeaponSteelDefender")
            .SetGuiPresentation(Category.Monster,
                CustomIcons.GetSprite("SteelDefenderMonster", Resources.SteelDefenderMonster, 160,
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
                FeatureDefinitionSenses.SenseDarkvision12,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                FeatureDefinitionPowerBuilder
                    .Create("PowerInnovationWeaponSteelDefenderRepair")
                    .SetGuiPresentation(Category.Feature,
                        CustomIcons.GetSprite("SteelDefenderRepair", Resources.SteelDefenderRepair, 
                            256, 128))
                    .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest, 1, 3)
                    // RAW this can heal any other Inventor construct, this version only heals self
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                            .SetEffectForms(EffectFormBuilder
                                .Create()
                                .SetHealingForm(HealingComputation.Dice, 4, DieType.D8, 2, false,
                                    HealingCap.MaximumHitPoints)
                                .Build())
                            .Build())
                    .AddToDB(),
                FeatureDefinitionConditionAffinityBuilder
                    .Create("FeatureInnovationWeaponSteelDefenderInitiative")
                    .SetGuiPresentationNoContent()
                    .SetConditionAffinityType(ConditionAffinityType.Immunity)
                    .SetConditionType(ConditionDefinitions.ConditionSurprised)
                    .SetCustomSubFeatures(ForceInitiativeToSummoner.Mark)
                    .AddToDB(),
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinitySteelDefenderBasic")
                    .SetGuiPresentationNoContent()
                    .SetDefaultAllowedActonTypes()
                    .SetForbiddenActions(Id.AttackMain, Id.AttackOff, Id.AttackReadied, Id.Ready, Id.Shove,
                        Id.PowerMain, Id.PowerBonus, Id.PowerReaction, Id.SpendPower)
                    .SetCustomSubFeatures(new SummonerHasConditionOrKOd())
                    .AddToDB(),
                FeatureDefinitionActionAffinitys.ActionAffinityFightingStyleProtection,
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
            .SetAlignment("Neutral")
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

    private static FeatureDefinition BuildCommandSteelDefender()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerInventorWeaponSteelDefenderCommand")
            .SetGuiPresentation(Category.Feature, Command) //TODO: make proper icon
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create(CommandSteelDefenderCondition)
                            .SetGuiPresentationNoContent()
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            //TODO: Double check duration equals 1 won't break things
                            // .SetDuration(DurationType.Round, 0, false)
                            .SetDuration(DurationType.Round, 1)
                            .SetSpecialDuration(true)
                            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
                            .AddToDB(), ConditionForm.ConditionOperation.Add)
                        .Build())
                    .Build())
            .SetCustomSubFeatures(new ShowInCombatWhenHasBlade())
            .AddToDB();
    }

    private static FeatureDefinition BuildArcaneJolt()
    {
        //TODO: make Steel defender able to trigger this power
        //TODO: bonus points if we manage to add healing part of this ability
        return FeatureDefinitionPowerBuilder
            .Create("PowerInnovationWeaponArcaneJolt")
            .SetGuiPresentation(Category.Feature,
                CustomIcons.GetSprite("InventorArcaneJolt", Resources.InventorArcaneJolt, 256, 128))
            .SetUsesAbilityBonus(ActivationTime.OnAttackHit, RechargeRate.LongRest, AttributeDefinitions.Intelligence,
                1, 0)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeForce, 2, DieType.D6)
                    .Build())
                .Build())
            .SetCustomSubFeatures(
                CountPowerUseInSpecialFeatures.Marker,
                ValidatorPowerUse.UsedLessTimesThan(1),
                PowerVisibilityModifier.Default)
            .SetShowCasting(false)
            .AddToDB();
    }

    private class SummonerHasConditionOrKOd : IDefinitionApplicationValidator
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character)
        {
            //can act freely outside of battle
            if (!ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress) { return false; }

            var summoner = character.GetMySummoner()?.RulesetCharacter;

            //shouldn't happen, but do not apply in this case
            if (summoner == null) { return false; }

            //can act if summoner is KO
            if (summoner.IsUnconscious) { return false; }

            //can act if summoner commanded
            if (summoner.HasConditionOfType(CommandSteelDefenderCondition)) { return false; }

            return true;
        }
    }

    private class ShowInCombatWhenHasBlade : IPowerUseValidity
    {
        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            if (!ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress) { return false; }

            return character.powersUsedByMe.Any(p => p.sourceDefinition.Name == SummonSteelDefenderPower);
        }
    }
}
