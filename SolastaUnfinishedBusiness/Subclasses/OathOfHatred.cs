using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
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
[UsedImplicitly]
public sealed class OathOfHatred : AbstractSubclass
{
    public OathOfHatred()
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
                BuildSpellGroup(13, Banishment, DreadfulOmen),
                BuildSpellGroup(17, HoldMonster, DispelEvilAndGood))
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
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
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
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
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
            .AddCustomSubFeatures(new PhysicalAttackFinishedByMeDauntlessPursuer(conditionDauntlessPursuer))
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

        //
        // Level 20
        //

        var savingThrowAffinityHatredArdentHate = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("SavingThrowAffinityHatredArdentHate")
            .SetGuiPresentation("PowerHatredArdentHate", Category.Feature)
            .SetAffinities(CharacterSavingThrowAffinity.Advantage, false,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var conditionHatredArdentHate = ConditionDefinitionBuilder
            .Create("ConditionHatredArdentHate")
            .SetGuiPresentation(Category.Condition, ConditionDispellingEvilAndGood)
            .SetPossessive()
            .AddFeatures(savingThrowAffinityHatredArdentHate)
            .AddToDB();

        var powerHatredArdentHate = FeatureDefinitionPowerBuilder
            .Create("PowerHatredArdentHate")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerArdentHate", Resources.PowerArdentHate, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetParticleEffectParameters(PowerFighterActionSurge)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionHatredArdentHate, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        conditionHatredArdentHate.AddCustomSubFeatures(new CustomBehaviorArdentHate(powerHatredArdentHate));

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
            .AddFeaturesAtLevel(20, powerHatredArdentHate)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Paladin;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class PhysicalAttackFinishedByMeDauntlessPursuer : IPhysicalAttackFinishedByMe
    {
        private readonly ConditionDefinition _conditionDauntlessPursuerAfterAttack;

        internal PhysicalAttackFinishedByMeDauntlessPursuer(ConditionDefinition conditionDauntlessPursuerAfterAttack)
        {
            _conditionDauntlessPursuerAfterAttack = conditionDauntlessPursuerAfterAttack;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome outcome,
            int damageAmount)
        {
            if (outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            if (attackMode?.actionType != ActionDefinitions.ActionType.Reaction)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                _conditionDauntlessPursuerAfterAttack.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                _conditionDauntlessPursuerAfterAttack.Name,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorArdentHate(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower power)
        : IModifyDamageAffinity, ITryAlterOutcomePhysicalAttack
    {
        public void ModifyDamageAffinity(RulesetActor defender, RulesetActor attacker, List<FeatureDefinition> features)
        {
            features.RemoveAll(x =>
                x is IDamageAffinityProvider { DamageAffinityType: DamageAffinityType.Resistance });
        }

        public IEnumerator OnAttackTryAlterOutcome(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter me,
            GameLocationCharacter target,
            ActionModifier attackModifier)
        {
            var rulesetAttacker = me.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
                !me.OncePerTurnIsValid(power.Name) || !me.CanPerceiveTarget(target))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var guiMe = new GuiCharacter(me);
            var guiTarget = new GuiCharacter(target);

            var reactionParams = new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter = Gui.Format("Reaction/&CustomReactionHatredArdentHateDescription",
                    guiMe.Name,
                    guiTarget.Name)
            };
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("HatredArdentHate", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            me.UsedSpecialFeatures.TryAdd(power.Name, 1);
            action.AttackRoll += -action.AttackSuccessDelta;
            action.AttackSuccessDelta = 0;
            action.AttackRollOutcome = RollOutcome.Success;
        }
    }
}
