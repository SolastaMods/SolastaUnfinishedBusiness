using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static AttributeDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfTheDiscordance : AbstractSubclass
{
    private const string Name = "WayOfTheDiscordance";


    private const string ConditionProfoundTurmoilName = $"Condition{Name}ProfoundTurmoil";

    private static readonly ConditionDefinition ConditionProfoundTurmoil = ConditionDefinitionBuilder
        .Create(ConditionProfoundTurmoilName)
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDoomLaughter)
        .SetPossessive()
        .SetConditionType(ConditionType.Detrimental)
        .SetSpecialDuration(DurationType.Minute, 1)
        .CopyParticleReferences(ConditionDefinitions.ConditionMalediction)
        .AddFeatures(
            FeatureDefinitionSavingThrowAffinityBuilder
                .Create($"SavingThrowAffinity{Name}ProfoundTurmoil")
                .SetGuiPresentation(ConditionProfoundTurmoilName, Category.Condition)
                .SetModifiers(FeatureDefinitionSavingThrowAffinity.ModifierType.RemoveDice, DieType.D4, 1, false,
                    Charisma, Constitution, Dexterity, Intelligence, Strength, Wisdom)
                .AddToDB(),
            FeatureDefinitionAttackModifierBuilder
                .Create($"AttackModifier{Name}ProfoundTurmoil")
                .SetGuiPresentation(ConditionProfoundTurmoilName, Category.Condition)
                .SetAttackRollModifier(-2)
                .AddToDB())
        .AddToDB();

    private static readonly ConditionDefinition ConditionProfoundTurmoilMark = ConditionDefinitionBuilder
        .Create($"Condition{Name}ProfoundTurmoilMark")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialDuration(DurationType.Permanent)
        .CopyParticleReferences(ConditionDefinitions.ConditionMalediction)
        .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerProfoundTurmoil = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}ProfoundTurmoil")
        .SetGuiPresentation(Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetSavingThrowData(false, Charisma, false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency, Wisdom, 8)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .SetConditionForm(ConditionProfoundTurmoilMark, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                        .SetConditionForm(ConditionProfoundTurmoil, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
        .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
        .AddToDB();

    internal WayOfTheDiscordance()
    {
        var conditionDiscordance = ConditionDefinitionBuilder
            .Create($"Condition{Name}Discordance")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMarkedByBrandingSmite)
            .SetConditionType(ConditionType.Detrimental)
            .SetSilent(Silent.WhenRemoved)
            .AllowMultipleInstances()
            .SetPossessive()
            .AddToDB();

        var powerDiscordanceDamage = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DiscordanceDamage")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(Bane)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D4)
                            .Build())
                    .Build())
            .AddToDB();

        var powerDiscordance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DiscordanceBase")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetReactionContext((ReactionTriggerContext)ExtraReactionContext.Custom)
            .AddToDB();

        powerDiscordance.SetCustomSubFeatures(
            new AttackEffectAfterDamageDiscordance(conditionDiscordance, powerDiscordanceDamage));

        var powerBurstOfDisharmonyPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BurstOfDisharmony")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerBurstOfDisharmony", Resources.PowerBurstOfDisharmony, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .AddToDB();

        var powerBurstOfDisharmonyList = new List<FeatureDefinitionPower>();

        for (var i = 6; i >= 1; i--)
        {
            var a = i;

            var powerBurstOfDisharmony = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{Name}BurstOfDisharmony{i}")
                .SetGuiPresentation(
                    Gui.Format($"Feature/&Power{Name}SubBurstOfDisharmonyTitle", i.ToString()),
                    Gui.Format($"Feature/&Power{Name}SubBurstOfDisharmonyDescription",
                        i.ToString(),
                        (i + 2).ToString()))
                .SetSharedPool(ActivationTime.BonusAction, powerBurstOfDisharmonyPool)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Cube, 3, 3)
                        .SetDurationData(DurationType.Minute, 1)
                        .SetParticleEffectParameters(DreadfulOmen)
                        .SetSavingThrowData(
                            false,
                            Constitution,
                            true,
                            EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                                .SetDamageForm(DamageTypeNecrotic, 2 + i, DieType.D6)
                                .Build(),
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(
                                    conditionDiscordance,
                                    ConditionForm.ConditionOperation.Add)
                                .Build())
                        .Build())
                .SetCustomSubFeatures(
                    new ValidatorsPowerUse(
                        c => c.RemainingKiPoints >= a &&
                             c.TryGetAttributeValue(ProficiencyBonus) >= a))
                .AddToDB();

            powerBurstOfDisharmonyList.Add(powerBurstOfDisharmony);
        }

        PowerBundle.RegisterPowerBundle(
            powerBurstOfDisharmonyPool, false,
            powerBurstOfDisharmonyList
        );

        var featureSetDiscordance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Discordance")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerDiscordance)
            .AddToDB();

        var featureSetSchism = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Schism")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();


        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheDiscordance, 256))
            .AddFeaturesAtLevel(3, featureSetDiscordance)
            .AddFeaturesAtLevel(6, featureSetSchism)
            .AddFeaturesAtLevel(11, powerBurstOfDisharmonyPool)
            .AddFeaturesAtLevel(17, PowerProfoundTurmoil)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }


    private static void ApplyProfoundTurmoil(IControllableCharacter attacker, GameLocationCharacter defender)
    {
        var rulesetAttacker = attacker.RulesetCharacter;
        var rulesetDefender = defender.RulesetCharacter;

        if (rulesetDefender.IsDeadOrDying ||
            rulesetDefender.HasAnyConditionOfType(ConditionProfoundTurmoilMark.Name) ||
            rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Monk) < 17)
        {
            return;
        }

        var rulesetPower = UsablePowersProvider.Get(PowerProfoundTurmoil, rulesetAttacker);
        var effectPower = new RulesetEffectPower(rulesetAttacker, rulesetPower);

        effectPower.ApplyEffectOnCharacter(rulesetDefender, true, defender.LocationPosition);
    }

    // apply the logic to add discordance and profound turmoil conditions and to determine if it's time to explode
    private sealed class AttackEffectAfterDamageDiscordance : IActionFinished, IAttackEffectAfterDamage
    {
        private const int DiscordanceLimit = 3;
        private readonly ConditionDefinition _conditionDiscordance;
        private readonly FeatureDefinitionPower _powerDiscordanceDamage;

        public AttackEffectAfterDamageDiscordance(
            ConditionDefinition conditionDiscordance,
            FeatureDefinitionPower powerDiscordanceDamage)
        {
            _conditionDiscordance = conditionDiscordance;
            _powerDiscordanceDamage = powerDiscordanceDamage;
        }

        public IEnumerator OnActionFinished(CharacterAction action)
        {
            var gameLocationAttacker = action.ActingCharacter;
            var rulesetAttacker = gameLocationAttacker.RulesetCharacter;

            // force expend ki points depending on power level used
            if (action is CharacterActionUsePower characterActionUsePower &&
                characterActionUsePower.activePower.PowerDefinition.Name.StartsWith($"Power{Name}BurstOfDisharmony"))
            {
                var name = characterActionUsePower.activePower.PowerDefinition.Name;
                var kiPoints = int.Parse(name.Substring(name.Length - 1, 1));
                var kiPointsAltered = rulesetAttacker.KiPointsAltered;

                rulesetAttacker.ForceKiPointConsumption(kiPoints);
                kiPointsAltered?.Invoke(rulesetAttacker, rulesetAttacker.RemainingKiPoints);
            }

            // handle Schism behavior
            // if in the future we need to nerf this, gotta add a check for RemainingRounds == 1
            if (GetMonkLevel(rulesetAttacker) >= 6)
            {
                foreach (var gameLocationDefender in action.ActionParams.TargetCharacters
                             .Where(t => t.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                         t.RulesetCharacter.AllConditions
                                             .Any(x => x.ConditionDefinition ==
                                                       ConditionDefinitions.ConditionStunned_MonkStunningStrike &&
                                                       x.RemainingRounds <= 1))
                             .ToList()) // avoid changing enumerator
                {
                    ApplyCondition(gameLocationAttacker, gameLocationDefender, _conditionDiscordance);
                }
            }

            // although it should be one target only, we better keep it compatible for any future feature
            foreach (var gameLocationDefender in action.ActionParams.TargetCharacters
                         .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                         .Select(gameLocationCharacter => new
                         {
                             gameLocationCharacter,
                             discordanceCount = gameLocationCharacter.RulesetCharacter.AllConditions
                                 .FindAll(x => x.ConditionDefinition == _conditionDiscordance)
                                 .Count
                         })
                         .Where(t =>
                             !t.gameLocationCharacter.RulesetCharacter.IsDeadOrDyingOrUnconscious &&
                             t.discordanceCount >= DiscordanceLimit)
                         .Select(t => t.gameLocationCharacter)
                         .ToList()) // avoid changing enumerator
            {
                var rulesetDefender = gameLocationDefender.RulesetCharacter;

                if (rulesetDefender == null || rulesetDefender.IsDeadOrDying)
                {
                    continue;
                }

                // remove conditions up to the limit to also support Schism scenario
                rulesetDefender.AllConditions
                    .FindAll(x => x.ConditionDefinition == _conditionDiscordance)
                    .OrderBy(x => x.RemainingRounds)
                    .Take(DiscordanceLimit)
                    .ToList() // avoid changing enumerator
                    .ForEach(x => rulesetDefender.RemoveCondition(x));

                // setup explosion power and increase damage dice based on Monk progression
                var usablePower = UsablePowersProvider.Get(_powerDiscordanceDamage, rulesetAttacker);
                var effectPower = new RulesetEffectPower(rulesetAttacker, usablePower);
                var damageForm = effectPower.EffectDescription.FindFirstDamageForm();
                var monkLevel = GetMonkLevel(rulesetAttacker);

                if (damageForm == null || monkLevel <= 0)
                {
                    continue;
                }

                damageForm.BonusDamage = ComputeAbilityScoreModifier(rulesetAttacker.TryGetAttributeValue(Wisdom));
                damageForm.DieType = FeatureDefinitionAttackModifiers.AttackModifierMonkMartialArtsImprovedDamage
                    .DieTypeByRankTable.Find(x => x.Rank == monkLevel).DieType;

                effectPower.EffectDescription.effectParticleParameters.targetParticleReference =
                    effectPower.EffectDescription.effectParticleParameters.conditionStartParticleReference;

                EffectHelpers.StartVisualEffect(
                    gameLocationAttacker, gameLocationDefender, Malediction,
                    EffectHelpers.EffectType.Effect);
                effectPower.ApplyEffectOnCharacter(rulesetDefender, true, gameLocationDefender.LocationPosition);

                ApplyProfoundTurmoil(gameLocationAttacker, gameLocationDefender);
            }

            yield break;
        }

        // only add condition if monk weapon or unarmed
        public void OnAttackEffectAfterDamage(
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

            if (attackMode is not { SourceDefinition: ItemDefinition item } ||
                !item.WeaponDescription.IsMonkWeaponOrUnarmed())
            {
                return;
            }

            ApplyCondition(attacker, defender, _conditionDiscordance);
        }

        private static void ApplyCondition(
            IControllableCharacter attacker,
            IControllableCharacter defender,
            BaseDefinition conditionDefinition)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker == null || rulesetDefender == null || rulesetDefender.IsDeadOrDying)
            {
                return;
            }

            rulesetDefender.InflictCondition(
                conditionDefinition.Name,
                DurationType.Minute,
                1,
                TurnOccurenceType.EndOfTurn,
                TagCombat,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }

        // return the Monk level factoring in wildshape multiclass scenarios
        private static int GetMonkLevel(RulesetCharacter rulesetCharacter)
        {
            return rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Monk);
        }
    }
}
