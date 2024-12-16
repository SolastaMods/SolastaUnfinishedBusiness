using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private static readonly FeatureDefinitionFeatureSet FeatureSetRangerDeftExplorer =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRangerDeftExplorer")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolRangerDeftExplorerLanguages")
                    .SetGuiPresentationNoContent(true)
                    .SetPool(HeroDefinitions.PointsPoolType.Language, 2)
                    .AddToDB(),
                FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolRangerDeftExplorerSkills")
                    .SetGuiPresentationNoContent(true)
                    .SetPool(HeroDefinitions.PointsPoolType.Expertise, 1)
                    .AddCustomSubFeatures(new TryAlterOutcomeAttributeCheckPrimalKnowledge())
                    .AddToDB())
            .AddToDB();

    private static readonly FeatureDefinitionMovementAffinity MovementAffinityRangerRoving =
        FeatureDefinitionMovementAffinityBuilder
            .Create("MovementAffinityRangerRoving")
            .SetGuiPresentation(Category.Feature)
            .SetBaseSpeedAdditiveModifier(2)
            .SetClimbing(true)
            .AddToDB();

    private static readonly FeatureDefinitionPointPool PointPoolRangerExpertise = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolRangerExpertise")
        .SetGuiPresentation(Category.Feature)
        .SetPool(HeroDefinitions.PointsPoolType.Expertise, 2)
        .AddToDB();

    private static readonly FeatureDefinitionCombatAffinity CombatAffinityRangerPreciseHunter =
        FeatureDefinitionCombatAffinityBuilder
            .Create("CombatAffinityRangerPreciseHunter")
            .SetGuiPresentation(Category.Feature)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(SituationalContext.TargetHasCondition, ConditionDefinitions.ConditionMarkedByHunter)
            .DisableAutoFormatDescription()
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerRangerTireless = FeatureDefinitionPowerBuilder
        .Create("PowerRangerTireless")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerRangerTireless", Resources.PowerTireless, 256, 128))
        .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
        .SetExplicitAbilityScore(AttributeDefinitions.Wisdom)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.UntilAnyRest)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetTempHpForm(5, DieType.D8, 1)
                        .Build())
                .SetCasterEffectParameters(Command)
                .Build())
        .AddCustomSubFeatures(new CustomBehaviorTireless())
        .AddToDB();

    private static readonly FeatureDefinition FeatureRangerRelentlessHunter = FeatureDefinitionBuilder
        .Create("FeatureRangerRelentlessHunter")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new CustomBehaviorRelentlessHunter())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerFavoredEnemyHuntersMark = FeatureDefinitionPowerBuilder
        .Create("PowerFavoredEnemyHuntersMark")
        .SetGuiPresentation(HuntersMark.GuiPresentation)
        .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 1, 0)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create(HuntersMark)
                .SetEffectForms()
                .Build())
        .AddCustomSubFeatures(
            HasModifiedUses.Marker,
            new CustomBehaviorFavoredEnemyHuntersMark())
        .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetRangerFavoredEnemy =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRangerFavoredEnemy")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                PowerFavoredEnemyHuntersMark,
                FeatureDefinitionAutoPreparedSpellsBuilder
                    .Create("AutoPreparedHuntersMark")
                    .SetGuiPresentationNoContent(true)
                    .SetAutoTag("Ranger")
                    .SetSpellcastingClass(Ranger)
                    .SetPreparedSpellGroups(BuildSpellGroup(1, HuntersMark))
                    .AddToDB())
            .AddToDB();

    internal static void SwitchRangerSpellCastingAtOne()
    {
        var level = Main.Settings.EnableRangerSpellCastingAtLevel1 ? 1 : 2;

        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellRanger))
        {
            featureUnlock.level = level;
        }

        // allows back and forth compatibility with EnableRitualOnAllCasters2024
        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureSetClericRitualCasting))
        {
            featureUnlock.level = level;
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        if (Main.Settings.EnableRangerSpellCastingAtLevel1)
        {
            FeatureDefinitionCastSpells.CastSpellRanger.slotsPerLevels = SharedSpellsContext.HalfRoundUpCastingSlots;
            SharedSpellsContext.ClassCasterType[RangerClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp;
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellRanger.slotsPerLevels = SharedSpellsContext.HalfCastingSlots;
            SharedSpellsContext.ClassCasterType[RangerClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.Half;
        }
    }

    internal static void SwitchRangerDeftExplorer()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == FeatureSetRangerDeftExplorer);

        if (Main.Settings.EnableRangerDeftExplorer2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetRangerDeftExplorer, 2));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerExpertise()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == PointPoolRangerExpertise);

        if (Main.Settings.EnableRangerExpertise2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(PointPoolRangerExpertise, 9));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerFavoredEnemy()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == FeatureSetRangerFavoredEnemy ||
                x.FeatureDefinition == RulesContext.InvocationPoolRangerTerrainType ||
                x.FeatureDefinition == RulesContext.InvocationPoolRangerPreferredEnemy);

        if (Main.Settings.EnableRangerFavoredEnemy2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetRangerFavoredEnemy, 1));
        }
        else
        {
            Ranger.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(RulesContext.InvocationPoolRangerTerrainType, 1),
                new FeatureUnlockByLevel(RulesContext.InvocationPoolRangerPreferredEnemy, 1),
                new FeatureUnlockByLevel(RulesContext.InvocationPoolRangerTerrainType, 6),
                new FeatureUnlockByLevel(RulesContext.InvocationPoolRangerPreferredEnemy, 6),
                new FeatureUnlockByLevel(RulesContext.InvocationPoolRangerTerrainType, 10),
                new FeatureUnlockByLevel(RulesContext.InvocationPoolRangerPreferredEnemy, 14));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerFeralSenses()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == Level20Context.SenseRangerFeralSenses ||
                x.FeatureDefinition == Level20Context.SenseRangerFeralSenses2024);

        Ranger.FeatureUnlocks.Add(Main.Settings.EnableRangerFeralSenses2024
            ? new FeatureUnlockByLevel(Level20Context.SenseRangerFeralSenses2024, 18)
            : new FeatureUnlockByLevel(Level20Context.SenseRangerFeralSenses, 18));

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerFoeSlayers()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == Level20Context.FeatureFoeSlayer ||
                x.FeatureDefinition == Level20Context.FeatureFoeSlayer2024);

        Ranger.FeatureUnlocks.Add(Main.Settings.EnableRangerFoeSlayers2024
            ? new FeatureUnlockByLevel(Level20Context.FeatureFoeSlayer2024, 20)
            : new FeatureUnlockByLevel(Level20Context.FeatureFoeSlayer, 20));

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerNatureShroud()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == FeatureDefinitionPowerNatureShroud);

        if (Main.Settings.EnableRangerNatureShroud2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureDefinitionPowerNatureShroud, 14));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerVanish()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == ActionAffinityRangerVanish);

        if (!Main.Settings.RemoveRangerVanish2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(ActionAffinityRangerVanish, 14));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerPreciseHunter()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == CombatAffinityRangerPreciseHunter);

        if (Main.Settings.EnableRangerPreciseHunter2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(CombatAffinityRangerPreciseHunter, 17));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerRelentlessHunter()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == FeatureRangerRelentlessHunter);

        if (Main.Settings.EnableRangerRelentlessHunter2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureRangerRelentlessHunter, 13));
        }

        FeatureRangerRelentlessHunter.GuiPresentation.description =
            Main.Settings.EnableRangerRelentlessHunter2024AsNoConcentration
                ? "Feature/&FeatureRangerRelentlessHunterExtendedDescription"
                : "Feature/&FeatureRangerRelentlessHunterDescription";

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerRoving()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == MovementAffinityRangerRoving);

        if (Main.Settings.EnableRangerRoving2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(MovementAffinityRangerRoving, 6));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerTireless()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == PowerRangerHideInPlainSight ||
                x.FeatureDefinition == PowerRangerTireless);

        Ranger.FeatureUnlocks.Add(Main.Settings.EnableRangerTireless2024
            ? new FeatureUnlockByLevel(PowerRangerTireless, 10)
            : new FeatureUnlockByLevel(PowerRangerHideInPlainSight, 10));

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private sealed class CustomBehaviorTireless : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == PowerRangerTireless;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var levels = character.GetClassLevel(Ranger);
            var halfRoundUp = (levels + 1) / 2;

            effectDescription.EffectForms[0].TemporaryHitPointsForm.BonusHitPoints = halfRoundUp;

            return effectDescription;
        }
    }

    private sealed class CustomBehaviorFavoredEnemyHuntersMark : IPowerOrSpellFinishedByMe, IModifyPowerPoolAmount
    {
        public FeatureDefinitionPower PowerPool => PowerFavoredEnemyHuntersMark;

        public int PoolChangeAmount(RulesetCharacter character)
        {
            var levels = character.GetClassLevel(Ranger);
            var usages = levels switch
            {
                < 5 => 2,
                < 9 => 3,
                < 13 => 4,
                < 17 => 5,
                _ => 6
            };

            return usages;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActingCharacter.MyExecuteActionCastNoCost(HuntersMark, 0, action.ActionParams);

            yield break;
        }
    }

    private sealed class CustomBehaviorRelentlessHunter
        : IPreventRemoveConcentrationOnDamage, IModifyConcentrationRequirement
    {
        public bool RequiresConcentration(RulesetCharacter rulesetCharacter, RulesetEffectSpell rulesetEffectSpell)
        {
            if (!Main.Settings.EnableRangerRelentlessHunter2024)
            {
                return rulesetEffectSpell.SpellDefinition.RequiresConcentration;
            }

            return rulesetEffectSpell.SpellDefinition != HuntersMark;
        }

        public SpellDefinition[] SpellsThatShouldNotRollConcentrationCheckFromDamage(RulesetCharacter rulesetCharacter)
        {
            return Main.Settings.EnableRangerRelentlessHunter2024AsNoConcentration ? [] : [HuntersMark];
        }
    }
}
