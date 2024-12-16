using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.PatronEldritchSurge;

namespace SolastaUnfinishedBusiness.Subclasses.Builders;

internal static class EldritchVersatilityBuilders
{
    public enum PointAction
    {
        Modify,
        Require
    }

    public enum PointUsage
    {
        EarnPoints,
        EldritchAegis,
        EldritchWard,
        BattlefieldConversionSuccess,
        BattlefieldConversionFailure,
        BlastBreakthrough,
        BlastEmpower
    }

    private const string Name = "EldritchVersatility";

    public static readonly FeatureDefinitionPower PowerEldritchVersatilityPointPool = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}PointPool")
        .SetGuiPresentation(Category.Feature)
        .SetUsesFixed(ActivationTime.Permanent)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder.ConditionForm(VersatilitySupportRulesetCondition.BindingDefinition))
                .Build())
        .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, RestrictEffectToNotTerminateWhileUnconscious.Marker)
        .AddToDB();

    public static readonly FeatureDefinitionCustomInvocationPool Learn1Versatility =
        CustomInvocationPoolDefinitionBuilder
            .Create($"Feature{Name}Learn1")
            .SetGuiPresentation(Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.EldritchVersatilityPool)
            .AddToDB();

    public static readonly FeatureDefinitionCustomInvocationPool Learn2Versatility =
        CustomInvocationPoolDefinitionBuilder
            .Create($"Feature{Name}Learn2")
            .SetGuiPresentation(Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.EldritchVersatilityPool, 2)
            .AddToDB();

    public static readonly FeatureDefinitionCustomInvocationPool UnLearn1Versatility =
        CustomInvocationPoolDefinitionBuilder
            .Create($"Feature{Name}UnLearn1")
            .SetGuiPresentationNoContent(true)
            .Setup(InvocationPoolTypeCustom.Pools.EldritchVersatilityPool, 1, true)
            .AddToDB();

    private static readonly FeatureDefinition FeatureEldritchVersatilityGrantPoolAndSwitch =
        FeatureDefinitionBuilder
            .Create($"Feature{Name}GrantPoolAndSwitch")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new EldritchVersatilityAdeptCustom())
            .AddToDB();

    public static readonly FeatDefinition FeatEldritchVersatilityAdept = FeatDefinitionBuilder
        .Create($"Feat{Name}Adept")
        .SetGuiPresentation(Category.Feat, hidden: true)
        .AddFeatures(Learn1Versatility, FeatureEldritchVersatilityGrantPoolAndSwitch)
        .AddToDB();

    private static readonly ConditionDefinition ConditionEldritchSurgeBlastOverload = ConditionDefinitionBuilder
        .Create($"Condition{PatronEldritchSurge.Name}BlastOverload")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRaging)
        .SetFeatures(
            FeatureDefinitionAdditionalActionBuilder
                .Create($"AdditionalAction{PatronEldritchSurge.Name}BlastOverload")
                .SetGuiPresentationNoContent(true)
                .SetActionType(ActionType.Main)
                .AddToDB())
        .AddCustomSubFeatures(new OnConditionAddedOrRemovedBlastOverload())
        .CopyParticleReferences(ConditionDefinitions.ConditionRaging)
        .AddToDB();

    public static readonly FeatureDefinitionPower PowerBlastOverload = FeatureDefinitionPowerBuilder
        .Create($"Power{PatronEldritchSurge.Name}BlastOverload")
        .SetGuiPresentation(Category.Feature, Sprites.GetSprite("BlastOverload", Resources.BlastOverload, 128))
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionEldritchSurgeBlastOverload))
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBarbarianRageStart)
                .Build())
        .AddToDB();

    internal static void BuildVersatilities()
    {
        #region Strength Power

        var name = "BlastEmpower";
        var sprite = Sprites.GetSprite(name, Resources.BlastEmpower, 128);

        FeatureDefinition featureOrPower = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageInvocationAgonizingBlast, $"Feature{Name}{name}")
            .SetNotificationTag("BlastEmpower")
            .SetDamageValueDetermination(ExtraAdditionalDamageValueDetermination.CustomModifier)
            .AddCustomSubFeatures(
                new CustomModifierProvider(hero =>
                {
                    hero.GetVersatilitySupportCondition(out var supportCondition);
                    return supportCondition is null
                        ? 0
                        : GetAbilityScoreModifier(hero, AttributeDefinitions.Strength, supportCondition);
                }),
                new BlastEmpowerCustom($"Invocation{Name}{name}"))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, featureOrPower, new BlastEmpowerActiveSwitch($"Invocation{Name}{name}"));

        name = "BlastBreakthrough";
        sprite = Sprites.GetSprite(name, Resources.BlastBreakthrough, 128);
        featureOrPower = FeatureDefinitionBuilder
            .Create($"Feature{Name}{name}")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new BlastBreakthroughCustom($"Invocation{Name}{name}"))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, featureOrPower);

        #endregion

        #region Intelligence Power

        name = "BattlefieldShorthand";
        sprite = Sprites.GetSprite(name, Resources.BattlefieldShorthand, 128);
        featureOrPower = FeatureDefinitionBuilder
            .Create($"Feature{Name}{name}")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new BattlefieldShorthandCopy())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, featureOrPower, new BattlefieldShorthandRemoveCopied());

        name = "BattlefieldConversion";
        sprite = Sprites.GetSprite(name, Resources.BattlefieldConversion, 128);
        featureOrPower = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{name}")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(SpellDefinitions.Haste)
                    .Build())
            .AddCustomSubFeatures(
                ModifyPowerFromInvocation.Marker,
                new BattlefieldConversionRestoreSlot())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, featureOrPower);

        #endregion

        #region Wisdom Power

        name = "EldritchAegis";
        sprite = Sprites.GetSprite(name, Resources.EldritchAegis, 128);
        featureOrPower = FeatureDefinitionBuilder
            .Create($"Feature{Name}{name}")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new EldritchAegisTwistHit())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, featureOrPower);

        name = "EldritchWard";
        sprite = Sprites.GetSprite(name, Resources.EldritchWard, 128);
        featureOrPower = FeatureDefinitionBuilder
            .Create($"Feature{Name}{name}")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new EldritchWardAidSave())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, featureOrPower);

        #endregion
    }

    public static bool GetVersatilitySupportCondition(
        this RulesetCharacter rulesetCharacter,
        out VersatilitySupportRulesetCondition supportCondition)
    {
        return VersatilitySupportRulesetCondition.GetCustomConditionFromCharacter(
            rulesetCharacter, out supportCondition);
    }

    private static void BuildFeatureInvocation(
        string name,
        AssetReferenceSprite sprite,
        FeatureDefinition feature,
        params object[] subFeatures)
    {
        _ = CustomInvocationDefinitionBuilder
            .Create($"Invocation{Name}{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetPoolType(InvocationPoolTypeCustom.Pools.EldritchVersatilityPool)
            .SetGrantedFeature(feature)
            .SetRequirements(0)
            .AddCustomSubFeatures(subFeatures)
            .AddToDB();
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static int GetAbilityScoreModifier(
        RulesetCharacter ownerCharacter,
        string abilityScore,
        VersatilitySupportRulesetCondition supportCondition)
    {
        return AttributeDefinitions.ComputeAbilityScoreModifier(Math.Max(Math.Max(
                ownerCharacter.TryGetAttributeValue(abilityScore),
                supportCondition.ReplacedAbilityScore == abilityScore ? supportCondition.ReplacedAbilityScoreValue : 0),
            supportCondition.IsOverload ? 22 : 0));
    }

    private static void InflictCondition(
        BaseDefinition condition,
        RulesetCharacter sourceCharacter,
        RulesetActor targetCharacter)
    {
        targetCharacter.InflictCondition(
            condition.Name,
            DurationType.Round,
            1,
            TurnOccurenceType.StartOfTurn,
            AttributeDefinitions.TagEffect,
            sourceCharacter.guid,
            sourceCharacter.CurrentFaction.Name,
            1,
            condition.Name,
            0,
            0,
            0);
    }

    private static bool IsInvocationActive(
        RulesetCharacter featureOwner, string invocationName, out RulesetInvocation rulesetInvocation)
    {
        foreach (var invocation in featureOwner.Invocations.Where(invocation =>
                     invocation.Active && invocation.invocationDefinition.Name == invocationName))
        {
            rulesetInvocation = invocation;

            return true;
        }

        rulesetInvocation = null;

        return false;
    }

    internal class VersatilitySupportRulesetCondition
        : RulesetConditionCustom<VersatilitySupportRulesetCondition>, IBindToRulesetConditionCustom
    {
        private static readonly int[] ProficiencyIncreaseLevels = [1, 5, 11, 17];

        static VersatilitySupportRulesetCondition()
        {
            Category = "EldritchVersatility";
            Marker = new VersatilitySupportRulesetCondition();
            BindingDefinition = ConditionDefinitionBuilder
                .Create("ConditionEldritchVersatility")
                .SetGuiPresentationNoContent(true)
                .SetSpecialInterruptions(ConditionInterruption.LevelUp, ConditionInterruption.LongRest)
                .AddCustomSubFeatures(
                    Marker,
                    new OnConditionAddedOrRemovedVersatility())
                .SetSilent(Silent.WhenAddedOrRemoved)
                .AddToDB();
        }

        public List<SpellDefinition> CopiedSpells { get; } = [];
        public int CurrentPoints { get; private set; }
        public bool IsValidBlastBreakthrough { get; set; }
        public int CreateSlotDC { get; private set; }
        public int MaxPoints { get; private set; }
        public string ReplacedAbilityScore { get; set; }
        public List<string> StrPowerPriority { get; private set; } = [];
        public int ReplacedAbilityScoreValue { get; private set; }
        private int SlotLevel { get; set; }
        public int BeamNumber { get; private set; }
        private int LearntAmount { get; set; }
        public bool IsOverload { get; set; }
        private bool HasBlastPursuit { get; set; }

        public void ReplaceRulesetCondition(
            RulesetCondition originalRulesetCondition, out RulesetCondition replacedRulesetCondition)
        {
            replacedRulesetCondition = GetFromPoolAndCopyOriginalRulesetCondition(originalRulesetCondition);
        }

        public bool TryEarnOrSpendPoints(PointAction action, PointUsage usage = PointUsage.EarnPoints, int value = 0)
        {
            if (value < 0)
            {
                return false;
            }

            var modifyCurrent = 0;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (usage)
            {
                case PointUsage.EarnPoints:
                    modifyCurrent = Math.Min(value, MaxPoints - CurrentPoints);
                    break;
                case PointUsage.EldritchAegis:
                case PointUsage.EldritchWard:
                    modifyCurrent = -value;
                    break;
                case PointUsage.BattlefieldConversionSuccess:
                    modifyCurrent = -Math.Min(2 * SlotLevel, MaxPoints);
                    break;
                case PointUsage.BattlefieldConversionFailure:
                    modifyCurrent = -SlotLevel;
                    break;
                case PointUsage.BlastBreakthrough:
                    modifyCurrent = -BeamNumber;

                    if (StrPowerPriority[0] != "BlastBreakthroughTitle")
                    {
                        modifyCurrent -= BeamNumber;
                    }

                    break;
                case PointUsage.BlastEmpower:
                    modifyCurrent = -BeamNumber;

                    if (StrPowerPriority[0] != "BlastEmpowerTitle")
                    {
                        modifyCurrent -= BeamNumber;
                    }

                    break;
            }

            if (IsOverload)
            {
                modifyCurrent = Math.Max(0, modifyCurrent);
            }

            switch (action)
            {
                case PointAction.Require:
                    return CurrentPoints + modifyCurrent >= 0;

                case PointAction.Modify:
                    if (CurrentPoints + modifyCurrent < 0)
                    {
                        return false;
                    }

                    modifyCurrent = usage switch
                    {
                        PointUsage.BlastBreakthrough or PointUsage.BlastEmpower => Math.Max(modifyCurrent, -BeamNumber),
                        _ => modifyCurrent
                    };

                    CurrentPoints += modifyCurrent;

                    return true;

                default:
                    return false;
            }
        }

        private void InitSupportCondition([NotNull] RulesetCharacter ownerCharacter)
        {
            var ownerHero = ownerCharacter.GetOriginalHero();

            if (ownerHero == null)
            {
                return;
            }

            var characterLevel = ownerHero.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var proficiencyBonus = ownerHero.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            MaxPoints = ownerHero.GetClassLevel(CharacterClassDefinitions.Warlock);

            if (ownerHero.HasSubFeatureOfType<ModifyEffectDescriptionEldritchBlast>())
            {
                BeamNumber =
                    1 + ModifyEffectDescriptionEldritchBlast.ComputeAdditionalBeamCount(characterLevel, MaxPoints);
            }
            else
            {
                BeamNumber = ProficiencyIncreaseLevels.Count(x => characterLevel >= x);
            }

            if (ownerHero.TrainedFeats.Contains(FeatEldritchVersatilityAdept))
            {
                MaxPoints += proficiencyBonus;
            }

            CurrentPoints = 0;
            SlotLevel = SharedSpellsContext.GetWarlockSpellLevel(ownerHero);
            CreateSlotDC = 8 + proficiencyBonus + (2 * SlotLevel);
            IsValidBlastBreakthrough = false;
            IsOverload = false;
            HasBlastPursuit = ownerHero.HasAnyFeature(FeatureBlastReload);
            LearntAmount =
                ownerHero.TrainedInvocations.Count(
                    x => x.Name.Contains($"Invocation{EldritchVersatilityBuilders.Name}"));
            ReplacedAbilityScore = string.Empty;
            StrPowerPriority.Clear();

            var names = ownerHero.GetSubFeaturesByType<ToggleableOnInvocation>().Select(x => x.InvocationName);

            ownerHero.Invocations.DoIf(x =>
                x.Active && names.Contains(x.InvocationDefinition.Name), y => y.Toggle());

            ModifyAttributeScores(ownerHero, ReplacedAbilityScore);

            ReplacedAbilityScoreValue = 10 + (2 * proficiencyBonus);
            CopiedSpells.Clear();
        }

        public void ModifyAttributeScores(RulesetCharacterHero hero, string abilityScore)
        {
            var names = new List<string>
            {
                AttributeDefinitions.Strength, AttributeDefinitions.Intelligence, AttributeDefinitions.Wisdom
            };

            hero.Attributes.DoIf(x => names.Contains(x.Key),
                y => y.Value.ActiveModifiers.RemoveAll(z => z.Tags.Contains(Name)));

            if (abilityScore == string.Empty)
            {
                hero.Attributes.DoIf(x =>
                        names.Contains(x.Key),
                    y => y.Value.ActiveModifiers.TryAdd(
                        RulesetAttributeModifier
                            .BuildAttributeModifier(AttributeModifierOperation.Additive, LearntAmount, Name)));
            }
        }

        [UsedImplicitly]
        public override void SerializeAttributes(IAttributesSerializer serializer, IVersionProvider versionProvider)
        {
            base.SerializeAttributes(serializer, versionProvider);

            try
            {
                MaxPoints = serializer.SerializeAttribute("MaxPoints", MaxPoints);
                CurrentPoints = serializer.SerializeAttribute("CurrentPoints", CurrentPoints);
                CreateSlotDC = serializer.SerializeAttribute("CreateSlotDC", CreateSlotDC);
                SlotLevel = serializer.SerializeAttribute("SlotLevel", SlotLevel);
                BeamNumber = serializer.SerializeAttribute("BeamNumber", BeamNumber);
                IsValidBlastBreakthrough =
                    serializer.SerializeAttribute("IsValidBlastBreakthrough", IsValidBlastBreakthrough);
                IsOverload = serializer.SerializeAttribute("IsOverload", IsOverload);
                HasBlastPursuit = serializer.SerializeAttribute("HasBlastPursuit", HasBlastPursuit);
                ReplacedAbilityScore = serializer.SerializeAttribute("ReplacedAbilityScore", ReplacedAbilityScore);
                ReplacedAbilityScoreValue =
                    serializer.SerializeAttribute("ReplacedAbilityScoreValue", ReplacedAbilityScoreValue);
                LearntAmount = serializer.SerializeAttribute("LearntAmount", LearntAmount);
            }
            catch (Exception ex)
            {
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] error with VersatilityPointsRulesetCondition serialization (may be caused by mods or bad versioning implementation) " +
                    ex.Message, ex));
            }
        }

        [UsedImplicitly]
        public override void SerializeElements(IElementsSerializer serializer, IVersionProvider versionProvider)
        {
            base.SerializeElements(serializer, versionProvider);

            try
            {
                BaseDefinition.SerializeDatabaseReferenceList(
                    serializer, "CopiedSpells", "SpellDefinition", CopiedSpells);
                StrPowerPriority = serializer.SerializeElement("StrPowerPriority", StrPowerPriority);

                if (serializer.Mode == Serializer.SerializationMode.Read)
                {
                    CopiedSpells.RemoveAll(x => x is null);
                }
            }
            catch (Exception ex)
            {
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] error with VersatilityPointsRulesetCondition serialization (may be caused by mods or bad versioning implementation) " +
                    ex.Message, ex));
            }
        }

        protected override void ClearCustomStates()
        {
            CopiedSpells.Clear();
            MaxPoints = 0;
            CurrentPoints = 0;
            CreateSlotDC = 0;
            SlotLevel = 0;
            BeamNumber = 0;
            ReplacedAbilityScore = string.Empty;
            StrPowerPriority.Clear();
            ReplacedAbilityScoreValue = 0;
            LearntAmount = 0;
            IsValidBlastBreakthrough = false;
            IsOverload = false;
            HasBlastPursuit = false;
        }

        private sealed class OnConditionAddedOrRemovedVersatility :
            IOnConditionAddedOrRemoved, IMagicEffectBeforeHitConfirmedOnEnemy, ICharacterTurnStartListener
        {
            public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
            {
                var rulesetCharacter = locationCharacter.RulesetCharacter;
                if (!rulesetCharacter.GetVersatilitySupportCondition(out var supportCondition) ||
                    !rulesetCharacter.HasAnyFeature(FeatureBlastPursuit))
                {
                    return;
                }

                supportCondition.TryEarnOrSpendPoints(PointAction.Modify, PointUsage.EarnPoints,
                    supportCondition.BeamNumber);
            }

            public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
                GameLocationBattleManager battleManager,
                GameLocationCharacter attacker,
                GameLocationCharacter defender,
                ActionModifier actionModifier,
                RulesetEffect rulesetEffect,
                List<EffectForm> actualEffectForms,
                bool firstTarget,
                bool criticalHit)
            {
                var characterAttacker = attacker.RulesetCharacter;

                if (!IsEldritchBlast(rulesetEffect)
                    || !characterAttacker.GetVersatilitySupportCondition(out var supportCondition))
                {
                    yield break;
                }

                supportCondition.TryEarnOrSpendPoints(PointAction.Modify, PointUsage.EarnPoints,
                    characterAttacker.HasAnyFeature(FeatureBlastPursuit) ? 2 : 1);
            }

            // Do first time init
            public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
            {
                if (rulesetCondition is VersatilitySupportRulesetCondition supportCondition)
                {
                    supportCondition.InitSupportCondition(target);
                }
            }

            // The clearing is called when next time this condition is added
            // So we can do some cleaning here
            // This condition is removed on level up and long rest, so we reapply it.
            // But don't do this right here because if we are respec the character, this should cause a dead loop
            // We just remove the power from power used by me list.
            public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
            {
                // ReSharper disable once InvocationIsSkipped
                Main.Log("Condition Versatility interrupted");

                if (target.CurrentHitPoints <= 0)
                {
                    return;
                }

                var warlockRepertoire =
                    target.GetOriginalHero()!.SpellRepertoires.Find(x =>
                        x.SpellCastingClass == CharacterClassDefinitions.Warlock);

                warlockRepertoire.ExtraSpellsByTag.Remove("BattlefieldShorthand");
                target.PowersUsedByMe.RemoveAll(x => x.PowerDefinition == PowerEldritchVersatilityPointPool);
            }
        }
    }

    private sealed class BlastBreakthroughCustom :
        ToggleableOnInvocation, IOnSpellCasted, IModifyDamageAffinity, IActionFinishedByMe
    {
        private static readonly ConditionDefinition ConditionBlastBreakthroughRemoveImmunity =
            ConditionDefinitionBuilder.Create("ConditionBlastBreakthroughRemoveImmunity")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(new BlastBreakthroughRemoveImmunityCustom())
                .AddToDB();

        public BlastBreakthroughCustom(string invocationName)
        {
            InvocationName = invocationName;
        }

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;
            var actionParams = action.ActionParams;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (Gui.Battle == null ||
                !IsEldritchBlast(actionParams.RulesetEffect) ||
                !rulesetCharacter.GetVersatilitySupportCondition(out var supportCondition))
            {
                yield break;
            }

            // Invalidate damage affinity and spell immunity
            if (!supportCondition.IsValidBlastBreakthrough)
            {
                yield break;
            }

            supportCondition.IsValidBlastBreakthrough = false;
            actionParams.targetCharacters.ForEach(x =>
                x.RulesetCharacter
                    .RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagEffect,
                        ConditionBlastBreakthroughRemoveImmunity.Name)
            );
        }

        public void ModifyDamageAffinity(
            [UsedImplicitly] RulesetActor defender, RulesetActor attacker, List<FeatureDefinition> features)
        {
            if (attacker is not RulesetCharacter attackCharacter ||
                !attackCharacter.GetVersatilitySupportCondition(out var supportCondition) ||
                !supportCondition.IsValidBlastBreakthrough)
            {
                return;
            }

            var modifier = GetAbilityScoreModifier(attackCharacter, AttributeDefinitions.Strength, supportCondition);

            if (modifier < 3)
            {
                return;
            }

            features.RemoveAll(x => x is IDamageAffinityProvider { DamageAffinityType: DamageAffinityType.Resistance });

            if (modifier < 4)
            {
                return;
            }

            features.RemoveAll(x => x is IDamageAffinityProvider { DamageAffinityType: DamageAffinityType.Immunity });
        }

        public int Priority => 1;

        // Spend reserved points on cast EB if success
        public IEnumerator OnSpellCasted(RulesetCharacter featureOwner, GameLocationCharacter caster,
            CharacterActionCastSpell castAction, RulesetEffectSpell selectEffectSpell,
            RulesetSpellRepertoire selectedRepertoire, SpellDefinition selectedSpellDefinition)
        {
            if (Gui.Battle == null ||
                caster.RulesetCharacter != featureOwner ||
                selectedSpellDefinition != SpellDefinitions.EldritchBlast ||
                !featureOwner.GetVersatilitySupportCondition(out var supportCondition) ||
                !supportCondition.TryEarnOrSpendPoints(PointAction.Modify, PointUsage.BlastBreakthrough))
            {
                yield break;
            }

            supportCondition.IsValidBlastBreakthrough = true;

            var modifier = GetAbilityScoreModifier(featureOwner, AttributeDefinitions.Strength, supportCondition);

            if (modifier < 1)
            {
                yield break;
            }

            castAction.ActionParams.ActionModifiers.ForEach(x =>
                {
                    x.AttacktoHitTrends.Add(new TrendInfo(modifier, FeatureSourceType.Invocation, InvocationName,
                        null));
                    x.AttackRollModifier += modifier;
                }
            );

            if (modifier < 2)
            {
                yield break;
            }

            castAction.ActionParams.ActionModifiers.ForEach(x =>
                x.AttackAdvantageTrends.RemoveAll(y => y.sourceType == FeatureSourceType.Proximity));
            if (modifier < 5)
            {
                yield break;
            }

            castAction.ActionParams.targetCharacters.ForEach(
                x =>
                    InflictCondition(ConditionBlastBreakthroughRemoveImmunity, featureOwner, x.RulesetCharacter));
        }

        // Handle toggled in UI
        public override void OnInvocationToggled(GameLocationCharacter locCharacter, RulesetInvocation invocation)
        {
            var rulesetCharacter = locCharacter.RulesetCharacter;

            if (!rulesetCharacter.GetVersatilitySupportCondition(out var supportCondition))
            {
                return;
            }

            if (invocation.Active)
            {
                supportCondition.StrPowerPriority.Add("BlastBreakthroughTitle");
            }
            else
            {
                supportCondition.StrPowerPriority.Remove("BlastBreakthroughTitle");
            }
        }
    }

    private class BlastBreakthroughRemoveImmunityCustom : IRemoveSpellOrSpellLevelImmunity
    {
        public bool IsValid(RulesetCharacter character, RulesetCondition holdingCondition)
        {
            return true;
        }

        public bool ShouldRemoveImmunity(Func<SpellDefinition, bool> isImmuneToSpell)
        {
            return isImmuneToSpell(SpellDefinitions.EldritchBlast);
        }

        public bool ShouldRemoveImmunityLevel(Func<int, int, bool> isImmuneToSpellLevel)
        {
            return isImmuneToSpellLevel(0, 0);
        }
    }

    private sealed class BattlefieldShorthandCopy : IOnSpellCasted
    {
        public int Priority => 3;

        public IEnumerator OnSpellCasted(
            RulesetCharacter featureOwner,
            GameLocationCharacter caster,
            CharacterActionCastSpell castAction,
            RulesetEffectSpell selectEffectSpell,
            RulesetSpellRepertoire selectedRepertoire,
            SpellDefinition selectedSpellDefinition)
        {
            if (!featureOwner.GetVersatilitySupportCondition(out var supportCondition))
            {
                yield break;
            }

            if (featureOwner != caster.RulesetCharacter)
            {
                // Nobody identified the spell
                if (string.IsNullOrEmpty(castAction.ActiveSpell.IdentifiedBy))
                {
                    yield break;
                }

                var owner = GameLocationCharacter.GetFromActor(featureOwner);

                if (owner != null && (!owner.IsWithinRange(caster, 12) || !caster.CanPerceiveTarget(owner)))
                {
                    yield break;
                }
            }

            var warlockRepertoire = featureOwner.GetOriginalHero()!
                .SpellRepertoires.Find(x => x.SpellCastingClass == CharacterClassDefinitions.Warlock);

            if (warlockRepertoire is null)
            {
                yield break;
            }

            if (SpellsContext.SpellsChildMaster.TryGetValue(selectedSpellDefinition, out var parentSpell))
            {
                selectedSpellDefinition = parentSpell;
            }

            var spellLevel = selectedSpellDefinition.SpellLevel;
            var allKnownSpells = warlockRepertoire.EnumerateAvailableExtraSpells();

            allKnownSpells.AddRange(warlockRepertoire.KnownCantrips);
            allKnownSpells.AddRange(warlockRepertoire.knownSpells);
            warlockRepertoire.ExtraSpellsByTag.TryAdd("BattlefieldShorthand", []);

            // If the caster has this feature, check if the spell is copied, if so, return
            if (featureOwner == caster.RulesetCharacter
                && supportCondition.CopiedSpells.Contains(selectedSpellDefinition))
            {
                yield break;
            }

            // Maximum copy-able spell level is half pool size
            if (allKnownSpells.Contains(selectedSpellDefinition)
                || (2 * spellLevel) - 1 > warlockRepertoire.SpellCastingLevel)
            {
                yield break;
            }

            // You yourself should pass a check again to copy it if not overload
            if (!supportCondition.IsOverload)
            {
                var checkModifier = new ActionModifier
                {
                    AbilityCheckModifier =
                        GetAbilityScoreModifier(featureOwner, AttributeDefinitions.Intelligence, supportCondition) -
                        AttributeDefinitions.ComputeAbilityScoreModifier(
                            featureOwner.TryGetAttributeValue(AttributeDefinitions.Intelligence))
                };

                checkModifier.AbilityCheckModifierTrends.Add(new TrendInfo(checkModifier.AbilityCheckModifier,
                    FeatureSourceType.CharacterFeature, "PowerPatronEldritchSurgeVersatilitySwitchPool", null));

                var glc = GameLocationCharacter.GetFromActor(featureOwner);

                if (glc != null)
                {
                    var abilityCheckRoll = glc.RollAbilityCheck(
                        AttributeDefinitions.Intelligence, SkillDefinitions.Arcana,
                        14 + spellLevel + Math.Max(-6, spellLevel - supportCondition.CurrentPoints),
                        AdvantageType.None,
                        checkModifier,
                        false,
                        -1,
                        out var rollOutcome,
                        out var successDelta,
                        true);

                    //PATCH: support for Bardic Inspiration roll off battle and ITryAlterOutcomeAttributeCheck
                    var abilityCheckData = new AbilityCheckData
                    {
                        AbilityCheckRoll = abilityCheckRoll,
                        AbilityCheckRollOutcome = rollOutcome,
                        AbilityCheckSuccessDelta = successDelta,
                        AbilityCheckActionModifier = checkModifier,
                        Action = castAction
                    };

                    yield return TryAlterOutcomeAttributeCheck
                        .HandleITryAlterOutcomeAttributeCheck(glc, abilityCheckData);

                    castAction.AbilityCheckRoll = abilityCheckData.AbilityCheckRoll;
                    castAction.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckRollOutcome;
                    castAction.AbilityCheckSuccessDelta = abilityCheckData.AbilityCheckSuccessDelta;

                    // Fails check
                    if (castAction.AbilityCheckRollOutcome > RollOutcome.Success)
                    {
                        yield break;
                    }
                }
                else
                {
                    yield break;
                }
            }

            var console = Gui.Game.GameConsole;
            var entry = new GameConsoleEntry(
                "Feedback/BattlefieldShorthandCopySpellSuccess", console.consoleTableDefinition) { Indent = true };

            console.AddCharacterEntry(featureOwner, entry);
            entry.AddParameter(
                ConsoleStyleDuplet.ParameterType.Positive, selectedSpellDefinition.GuiPresentation.Title);
            console.AddEntry(entry);
            supportCondition.CopiedSpells.Add(selectedSpellDefinition);
            warlockRepertoire.ExtraSpellsByTag["BattlefieldShorthand"].Add(selectedSpellDefinition);
        }
    }

    private sealed class BattlefieldConversionRestoreSlot : IPowerOrSpellFinishedByMe, IValidatePowerUse
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var gameLocationCharacter = action.ActingCharacter;
            var featureOwner = gameLocationCharacter.RulesetCharacter;

            featureOwner.GetVersatilitySupportCondition(out var supportCondition);

            if (!supportCondition.IsOverload)
            {
                // Check to restore slot
                var checkModifier = new ActionModifier
                {
                    AbilityCheckModifier =
                        GetAbilityScoreModifier(featureOwner, AttributeDefinitions.Intelligence, supportCondition) -
                        AttributeDefinitions.ComputeAbilityScoreModifier(
                            featureOwner.TryGetAttributeValue(AttributeDefinitions.Intelligence))
                };

                checkModifier.AbilityCheckModifierTrends.Add(new TrendInfo(checkModifier.AbilityCheckModifier,
                    FeatureSourceType.CharacterFeature, "PowerPatronEldritchSurgeVersatilitySwitchPool", null));

                var abilityCheckRoll = gameLocationCharacter.RollAbilityCheck(
                    AttributeDefinitions.Intelligence, SkillDefinitions.Arcana,
                    supportCondition.CreateSlotDC,
                    AdvantageType.None,
                    checkModifier,
                    false,
                    -1,
                    out var rollOutcome,
                    out var successDelta,
                    true);

                //PATCH: support for Bardic Inspiration roll off battle and ITryAlterOutcomeAttributeCheck
                var abilityCheckData = new AbilityCheckData
                {
                    AbilityCheckRoll = abilityCheckRoll,
                    AbilityCheckRollOutcome = rollOutcome,
                    AbilityCheckSuccessDelta = successDelta,
                    AbilityCheckActionModifier = checkModifier,
                    Action = action
                };

                yield return TryAlterOutcomeAttributeCheck
                    .HandleITryAlterOutcomeAttributeCheck(gameLocationCharacter, abilityCheckData);

                action.AbilityCheckRoll = abilityCheckData.AbilityCheckRoll;
                action.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckRollOutcome;
                action.AbilityCheckSuccessDelta = abilityCheckData.AbilityCheckSuccessDelta;

                // If success increase DC, otherwise decrease DC
                var createSuccess = action.AbilityCheckRollOutcome <= RollOutcome.Success;

                // Log to notify outcome and new DC
                var console = Gui.Game.GameConsole;
                var entry = createSuccess
                    ? new GameConsoleEntry("Feedback/&BattlefieldConversionCreateSlotSuccess",
                        console.consoleTableDefinition) { Indent = true }
                    : new GameConsoleEntry("Feedback/&BattlefieldConversionCreateSlotFailure",
                        console.consoleTableDefinition) { Indent = true };

                console.AddCharacterEntry(featureOwner, entry);
                entry.AddParameter(ConsoleStyleDuplet.ParameterType.AbilityInfo,
                    supportCondition.CreateSlotDC.ToString());
                console.AddEntry(entry);

                // If fails
                if (action.AbilityCheckRollOutcome > RollOutcome.Success)
                {
                    supportCondition.TryEarnOrSpendPoints(PointAction.Modify, PointUsage.BattlefieldConversionFailure);
                    yield break;
                }
            }

            supportCondition.TryEarnOrSpendPoints(PointAction.Modify, PointUsage.BattlefieldConversionSuccess);

            var rulesetHero = featureOwner.GetOriginalHero();

            if (rulesetHero == null)
            {
                yield break;
            }

            var warlockRepertoire =
                rulesetHero.SpellRepertoires.Find(x => x.SpellCastingClass == CharacterClassDefinitions.Warlock);
            var slotLevelIndex = SharedSpellsContext.IsMulticaster(rulesetHero)
                ? -1
                : SharedSpellsContext.GetWarlockSpellLevel(rulesetHero);

            // Restore Slot and refresh
            if (!warlockRepertoire.usedSpellsSlots.TryGetValue(slotLevelIndex, out var warlockUsedSlots))
            {
                yield break;
            }

            warlockRepertoire.usedSpellsSlots[slotLevelIndex] = Math.Max(0, warlockUsedSlots - 1);
            warlockRepertoire.RepertoireRefreshed?.Invoke(warlockRepertoire);
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            var rulesetHero = character.GetOriginalHero();

            if (rulesetHero is not { IsDeadOrDyingOrUnconscious: false })
            {
                return false;
            }

            // Have spent pact slot && have enough points
            return SharedSpellsContext.GetWarlockUsedSlots(rulesetHero) > 0 &&
                   character.GetVersatilitySupportCondition(out var supportCondition) &&
                   supportCondition.TryEarnOrSpendPoints(PointAction.Require, PointUsage.BattlefieldConversionSuccess);
        }
    }

    private sealed class EldritchAegisTwistHit : ITryAlterOutcomeAttack
    {
        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var helperCharacter = helper.RulesetCharacter;
            var defenderCharacter = defender.RulesetCharacter;

            if (defenderCharacter == null)
            {
                yield break;
            }

            var alreadyBlocked = EldritchAegisSupportRulesetCondition.GetCustomConditionFromCharacter(
                defenderCharacter, out var eldritchAegisSupportCondition);

            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                helper.IsOppositeSide(defender.Side) ||
                (!(alreadyBlocked && eldritchAegisSupportCondition.SourceGuid == helperCharacter.Guid) &&
                 (!defender.IsWithinRange(helper, 7) || !helper.CanPerceiveTarget(defender))))
            {
                yield break;
            }

            if ((!helper.CanReact() && !alreadyBlocked) ||
                !helper.RulesetCharacter.GetVersatilitySupportCondition(out var supportCondition))
            {
                yield break;
            }

            var armorClass = defenderCharacter.TryGetAttributeValue(AttributeDefinitions.ArmorClass);
            var attackRoll = action.AttackRoll;
            var totalAttack =
                attackRoll
                + (attackMode?.ToHitBonus ?? rulesetEffect?.MagicAttackBonus ?? 0)
                + actionModifier.AttackRollModifier;

            // some other reaction saved it already
            if (armorClass > totalAttack)
            {
                yield break;
            }

            var requiredACAddition = totalAttack - armorClass + 1;

            var console = Gui.Game.GameConsole;
            var entry =
                new GameConsoleEntry("Feedback/EldritchAegisGiveACBonus", console.consoleTableDefinition)
                {
                    Indent = true
                };

            console.AddCharacterEntry(helperCharacter, entry);
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, $"{requiredACAddition}");

            // If already applied, we just auto add AC if it's not enough.
            var modifier = GetAbilityScoreModifier(helperCharacter, AttributeDefinitions.Wisdom, supportCondition);

            if (alreadyBlocked)
            {
                // maximum AC bonus is wisdom modifier
                if (eldritchAegisSupportCondition.ACBonus + requiredACAddition > modifier)
                {
                    yield break;
                }

                if (!supportCondition.TryEarnOrSpendPoints(
                        PointAction.Modify, PointUsage.EldritchAegis, requiredACAddition))
                {
                    yield break;
                }

                eldritchAegisSupportCondition.ACBonus += requiredACAddition;
                console.AddEntry(entry);
                defenderCharacter.RefreshArmorClass(true);

                yield break;
            }

            // If not, can we prevent hit?
            if (requiredACAddition > modifier ||
                !supportCondition.TryEarnOrSpendPoints(
                    PointAction.Require, PointUsage.EldritchAegis, requiredACAddition))
            {
                yield break;
            }

            // any reaction within an attack flow must use the attacker as waiter
            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                attacker,
                "EldritchAegis",
                "CustomReactionEldritchAegis".Formatted(Category.Reaction, defender.Name),
                ReactionValidated,
                battleManager: battleManager,
                resource: new ReactionResourceEldritchVersatilityPoints(requiredACAddition));

            yield break;

            void ReactionValidated()
            {
                supportCondition.TryEarnOrSpendPoints(PointAction.Modify, PointUsage.EldritchAegis, requiredACAddition);
                InflictCondition(
                    EldritchAegisSupportRulesetCondition.BindingDefinition, helperCharacter, defenderCharacter);
                EldritchAegisSupportRulesetCondition.GetCustomConditionFromCharacter(
                    defenderCharacter, out eldritchAegisSupportCondition);
                eldritchAegisSupportCondition.ACBonus = requiredACAddition;
                defenderCharacter.RefreshArmorClass(true);
                console.AddEntry(entry);
            }
        }

        private sealed class EldritchAegisSupportRulesetCondition :
            RulesetConditionCustom<EldritchAegisSupportRulesetCondition>, IBindToRulesetConditionCustom
        {
            static EldritchAegisSupportRulesetCondition()
            {
                Category = AttributeDefinitions.TagEffect;
                Marker = new EldritchAegisSupportRulesetCondition();
                BindingDefinition = ConditionDefinitionBuilder
                    .Create("ConditionEldritchAegisAddAC")
                    .SetGuiPresentation(SolastaUnfinishedBusiness.Builders.Category.Condition,
                        ConditionMagicallyArmored)
                    .SetPossessive()
                    .AddCustomSubFeatures(
                        Marker,
                        new EldritchAegisModifyAC())
                    .AddToDB();
            }

            public int ACBonus { get; set; }

            public void ReplaceRulesetCondition(RulesetCondition originalRulesetCondition,
                out RulesetCondition replacedRulesetCondition)
            {
                replacedRulesetCondition = GetFromPoolAndCopyOriginalRulesetCondition(originalRulesetCondition);
            }

            protected override void ClearCustomStates()
            {
                ACBonus = 0;
            }

            public override void SerializeAttributes(IAttributesSerializer serializer, IVersionProvider versionProvider)
            {
                base.SerializeAttributes(serializer, versionProvider);
                try
                {
                    ACBonus = serializer.SerializeAttribute("ACBonus", ACBonus);
                }
                catch (Exception ex)
                {
                    Trace.LogException(
                        new Exception("Error with EldritchAegisSupportCondition serialization" + ex.Message, ex));
                }
            }

            private class EldritchAegisModifyAC : IModifyAC
            {
                public void ModifyAC(
                    RulesetCharacter owner,
                    [UsedImplicitly] bool callRefresh,
                    [UsedImplicitly] bool dryRun,
                    [UsedImplicitly] FeatureDefinition dryRunFeature,
                    RulesetAttribute armorClass)
                {
                    GetCustomConditionFromCharacter(owner, out var supportCondition);
                    var acBonus = supportCondition.ACBonus;
                    var attributeModifier = RulesetAttributeModifier.BuildAttributeModifier(
                        AttributeModifierOperation.Additive,
                        acBonus, AttributeDefinitions.TagEffect);
                    var trendInfo = new TrendInfo(acBonus, FeatureSourceType.Condition, BindingDefinition.Name, null,
                        attributeModifier);

                    armorClass.AddModifier(attributeModifier);
                    armorClass.ValueTrends.Add(trendInfo);
                }
            }
        }
    }

    private class ReactionResourceEldritchVersatilityPoints(int requestPoints)
        : ICustomReactionResource, ICustomReactionCustomResourceUse
    {
        public string GetRequestPoints(CharacterReactionItem item)
        {
            var character = item.guiCharacter.RulesetCharacter;

            character.GetVersatilitySupportCondition(out _);

            return $"{requestPoints}";
        }

        public AssetReferenceSprite Icon => Sprites.EldritchVersatilityResourceIcon;

        public string GetUses(RulesetCharacter character)
        {
            character.GetVersatilitySupportCondition(out var supportCondition);

            return $"{supportCondition.CurrentPoints}";
        }
    }

    private class EldritchWardAidSave : ITryAlterOutcomeSavingThrow
    {
        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            SavingThrowData savingThrowData,
            bool hasHitVisual)
        {
            if (savingThrowData.SaveOutcome != RollOutcome.Failure ||
                helper.IsOppositeSide(defender.Side))
            {
                yield break;
            }

            var defenderCharacter = defender.RulesetCharacter;
            var helperCharacter = helper.RulesetCharacter;
            var alreadyWarded = EldritchWardSupportRulesetCondition.GetCustomConditionFromCharacter(
                defenderCharacter, out var eldritchWardSupportCondition);
            if (!(alreadyWarded && eldritchWardSupportCondition.SourceGuid == helperCharacter.Guid) &&
                (!defender.IsWithinRange(helper, 7) || !helper.CanPerceiveTarget(defender)))
            {
                yield break;
            }

            if ((!helper.CanReact() && !alreadyWarded) ||
                !helper.RulesetCharacter.GetVersatilitySupportCondition(out var supportCondition))
            {
                yield break;
            }

            var requiredSaveAddition = -savingThrowData.SaveOutcomeDelta;
            var modifier = GetAbilityScoreModifier(helperCharacter, AttributeDefinitions.Wisdom, supportCondition);
            var console = Gui.Game.GameConsole;
            var entry =
                new GameConsoleEntry("Feedback/&EldritchWardGiveSaveBonus", console.consoleTableDefinition)
                {
                    Indent = true
                };

            console.AddCharacterEntry(helperCharacter, entry);
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, $"{requiredSaveAddition}");
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.SuccessfulRoll,
                Gui.Format(GameConsole.SaveSuccessOutcome, savingThrowData.SaveDC.ToString()));

            if (alreadyWarded)
            {
                if (eldritchWardSupportCondition.SaveBonus + requiredSaveAddition > modifier)
                {
                    yield break;
                }

                if (!supportCondition.TryEarnOrSpendPoints(
                        PointAction.Modify, PointUsage.EldritchWard, requiredSaveAddition))
                {
                    yield break;
                }

                eldritchWardSupportCondition.SaveBonus += requiredSaveAddition;
                savingThrowData.SaveOutcome = RollOutcome.Success;
                savingThrowData.SaveOutcomeDelta = 0;
                console.AddEntry(entry);

                yield break;
            }

            if (requiredSaveAddition > modifier ||
                !supportCondition.TryEarnOrSpendPoints(
                    PointAction.Require, PointUsage.EldritchWard, requiredSaveAddition))
            {
                yield break;
            }

            // any reaction within a saving flow must use the yielder as waiter
            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingReaction,
                helper,
                "EldritchWard",
                "CustomReactionEldritchWard".Formatted(Category.Reaction, defender.Name),
                ReactionValidated,
                battleManager: battleManager,
                resource: new ReactionResourceEldritchVersatilityPoints(requiredSaveAddition));

            yield break;

            void ReactionValidated()
            {
                supportCondition.TryEarnOrSpendPoints(
                    PointAction.Modify, PointUsage.EldritchWard, requiredSaveAddition);

                InflictCondition(
                    EldritchWardSupportRulesetCondition.BindingDefinition, helperCharacter, defenderCharacter);
                EldritchWardSupportRulesetCondition.GetCustomConditionFromCharacter(
                    defenderCharacter, out eldritchWardSupportCondition);
                eldritchWardSupportCondition.SaveBonus = requiredSaveAddition;

                savingThrowData.SaveOutcomeDelta = 0;
                savingThrowData.SaveOutcome = RollOutcome.Success;

                console.AddEntry(entry);
            }
        }

        private sealed class EldritchWardSupportRulesetCondition :
            RulesetConditionCustom<EldritchWardSupportRulesetCondition>, IBindToRulesetConditionCustom
        {
            static EldritchWardSupportRulesetCondition()
            {
                Category = AttributeDefinitions.TagEffect;
                Marker = new EldritchWardSupportRulesetCondition();
                BindingDefinition = ConditionDefinitionBuilder
                    .Create("ConditionEldritchWardAddSave")
                    .SetGuiPresentation(SolastaUnfinishedBusiness.Builders.Category.Condition,
                        ConditionMagicallyArmored)
                    .SetPossessive()
                    .AddCustomSubFeatures(
                        Marker, new EldritchWardSaveBonus())
                    .AddToDB();
            }

            public int SaveBonus { get; set; }

            public void ReplaceRulesetCondition(RulesetCondition originalRulesetCondition,
                out RulesetCondition replacedRulesetCondition)
            {
                replacedRulesetCondition = GetFromPoolAndCopyOriginalRulesetCondition(originalRulesetCondition);
            }

            protected override void ClearCustomStates()
            {
                SaveBonus = 0;
            }

            public override void SerializeAttributes(IAttributesSerializer serializer, IVersionProvider versionProvider)
            {
                base.SerializeAttributes(serializer, versionProvider);
                try
                {
                    SaveBonus = serializer.SerializeAttribute("SaveBonus", SaveBonus);
                }
                catch (Exception ex)
                {
                    Trace.LogException(
                        new Exception("Error with EldritchWardSupportCondition serialization" + ex.Message, ex));
                }
            }

            private sealed class EldritchWardSaveBonus : IRollSavingThrowInitiated
            {
                public void OnSavingThrowInitiated(
                    RulesetActor rulesetActorCaster,
                    RulesetActor rulesetActorDefender,
                    ref int saveBonus,
                    ref string abilityScoreName,
                    BaseDefinition sourceDefinition,
                    List<TrendInfo> modifierTrends,
                    List<TrendInfo> advantageTrends,
                    ref int rollModifier,
                    ref int saveDC,
                    ref bool hasHitVisual,
                    RollOutcome outcome,
                    int outcomeDelta,
                    List<EffectForm> effectForms)
                {
                    GetCustomConditionFromCharacter(rulesetActorDefender, out var supportCondition);

                    var acBonus = supportCondition.SaveBonus;
                    rollModifier += acBonus;
                    modifierTrends.Add(new TrendInfo(acBonus, FeatureSourceType.Condition, BindingDefinition.Name,
                        BindingDefinition));
                }
            }
        }
    }

    private class BlastEmpowerCustom : ToggleableOnInvocation
    {
        public BlastEmpowerCustom(string name)
        {
            InvocationName = name;
        }

        // Handle toggled in UI
        public override void OnInvocationToggled(GameLocationCharacter locCharacter, RulesetInvocation invocation)
        {
            var rulesetCharacter = locCharacter.RulesetCharacter;
            if (!rulesetCharacter.GetVersatilitySupportCondition(out var supportCondition))
            {
                return;
            }

            if (invocation.Active)
            {
                supportCondition.StrPowerPriority.Add("BlastEmpowerTitle");
            }
            else
            {
                supportCondition.StrPowerPriority.Remove("BlastEmpowerTitle");
            }
        }
    }

    private class OnConditionAddedOrRemovedBlastOverload : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target.GetVersatilitySupportCondition(out var supportCondition))
            {
                supportCondition.IsOverload = true;
            }
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (!target.GetVersatilitySupportCondition(out var supportCondition))
            {
                return;
            }

            supportCondition.IsOverload = false;
        }
    }

    private class EldritchVersatilityAdeptCustom : ICustomLevelUpLogic
    {
        public void ApplyFeature(RulesetCharacterHero hero, [UsedImplicitly] string tag)
        {
            if (hero.HasAnyFeature(PowerEldritchVersatilityPointPool))
            {
                return;
            }

            tag = hero.ActiveFeatures.ContainsKey(tag) ? tag : hero.ActiveFeatures.Keys.Last();
            hero.ActiveFeatures[tag].TryAdd(PowerEldritchVersatilityPointPool);
            hero.ActiveFeatures[tag].TryAdd(PowerVersatilitySwitch);
        }

        public void RemoveFeature(RulesetCharacterHero hero, [UsedImplicitly] string tag)
        {
            if (hero.GetSubclassLevel(CharacterClassDefinitions.Warlock, PatronEldritchSurge.Name) >= 1)
            {
                return;
            }

            hero.ActiveFeatures.Values.Do(x =>
                x.RemoveAll(y => y == PowerEldritchVersatilityPointPool || y == PowerVersatilitySwitch));
            hero.UsablePowers.RemoveAll(x => x.PowerDefinition == PowerVersatilitySwitch);
        }
    }

    private abstract class ToggleableOnInvocation : IOnInvocationToggled
    {
        public string InvocationName;
        public abstract void OnInvocationToggled(GameLocationCharacter character, RulesetInvocation rulesetInvocation);
    }

    private class BlastEmpowerActiveSwitch(string invocationName) : IActionFinishedByMe, IOnSpellCasted
    {
        private string InvocationName { get; } = invocationName;

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;
            var actionParams = action.ActionParams;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (Gui.Battle == null ||
                !IsEldritchBlast(actionParams.RulesetEffect) ||
                !rulesetCharacter.GetVersatilitySupportCondition(out var supportCondition))
            {
                yield break;
            }

            var invocation = rulesetCharacter.Invocations.Find(invocation =>
                !invocation.Active && invocation.invocationDefinition.Name == InvocationName);

            if (invocation != null &&
                supportCondition.StrPowerPriority.Contains("BlastEmpowerTitle"))
            {
                invocation.Toggle();
            }
        }

        public int Priority => 1;

        // Spend reserved points on cast EB
        public IEnumerator OnSpellCasted(RulesetCharacter featureOwner, GameLocationCharacter caster,
            CharacterActionCastSpell castAction, RulesetEffectSpell selectEffectSpell,
            RulesetSpellRepertoire selectedRepertoire, SpellDefinition selectedSpellDefinition)
        {
            if (Gui.Battle == null ||
                caster.RulesetCharacter != featureOwner ||
                selectedSpellDefinition != SpellDefinitions.EldritchBlast ||
                !IsInvocationActive(featureOwner, InvocationName, out var invocation) ||
                !featureOwner.GetVersatilitySupportCondition(out var supportCondition))
            {
                yield break;
            }

            if (!supportCondition.TryEarnOrSpendPoints(PointAction.Modify, PointUsage.BlastEmpower))
            {
                invocation.Toggle();
            }
        }
    }

    // Split this part to become a sub feature of invocation definition to make sure that the spells get removed even when the invocation is toggled off.
    private class BattlefieldShorthandRemoveCopied : IOnSpellCasted
    {
        public int Priority => 2;

        public IEnumerator OnSpellCasted(RulesetCharacter featureOwner, GameLocationCharacter caster,
            CharacterActionCastSpell castAction, [UsedImplicitly] RulesetEffectSpell selectEffectSpell,
            [UsedImplicitly] RulesetSpellRepertoire selectedRepertoire, SpellDefinition selectedSpellDefinition)
        {
            if (!featureOwner.GetVersatilitySupportCondition(out var supportCondition))
            {
                yield break;
            }

            var warlockRepertoire = featureOwner.GetOriginalHero()!
                .SpellRepertoires.Find(x => x.SpellCastingClass == CharacterClassDefinitions.Warlock);

            if (warlockRepertoire is null)
            {
                yield break;
            }

            if (SpellsContext.SpellsChildMaster.TryGetValue(selectedSpellDefinition, out var parentSpell))
            {
                selectedSpellDefinition = parentSpell;
            }

            // If the caster has this feature, check if the spell is copied, if so, remove it both from copied list and repertoire
            if (featureOwner != caster.RulesetCharacter ||
                !supportCondition.CopiedSpells.Remove(selectedSpellDefinition))
            {
                yield break;
            }

            warlockRepertoire.ExtraSpellsByTag.TryGetValue("BattlefieldShorthand", out var spells);
            spells?.Remove(selectedSpellDefinition);
        }
    }
}
