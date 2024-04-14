using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class UnityImageExtensions
{
    internal static void SetupSprite(
        [NotNull] this Image imageComponent,
        [CanBeNull] AssetReferenceSprite spriteReference,
        bool changeActiveStatus = false)
    {
        if (imageComponent.sprite)
        {
            Gui.ReleaseAddressableAsset(imageComponent.sprite);
            imageComponent.sprite = null;
        }

        if (spriteReference != null && spriteReference.RuntimeKeyIsValid())
        {
            if (changeActiveStatus)
            {
                imageComponent.gameObject.SetActive(true);
            }

            imageComponent.sprite = Gui.LoadAssetSync<Sprite>(spriteReference);
        }
        else
        {
            if (changeActiveStatus)
            {
                imageComponent.gameObject.SetActive(false);
            }
        }
    }
}
