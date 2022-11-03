using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Utils;

public static class CustomSprite
{
    #region Conditions

    internal static AssetReferenceSprite ConditionGuardian =>
        CustomIcons.GetSprite("ConditionGuardian", Resources.ConditionGuardian, 32);

    internal static AssetReferenceSprite ConditionInfiltrate =>
        CustomIcons.GetSprite("ConditionInfiltrate", Resources.ConditionInfiltrate, 32);

    #endregion

    #region Powers

    internal static AssetReferenceSprite PowerGuardianMode =>
        CustomIcons.GetSprite("PowerGuardianMode", Resources.PowerGuardianMode, 256, 128);

    internal static AssetReferenceSprite PowerInfiltratorMode =>
        CustomIcons.GetSprite("PowerInfiltratorMode", Resources.PowerInfiltratorMode, 256, 128);

    internal static AssetReferenceSprite PowerDefensiveField =>
        CustomIcons.GetSprite("PowerDefensiveField", Resources.PowerDefensiveField, 256, 128);

    #endregion

    #region Items

    internal static AssetReferenceSprite ItemThundergauntlet =>
        CustomIcons.GetSprite("ItemThundergauntlet", Resources.ItemThundergauntlet, 128);

    internal static AssetReferenceSprite ItemGemLightning =>
        CustomIcons.GetSprite("ItemGemLightning", Resources.ItemGemLightning, 128);

    #endregion
}
