using System.Collections;
using JetBrains.Annotations;
using TA;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionPushedCustom : CharacterAction
#pragma warning restore CA1050
{
    private readonly bool _forceFallProne;
    private readonly bool _forceSourceCharacterTurnTowardsTargetAfterPush;
    private readonly bool _forceTurnTowardsSourceCharacterAfterPush;
    private readonly GameLocationCharacter _sourceCharacter;

    public CharacterActionPushedCustom(CharacterActionParams actionParams) : base(actionParams)
    {
        // Main.Log2($"CharacterActionPushedCustom [{ActionParams.ActingCharacter.Name}]", true);
        _forceTurnTowardsSourceCharacterAfterPush = actionParams.BoolParameter;
        _forceSourceCharacterTurnTowardsTargetAfterPush = actionParams.BoolParameter2;
        _forceFallProne = actionParams.BoolParameter4;

        if (actionParams.TargetCharacters.Count <= 0)
        {
            return;
        }

        _sourceCharacter = actionParams.TargetCharacters[0];
    }

    public override IEnumerator ExecuteImpl()
    {
        // Main.Log2($"CharacterActionPushedCustom [{ActionParams.ActingCharacter.Name}] ExecuteImpl START", true);
        if (!GameLocationCharacter.IsValidCharacter(ActionParams.ActingCharacter) || ActionParams.Positions.Count == 0)
        {
            yield break;
        }

        var character = ActionParams.ActingCharacter;
        var position = ActionParams.Positions[0];

        if (character.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
        {
            yield break;
        }

        var canStay = ServiceRepository.GetService<IGameLocationPositioningService>()
            .CanCharacterStayAtPosition(character, position, true);

        var parameters = new GameLocationCharacterDefinitions.MoveStartedParameters
        {
            GameCharacter = character,
            SourcePosition = character.LocationPosition,
            SourceOrientation = character.Orientation,
            SourceSide = character.StandingSide,
            DestinationPosition = position,
            DestinationOrientation = character.Orientation,
            DestinationSide = CellFlags.Side.Bottom,
            MoveMode = character.DefaultMoveMode,
            MoveStance = ActionDefinitions.MoveStance.Run,
            DifficultTerrain = false,
            LastMove = canStay,
            CheckStairs = false
        };

        var forceProne = false;
        var willFall = parameters.MoveStance == ActionDefinitions.MoveStance.Forced;

        if (_forceFallProne && !ActingCharacter.Prone)
        {
            forceProne = ActingCharacter.SetProne(true);
        }

        character.Pushed = true;
        character.StartMoveTo(ref parameters);

        yield return character.EventSystem.UpdateMotionsAndWaitForEvent(
            GameLocationCharacterEventSystem.Event.MovementStepEnd);

        character.StopMoving(character.Orientation);

        if (forceProne)
        {
            if (character.IsReceivingDamage)
            {
                yield return ActingCharacter.EventSystem.WaitForEvent(
                    GameLocationCharacterEventSystem.Event.ProneInAnimationEnd);
            }
            else
            {
                yield return ActingCharacter.EventSystem.WaitForEvent(
                    GameLocationCharacterEventSystem.Event.FallAnimationEnd);
            }
        }
        else if (willFall && !character.Falling && !character.IsReceivingDamage)
        {
            yield return ActingCharacter.EventSystem.WaitForEvent(
                GameLocationCharacterEventSystem.Event.FallAnimationEnd);
        }
        // else if(!willFall)
        // {
        //     ActingCharacter.EventSystem.SendEvent(GameLocationCharacterEventSystem.Event.FallAnimationEnd);
        // }

        if (_forceTurnTowardsSourceCharacterAfterPush && _sourceCharacter != null)
        {
            character.TurnTowards(_sourceCharacter);

            if (_forceSourceCharacterTurnTowardsTargetAfterPush)
            {
                _sourceCharacter.TurnTowards(character);
            }

            const GameLocationCharacterEventSystem.Event ROTATION_END =
                GameLocationCharacterEventSystem.Event.RotationEnd;

            var characterTurnCoroutine =
                Coroutine.StartCoroutine(character.EventSystem.UpdateMotionsAndWaitForEvent(ROTATION_END));
            var sourceCharacterTurnCoroutine =
                _forceSourceCharacterTurnTowardsTargetAfterPush
                    ? Coroutine.StartCoroutine(_sourceCharacter.EventSystem
                        .UpdateMotionsAndWaitForEvent(ROTATION_END))
                    : null;
            while (!characterTurnCoroutine.Empty || sourceCharacterTurnCoroutine is { Empty: false })
            {
                if (sourceCharacterTurnCoroutine is { Empty: false })
                {
                    sourceCharacterTurnCoroutine.Run();

                    if (sourceCharacterTurnCoroutine.IsFinished)
                    {
                        sourceCharacterTurnCoroutine.Reset();
                    }
                }

                if (!characterTurnCoroutine.Empty)
                {
                    characterTurnCoroutine.Run();

                    if (characterTurnCoroutine.IsFinished)
                    {
                        characterTurnCoroutine.Reset();
                    }
                }

                yield return null;
            }
        }

        character.Pushed = false;
        // Main.Log2($"CharacterActionPushedCustom [{ActionParams.ActingCharacter.Name}] ExecuteImpl FINISH", true);
    }
}
