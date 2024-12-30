using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaUnfinishedBusiness.Models;

internal static class ClassesContext
{
    internal static void Load()
    {
        InventorClass.Build();

        LoadMonkKatanaSpecialization();
        SwitchBarbarianFightingStyle();
        SwitchBardScimitarSpecialization();
        SwitchMonkAbundantKi();
        SwitchMonkFightingStyle();
        SwitchMonkHandwrapsGauntletSlot();
        SwitchMonkImprovedUnarmoredMovement();
        SwitchRangerHumanoidFavoredEnemy();
        SwitchRogueFightingStyle();
        SwitchRogueScimitarSpecialization();
        SwitchRogueStrSaving();
        SwitchSorcererMagicalGuidance();
    }

    #region Ranger

    internal static void SwitchRangerHumanoidFavoredEnemy()
    {
        if (Main.Settings.AddHumanoidFavoredEnemyToRanger)
        {
            AdditionalDamageRangerFavoredEnemyChoice.featureSet.Add(CommonBuilders
                .AdditionalDamageMarshalFavoredEnemyHumanoid);
        }
        else
        {
            AdditionalDamageRangerFavoredEnemyChoice.featureSet.Remove(CommonBuilders
                .AdditionalDamageMarshalFavoredEnemyHumanoid);
        }

        AdditionalDamageRangerFavoredEnemyChoice.FeatureSet.Sort(Sorting.CompareTitle);
    }

    #endregion

    #region Bard

    internal static void SwitchBardScimitarSpecialization()
    {
        ProficiencyBardWeapon.Proficiencies.Remove(WeaponTypeDefinitions.ScimitarType.Name);

        if (Main.Settings.EnableBardScimitarSpecialization)
        {
            ProficiencyBardWeapon.Proficiencies.Add(WeaponTypeDefinitions.ScimitarType.Name);
        }
    }

    #endregion

    #region Barbarian

    internal static readonly FeatureDefinitionFightingStyleChoice FightingStyleChoiceBarbarian =
        FeatureDefinitionFightingStyleChoiceBuilder
            .Create("FightingStyleChoiceBarbarian")
            .SetGuiPresentation("FighterFightingStyle", Category.Feature)
            .SetFightingStyles(
                "Crippling",
                "Dueling",
                "GreatWeapon",
                "TwoWeapon")
            .AddToDB();

    internal static void SwitchBarbarianFightingStyle()
    {
        if (Main.Settings.EnableBarbarianFightingStyle)
        {
            Barbarian.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FightingStyleChoiceBarbarian, 2));
        }
        else
        {
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 2 && x.FeatureDefinition == FightingStyleChoiceBarbarian);
        }

        Barbarian.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    #endregion

    #region Monk

    private static readonly FeatureDefinitionAttributeModifier AttributeModifierMonkAbundantKi =
        FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierMonkAbundantKi")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.AddHalfProficiencyBonus, AttributeDefinitions.KiPoints)
            .SetSituationalContext(SituationalContext.NotWearingArmorOrShield)
            .AddToDB();

    internal static readonly FeatureDefinitionFightingStyleChoice FightingStyleChoiceMonk =
        FeatureDefinitionFightingStyleChoiceBuilder
            .Create("FightingStyleChoiceMonk")
            .SetGuiPresentation("FighterFightingStyle", Category.Feature)
            .SetFightingStyles(
                "Archery",
                "BlindFighting",
                "Dueling",
                "HandAndAHalf")
            .AddToDB();

    internal static void SwitchMonkAbundantKi()
    {
        Monk.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == AttributeModifierMonkAbundantKi);

        if (Main.Settings.EnableMonkAbundantKi)
        {
            Monk.FeatureUnlocks.Add(new FeatureUnlockByLevel(AttributeModifierMonkAbundantKi, 2));
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkFightingStyle()
    {
        Monk.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FightingStyleChoiceMonk);

        if (Main.Settings.EnableMonkFightingStyle)
        {
            Monk.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(FightingStyleChoiceMonk, 2));
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkImprovedUnarmoredMovement()
    {
        if (Main.Settings.EnableMonkImprovedUnarmoredMovement)
        {
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.description =
                "Feature/&MonkAlternateUnarmoredMovementImprovedDescription";
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.title =
                "Feature/&MonkAlternateUnarmoredMovementImprovedTitle";
            MovementAffinityMonkUnarmoredMovementImproved.canMoveOnWalls = true;
        }
        else
        {
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.description =
                "Feature/&MonkUnarmoredMovementImprovedDescription";
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.title =
                "Feature/&MonkUnarmoredMovementImprovedTitle";
            MovementAffinityMonkUnarmoredMovementImproved.canMoveOnWalls = false;
        }
    }

    internal static void SwitchMonkHandwrapsGauntletSlot()
    {
        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
        {
            if (item is not { WeaponDescription.weaponType: EquipmentDefinitions.WeaponTypeUnarmedStrike })
            {
                continue;
            }

            if (item == ItemDefinitions.UnarmedStrikeBase) { continue; }

            if (Main.Settings.EnableMonkHandwrapsOnGauntletSlot)
            {
                item.SlotTypes.Add(EquipmentDefinitions.SlotTypeGloves);
                item.SlotsWhereActive.Add(EquipmentDefinitions.SlotTypeGloves);
            }
            else
            {
                item.SlotTypes.Remove(EquipmentDefinitions.SlotTypeGloves);
                item.SlotsWhereActive.Remove(EquipmentDefinitions.SlotTypeGloves);
            }
        }
    }

    private static void LoadMonkKatanaSpecialization()
    {
        ProficiencyMonkWeapon.AddCustomSubFeatures(new CustomLevelUpLogicMonkKatanaSpecialization());
    }

    private sealed class CustomLevelUpLogicMonkKatanaSpecialization : ICustomLevelUpLogic
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            if (!Main.Settings.EnableMonkKatanaSpecialization)
            {
                return;
            }

            const string PREFIX = "CustomInvocationMonkWeaponSpecialization";

            var heroBuildingData = hero.GetHeroBuildingData();
            var invocationKatanaType = GetDefinition<InvocationDefinition>($"{PREFIX}KatanaWeaponType");

            heroBuildingData.LevelupTrainedInvocations.Add(tag, [invocationKatanaType]);
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }
    }

    #endregion

    #region Rogue

    internal static readonly FeatureDefinitionFightingStyleChoice FightingStyleChoiceRogue =
        FeatureDefinitionFightingStyleChoiceBuilder
            .Create("FightingStyleChoiceRogue")
            .SetGuiPresentation("FighterFightingStyle", Category.Feature)
            .SetFightingStyles(
                "Archery",
                "Defense",
                "HandAndAHalf",
                "TwoWeapon")
            .AddToDB();

    internal static void SwitchRogueFightingStyle()
    {
        if (Main.Settings.EnableRogueFightingStyle)
        {
            Rogue.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FightingStyleChoiceRogue, 2));
        }
        else
        {
            Rogue.FeatureUnlocks.RemoveAll(x => x.level == 2 && x.FeatureDefinition == FightingStyleChoiceRogue);
        }

        Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRogueScimitarSpecialization()
    {
        ProficiencyRogueWeapon.Proficiencies.Remove(WeaponTypeDefinitions.ScimitarType.Name);

        if (Main.Settings.EnableRogueScimitarSpecialization)
        {
            ProficiencyRogueWeapon.Proficiencies.Add(WeaponTypeDefinitions.ScimitarType.Name);
        }
    }

    private static void SwitchRogueStrSaving()
    {
        var powerNames = new List<string>
        {
            "PowerRogueCunningStrikeDisarm",
            //"PowerRogueCunningStrikePoison",
            "PowerRogueCunningStrikeTrip",
            //"PowerRogueCunningStrikeWithdraw",
            //"PowerRogueDeviousStrikeDaze",
            //"PowerRogueDeviousStrikeKnockOut",
            "PowerRogueDeviousStrikeObscure",
            "PowerRoguishOpportunistDebilitatingStrike",
            "PowerRoguishOpportunistImprovedDebilitatingStrike",
            "PowerRoguishBladeCallerHailOfBlades"
        };

        foreach (var power in DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
                     .Where(x => powerNames.Contains(x.Name)))
        {
            power.AddCustomSubFeatures(new ModifyEffectDescriptionSavingThrowRogue(power));
        }
    }

    #endregion

    #region Sorcerer

    private static readonly FeatureDefinition FeatureSorcererMagicalGuidance = FeatureDefinitionBuilder
        .Create("FeatureSorcererMagicalGuidance")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new TryAlterOutcomeAttributeCheckSorcererMagicalGuidance())
        .AddToDB();

    internal static void SwitchSorcererMagicalGuidance()
    {
        if (Main.Settings.EnableSorcererMagicalGuidance)
        {
            Sorcerer.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(FeatureSorcererMagicalGuidance, 5));
        }
        else
        {
            Sorcerer.FeatureUnlocks.RemoveAll(x =>
                x.level == 5 && x.FeatureDefinition == FeatureSorcererMagicalGuidance);
        }

        Sorcerer.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }


    private sealed class TryAlterOutcomeAttributeCheckSorcererMagicalGuidance : ITryAlterOutcomeAttributeCheck
    {
        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;

            if (abilityCheckData.AbilityCheckRoll == 0 ||
                abilityCheckData.AbilityCheckRollOutcome != RollOutcome.Failure ||
                helper != defender ||
                rulesetHelper.RemainingSorceryPoints == 0)
            {
                yield break;
            }

            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingFree,
                defender,
                "MagicalGuidanceCheck",
                "CustomReactionMagicalGuidanceCheckDescription"
                    .Formatted(Category.Reaction, defender.Name, helper.Name),
                ReactionValidated,
                battleManager: battleManager,
                resource: ReactionResourceSorceryPoints.Instance);

            yield break;

            void ReactionValidated()
            {
                rulesetHelper.SpendSorceryPoints(1);

                var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _,
                    out _);
                var previousRoll = abilityCheckData.AbilityCheckRoll;

                abilityCheckData.AbilityCheckSuccessDelta += dieRoll - abilityCheckData.AbilityCheckRoll;
                abilityCheckData.AbilityCheckRoll = dieRoll;
                abilityCheckData.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckSuccessDelta >= 0
                    ? RollOutcome.Success
                    : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    "Feature/&FeatureSorcererMagicalGuidanceTitle",
                    "Feedback/&MagicalGuidanceCheckToHitRoll",
                    extra:
                    [
                        (dieRoll > previousRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            dieRoll.ToString()),
                        (previousRoll > dieRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            previousRoll.ToString())
                    ]);
            }
        }
    }

    #endregion
}
