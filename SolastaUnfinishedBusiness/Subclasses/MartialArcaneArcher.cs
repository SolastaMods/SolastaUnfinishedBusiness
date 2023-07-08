using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialArcaneArcher : AbstractSubclass
{
    private const string Name = "MartialArcaneArcher";

    private const ActionDefinitions.Id ArcaneArcherToggle = (ActionDefinitions.Id)ExtraActionId.ArcaneArcherToggle;

    private static readonly Dictionary<FeatureDefinitionPower, ArcaneShotData> ArcaneShotPowers = new();

    internal MartialArcaneArcher()
    {
        // LEVEL 03

        // Arcane Lore

        var proficiencyArcana = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Arcana")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Arcana)
            .AddToDB();

        var proficiencyNature = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Nature")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Nature)
            .AddToDB();

        var featureSetArcaneLore = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ArcaneLore")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(proficiencyArcana, proficiencyNature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddToDB();

        // Arcane Magic

        var spellListArcaneMagic = SpellListDefinitionBuilder
            .Create($"SpellList{Name}ArcaneMagic")
            .SetGuiPresentationNoContent(true)
            .FinalizeSpells()
            .AddToDB();

        //explicitly re-use wizard spell list, so custom cantrips selected for wizard will show here 
        spellListArcaneMagic.SpellsByLevel[0].Spells = SpellListDefinitions.SpellListWizard.SpellsByLevel[0].Spells;

        var castSpellArcaneMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, $"CastSpell{Name}ArcaneMagic")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
            .SetSpellList(spellListArcaneMagic)
            .AddToDB();

        // Arcane Shot

        var actionAffinityAudaciousWhirlToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityArcaneArcherToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.ArcaneArcherToggle)
            .AddToDB();

        var powerArcaneShot = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneShot")
            .SetGuiPresentation($"FeatureSet{Name}ArcaneShot", Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackHitWithBow, RechargeRate.ShortRest, 1, 0)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .SetCustomSubFeatures(
                IsPowerPool.Marker,
                HasModifiedUses.Marker,
                new SpendPowerFinishedByMeArcaneShot(),
                new RestrictReactionAttackMode((_, character, _) =>
                    character.RulesetCharacter.IsToggleEnabled(ArcaneArcherToggle)))
            .AddToDB();

        BuildArcaneShotPowers(powerArcaneShot);
        CreateArcaneArcherChoices(ArcaneShotPowers.Keys);
        PowerBundle.RegisterPowerBundle(powerArcaneShot, true, ArcaneShotPowers.Keys);

        var powerArcaneShotAdditionalUse1 = FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}ArcaneShotUse1")
            .SetGuiPresentation(Category.Feature)
            .SetFixedValue(powerArcaneShot, 1)
            .AddToDB();

        var powerArcaneShotAdditionalUse2 = FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}ArcaneShotUse2")
            .SetGuiPresentationNoContent(true)
            .SetFixedValue(powerArcaneShot, 2)
            .AddToDB();

        var invocationPoolArcaneShotChoice1 =
            CustomInvocationPoolDefinitionBuilder
                .Create("InvocationPoolArcaneShotChoice1")
                .SetGuiPresentation(Category.Feature)
                .Setup(InvocationPoolTypeCustom.Pools.ArcaneShotChoice)
                .AddToDB();

        var invocationPoolArcaneShotChoice2 =
            CustomInvocationPoolDefinitionBuilder
                .Create("InvocationPoolArcaneShotChoice2")
                .SetGuiPresentation(Category.Feature)
                .Setup(InvocationPoolTypeCustom.Pools.ArcaneShotChoice, 2)
                .AddToDB();

        var featureSetArcaneShot = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ArcaneShot")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                actionAffinityAudaciousWhirlToggle,
                invocationPoolArcaneShotChoice2,
                powerArcaneShotAdditionalUse2,
                powerArcaneShot)
            .AddToDB();

        // LEVEL 07

        // Magic Arrow

        var featureMagicArrow = FeatureDefinitionBuilder
            .Create($"Feature{Name}MagicArrow")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ModifyWeaponAttackModeMagicArrow())
            .AddToDB();

        // Guided Shot

        var featureGuidedShot = FeatureDefinitionBuilder
            .Create($"Feature{Name}GuidedShot")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureGuidedShot.SetCustomSubFeatures(new PhysicalAttackTryAlterOutcomeGuidedShot(featureGuidedShot));

        // LEVEL 10

        // Arcane Shot Additional Use

        // Arcane Shot Choice

        // LEVEL 15

        // Ever-Ready Shot

        var featureEverReadyShot = FeatureDefinitionBuilder
            .Create($"Feature{Name}EverReadyShot")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureEverReadyShot.SetCustomSubFeatures(new BattleStartedListenerEverReadyShot(
            featureEverReadyShot, powerArcaneShot));

        // LEVEL 18

        // Arcane Shot Additional Use

        // Arcane Shot Choice

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RangerSwiftBlade)
            .AddFeaturesAtLevel(3,
                featureSetArcaneLore,
                castSpellArcaneMagic,
                featureSetArcaneShot)
            .AddFeaturesAtLevel(7,
                featureMagicArrow,
                featureGuidedShot,
                invocationPoolArcaneShotChoice1)
            .AddFeaturesAtLevel(10,
                powerArcaneShotAdditionalUse1,
                invocationPoolArcaneShotChoice1)
            .AddFeaturesAtLevel(15,
                featureEverReadyShot,
                invocationPoolArcaneShotChoice1)
            .AddFeaturesAtLevel(18,
                powerArcaneShotAdditionalUse1,
                invocationPoolArcaneShotChoice1)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static IsWeaponValidHandler IsBow =>
        ValidatorsWeapon.IsOfWeaponType(WeaponTypeDefinitions.LongbowType, WeaponTypeDefinitions.ShortbowType);

    private static void BuildArcaneShotPowers(FeatureDefinitionPower pool)
    {
        // Banishing Arrow

        var powerBanishingArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BanishingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Banishment)
            //.SetUsesFixed(ActivationTime.NoCost)
            .SetSharedPool(ActivationTime.NoCost, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(SpellDefinitions.Banishment)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Charisma, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 0, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 11)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerBanishingArrow,
            new ArcaneShotData
            {
                DebuffCondition = ConditionDefinitions.ConditionBanished,
                SavingThrowAbility = AttributeDefinitions.Charisma
            });

        // Beguiling Arrow

        var powerBeguilingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BeguilingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.CharmPerson)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1)
                    .SetParticleEffectParameters(SpellDefinitions.CharmPerson)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePsychic, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerBeguilingArrow,
            new ArcaneShotData
            {
                DebuffCondition = ConditionDefinitions.ConditionCharmed,
                SavingThrowAbility = AttributeDefinitions.Wisdom
            });

        // Bursting Arrow

        var powerBurstingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BurstingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.EldritchBlast)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerBurstingArrow, new ArcaneShotData());

        // Enfeebling Arrow

        var powerEnfeeblingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EnfeeblingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.RayOfEnfeeblement)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(SpellDefinitions.RayOfEnfeeblement)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerEnfeeblingArrow,
            new ArcaneShotData
            {
                DebuffCondition = ConditionDefinitions.ConditionEnfeebled,
                SavingThrowAbility = AttributeDefinitions.Constitution
            });

        // Grasping Arrow

        var powerGraspingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GraspingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Entangle)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(SpellDefinitions.Entangle)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeSlashing, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerGraspingArrow,
            new ArcaneShotData
            {
                DebuffCondition = ConditionDefinitions.ConditionRestrained,
                SavingThrowAbility = AttributeDefinitions.Strength
            });

        // Shadow Arrow

        var powerShadowArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ShadowArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Blindness)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(SpellDefinitions.Blindness)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePsychic, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerShadowArrow,
            new ArcaneShotData
            {
                DebuffCondition = ConditionDefinitions.ConditionBlinded,
                SavingThrowAbility = AttributeDefinitions.Wisdom
            });
    }

    private static void CreateArcaneArcherChoices(IEnumerable<FeatureDefinitionPower> powers)
    {
        foreach (var power in powers)
        {
            var name = power.Name.Replace("Power", string.Empty);
            var guiPresentation = power.guiPresentation;

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{name}")
                .SetGuiPresentation(guiPresentation)
                .SetPoolType(InvocationPoolTypeCustom.Pools.ArcaneShotChoice)
                .SetGrantedFeature(power)
                .SetCustomSubFeatures(HiddenInvocation.Marker)
                .AddToDB();
        }
    }

    private static void TryInflictArcaneShotCondition(
        IControllableCharacter attacker,
        IControllableCharacter defender,
        FeatureDefinition featureDefinition,
        ConditionDefinition conditionDefinition,
        string savingThrowAbility)
    {
        var rulesetAttacker = attacker.RulesetCharacter;
        var rulesetDefender = defender.RulesetCharacter;

        if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
            rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
        {
            return;
        }

        var modifierTrend = rulesetDefender.actionModifier.savingThrowModifierTrends;
        var advantageTrends = rulesetDefender.actionModifier.savingThrowAdvantageTrends;
        var attackerIntModifier =
            AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Intelligence));
        var profBonus =
            AttributeDefinitions.ComputeProficiencyBonus(
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.CharacterLevel));
        var defenderSavingThrowModifier =
            AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetDefender.TryGetAttributeValue(savingThrowAbility));

        rulesetDefender.RollSavingThrow(0, AttributeDefinitions.Strength, featureDefinition, modifierTrend,
            advantageTrends, defenderSavingThrowModifier, 8 + profBonus + attackerIntModifier, false,
            out var savingOutcome,
            out _);

        if (savingOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
        {
            return;
        }

        rulesetDefender.InflictCondition(
            conditionDefinition.Name,
            DurationType.Round,
            1,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagCombat,
            rulesetAttacker.guid,
            rulesetAttacker.CurrentFaction.Name,
            1,
            null,
            0,
            0,
            0);
    }

    private static void InflictBurstingArrowAreaDamage(GameLocationCharacter attacker, GameLocationCharacter defender)
    {
        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

        if (gameLocationBattleService == null)
        {
            return;
        }

        var damageRoll = RollDie(DieType.D6, AdvantageType.None, out _, out _);
        var dices = new List<int> { damageRoll };
        var damageForm = new DamageForm
        {
            DamageType = DamageTypeForce, DieType = DieType.D6, DiceNumber = 1, BonusDamage = 0
        };

        // apply damage to all targets
        foreach (var rulesetDefender in gameLocationBattleService.Battle.GetMyContenders(defender.Side)
                     .Where(x => gameLocationBattleService.IsWithin1Cell(defender, x))
                     .ToList() // avoid changing enumerator
                     .Select(targetCharacter => targetCharacter.RulesetCharacter))
        {
            RulesetActor.InflictDamage(
                damageRoll,
                damageForm,
                DamageTypeForce,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                rulesetDefender,
                false,
                attacker.Guid,
                false,
                new List<string>(),
                new RollInfo(DieType.D6, dices, 0),
                false,
                out _);
        }
    }

    private struct ArcaneShotData
    {
        public ConditionDefinition DebuffCondition;
        public string SavingThrowAbility;
    }

    //
    // Arcane Shot
    //

    private sealed class SpendPowerFinishedByMeArcaneShot : ISpendPowerFinishedByMe, IPhysicalAttackFinishedByMe
    {
        private FeatureDefinitionPower PowerSpent { get; set; }

        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            if (!IsBow(attackerAttackMode, null, null))
            {
                yield break;
            }

            if (PowerSpent == null || !ArcaneShotPowers.TryGetValue(PowerSpent, out var arcaneShotData))
            {
                yield break;
            }

            //TODO: consider improve this in the future with a proper action defined on arcaneShotData
            if (PowerSpent.Name == $"Power{Name}BurstingArrow")
            {
                InflictBurstingArrowAreaDamage(attacker, defender);
            }
            else
            {
                TryInflictArcaneShotCondition(
                    attacker, defender, PowerSpent, arcaneShotData.DebuffCondition, arcaneShotData.SavingThrowAbility);
            }

            PowerSpent = null;
        }

        public IEnumerator OnSpendPowerFinishedByMe(CharacterActionSpendPower action, FeatureDefinitionPower power)
        {
            PowerSpent = power;

            yield break;
        }
    }

    //
    // Magic Arrow
    //

    private sealed class ModifyWeaponAttackModeMagicArrow : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            if (attackMode == null || !IsBow(attackMode, null, character))
            {
                return;
            }

            attackMode.AttackTags.TryAdd(TagsDefinitions.MagicalWeapon);
        }
    }

    //
    // Guided Shot
    //

    private class PhysicalAttackTryAlterOutcomeGuidedShot : IPhysicalAttackTryAlterOutcome
    {
        private readonly FeatureDefinition _featureDefinition;

        public PhysicalAttackTryAlterOutcomeGuidedShot(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public IEnumerator OnAttackTryAlterOutcome(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter me,
            GameLocationCharacter target,
            ActionModifier attackModifier)
        {
            var attackMode = action.actionParams.attackMode;
            var rulesetAttacker = me.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
                !IsBow(attackMode, null, null))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingFree);
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("MartialArcaneArcherGuidedShot", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var totalRoll = (action.AttackRoll + attackMode.ToHitBonus).ToString();
            var rollCaption = action.AttackRoll == 1
                ? "Feedback/&RollCheckCriticalFailureTitle"
                : "Feedback/&CriticalAttackFailureOutcome";

            rulesetAttacker.LogCharacterUsedFeature(_featureDefinition,
                "Feedback/&TriggerRerollLine",
                false,
                (ConsoleStyleDuplet.ParameterType.Base, $"{action.AttackRoll}+{attackMode.ToHitBonus}"),
                (ConsoleStyleDuplet.ParameterType.FailedRoll, Gui.Format(rollCaption, totalRoll)));

            var roll = rulesetAttacker.RollAttack(
                attackMode.toHitBonus,
                target.RulesetCharacter,
                attackMode.sourceDefinition,
                attackModifier.attackToHitTrends,
                false,
                new List<TrendInfo>
                {
                    new(1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition)
                },
                attackMode.ranged,
                false,
                attackModifier.attackRollModifier,
                out var outcome,
                out var successDelta,
                -1,
                // testMode true avoids the roll to display on combat log as the original one will get there with altered results
                true);

            attackModifier.ignoreAdvantage = false;
            attackModifier.attackAdvantageTrends =
                new List<TrendInfo>
                {
                    new(1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition)
                };
            action.AttackRollOutcome = outcome;
            action.AttackSuccessDelta = successDelta;
            action.AttackRoll = roll;
        }
    }

    //
    // Ready Shot
    //

    private sealed class BattleStartedListenerEverReadyShot : ICharacterBattleStartedListener
    {
        private readonly FeatureDefinition _featureDefinition;
        private readonly FeatureDefinitionPower _powerArcaneShot;

        public BattleStartedListenerEverReadyShot(
            FeatureDefinition featureDefinition,
            FeatureDefinitionPower powerArcaneShot)
        {
            _featureDefinition = featureDefinition;
            _powerArcaneShot = powerArcaneShot;
        }

        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var character = locationCharacter.RulesetCharacter;

            if (character == null)
            {
                return;
            }

            var levels = character.GetClassLevel(CharacterClassDefinitions.Fighter);

            if (levels < 18)
            {
                return;
            }

            if (character.CanUsePower(_powerArcaneShot))
            {
                return;
            }

            character.RepayPowerUse(UsablePowersProvider.Get(_powerArcaneShot, character));
            character.LogCharacterUsedFeature(_featureDefinition);
        }
    }
}
