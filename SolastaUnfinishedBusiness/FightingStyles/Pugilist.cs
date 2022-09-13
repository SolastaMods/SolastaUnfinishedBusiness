using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

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
            .Create("ActionAffinityFightingStylePugilist", DefinitionBuilder.CENamespaceGuid)
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
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
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

internal static class PugilistFightingStyle
{
    // Removes check that makes `ShoveBonus` action unavailable if character has no shield
    // Replaces call to RulesetActor.IsWearingShield with custom method that always returns true
    public static void RemoveShieldRequiredForBonusPush(List<CodeInstruction> codes)
    {
        var customMethod = new Func<RulesetActor, bool>(CustomMethod).Method;

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

    private static bool CustomMethod(RulesetActor actor)
    {
        return true;
    }
}
