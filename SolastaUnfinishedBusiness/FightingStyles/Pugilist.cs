using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Pugilist : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = CustomizableFightingStyleBuilder
        .Create("Pugilist")
        .SetGuiPresentation(Category.FightingStyle, TraditionLight.GuiPresentation.SpriteReference)
        .SetFeatures(
            FeatureDefinitionActionAffinityBuilder
                .Create("ActionAffinityFightingStylePugilist")
                .SetGuiPresentation("Pugilist", Category.FightingStyle)
                .SetDefaultAllowedActonTypes()
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

    // Removes check that makes `ShoveBonus` action unavailable if character has no shield
    // Replaces call to RulesetActor.IsWearingShield with custom method that always returns true
    internal static void RemoveShieldRequiredForBonusPush(List<CodeInstruction> codes)
    {
        static bool True(RulesetActor actor)
        {
            return true;
        }

        var customMethod = new Func<RulesetActor, bool>(True).Method;

        var bindIndex = codes.FindIndex(x =>
        {
            if (x.operand == null)
            {
                return false;
            }

            var operand = x.operand.ToString();

            return operand.Contains("IsWearingShield");
        });

        if (bindIndex > 0)
        {
            codes[bindIndex] = new CodeInstruction(OpCodes.Call, customMethod);
        }
    }

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
