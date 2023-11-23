using System.Collections.Generic;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IRollSavingThrowFinished
{
    [UsedImplicitly]
    public void OnSavingThrowFinished(
        RulesetCharacter caster,
        RulesetCharacter defender,
        int saveBonus,
        string abilityScoreName,
        BaseDefinition sourceDefinition,
        List<TrendInfo> modifierTrends,
        List<TrendInfo> advantageTrends,
        int rollModifier,
        int saveDC,
        bool hasHitVisual,
        ref RollOutcome outcome,
        ref int outcomeDelta,
        List<EffectForm> effectForms);
}
