using System.Collections.Generic;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IRollSavingThrowFinished
{
    [UsedImplicitly]
    public void OnSavingThrowFinished(
        RulesetActor rulesetActorCaster,
        RulesetActor rulesetActorDefender,
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
