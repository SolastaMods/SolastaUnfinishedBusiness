using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Spells;
using SolastaUnfinishedBusiness.Subclasses.Builders;
using TA;
using UnityEngine;
using static MotionForm;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using GravityFissure = SolastaUnfinishedBusiness.Spells.SpellBuilders.PowerOrSpellFinishedByMeGravityFissure;

namespace SolastaUnfinishedBusiness.CustomUI;

public class CursorMotionHelper : MonoBehaviour
{
    private static GameObject _chainHelperPrefab;
    internal static readonly Vector3 Center = new(0.5f, 0.5f, 0.5f);

    private readonly List<int3> _gravityFissureTiles = [];

    private readonly Dictionary<ulong, ActionChainHelper> _helpers = [];
    private Vector3 _actingCharacterCenter = Vector3.zero;
    private int3 _aimedPosition = int3.zero;
    private IGameLocationCharacterService _characterService;

    private CursorLocation _cursor;
    private IGameLocationEnvironmentService _envService;
    [CanBeNull] private MotionInfo _info;

    private bool _isGravityFissure;
    private IGameLocationPositioningService _positioningService;
    private string _positionsKey = "";
    private IGameLocationSelectionService _selectionService;

    private GameLocationCharacter ActingCharacter => _cursor.ActionParams.ActingCharacter;

    internal static void Initialize(GameObject chainHelperPrefab)
    {
        _chainHelperPrefab = chainHelperPrefab;
    }

    internal static void Activate(CursorLocation cursor)
    {
        if (!Main.Settings.ShowMotionFormPreview) { return; }

        if (!cursor.TryGetComponent<CursorMotionHelper>(out var helper))
        {
            helper = cursor.gameObject.AddComponent<CursorMotionHelper>();
            helper.Init(cursor);
        }

        helper.DoActivate();
    }

    internal static void Deactivate(CursorLocation cursor)
    {
        if (cursor.TryGetComponent<CursorMotionHelper>(out var helper))
        {
            helper.DoDeactivate();
        }
    }

    internal static void RefreshHover(CursorLocationGeometricShape cursor)
    {
        if (!Main.Settings.ShowMotionFormPreview) { return; }

        if (cursor.TryGetComponent<CursorMotionHelper>(out var helper))
        {
            helper.RefreshAoETargets(cursor);
        }
    }

    private void Init(CursorLocation cursor)
    {
        _cursor = cursor;
    }

    private void DoActivate()
    {
        _selectionService = _cursor.SelectionService;
        _positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
        _envService = ServiceRepository.GetService<IGameLocationEnvironmentService>();
        _characterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        _actingCharacterCenter = _positioningService.ComputeGravityCenterPosition(ActingCharacter);
        _info = BuildInfo();

        if (_info == null) { return; }

        if (_cursor is not CursorLocationSelectTarget)
        {
            return;
        }

        _selectionService.CharacterHoverChange += HoverChanged;
        _selectionService.TargetSelectionChange += TargetsChanged;
    }

    private void DoDeactivate()
    {
        if (_cursor is CursorLocationSelectTarget)
        {
            _selectionService.CharacterHoverChange -= HoverChanged;
            _selectionService.TargetSelectionChange -= TargetsChanged;
        }

        _gravityFissureTiles.Clear();
        DestroyHelpers();
    }

    private void UpdateHelper([CanBeNull] GameLocationCharacter target)
    {
        if (target == null) { return; }

        var helper = GetHelper(target);

        if (!helper) { return; }

        helper.Clear();

        var shift = GetTargetShift(target);

        if (shift == int3.zero) { return; }

        var sameSide = ActingCharacter.Side == target.Side;
        var pos = target.LocationPosition + shift;
        var src = _positioningService.ComputeGravityCenterPosition(target);
        var dst = src + shift.ToVector3();

        helper.PlaceGhostWithoutPath(target, pos, sameSide);
        helper.PlaceDropLine(src, dst, sameSide);
    }

    private bool IsValidTarget(GameLocationCharacter target)
    {
        if (_info == null) { return false; }

        return _cursor is not CursorLocationSelectTarget selectTarget || selectTarget.IsValidTarget(target);
    }

    private bool HasTarget(ulong guid)
    {
        return _selectionService.SelectedTargets.Any(t => t.Guid == guid) ||
               (_cursor as CursorLocationGeometricShape)?.affectedCharacters.Any(c => c.Guid == guid) == true;
    }

    [CanBeNull]
    private ActionChainHelper GetHelper([CanBeNull] GameLocationCharacter target)
    {
        if (target == null || !IsValidTarget(target)) { return null; }

        var guid = target.Guid;

        if (_helpers.TryGetValue(guid, out var helper))
        {
            return helper;
        }

        helper = Instantiate(_chainHelperPrefab, transform).GetComponent<ActionChainHelper>();
        helper.Activate(target.RulesetCharacter);
        _helpers.Add(guid, helper);

        return helper;
    }

    private static void DestroyHelper(ActionChainHelper helper)
    {
        helper.Deactivate();
        Destroy(helper);
    }

    private void DestroyHelper(ulong guid)
    {
        if (!_helpers.TryGetValue(guid, out var helper))
        {
            return;
        }

        _helpers.Remove(guid);
        DestroyHelper(helper);
    }

    private void DestroyHelpers()
    {
        var helpers = _helpers.Values.ToList();
        _helpers.Clear();
        helpers.ForEach(DestroyHelper);
    }

    private int3 GetTargetShift(GameLocationCharacter target)
    {
        if (_info == null) { return int3.zero; }

        var src = _info.Type switch
        {
            DirectionType.Down => (target.locationPosition + new int3(0, 10, 0)).ToVector3() + Center,
            _ when _info.FromOrigin && _isGravityFissure => GetPositionForGravityFissure(target),
            _ when _info.FromOrigin => _aimedPosition.ToVector3() + Center,
            _ => _actingCharacterCenter
        };

        var reverse = _info.Type == DirectionType.Pull;
        var distance = _info.Distance;

        if (_envService.ComputePushDestination(src, target, distance, reverse, _positioningService, out var dst, out _))
        {
            return dst - target.LocationPosition;
        }

        return int3.zero;
    }

    private void HoverChanged(GameLocationCharacterSelection.HoverChangeMode mode, GameLocationCharacter character)
    {
        if (character?.RulesetCharacter == null) { return; }

        if (HasTarget(character.Guid)) { return; }

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (mode)
        {
            case GameLocationCharacterSelection.HoverChangeMode.Add:
                UpdateHelper(character);
                break;
            case GameLocationCharacterSelection.HoverChangeMode.Remove:
                DestroyHelper(character.Guid);
                break;
        }
    }

    private void RefreshAoETargets(CursorLocationGeometricShape cursor)
    {
        if (_info == null) { return; }

        var key = PosKey(cursor);
        var moved = _isGravityFissure
            ? _positionsKey != key
            : _aimedPosition != cursor.aimedPosition;

        _aimedPosition = cursor.aimedPosition;
        _positionsKey = key;

        if (!moved) { return; }

        var old = _helpers.Keys.ToList();
        var targets = GetAoETargets(cursor);
        foreach (var target in targets
                     .Where(target => target.RulesetCharacter != null)
                     .Where(target => !old.Remove(target.Guid) || _info.FromOrigin))
        {
            UpdateHelper(target);
        }

        old.ForEach(DestroyHelper);
    }

    private static string PosKey(CursorLocationGeometricShape cursor)
    {
        return string.Join(", ", cursor.coveredPlanePositions);
    }

    private List<GameLocationCharacter> GetAoETargets(CursorLocationGeometricShape cursor)
    {
        var affectedTargets = cursor.affectedCharacters.ToList();

        if (!_isGravityFissure)
        {
            return affectedTargets;
        }

        _gravityFissureTiles.Clear();

        var caster = ActingCharacter;
        var cursorHovered = cursor.HoveredPosition + new Vector3(0.0f, 0.5f, 0.0f);
        _gravityFissureTiles.AddRange(GravityFissure.GetAffectedPositions(ActingCharacter,
            _cursor.ActionParams.RulesetEffect, cursorHovered));

        var targets = GravityFissure.GetPullTargets(caster, _gravityFissureTiles, _characterService);

        return targets;
    }

    private Vector3 GetPositionForGravityFissure(GameLocationCharacter target)
    {
        return GravityFissure.GetPositionForGravityFissure(target, _gravityFissureTiles, _positioningService)
            .ToVector3() + Center;
    }

    private void TargetsChanged(
        GameLocationCharacterSelection.TargetChangeMode mode,
        GameLocationCharacter character,
        int count)
    {
        if (character?.RulesetCharacter == null) { return; }

        switch (mode)
        {
            case GameLocationCharacterSelection.TargetChangeMode.Select:
                UpdateHelper(character);
                break;
            case GameLocationCharacterSelection.TargetChangeMode.Increase:
                break;
            case GameLocationCharacterSelection.TargetChangeMode.Deselect:
                DestroyHelper(character.Guid);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    [CanBeNull]
    private MotionInfo BuildInfo()
    {
        _isGravityFissure = false;

        var effect = _cursor.ActionParams.RulesetEffect;

        if (effect == null) { return null; }

        _isGravityFissure = effect.SourceDefinition == SpellBuilders.GravityFissure;

        var character = ActingCharacter.RulesetCharacter;

        //Process Gravity Fissure
        if (_isGravityFissure)
        {
            return new MotionInfo
            {
                Distance = VerticalPushPullMotion.PullOntoCaster, Type = DirectionType.Pull, FromOrigin = true
            };
        }

        //Process Eldritch Blast
        if (effect.Name == SpellDefinitions.EldritchBlast.Name)
        {
            if (character.HasActiveInvocation(InvocationDefinitions.RepellingBlast))
            {
                return new MotionInfo { Distance = 2, Type = DirectionType.Push, FromOrigin = false };
            }

            return character.HasActiveInvocation(InvocationsBuilders.GraspingBlast)
                ? new MotionInfo { Distance = 2, Type = DirectionType.Pull, FromOrigin = false }
                : null;
        }

        //TODO: check MotionForm.MotionType.PushFromWall
        var motion = effect.EffectDescription.effectForms
            .Where(f => f.FormType == EffectForm.EffectFormType.Motion)
            .Select(f => f.MotionForm)
            .FirstOrDefault(m => m.Type
                is MotionType.DragToOrigin or MotionType.PushFromOrigin or (MotionType)ExtraMotionType.PushDown);

        if (motion == null) { return null; }

        var fromOrigin = effect.SourceDefinition.HasSubFeatureOfType<ForcePushOrDragFromEffectPoint>();

        var type = motion.Type switch
        {
            (MotionType)ExtraMotionType.PushDown => DirectionType.Down,
            MotionType.DragToOrigin => DirectionType.Pull,
            _ => DirectionType.Push
        };

        return new MotionInfo { Distance = motion.Distance, Type = type, FromOrigin = fromOrigin };
    }
}

internal class MotionInfo
{
    public int Distance;
    public bool FromOrigin;
    public DirectionType Type;
}

internal enum DirectionType
{
    Push, Pull, Down
}
