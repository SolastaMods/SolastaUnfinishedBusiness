using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaCommunityExpansion.FightingStyles;

internal sealed class Pugilist : AbstractFightingStyle
{
    private FightingStyleDefinitionCustomizable instance;

    [NotNull]
    internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
    {
        return new List<FeatureDefinitionFightingStyleChoice>
        {
            FeatureDefinitionFightingStyleChoices.FightingStyleChampionAdditional,
            FeatureDefinitionFightingStyleChoices.FightingStyleFighter,
            FeatureDefinitionFightingStyleChoices.FightingStyleRanger
        };
    }

    internal override FightingStyleDefinition GetStyle()
    {
        if (instance != null)
        {
            return instance;
        }

        var gui = GuiPresentationBuilder.Build("Pugilist", Category.FightingStyle,
            PathBerserker.GuiPresentation.SpriteReference);

        var actionAffinityPugilist = FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityPugilist", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(gui)
            .SetDefaultAllowedActonTypes()
            .SetAuthorizedActions(Id.ShoveBonus)
            .SetCustomSubFeatures(
                new AddExtraUnarmedAttack(ActionType.Bonus),
                new AdditionalUnarmedDice(),
                new FeatureApplicationValidator(CharacterValidators.HasUnarmedHand)
            )
            .AddToDB();

        instance = CustomizableFightingStyleBuilder
            .Create("Pugilist", DefinitionBuilder.CENamespaceGuid)
            .SetFeatures(actionAffinityPugilist)
            .SetGuiPresentation(gui)
            .SetIsActive(_ => true)
            .AddToDB();

        return instance;
    }

    private sealed class AdditionalUnarmedDice : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
        {
            if (!WeaponValidators.IsUnarmedWeapon(attackMode))
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

            var additionalDice = new EffectFormBuilder()
                .SetDamageForm(diceNumber: 1, dieType: DieType.D4, damageType: damage.damageType)
                .Build();

            effectDescription.EffectForms.Insert(k + 1, additionalDice);
        }
    }
}
