using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static EquipmentDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ArmorFeats
{
    internal const string ConditionShieldTechniquesResistanceName = "ConditionShieldTechniquesResistance";

    // this is entirely implemented on rulesetCharacterHero transpiler using context validations below
    // they change max dexterity to 3 and remove any instance of Stealth Disadvantage checks
    private static readonly FeatDefinition FeatMediumArmorMaster = FeatDefinitionBuilder
        .Create("FeatMediumArmorMaster")
        .SetGuiPresentation(Category.Feat)
        .SetArmorProficiencyPrerequisite(MediumArmorCategory)
        .AddToDB();

    internal static bool IsFeatMediumArmorMasterContextValid(
        ItemDefinition itemDefinition,
        RulesetCharacterHero rulesetCharacterHero)
    {
        return itemDefinition.IsArmor &&
               IsFeatMediumArmorMasterContextValid(itemDefinition.ArmorDescription, rulesetCharacterHero);
    }

    internal static bool IsFeatMediumArmorMasterContextValid(
        ArmorDescription armorDescription,
        RulesetCharacterHero rulesetCharacterHero)
    {
        return armorDescription.ArmorTypeDefinition.ArmorCategory == MediumArmorCategory &&
               rulesetCharacterHero.TrainedFeats.Contains(FeatMediumArmorMaster);
    }

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var proficiencyFeatMediumArmor = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatMediumArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor,
                MediumArmorCategory,
                ShieldCategory)
            .AddToDB();

        var featMediumArmorDex = FeatDefinitionBuilder
            .Create("FeatMediumArmorDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(proficiencyFeatMediumArmor, AttributeModifierCreed_Of_Misaye)
            .SetArmorProficiencyPrerequisite(LightArmorCategory)
            .SetFeatFamily("MediumArmor")
            .AddToDB();

        var featMediumArmorStr = FeatDefinitionBuilder
            .Create("FeatMediumArmorStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(proficiencyFeatMediumArmor, AttributeModifierCreed_Of_Einar)
            .SetArmorProficiencyPrerequisite(LightArmorCategory)
            .SetFeatFamily("MediumArmor")
            .AddToDB();

        var featHeavyArmorMaster = FeatDefinitionBuilder
            .Create("FeatHeavyArmorMaster")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Einar,
                FeatureDefinitionReduceDamageBuilder
                    .Create("ReduceDamageFeatHeavyArmorMaster")
                    .SetGuiPresentation("FeatHeavyArmorMaster", Category.Feat)
                    .SetAlwaysActiveReducedDamage((_, _) => 3,
                        DamageTypeBludgeoning, DamageTypePiercing, DamageTypeSlashing)
                    .AddCustomSubFeatures(ValidatorsCharacter.HasHeavyArmor)
                    .AddToDB())
            .SetArmorProficiencyPrerequisite(HeavyArmorCategory)
            .AddToDB();

        var featShieldTechniques = BuildFeatShieldTechniques();

        feats.AddRange(
            featMediumArmorDex, featMediumArmorStr, FeatMediumArmorMaster, featHeavyArmorMaster, featShieldTechniques);

        var featGroupMediumArmor = GroupFeats.MakeGroup("FeatGroupMediumArmor", "MediumArmor",
            featMediumArmorDex,
            featMediumArmorStr);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(featShieldTechniques);

        GroupFeats.MakeGroup("FeatGroupArmor", null,
            featGroupMediumArmor,
            FeatMediumArmorMaster,
            featHeavyArmorMaster,
            ArmorMaster,
            DiscretionOfTheCoedymwarth,
            MightOfTheIronLegion,
            SturdinessOfTheTundra);
    }

    private static FeatDefinition BuildFeatShieldTechniques()
    {
        const string Name = "FeatShieldTechniques";

        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{Name}Mark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddToDB();

        var actionAffinityShieldTechniques = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(ActionDefinitions.Id.ShoveBonus)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasShield, ValidatorsCharacter.HasAttacked))
            .AddToDB();

        var powerShieldTechniques = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .AddToDB();

        powerShieldTechniques.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorShieldTechniques(powerShieldTechniques, conditionMark));

        return FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(actionAffinityShieldTechniques, powerShieldTechniques)
            .SetArmorProficiencyPrerequisite(ShieldCategory)
            .AddToDB();
    }

    private sealed class CustomBehaviorShieldTechniques(
        FeatureDefinitionPower powerShieldTechniques,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionMark)
        : IRollSavingThrowInitiated, ITryAlterOutcomeAttack, IMagicEffectBeforeHitConfirmedOnMe
    {
        // halve any damage taken
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (!defender.CanReact() || !rulesetDefender.IsWearingShield())
            {
                yield break;
            }

            if (!rulesetEffect.EffectDescription.HasSavingThrow ||
                rulesetEffect.EffectDescription.SavingThrowAbility != AttributeDefinitions.Dexterity ||
                !actualEffectForms.Exists(x => x.FormType == EffectForm.EffectFormType.Damage))
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerShieldTechniques, rulesetDefender);
            var actionParams = new CharacterActionParams(defender, ActionDefinitions.Id.PowerNoCost)
            {
                StringParameter = "ShieldTechniques",
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetDefender, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(actionParams, "UsePower", defender);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!actionParams.ReactionValidated)
            {
                yield break;
            }

            actionModifier.DefenderDamageMultiplier *= 0.5f;
            rulesetDefender.DamageHalved(rulesetDefender, powerShieldTechniques);

            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetDefender.InflictCondition(
                conditionMark.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionMark.Name,
                0,
                0,
                0);
        }

        // add +2 on DEX savings
        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
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
            if (abilityScoreName != AttributeDefinitions.Dexterity || !defender.IsWearingShield())
            {
                return;
            }

            rollModifier += 2;
            modifierTrends.Add(
                new TrendInfo(2, FeatureSourceType.Power, powerShieldTechniques.Name, powerShieldTechniques));
        }

        public int HandlerPriority => 10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var battleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager ||
                helper != defender ||
                rulesetEffect == null)
            {
                yield break;
            }

            var actualEffectForms = rulesetEffect.EffectDescription.EffectForms ?? [];

            yield return OnMagicEffectBeforeHitConfirmedOnMe(battleManager, attacker, defender, actionModifier,
                rulesetEffect, actualEffectForms, false, false);
        }
    }
}
