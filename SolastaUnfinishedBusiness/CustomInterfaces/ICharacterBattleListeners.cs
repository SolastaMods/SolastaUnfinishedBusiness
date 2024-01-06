using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Spells;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICharacterBeforeTurnStartListener
{
    void OnCharacterBeforeTurnStarted(GameLocationCharacter locationCharacter);
}

public interface ICharacterTurnStartListener
{
    void OnCharacterTurnStarted(GameLocationCharacter locationCharacter);
}

public interface ICharacterTurnEndListener
{
    void OnCharacterTurnEnded(GameLocationCharacter locationCharacter);
}

public interface IInitiativeEndListener
{
    IEnumerator OnInitiativeEnded(GameLocationCharacter locationCharacter);
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
     * notifies custom features before that character's combat turn has starter
     */
    public static void OnCharacterBeforeTurnStarted(GameLocationCharacter locationCharacter)
    {
        if (locationCharacter.destroying || locationCharacter.destroyedBody)
        {
            return;
        }

        var rulesetCharacter = locationCharacter.RulesetCharacter;

        if (rulesetCharacter == null)
        {
            return;
        }

        foreach (var listener in rulesetCharacter.GetSubFeaturesByType<ICharacterBeforeTurnStartListener>())
        {
            listener.OnCharacterBeforeTurnStarted(locationCharacter);
        }
    }

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

        if (locationCharacter.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } rulesetCharacter)
        {
            return;
        }

        //PATCH: supports vigilance feature on Martial Guardian
        MartialGuardian.HandleVigilance(rulesetCharacter);
        SpellBuilders.HandleSkinOfRetribution();

        //PATCH: supports EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack
        if (Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack &&
            rulesetCharacter.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Monk) > 0)
        {
            rulesetCharacter.InflictCondition(
                "ConditionMonkMartialArtsUnarmedStrikeBonus",
                RuleDefinitions.DurationType.Round,
                0,
                RuleDefinitions.TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.Guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                "ConditionMonkMartialArtsUnarmedStrikeBonus",
                0,
                0,
                0);
        }

        foreach (var listener in rulesetCharacter.GetSubFeaturesByType<ICharacterTurnStartListener>())
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

        if (locationCharacter.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } rulesetCharacter)
        {
            return;
        }

        var listeners = rulesetCharacter.GetSubFeaturesByType<ICharacterTurnEndListener>();

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

        if (locationCharacter.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } rulesetCharacter)
        {
            return;
        }

        var listeners = rulesetCharacter.GetSubFeaturesByType<ICharacterBattleStartedListener>();

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

        if (locationCharacter.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } rulesetCharacter)
        {
            return;
        }

        var listeners = rulesetCharacter.GetSubFeaturesByType<ICharacterBattleEndedListener>();

        foreach (var listener in listeners)
        {
            listener.OnCharacterBattleEnded(locationCharacter);
        }
    }
}
