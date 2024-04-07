using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class RaceFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featDarkElfMagic = BuildDarkElfMagic();
        var featDragonWings = BuildDragonWings();
        var featDwarvenFortitude = BuildDwarvenFortitude();
        var featInfernalConstitution = BuildInfernalConstitution();
        var featOrcishAggression = BuildOrcishAggression();
        var featWoodElfMagic = BuildWoodElfMagic();
        var featGroupDragonFear = BuildDragonFear(feats);
        var featGroupDragonHide = BuildDragonHide(feats);
        var featGroupsElvenAccuracy = BuildElvenAccuracy(feats);
        var featGroupFadeAway = BuildFadeAway(feats);
        var featGroupFlamesOfPhlegethos = BuildFlamesOfPhlegethos(feats);
        var featGroupOrcishFury = BuildOrcishFury(feats);
        var featGroupRevenantGreatSword = BuildRevenant(feats);
        var featGroupSecondChance = BuildSecondChance(feats);
        var featGroupSquatNimbleness = BuildSquatNimbleness(feats);

        feats.AddRange(
            featDarkElfMagic,
            featDragonWings,
            featDwarvenFortitude,
            featInfernalConstitution,
            featOrcishAggression,
            featWoodElfMagic);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(featGroupFadeAway);
        GroupFeats.FeatGroupTwoHandedCombat.AddFeats(featGroupRevenantGreatSword);
        GroupFeats.FeatGroupSkills.AddFeats(featGroupSquatNimbleness);
        GroupFeats.MakeGroup("FeatGroupRaceBound", null,
            featDarkElfMagic,
            featDragonWings,
            featDwarvenFortitude,
            featInfernalConstitution,
            featOrcishAggression,
            featWoodElfMagic,
            featGroupDragonFear,
            featGroupDragonHide,
            featGroupsElvenAccuracy,
            featGroupFadeAway,
            featGroupFlamesOfPhlegethos,
            featGroupOrcishFury,
            featGroupRevenantGreatSword,
            featGroupSecondChance,
            featGroupSquatNimbleness);
    }

    #region Dragon Wings

    private static FeatDefinitionWithPrerequisites BuildDragonWings()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatDragonWings")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionPowerBuilder
                    .Create("PowerFeatDragonWings")
                    .SetGuiPresentation("FeatDragonWings", Category.Feat,
                        Sprites.GetSprite("PowerCallForCharge", Resources.PowerCallForCharge, 256, 128))
                    .SetUsesProficiencyBonus(ActivationTime.BonusAction)
                    .AddCustomSubFeatures(new ValidatorsValidatePowerUse(ValidatorsCharacter.DoesNotHaveHeavyArmor))
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                            .SetDurationData(DurationType.Minute, 1)
                            .SetEffectForms(
                                EffectFormBuilder
                                    .Create()
                                    .SetConditionForm(
                                        ConditionDefinitions.ConditionFlying12, ConditionForm.ConditionOperation.Add)
                                    .Build())
                            .Build())
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsDragonborn)
            .AddToDB();
    }

    #endregion

    #region Fade Away

    private static FeatDefinition BuildFadeAway(List<FeatDefinition> feats)
    {
        const string FadeAway = "FadeAway";

        var powerFeatFadeAwayInvisible = FeatureDefinitionPowerBuilder
            .Create("PowerFeatFadeAwayInvisible")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ReactionTriggerContext.DamagedByAnySource)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Invisibility.EffectDescription)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .Build())
            .AddToDB();

        var featFadeAwayDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatFadeAwayDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                powerFeatFadeAwayInvisible)
            .SetValidators(ValidatorsFeat.IsGnome)
            .SetFeatFamily(FadeAway)
            .AddToDB();

        var featFadeAwayInt = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatFadeAwayInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                powerFeatFadeAwayInvisible)
            .SetValidators(ValidatorsFeat.IsGnome)
            .SetFeatFamily(FadeAway)
            .AddToDB();

        feats.AddRange(featFadeAwayDex, featFadeAwayInt);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupFadeAway",
            FadeAway,
            ValidatorsFeat.IsGnome,
            featFadeAwayDex,
            featFadeAwayInt);
    }

    #endregion

    #region Elven Accuracy

    private static FeatDefinition BuildElvenAccuracy(List<FeatDefinition> feats)
    {
        const string ElvenPrecision = "ElvenPrecision";

        var featElvenAccuracyDexterity = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyDexterity")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        var featElvenAccuracyIntelligence = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyIntelligence")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        var featElvenAccuracyWisdom = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyWisdom")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        var featElvenAccuracyCharisma = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyCharisma")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        feats.AddRange(
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom,
            featElvenAccuracyCharisma);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupElvenAccuracy",
            ElvenPrecision,
            ValidatorsFeat.IsElfOfHalfElf,
            featElvenAccuracyCharisma,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom);
    }

    #endregion

    #region Infernal Constitution

    private static FeatDefinitionWithPrerequisites BuildInfernalConstitution()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatInfernalConstitution")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Arun,
                SavingThrowAffinityAntitoxin,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityPoisonResistance)
            .SetValidators(ValidatorsFeat.IsTiefling)
            .AddToDB();
    }

    #endregion

    #region Revenant

    private static FeatDefinition BuildRevenant(List<FeatDefinition> feats)
    {
        const string RevenantGreatSword = "RevenantGreatSword";

        var validWeapon = ValidatorsWeapon.IsOfWeaponType(GreatswordType);

        var attributeModifierFeatRevenantGreatSwordArmorClass = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatRevenantGreatSwordArmorClass")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.HasGreatswordInHands)
            .AddCustomSubFeatures(
                new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, validWeapon))
            .AddToDB();

        var featRevenantGreatSwordDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRevenantGreatSwordDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye, attributeModifierFeatRevenantGreatSwordArmorClass)
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(RevenantGreatSword)
            .AddToDB();

        var featRevenantGreatSwordStr = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRevenantGreatSwordStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Einar, attributeModifierFeatRevenantGreatSwordArmorClass)
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(RevenantGreatSword)
            .AddToDB();

        feats.AddRange(featRevenantGreatSwordDex, featRevenantGreatSwordStr);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupRevenantGreatSword",
            RevenantGreatSword,
            ValidatorsFeat.IsElfOfHalfElf,
            featRevenantGreatSwordDex,
            featRevenantGreatSwordStr);
    }

    #endregion

    #region Squat Nimbleness

    private static FeatDefinition BuildSquatNimbleness(List<FeatDefinition> feats)
    {
        const string SquatNimbleness = "SquatNimbleness";

        var featSquatNimblenessDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSquatNimblenessDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                FeatureDefinitionMovementAffinitys.MovementAffinitySixLeaguesBoots,
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyFeatSquatNimblenessAcrobatics")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Acrobatics)
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsSmallRace)
            .SetFeatFamily(SquatNimbleness)
            .AddToDB();

        var featSquatNimblenessStr = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSquatNimblenessStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Einar,
                FeatureDefinitionMovementAffinitys.MovementAffinitySixLeaguesBoots,
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyFeatSquatNimblenessAthletics")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Athletics)
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsSmallRace)
            .SetFeatFamily(SquatNimbleness)
            .AddToDB();

        feats.AddRange(featSquatNimblenessStr, featSquatNimblenessDex);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupSquatNimbleness",
            SquatNimbleness,
            ValidatorsFeat.IsSmallRace,
            featSquatNimblenessDex,
            featSquatNimblenessStr);
    }

    #endregion

    #region Dark-Elf Magic

    private static FeatDefinitionWithPrerequisites BuildDarkElfMagic()
    {
        const string Name = "FeatDarkElfMagic";

        var detectMagicCantrip = SpellDefinitionBuilder
            .Create(DetectMagic, "DetectMagicCantrip")
            .SetSpellLevel(0)
            .AddToDB();

        var levitateSpell = SpellDefinitionBuilder
            .Create(Levitate, "LevitateSpell")
            .SetSpellLevel(1)
            .AddToDB();

        var dispelMagicSpell = SpellDefinitionBuilder
            .Create(DispelMagic, "DispelMagicSpell")
            .SetSpellLevel(2)
            .AddToDB();

        var spellListCantrip = SpellListDefinitionBuilder
            .Create($"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(0, detectMagicCantrip)
            .SetSpellsAtLevel(1, levitateSpell)
            .SetSpellsAtLevel(2, dispelMagicSpell)
            .FinalizeSpells(true, 1)
            .AddToDB();

        var castSpell = FeatureDefinitionCastSpellBuilder
            .Create($"CastSpell{Name}")
            .SetGuiPresentation(Name, Category.Feat)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellKnowledge(SpellKnowledge.FixedList)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetSlotsPerLevel(SharedSpellsContext.RaceCastingSlots)
            .SetReplacedSpells(1, 0)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .AddCustomSubFeatures(new OtherFeats.SpellTag("DarkElfMagic"))
            .SetSpellList(spellListCantrip)
            .AddToDB();

        var feat = FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(castSpell)
            .SetValidators(ValidatorsFeat.IsDarkElfOrHalfElfDark, ValidatorsFeat.IsLevel4)
            .AddToDB();

        return feat;
    }

    #endregion

    #region Wood-Elf Magic

    private static FeatDefinitionWithPrerequisites BuildWoodElfMagic()
    {
        const string Name = "FeatWoodElfMagic";

        var spellListCantrip = SpellListDefinitionBuilder
            .Create($"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, Longstrider)
            .SetSpellsAtLevel(2, PassWithoutTrace)
            .FinalizeSpells(true, 1)
            .AddToDB();

        //explicitly re-use druid spell list, so custom cantrips selected for druid will show here 
        spellListCantrip.SpellsByLevel[0].Spells = SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells;

        var castSpell = FeatureDefinitionCastSpellBuilder
            .Create($"CastSpell{Name}")
            .SetGuiPresentation(Name, Category.Feat)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
            .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetSlotsPerLevel(SharedSpellsContext.RaceCastingSlots)
            .SetReplacedSpells(1, 0)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .AddCustomSubFeatures(new OtherFeats.SpellTag("WoodElfMagic", true))
            .SetSpellList(spellListCantrip)
            .AddToDB();

        var pointPoolCantrip = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{Name}Cantrip")
            .SetGuiPresentationNoContent(true)
            .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1, spellListCantrip, "WoodElfMagic")
            .AddToDB();

        var feat = FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(castSpell, pointPoolCantrip)
            .SetValidators(ValidatorsFeat.IsSylvanElf, ValidatorsFeat.IsLevel4)
            .AddToDB();

        return feat;
    }

    #endregion

    #region Dragon Fear

    private static FeatDefinition BuildDragonFear(List<FeatDefinition> feats)
    {
        const string DragonFear = "DragonFear";

        var power = FeatureDefinitionPowerBuilder
            .Create("PowerFeatDragonFear")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerSorcererHauntedSoulVengefulSpirits)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Charisma, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.StartOfTurn, true)
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(Fear)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(new MagicEffectFinishedByMeAnyDragonFear(power));

        var featDragonFearStr = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatDragonFearStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(power, AttributeModifierCreed_Of_Einar)
            .SetValidators(ValidatorsFeat.IsDragonborn)
            .SetFeatFamily(DragonFear)
            .AddToDB();

        var featDragonFearCon = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatDragonFearCon")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(power, AttributeModifierCreed_Of_Arun)
            .SetValidators(ValidatorsFeat.IsDragonborn)
            .SetFeatFamily(DragonFear)
            .AddToDB();

        var featDragonFearCha = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatDragonFearCha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(power, AttributeModifierCreed_Of_Solasta)
            .SetValidators(ValidatorsFeat.IsDragonborn)
            .SetFeatFamily(DragonFear)
            .AddToDB();

        feats.AddRange(featDragonFearStr, featDragonFearCon, featDragonFearCha);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupDragonFear",
            DragonFear,
            ValidatorsFeat.IsDragonborn,
            featDragonFearStr,
            featDragonFearCon,
            featDragonFearCha);
    }

    private sealed class MagicEffectFinishedByMeAnyDragonFear(
        FeatureDefinitionPower powerDragonFear) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action.ActionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower)
            {
                yield break;
            }

            RulesetUsablePower usablePower;
            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            if (rulesetEffectPower.PowerDefinition.Name.StartsWith("PowerDragonbornBreathWeapon"))
            {
                usablePower = PowerProvider.Get(powerDragonFear, rulesetAttacker);
                rulesetAttacker.UsePower(usablePower);
            }
            else if (rulesetEffectPower.PowerDefinition == powerDragonFear)
            {
                usablePower = rulesetAttacker.UsablePowers.FirstOrDefault(x =>
                    x.PowerDefinition.Name.StartsWith("PowerDragonbornBreathWeapon"));

                if (usablePower != null)
                {
                    rulesetAttacker.UsePower(usablePower);
                }
            }
        }
    }

    #endregion

    #region Dragon Hide

    private static FeatDefinition BuildDragonHide(List<FeatDefinition> feats)
    {
        const string Name = "FeatDragonHide";

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityDragonHideToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.DragonHideToggle)
            .AddCustomSubFeatures(new ModifyWeaponAttackModeDragonHide())
            .AddToDB();

        var attributeModifier = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}")
            .SetGuiPresentation("FeatGroupDragonHide", Category.Feat)
            .SetSituationalContext(SituationalContext.NotWearingArmorOrMageArmor)
            .SetDexPlusAbilityScore(AttributeDefinitions.ArmorClass, AttributeDefinitions.Constitution)
            .AddToDB();

        var featDragonHideStr = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Str")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(actionAffinityToggle, attributeModifier, AttributeModifierCreed_Of_Einar)
            .SetValidators(ValidatorsFeat.IsDragonborn)
            .SetFeatFamily(Name)
            .AddToDB();

        var featDragonHideCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(actionAffinityToggle, attributeModifier, AttributeModifierCreed_Of_Arun)
            .SetValidators(ValidatorsFeat.IsDragonborn)
            .SetFeatFamily(Name)
            .AddToDB();

        var featDragonHideCha = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Cha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(actionAffinityToggle, attributeModifier, AttributeModifierCreed_Of_Solasta)
            .SetValidators(ValidatorsFeat.IsDragonborn)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(featDragonHideStr, featDragonHideCon, featDragonHideCha);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupDragonHide",
            Name,
            ValidatorsFeat.IsDragonborn,
            featDragonHideStr,
            featDragonHideCon,
            featDragonHideCha);
    }

    private sealed class ModifyWeaponAttackModeDragonHide : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsUnarmed(attackMode) ||
                !character.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.DragonHideToggle))
            {
                return;
            }

            var damage = attackMode?.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            damage.DamageType = DamageTypeSlashing;
        }
    }

    #endregion

    #region Dwarven Fortitude

    private static FeatDefinitionWithPrerequisites BuildDwarvenFortitude()
    {
        const string Name = "FeatDwarvenFortitude";

        var feature = FeatureDefinitionAttributeModifierBuilder
            .Create($"Feature{Name}")
            .SetGuiPresentation(Name, Category.Feat)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Constitution, 1)
            .AddToDB();

        feature.AddCustomSubFeatures(new ActionFinishedByMeDwarvenFortitude(feature));

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .AddFeatures(feature)
            .SetValidators(ValidatorsFeat.IsDwarf)
            .AddToDB();
    }

    private sealed class ActionFinishedByMeDwarvenFortitude(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition feature) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            if (characterAction.ActionId is not ActionDefinitions.Id.Dodge)
            {
                yield break;
            }

            var attacker = characterAction.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetHero = rulesetAttacker.GetOriginalHero();

            if (rulesetHero == null || rulesetHero.RemainingHitDiceCount() == 0)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
                {
                    StringParameter = "Reaction/&CustomReactionDwarvenFortitudeDescription"
                };
            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("DwarvenFortitude", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                attacker, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            EffectHelpers.StartVisualEffect(attacker, attacker, CureWounds, EffectHelpers.EffectType.Effect);
            rulesetHero.HitDieRolled += HitDieRolled;
            rulesetHero.RollHitDie();
            rulesetHero.HitDieRolled -= HitDieRolled;
        }

        private void HitDieRolled(
            RulesetCharacter character,
            DieType dieType,
            int value,
            AdvantageType advantageType,
            int roll1,
            int roll2,
            int modifier,
            bool isBonus)
        {
            const string BASE_LINE = "Feedback/&DwarvenFortitudeHitDieRolled";

            character.ShowDieRoll(
                dieType, roll1, roll2, advantage: advantageType, title: feature.GuiPresentation.Title);

            character.LogCharacterActivatesAbility(
                Gui.NoLocalization, BASE_LINE, true,
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, $"{value - modifier}+{modifier}"),
                    (ConsoleStyleDuplet.ParameterType.Positive, $"{value}")
                ]);
        }
    }

    #endregion

    #region Flames of Phlegethos

    private static FeatDefinition BuildFlamesOfPhlegethos(List<FeatDefinition> feats)
    {
        const string Name = "FeatFlamesOfPhlegethos";

        var powerRetaliate = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Retaliate")
            .SetGuiPresentation("FeatGroupFlamesOfPhlegethos", Category.Feat)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeFire, 1, DieType.D4))
                    .SetParticleEffectParameters(FireBolt)
                    .Build())
            .AddToDB();

        var damageAffinity = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetRetaliate(powerRetaliate, 1)
            .AddToDB();

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDivineFavor)
            .SetPossessive()
            .SetFeatures(damageAffinity)
            .CopyParticleReferences(ConditionDefinitions.ConditionOnFire)
            .AddToDB();

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation("FeatGroupFlamesOfPhlegethos", Category.Feat)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(condition),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 6, 6,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        power.AddCustomSubFeatures(new MagicEffectFinishedByMeAnyFlamesOfPhlegethos(power));

        var dieRollModifierFire = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}Fire")
            .SetGuiPresentation("FeatGroupFlamesOfPhlegethos", Category.Feat)
            .SetModifiers(RollContext.MagicDamageValueRoll, 1, 1, 1,
                "Feature/&DieRollModifierFeatFlamesOfPhlegethosReroll")
            .AddCustomSubFeatures(new ValidateDieRollModifierFlamesOfPhlegethos())
            .AddToDB();

        var flamesCha = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Cha")
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsTiefling)
            .SetFeatures(AttributeModifierCreed_Of_Solasta, dieRollModifierFire, power)
            .SetFeatFamily(Name)
            .AddToDB();

        var flamesInt = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Int")
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsTiefling)
            .SetFeatures(AttributeModifierCreed_Of_Pakri, dieRollModifierFire, power)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(flamesCha, flamesInt);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupFlamesOfPhlegethos", Name, ValidatorsFeat.IsTiefling, flamesCha, flamesInt);
    }

    private sealed class MagicEffectFinishedByMeAnyFlamesOfPhlegethos(FeatureDefinitionPower power)
        : IMagicEffectFinishedByMeAny
    {
        public IEnumerator OnMagicEffectFinishedByMeAny(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            if (!action.ActionParams.activeEffect.EffectDescription.EffectForms.Any(x =>
                    x.FormType == EffectForm.EffectFormType.Damage && x.DamageForm.DamageType == DamageTypeFire))
            {
                yield break;
            }

            if (ServiceRepository.GetService<IGameLocationBattleService>()
                    is not GameLocationBattleManager gameLocationBattleManager ||
                ServiceRepository.GetService<IGameLocationActionService>()
                    is not GameLocationActionManager gameLocationActionManager ||
                ServiceRepository.GetService<IRulesetImplementationService>()
                    is not RulesetImplementationManager implementationManagerService)
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(power, rulesetCharacter);
            var reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { new ActionModifier() },
                StringParameter = "PowerFeatFlamesOfPhlegethos",
                RulesetEffect = implementationManagerService
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { attacker }
            };

            var count = gameLocationActionManager.PendingReactionRequestGroups.Count;

            gameLocationActionManager.ReactToUsePower(reactionParams, "UsePower", attacker);

            yield return gameLocationBattleManager.WaitForReactions(attacker, gameLocationActionManager, count);
        }
    }

    private sealed class ValidateDieRollModifierFlamesOfPhlegethos : IValidateDieRollModifier
    {
        public bool CanModifyRoll(
            RulesetCharacter character,
            List<FeatureDefinition> features,
            List<string> damageTypes)
        {
            return damageTypes.Contains(DamageTypeFire);
        }
    }

    #endregion

    #region Orcish Aggression

    private static FeatDefinitionWithPrerequisites BuildOrcishAggression()
    {
        const string Name = "FeatOrcishAggression";

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 24, TargetType.Position)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            new CustomBehaviorOrcishAggression(power));

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsHalfOrc)
            .SetFeatures(power)
            .AddToDB();
    }

    private sealed class CustomBehaviorOrcishAggression(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerOrcishAggression)
        : IFilterTargetingPosition, IModifyEffectDescription, IMagicEffectFinishedByMe
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            cursorLocationSelectPosition.validPositionsCache.Clear();

            if (Gui.Battle == null)
            {
                yield break;
            }

            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var visibilityService =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var maxRange = actingCharacter.MaxTacticalMoves;
            var enemies = Gui.Battle.GetContenders(actingCharacter);
            var validDestinations = ServiceRepository.GetService<IGameLocationPathfindingService>()
                .ComputeValidDestinations(actingCharacter, false, maxRange);

            foreach (var position in validDestinations.Select(x => x.position))
            {
                if (!visibilityService.MyIsCellPerceivedByCharacter(position, actingCharacter) ||
                    !positioningService.CanPlaceCharacter(
                        actingCharacter, position, CellHelpers.PlacementMode.Station) ||
                    !positioningService.CanCharacterStayAtPosition_Floor(
                        actingCharacter, position, onlyCheckCellsWithRealGround: true))
                {
                    continue;
                }

                if (DistanceCalculation.GetDistanceFromPositions(position, actingCharacter.LocationPosition) > maxRange)
                {
                    continue;
                }

                foreach (var enemy in enemies)
                {
                    if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                    {
                        yield return null;
                    }

                    var currentDistance = DistanceCalculation.GetDistanceFromCharacters(actingCharacter, enemy);
                    var newDistance = DistanceCalculation.GetDistanceFromPositions(position, enemy.LocationPosition);

                    if (newDistance >= currentDistance)
                    {
                        continue;
                    }

                    cursorLocationSelectPosition.validPositionsCache.Add(position);
                }
            }
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var targetPosition = action.ActionParams.Positions[0];
            var actionParams =
                new CharacterActionParams(actingCharacter, ActionDefinitions.Id.TacticalMove)
                {
                    Positions = { targetPosition }
                };

            actingCharacter.UsedTacticalMoves = 0;
            ServiceRepository.GetService<IGameLocationActionService>()?.ExecuteAction(actionParams, null, true);

            yield break;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerOrcishAggression;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var glc = GameLocationCharacter.GetFromActor(character);

            if (glc == null)
            {
                return effectDescription;
            }

            //effectDescription.rangeParameter = glc.MaxTacticalMoves;

            return effectDescription;
        }
    }

    #endregion

    #region Orcish Fury

    private static FeatDefinition BuildOrcishFury(List<FeatDefinition> feats)
    {
        const string Name = "FeatOrcishFury";

        var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("OrcishFury")
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
            .AddToDB();

        var conditionAdditionalDamage = ConditionDefinitionBuilder
            .Create($"Condition{Name}AdditionalDamage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamage)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImpishWrath")
            .SetGuiPresentation("FeatGroupOrcishFury", Category.Feat)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .DelegatedToAction()
            .AddToDB();

        power.AddCustomSubFeatures(new CustomBehaviorOrcishFury(power, conditionAdditionalDamage));

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "OrcishFuryToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.OrcishFuryToggle)
            .SetActivatedPower(power)
            .AddToDB();

        var actionAffinityImpishWrathToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityOrcishFuryToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.OrcishFuryToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(power)))
            .AddToDB();

        var orcishFuryStr = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Str")
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsHalfOrc)
            .SetFeatures(AttributeModifierCreed_Of_Einar, actionAffinityImpishWrathToggle, power)
            .SetFeatFamily(Name)
            .AddToDB();

        var orcishFuryCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsHalfOrc)
            .SetFeatures(AttributeModifierCreed_Of_Arun, actionAffinityImpishWrathToggle, power)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(orcishFuryStr, orcishFuryCon);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupOrcishFury", Name, ValidatorsFeat.IsHalfOrc, orcishFuryStr, orcishFuryCon);
    }

    private sealed class CustomBehaviorOrcishFury(
        FeatureDefinitionPower power,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDefinition)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe,
            IPhysicalAttackBeforeHitConfirmedOnMe, IMagicEffectBeforeHitConfirmedOnMe, IActionFinishedByEnemy
    {
        private bool _knockOutPrevented;
        private bool _shouldTrigger;

        public IEnumerator OnActionFinishedByEnemy(CharacterAction characterAction, GameLocationCharacter target)
        {
            if (!_shouldTrigger)
            {
                yield break;
            }

            var rulesetTarget = target.RulesetCharacter;

            _shouldTrigger = false;
            rulesetTarget.KnockOutPrevented -= KnockOutPreventedHandler;

            if (!_knockOutPrevented)
            {
                yield break;
            }

            _knockOutPrevented = false;

            if (!target.CanReact())
            {
                yield break;
            }

            rulesetTarget.LogCharacterUsedPower(power);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var attackMode = target.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
            var attackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            attackModeCopy.Copy(attackMode);
            attackModeCopy.ActionType = ActionDefinitions.ActionType.Reaction;

            var attackActionParams =
                new CharacterActionParams(target, ActionDefinitions.Id.AttackOpportunity)
                {
                    AttackMode = attackModeCopy,
                    TargetCharacters = { characterAction.ActingCharacter },
                    ActionModifiers = { new ActionModifier() }
                };

            actionService.ExecuteAction(attackActionParams, null, true);
        }

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender.KnockOutPrevented += KnockOutPreventedHandler;
            _shouldTrigger = true;
            _knockOutPrevented = false;

            yield break;
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!ValidatorsWeapon.IsOfWeaponType(CustomSituationalContext.SimpleOrMartialWeapons)
                    (attackMode, null, null) ||
                !rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.OrcishFuryToggle) ||
                rulesetAttacker.GetRemainingPowerUses(power) == 0)
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                conditionDefinition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionDefinition.Name,
                0,
                0,
                0);
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender.KnockOutPrevented += KnockOutPreventedHandler;
            _shouldTrigger = true;
            _knockOutPrevented = false;

            yield break;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!ValidatorsWeapon.IsOfWeaponType(CustomSituationalContext.SimpleOrMartialWeapons)
                    (attackMode, null, null) ||
                !rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.OrcishFuryToggle) ||
                rulesetAttacker.GetRemainingPowerUses(power) == 0)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(power, rulesetAttacker);

            rulesetAttacker.UsePower(usablePower);
        }

        private void KnockOutPreventedHandler(RulesetCharacter character, BaseDefinition source)
        {
            _knockOutPrevented = source == DamageAffinityHalfOrcRelentlessEndurance;
        }
    }

    #endregion

    #region Second Chance

    private static FeatDefinition BuildSecondChance(List<FeatDefinition> feats)
    {
        const string Name = "FeatSecondChance";

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .AddToDB();

        var feature = FeatureDefinitionBuilder
            .Create($"Feature{Name}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        feature.AddCustomSubFeatures(new TryAlterOutcomeAttackSecondChance(feature, condition));

        var secondChanceDex = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Dex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Misaye)
            .SetValidators(ValidatorsFeat.IsHalfling)
            .SetFeatFamily(Name)
            .AddToDB();

        var secondChanceCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Arun)
            .SetValidators(ValidatorsFeat.IsHalfling)
            .SetFeatFamily(Name)
            .AddToDB();

        var secondChanceCha = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Cha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Solasta)
            .SetValidators(ValidatorsFeat.IsHalfling)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(secondChanceDex, secondChanceCon, secondChanceCha);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupSecondChance", Name, ValidatorsFeat.IsHalfling, secondChanceDex, secondChanceCon,
            secondChanceCha);
    }

    private sealed class TryAlterOutcomeAttackSecondChance(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureSecondChance,
        ConditionDefinition conditionSecondChance) : ITryAlterOutcomeAttack
    {
        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (defender != helper ||
                !defender.CanReact() ||
                !defender.CanPerceiveTarget(attacker) ||
                rulesetDefender.HasConditionOfType(conditionSecondChance))
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "Reaction/&CustomReactionSecondChanceDescription"
                };
            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("SecondChance", reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                conditionSecondChance.Name,
                DurationType.UntilAnyRest,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                conditionSecondChance.Name,
                0,
                0,
                0);

            var attackRoll = action.AttackRoll;
            var outcome = action.AttackRollOutcome;
            var rollCaption = outcome == RollOutcome.CriticalSuccess
                ? "Feedback/&RollAttackCriticalSuccessTitle"
                : "Feedback/&RollAttackSuccessTitle";

            var rulesetAttacker = attacker.RulesetCharacter;
            var attackMode = action.actionParams.attackMode;
            var activeEffect = action.ActionParams.activeEffect;

            int roll;
            int toHitBonus;
            int successDelta;

            if (attackMode != null)
            {
                toHitBonus = attackMode.ToHitBonus;
                roll = rulesetAttacker.RollAttack(
                    toHitBonus,
                    defender.RulesetCharacter,
                    attackMode.SourceDefinition,
                    attackMode.ToHitBonusTrends,
                    false,
                    attackModifier.AttackAdvantageTrends,
                    attackMode.ranged,
                    false,
                    attackModifier.AttackRollModifier,
                    out outcome,
                    out successDelta,
                    -1,
                    true);
            }
            else if (activeEffect != null)
            {
                toHitBonus = activeEffect.MagicAttackBonus;
                roll = rulesetAttacker.RollMagicAttack(
                    activeEffect,
                    defender.RulesetCharacter,
                    activeEffect.GetEffectSource(),
                    attackModifier.AttacktoHitTrends,
                    attackModifier.AttackAdvantageTrends,
                    false,
                    attackModifier.AttackRollModifier,
                    out outcome,
                    out successDelta,
                    -1,
                    true);
            }
            // should never happen
            else
            {
                yield break;
            }

            rulesetDefender.LogCharacterUsedFeature(
                featureSecondChance,
                "Feedback/&TriggerRerollLine",
                false,
                (ConsoleStyleDuplet.ParameterType.Base, $"{attackRoll}+{toHitBonus}"),
                (ConsoleStyleDuplet.ParameterType.SuccessfulRoll,
                    Gui.Format(rollCaption, $"{attackRoll + toHitBonus}")));

            action.AttackRollOutcome = outcome;
            action.AttackSuccessDelta = successDelta;
            action.AttackRoll = roll;
        }
    }

    #endregion
}
