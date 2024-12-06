using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private sealed class CustomBehaviorHeightenedMetabolism(
        ConditionDefinition conditionFlurryOfBlowsHeightenedMetabolism,
        ConditionDefinition conditionFlurryOfBlowsFreedomHeightenedMetabolism)
        : IModifyEffectDescription, IMagicEffectFinishedByMe
    {
        private readonly EffectForm _effectForm =
            EffectFormBuilder.ConditionForm(conditionFlurryOfBlowsHeightenedMetabolism);

        private readonly EffectForm _effectFormFreedom =
            EffectFormBuilder.ConditionForm(conditionFlurryOfBlowsFreedomHeightenedMetabolism);

        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var definition = action.ActionParams.activeEffect.SourceDefinition;

            if (definition != PowerMonkPatientDefense &&
                definition != PowerMonkPatientDefenseSurvival3 &&
                definition != PowerMonkPatientDefenseSurvival6)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var dieType = rulesetCharacter.GetMonkDieType();
            var tempHp = rulesetCharacter.RollDiceAndSum(dieType, RollContext.HealValueRoll, 2, []);

            rulesetCharacter.ReceiveTemporaryHitPoints(
                tempHp, DurationType.Round, 1, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return Main.Settings.EnableMonkHeightenedMetabolism &&
                   character.GetClassLevel(Monk) >= 10 &&
                   (definition == PowerMonkFlurryOfBlows ||
                    definition == PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement ||
                    definition == PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement);
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (definition == PowerMonkFlurryOfBlows)
            {
                effectDescription.EffectForms.TryAdd(_effectForm);
            }
            else if (definition == PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement)
            {
                effectDescription.EffectForms.TryAdd(_effectForm);
            }
            else if (definition == PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement)
            {
                effectDescription.EffectForms.TryAdd(_effectFormFreedom);
            }

            return effectDescription;
        }
    }

    private sealed class CustomLevelUpLogicMonkBodyAndMind : ICustomLevelUpLogic
    {
        public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            hero.ModifyAttributeAndMax(AttributeDefinitions.Dexterity, 4);
            hero.ModifyAttributeAndMax(AttributeDefinitions.Wisdom, 4);
            hero.RefreshAll();
        }

        public void RemoveFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            hero.ModifyAttributeAndMax(AttributeDefinitions.Dexterity, -4);
            hero.ModifyAttributeAndMax(AttributeDefinitions.Wisdom, -4);
            hero.RefreshAll();
        }
    }

    private static readonly FeatureDefinition FeatureMonkHeightenedMetabolism = FeatureDefinitionBuilder
        .Create("FeatureMonkHeightenedMetabolism")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(
            new CustomBehaviorHeightenedMetabolism(
                ConditionDefinitionBuilder
                    .Create("ConditionMonkFlurryOfBlowsHeightenedMetabolism")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetFeatures(
                        FeatureDefinitionAttackModifierBuilder
                            .Create(AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonus,
                                "AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusHeightenedMetabolism")
                            .SetUnarmedStrike(3)
                            .AddToDB())
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionMonkFlurryOfBlowsFreedomHeightenedMetabolism")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetFeatures(
                        FeatureDefinitionAttackModifierBuilder
                            .Create(AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedom,
                                "AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedomHeightenedMetabolism")
                            .SetUnarmedStrike(4)
                            .AddToDB())
                    .AddToDB()))
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkStepOfTheWindHeightenedMetabolism =
        FeatureDefinitionPowerBuilder
            .Create("PowerMonkStepOfTheWindHeightenedMetabolism")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerStepOfTheWind", Resources.PowerStepOfTheWind, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerMonkStepOfTheWindDash)
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .AddEffectForms(PowerMonkStepOftheWindDisengage.EffectDescription.EffectForms[0])
                    .SetCasterEffectParameters(PowerOathOfTirmarGoldenSpeech)
                    .Build())
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkSuperiorDefense = FeatureDefinitionPowerBuilder
        .Create("PowerMonkSuperiorDefense")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerMonkSuperiorDefense", Resources.PowerMonkSuperiorDefense, 256, 128))
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 3, 3)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create("ConditionMonkSuperiorDefense")
                                .SetGuiPresentation(Category.Condition, ConditionAuraOfProtection)
                                .SetPossessive()
                                .AddFeatures(
                                    DamageAffinityAcidResistance,
                                    DamageAffinityBludgeoningResistanceTrue,
                                    DamageAffinityColdResistance,
                                    DamageAffinityFireResistance,
                                    DamageAffinityLightningResistance,
                                    DamageAffinityNecroticResistance,
                                    DamageAffinityPiercingResistanceTrue,
                                    DamageAffinityPoisonResistance,
                                    DamageAffinityPsychicResistance,
                                    DamageAffinityRadiantResistance,
                                    DamageAffinitySlashingResistanceTrue,
                                    DamageAffinityThunderResistance)
                                .SetConditionParticleReference(ConditionHolyAura)
                                .SetCancellingConditions(
                                    DatabaseRepository.GetDatabase<ConditionDefinition>().Where(x =>
                                        x.IsSubtypeOf(RuleDefinitions.ConditionIncapacitated)).ToArray())
                                .AddCancellingConditions(ConditionCharmedByHypnoticPattern)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .SetCasterEffectParameters(PowerOathOfTirmarGoldenSpeech)
                .Build())
        .AddToDB();

    private static readonly FeatureDefinition FeatureMonkBodyAndMind = FeatureDefinitionBuilder
        .Create("FeatureMonkBodyAndMind")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new CustomLevelUpLogicMonkBodyAndMind())
        .AddToDB();

    private static void LoadMonkHeightenedMetabolism()
    {
        var validatePower = new ValidatorsValidatePowerUse(c =>
            !Main.Settings.EnableMonkHeightenedMetabolism || c.GetClassLevel(Monk) < 10);

        PowerMonkStepOfTheWindDash.AddCustomSubFeatures(validatePower);
        PowerMonkStepOftheWindDisengage.AddCustomSubFeatures(validatePower);
    }

    internal static void SwitchMonkDoNotRequireAttackActionForFlurry()
    {
        FeatureSetMonkFlurryOfBlows.GuiPresentation.description =
            Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry
                ? "Feature/&FeatureSetAlternateMonkFlurryOfBlowsDescription"
                : "Feature/&FeatureSetMonkFlurryOfBlowsDescription";
    }

    private static readonly List<DieTypeByRank> MonkUnarmedDieTypeByRank =
        [.. AttackModifierMonkMartialArtsImprovedDamage.DieTypeByRankTable];

    private static readonly List<DieTypeByRank> MonkUnarmedDieTypeByRank2024 =
    [
        new() { dieType = DieType.D6, rank = 1 },
        new() { dieType = DieType.D6, rank = 2 },
        new() { dieType = DieType.D6, rank = 3 },
        new() { dieType = DieType.D6, rank = 4 },
        new() { dieType = DieType.D8, rank = 5 },
        new() { dieType = DieType.D8, rank = 6 },
        new() { dieType = DieType.D8, rank = 7 },
        new() { dieType = DieType.D8, rank = 8 },
        new() { dieType = DieType.D8, rank = 9 },
        new() { dieType = DieType.D8, rank = 10 },
        new() { dieType = DieType.D10, rank = 11 },
        new() { dieType = DieType.D10, rank = 12 },
        new() { dieType = DieType.D10, rank = 13 },
        new() { dieType = DieType.D10, rank = 14 },
        new() { dieType = DieType.D10, rank = 15 },
        new() { dieType = DieType.D10, rank = 16 },
        new() { dieType = DieType.D12, rank = 17 },
        new() { dieType = DieType.D12, rank = 18 },
        new() { dieType = DieType.D12, rank = 19 },
        new() { dieType = DieType.D12, rank = 20 }
    ];

    internal static void SwitchOneDndMonkUnarmedDieTypeProgression()
    {
        AttackModifierMonkMartialArtsImprovedDamage.dieTypeByRankTable =
            Main.Settings.SwapMonkToUseOneDndUnarmedDieTypeProgression
                ? MonkUnarmedDieTypeByRank2024
                : MonkUnarmedDieTypeByRank;

        AttackModifierMonkMartialArtsImprovedDamage.GuiPresentation.Description =
            Main.Settings.SwapMonkToUseOneDndUnarmedDieTypeProgression
                ? "Feature/&AttackModifierMonkMartialArtsExtendedDescription"
                : "Feature/&AttackModifierMonkMartialArtsDescription";
    }

    internal static void SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack()
    {
        if (Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack)
        {
            PowerMonkMartialArts.GuiPresentation.description =
                "Feature/&AttackModifierMonkMartialArtsUnarmedStrikeBonusDescription";
            PowerMonkMartialArts.GuiPresentation.title =
                "Feature/&AttackModifierMonkMartialArtsUnarmedStrikeBonusTitle";
            PowerMonkMartialArts.GuiPresentation.hidden = true;
            PowerMonkMartialArts.activationTime = ActivationTime.NoCost;
        }
        else
        {
            PowerMonkMartialArts.GuiPresentation.description = "Action/&MartialArtsDescription";
            PowerMonkMartialArts.GuiPresentation.title = "Action/&MartialArtsTitle";
            PowerMonkMartialArts.GuiPresentation.hidden = false;
            PowerMonkMartialArts.activationTime = ActivationTime.OnAttackHitMartialArts;
        }

        if (Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack)
        {
            Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchMonkHeightenedMetabolism()
    {
        Monk.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureMonkHeightenedMetabolism ||
            x.FeatureDefinition == PowerMonkStepOfTheWindHeightenedMetabolism);

        if (Main.Settings.EnableMonkHeightenedMetabolism)
        {
            Monk.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureMonkHeightenedMetabolism, 10),
                new FeatureUnlockByLevel(PowerMonkStepOfTheWindHeightenedMetabolism, 10));
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkSuperiorDefenseToReplaceEmptyBody()
    {
        Monk.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == Level20Context.PowerMonkEmptyBody ||
            x.FeatureDefinition == PowerMonkSuperiorDefense);

        Monk.FeatureUnlocks.Add(
            Main.Settings.EnableMonkSuperiorDefenseToReplaceEmptyBody
                ? new FeatureUnlockByLevel(PowerMonkSuperiorDefense, 18)
                : new FeatureUnlockByLevel(Level20Context.PowerMonkEmptyBody, 18));

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkBodyAndMindToReplacePerfectSelf()
    {
        Monk.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == Level20Context.FeatureMonkPerfectSelf ||
            x.FeatureDefinition == FeatureMonkBodyAndMind);

        Monk.FeatureUnlocks.Add(
            Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf
                ? new FeatureUnlockByLevel(FeatureMonkBodyAndMind, 20)
                : new FeatureUnlockByLevel(Level20Context.FeatureMonkPerfectSelf, 20));

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }
}
