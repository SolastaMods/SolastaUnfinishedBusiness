using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PathOfTheElements : AbstractSubclass
{
    private const string Name = "PathOfTheElements";
    private const string ElementalBlessing = "ElementalBlessing";

    internal PathOfTheElements()
    {
        // LEVEL 03

        // Elemental Fury

        var ancestryStorm = FeatureDefinitionAncestryBuilder
            .Create($"Ancestry{Name}Storm")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}Description", Gui.Localize("Rules/&DamageLightningTitle")))
            .SetAncestry(ExtraAncestryType.PathOfTheElements)
            .SetDamageType(DamageTypeLightning)
            .AddToDB();

        ancestryStorm.SetCustomSubFeatures(new CharacterTurnEndedElementalFury(ancestryStorm));

        var ancestryBlizzard = FeatureDefinitionAncestryBuilder
            .Create($"Ancestry{Name}Blizzard")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}Description", Gui.Localize("Rules/&DamageColdTitle")))
            .SetAncestry(ExtraAncestryType.PathOfTheElements)
            .SetDamageType(DamageTypeCold)
            .AddToDB();

        ancestryBlizzard.SetCustomSubFeatures(new CharacterTurnEndedElementalFury(ancestryBlizzard));

        var ancestryWildfire = FeatureDefinitionAncestryBuilder
            .Create($"Ancestry{Name}Wildfire")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}Description", Gui.Localize("Rules/&DamageFireTitle")))
            .SetAncestry(ExtraAncestryType.PathOfTheElements)
            .SetDamageType(DamageTypeFire)
            .AddToDB();

        ancestryWildfire.SetCustomSubFeatures(new CharacterTurnEndedElementalFury(ancestryWildfire));

        var featureSetElementalFury = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ElementalFury")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSet(
                ancestryStorm,
                ancestryBlizzard,
                ancestryWildfire)
            .SetAncestryType(ExtraAncestryType.PathOfTheElements)
            .AddToDB();

        // LEVEL 06

        // Storm

        var conditionElementalBlessingStorm = ConditionDefinitionBuilder
            .Create($"Condition{Name}{ElementalBlessing}Storm")
            .SetGuiPresentation($"Condition{Name}{ElementalBlessing}", Category.Condition,
                ConditionDefinitions.ConditionBlessed)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance)
            .AddToDB();

        var powerElementalBlessingStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalBlessing}Storm")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.UntilShortRest)
                .Build())
            .AddToDB();

        powerElementalBlessingStorm.SetCustomSubFeatures(new CustomBehaviorElementalBlessing(
            powerElementalBlessingStorm, conditionElementalBlessingStorm));

        // Blizzard

        var conditionElementalBlessingBlizzard = ConditionDefinitionBuilder
            .Create($"Condition{Name}{ElementalBlessing}Blizzard")
            .SetGuiPresentation($"Condition{Name}{ElementalBlessing}", Category.Condition,
                ConditionDefinitions.ConditionBlessed)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance)
            .AddToDB();

        var powerElementalBlessingBlizzard = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalBlessing}Blizzard")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.UntilShortRest)
                .Build())
            .AddToDB();

        powerElementalBlessingBlizzard.SetCustomSubFeatures(new CustomBehaviorElementalBlessing(
            powerElementalBlessingBlizzard, conditionElementalBlessingBlizzard));

        // Wildfire

        var conditionElementalBlessingWildfire = ConditionDefinitionBuilder
            .Create($"Condition{Name}{ElementalBlessing}Wildfire")
            .SetGuiPresentation($"Condition{Name}{ElementalBlessing}", Category.Condition,
                ConditionDefinitions.ConditionBlessed)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance)
            .AddToDB();

        var powerElementalBlessingWildfire = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalBlessing}Wildfire")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.UntilShortRest)
                .Build())
            .AddToDB();

        powerElementalBlessingWildfire.SetCustomSubFeatures(new CustomBehaviorElementalBlessing(
            powerElementalBlessingWildfire, conditionElementalBlessingWildfire));

        // Elemental Blessing

        var featureSetElementalBlessing = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{ElementalBlessing}")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.DeterminedByAncestry)
            .SetAncestryType(ExtraAncestryType.PathOfTheElements, DamageTypeLightning, DamageTypeCold, DamageTypeFire)
            .AddFeatureSet(
                powerElementalBlessingStorm,
                powerElementalBlessingBlizzard,
                powerElementalBlessingWildfire)
            .AddToDB();

        // LEVEL 10


        // LEVEL 14


        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheElements, 256))
            .AddFeaturesAtLevel(3, featureSetElementalFury)
            .AddFeaturesAtLevel(6, featureSetElementalBlessing)
            .AddFeaturesAtLevel(10)
            .AddFeaturesAtLevel(14)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Elemental Fury
    //

    private sealed class CharacterTurnEndedElementalFury : ICharacterTurnEndListener
    {
        private static FeatureDefinitionAncestry _ancestry;

        public CharacterTurnEndedElementalFury(FeatureDefinitionAncestry ancestry)
        {
            _ancestry = ancestry;
        }

        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
            var battle = Gui.Battle;

            if (battle == null)
            {
                return;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var rulesetAttacker = locationCharacter.RulesetCharacter;

            foreach (var targetLocationCharacter in battle.AllContenders
                         .Where(x =>
                             x.Side != locationCharacter.Side &&
                             gameLocationBattleService.IsWithin1Cell(locationCharacter, x)))
            {
                var rulesetDefender = targetLocationCharacter.RulesetCharacter;
                var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Barbarian);
                int diceNumber;
                DieType dieType;

                switch (classLevel)
                {
                    case < 6:
                        diceNumber = 1;
                        dieType = DieType.D6;
                        break;
                    case < 10:
                        diceNumber = 1;
                        dieType = DieType.D10;
                        break;
                    case < 14:
                        diceNumber = 2;
                        dieType = DieType.D6;
                        break;
                    default:
                        diceNumber = 2;
                        dieType = DieType.D10;
                        break;
                }

                var damageForm = new DamageForm
                {
                    DamageType = _ancestry.damageType, DieType = dieType, DiceNumber = diceNumber
                };

                rulesetAttacker.RollDie(dieType, RollContext.AttackDamageValueRoll, true, AdvantageType.None,
                    out var firstRoll, out _);

                GameConsoleHelper.LogCharacterUsedFeature(rulesetAttacker, _ancestry);

                RulesetActor.InflictDamage(
                    firstRoll,
                    damageForm,
                    _ancestry.damageType,
                    new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                    rulesetDefender,
                    false,
                    rulesetAttacker.Guid,
                    false,
                    new List<string>(),
                    new RollInfo(dieType, new List<int> { firstRoll }, firstRoll),
                    true,
                    out _);
            }
        }
    }

    //
    // Elemental Blessing
    //

    private class CustomBehaviorElementalBlessing : IOnAfterActionFeature, ICharacterTurnStartListener
    {
        private readonly FeatureDefinitionPower _powerDefinition;
        private readonly ConditionDefinition _conditionDefinition;

        public CustomBehaviorElementalBlessing(
            FeatureDefinitionPower powerDefinition,
            ConditionDefinition conditionDefinition)
        {
            _powerDefinition = powerDefinition;
            _conditionDefinition = conditionDefinition;
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var sourceRulesetCharacter = locationCharacter.RulesetCharacter;

            if (sourceRulesetCharacter.HasAnyConditionOfType(ConditionRaging))
            {
                AddCondition(locationCharacter);
            }
            else
            {
                RemoveCondition(locationCharacter.RulesetCharacter);
            }
        }

        public void OnAfterAction(CharacterAction action)
        {
            switch (action)
            {
                case CharacterActionSpendPower characterActionSpendPower when
                    characterActionSpendPower.activePower.PowerDefinition == _powerDefinition:
                    AddCondition(action.ActingCharacter);
                    break;
                case CharacterActionRageStop:
                    RemoveCondition(action.ActingCharacter.RulesetCharacter);
                    break;
            }
        }

        private void RemoveCondition(RulesetActor rulesetCharacter)
        {
            var battle = Gui.Battle;

            if (battle == null)
            {
                return;
            }

            foreach (var targetLocationCharacter in battle.AllContenders
                         .Where(x => x.Side == rulesetCharacter.Side))
            {
                var targetRulesetCharacter = targetLocationCharacter.RulesetCharacter;
                var rulesetCondition =
                    targetRulesetCharacter.AllConditions.FirstOrDefault(x =>
                        x.ConditionDefinition == _conditionDefinition);

                if (rulesetCondition != null)
                {
                    targetRulesetCharacter.RemoveCondition(rulesetCondition);
                }
            }
        }

        private void AddCondition(GameLocationCharacter sourceLocationCharacter)
        {
            var battle = Gui.Battle;

            if (battle == null)
            {
                return;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var factionName = sourceLocationCharacter.RulesetCharacter.CurrentFaction.Name;

            foreach (var targetLocationCharacter in battle.AllContenders
                         .Where(x =>
                             x.Side == sourceLocationCharacter.Side &&
                             gameLocationBattleService.IsWithinXCells(sourceLocationCharacter, x, 2)))
            {
                var condition = RulesetCondition.CreateActiveCondition(
                    targetLocationCharacter.Guid,
                    _conditionDefinition,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfSourceTurn,
                    sourceLocationCharacter.Guid,
                    factionName);

                targetLocationCharacter.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagEffect,
                    condition);
            }
        }
    }
}
