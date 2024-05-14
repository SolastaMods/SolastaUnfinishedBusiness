using System.Collections.Generic;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class RopeItUp : AbstractFightingStyle
{
    internal const string RopeItUpName = "RopeItUp";

    private static readonly FeatureDefinition FeatureRopeItUp = FeatureDefinitionAttributeModifierBuilder
        .Create($"AttributeModifier{RopeItUpName}")
        .SetGuiPresentation(RopeItUpName, Category.FightingStyle)
        .AddCustomSubFeatures(ReturningWeapon.AlwaysValid, new ModifyWeaponAttackModeRopeItUp())
        .AddToDB();

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(RopeItUpName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(RopeItUpName, Resources.RopeItUp, 256))
        .SetFeatures(FeatureRopeItUp)
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        CharacterContext.FightingStyleChoiceBarbarian,
        CharacterContext.FightingStyleChoiceMonk,
        CharacterContext.FightingStyleChoiceRogue,
        FightingStyleChampionAdditional,
        FightingStyleFighter,
        FightingStylePaladin,
        FightingStyleRanger
    ];

    private sealed class ModifyWeaponAttackModeRopeItUp : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (attackMode?.thrown != true)
            {
                return;
            }

            attackMode.closeRange += 2;
            attackMode.maxRange += 4;
        }
    }
}
