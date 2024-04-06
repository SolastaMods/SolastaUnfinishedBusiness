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
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class RaceFeats
{
    private const string ElvenPrecision = "ElvenPrecision";
    private const string FadeAway = "FadeAway";
    private const string RevenantGreatSword = "RevenantGreatSword";
    private const string SquatNimbleness = "SquatNimbleness";

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        // Dragon Wings
        var featDragonWings = FeatDefinitionWithPrerequisitesBuilder
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

        //
        // Fade Away support
        //

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

        // Fade Away (Dexterity)
        var featFadeAwayDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatFadeAwayDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                powerFeatFadeAwayInvisible)
            .SetValidators(ValidatorsFeat.IsGnome)
            .SetFeatFamily(FadeAway)
            .AddToDB();

        // Fade Away (Intelligence)
        var featFadeAwayInt = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatFadeAwayInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                powerFeatFadeAwayInvisible)
            .SetValidators(ValidatorsFeat.IsGnome)
            .SetFeatFamily(FadeAway)
            .AddToDB();

        // Elven Accuracy (Dexterity)
        var featElvenAccuracyDexterity = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyDexterity")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        // Elven Accuracy (Intelligence)
        var featElvenAccuracyIntelligence = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyIntelligence")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        // Elven Accuracy (Wisdom)
        var featElvenAccuracyWisdom = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyWisdom")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        // Elven Accuracy (Charisma)
        var featElvenAccuracyCharisma = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyCharisma")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .AddCustomSubFeatures(Behaviors.Specific.ElvenPrecision.ElvenPrecisionContext.Mark)
            .AddToDB();

        //
        // Revenant support
        //

        var validWeapon = ValidatorsWeapon.IsOfWeaponType(GreatswordType);

        var attributeModifierFeatRevenantGreatSwordArmorClass = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatRevenantGreatSwordArmorClass")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.HasGreatswordInHands)
            .AddCustomSubFeatures(
                new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, validWeapon))
            .AddToDB();

        // Revenant Great Sword (Dexterity)
        var featRevenantGreatSwordDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRevenantGreatSwordDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye, attributeModifierFeatRevenantGreatSwordArmorClass)
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(RevenantGreatSword)
            .AddToDB();

        // Revenant Great Sword (Strength)
        var featRevenantGreatSwordStr = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRevenantGreatSwordStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Einar, attributeModifierFeatRevenantGreatSwordArmorClass)
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(RevenantGreatSword)
            .AddToDB();

        //
        // Squat Nimbleness
        //
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

        //Infernal Constitution
        var featInfernalConstitution = FeatDefinitionWithPrerequisitesBuilder
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

        var featDarkElfMagic = BuildDarkElfMagic();
        var featDwarvenFortitude = BuildDwarvenFortitude();
        var featWoodElfMagic = BuildWoodElfMagic();
        var featGroupFlamesOfPhlegethos = BuildFlamesOfPhlegethos(feats);
        var featGroupOrcishFury = BuildOrcishFury(feats);
        var featGroupSecondChance = BuildSecondChance(feats);

        //
        // set feats to be registered in mod settings
        //

        feats.AddRange(
            featDarkElfMagic,
            featDragonWings,
            featDwarvenFortitude,
            featFadeAwayDex,
            featFadeAwayInt,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom,
            featElvenAccuracyCharisma,
            featRevenantGreatSwordDex,
            featRevenantGreatSwordStr,
            featSquatNimblenessDex,
            featSquatNimblenessStr,
            featInfernalConstitution,
            featWoodElfMagic);

        var featGroupsElvenAccuracy = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupElvenAccuracy",
            ElvenPrecision,
            ValidatorsFeat.IsElfOfHalfElf,
            featElvenAccuracyCharisma,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom);

        var featGroupFadeAway = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupFadeAway",
            FadeAway,
            ValidatorsFeat.IsGnome,
            featFadeAwayDex,
            featFadeAwayInt);

        var featGroupRevenantGreatSword = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupRevenantGreatSword",
            RevenantGreatSword,
            ValidatorsFeat.IsElfOfHalfElf,
            featRevenantGreatSwordDex,
            featRevenantGreatSwordStr);

        var featGroupSquatNimbleness = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupSquatNimbleness",
            SquatNimbleness,
            ValidatorsFeat.IsSmallRace,
            featSquatNimblenessDex,
            featSquatNimblenessStr);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(featGroupFadeAway);

        GroupFeats.FeatGroupTwoHandedCombat.AddFeats(featGroupRevenantGreatSword);

        GroupFeats.MakeGroup("FeatGroupRaceBound", null,
            featDarkElfMagic,
            featDragonWings,
            featDwarvenFortitude,
            featInfernalConstitution,
            featGroupsElvenAccuracy,
            featGroupFadeAway,
            featGroupFlamesOfPhlegethos,
            featGroupOrcishFury,
            featGroupRevenantGreatSword,
            featGroupSecondChance,
            featGroupSquatNimbleness,
            featWoodElfMagic);
    }

    #region Dark-Elf Magic

    private static FeatDefinitionWithPrerequisites BuildDarkElfMagic()
    {
        const string Name = "FeatDarkElfMagic";

        var powerDetectMagic = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DetectMagic")
            .SetGuiPresentation(DetectMagic.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(DetectMagic)
                    .Build())
            .AddToDB();

        var powerLevitate = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Levitate")
            .SetGuiPresentation(Levitate.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Levitate)
                    .Build())
            .AddToDB();

        var powerDispelMagic = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DispelMagic")
            .SetGuiPresentation(DispelMagic.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(DispelMagic)
                    .Build())
            .AddToDB();

        var feat = FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerDetectMagic, powerLevitate, powerDispelMagic)
            .SetValidators(ValidatorsFeat.IsDarkElfOrHalfElfDark)
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
            .FinalizeSpells()
            .AddToDB();

        //explicitly re-use druid spell list, so custom cantrips selected for druid will show here 
        spellListCantrip.SpellsByLevel[0].Spells = SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells;

        var castSpellCantrip = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, $"CastSpell{Name}")
            .SetGuiPresentationNoContent(true)
            .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
            .SetSpellList(spellListCantrip)
            .AddToDB();

        var powerLongstrider = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Longstrider")
            .SetGuiPresentation(Longstrider.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Longstrider)
                    .Build())
            .AddToDB();

        var powerPassWithoutTrace = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PassWithoutTrace")
            .SetGuiPresentation(PassWithoutTrace.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PassWithoutTrace)
                    .Build())
            .AddToDB();

        var feat = FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(castSpellCantrip, powerLongstrider, powerPassWithoutTrace)
            .SetValidators(ValidatorsFeat.IsSylvanElf)
            .AddToDB();

        return feat;
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
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
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
