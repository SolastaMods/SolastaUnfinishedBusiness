using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
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
        //Paladins subclass spells based off 5e Oath of Vengeance
        var autoPreparedSpellsHatred = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsHatred")
            .SetGuiPresentation("DomainSpells", Category.Feature)
            .SetPreparedSpellGroups(
            BuildSpellGroup(3, Bane, HuntersMark),
            BuildSpellGroup(5, HoldPerson, MistyStep),
            BuildSpellGroup(9, Haste, ProtectionFromEnergy))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        //Hateful Gaze ability causing fear
        var powerHatefulGaze = FeatureDefinitionPowerBuilder
            .Create("PowerHatefulGaze")
            .SetGuiPresentation(Category.Feature, PowerSorcererHauntedSoulSpiritVisage)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                .Create()
                .SetEffectForms(
                    EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitions.ConditionFrightenedFear,
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false)
                    .Build())
                   .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                   .SetDurationData(DurationType.Round, 1)
                   .Build())
            .AddToDB();


        var powerScornfulPrayerFeature = FeatureDefinitionPowerBuilder
             .Create("PowerScornfulPrayerFeature")
             .SetGuiPresentation(ConditionEnfeebled.GuiPresentation)
             .SetUsesFixed(ActivationTime.Action)
             .AddToDB();

       var conditionScornfulPrayer = ConditionDefinitionBuilder
            .Create(ConditionCursedByBestowCurseAttackRoll, "ConditionScornfulPrayer")
            .SetGuiPresentation(Category.Condition,ConditionCursedByBestowCurseAttackRoll)
            .AddFeatures(CombatAffinityEnfeebled)
            .AddFeatures(powerScornfulPrayerFeature)
            .AddToDB();
        
        //Scornful Prayer cursing attack rolls and enfeebling the foe off a wisdom saving throw
        var powerScornfulPrayer = FeatureDefinitionPowerBuilder
            .Create("PowerScornfulPrayer")
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
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false)
                    .Build())
                .SetDurationData(DurationType.Round, 3)
                .SetTargetingData(Side.Enemy, RangeType.Distance, 10, TargetType.Individuals)
                .SetSavingThrowData(
                    false,
                    AttributeDefinitions.Wisdom,
                    true,
                    EffectDifficultyClassComputation.FixedValue,
                    AttributeDefinitions.Wisdom,
                    14)
                .Build())
            .AddToDB();

        var conditionDauntlessPursuer = ConditionDefinitionBuilder
            .Create(ConditionCarriedByWind, "ConditionDauntlessPursuer")
            .SetGuiPresentation(Category.Condition, ConditionCarriedByWind)
            .AddFeatures(FeatureDefinitionMovementAffinitys.MovementAffinityCarriedByWind)
            .AddToDB();

        //Dauntless Pursuer being a carried by the wind that only procs on successful reaction hit
        var featureDauntlessPursuer = FeatureDefinitionBuilder
            .Create("FeatureDauntlessPursuer")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new IOnAttackEffectsDauntlessPursuer(conditionDauntlessPursuer))
            .AddToDB();

        //Elavated Hate allowing at level 3 to select a favored foe
        var featureElevatedHate = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.AdditionalDamageRangerFavoredEnemyChoice,"FeatureElevatedHate")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("OathOfHatred")
            .SetGuiPresentation(Category.Subclass, SorcerousHauntedSoul)
            .AddFeaturesAtLevel(3,
            featureElevatedHate,
            powerHatefulGaze,
            powerScornfulPrayer,
            autoPreparedSpellsHatred)
            .AddFeaturesAtLevel(7, featureDauntlessPursuer)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
    .SubclassChoicePaladinSacredOaths;

    private sealed class IOnAttackEffectsDauntlessPursuer : IAfterAttackEffect
     {
        private readonly ConditionDefinition _conditionDauntlessPursuerAfterAttack;
        internal IOnAttackEffectsDauntlessPursuer(ConditionDefinition conditionDauntlessPursuerAfterAttack)
        {
            _conditionDauntlessPursuerAfterAttack = conditionDauntlessPursuerAfterAttack;
        }

        public void AfterOnAttackHit(
       GameLocationCharacter attacker,
       GameLocationCharacter defender,
       RuleDefinitions.RollOutcome outcome,
       CharacterActionParams actionParams,
       RulesetAttackMode attackMode,
       ActionModifier attackModifier)
        {
            if(outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                return;
            }
           
            if(attackMode.actionType != ActionDefinitions.ActionType.Reaction)
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
