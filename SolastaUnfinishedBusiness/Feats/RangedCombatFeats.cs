using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class RangedCombatFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featBowMastery = BuildBowMastery();
        var featDeadEye = BuildDeadEye();
        var featRangedExpert = BuildRangedExpert();
        var featZenArcher = BuildZenArcher();

        feats.AddRange(
            featBowMastery, featDeadEye, featRangedExpert, featZenArcher);

        GroupFeats.FeatGroupRangedCombat.AddFeats(
            featBowMastery,
            featDeadEye,
            featRangedExpert,
            featZenArcher);
    }

    private static FeatDefinition BuildZenArcher()
    {
        const string Name = "ZenArcher";

        var attackModifier = FeatureDefinitionAttackModifierBuilder
            .Create($"Feature{Name}")
            .SetGuiPresentation(Name, Category.FightingStyle)
            .AddCustomSubFeatures(
                new CanUseAttribute(
                    AttributeDefinitions.Wisdom,
                    ValidatorsWeapon.IsOfWeaponType(
                        LongbowType,
                        ShortbowType)))
            .AddToDB();

        return FeatDefinitionBuilder
            .Create($"Feat{Name}")
            .SetGuiPresentation(Name, Category.FightingStyle)
            .SetFeatures(
                DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Maraike,
                attackModifier)
            .AddToDB();
    }

    private static FeatDefinition BuildBowMastery()
    {
        const string NAME = "FeatBowMastery";

        var isLongOrShortBowOrCrossbow = ValidatorsWeapon.IsOfWeaponType(
            LongbowType, ShortbowType, HeavyCrossbowType, LightCrossbowType, CustomWeaponsContext.HandXbowWeaponType);

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) =>
                            (OperationType.Set, isLongOrShortBowOrCrossbow(mode, null, character))),
                        new AddExtraRangedAttack(
                            ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(LongbowType),
                            ValidatorsCharacter.HasUsedWeaponType(LongbowType)),
                        new AddExtraRangedAttack(
                            ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(ShortbowType),
                            ValidatorsCharacter.HasUsedWeaponType(ShortbowType)),
                        new AddExtraRangedAttack(
                            ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(HeavyCrossbowType),
                            ValidatorsCharacter.HasUsedWeaponType(HeavyCrossbowType)),
                        new AddExtraRangedAttack(
                            ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(LightCrossbowType),
                            ValidatorsCharacter.HasUsedWeaponType(LightCrossbowType)),
                        new AddExtraRangedAttack(
                            ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(CustomWeaponsContext.HandXbowWeaponType),
                            ValidatorsCharacter.HasUsedWeaponType(CustomWeaponsContext.HandXbowWeaponType)))
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildDeadEye()
    {
        const string Name = "FeatDeadeye";

        var actionAffinityDeadEyeToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityDeadEyeToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.DeadEyeToggle)
            .AddToDB();

        var featDeadeye = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                actionAffinityDeadEyeToggle,
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}")
                    .SetGuiPresentation(Name, Category.Feat, Gui.NoLocalization)
                    .SetIgnoreCover()
                    .AddCustomSubFeatures(new BumpWeaponWeaponAttackRangeToMax(ValidatorsWeapon.AlwaysValid))
                    .AddToDB())
            .AddToDB();

        featDeadeye.AddCustomSubFeatures(new ModifyWeaponAttackModeFeatDeadeye(featDeadeye));

        return featDeadeye;
    }

    private static FeatDefinition BuildRangedExpert()
    {
        const string NAME = "FeatRangedExpert";

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierFightingStyleTwoWeapon,
                        $"AttackModifier{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        ValidatorsCharacter.HasOffhandWeaponType(
                            CustomWeaponsContext.HandXbowWeaponType, CustomWeaponsContext.LightningLauncherType),
                        new RemoveRangedAttackInMeleeDisadvantage(),
                        new InnovationArmor.AddLauncherAttack(ActionType.Bonus,
                            InnovationArmor.InInfiltratorMode,
                            ValidatorsCharacter.HasAttacked),
                        new AddExtraRangedAttack(
                            ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(CustomWeaponsContext.HandXbowWeaponType),
                            ValidatorsCharacter.HasAttacked))
                    .AddToDB())
            .AddToDB();
    }

    //
    // HELPERS
    //

    private sealed class ModifyWeaponAttackModeFeatDeadeye(FeatDefinition featDefinition) : IModifyWeaponAttackMode
    {
        private const int ToHit = -5;
        private const int ToDamage = +10;

        private readonly TrendInfo _attackTrendInfo =
            new(ToHit, FeatureSourceType.Feat, featDefinition.Name, featDefinition);

        private readonly TrendInfo _damageTrendInfo =
            new(ToDamage, FeatureSourceType.Feat, featDefinition.Name, featDefinition);

        public void ModifyWeaponAttackMode(
            RulesetCharacter character,
            RulesetAttackMode attackMode,
            RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            if (!character.IsToggleEnabled((Id)ExtraActionId.DeadEyeToggle))
            {
                return;
            }

            if (!ValidatorsWeapon.IsOfWeaponType(DartType)(attackMode, null, null) &&
                attackMode is not { ranged: true })
            {
                return;
            }

            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            attackMode.ToHitBonus += ToHit;
            attackMode.ToHitBonusTrends.Add(_attackTrendInfo);
            damage.BonusDamage += ToDamage;
            damage.DamageBonusTrends.Add(_damageTrendInfo);
        }
    }
}
