using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using static ActionDefinitions;
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

        var offhandEffect = new EffectDescriptionBuilder();

        offhandEffect.SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.MeleeHit, 1,
            RuleDefinitions.TargetType.Individuals);
        offhandEffect.AddEffectForm(new EffectFormBuilder().CreatedByCharacter()
            .SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus)
            .SetDamageForm(false, RuleDefinitions.DieType.D10, "DamageBludgeoning", 1,
                RuleDefinitions.DieType.D8, 1,
                RuleDefinitions.HealFromInflictedDamage.Never, new List<RuleDefinitions.TrendInfo>()).Build());

        var gui = GuiPresentationBuilder.Build("PugilistFighting", Category.FightingStyle,
            PathBerserker.GuiPresentation.SpriteReference);

        var pugilistAdditionalDamage = FeatureDefinitionActionAffinityBuilder
            .Create("AdditionalDamagePugilist", "36d24b2e-8ef4-4037-a82f-05e63d56f3d2")
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
            .Create("PugilistFightingStlye", "b14f91dc-8706-498b-a9a0-d583b7b00d09")
            .SetFeatures(pugilistAdditionalDamage)
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
            var k = effectDescription.EffectForms
                .FindIndex(form => form.FormType == EffectForm.EffectFormType.Damage);

            if (k < 0)
            {
                return;
            }

            var additionalDice = new EffectFormBuilder()
                .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D4)
                .Build();

            effectDescription.EffectForms.Insert(k + 1, additionalDice);
        }
    }
}
