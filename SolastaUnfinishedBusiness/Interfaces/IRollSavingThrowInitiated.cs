using System.Collections.Generic;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IRollSavingThrowInitiated
{
    [UsedImplicitly]
    public void OnSavingThrowInitiated(
        RulesetActor rulesetActorCaster,
        RulesetActor rulesetActorDefender,
        ref int saveBonus,
        ref string abilityScoreName,
        BaseDefinition sourceDefinition,
        List<TrendInfo> modifierTrends,
        List<TrendInfo> advantageTrends,
        ref int rollModifier,
        ref int saveDC,
        ref bool hasHitVisual,
        RollOutcome outcome,
        int outcomeDelta,
        List<EffectForm> effectForms);
}
