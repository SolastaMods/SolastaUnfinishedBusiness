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
