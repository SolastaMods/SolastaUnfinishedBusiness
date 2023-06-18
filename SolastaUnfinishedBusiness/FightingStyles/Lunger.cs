using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Lunger : AbstractFightingStyle
{
    internal const string Name = "Lunger";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(Name)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(Name, Resources.Lunger, 256))
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("FeatureLunger")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(new ModifyWeaponAttackModeLunger())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class ModifyWeaponAttackModeLunger : ModifyWeaponAttackModeBase
    {
        public ModifyWeaponAttackModeLunger() : base(IsWeaponValid, Name, ValidatorsCharacter.HasFreeHand)
        {
        }

        protected override void TryModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
           IncreaseReach(attackMode);
        }

        private static bool IsWeaponValid(RulesetAttackMode attackMode, RulesetItem rulesetItem,
            RulesetCharacter rulesetCharacter)
        {
            var item = attackMode?.SourceObject as RulesetItem ?? rulesetItem;
            return ValidatorsWeapon.IsMelee(item) &&
                   !ValidatorsWeapon.HasAnyWeaponTag(item, TagsDefinitions.WeaponTagHeavy);
        }
    }
}
