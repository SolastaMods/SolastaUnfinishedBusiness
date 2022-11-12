using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static RuleDefinitions;
using static RuleDefinitions.RollContext;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ZappaFeats
{
    private const string FeatSavageAttackerName = "FeatSavageAttacker";
    internal const string ElvenAccuracyTag = "ElvenAccuracy";

    internal static void CheckElvenPrecisionContext(
        bool result,
        RulesetCharacter character,
        RulesetAttackMode attackMode)
    {
        if (!result || character is not RulesetCharacterHero hero || attackMode == null)
        {
            return;
        }

        foreach (var feat in hero.TrainedFeats
                     .Where(x => x.Name.Contains(ElvenAccuracyTag)))
        {
            var elvenPrecisionContext = feat.GetFirstSubFeatureOfType<ElvenPrecisionContext>();

            if (elvenPrecisionContext != null)
            {
                elvenPrecisionContext.Qualified =
                    attackMode.abilityScore is not AttributeDefinitions.Strength or AttributeDefinitions.Constitution;
            }
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
                    .Create("ModifyAttackModeForWeaponFeatDeadeye")
                    .SetGuiPresentation("FeatDeadeye", Category.Feat)
                    .SetCustomSubFeatures(new ModifyDeadeyeAttackPower())
                    .AddToDB())
            .AddToDB();

        var concentrationProvider = new StopPowerConcentrationProvider(
            "Deadeye",
            "Tooltip/&DeadeyeConcentration",
            Sprites.GetSprite("DeadeyeConcentrationIcon", Resources.DeadeyeConcentrationIcon, 64, 64));

        var conditionDeadeyeTrigger = ConditionDefinitionBuilder
            .Create("ConditionDeadeyeTrigger")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(DurationType.Permanent)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("TriggerFeatureDeadeye")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(concentrationProvider)
                    .AddToDB())
            .AddToDB();

        var powerDeadeye = FeatureDefinitionPowerBuilder
            .Create("PowerDeadeye")
            .SetGuiPresentation("FeatDeadeye", Category.Feat,
                Sprites.GetSprite("DeadeyeIcon", Resources.DeadeyeIcon, 128, 64))
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
                    .AddToDB())
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
                            .Build())
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
                    .AddToDB())
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
        var featDeadEye = BuildDeadEye();

        // Dual Weapon Defense
        var featDualWeaponDefense = FeatDefinitionBuilder
            .Create("FeatDualWeaponDefense")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierSwiftBladeBladeDance)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();

        // Elven Accuracy (Dexterity)
        var featElvenAccuracyDexterity = FeatDefinitionBuilder
            .Create("FeatElvenAccuracyDexterity")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye) // accuracy roll is handled by patches
            //.SetValidators(IsElfOrHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        // Elven Accuracy (Intelligence)
        var featElvenAccuracyIntelligence = FeatDefinitionBuilder
            .Create("FeatElvenAccuracyIntelligence")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri) // accuracy roll is handled by patches
            //.SetValidators(IsElfOrHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        // Elven Accuracy (Wisdom)
        var featElvenAccuracyWisdom = FeatDefinitionBuilder
            .Create("FeatElvenAccuracyWisdom")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike) // accuracy roll is handled by patches
            //.SetValidators(IsElfOrHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        // Elven Accuracy (Charisma)
        var featElvenAccuracyCharisma = FeatDefinitionBuilder
            .Create("FeatElvenAccuracyCharisma")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta) // accuracy roll is handled by patches
            //.SetValidators(IsElfOrHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(new ElvenPrecisionContext())
            .AddToDB();

        // Marksman
        var featMarksman = FeatDefinitionBuilder
            .Create("FeatMarksman")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(ActionAffinityMarksmanReactionShot)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();

        // Metamagic
        var attributeModifierSorcererSorceryPointsBonus2 = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierSorcererSorceryPointsBase, "AttributeModifierSorcererSorceryPointsBonus2")
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.SorceryPoints, 2)
            .AddToDB();

        var metaMagicFeats = new List<FeatDefinition>();
        var dbMetamagicOptionDefinition = DatabaseRepository.GetDatabase<MetamagicOptionDefinition>();

        metaMagicFeats.SetRange(dbMetamagicOptionDefinition
            .Select(metamagicOptionDefinition => FeatDefinitionBuilder
                .Create($"FeatAdept{metamagicOptionDefinition.Name}")
                .SetGuiPresentation(
                    Gui.Format("Feat/&FeatAdeptMetamagicTitle", metamagicOptionDefinition.FormatTitle()),
                    Gui.Format("Feat/&FeatAdeptMetamagicDescription", metamagicOptionDefinition.FormatTitle()))
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus2,
                    FeatureDefinitionBuilder
                        .Create($"CustomCodeFeatAdept{metamagicOptionDefinition.Name}")
                        .SetGuiPresentationNoContent(true)
                        .SetCustomSubFeatures(new CustomCodeFeatMetamagicAdept(metamagicOptionDefinition))
                        .AddToDB())
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .AddToDB()));

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
        // OTHER FEATS
        //

        // Savage Attacker
        var featSavageAttacker = FeatDefinitionBuilder
            .Create(FeatSavageAttackerName)
            .SetFeatures(
                FeatureDefinitionDieRollModifierBuilder
                    .Create("DieRollModifierFeatSavageAttacker")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(AttackDamageValueRoll, 1, 1, 1, "Feat/&FeatSavageAttackerReroll")
                    .AddToDB(),
                FeatureDefinitionDieRollModifierBuilder
                    .Create("DieRollModifierFeatSavageMagicAttacker")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(MagicDamageValueRoll, 1, 1, 1, "Feat/&FeatSavageAttackerReroll")
                    .AddToDB())
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Improved Critical
        var featImprovedCritical = FeatDefinitionBuilder
            .Create("FeatImprovedCritical")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierMartialChampionImprovedCritical)
            .AddToDB();

        // Superior Critical
        var featSuperiorCritical = FeatDefinitionBuilder
            .Create("FeatSuperiorCritical")
            .SetGuiPresentation(Category.Feat)
            .SetKnownFeatsPrerequisite(featImprovedCritical.Name)
            .SetFeatures(AttributeModifierMartialChampionSuperiorCritical)
            .AddToDB();

        // Tough
        var featTough = FeatDefinitionBuilder
            .Create("FeatTough")
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatTough")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                        AttributeDefinitions.HitPointBonusPerLevel, 2)
                    .AddToDB())
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        //
        // set feats to be registered in mod settings
        //

        feats.AddRange(
            featArcaneDefense,
            featArcanePrecision,
            featCharismaticDefense,
            featCharismaticPrecision,
            featDualWeaponDefense,
            featDeadEye,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom,
            featElvenAccuracyCharisma,
            featMarksman,
            featWiseDefense,
            featWisePrecision,
            featSavageAttacker,
            featTough,
            featImprovedCritical,
            featSuperiorCritical);

        feats.AddRange(metaMagicFeats);

        GroupFeats.MakeGroup("FeatGroupDefenseExpert", DefenseExpert,
            featArcaneDefense,
            featCharismaticDefense,
            featWiseDefense);

        GroupFeats.MakeGroup("FeatGroupElvenAccuracy", ElvenPrecision,
            // FeatDefinitionWithPrerequisitesBuilder
            //     .Create("FeatGroupElvenAccuracy")
            //     .SetGuiPresentation(Category.Feat)
            //     .SetValidators(IsElfOrHalfElf)
            //     .SetFeatFamily(ElvenPrecision)
            //     .AddToDB(),
            featElvenAccuracyCharisma,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom);

        GroupFeats.MakeGroup("FeatGroupPrecisionFocused", PrecisionFocused,
            featArcanePrecision,
            featCharismaticPrecision,
            featWisePrecision);

        GroupFeats.MakeGroup("FeatGroupMetamagic", null, metaMagicFeats);
    }
}

internal sealed class CustomCodeFeatMetamagicAdept : IFeatureDefinitionCustomCode
{
    // private bool MetamagicTrained { get; set; }

    private MetamagicOptionDefinition MetamagicOption { get; }

    public CustomCodeFeatMetamagicAdept(MetamagicOptionDefinition metamagicOption)
    {
        MetamagicOption = metamagicOption;
    }

    public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
    {
        if (hero.MetamagicFeatures.ContainsKey(MetamagicOption))
        {
            return;
        }

        hero.TrainMetaMagicOptions(new List<MetamagicOptionDefinition> { MetamagicOption });

        // MetamagicTrained = true;
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

        var proficiency = character.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
        var toHit = -proficiency;
        var toDamage = 2 * proficiency;

        attackMode.ToHitBonus += toHit;
        attackMode.ToHitBonusTrends.Add(new TrendInfo(toHit,
            FeatureSourceType.Power, "Deadeye", null));

        damage.BonusDamage += toDamage;
        damage.DamageBonusTrends.Add(new TrendInfo(toDamage,
            FeatureSourceType.Power, "Deadeye", null));
    }
}
