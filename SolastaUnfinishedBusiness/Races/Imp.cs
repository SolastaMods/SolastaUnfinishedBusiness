using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
namespace SolastaUnfinishedBusiness.Races;
internal class RaceImpBuilder
{
    internal static CharacterRaceDefinition RaceImp { get; } = BuildImp();

    private const string Name = "Imp";

    [NotNull]
    private static CharacterRaceDefinition BuildImp()
    { 
        var raceImp = CharacterRaceDefinitionBuilder
            .Create(CharacterRaceDefinitions.Tiefling, $"Race{Name}")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision,
                FeatureDefinitionFeatureSets.FeatureSetHumanLanguages
                )
            .AddToDB();

        RacesContext.RaceScaleMap[raceImp] = 7f / 9.4f;

        raceImp.subRaces = new List<CharacterRaceDefinition>
        {
            BuildImpInfernal(raceImp),
            BuildImpForest(raceImp)
        }; 
        return raceImp;
    }

    #region Infernal Imp
    private static CharacterRaceDefinition BuildImpInfernal(CharacterRaceDefinition raceImp)
    {
        var Name = "ImpInfernal";

        var spriteReference = Sprites.GetSprite(Name, Resources.ImpInfernal, 1024, 512);

        var featureSetImpInfernalFiendishResistance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}FiendishResistance")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance)
            .AddToDB();

        var featureSetImpInfernalAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                FeatureDefinitionAttributeModifiers.AttributeModifierDragonbornAbilityScoreIncreaseCha
            )
            .AddToDB();

        var castSpellImpInfernal = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, $"CastSpell{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(
                SpellListDefinitionBuilder
                    .Create($"SpellList{Name}")
                    .SetGuiPresentationNoContent(true)
                    .ClearSpells()
                    .SetSpellsAtLevel(0, SpellDefinitions.ViciousMockery)
                    .SetSpellsAtLevel(1, SpellDefinitions.Invisibility)
                    .FinalizeSpells(true, 1)
                    .AddToDB())
            .AddToDB();

        var raceImpInfernal = CharacterRaceDefinitionBuilder
            .Create(raceImp, $"Race{Name}")
            .SetBaseHeight(42)
            .SetGuiPresentation(Category.Race, spriteReference)
            .SetFeaturesAtLevel(1,
                featureSetImpInfernalAbilityScoreIncrease,
                featureSetImpInfernalFiendishResistance,
                castSpellImpInfernal
                )
            .AddToDB();
        raceImpInfernal.racePresentation.preferedSkinColors = new TA.RangedInt(15, 19);
        return raceImpInfernal;
    }
    #endregion

    #region Forest Imp
    private static CharacterRaceDefinition BuildImpForest(CharacterRaceDefinition raceImp)
    {
        var Name = "ImpForest";

        var spriteReference = Sprites.GetSprite(Name, Resources.ImpForest, 1024, 512);

        var featureSetImpForestAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                FeatureDefinitionAttributeModifiers.AttributeModifierHalfOrcAbilityScoreIncreaseCon
            )
            .AddToDB();

        var actionAffinityImpForestInnateCunning = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}InnateCunning")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions(
                ActionDefinitions.Id.DisengageBonus,
                ActionDefinitions.Id.HideBonus)
           .AddToDB();

        var powerImpForestImpishWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImpishWrath")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerImpForestImpishWrath.AddCustomSubFeatures(
            new AttackBeforeHitConfirmedImpishWrath(powerImpForestImpishWrath));

        var raceImpForest = CharacterRaceDefinitionBuilder
            .Create(raceImp, $"Race{Name}")
            .SetGuiPresentation(Category.Race, spriteReference)
            .SetFeaturesAtLevel(1,
                featureSetImpForestAbilityScoreIncrease,
                actionAffinityImpForestInnateCunning,
                powerImpForestImpishWrath,
                FeatureDefinitionFeatureSets.FeatureSetElfFeyAncestry
                )
            .AddToDB();

        raceImpForest.racePresentation.preferedSkinColors = new TA.RangedInt(28, 37);
        return raceImpForest;
    }
    internal class AttackBeforeHitConfirmedImpishWrath : IPhysicalAttackFinishedByMe, IMagicalAttackFinishedByMe
    {
        private FeatureDefinitionPower _powerPool;

        public AttackBeforeHitConfirmedImpishWrath(FeatureDefinitionPower powerPool)
        {
            _powerPool = powerPool;
        }

        private IEnumerator HandleImpishWrath(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            string damageType = DamageTypeBludgeoning)
        {

            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            if (rulesetAttacker is not { IsDeadOrUnconscious: false })
            {
                yield break;
            }

            if (rulesetDefender is not { IsDeadOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetAttacker.CanUsePower(_powerPool))
            {
                yield break;
            }

            // maybe add some toggle here similar to Paladin Smite

            var usablePower = UsablePowersProvider.Get(_powerPool, rulesetAttacker);
            var bonusDamage = AttributeDefinitions.ComputeProficiencyBonus(
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.CharacterLevel));

            var reactionParams = new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter = Gui.Format("Reaction/&CustomReactionImpishWrathDescription", 
                    bonusDamage.ToString(), rulesetDefender.Name),
                UsablePower = usablePower
            };

            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("ImpishWrath", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                attacker, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetAttacker.UsePower(usablePower);

            var damage = new DamageForm
            {
                DamageType = damageType,
                DieType = DieType.D1,
                DiceNumber = 0,
                BonusDamage = bonusDamage
            };

            var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
            {
                sourceCharacter = rulesetAttacker,
                targetCharacter = rulesetDefender,
                position = defender.LocationPosition
            };

            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            implementationService.ApplyEffectForms(
                new List<EffectForm> { new() { damageForm = damage } },
                applyFormsParams,
                new List<string> { damageType },
                out _,
                out _);
        }

        public IEnumerator OnMagicalAttackFinishedByMe(
            CharacterActionMagicEffect action, 
            GameLocationCharacter attacker, 
            GameLocationCharacter defender)
        {
            var rulesetEffect = action.actionParams.RulesetEffect;

            if (!rulesetEffect.EffectDescription.HasFormOfType(EffectForm.EffectFormType.Damage))
            {
                yield break;
            }
            
            if (action.AttackRollOutcome != RollOutcome.Success
                && action.AttackRollOutcome != RollOutcome.CriticalSuccess
                && action.SaveOutcome != RollOutcome.Success
                && action.SaveOutcome != RollOutcome.CriticalSuccess
                )
            {
                yield break;
            }


            yield return HandleImpishWrath(attacker,
                defender, 
                rulesetEffect.EffectDescription?.FindFirstDamageForm()?.damageType);
        }

        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action, 
            GameLocationCharacter attacker, 
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome, 
            int damageAmount)
        {
            if (action.AttackRollOutcome != RollOutcome.Success
                && action.AttackRollOutcome != RollOutcome.CriticalSuccess
                )
            {
                yield break;
            }
            yield return HandleImpishWrath(attacker,
                defender,
                attackerAttackMode.EffectDescription?.FindFirstDamageForm()?.damageType);
        }
    }

    #endregion

}
