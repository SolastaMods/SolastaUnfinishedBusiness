using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using TA;
using UnityEngine;
using static MotionForm;

namespace SolastaUnfinishedBusiness.CustomUI;

public class CursorMotionHelper : MonoBehaviour
{
    private static GameObject ChainHelperPrefab;
    private static readonly Vector3 CENTER = new(0.5f, 0.5f, 0.5f);

    private CursorLocation Cursor;
    private IGameLocationSelectionService SelectionService;
    private IGameLocationPositioningService PositioningService;
    private IGameLocationEnvironmentService EnvService;
    [CanBeNull] private MotionInfo Info;
    private int3 AimedPosition = int3.zero;

    private readonly Dictionary<ulong, ActionChainHelper> Helpers = new();

    private GameLocationCharacter ActingCharacter => Cursor.ActionParams.ActingCharacter;

    internal static void Initialize(GameObject chainHelperPrefab)
    {
        ChainHelperPrefab = chainHelperPrefab;
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
        Cursor = cursor;
    }

    private void DoActivate()
    {
        SelectionService = Cursor.SelectionService;
        PositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
        EnvService = ServiceRepository.GetService<IGameLocationEnvironmentService>();

        Info = BuildInfo();
        if (Info == null) { return; }

        if (Cursor is CursorLocationSelectTarget)
        {
            SelectionService.CharacterHoverChange += HoverChanged;
            SelectionService.TargetSelectionChange += TargetsChanged;
        }
    }

    private void DoDeactivate()
    {
        if (Cursor is CursorLocationSelectTarget)
        {
            SelectionService.CharacterHoverChange -= HoverChanged;
            SelectionService.TargetSelectionChange -= TargetsChanged;
        }

        DestroyHelpers();
    }

    private void UpdateHelper([CanBeNull] GameLocationCharacter target)
    {
        if (target == null) { return; }

        var helper = GetHelper(target);
        if (helper == null) { return; }

        helper.Clear();
        var shift = GetTargetShift(target);
        if (shift == int3.zero) { return; }

        var sameSide = ActingCharacter.Side == target.Side;
        var pos = target.LocationPosition + shift;
        var src = PositioningService.ComputeGravityCenterPosition(target);
        var dst = src + shift.ToVector3();

        helper.PlaceGhostWithoutPath(target, pos, sameSide);
        helper.PlaceDropLine(src, dst, sameSide);
    }

    private bool IsValidTarget(GameLocationCharacter target)
    {
        if (Info == null) { return false; }

        if (Cursor is not CursorLocationSelectTarget selectTarget) { return true; }

        return selectTarget.IsValidTarget(target);
    }

    private bool HasTarget(ulong guid)
    {
        return SelectionService.SelectedTargets.Any(t => t.Guid == guid)
               || (Cursor as CursorLocationGeometricShape)?.affectedCharacters.Any(c => c.Guid == guid) == true;
    }

    [CanBeNull]
    ActionChainHelper GetHelper([CanBeNull] GameLocationCharacter target)
    {
        if (target == null || !IsValidTarget(target)) { return null; }

        var guid = target.Guid;
        if (!Helpers.TryGetValue(guid, out var helper))
        {
            helper = Instantiate(ChainHelperPrefab, this.transform).GetComponent<ActionChainHelper>();
            helper.Activate(target.RulesetCharacter);
            Helpers.Add(guid, helper);
        }

        return helper;
    }

    private void DestroyHelper(ActionChainHelper helper)
    {
        helper.Deactivate();
        Destroy(helper);
    }

    private void DestroyHelper(ulong guid)
    {
        if (Helpers.TryGetValue(guid, out var helper))
        {
            Helpers.Remove(guid);
            DestroyHelper(helper);
        }
    }

    private void DestroyHelpers()
    {
        var helpers = Helpers.Values.ToList();
        Helpers.Clear();
        helpers.ForEach(DestroyHelper);
    }

    private int3 GetTargetShift(GameLocationCharacter target)
    {
        if (Info == null) { return int3.zero; }

        //TODO: cache acting character and their gravity on Activate?
        var src = Info.Type switch
        {
            DirectionType.Down => (target.locationPosition + new int3(0, 10, 0)).ToVector3() + CENTER,
            _ when Info.FromOrigin => AimedPosition.ToVector3() + CENTER,
            _ => PositioningService.ComputeGravityCenterPosition(ActingCharacter)
        };

        var reverse = Info.Type == DirectionType.Pull;
        var distance = Info.Distance;
        if (EnvService.ComputePushDestination(src, target, distance, reverse, PositioningService, out var dst, out _))
        {
            return dst - target.LocationPosition;
        }

        return int3.zero;
    }

    private void HoverChanged(GameLocationCharacterSelection.HoverChangeMode mode, GameLocationCharacter character)
    {
        if (character?.RulesetCharacter == null) { return; }

        if (HasTarget(character.Guid)) { return; }

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
        if (Info == null) { return; }

        var old = Helpers.Keys.ToList();
        var targets = cursor.affectedCharacters.ToList();
        var moved = AimedPosition != cursor.aimedPosition;
        AimedPosition = cursor.aimedPosition;
        foreach (var target in targets)
        {
            if (target.RulesetCharacter == null) { continue; }

            if (old.Remove(target.Guid) && !moved)
            {
                continue;
            }

            UpdateHelper(target);
        }

        old.ForEach(DestroyHelper);
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
        var effect = Cursor.ActionParams.RulesetEffect;
        if (effect == null) { return null; }

        //TODO: check special cases like Eldritch Blast with invocations
        //TODO: check MotionForm.MotionType.PushFromWall
        var motion = effect.EffectDescription.effectForms
            .Where(f => f.FormType == EffectForm.EffectFormType.Motion)
            .Select(f => f.MotionForm)
            .FirstOrDefault(m => m.Type
                is MotionType.DragToOrigin or MotionType.PushFromOrigin or (MotionType)ExtraMotionType.PushDown);

        if (motion == null) { return null; }

        var fromOrigin = effect.SourceDefinition.HasSubFeatureOfType<ForcePushOrDragFromEffectPoint>();

        DirectionType type = motion.Type switch
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
    public DirectionType Type;
    public bool FromOrigin;
}

internal enum DirectionType
{
    Push, Pull, Down
}
