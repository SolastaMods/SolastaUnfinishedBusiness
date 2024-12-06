using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.CustomUI;

public static class Sprites
{
    #region Map Icons

    internal static AssetReferenceSprite Teleport =>
        GetSprite("Teleport", Resources.Teleport, 24);

    #endregion

    #region Tutorial

    internal static AssetReferenceSprite TutorialActionSwitching =>
        GetSprite("TutorialActionSwitching", Resources.TutorialActionSwitching, 512, 256);

    #endregion

    #region Spells

    internal static AssetReferenceSprite FarStep =>
        GetSprite("FarStep", Resources.FarStep, 128);

    #endregion

    #region Misc

    internal static AssetReferenceSprite ArcaneWardPoints =>
        GetSprite("ArcaneWardPointsIcon", Resources.ArcaneWardPoints, 64);
    internal static AssetReferenceSprite BardicDiceResourceIcon =>
        GetSprite("BardicDiceResourceIcon", Resources.BardicDiceResourceIcon, 64);

    internal static AssetReferenceSprite ChannelDivinityResourceIcon =>
        GetSprite("ChannelDivinityResourceIcon", Resources.ChannelDivinityResourceIcon, 64);

    internal static AssetReferenceSprite GambitResourceIcon =>
        GetSprite("GambitResourceIcon", Resources.GambitResourceIcon, 64);

    internal static AssetReferenceSprite EldritchVersatilityResourceIcon =>
        GetSprite("EldritchVersatilityResourceIcon", Resources.EldritchVersatilityResourceIcon, 64);

    internal static AssetReferenceSprite SorceryPointsResourceIcon =>
        GetSprite("SorceryPointsResourceIcon", Resources.SorceryPoints, 64);

    #endregion

    #region UI

    private static AssetReferenceSprite CheckOn =>
        GetSprite("CheckOn", Resources.checkOn, 12);

    internal static Texture CheckOnTexture => GetSpriteByGuid(CheckOn.AssetGUID)?.texture;

    private static AssetReferenceSprite CheckOff =>
        GetSprite("CheckOff", Resources.checkOff, 12);

    internal static Texture CheckOffTexture => GetSpriteByGuid(CheckOff.AssetGUID)?.texture;

    private static AssetReferenceSprite Expanded =>
        GetSprite("Expanded", Resources.expanded, 12);

    internal static Texture ExpandedTexture => GetSpriteByGuid(Expanded.AssetGUID)?.texture;

    private static AssetReferenceSprite Collapsed =>
        GetSprite("Collapsed", Resources.collapsed, 12);

    internal static Texture CollapsedTexture => GetSpriteByGuid(Collapsed.AssetGUID)?.texture;

    #endregion

    #region Conditions

    internal static AssetReferenceSprite ConditionGuardian =>
        GetSprite("ConditionGuardian", Resources.ConditionGuardian, 32);

    internal static AssetReferenceSprite ConditionInfiltrate =>
        GetSprite("ConditionInfiltrate", Resources.ConditionInfiltrate, 32);

    internal static AssetReferenceSprite ConditionGambit =>
        GetSprite("ConditionGambit", Resources.ConditionGambit, 32);

    #endregion

    #region Actions

    internal static AssetReferenceSprite ActionPlaneMagic =>
        GetSprite("ActionPlaneMagic", Resources.ActionPlaneMagic, 80);

    internal static AssetReferenceSprite ActionInfuse =>
        GetSprite("ActionInfuse", Resources.ActionInfuse, 80);

    internal static AssetReferenceSprite ActionCrystalDefenseOff =>
        GetSprite("ActionCrystalDefenseOff", Resources.ActionCrystalDefenseOff, 80);

    internal static AssetReferenceSprite ActionCrystalDefenseOn =>
        GetSprite("ActionCrystalDefenseOn", Resources.ActionCrystalDefenseOn, 80);

    internal static AssetReferenceSprite ActionGambit =>
        GetSprite("ActionGambit", Resources.ActionGambit, 80);

    internal static AssetReferenceSprite ActionEldritchVersatility =>
        GetSprite("ActionEldritchVersatility", Resources.ActionEldritchVersatility, 80);

    #endregion

    #region Weapon Icons

    #region Katana Icons
    internal static AssetReferenceSprite _katanaIcon;

    [NotNull]
    internal static AssetReferenceSprite KatanaIcon =>
    _katanaIcon ??= Sprites.GetSprite("Katana", SolastaUnfinishedBusiness.Properties.Resources.Katana, 128);

    #endregion

    #region Halberd Icons

    internal static AssetReferenceSprite
        _halberdIcon,
        _halberdPrimedIcon,
        _halberdP1Icon,
        _halberdP2Icon,
        _halberdP3Icon,
        _halberdLightningIcon;

    [NotNull]
    internal static AssetReferenceSprite HalberdIcon =>
        _halberdIcon ??= Sprites.GetSprite("Halberd", SolastaUnfinishedBusiness.Properties.Resources.Halberd, 128);

    [NotNull]
    internal static AssetReferenceSprite HalberdPrimedIcon => _halberdPrimedIcon ??=
        Sprites.GetSprite("HalberdPrimed", SolastaUnfinishedBusiness.Properties.Resources.HalberdPrimed, 128);

    [NotNull]
    internal static AssetReferenceSprite HalberdP1Icon => _halberdP1Icon ??=
        Sprites.GetSprite("Halberd_1", SolastaUnfinishedBusiness.Properties.Resources.Halberd_1, 128);

    [NotNull]
    internal static AssetReferenceSprite HalberdP2Icon => _halberdP2Icon ??=
        Sprites.GetSprite("Halberd_2", SolastaUnfinishedBusiness.Properties.Resources.Halberd_2, 128);

    [NotNull]
    internal static AssetReferenceSprite HalberdP3Icon => _halberdP3Icon ??=
        Sprites.GetSprite("Halberd_3", SolastaUnfinishedBusiness.Properties.Resources.Halberd_2, 128);

    [NotNull]
    internal static AssetReferenceSprite HalberdLightningIcon => _halberdLightningIcon ??=
        Sprites.GetSprite("HalberdLightning", SolastaUnfinishedBusiness.Properties.Resources.HalberdLightning, 128);

        #endregion

        #region Pike Icons

    internal static AssetReferenceSprite
        _pikeIcon,
        _pikePrimedIcon,
        _pikeP1Icon,
        _pikeP2Icon,
        _pikeP3Icon,
        _pikeLightningIcon;

    [NotNull]
    internal static AssetReferenceSprite PikeIcon =>
        _pikeIcon ??= Sprites.GetSprite("Pike", SolastaUnfinishedBusiness.Properties.Resources.Pike, 128);

    [NotNull]
    internal static AssetReferenceSprite PikePrimedIcon => _pikePrimedIcon ??=
        Sprites.GetSprite("PikePrimed", SolastaUnfinishedBusiness.Properties.Resources.PikePrimed, 128);

    [NotNull]
    internal static AssetReferenceSprite PikeP1Icon => _pikeP1Icon ??=
        Sprites.GetSprite("Pike_1", SolastaUnfinishedBusiness.Properties.Resources.Pike_1, 128);

    [NotNull]
    internal static AssetReferenceSprite PikeP2Icon => _pikeP2Icon ??=
        Sprites.GetSprite("Pike_2", SolastaUnfinishedBusiness.Properties.Resources.Pike_2, 128);

    [NotNull]
    internal static AssetReferenceSprite PikeP3Icon => _pikeP3Icon ??=
        Sprites.GetSprite("Pike_3", SolastaUnfinishedBusiness.Properties.Resources.Pike_2, 128);

    [NotNull]
    internal static AssetReferenceSprite PikePsychicIcon => _pikeLightningIcon ??=
        Sprites.GetSprite("PikePsychic", SolastaUnfinishedBusiness.Properties.Resources.PikePsychic, 128);

    #endregion

        #region Long Mace Icons

    internal static AssetReferenceSprite
        _longMaceIcon,
        _longMacePrimedIcon,
        _longMaceP1Icon,
        _longMaceP2Icon,
        _longMaceP3Icon,
        _longMaceLightningIcon;

    [NotNull]
    internal static AssetReferenceSprite LongMaceIcon =>
        _longMaceIcon ??= Sprites.GetSprite("LongMace", SolastaUnfinishedBusiness.Properties.Resources.LongMace, 128);

    [NotNull]
    internal static AssetReferenceSprite LongMacePrimedIcon => _longMacePrimedIcon ??=
        Sprites.GetSprite("LongMacePrimed", SolastaUnfinishedBusiness.Properties.Resources.LongMacePrimed, 128);

    [NotNull]
    internal static AssetReferenceSprite LongMaceP1Icon => _longMaceP1Icon ??=
        Sprites.GetSprite("LongMace_1", SolastaUnfinishedBusiness.Properties.Resources.LongMace_1, 128);

    [NotNull]
    internal static AssetReferenceSprite LongMaceP2Icon => _longMaceP2Icon ??=
        Sprites.GetSprite("LongMace_2", SolastaUnfinishedBusiness.Properties.Resources.LongMace_2, 128);

    [NotNull]
    internal static AssetReferenceSprite LongMaceP3Icon => _longMaceP3Icon ??=
        Sprites.GetSprite("LongMace_3", SolastaUnfinishedBusiness.Properties.Resources.LongMace_2, 128);

    [NotNull]
    internal static AssetReferenceSprite LongMaceThunderIcon => _longMaceLightningIcon ??=
        Sprites.GetSprite("LongMaceThunder", SolastaUnfinishedBusiness.Properties.Resources.LongMaceThunder, 128);

    #endregion

        #region Hand Crossbow Icons

    internal static AssetReferenceSprite _handXbowIcon,
        _handXbowPrimedIcon,
        _handXbowP1Icon,
        _handXbowP2Icon,
        _handXbowP3Icon,
        _handXbowAcidIcon;

    [NotNull]
    internal static AssetReferenceSprite HandXbowIcon =>
        _handXbowIcon ??= Sprites.GetSprite("HandXbow", SolastaUnfinishedBusiness.Properties.Resources.HandXbow, 128);

    [NotNull]
    internal static AssetReferenceSprite HandXbowPrimedIcon => _handXbowPrimedIcon ??=
        Sprites.GetSprite("HandXbowPrimed", SolastaUnfinishedBusiness.Properties.Resources.HandXbowPrimed, 128);

    [NotNull]
    internal static AssetReferenceSprite HandXbowP1Icon => _handXbowP1Icon ??=
        Sprites.GetSprite("HandXbow_1", SolastaUnfinishedBusiness.Properties.Resources.HandXbow_1, 128);

    [NotNull]
    internal static AssetReferenceSprite HandXbowP2Icon => _handXbowP2Icon ??=
        Sprites.GetSprite("HandXbow_2", SolastaUnfinishedBusiness.Properties.Resources.HandXbow_2, 128);

    [NotNull]
    internal static AssetReferenceSprite HandXbowP3Icon => _handXbowP3Icon ??=
        Sprites.GetSprite("HandXbow_3", SolastaUnfinishedBusiness.Properties.Resources.HandXbow_2, 128);

    [NotNull]
    internal static AssetReferenceSprite HandXbowAcidIcon => _handXbowAcidIcon ??=
        Sprites.GetSprite("HandXbowAcid", SolastaUnfinishedBusiness.Properties.Resources.HandXbowAcid, 128);

    #endregion

        #region Produced Flame Icons

    internal static AssetReferenceSprite _producedFlameThrow;

    [NotNull]
    internal static AssetReferenceSprite ProducedFlameThrow => _producedFlameThrow ??=
        Sprites.GetSprite("ProducedFlameThrow", SolastaUnfinishedBusiness.Properties.Resources.ProducedFlameThrow, 128);

        #endregion

    #endregion

    #region CustomSprites

    private static readonly Dictionary<string, Sprite> SpritesByGuid = new();

    /// <summary>
    ///     Convert a bitmap stored as an embedded resource to a Sprite.
    /// </summary>
    [NotNull]
    internal static Sprite GetOrCreateSprite(
        string name,
        Byte[] bitmap,
        int size)
    {
        return GetOrCreateSprite(name, bitmap, size, size);
    }

    [NotNull]
    private static Sprite GetOrCreateSprite(
        string name,
        Byte[] bitmap,
        int sizeX,
        int sizeY)
    {
        var (id, guid) = GetSpriteId(name, sizeX, sizeY);

        if (SpritesByGuid.TryGetValue(guid, out var sprite))
        {
            return sprite;
        }

        var texture = new Texture2D(sizeX, sizeY, TextureFormat.DXT5, false);

        texture.LoadImage(bitmap);
        sprite = Sprite.Create(texture, new Rect(0, 0, sizeX, sizeY), new Vector2(0, 0));

        SpritesByGuid[guid] = sprite;
        sprite.name = id;

        return sprite;
    }

    [CanBeNull]
    internal static Sprite GetSpriteByGuid([NotNull] string guid)
    {
        return SpritesByGuid.TryGetValue(guid, out var sprite) ? sprite : null;
    }

    /// <summary>
    ///     Create a unique Id to serve as id of a sprite in our internal cache and as the guid for AssetReference
    /// </summary>
    /// <param name="name"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private static (string id, string guid) GetSpriteId(string name, int x, int y)
    {
        var id = $"_CE_{name}[{x},{y}]";

        return (id, GetSpriteGuid(id));
    }

    [NotNull]
    private static string GetSpriteGuid([NotNull] string id)
    {
        return GuidHelper.Create(DefinitionBuilder.CeNamespaceGuid, id).ToString();
    }

    [NotNull]
    internal static AssetReferenceSprite GetSprite(
        string name,
        Byte[] bitmap,
        int size)
    {
        return GetSprite(name, bitmap, size, size);
    }

    [NotNull]
    internal static AssetReferenceSprite GetSprite(
        string name,
        Byte[] bitmap,
        int sizeX,
        int sizeY)
    {
        var sprite = GetOrCreateSprite(name, bitmap, sizeX, sizeY);

        return new AssetReferenceSprite(GetSpriteGuid(sprite.name));
    }

    #endregion

    

}



