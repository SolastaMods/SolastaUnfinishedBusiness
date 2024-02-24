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

        var conditionShieldTechniquesSavingThrow = ConditionDefinitionBuilder
            .Create($"Condition{Name}SavingThrow")
            .SetGuiPresentation(Name, Category.Feat)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{Name}")
                    .SetGuiPresentation("Feature/&IndomitableResistanceTitle", Gui.NoLocalization)
                    .SetModifiers(FeatureDefinitionSavingThrowAffinity.ModifierType.SourceAbility, DieType.D1, 1, false,
                        AttributeDefinitions.Dexterity)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
            .AddToDB();

        var powerShieldTechniques = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionShieldTechniquesResistance))
                    .Build())
            .AddToDB();

        powerShieldTechniques.AddCustomSubFeatures(
            new CustomBehaviorShieldTechniques(powerShieldTechniques, conditionShieldTechniquesSavingThrow));

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
        ConditionDefinition conditionShieldTechniquesSavingThrow)
        : IRollSavingThrowInitiated, IMagicEffectBeforeHitConfirmedOnMe
    {
        // halve any damage taken
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

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

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerShieldTechniques, rulesetDefender);
            var actionParams = new CharacterActionParams(defender, ActionDefinitions.Id.PowerNoCost)
            {
                StringParameter = "ShieldTechniques",
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManagerService
                    .MyInstantiateEffectPower(rulesetDefender, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };

            var count = gameLocationActionService.PendingReactionRequestGroups.Count;

            gameLocationActionService.ReactToUsePower(actionParams, "UsePower", defender);

            yield return gameLocationBattleService.WaitForReactions(defender, gameLocationActionService, count);
        }

        // add +2 on DEX savings
        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> advantageTrends,
            int saveDC,
            bool hasHitVisual,
            List<EffectForm> effectForms)
        {
            if (abilityScoreName != AttributeDefinitions.Dexterity || !defender.IsWearingShield())
            {
                return;
            }

            defender.InflictCondition(
                conditionShieldTechniquesSavingThrow.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagStatus,
                caster.Guid,
                caster.CurrentFaction.Name,
                1,
                conditionShieldTechniquesSavingThrow.Name,
                0,
                2,
                0);
        }
    }
}
