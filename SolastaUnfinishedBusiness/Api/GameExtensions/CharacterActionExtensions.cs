using System;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class CharacterActionExtensions
{
    // ReSharper disable once InconsistentNaming
    internal static int GetSaveDC(this CharacterAction action)
    {
        var actionParams = action.actionParams;
        var magicEffect = actionParams.RulesetEffect;
        var saveDc = 0;

        if (magicEffect == null)
        {
            var attack = actionParams.AttackMode;
            var effect = attack.effectDescription;

            switch (effect.DifficultyClassComputation)
            {
                case EffectDifficultyClassComputation.FixedValue:
                    saveDc = effect.FixedSavingThrowDifficultyClass;
                    break;

                case EffectDifficultyClassComputation.SpellCastingFeature:
                {
                    var repertoire = action.ActingCharacter.RulesetCharacter?.GetClassSpellRepertoire();
                    if (repertoire != null)
                    {
                        saveDc = repertoire.SaveDC;
                    }

                    break;
                }
                //TODO: implement missing computation methods (like Ki and Breath Weapon)
                case EffectDifficultyClassComputation.AbilityScoreAndProficiency:
                    break;
                case EffectDifficultyClassComputation.Ki:
                    break;
                case EffectDifficultyClassComputation.BreathWeapon:
                    break;
                case EffectDifficultyClassComputation.CustomAbilityModifierAndProficiency:
                    break;
                default:
                    throw new ArgumentException("effect.DifficultyClassComputation");
            }
        }
        else
        {
            saveDc = magicEffect.SaveDC;
        }

        return saveDc;
    }

    internal static string FormatTitle(this CharacterAction action)
    {
        var magicEffect = action.actionParams.RulesetEffect;

        return magicEffect == null
            ? Gui.Localize("Action/&AttackTitle")
            : magicEffect.SourceDefinition.FormatTitle();
    }

    internal static bool ActionShouldKeepConcentration()
    {
        var action = Global.CurrentAction;
        var isProtectedUsePower = action is CharacterActionUsePower { activePower: not null } actionUsePower
                                  && actionUsePower.activePower.PowerDefinition
                                      .HasSubFeatureOfType<IPreventRemoveConcentrationOnPowerUse>();

        if (isProtectedUsePower)
        {
            return true;
        }

        var isProtectedSpendPower = action is CharacterActionSpendPower { activePower: not null } actionSpendPower
                                    && actionSpendPower.activePower.PowerDefinition
                                        .HasSubFeatureOfType<IPreventRemoveConcentrationOnPowerUse>();

        return isProtectedSpendPower;
    }
}
