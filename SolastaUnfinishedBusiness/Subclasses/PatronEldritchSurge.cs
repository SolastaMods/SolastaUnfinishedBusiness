using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.Builders.EldritchVersatilityBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public class PatronEldritchSurge : AbstractSubclass
{
    public const string Name = "PatronEldritchSurge";

    // LEVEL 01 Blast Exclusive
    private static readonly FeatureDefinitionBonusCantrips BonusCantripBlastExclusive =
        FeatureDefinitionBonusCantripsBuilder
            .Create($"BonusCantrips{Name}BlastExclusive")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(EldritchBlast)
            .AddCustomSubFeatures(new ModifyEffectDescriptionEldritchBlast())
            .AddToDB();

    // LEVEL 01 Versatility Switch
    public static readonly FeatureDefinitionPower PowerVersatilitySwitch = BuildVersatilitySwitch();

    // LEVEL 06 Blast Pursuit
    public static readonly FeatureDefinition FeatureBlastPursuit = FeatureDefinitionBuilder
        .Create($"Feature{Name}BlastPursuit")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new OnReducedToZeroHpByMeBlastPursuit())
        .AddToDB();

    // LEVEL 10 Blast Reload
    public static readonly FeatureDefinitionPower FeatureBlastReload = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}BlastReload")
        .SetGuiPresentation(Category.Feature)
        .SetUsesFixed(ActivationTime.Permanent)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            BlastReloadSupportRulesetCondition.BindingDefinition,
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
        .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
        .AddToDB();

    public PatronEldritchSurge()
    {
        var builder = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronEldritchSurge, 256))
            .AddFeaturesAtLevel(1,
                BonusCantripBlastExclusive,
                Learn2Versatility,
                PowerEldritchVersatilityPointPool,
                PowerVersatilitySwitch)
            .AddFeaturesAtLevel(6,
                FeatureBlastPursuit,
                Learn1Versatility)
            .AddFeaturesAtLevel(10,
                FeatureBlastReload,
                Learn1Versatility)
            .AddFeaturesAtLevel(14,
                PowerBlastOverload,
                Learn1Versatility);

        for (var i = 2; i <= 20; i++)
        {
            builder.AddFeaturesAtLevel(i, UnLearn1Versatility);
        }

        BuildVersatilities();
        Subclass = builder.AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPower BuildVersatilitySwitch()
    {
        var pool = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}VersatilitySwitchPool")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("VersatilitySwitch", Resources.VersatilitySwitch, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
            .AddCustomSubFeatures(new RechargePoolWhenBattleEnd())
            .AddToDB();

        var powerVersatilitySwitchStr =
            FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{Name}VersatilitySwitchStr")
                .SetGuiPresentation(Category.Feature)
                .SetSharedPool(ActivationTime.NoCost, pool)
                .AddCustomSubFeatures(new VersatilitySwitchCustom(AttributeDefinitions.Strength))
                .AddToDB();

        var powerVersatilitySwitchInt =
            FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{Name}VersatilitySwitchInt")
                .SetGuiPresentation(Category.Feature)
                .SetSharedPool(ActivationTime.NoCost, pool)
                .AddCustomSubFeatures(new VersatilitySwitchCustom(AttributeDefinitions.Intelligence))
                .AddToDB();

        var powerVersatilitySwitchWis =
            FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{Name}VersatilitySwitchWis")
                .SetGuiPresentation(Category.Feature)
                .SetSharedPool(ActivationTime.NoCost, pool)
                .AddCustomSubFeatures(new VersatilitySwitchCustom(AttributeDefinitions.Wisdom))
                .AddToDB();

        var powerVersatilitySwitchNone =
            FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{Name}VersatilitySwitchNone")
                .SetGuiPresentation(Category.Feature)
                .SetSharedPool(ActivationTime.NoCost, pool)
                .AddCustomSubFeatures(new VersatilitySwitchCustom(string.Empty))
                .AddToDB();

        PowerBundle.RegisterPowerBundle(pool, false,
            powerVersatilitySwitchStr,
            powerVersatilitySwitchInt,
            powerVersatilitySwitchWis,
            powerVersatilitySwitchNone);

        return pool;
    }

    public static bool IsEldritchBlast(RulesetEffect rulesetEffect)
    {
        return rulesetEffect is RulesetEffectSpell rulesetEffectSpell
               && rulesetEffectSpell.SpellDefinition == EldritchBlast;
    }

    private class VersatilitySwitchCustom(string replacedAbilityScore) : IPowerOrSpellFinishedByMe
    {
        private string ReplacedAbilityScore { get; } = replacedAbilityScore;

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.GetVersatilitySupportCondition(out var supportCondition);
            supportCondition.ReplacedAbilityScore = ReplacedAbilityScore;
            supportCondition.ModifyAttributeScores(rulesetCharacter.GetOriginalHero(), ReplacedAbilityScore);

            // Auto recharge out of combat
            if (Gui.Battle is null)
            {
                rulesetCharacter.GetOriginalHero()!.UsablePowers.DoIf(x =>
                        x.PowerDefinition == PowerVersatilitySwitch,
                    y => y.Recharge());
            }

            yield break;
        }
    }

    public sealed class ModifyEffectDescriptionEldritchBlast : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == EldritchBlast
                   && character.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name) > 0;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter rulesetCharacter,
            RulesetEffect rulesetEffect)
        {
            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero == null)
            {
                return effectDescription;
            }

            var totalLevel = rulesetHero.classesHistory.Count;
            var warlockClassLevel = rulesetHero.GetClassLevel(CharacterClassDefinitions.Warlock);
            var additionalBeamCount = ComputeAdditionalBeamCount(totalLevel, warlockClassLevel);

            effectDescription.effectAdvancement.Clear();
            effectDescription.targetParameter = 1 + additionalBeamCount;

            return effectDescription;
        }

        public static int ComputeAdditionalBeamCount(int totalLevel, int warlockClassLevel)
        {
            var determinantLevel = warlockClassLevel - (2 * (totalLevel - warlockClassLevel));
            var increaseLevels = new[] { 3, 8, 13, 18 };

            return increaseLevels.Count(level => determinantLevel >= level);
        }
    }

    private sealed class OnReducedToZeroHpByMeBlastPursuit : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!IsEldritchBlast(activeEffect)
                || !rulesetAttacker.GetVersatilitySupportCondition(out var supportCondition))
            {
                yield break;
            }

            supportCondition.TryEarnOrSpendPoints(
                PointAction.Modify, PointUsage.EarnPoints, supportCondition.BeamNumber);
        }
    }

    internal interface IQualifySpellToRepertoireLine
    {
        void QualifySpells(RulesetCharacter character, SpellRepertoireLine line, IEnumerable<SpellDefinition> spells);
    }

    private sealed class BlastReloadCustom :
        IMagicEffectFinishedByMeAny, ICharacterTurnStartListener, IQualifySpellToRepertoireLine
    {
        public void OnCharacterTurnStarted(GameLocationCharacter gameLocationCharacter)
        {
            // clean up cantrips list on every turn start
            // combat condition will be removed automatically after combat
            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            if (!BlastReloadSupportRulesetCondition.GetCustomConditionFromCharacter(
                    rulesetCharacter, out var supportCondition))
            {
                return;
            }

            supportCondition.CantripsUsedThisTurn.Clear();
        }

        public IEnumerator OnMagicEffectFinishedByMeAny(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var actionParams = action.ActionParams;
            var rulesetCharacter = attacker.RulesetCharacter;

            // only collect cantrips
            if (Gui.Battle == null ||
                actionParams.activeEffect is not RulesetEffectSpell rulesetEffectSpell ||
                rulesetEffectSpell.SpellDefinition.SpellLevel != 0)
            {
                yield break;
            }

            if (!BlastReloadSupportRulesetCondition.GetCustomConditionFromCharacter(
                    rulesetCharacter, out var supportCondition))
            {
                yield break;
            }

            supportCondition.CantripsUsedThisTurn.TryAdd(rulesetEffectSpell.SpellDefinition);
        }

        public void QualifySpells(
            RulesetCharacter rulesetCharacter,
            SpellRepertoireLine spellRepertoireLine,
            IEnumerable<SpellDefinition> spells)
        {
            // _cantripsUsedThisTurn only has entries for Eldritch Surge of at least level 14
            if (spellRepertoireLine.actionType != ActionType.Bonus
                || !BlastReloadSupportRulesetCondition.GetCustomConditionFromCharacter(
                    rulesetCharacter, out var supportCondition))
            {
                return;
            }

            spellRepertoireLine.relevantSpells.AddRange(supportCondition.CantripsUsedThisTurn.Intersect(spells));
        }
    }

    private class BlastReloadSupportRulesetCondition :
        RulesetConditionCustom<BlastReloadSupportRulesetCondition>, IBindToRulesetConditionCustom
    {
        public readonly List<SpellDefinition> CantripsUsedThisTurn = [];

        static BlastReloadSupportRulesetCondition()
        {
            Category = AttributeDefinitions.TagEffect;
            Marker = new BlastReloadSupportRulesetCondition();
            BindingDefinition = ConditionDefinitionBuilder
                .Create($"Condition{PatronEldritchSurge.Name}BlastReloadSupport")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .AddCustomSubFeatures(Marker, new BlastReloadCustom())
                .AddToDB();
        }

        public void ReplaceRulesetCondition(
            RulesetCondition originalRulesetCondition, out RulesetCondition replacedRulesetCondition)
        {
            replacedRulesetCondition = GetFromPoolAndCopyOriginalRulesetCondition(originalRulesetCondition);
        }

        public override void SerializeElements(IElementsSerializer serializer, IVersionProvider versionProvider)
        {
            base.SerializeElements(serializer, versionProvider);

            try
            {
                BaseDefinition.SerializeDatabaseReferenceList(
                    serializer, "CantripsUsedThisTurn", "SpellDefinition", CantripsUsedThisTurn);

                if (serializer.Mode == Serializer.SerializationMode.Read)
                {
                    CantripsUsedThisTurn.RemoveAll(x => x is null);
                }
            }
            catch (Exception ex)
            {
                Trace.LogException(
                    new Exception("Error with EldritchSurgeSupportCondition serialization" + ex.Message, ex));
            }
        }

        protected override void ClearCustomStates()
        {
            CantripsUsedThisTurn.Clear();
        }
    }

    private class RechargePoolWhenBattleEnd : ICharacterBattleEndedListener
    {
        public void OnCharacterBattleEnded(GameLocationCharacter locationCharacter)
        {
            locationCharacter.RulesetCharacter.GetOriginalHero()!.UsablePowers.DoIf(x =>
                    x.PowerDefinition == PowerVersatilitySwitch,
                y => y.Recharge());
        }
    }
}
