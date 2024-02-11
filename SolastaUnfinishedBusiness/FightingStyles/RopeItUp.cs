using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class RopeItUp : AbstractFightingStyle
{
    private const string Name = "RopeItUp";

    private static readonly FeatureDefinition FeatureRopeItUp = FeatureDefinitionAttributeModifierBuilder
        .Create($"AttributeModifier{Name}")
        .SetGuiPresentation(Name, Category.FightingStyle)
        .AddCustomSubFeatures(ReturningWeapon.AlwaysValid, new ModifyWeaponAttackModeRopeItUp())
        .AddToDB();

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(Name)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(Name, Resources.RopeItUp, 256))
        .SetFeatures(FeatureRopeItUp)
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    ];

    private sealed class ModifyWeaponAttackModeRopeItUp : IModifyWeaponAttackMode, IPhysicalAttackInitiatedByMe
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (attackMode?.thrown != true)
            {
                return;
            }

            attackMode.closeRange += 2;
            attackMode.maxRange += 2;
        }

        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            if (attackModifier.Proximity == AttackProximity.Melee)
            {
                yield break;
            }

            attackMode.AddAttackTagAsNeeded(TagsDefinitions.MagicalWeapon);
            attackMode.toHitBonus += 1;
            attackMode.ToHitBonusTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, FeatureRopeItUp.Name, FeatureRopeItUp));

            var damage = attackMode.effectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                yield break;
            }

            damage.bonusDamage += 1;
            damage.DamageBonusTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, FeatureRopeItUp.Name, FeatureRopeItUp));
        }
    }
}
