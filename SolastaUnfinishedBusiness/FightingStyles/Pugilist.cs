using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Pugilist : AbstractFightingStyle
{
    private const string PugilistName = "Pugilist";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(PugilistName)
        .SetGuiPresentation(Category.FightingStyle, TraditionLight)
        .SetFeatures(
            FeatureDefinitionActionAffinityBuilder
                .Create("ActionAffinityFightingStylePugilist")
                .SetGuiPresentation(PugilistName, Category.FightingStyle)
                .SetDefaultAllowedActionTypes()
                .SetAuthorizedActions(Id.ShoveBonus)
                .SetCustomSubFeatures(
                    new AddExtraUnarmedAttack(ActionType.Bonus),
                    new AdditionalUnarmedDice(),
                    new ValidatorsDefinitionApplication(ValidatorsCharacter.HasUnarmedHand))
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStyleRanger
    };

    private sealed class AdditionalUnarmedDice : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsUnarmedWeapon(attackMode))
            {
                return;
            }

            var effectDescription = attackMode.EffectDescription;
            var damage = effectDescription.FindFirstDamageForm();
            var k = effectDescription.EffectForms.FindIndex(form => form.damageForm == damage);

            if (k < 0 || damage == null)
            {
                return;
            }

            var additionalDice = EffectFormBuilder
                .Create()
                .SetDamageForm(damage.damageType, 1, DieType.D4)
                .Build();

            effectDescription.EffectForms.Insert(k + 1, additionalDice);
        }
    }
}
