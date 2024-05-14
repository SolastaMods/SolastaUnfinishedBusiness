using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PathOfTheWildMagic //: AbstractSubclass
{
    private const string Name = "PathOfTheWildMagic";

    public PathOfTheWildMagic()
    {
        // Magic Awareness

        var powerMagicAwareness = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MagicAwareness")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.DetectMagic)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpellDefinitions.DetectMagic)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self, 0)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDivinationForm(
                                DivinationForm.Type.RevealEntitiesBearingTags,
                                [], [TagsDefinitions.Magical], 12)
                            .Build(),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDetectMagicSight))
                    .Build())
            .AddToDB();

        // Wild Surge

        var featureSetWildSurge = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}WildSurge")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet()
            .AddToDB();

        // Bolstering Magic

        var powerBolsteringMagic = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BolsteringMagic")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        var conditionBolsteringMagicRoll = ConditionDefinitionBuilder
            .Create($"Condition{Name}BolsteringMagicRoll")
            .SetGuiPresentation(Category.Condition)
            .AddToDB();

        conditionBolsteringMagicRoll.AddCustomSubFeatures(
            new CustomBehaviorBolsteringMagicRoll(conditionBolsteringMagicRoll));

        var powerBolsteringMagicRoll = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BolsteringMagicRoll")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.Action, powerBolsteringMagic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBolsteringMagicRoll))
                    .Build())
            .AddToDB();

        var db = DatabaseRepository.GetDatabase<FeatureDefinitionMagicAffinity>();
        var conditions = new ConditionDefinition[3];

        for (var i = 1; i <= 3; i++)
        {
            conditions[i - 1] = ConditionDefinitionBuilder
                .Create($"Condition{Name}BolsteringMagicSlot{i}")
                .SetGuiPresentation($"Condition{Name}BolsteringMagicSlot", Category.Condition)
                .SetSilent(Silent.WhenRemoved)
                .SetFeatures(db.GetElement($"MagicAffinityAdditionalSpellSlot{i}"))
                .AddToDB();
        }

        var powerBolsteringMagicSpell = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BolsteringMagicSpell")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.Action, powerBolsteringMagic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditions[0],
                                ConditionForm.ConditionOperation.AddRandom,
                                false, false, conditions)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(new FilterTargetingCharacterBolsteringMagicSpell(conditions))
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerBolsteringMagic, false,
            powerBolsteringMagicRoll, powerBolsteringMagicSpell);

        var featureSetBolsteringMagic = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BolsteringMagic")
            .SetGuiPresentation($"Power{Name}BolsteringMagic", Category.Feature)
            .SetFeatureSet(powerBolsteringMagic, powerBolsteringMagicRoll, powerBolsteringMagicSpell)
            .AddToDB();

        // Unstable Backslash

        var featureUnstableBackslash = FeatureDefinitionBuilder
            .Create($"Feature{Name}UnstableBackslash")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Controlled Surge

        var featureControlledSurge = FeatureDefinitionBuilder
            .Create($"Feature{Name}ControlledSurge")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.PathBerserker)
            .AddFeaturesAtLevel(3, powerMagicAwareness, featureSetWildSurge)
            .AddFeaturesAtLevel(6, featureSetBolsteringMagic)
            .AddFeaturesAtLevel(10, featureUnstableBackslash)
            .AddFeaturesAtLevel(14, featureControlledSurge)
            .AddToDB();
    }

    internal CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal CharacterSubclassDefinition Subclass { get; }

    internal FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal DeityDefinition DeityDefinition { get; }

    private sealed class CustomBehaviorBolsteringMagicRoll(
        ConditionDefinition conditionBolsteringMagicRoll) : ITryAlterOutcomeAttack, ITryAlterOutcomeAttributeCheck
    {
        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier)
        {
            if (helper != defender)
            {
                yield break;
            }

            var advantageType = actionModifier.AttackAdvantageTrend switch
            {
                > 0 => AdvantageType.Advantage,
                < 0 => AdvantageType.Disadvantage,
                _ => AdvantageType.None
            };

            var roll = RollDie(DieType.D3, advantageType, out _, out _);

            actionModifier.AttacktoHitTrends.Add(
                new TrendInfo(roll, FeatureSourceType.CharacterFeature, conditionBolsteringMagicRoll.Name,
                    conditionBolsteringMagicRoll));
        }

        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier abilityCheckModifier)
        {
            if (helper != defender ||
                abilityCheckData.AbilityCheckRollOutcome != RollOutcome.Failure)
            {
                yield break;
            }
        }
    }

    private sealed class FilterTargetingCharacterBolsteringMagicSpell(
        params ConditionDefinition[] conditions) : IFilterTargetingCharacter
    {
        private readonly string[] _conditionNames = conditions.Select(x => x.Name).ToArray();

        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var isValid = !target.RulesetActor.HasAnyConditionOfType(_conditionNames);

            if (isValid)
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Tooltip/&MustNotHaveBolsteringMagicSpell");

            return false;
        }
    }
}
