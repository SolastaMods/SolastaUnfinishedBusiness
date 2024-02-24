using System.Collections.Generic;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IRollSavingThrowInitiated
{
    [UsedImplicitly]
    public void OnSavingThrowInitiated(
        RulesetCharacter caster,
        RulesetCharacter defender,
        ref string abilityScoreName,
        BaseDefinition sourceDefinition,
        List<TrendInfo> advantageTrends,
        int saveDC,
        bool hasHitVisual,
        List<EffectForm> effectForms);
}
