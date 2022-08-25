using SolastaCommunityExpansion.Api.Extensions;

namespace SolastaCommunityExpansion.CustomInterfaces;

public interface ICharacterTurnStartListener
{
    void OnChracterTurnStarted(GameLocationCharacter locationCharacter);
}

public interface ICharacterTurnEndListener
{
    void OnChracterTurnEnded(GameLocationCharacter locationCharacter);
}

public interface ICharacterBattlStartedListener
{
    void OnChracterBattleStarted(GameLocationCharacter locationCharacter, bool surprise);
}

public interface ICharacterBattlEndedListener
{
    void OnChracterBattleEnded(GameLocationCharacter locationCharacter);
}

internal static class CharacterBattleListenersPatch
{
    /**
     * Patch implementation
     * notifies custom features that character's combat turn has starter
     */
    internal static void OnChracterTurnStarted(GameLocationCharacter locationCharacter)
    {
        if (locationCharacter.destroying || locationCharacter.destroyedBody)
        {
            return;
        }

        var character = locationCharacter.RulesetCharacter;
        var listeners = character?.GetSubFeaturesByType<ICharacterTurnStartListener>();

        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnChracterTurnStarted(locationCharacter);
        }
    }

    /**
     * Patch implementation
     * notifies custom features that character's combat turn has ended
     */
    internal static void OnChracterTurnEnded(GameLocationCharacter locationCharacter)
    {
        if (locationCharacter.destroying || locationCharacter.destroyedBody)
        {
            return;
        }

        var character = locationCharacter.RulesetCharacter;
        var listeners = character?.GetSubFeaturesByType<ICharacterTurnEndListener>();

        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnChracterTurnEnded(locationCharacter);
        }
    }

    /**
     * Patch implementation
     * notifies custom features that character's combat has starter
     */
    internal static void OnChracterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
    {
        if (locationCharacter.destroying || locationCharacter.destroyedBody)
        {
            return;
        }

        var character = locationCharacter.RulesetCharacter;
        var listeners = character?.GetSubFeaturesByType<ICharacterBattlStartedListener>();

        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnChracterBattleStarted(locationCharacter, surprise);
        }
    }

    /**
     * Patch implementation
     * notifies custom features that character's combat has ended
     */
    internal static void OnChracterBattleEnded(GameLocationCharacter locationCharacter)
    {
        if (locationCharacter.destroying || locationCharacter.destroyedBody)
        {
            return;
        }

        var character = locationCharacter.RulesetCharacter;
        var listeners = character?.GetSubFeaturesByType<ICharacterBattlEndedListener>();

        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnChracterBattleEnded(locationCharacter);
        }
    }
}