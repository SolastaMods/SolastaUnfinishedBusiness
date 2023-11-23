using System.Collections.Generic;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IRollSavingThrowInitiated
{
    [UsedImplicitly]
    public void OnSavingThrowInitiated(
        RulesetCharacter caster,
        RulesetCharacter defender,
        ref int saveBonus,
        ref string abilityScoreName,
        BaseDefinition sourceDefinition,
        List<TrendInfo> modifierTrends,
        List<TrendInfo> advantageTrends,
        ref int rollModifier,
        int saveDC,
        bool hasHitVisual,
        ref RollOutcome outcome,
        ref int outcomeDelta,
        List<EffectForm> effectForms);
}
