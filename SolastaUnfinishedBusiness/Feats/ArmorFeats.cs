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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ArmorFeats
{
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

        var powerShieldTechniques = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerShieldTechniques.AddCustomSubFeatures(new CustomBehaviorShieldTechniques(powerShieldTechniques));

        return FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(actionAffinityShieldTechniques, powerShieldTechniques)
            .SetArmorProficiencyPrerequisite(LightArmorCategory)
            .AddToDB();
    }

    private sealed class CustomBehaviorShieldTechniques : IModifySavingThrow, ITryAlterOutcomeFailedSavingThrow
    {
        private readonly FeatureDefinitionPower _powerShieldTechniques;

        public CustomBehaviorShieldTechniques(FeatureDefinitionPower powerShieldTechniques)
        {
            _powerShieldTechniques = powerShieldTechniques;
        }

        // validate savings bonus to only be DEX wielding shield
        public bool IsValid(
            RulesetActor rulesetActor,
            RulesetActor rulesetCaster,
            IEnumerable<EffectForm> effectForms,
            string attributeScore)
        {
            return attributeScore == AttributeDefinitions.Dexterity
                   && rulesetActor is RulesetCharacterHero hero && hero.IsWearingShield();
        }

        // add +2 on DEX savings
        public string AttributeAndActionModifier(
            RulesetActor rulesetActor,
            ActionModifier actionModifier,
            string attribute)
        {
            actionModifier.SavingThrowModifier += 2;
            // for some reason this isn't displaying on log
            actionModifier.SavingThrowModifierTrends.Add(
                new TrendInfo(2, FeatureSourceType.Power, _powerShieldTechniques.Name, _powerShieldTechniques));

            return attribute;
        }

        // offer to reroll DEX saving on self or ally
        public IEnumerator OnFailedSavingTryAlterOutcome(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier saveModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            var savingThrowAbility = action.ActionParams.RulesetEffect?.EffectDescription.SavingThrowAbility
                                     ?? action.ActionParams.AttackMode?.EffectDescription.SavingThrowAbility;

            if (savingThrowAbility != AttributeDefinitions.Dexterity)
            {
                yield break;
            }

            if (!ShouldTrigger(battleManager, defender, helper))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(helper, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = FormatReactionDescription(action, attacker, defender, helper)
                };
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("ShieldTechniques", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(helper, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            helper.RulesetCharacter.LogCharacterUsedPower(_powerShieldTechniques);
            action.RolledSaveThrow = TryModifyRoll(action, attacker, defender, saveModifier, hasHitVisual);
        }

        private static bool ShouldTrigger(
            IGameLocationBattleService gameLocationBattleService,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            return helper.CanReact()
                   && helper.RulesetCharacter.IsWearingShield()
                   && !defender.IsOppositeSide(helper.Side)
                   && gameLocationBattleService.IsWithin1Cell(helper, defender);
        }

        private static bool TryModifyRoll(
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier saveModifier,
            bool hasHitVisual)
        {
            var actionParams = action.ActionParams;

            action.RolledSaveThrow = actionParams.RulesetEffect == null
                ? actionParams.AttackMode.TryRollSavingThrow(attacker.RulesetCharacter,
                    defender.RulesetActor, saveModifier, actionParams.AttackMode.EffectDescription.EffectForms,
                    out var saveOutcome, out var saveOutcomeDelta)
                : actionParams.RulesetEffect.TryRollSavingThrow(attacker.RulesetCharacter, attacker.Side,
                    defender.RulesetActor, saveModifier, actionParams.RulesetEffect.EffectDescription.EffectForms,
                    hasHitVisual, out saveOutcome, out saveOutcomeDelta);

            action.SaveOutcome = saveOutcome;
            action.SaveOutcomeDelta = saveOutcomeDelta;

            return true;
        }

        private static string FormatReactionDescription(
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var text = defender == helper
                ? "Reaction/&CustomReactionShieldTechniquesDescriptionSelf"
                : "Reaction/&CustomReactionShieldTechniquesDescriptionAlly";

            return Gui.Format(text, defender.Name, attacker.Name, action.FormatTitle());
        }
    }
}
