using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomInterfaces;

public interface IOnRollAttackMode
{
    void OnRollAttackMode(
        RulesetCharacter attacker,
        ref RulesetAttackMode attackMode, 
        RulesetActor target,
        BaseDefinition attackMethod,
        ref List<RuleDefinitions.TrendInfo> toHitTrends,
        bool ignoreAdvantage, 
        ref List<RuleDefinitions.TrendInfo> advantageTrends,
        bool opportunity, 
        int rollModifier);
}

public delegate void OnRollAttackModeDelegate(
    RulesetCharacter attacker,
    ref RulesetAttackMode attackMode, 
    RulesetActor target,
    BaseDefinition attackMethod,
    ref List<RuleDefinitions.TrendInfo> toHitTrends,
    bool ignoreAdvantage, 
    ref List<RuleDefinitions.TrendInfo> advantageTrends,
    bool opportunity, 
    int rollModifier);
