using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class OathOfDread : AbstractSubclass
{
    private const string Name = "OathOfDread";

    internal const string ConditionAspectOfDreadName = $"Condition{Name}AspectOfDread";

    internal static readonly ConditionDefinition ConditionAspectOfDreadEnemy = ConditionDefinitionBuilder
        .Create($"Condition{Name}AspectOfDreadEnemy")
        .SetGuiPresentation(ConditionAspectOfDreadName, Category.Condition)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetPossessive()
        .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
        .SetFeatures(
            FeatureDefinitionSavingThrowAffinityBuilder
                .Create($"SavingThrowAffinity{Name}AspectOfDreadEnemy")
                .SetGuiPresentation(ConditionAspectOfDreadName, Category.Condition, Gui.NoLocalization)
                .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                    AttributeDefinitions.Strength,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Charisma)
                .AddToDB())
        .AddToDB();

    public OathOfDread()
    {
        //
        // LEVEL 03
        //

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Name, Category.Subclass, "Feature/&DomainSpellsDescription")
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Bane, ShieldOfFaith),
                BuildSpellGroup(5, HoldPerson, SeeInvisibility),
                BuildSpellGroup(9, Fear, Haste),
                BuildSpellGroup(13, GuardianOfFaith, PhantasmalKiller),
                BuildSpellGroup(17, DominatePerson, HoldMonster))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        // Mark of Submission

        const string MARK_OF_SUBMISSION = "MarkOfTheSubmission";

        var conditionMarkOfTheSubmission = ConditionDefinitionBuilder
            .Create($"Condition{Name}{MARK_OF_SUBMISSION}")
            .SetGuiPresentation(Category.Condition, ConditionMarkedByBrandingSmite)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .AddToDB();

        var combatAffinityMarkOfTheSubmission = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}{MARK_OF_SUBMISSION}")
            .SetGuiPresentation($"Condition{Name}{MARK_OF_SUBMISSION}", Category.Condition, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(SituationalContext.TargetHasCondition, conditionMarkOfTheSubmission)
            .AddToDB();

        var powerMarkOfTheSubmission = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{MARK_OF_SUBMISSION}")
            .SetGuiPresentation($"FeatureSet{Name}{MARK_OF_SUBMISSION}", Category.Feature,
                Sprites.GetSprite(MARK_OF_SUBMISSION, Resources.PowerMarkOfTheSubmission, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 2, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(BestowCurseOnAttackRoll)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionMarkOfTheSubmission,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var featureSetMarkOfTheSubmission = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{MARK_OF_SUBMISSION}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(combatAffinityMarkOfTheSubmission, powerMarkOfTheSubmission)
            .AddToDB();

        // Dreadful Presence

        const string DREADFUL_PRESENCE = "DreadfulPresence";

        var powerDreadfulPresence = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{DREADFUL_PRESENCE}")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite(DREADFUL_PRESENCE, Resources.PowerDreadfulPresence, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(PowerFighterActionSurge)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 07

        // Aura of Domination

        const string AURA_DOMINATION = "AuraOfDomination";

        var featureAuraOfDomination = FeatureDefinitionBuilder
            .Create($"Feature{Name}{AURA_DOMINATION}")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var conditionAuraOfDomination = ConditionDefinitionBuilder
            .Create($"Condition{Name}{AURA_DOMINATION}")
            .SetGuiPresentation(Category.Condition, ConditionBaned)
            .SetConditionType(ConditionType.Detrimental)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive()
            .SetFeatures(featureAuraOfDomination)
            .SetConditionParticleReference(ConditionBaned.conditionParticleReference)
            .AddToDB();

        featureAuraOfDomination.AddCustomSubFeatures(
            new CharacterTurnStartListenerAuraOfDomination(conditionAuraOfDomination));

        var powerAuraOfDomination = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, $"Power{Name}{AURA_DOMINATION}")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite(AURA_DOMINATION, Resources.PowerAuraOfDomination, 256, 128))
            .AddToDB();

        // keep it simple and ensure it'll follow any changes from Aura of Protection
        powerAuraOfDomination.EffectDescription.targetSide = Side.Enemy;
        powerAuraOfDomination.EffectDescription.targetExcludeCaster = true;
        powerAuraOfDomination.EffectDescription.EffectForms[0] = EffectFormBuilder
            .Create()
            .SetConditionForm(conditionAuraOfDomination, ConditionForm.ConditionOperation.Add)
            .Build();

        // LEVEL 15

        // Harrowing Crusade

        const string HARROWING_CRUSADE = "HarrowingCrusade";

        var featureHarrowingCrusade = FeatureDefinitionBuilder
            .Create($"Feature{Name}{HARROWING_CRUSADE}")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new ReactToAttackOnMeOrMeFinishedHarrowingCrusade(conditionMarkOfTheSubmission))
            .AddToDB();

        //
        // LEVEL 18
        //

        // Improved Aura of Domination

        var powerAuraOfDomination18 = FeatureDefinitionPowerBuilder
            .Create(powerAuraOfDomination, $"Power{Name}AuraOfDomination18")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(powerAuraOfDomination)
            .AddToDB();

        powerAuraOfDomination18.EffectDescription.targetParameter = 13;

        //
        // LEVEL 20
        //

        // Aspect of Dread

        var additionalDamageAspectOfDread = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}AspectOfDread")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("AspectOfDread")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .AddToDB();

        var featureSetAspectOfDread = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AspectOfDreadDamageResistance")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var conditionAspectOfDread = ConditionDefinitionBuilder
            .Create(ConditionAspectOfDreadName)
            .SetGuiPresentation(Category.Condition, ConditionPactChainImp)
            .SetPossessive()
            .AddFeatures(additionalDamageAspectOfDread, featureSetAspectOfDread)
            .AddToDB();

        foreach (var damage in DatabaseRepository.GetDatabase<DamageDefinition>())
        {
            var title = Gui.Localize($"Rules/&{damage.Name}Title");
            var damageAffinityAspectOfDread = FeatureDefinitionDamageAffinityBuilder
                .Create($"DamageAffinity{Name}AspectOfDread{damage.Name}")
                .SetGuiPresentation(title, Gui.Format("Feature/&DamageResistanceFormat", title))
                .SetDamageType(damage.Name)
                .SetDamageAffinityType(DamageAffinityType.Resistance)
                .AddToDB();

            featureSetAspectOfDread.FeatureSet.Add(damageAffinityAspectOfDread);
        }

        var powerAspectOfDread = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AspectOfDread")
            .SetGuiPresentation(Category.Feature, Sprites
                .GetSprite("PowerArdentHate", Resources.PowerArdentHate, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(PowerFighterActionSurge)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionAspectOfDread, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.OathOfDread, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                featureSetMarkOfTheSubmission,
                powerDreadfulPresence)
            .AddFeaturesAtLevel(7,
                powerAuraOfDomination)
            .AddFeaturesAtLevel(15,
                featureHarrowingCrusade)
            .AddFeaturesAtLevel(18,
                powerAuraOfDomination18)
            .AddFeaturesAtLevel(20,
                powerAspectOfDread)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Paladin;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Aura of Domination
    //

    private sealed class CharacterTurnStartListenerAuraOfDomination(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionAuraOfDomination) : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetDefender = locationCharacter.RulesetActor;
            var rulesetCondition = rulesetDefender.AllConditions.FirstOrDefault(x =>
                x.ConditionDefinition == conditionAuraOfDomination);

            if (rulesetCondition == null)
            {
                return;
            }

            var rulesetAttacker = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);
            var hasFrightenedFromSource = rulesetDefender.AllConditions.Any(x =>
                x.SourceGuid == rulesetAttacker.Guid &&
                (x.ConditionDefinition == ConditionDefinitions.ConditionFrightened ||
                 x.ConditionDefinition.IsSubtypeOf(RuleDefinitions.ConditionFrightened)));

            if (!hasFrightenedFromSource)
            {
                return;
            }

            rulesetDefender.InflictCondition(
                CustomConditionsContext.StopMovement.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                CustomConditionsContext.StopMovement.Name,
                0,
                0,
                0);

            var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Paladin);
            var totalDamage = classLevel / 2;
            var damageForm = new DamageForm
            {
                DamageType = DamageTypePsychic, DieType = DieType.D1, DiceNumber = 0, BonusDamage = totalDamage
            };


            var locationCharacterAttacker = GameLocationCharacter.GetFromActor(rulesetAttacker);

            if (locationCharacterAttacker != null)
            {
                EffectHelpers.StartVisualEffect(locationCharacterAttacker, locationCharacter, DreadfulOmen);
            }

            var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
            {
                sourceCharacter = rulesetAttacker,
                targetCharacter = rulesetDefender,
                position = locationCharacter.LocationPosition
            };

            RulesetActor.InflictDamage(
                totalDamage,
                damageForm,
                DamageTypePsychic,
                applyFormsParams,
                rulesetDefender,
                false,
                rulesetAttacker.Guid,
                false,
                [],
                new RollInfo(DieType.D1, [], totalDamage),
                true,
                out _);
        }
    }

    //
    // Harrowing Crusade
    //

    private class ReactToAttackOnMeOrMeFinishedHarrowingCrusade(ConditionDefinition conditionMarkOfTheSubmission)
        : IPhysicalAttackFinishedOnMeOrAlly
    {
        public IEnumerator OnPhysicalAttackFinishedOnMeOrAlly(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var actionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (!actionManager)
            {
                yield break;
            }

            if (helper.IsMyTurn())
            {
                yield break;
            }

            if (!helper.CanReact())
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var hasFrightened = rulesetAttacker.AllConditions.Any(x =>
                x.ConditionDefinition == ConditionDefinitions.ConditionFrightened ||
                x.ConditionDefinition.IsSubtypeOf(RuleDefinitions.ConditionFrightened));

            if (!hasFrightened && !rulesetAttacker.HasConditionOfType(conditionMarkOfTheSubmission))
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = helper.GetFirstMeleeModeThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                (retaliationMode, retaliationModifier) = helper.GetFirstRangedModeThatCanAttack(attacker);

                if (retaliationMode == null)
                {
                    yield break;
                }
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            var actionParams = new CharacterActionParams(helper, ActionDefinitions.Id.AttackOpportunity)
            {
                StringParameter = helper.Name,
                ActionModifiers = { retaliationModifier },
                AttackMode = retaliationMode,
                TargetCharacters = { attacker }
            };
            var reactionRequest = new ReactionRequestReactionAttack("HarrowingCrusade", actionParams);
            var count = actionManager.PendingReactionRequestGroups.Count;

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);
        }
    }
}
