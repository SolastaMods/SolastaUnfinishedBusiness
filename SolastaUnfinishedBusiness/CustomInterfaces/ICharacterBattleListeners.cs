using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICharacterTurnStartListener
{
    void OnCharacterTurnStarted(GameLocationCharacter locationCharacter);
}

public interface ICharacterTurnEndListener
{
    void OnCharacterTurnEnded(GameLocationCharacter locationCharacter);
}

public interface ICharacterBattleStartedListener
{
    [UsedImplicitly]
    void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise);
}

public interface ICharacterBattleEndedListener
{
    void OnCharacterBattleEnded(GameLocationCharacter locationCharacter);
}

public static class CharacterBattleListenersPatch
{
    /**
     * Patch implementation
     * notifies custom features that character's combat turn has starter
     */
    public static void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
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
    public static void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
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
    public static void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
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
    public static void OnCharacterBattleEnded(GameLocationCharacter locationCharacter)
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
