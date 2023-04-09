using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

//Paladin Oath inspired from 5e Oath of Vengeance
internal sealed class OathOfHatred : AbstractSubclass
{
    internal OathOfHatred()
    {
        //
        // LEVEL 03
        //

        //Paladins subclass spells based off 5e Oath of Vengeance
        var autoPreparedSpellsHatred = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsHatred")
            .SetGuiPresentation("Subclass/&OathOfHatredTitle", "Feature/&DomainSpellsDescription")
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Bane, HuntersMark),
                BuildSpellGroup(5, HoldPerson, MistyStep),
                BuildSpellGroup(9, Haste, ProtectionFromEnergy),
                BuildSpellGroup(13, Banishment, DreadfulOmen))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();


        //Elevated Hate allowing at level 3 to select a favored foe
        var featureSetHatredElevatedHate = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.AdditionalDamageRangerFavoredEnemyChoice,
                "FeatureSetHatredElevatedHate")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        //Hateful Gaze ability causing fear
        var powerHatredHatefulGaze = FeatureDefinitionPowerBuilder
            .Create("PowerHatredHatefulGaze")
            .SetGuiPresentation(Category.Feature, PowerSorcererHauntedSoulSpiritVisage)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFrightenedFear,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerHatredScornfulPrayerFeature = FeatureDefinitionPowerBuilder
            .Create("PowerHatredScornfulPrayerFeature")
            .SetGuiPresentation(ConditionEnfeebled.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action)
            .AddToDB();

        var conditionScornfulPrayer = ConditionDefinitionBuilder
            .Create(ConditionCursedByBestowCurseAttackRoll, "ConditionScornfulPrayer")
            .SetGuiPresentation(Category.Condition, ConditionCursedByBestowCurseAttackRoll)
            .AddFeatures(CombatAffinityEnfeebled)
            .AddFeatures(powerHatredScornfulPrayerFeature)
            .AddToDB();

        //Scornful Prayer cursing attack rolls and enfeebling the foe off a wisdom saving throw
        var powerHatredScornfulPrayer = FeatureDefinitionPowerBuilder
            .Create("PowerHatredScornfulPrayer")
            .SetGuiPresentation(Category.Feature, PowerMartialCommanderInvigoratingShout)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionScornfulPrayer,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetDurationData(DurationType.Round, 3)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Individuals)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.FixedValue,
                        AttributeDefinitions.Wisdom,
                        14)
                    .Build())
            .AddToDB();

        //
        // LEVEL 7
        //

        var conditionDauntlessPursuer = ConditionDefinitionBuilder
            .Create(ConditionCarriedByWind, "ConditionDauntlessPursuer")
            .SetGuiPresentation(Category.Condition, ConditionCarriedByWind)
            .AddFeatures(FeatureDefinitionMovementAffinitys.MovementAffinityCarriedByWind)
            .AddToDB();

        //Dauntless Pursuer being a carried by the wind that only processes on successful reaction hit
        var featureDauntlessPursuer = FeatureDefinitionBuilder
            .Create("FeatureHatredDauntlessPursuer")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new OnAttackEffectsDauntlessPursuer(conditionDauntlessPursuer))
            .AddToDB();

        //
        // Level 15
        //

        // TODO: implement Soul of Vengeance instead
        var featureSetHatredResistance = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetHatredResistance")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("OathOfHatred")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("OathOfHatred", Resources.OathOfHatred, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpellsHatred,
                featureSetHatredElevatedHate,
                powerHatredHatefulGaze,
                powerHatredScornfulPrayer)
            .AddFeaturesAtLevel(7, featureDauntlessPursuer)
            .AddFeaturesAtLevel(15, featureSetHatredResistance)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class OnAttackEffectsDauntlessPursuer : IAfterAttackEffect
    {
        private readonly ConditionDefinition _conditionDauntlessPursuerAfterAttack;

        internal OnAttackEffectsDauntlessPursuer(ConditionDefinition conditionDauntlessPursuerAfterAttack)
        {
            _conditionDauntlessPursuerAfterAttack = conditionDauntlessPursuerAfterAttack;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                return;
            }

            if (attackMode?.actionType != ActionDefinitions.ActionType.Reaction)
            {
                return;
            }

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                attacker.RulesetCharacter.Guid,
                _conditionDauntlessPursuerAfterAttack,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                attacker.RulesetCharacter.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name
            );

            attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }
}
