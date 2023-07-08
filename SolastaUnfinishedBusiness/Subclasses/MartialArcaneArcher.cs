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

        //explicitly re-use wizard spell list, so custom cantrips selected for druid will show here 
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

        var arcaneShotPowers = BuildArcaneShotPowers();

        var powerArcaneShot = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneShot")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnAttackHitWithBow, RechargeRate.ShortRest, 1, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .SetCustomSubFeatures(
                new RestrictReactionAttackMode((_, character, _) =>
                    character.RulesetCharacter.IsToggleEnabled(ArcaneArcherToggle)))
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerArcaneShot, true, arcaneShotPowers);

        var arcaneShotPowersWithoutAttackRoll = BuildArcaneShotPowersWithoutAttackRoll();

        CreateArcaneArcherChoices(arcaneShotPowers.Union(arcaneShotPowersWithoutAttackRoll));

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

        // Improved Arcane Shot

        var powerImprovedArcaneShot = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedArcaneShot")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackHitWithBow, RechargeRate.ShortRest, 1, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .SetCustomSubFeatures(
                new RestrictReactionAttackMode((_, character, _) =>
                    character.RulesetCharacter.IsToggleEnabled(ArcaneArcherToggle)))
            .SetOverriddenPower(powerArcaneShot)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerImprovedArcaneShot, true, arcaneShotPowers);
        
        // LEVEL 15

        // Ever-Ready Shot

        var featureEverReadyShot = FeatureDefinitionBuilder
            .Create($"Feature{Name}EverReadyShot")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // LEVEL 18

        // Master Arcane Shot

        var powerMasterArcaneShot = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MasterArcaneShot")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackHitWithBow, RechargeRate.ShortRest, 1, 4)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .SetCustomSubFeatures(
                new RestrictReactionAttackMode((_, character, _) =>
                    character.RulesetCharacter.IsToggleEnabled(ArcaneArcherToggle)))
            .SetOverriddenPower(powerImprovedArcaneShot)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerMasterArcaneShot, true, arcaneShotPowers);
        
        featureEverReadyShot.SetCustomSubFeatures(new BattleStartedListenerEverReadyShot(
            featureEverReadyShot, powerImprovedArcaneShot, powerMasterArcaneShot));

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
                powerImprovedArcaneShot,
                invocationPoolArcaneShotChoice1)
            .AddFeaturesAtLevel(15,
                invocationPoolArcaneShotChoice1)
            .AddFeaturesAtLevel(18,
                powerMasterArcaneShot,
                featureEverReadyShot,
                invocationPoolArcaneShotChoice1)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static IsWeaponValidHandler IsBow =>
        ValidatorsWeapon.IsOfWeaponType(WeaponTypeDefinitions.LongbowType, WeaponTypeDefinitions.ShortbowType);

    private static List<FeatureDefinitionPower> BuildArcaneShotPowers()
    {
        // Banishing Arrow

        var powerBanishingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BanishingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Charisma, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 0, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 11)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionBanished,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Beguiling Arrow

        var powerBeguilingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BeguilingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePsychic, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Bursting Arrow

        var powerBurstingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BurstingArrow")
            .SetGuiPresentation(Category.Feature)
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

        // Enfeebling Arrow

        var powerEnfeeblingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EnfeeblingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionEnfeebled,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Grasping Arrow

        var powerGraspingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GraspingArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeSlashing, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionRestrained,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // Shadow Arrow

        var powerShadowArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ShadowArrow")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePsychic, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 5)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionBlinded,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        return new List<FeatureDefinitionPower>
        {
            powerBanishingArrow,
            powerBeguilingArrow,
            powerBurstingArrow,
            powerEnfeeblingArrow,
            powerGraspingArrow,
            powerShadowArrow
        };
    }

    private static IEnumerable<FeatureDefinitionPower> BuildArcaneShotPowersWithoutAttackRoll()
    {
        // Piercing Arrow

        var powerPiercingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PiercingArrow")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Seeking Arrow

        var powerSeekingArrow = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SeekingArrow")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        return new List<FeatureDefinitionPower> { powerPiercingArrow, powerSeekingArrow };
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

    private sealed class ModifyWeaponAttackModeMagicArrow : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            if (attackMode == null || !IsBow(attackMode, null, character))
            {
                return;
            }

            attackMode.AttackTags.TryAdd("MagicalWeapon");
        }
    }

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

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } || !IsBow(attackMode, null, null))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter = "Reaction/&CustomReactionRavenDeadlyAimReactDescription"
            };
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("RavenDeadlyAim", reactionParams);

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

    private sealed class BattleStartedListenerEverReadyShot : ICharacterBattleStartedListener
    {
        private readonly FeatureDefinition _featureDefinition;
        private readonly FeatureDefinitionPower _powerDefinition10;
        private readonly FeatureDefinitionPower _powerDefinition18;

        public BattleStartedListenerEverReadyShot(
            FeatureDefinition featureDefinition,
            FeatureDefinitionPower featureDefinitionPower10,
            FeatureDefinitionPower featureDefinitionPower18)
        {
            _featureDefinition = featureDefinition;
            _powerDefinition10 = featureDefinitionPower10;
            _powerDefinition18 = featureDefinitionPower18;
        }

        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var character = locationCharacter.RulesetCharacter;

            if (character == null)
            {
                return;
            }

            var levels = character.GetClassLevel(CharacterClassDefinitions.Fighter);
            var power = levels >= 18 ? _powerDefinition18 : _powerDefinition10;
            var usablePower = UsablePowersProvider.Get(power, character);

            if (character.GetRemainingUsesOfPower(usablePower) > 0)
            {
                return;
            }

            character.RepayPowerUse(UsablePowersProvider.Get(power, character));
            character.LogCharacterUsedFeature(_featureDefinition);
        }
    }
}
