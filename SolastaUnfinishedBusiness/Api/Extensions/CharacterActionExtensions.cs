using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Api.Extensions;

internal static class CharacterActionExtensions
{
    internal static int GetSaveDC(this CharacterAction action)
    {
        var actionParams = action.actionParams;
        var magicEffect = actionParams.RulesetEffect;
        var saveDc = 0;

        if (magicEffect == null)
        {
            var attack = actionParams.AttackMode;
            var effect = attack.effectDescription;

            if (effect.DifficultyClassComputation == EffectDifficultyClassComputation.FixedValue)
            {
                saveDc = effect.FixedSavingThrowDifficultyClass;
            }
            else if (effect.DifficultyClassComputation == EffectDifficultyClassComputation.SpellCastingFeature)
            {
                var repertoire = action.ActingCharacter.RulesetCharacter?.GetClassSpellRepertoire();
                if (repertoire != null)
                {
                    saveDc = repertoire.SaveDC;
                }
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
}
