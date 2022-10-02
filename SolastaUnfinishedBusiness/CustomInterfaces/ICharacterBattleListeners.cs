using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ICharacterTurnStartListener
{
    void OnCharacterTurnStarted(GameLocationCharacter locationCharacter);
}

internal interface ICharacterTurnEndListener
{
    void OnCharacterTurnEnded(GameLocationCharacter locationCharacter);
}

internal interface ICharacterBattleStartedListener
{
    void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise);
}

internal interface ICharacterBattleEndedListener
{
    void OnCharacterBattleEnded(GameLocationCharacter locationCharacter);
}

internal static class CharacterBattleListenersPatch
{
    /**
     * Patch implementation
     * notifies custom features that character's combat turn has starter
     */
    internal static void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
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
            listener.OnCharacterTurnStarted(locationCharacter);
        }
    }

    /**
     * Patch implementation
     * notifies custom features that character's combat turn has ended
     */
    internal static void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
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
            listener.OnCharacterTurnEnded(locationCharacter);
        }
    }

    /**
     * Patch implementation
     * notifies custom features that character's combat has starter
     */
    internal static void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
    {
        if (locationCharacter.destroying || locationCharacter.destroyedBody)
        {
            return;
        }

        var character = locationCharacter.RulesetCharacter;
        var listeners = character?.GetSubFeaturesByType<ICharacterBattleStartedListener>();

        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnCharacterBattleStarted(locationCharacter, surprise);
        }
    }

    /**
     * Patch implementation
     * notifies custom features that character's combat has ended
     */
    internal static void OnCharacterBattleEnded(GameLocationCharacter locationCharacter)
    {
        if (locationCharacter.destroying || locationCharacter.destroyedBody)
        {
            return;
        }

        var character = locationCharacter.RulesetCharacter;
        var listeners = character?.GetSubFeaturesByType<ICharacterBattleEndedListener>();

        if (listeners == null)
        {
            return;
        }

        foreach (var listener in listeners)
        {
            listener.OnCharacterBattleEnded(locationCharacter);
        }
    }
}
