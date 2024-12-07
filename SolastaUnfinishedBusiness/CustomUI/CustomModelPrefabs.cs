using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public static class CustomModelPrefabs
{
    private static AssetReference _katanaPrefab;

    private static AssetReference _launcherPrefab;

    private static AssetReference _thunderGauntletPrefab;

    private static AssetReference _longMacePrefab;

    private static AssetReference _pikePrefab;

    public static AssetReference GetKatanaPrefab()
    {
        return _katanaPrefab ??= CustomModels.GetCustomModelPrefab("Katana");
    }

    public static AssetReference GetLauncherPrefab()
    {
        return _launcherPrefab ??= CustomModels.GetCustomModelPrefab("LightningLauncher");
    }

    public static AssetReference GetThunderGauntletPrefab()
    {
        return _thunderGauntletPrefab ??= CustomModels.GetCustomModelPrefab("ThunderGauntlet");
    }

    public static AssetReference GetLongMacePrefab()
    {
        return _longMacePrefab ??= CustomModels.GetCustomModelPrefab("LongMace");
    }

    public static AssetReference GetPikePrefab()
    {
        return _pikePrefab ??= CustomModels.GetCustomModelPrefab("Pike");
    }
}
