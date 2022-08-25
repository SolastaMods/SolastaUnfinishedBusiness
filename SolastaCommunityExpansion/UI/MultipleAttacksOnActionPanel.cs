using SolastaCommunityExpansion.Api.Extensions;

namespace SolastaCommunityExpansion.UI;

public static class MultipleAttacksOnActionPanel
{
    /**
     * Patch implementation
     * Used to allow multiple attacks on action panel
     * Skips specified amount of attack modes for main and bonus action
     */
    internal static RulesetAttackMode GetExtraAttackMode(GameLocationCharacter locChar, RulesetAttackMode def,
        ActionDefinitions.Id actionId)
    {
        var skip = locChar.GetSkipAttackModes();

        if (skip == 0)
        {
            return def;
        }

        var skipped = 0;

        var result = def;
        switch (actionId)
        {
            case ActionDefinitions.Id.AttackMain:
            case ActionDefinitions.Id.AttackOff:
                foreach (var current in locChar.RulesetCharacter.AttackModes)
                {
                    var found = false;
                    if (current.AfterChargeOnly)
                    {
                        continue;
                    }

                    switch (current.ActionType)
                    {
                        case ActionDefinitions.ActionType.Main:
                            found = actionId == ActionDefinitions.Id.AttackMain;
                            break;
                        case ActionDefinitions.ActionType.Bonus:
                            found = actionId == ActionDefinitions.Id.AttackOff;
                            break;
                    }

                    if (found)
                    {
                        result = current;
                        if (skipped == skip)
                        {
                            break;
                        }

                        skipped++;
                    }
                }

                break;
            default:
                return def;
        }

        return result;
    }
}