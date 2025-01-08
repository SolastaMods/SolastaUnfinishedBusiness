using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
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
        var featBountifulLuck = BuildFeatBountifulLuck();
        var featDarkElfMagic = BuildDarkElfMagic();
        var featDragonWings = BuildDragonWings();
        var featDwarvenFortitude = BuildDwarvenFortitude();
        var featInfernalConstitution = BuildInfernalConstitution();
        var featWoodElfMagic = BuildWoodElfMagic();
        var featGroupDragonFear = BuildDragonFear(feats);
        var featGroupDragonHide = BuildDragonHide(feats);
        var featGroupsElvenAccuracy = BuildElvenAccuracy(feats);
        var featGroupFadeAway = BuildFadeAway(feats);
        var featGroupFlamesOfPhlegethos = BuildFlamesOfPhlegethos(feats);
        var featGroupGrudgeBearer = BuildGrudgeBearer(feats);
        var featGroupOrcishAggression = BuildOrcishAggression(feats);
        var featGroupOrcishFury = BuildOrcishFury(feats);
        var featGroupRevenantGreatSword = BuildRevenant(feats);
        var featGroupSecondChance = BuildSecondChance(feats);
        var featGroupSquatNimbleness = BuildSquatNimbleness(feats);

        feats.AddRange(
            featBountifulLuck,
            featDarkElfMagic,
            featDragonWings,
            featDwarvenFortitude,
            featInfernalConstitution,
            featWoodElfMagic);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(featGroupFadeAway);
        GroupFeats.FeatGroupTwoHandedCombat.AddFeats(featGroupRevenantGreatSword);
        GroupFeats.FeatGroupSkills.AddFeats(featGroupSquatNimbleness);
        GroupFeats.MakeGroup("FeatGroupRaceBound", null,
            featBountifulLuck,
            featDarkElfMagic,
            featDragonWings,
            featDwarvenFortitude,
            featInfernalConstitution,
            featWoodElfMagic,
            featGroupDragonFear,
            featGroupDragonHide,
            featGroupsElvenAccuracy,
            featGroupFadeAway,
            featGroupFlamesOfPhlegethos,
            featGroupGrudgeBearer,
            featGroupOrcishAggression,
            featGroupOrcishFury,
            featGroupRevenantGreatSword,
            featGroupSecondChance,
            featGroupSquatNimbleness);
    }

    #region Dragon Wings

    private static FeatDefinitionWithPrerequisites BuildDragonWings()
    {
        var condition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, "ConditionDragonWings")
            .SetGuiPresentation("FeatDragonWings", Category.Feat, ConditionDefinitions.ConditionFlying)
            .SetPossessive()
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetFeatures(FeatureDefinitionMoveModes.MoveModeFly12)
            .AddToDB();

        condition.GuiPresentation.description = Gui.EmptyContent;

        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatDragonWings")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionPowerBuilder
                    .Create("PowerFeatDragonWings")
                    .SetGuiPresentation("FeatDragonWings", Category.Feat,
                        Sprites.GetSprite("PowerCallForCharge", Resources.PowerCallForCharge, 256, 128))
                    .SetUsesProficiencyBonus(ActivationTime.BonusAction)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetDurationData(DurationType.Minute, 1)
                            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                            .SetEffectForms(EffectFormBuilder.ConditionForm(condition))
                            .Build())
                    .AddCustomSubFeatures(new ValidatorsValidatePowerUse(ValidatorsCharacter.DoesNotHaveHeavyArmor))
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
            .AddCustomSubFeatures(new FeatHelpers.SpellTag("DarkElfMagic"))
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
            .AddCustomSubFeatures(new FeatHelpers.SpellTag("WoodElfMagic", true))
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

    #region Squat Nimbleness

    private static FeatDefinition BuildSquatNimbleness(List<FeatDefinition> feats)
    {
        const string SquatNimbleness = "SquatNimbleness";

        var movementAffinitySquatNimbleness = FeatureDefinitionMovementAffinityBuilder
            .Create("MovementAffinitySquatNimbleness")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedAdditiveModifier(1)
            .AddToDB();

        var acrobaticsSkill = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatSquatNimblenessAcrobatics")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Acrobatics)
            .AddToDB();

        var featSquatNimblenessDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSquatNimblenessDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye, movementAffinitySquatNimbleness, acrobaticsSkill)
            .SetValidators(ValidatorsFeat.IsSmallRace)
            .SetFeatFamily(SquatNimbleness)
            .AddToDB();

        var athleticsSkill = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatSquatNimblenessAthletics")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Athletics)
            .AddToDB();

        var featSquatNimblenessStr = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSquatNimblenessStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Einar, movementAffinitySquatNimbleness, athleticsSkill)
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

    #region Grudge Bearer

    private static FeatDefinition BuildGrudgeBearer(List<FeatDefinition> feats)
    {
        const string Name = "FeatGrudgeBearer";

        var preferredEnemies = FeatureDefinitionFeatureSets.AdditionalDamageRangerFavoredEnemyChoice.FeatureSet;
        var preferredEnemySprites = new Dictionary<string, byte[]>
        {
            { "Aberration", Resources.PreferredEnemyAberration },
            { "Beast", Resources.PreferredEnemyBeast },
            { "Celestial", Resources.PreferredEnemyCelestial },
            { "Construct", Resources.PreferredEnemyConstruct },
            { "Dragon", Resources.PreferredEnemyDragon },
            { "Elemental", Resources.PreferredEnemyElemental },
            { "Fey", Resources.PreferredEnemyFey },
            { "Fiend", Resources.PreferredEnemyFiend },
            { "Giant", Resources.PreferredEnemyGiant },
            { "Humanoid", Resources.PreferredEnemyHumanoid },
            { "Monstrosity", Resources.PreferredEnemyMonstrosity },
            { "Ooze", Resources.PreferredEnemyOoze },
            { "Plant", Resources.PreferredEnemyPlant },
            { "Undead", Resources.PreferredEnemyUndead }
        };

        foreach (var featureDefinitionPreferredEnemy in preferredEnemies.OfType<FeatureDefinitionAdditionalDamage>())
        {
            var familyName = featureDefinitionPreferredEnemy.RequiredCharacterFamily.Name;
            var guiPresentation = featureDefinitionPreferredEnemy.RequiredCharacterFamily.GuiPresentation;
            var sprite = Sprites.GetSprite(familyName, preferredEnemySprites[familyName], 128);
            var enemyTitle = Gui.Localize($"CharacterFamily/&{familyName}Title");

            var combatAffinity = FeatureDefinitionCombatAffinityBuilder
                .Create($"CombatAffinity{Name}{familyName}")
                .SetGuiPresentation("FeatGroupGrudgeBearer", Category.Feat, Gui.NoLocalization)
                .SetAttackOnMeAdvantage(AdvantageType.Disadvantage)
                .SetOtherCharacterFamilyRestrictions(familyName)
                .AddToDB();

            combatAffinity.AddCustomSubFeatures(new ModifyAttackActionModifierGrudgeBearer(combatAffinity, familyName));

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{Name}{familyName}")
                .SetGuiPresentation(
                    Gui.Format(guiPresentation.Title, enemyTitle),
                    Gui.Format(guiPresentation.Description, enemyTitle),
                    sprite)
                .SetPoolType(InvocationPoolTypeCustom.Pools.GrudgeBearerChoice)
                .SetGrantedFeature(combatAffinity)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }

        var invocationPool = CustomInvocationPoolDefinitionBuilder
            .Create($"InvocationPool{Name}")
            .SetGuiPresentationNoContent(true)
            .Setup(InvocationPoolTypeCustom.Pools.GrudgeBearerChoice)
            .AddToDB();

        var featGrudgeBearerStr = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Str")
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsDwarf)
            .SetFeatures(AttributeModifierCreed_Of_Einar, invocationPool)
            .AddToDB();

        var featGrudgeBearerCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsDwarf)
            .SetFeatures(AttributeModifierCreed_Of_Arun, invocationPool)
            .AddToDB();

        var featGrudgeBearerWis = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Wis")
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsDwarf)
            .SetFeatures(AttributeModifierCreed_Of_Maraike, invocationPool)
            .AddToDB();

        feats.AddRange(featGrudgeBearerStr, featGrudgeBearerCon, featGrudgeBearerWis);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupGrudgeBearer", Name, ValidatorsFeat.IsDwarf,
            featGrudgeBearerStr, featGrudgeBearerCon, featGrudgeBearerWis);
    }

    private sealed class ModifyAttackActionModifierGrudgeBearer(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition,
        string familyName) : IModifyAttackActionModifier
    {
        private readonly TrendInfo _trendInfo =
            new(1, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition);

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (defender is not RulesetCharacterMonster monster ||
                monster.CharacterFamily != familyName)
            {
                return;
            }

            var battle = Gui.Battle;

            // always grant advantage on battle round zero
            if (battle == null)
            {
                attackModifier.AttackAdvantageTrends.Add(_trendInfo);

                return;
            }

            if (battle.CurrentRound > 1)
            {
                return;
            }

            // battle round one from here
            attackModifier.AttackAdvantageTrends.Add(_trendInfo);
        }
    }

    #endregion

    #region Bountiful Luck

    private static FeatDefinitionWithPrerequisites BuildFeatBountifulLuck()
    {
        const string Name = "FeatBountifulLuck";

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        FeatureDefinitionDieRollModifiers.DieRollModifierHalfingLucky.AddCustomSubFeatures(
            new ValidateDieRollModifierHalflingLucky(condition));

        var feat = FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create($"Feature{Name}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(new CustomBehaviorBountifulLuck(condition))
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsHalfling)
            .AddToDB();

        return feat;
    }

    private sealed class CustomBehaviorBountifulLuck(ConditionDefinition conditionBountifulLuck)
        : ITryAlterOutcomeAttack, ITryAlterOutcomeAttributeCheck, ITryAlterOutcomeSavingThrow
    {
        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            if (action.AttackRoll != 1 ||
                attacker == helper ||
                attacker.IsOppositeSide(helper.Side) ||
                !helper.CanReact() ||
                !helper.IsWithinRange(attacker, 6) ||
                !helper.CanPerceiveTarget(attacker))
            {
                yield break;
            }

            // any reaction within an attack flow must use the attacker as waiter
            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                attacker,
                "BountifulLuckAttack",
                "CustomReactionBountifulLuckAttackDescription".Formatted(Category.Reaction, attacker.Name,
                    defender.Name, helper.Name),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                var rulesetHelper = helper.RulesetCharacter;
                var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _,
                    out _);
                var previousRoll = action.AttackRoll;

                if (dieRoll <= action.AttackRoll)
                {
                    rulesetHelper.LogCharacterActivatesAbility(
                        "Feat/&FeatBountifulLuckyTitle",
                        "Feedback/&IsNotLuckyLower",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString()),
                            (ConsoleStyleDuplet.ParameterType.Positive, action.AttackRoll.ToString())
                        ]);

                    return;
                }

                action.AttackSuccessDelta += dieRoll - action.AttackRoll;
                action.AttackRoll = dieRoll;

                if (action.AttackSuccessDelta >= 0)
                {
                    action.AttackRollOutcome = dieRoll == 20 ? RollOutcome.CriticalSuccess : RollOutcome.Success;
                }

                rulesetHelper.InflictCondition(
                    conditionBountifulLuck.Name,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetHelper.guid,
                    rulesetHelper.CurrentFaction.Name,
                    1,
                    conditionBountifulLuck.Name,
                    0,
                    0,
                    0);

                rulesetHelper.LogCharacterActivatesAbility(
                    "Feat/&FeatBountifulLuckTitle",
                    "Feedback/&BountifulLuckAttackToHitRoll",
                    extra:
                    [
                        (dieRoll > previousRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            dieRoll.ToString()),
                        (previousRoll > dieRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            previousRoll.ToString())
                    ]);
            }
        }

        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            if (abilityCheckData.AbilityCheckRoll != 1 ||
                abilityCheckData.AbilityCheckRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure) ||
                helper == defender ||
                helper.IsOppositeSide(defender.Side) ||
                !helper.CanReact() ||
                !helper.IsWithinRange(defender, 6) ||
                !helper.CanPerceiveTarget(defender))
            {
                yield break;
            }

            // any reaction within an attribute check flow must use the yielder as waiter
            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                helper,
                "BountifulLuckCheck",
                "CustomReactionBountifulLuckCheckDescription".Formatted(Category.Reaction, defender.Name, helper.Name),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                var rulesetHelper = helper.RulesetCharacter;
                var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _,
                    out _);
                var previousRoll = abilityCheckData.AbilityCheckRoll;

                if (dieRoll <= abilityCheckData.AbilityCheckRoll)
                {
                    rulesetHelper.LogCharacterActivatesAbility(
                        "Feat/&FeatBountifulLuckyTitle",
                        "Feedback/&IsNotLuckyLower",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString()),
                            (ConsoleStyleDuplet.ParameterType.Positive, abilityCheckData.AbilityCheckRoll.ToString())
                        ]);

                    return;
                }

                abilityCheckData.AbilityCheckSuccessDelta += dieRoll - abilityCheckData.AbilityCheckRoll;
                abilityCheckData.AbilityCheckRoll = dieRoll;
                abilityCheckData.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckSuccessDelta >= 0
                    ? RollOutcome.Success
                    : RollOutcome.Failure;

                rulesetHelper.InflictCondition(
                    conditionBountifulLuck.Name,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetHelper.guid,
                    rulesetHelper.CurrentFaction.Name,
                    1,
                    conditionBountifulLuck.Name,
                    0,
                    0,
                    0);

                rulesetHelper.LogCharacterActivatesAbility(
                    "Feat/&FeatBountifulLuckTitle",
                    "Feedback/&BountifulLuckCheckToHitRoll",
                    extra:
                    [
                        (dieRoll > previousRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            dieRoll.ToString()),
                        (previousRoll > dieRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            previousRoll.ToString())
                    ]);
            }
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            SavingThrowData savingThrowData,
            bool hasHitVisual)
        {
            if (savingThrowData.SaveOutcome != RollOutcome.Failure ||
                helper == defender ||
                helper.IsOppositeSide(defender.Side) ||
                !helper.CanReact() ||
                !helper.IsWithinRange(defender, 6) ||
                !helper.CanPerceiveTarget(defender))
            {
                yield break;
            }

            var saveDC = savingThrowData.SaveDC;
            var rollModifier = savingThrowData.SaveBonusAndRollModifier;
            var savingRoll = savingThrowData.SaveOutcomeDelta - rollModifier + saveDC;

            if (savingRoll != 1)
            {
                yield break;
            }

            // any reaction within a saving flow must use the yielder as waiter
            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                helper,
                "BountifulLuckSaving",
                "CustomReactionBountifulLuckSavingDescription".Formatted(
                    Category.Reaction, defender.Name, attacker?.Name ?? ReactionRequestCustom.EnvTitle,
                    savingThrowData.Title),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                var rulesetHelper = helper.RulesetCharacter;
                var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _,
                    out _);

                if (dieRoll <= savingRoll)
                {
                    rulesetHelper.LogCharacterActivatesAbility(
                        "Feat/&FeatBountifulLuckyTitle",
                        "Feedback/&IsNotLuckyLower",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString()),
                            (ConsoleStyleDuplet.ParameterType.Positive, savingRoll.ToString())
                        ]);

                    return;
                }

                savingThrowData.SaveOutcomeDelta += dieRoll - savingRoll;
                savingThrowData.SaveOutcome =
                    savingThrowData.SaveOutcomeDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

                rulesetHelper.InflictCondition(
                    conditionBountifulLuck.Name,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetHelper.guid,
                    rulesetHelper.CurrentFaction.Name,
                    1,
                    conditionBountifulLuck.Name,
                    0,
                    0,
                    0);

                rulesetHelper.LogCharacterActivatesAbility(
                    "Feat/&FeatBountifulLuckTitle",
                    "Feedback/&BountifulLuckSavingToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString()),
                        (ConsoleStyleDuplet.ParameterType.Negative, savingRoll.ToString())
                    ]);
            }
        }
    }

    private sealed class ValidateDieRollModifierHalflingLucky(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition condition) : IValidateDieRollModifier
    {
        public bool CanModifyRoll(
            RulesetCharacter character,
            List<FeatureDefinition> features,
            List<string> damageTypes)
        {
            return !character.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, condition.Name);
        }
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
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerDragonFrightfulPresence)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(new MagicEffectFinishedByMeDragonFear(power));

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

    private sealed class MagicEffectFinishedByMeDragonFear(
        FeatureDefinitionPower powerDragonFear) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
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
                usablePower.Consume();
            }
            else if (rulesetEffectPower.PowerDefinition == powerDragonFear)
            {
                usablePower = rulesetAttacker.UsablePowers.FirstOrDefault(x =>
                    x.PowerDefinition.Name.StartsWith("PowerDragonbornBreathWeapon"));

                if (usablePower == null)
                {
                    yield break;
                }

                usablePower.Consume();
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
        public void ModifyWeaponAttackMode(
            RulesetCharacter character,
            RulesetAttackMode attackMode,
            RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            if (!ValidatorsWeapon.IsUnarmed(attackMode) ||
                !character.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.DragonHideToggle))
            {
                return;
            }

            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if ((int)damage.DieType < 3)
            {
                damage.DieType = DieType.D4;
            }

            if (damage.DiceNumber < 1)
            {
                damage.DiceNumber = 1;
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
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionDodge)
            {
                yield break;
            }

            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetHero = rulesetAttacker.GetOriginalHero();

            if (rulesetHero == null || rulesetHero.RemainingHitDiceCount() == 0)
            {
                yield break;
            }

            // any reaction within a ByMe trigger should use the acting character as waiter
            yield return attacker.MyReactToDoNothing(
                ExtraActionId.DoNothingFree,
                attacker,
                "DwarvenFortitude",
                "CustomReactionDwarvenFortitudeDescription".Localized(Category.Reaction),
                ReactionValidated);

            yield break;

            void ReactionValidated()
            {
                EffectHelpers.StartVisualEffect(attacker, attacker, CureWounds, EffectHelpers.EffectType.Effect);
                rulesetHero.HitDieRolled += HitDieRolled;
                rulesetHero.RollHitDie();
                rulesetHero.HitDieRolled -= HitDieRolled;
            }
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
            .CopyParticleReferences(ConditionDefinitions.ConditionOnFire1D4)
            .AddToDB();

        var lightSourceForm =
            Light.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation("FeatGroupFlamesOfPhlegethos", Category.Feat)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Light)
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
            .AddToDB();

        power.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new MagicEffectFinishedByMeFlamesOfPhlegethos(power));

        var dieRollModifierFire = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}")
            .SetGuiPresentationNoContent(true)
            .SetModifiers(RollContext.MagicDamageValueRoll, 1, 0, 1,
                "Feedback/&FlamesOfPhlegethosReroll")
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

    private sealed class MagicEffectFinishedByMeFlamesOfPhlegethos(FeatureDefinitionPower power)
        : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (!action.ActionParams.activeEffect.EffectDescription.EffectForms.Any(x =>
                    x.FormType == EffectForm.EffectFormType.Damage && x.DamageForm.DamageType is DamageTypeFire) ||
                action.Countered ||
                action is CharacterActionCastSpell { ExecutionFailed: true })
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(power, rulesetCharacter);

            // any reaction within a ByMe trigger should use the acting character as waiter
            yield return attacker.MyReactToUsePower(
                ActionDefinitions.Id.PowerNoCost,
                usablePower,
                [attacker],
                attacker,
                "PowerFeatFlamesOfPhlegethos");
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

    internal static FeatDefinitionWithPrerequisites FeatOrcishAggressionStr { get; private set; }
    internal static FeatDefinitionWithPrerequisites FeatOrcishAggressionCon { get; private set; }

    private static FeatDefinition BuildOrcishAggression(List<FeatDefinition> feats)
    {
        const string Name = "FeatOrcishAggression";

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerPatronTimekeeperAccelerate)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 0, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(ValidatorsCharacter.HasMeleeWeaponInMainHand, _ => Gui.Battle != null),
            new CustomBehaviorOrcishAggression(power));

        // kept name for backward compatibility
        FeatOrcishAggressionStr = FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsHalfOrc)
            .SetFeatures(AttributeModifierCreed_Of_Einar, power)
            .SetFeatFamily(Name)
            .AddToDB();

        FeatOrcishAggressionCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidatorsFeat.IsHalfOrc)
            .SetFeatures(AttributeModifierCreed_Of_Arun, power)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(FeatOrcishAggressionStr, FeatOrcishAggressionCon);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupOrcishAggression", Name, ValidatorsFeat.IsHalfOrc, FeatOrcishAggressionStr,
            FeatOrcishAggressionCon);
    }

    internal sealed class CustomBehaviorOrcishAggression(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerOrcishAggression)
        : IModifyEffectDescription, IPowerOrSpellFinishedByMe, IActionFinishedByMe, IFilterTargetingCharacter
    {
        private const string UsedTacticalMoves = "UsedTacticalMoves";
        private CharacterActionParams _actionParams;

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;

            if (action is not CharacterActionMoveStepWalk ||
                !actingCharacter.UsedSpecialFeatures.TryGetValue(UsedTacticalMoves, out var usedTacticalMoves))
            {
                yield break;
            }

            actingCharacter.UsedTacticalMoves = usedTacticalMoves;
            actingCharacter.UsedTacticalMovesChanged?.Invoke(actingCharacter);
            actingCharacter.UsedSpecialFeatures.Remove(UsedTacticalMoves);
        }

        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var service = ServiceRepository.GetService<IGameLocationBattleService>();
            var actingCharacter = __instance.ActionParams.ActingCharacter;

            if (service.CanChargeTarget(actingCharacter, target, actingCharacter.LocationPosition,
                    target.LocationPosition, actingCharacter.MaxTacticalMoves, out _))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Failure/&AllyMustBeAbleToChargeTarget");

            return false;
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

            if (glc != null)
            {
                effectDescription.rangeParameter = glc.MaxTacticalMoves;
            }

            return effectDescription;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var targetCharacter = action.ActionParams.TargetCharacters[0];

            _actionParams =
                new CharacterActionParams(actingCharacter, ActionDefinitions.Id.Charge)
                {
                    ActionModifiers = { new ActionModifier() },
                    TargetCharacters = { targetCharacter },
                    AttackMode = actingCharacter.FindActionAttackMode(ActionDefinitions.Id.AttackMain)
                };

            actingCharacter.UsedSpecialFeatures.TryAdd(UsedTacticalMoves, actingCharacter.UsedTacticalMoves);
            actingCharacter.UsedTacticalMoves = 0;
            actingCharacter.UsedTacticalMovesChanged?.Invoke(actingCharacter);
            ServiceRepository.GetService<IGameLocationActionService>().ExecuteAction(_actionParams, null, true);

            yield break;
        }

        internal static IEnumerator ExecuteImpl(CharacterActionCharge characterActionCharge)
        {
            var service = ServiceRepository.GetService<IGameLocationBattleService>();
            var origin = characterActionCharge.ActingCharacter.LocationPosition;
            var targetPosition = characterActionCharge.ActionParams.TargetCharacters[0].LocationPosition;
            var positions = new List<int3>();
            var orientation = characterActionCharge.ActingCharacter.Orientation;

            if (!service.CanChargeTarget(
                    characterActionCharge.ActingCharacter,
                    characterActionCharge.ActionParams.TargetCharacters[0],
                    origin,
                    targetPosition,
                    characterActionCharge.ActingCharacter.RemainingTacticalMoves,
                    out var destination,
                    positions))
            {
                yield break;
            }

            var path = new List<GameLocationCharacterDefinitions.PathStep>();

            path.AddRange(
                positions
                    .Where(x => x != characterActionCharge.ActingCharacter.LocationPosition)
                    .Select(x => new GameLocationCharacterDefinitions.PathStep
                    {
                        moveCost = 1, position = x, moveMode = MoveMode.Walk
                    }));

            var actionID = "Move_Charge_" + characterActionCharge.ActingCharacter.SystemName;
            var chargeActionParams = new CharacterActionParams(
                characterActionCharge.ActingCharacter,
                ActionDefinitions.Id.TacticalMove,
                ActionDefinitions.MoveStance.Charge,
                destination, orientation) { AttackMode = characterActionCharge.ActionParams.AttackMode };

            var characterActionMoveStepWalk = new CharacterActionMoveStepWalk(chargeActionParams, actionID, path);

            var attackActionParams = new CharacterActionParams(
                characterActionCharge.ActingCharacter, ActionDefinitions.Id.AttackFree,
                characterActionCharge.ActionParams.AttackMode, characterActionCharge.ActionParams.TargetCharacters[0],
                characterActionCharge.ActionParams.ActionModifiers[0]);

            var characterActionAttack = new CharacterActionAttack(attackActionParams);

            characterActionCharge.ResultingActions.Add(characterActionMoveStepWalk);
            characterActionCharge.ResultingActions.Add(characterActionAttack);
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
            .Create($"Power{Name}OrcishFury")
            .SetGuiPresentation("FeatGroupOrcishFury", Category.Feat)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .DelegatedToAction()
            .AddToDB();

        power.AddCustomSubFeatures(new CustomBehaviorOrcishFury(power, conditionAdditionalDamage));

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "OrcishFuryToggle")
            .SetOrUpdateGuiPresentation("FeatGroupOrcishFury", Category.Feat)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.OrcishFuryToggle)
            .SetActivatedPower(power)
            .OverrideClassName("Toggle")
            .AddToDB();

        var actionAffinityImpishWrathToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityOrcishFuryToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.OrcishFuryToggle)
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
        FeatureDefinitionPower powerOrcishFury,
        ConditionDefinition conditionDefinition)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackBeforeHitConfirmedOnMe,
            IMagicEffectBeforeHitConfirmedOnMe, IPhysicalAttackFinishedOnMe, IMagicEffectFinishedOnMe
    {
        #region Additional Damage

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
            var usablePower = PowerProvider.Get(powerOrcishFury, rulesetAttacker);

            if (!ValidatorsWeapon.IsOfWeaponType(CustomSituationalContext.SimpleOrMartialWeapons)
                    (attackMode, null, null) ||
                !rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.OrcishFuryToggle) ||
                rulesetAttacker.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            usablePower.Consume();
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

        #endregion

        #region Knock-Out

        private readonly string _knockOutPreventedTag = powerOrcishFury.Name + "KnockoutPrevented";

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
            attacker.UsedSpecialFeatures.TryAdd(powerOrcishFury.Name, 0);
            attacker.UsedSpecialFeatures[powerOrcishFury.Name] = 1;
            attacker.UsedSpecialFeatures.TryAdd(_knockOutPreventedTag, 0);
            attacker.UsedSpecialFeatures[_knockOutPreventedTag] = 0;

            yield break;
        }

        public IEnumerator OnMagicEffectFinishedOnMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            List<GameLocationCharacter> targets)
        {
            HandleKnockOutBehavior(action, defender);

            yield break;
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
            attacker.UsedSpecialFeatures.TryAdd(powerOrcishFury.Name, 0);
            attacker.UsedSpecialFeatures[powerOrcishFury.Name] = 1;
            attacker.UsedSpecialFeatures.TryAdd(_knockOutPreventedTag, 0);
            attacker.UsedSpecialFeatures[_knockOutPreventedTag] = 0;

            yield break;
        }

        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            HandleKnockOutBehavior(action, defender);

            yield break;
        }

        private void KnockOutPreventedHandler(RulesetCharacter rulesetCharacter, BaseDefinition source)
        {
            var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

            character.UsedSpecialFeatures.TryAdd(_knockOutPreventedTag, 0);
            character.UsedSpecialFeatures[_knockOutPreventedTag] =
                source == DamageAffinityHalfOrcRelentlessEndurance ? 1 : 0;
        }

        private void HandleKnockOutBehavior(CharacterAction action, GameLocationCharacter target)
        {
            var actingCharacter = action.ActingCharacter;

            if (!actingCharacter.UsedSpecialFeatures.TryGetValue(powerOrcishFury.Name, out var value) ||
                value == 0)
            {
                return;
            }

            actingCharacter.UsedSpecialFeatures.TryAdd(powerOrcishFury.Name, 0);

            var rulesetTarget = target.RulesetCharacter;

            rulesetTarget.KnockOutPrevented -= KnockOutPreventedHandler;

            if (!actingCharacter.UsedSpecialFeatures.TryGetValue(_knockOutPreventedTag, out var knockOutPrevented) ||
                knockOutPrevented == 0)
            {
                return;
            }

            actingCharacter.UsedSpecialFeatures.TryAdd(_knockOutPreventedTag, 0);

            if (Gui.Battle == null || !Gui.Battle.InitiativeRollFinished || !target.CanReact())
            {
                return;
            }

            rulesetTarget.LogCharacterUsedPower(powerOrcishFury);

            var attackMode = target.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            target.MyExecuteActionAttack(
                ActionDefinitions.Id.AttackOpportunity,
                action.ActingCharacter,
                attackMode,
                new ActionModifier());
        }

        #endregion
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
        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetHelper = helper.RulesetCharacter;

            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                helper != defender ||
                !defender.CanReact() ||
                !defender.CanPerceiveTarget(attacker) ||
                rulesetHelper.HasConditionOfType(conditionSecondChance))
            {
                yield break;
            }

            // any reaction within an attack flow must use the attacker as waiter
            yield return attacker.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                attacker,
                "SecondChance",
                "CustomReactionSecondChanceDescription".Localized(Category.Reaction),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                rulesetHelper.InflictCondition(
                    conditionSecondChance.Name,
                    DurationType.UntilAnyRest,
                    0,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetHelper.guid,
                    rulesetHelper.CurrentFaction.Name,
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

                int roll;
                int toHitBonus;
                int successDelta;

                if (attackMode != null)
                {
                    toHitBonus = attackMode.ToHitBonus + actionModifier.AttackRollModifier;
                    roll = rulesetAttacker.RollAttack(
                        toHitBonus,
                        defender.RulesetActor,
                        attackMode.SourceDefinition,
                        attackMode.ToHitBonusTrends,
                        false,
                        actionModifier.AttackAdvantageTrends,
                        attackMode.Ranged,
                        false,
                        actionModifier.AttackRollModifier,
                        out outcome,
                        out successDelta,
                        -1,
                        true);
                }
                else if (rulesetEffect != null)
                {
                    toHitBonus = rulesetEffect.MagicAttackBonus + actionModifier.AttackRollModifier;
                    roll = rulesetAttacker.RollMagicAttack(
                        rulesetEffect,
                        defender.RulesetActor,
                        rulesetEffect.GetEffectSource(),
                        actionModifier.AttacktoHitTrends,
                        actionModifier.AttackAdvantageTrends,
                        false,
                        actionModifier.AttackRollModifier,
                        out outcome,
                        out successDelta,
                        -1,
                        true);
                }
                // should never happen
                else
                {
                    return;
                }

                var sign = toHitBonus > 0 ? "+" : string.Empty;

                rulesetHelper.LogCharacterUsedFeature(
                    featureSecondChance,
                    "Feedback/&TriggerRerollLine",
                    false,
                    (ConsoleStyleDuplet.ParameterType.Base, $"{attackRoll}{sign}{toHitBonus}"),
                    (ConsoleStyleDuplet.ParameterType.FailedRoll,
                        Gui.Format(rollCaption, $"{attackRoll + toHitBonus}")));

                action.AttackRollOutcome = outcome;
                action.AttackSuccessDelta = successDelta;
                action.AttackRoll = roll;
            }
        }
    }

    #endregion
}
