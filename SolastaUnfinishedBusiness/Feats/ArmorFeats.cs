using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
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

        var actionAffinityShieldTechniques = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(ActionDefinitions.Id.ShoveBonus)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasShield, ValidatorsCharacter.HasAttacked))
            .AddToDB();

        var conditionShieldTechniquesResistance = ConditionDefinitionBuilder
            .Create(ConditionShieldTechniquesResistanceName)
            .SetGuiPresentation(Name, Category.Feat)
            .SetSilent(Silent.WhenAddedOrRemoved)
#if false
            .SetFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityForceDamageResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPiercingResistance,
                DamageAffinityPoisonResistance,
                DamageAffinityRadiantResistance,
                DamageAffinitySlashingResistance,
                DamageAffinityThunderResistance)
#endif
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .AddToDB();

        var powerShieldTechniques = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerShieldTechniques.AddCustomSubFeatures(
            new CustomBehaviorShieldTechniques(powerShieldTechniques, conditionShieldTechniquesResistance));

        return FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(actionAffinityShieldTechniques, powerShieldTechniques)
            .SetArmorProficiencyPrerequisite(ShieldCategory)
            .AddToDB();
    }

    private sealed class CustomBehaviorShieldTechniques :
        IRollSavingThrowInitiated, IMagicalAttackBeforeHitConfirmedOnMe
    {
        private readonly ConditionDefinition _conditionShieldTechniquesResistance;
        private readonly FeatureDefinitionPower _powerShieldTechniques;

        public CustomBehaviorShieldTechniques(
            FeatureDefinitionPower powerShieldTechniques,
            ConditionDefinition conditionShieldTechniquesResistance)
        {
            _powerShieldTechniques = powerShieldTechniques;
            _conditionShieldTechniquesResistance = conditionShieldTechniquesResistance;
        }

        // halve any damage taken
        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnMe(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (!defender.CanReact() || !defender.RulesetCharacter.IsWearingShield())
            {
                yield break;
            }

            if (!rulesetEffect.EffectDescription.HasSavingThrow ||
                rulesetEffect.EffectDescription.SavingThrowAbility != AttributeDefinitions.Dexterity ||
                !actualEffectForms.Exists(x => x.FormType == EffectForm.EffectFormType.Damage))
            {
                yield break;
            }

            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null || battleManager == null)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "Reaction/&CustomReactionShieldTechniquesReactDescription"
                };
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("ShieldTechniques", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(defender, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender.LogCharacterUsedPower(_powerShieldTechniques);
            rulesetDefender.InflictCondition(
                _conditionShieldTechniquesResistance.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetDefender.Guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                _conditionShieldTechniquesResistance.Name,
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
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta, List<EffectForm> effectForms)
        {
            if (abilityScoreName != AttributeDefinitions.Dexterity || !defender.IsWearingShield())
            {
                return;
            }

            rollModifier += 2;
            modifierTrends.Add(
                new TrendInfo(2, FeatureSourceType.Condition, _conditionShieldTechniquesResistance.Name,
                    _conditionShieldTechniquesResistance));
        }
    }
}
