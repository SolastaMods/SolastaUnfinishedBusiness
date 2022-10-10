using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Api.Extensions;

internal static class CharacterActionExtensions
{
    internal static int GetSaveDC(this CharacterAction action)
    {
        var actionParams = action.actionParams;
        var magicEffect = actionParams.RulesetEffect;
        var saveDC = 0;

        if (magicEffect == null)
        {
            var attack = actionParams.AttackMode;
            var effect = attack.effectDescription;
            if (effect.DifficultyClassComputation == EffectDifficultyClassComputation.FixedValue)
            {
                saveDC = effect.FixedSavingThrowDifficultyClass;
            }
            else if (effect.DifficultyClassComputation == EffectDifficultyClassComputation.SpellCastingFeature)
            {
                var repertoire = action.ActingCharacter.RulesetCharacter?.GetClassSpellRepertoire();
                if (repertoire != null)
                {
                    saveDC = repertoire.SaveDC;
                }
            }
        }
        else
        {
            saveDC = magicEffect.SaveDC;
        }

        return saveDC;
    }
}
