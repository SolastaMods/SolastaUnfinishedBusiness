using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using static SolastaUnfinishedBusiness.CustomBehaviors.ValidatorsFeat;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MetamagicOptionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ZappaFeats
{
    internal const string ElvenAccuracyTag = "ElvenAccuracy";

    internal static void CheckElvenPrecisionContext(bool result, RulesetCharacter character,
        RulesetAttackMode attackMode)
    {
        if (!result || character is not RulesetCharacterHero hero || attackMode == null)
        {
            return;
        }

        foreach (var elvenPrecisionContext in from feat in hero.TrainedFeats
                 where feat.Name.Contains(ElvenAccuracyTag)
                 select feat.GetFirstSubFeatureOfType<ElvenPrecisionContext>()
                 into context
                 where context != null
                 select context)
        {
            elvenPrecisionContext.Qualified =
                attackMode.abilityScore is not AttributeDefinitions.Strength or AttributeDefinitions.Constitution;
        }
    }

    private static FeatDefinition BuildDeadEye()
    {
        var conditionDeadeye = ConditionDefinitionBuilder
            .Create("ConditionDeadeye")
            .SetGuiPresentation("FeatDeadeye", Category.Feat)
            .SetDuration(DurationType.Round, 1)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("AttackModifierDeadeye")
                    .SetGuiPresentation("FeatDeadeye", Category.Feat)
                    .SetCustomSubFeatures(new ModifyDeadeyeAttackPower())
                    .AddToDB())
            .AddToDB();

        var concentrationProvider = new StopPowerConcentrationProvider(
            "Deadeye",
            "Tooltip/&DeadeyeConcentration",
            CustomIcons.GetSprite("DeadeyeConcentrationIcon",
                Resources.DeadeyeConcentrationIcon, 64, 64));

        var conditionDeadeyeTrigger = ConditionDefinitionBuilder
            .Create("ConditionDeadeyeTrigger")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(DurationType.Permanent)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("DeadeyeTriggerFeature")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(concentrationProvider)
                    .AddToDB())
            .AddToDB();

        var powerDeadeye = FeatureDefinitionPowerBuilder
            .Create("PowerDeadeye")
            .SetGuiPresentation("FeatDeadeye", Category.Feat,
                CustomIcons.GetSprite("DeadeyeIcon", Resources.DeadeyeIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeyeTrigger, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeye, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerDeadeye);

        var powerTurnOffDeadeye = FeatureDefinitionPowerBuilder
            .Create("PowerTurnOffDeadeye")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeyeTrigger, ConditionForm.ConditionOperation.Remove)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeye, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerTurnOffDeadeye);
        concentrationProvider.StopPower = powerTurnOffDeadeye;

        return FeatDefinitionBuilder
            .Create("FeatDeadeye")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerDeadeye,
                powerTurnOffDeadeye,
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityDeadeyeIgnoreDefender")
                    .SetGuiPresentation("FeatDeadeye", Category.Feat)
                    .SetIgnoreCover()
                    .SetCustomSubFeatures(new BumpWeaponAttackRangeToMax(ValidatorsWeapon.AlwaysValid))
                    .AddToDB())
            .AddToDB();
    }

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        const string DefenseExpert = "DefenseExpert";
        const string ElvenPrecision = "ElvenPrecision";
        const string PrecisionFocused = "PrecisionFocused";

        // Arcane Defense
        var featArcaneDefense = FeatDefinitionBuilder
            .Create("FeatArcaneDefense")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatArcaneDefenseAdd")
                    .SetGuiPresentationNoContent()
                    .SetSituationalContext(SituationalContext.NotWearingArmorOrMageArmor)
                    .SetDexPlusAbilityScore(AttributeDefinitions.ArmorClass, AttributeDefinitions.Intelligence)
                    .AddToDB()
            )
            .SetAbilityScorePrerequisite(AttributeDefinitions.Intelligence, 13)
            .SetFeatFamily(DefenseExpert)
            .AddToDB();

        // Arcane Precision
        var attackModifierArcanePrecision = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierArcanePrecision")
            .SetGuiPresentation("FeatArcanePrecision", Category.Feat, AttackModifierMagicWeapon)
            .SetAbilityScoreReplacement(AbilityScoreReplacement.SpellcastingAbility)
            .SetAdditionalAttackTag(TagsDefinitions.Magical)
            .AddToDB();

        var powerArcanePrecision = FeatureDefinitionPowerBuilder
            .Create("PowerArcanePrecision")
            .SetGuiPresentation("FeatArcanePrecision", Category.Feat, PowerDomainElementalLightningBlade)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 1 /* range */, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Weapon)
                    .SetCreatedByCharacter()
                    .SetDurationData(DurationType.Minute, 1 /* duration */)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 0,
                                new FeatureUnlockByLevel(attackModifierArcanePrecision, 0))
                            .Build()
                    )
                    .Build())
            .AddToDB();

        var featArcanePrecision = FeatDefinitionBuilder
            .Create("FeatArcanePrecision")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                powerArcanePrecision)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Intelligence, 13)
            .SetFeatFamily(PrecisionFocused)
            .AddToDB();

        // // Brutal Thug
        // var brutalThug =
        //     FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
        //         .Create("FeatBrutalThug")
        //         .SetFeatures(
        //             AdditionalDamageRoguishHoodlumNonFinesseSneakAttack,
        //             ProficiencyFighterWeapon
        //         )
        //         .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
        //         .SetGuiPresentation(Category.Feat)
        //         .SetValidators(ValidateMinCharLevel(4), ValidateHasStealthAttack)
        //         .AddToDB();

        // Charismatic Defense
        var featCharismaticDefense = FeatDefinitionBuilder
            .Create("FeatCharismaticDefense")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Solasta,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatCharismaticDefenseAdd")
                    .SetGuiPresentationNoContent()
                    .SetSituationalContext(SituationalContext.NotWearingArmorOrMageArmor)
                    .SetDexPlusAbilityScore(AttributeDefinitions.ArmorClass, AttributeDefinitions.Charisma)
                    .AddToDB()
            )
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .SetFeatFamily(DefenseExpert)
            .AddToDB();

        // Charismatic Precision
        var attackModifierCharismaticPrecision = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierCharismaticPrecision")
            .SetGuiPresentation("FeatCharismaticPrecision", Category.Feat, AttackModifierMagicWeapon)
            .SetAbilityScoreReplacement(AbilityScoreReplacement.SpellcastingAbility)
            .SetAdditionalAttackTag(TagsDefinitions.Magical)
            .AddToDB();

        var powerCharismaticPrecision = FeatureDefinitionPowerBuilder
            .Create("PowerCharismaticPrecision")
            .SetGuiPresentation("FeatCharismaticPrecision", Category.Feat, PowerDomainElementalLightningBlade)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Weapon)
                    .SetCreatedByCharacter()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 0,
                                new FeatureUnlockByLevel(attackModifierCharismaticPrecision, 0))
                            .Build())
                    .Build())
            .AddToDB();

        var featCharismaticPrecision = FeatDefinitionBuilder
            .Create("FeatCharismaticPrecision")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Solasta,
                powerCharismaticPrecision)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .SetFeatFamily(PrecisionFocused)
            .AddToDB();

        // Dead Eye
        var deadEye = BuildDeadEye();

        // Dual Weapon Defense
        var featDualWeaponDefense =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatDualWeaponDefense")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(AttributeModifierSwiftBladeBladeDance)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .AddToDB();

        // Elven Accuracy (Dexterity)
        var featElvenAccuracyDexterity =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatElvenAccuracyDexterity")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(AttributeModifierCreed_Of_Misaye) // accuracy roll is handled by patches
                .SetValidators(IsElfOrHalfElf)
                .SetFeatFamily(ElvenPrecision)
                .SetCustomSubFeatures(new ElvenPrecisionContext())
                .AddToDB();

        // Elven Accuracy (Intelligence)
        var featElvenAccuracyIntelligence =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatElvenAccuracyIntelligence")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(AttributeModifierCreed_Of_Pakri) // accuracy roll is handled by patches
                .SetValidators(IsElfOrHalfElf)
                .SetFeatFamily(ElvenPrecision)
                .SetCustomSubFeatures(new ElvenPrecisionContext())
                .AddToDB();

        // Elven Accuracy (Wisdom)
        var featElvenAccuracyWisdom =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatElvenAccuracyWisdom")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(AttributeModifierCreed_Of_Maraike) // accuracy roll is handled by patches
                .SetValidators(IsElfOrHalfElf)
                .SetFeatFamily(ElvenPrecision)
                .SetCustomSubFeatures(new ElvenPrecisionContext())
                .AddToDB();

        // Elven Accuracy (Charisma)
        var featElvenAccuracyCharisma =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatElvenAccuracyCharisma")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(AttributeModifierCreed_Of_Solasta) // accuracy roll is handled by patches
                .SetValidators(IsElfOrHalfElf)
                .SetFeatFamily(ElvenPrecision)
                .SetCustomSubFeatures(new ElvenPrecisionContext())
                .AddToDB();

        // Fast Hands
        // var fastHands =
        //     FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
        //         .Create("FeatFastHands")
        //         .SetFeatures(
        //             ActionAffinityRogueCunningAction,
        //             ActionAffinityThiefFastHands
        //         )
        //         .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
        //         .SetGuiPresentation(Category.Feat)
        //         .SetValidators(ValidateMinCharLevel(4), ValidateNotClass(Rogue))
        //         .AddToDB();

        // Fighting Surge (Dexterity)
        // var fightingSurgeDexterity =
        //     FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
        //         .Create("FeatFightingSurgeDexterity")
        //         .SetFeatures(
        //             AttributeModifierCreed_Of_Misaye,
        //             PowerFighterActionSurge
        //         )
        //         .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
        //         .SetGuiPresentation(Category.Feat)
        //         .SetValidators(ValidateNotClass(Fighter))
        //         .AddToDB();

        // Fighting Surge (Strength)
        // var fightingSurgeStrength =
        //     FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
        //         .Create("FeatFightingSurgeStrength")
        //         .SetFeatures(
        //             AttributeModifierCreed_Of_Einar,
        //             PowerFighterActionSurge
        //         )
        //         .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
        //         .SetGuiPresentation(Category.Feat)
        //         .SetValidators(ValidateNotClass(Fighter))
        //         .AddToDB();

        // Marksman
        var featMarksman =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMarksman")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(ActionAffinityMarksmanReactionShot)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .AddToDB();

        // Metamagic Sorcery Points Feature
        var attributeModifierSorcererSorceryPointsBonus2 = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierSorcererSorceryPointsBase, "AttributeModifierSorcererSorceryPointsBonus2")
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.SorceryPoints, 2)
            .AddToDB();

        // Metamagic Adept (Careful)
        var featMetamagicAdeptCareful =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptCareful")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnCareful,
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus2)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Distant)
        var featMetamagicAdeptDistant =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptDistant")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnDistant,
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus2)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Empowered)
        var featMetamagicAdeptEmpowered =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptEmpowered")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnEmpowered,
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus2)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Extended)
        var featMetamagicAdeptExtended =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptExtended")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnExtended,
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus2)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Heightened)
        var featMetamagicAdeptHeightened =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptHeightened")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnHeightened,
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus2,
                    attributeModifierSorcererSorceryPointsBonus2) // not a dup. adding 4 points
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Quickened)
        var featMetamagicAdeptQuickened =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptQuickened")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnQuickened,
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus2)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Twinned)
        var featMetamagicAdeptTwinned =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptTwinned")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnTwinned,
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus2)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Primal (Constitution)
        // var primalConstitution =
        //     FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
        //         .Create("FeatPrimalConstitution")
        //         .SetFeatures(
        //             AttributeModifierCreed_Of_Arun,
        //             ActionAffinityBarbarianRage,
        //             AttributeModifierBarbarianRagePointsAdd,
        //             AttributeModifierBarbarianRageDamageAdd,
        //             AttributeModifierBarbarianRageDamageAdd, // not a dup. I use add to allow compatibility with Barb class. 2 adds for +2 damage
        //             PowerBarbarianRageStart,
        //             AttributeModifierBarbarianUnarmoredDefense
        //         )
        //         .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
        //         .SetGuiPresentation(Category.Feat)
        //         .SetValidators(ValidateNotClass(Barbarian))
        //         .AddToDB();

        // Primal (Strength)
        // var primalStrength =
        //     FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
        //         .Create("FeatPrimalStrength")
        //         .SetFeatures(
        //             AttributeModifierCreed_Of_Einar,
        //             ActionAffinityBarbarianRage,
        //             AttributeModifierBarbarianRagePointsAdd,
        //             AttributeModifierBarbarianRageDamageAdd, // not a dup. I use add to allow compatibility with Barb class. 2 adds for +2 damage
        //             AttributeModifierBarbarianRageDamageAdd,
        //             PowerBarbarianRageStart,
        //             AttributeModifierBarbarianUnarmoredDefense
        //         )
        //         .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
        //         .SetGuiPresentation(Category.Feat)
        //         .SetValidators(ValidateNotClass(Barbarian))
        //         .AddToDB();

        // Shady
        // var shady = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
        //     .Create("FeatShady")
        //     .SetFeatures(
        //         AttributeModifierCreed_Of_Misaye,
        //         FeatureDefinitionAdditionalDamageBuilder
        //             .Create(AdditionalDamageRogueSneakAttack, "AdditionalDamageFeatShadySneakAttack")
        //             .SetGuiPresentation(Category.Feature)
        //             .SetDamageDice(DieType.D6, 1)
        //             .SetAdvancement(
        //                 (AdditionalDamageAdvancement)ExtraAdditionalDamageAdvancement.CharacterLevel,
        //                 (1, 0),
        //                 (2, 0),
        //                 (3, 0),
        //                 (4, 1),
        //                 (5, 1),
        //                 (6, 1),
        //                 (7, 1),
        //                 (8, 1),
        //                 (9, 1),
        //                 (10, 1),
        //                 (11, 1),
        //                 (12, 2),
        //                 (13, 2),
        //                 (14, 2),
        //                 (15, 2),
        //                 (16, 2),
        //                 (17, 2),
        //                 (18, 2),
        //                 (19, 2),
        //                 (20, 4)
        //             )
        //             .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
        //             .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
        //             .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
        //             .AddToDB()
        //     )
        //     .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
        //     .SetGuiPresentation(Category.Feat)
        //     .SetValidators(ValidateNotClass(Rogue))
        //     .AddToDB();

        // Wise Defense
        var featWiseDefense = FeatDefinitionBuilder
            .Create("FeatWiseDefense")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike,
                AttributeModifierMonkUnarmoredDefense)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Wisdom, 13)
            .SetFeatFamily(DefenseExpert)
            .AddToDB();

        // Wise Precision
        var attackModifierWisePrecision = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierWisePrecision")
            .SetGuiPresentation("FeatWisePrecision", Category.Feat, AttackModifierMagicWeapon)
            .SetAbilityScoreReplacement(AbilityScoreReplacement.SpellcastingAbility)
            .SetAdditionalAttackTag(TagsDefinitions.Magical)
            .AddToDB();

        var powerWisePrecision = FeatureDefinitionPowerBuilder
            .Create("PowerWisePrecision")
            .SetGuiPresentation("FeatWisePrecision", Category.Feat, PowerDomainElementalLightningBlade)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Weapon)
                    .SetCreatedByCharacter()
                    .SetDurationData(DurationType.Minute, 1 /* duration */)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 0,
                                new FeatureUnlockByLevel(attackModifierWisePrecision, 0))
                            .Build()
                    )
                    .Build())
            .AddToDB();

        var featWisePrecision = FeatDefinitionBuilder
            .Create("FeatWisePrecision")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike,
                powerWisePrecision)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Wisdom, 13)
            .SetFeatFamily(PrecisionFocused)
            .AddToDB();

        //
        // set feats to be registered in mod settings
        //

        feats.AddRange(
            featArcaneDefense,
            featArcanePrecision,
            // brutalThug,
            featCharismaticDefense,
            featCharismaticPrecision,
            featDualWeaponDefense,
            deadEye,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom,
            featElvenAccuracyCharisma,
            // fastHands,
            // fightingSurgeDexterity,
            // fightingSurgeStrength,
            featMarksman,
            featMetamagicAdeptCareful,
            featMetamagicAdeptDistant,
            featMetamagicAdeptEmpowered,
            featMetamagicAdeptExtended,
            featMetamagicAdeptHeightened,
            featMetamagicAdeptQuickened,
            featMetamagicAdeptTwinned,
            // primalConstitution,
            // primalStrength,
            // shady,
            featWiseDefense,
            featWisePrecision);

        GroupFeats.MakeGroup("FeatGroupDefenseExpert", DefenseExpert,
            featArcaneDefense,
            featCharismaticDefense,
            featWiseDefense);

        GroupFeats.MakeGroup(FeatDefinitionWithPrerequisitesBuilder
                .Create("FeatGroupElvenAccuracy")
                .SetGuiPresentation(Category.Feat)
                .SetValidators(IsElfOrHalfElf)
                .SetFeatFamily(ElvenPrecision)
                .AddToDB(),
            featElvenAccuracyCharisma,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom);

        GroupFeats.MakeGroup("FeatGroupPrecisionFocused", PrecisionFocused,
            featArcanePrecision,
            featCharismaticPrecision,
            featWisePrecision);

        GroupFeats.MakeGroup("FeatGroupMetamagic", null,
            featMetamagicAdeptCareful,
            featMetamagicAdeptDistant,
            featMetamagicAdeptEmpowered,
            featMetamagicAdeptExtended,
            featMetamagicAdeptHeightened,
            featMetamagicAdeptQuickened,
            featMetamagicAdeptTwinned);
    }
}

internal sealed class FeatureDefinitionMetamagicOptionBuilder : DefinitionBuilder<
    FeatureDefinitionMetamagicOption, FeatureDefinitionMetamagicOptionBuilder>
{
    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnCareful =
        CreateAndAddToDB(MetamagicCarefullSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnDistant =
        CreateAndAddToDB(MetamagicDistantSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnEmpowered =
        CreateAndAddToDB(MetamagicEmpoweredSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnExtended =
        CreateAndAddToDB(MetamagicExtendedSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnHeightened =
        CreateAndAddToDB(MetamagicHeightenedSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnQuickened =
        CreateAndAddToDB(MetamagicQuickenedSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnTwinned =
        CreateAndAddToDB(MetamagicTwinnedSpell);

    private FeatureDefinitionMetamagicOptionBuilder(MetamagicOptionDefinition metamagicOption)
        : base($"MetamagicLearn{metamagicOption.Name}", CeNamespaceGuid)
    {
        Definition.MetamagicOption = metamagicOption;
    }

    private static FeatureDefinitionMetamagicOption CreateAndAddToDB(MetamagicOptionDefinition metamagicOption)
    {
        return new FeatureDefinitionMetamagicOptionBuilder(metamagicOption)
            .SetGuiPresentationNoContent()
            .AddToDB();
    }
}

internal sealed class FeatureDefinitionMetamagicOption : FeatureDefinition, IFeatureDefinitionCustomCode
{
    private bool MetamagicTrained { get; set; }

    internal MetamagicOptionDefinition MetamagicOption { get; set; }

    public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
    {
        if (hero.MetamagicFeatures.ContainsKey(MetamagicOption))
        {
            return;
        }

        hero.TrainMetaMagicOptions(new List<MetamagicOptionDefinition> { MetamagicOption });

        MetamagicTrained = true;
    }

#if false
    public void RemoveFeature(RulesetCharacterHero hero, string tag)
    {
        if (!MetamagicTrained)
        {
            return;
        }

        hero.MetamagicFeatures.Remove(MetamagicOption);

        MetamagicTrained = false;
    }
#endif
}

internal sealed class ElvenPrecisionContext
{
    internal bool Qualified { get; set; }
}

internal sealed class ModifyDeadeyeAttackPower : IModifyAttackModeForWeapon
{
    public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
    {
        var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

        if (damage == null)
        {
            return;
        }

        if (attackMode is not { Reach: false, Ranged: true })
        {
            return;
        }

        const int TO_HIT = -5;
        const int TO_DAMAGE = 10;

        attackMode.ToHitBonus += TO_HIT;
        attackMode.ToHitBonusTrends.Add(new TrendInfo(TO_HIT,
            FeatureSourceType.Power, "Deadeye", null));

        damage.BonusDamage += TO_DAMAGE;
        damage.DamageBonusTrends.Add(new TrendInfo(TO_DAMAGE,
            FeatureSourceType.Power, "Deadeye", null));
    }
}
