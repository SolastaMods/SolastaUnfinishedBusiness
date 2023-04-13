using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Lunger : AbstractFightingStyle
{
    private const string Name = "Lunger";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(Name)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(Name, Resources.Lunger, 256))
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("FeatureLunger")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(new ModifyAttackModeForWeaponLunger())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class ModifyAttackModeForWeaponLunger : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            var itemDefinition = attackMode?.SourceDefinition as ItemDefinition;
            var offHandWeapon = character.GetOffhandWeapon();

            if (offHandWeapon?.ItemDefinition != null ||
                attackMode == null ||
                !ValidatorsWeapon.IsMelee(itemDefinition) ||
                ValidatorsWeapon.HasAnyWeaponTag(itemDefinition, TagsDefinitions.WeaponTagHeavy))
            {
                return;
            }

            attackMode.reach = true;
            attackMode.reachRange = 2;
        }
    }
}
