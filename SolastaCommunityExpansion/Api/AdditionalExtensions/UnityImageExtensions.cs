using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Api.AdditionalExtensions;

public static class UnityImageExtensions
{
    public static void SetupSprite(this Image imageComponent, GuiPresentation presentation,
        bool changeActiveStatus = false)
    {
        SetupSprite(imageComponent, presentation.SpriteReference);
    }

    public static void SetupSprite(this Image imageComponent, AssetReferenceSprite spriteReference,
        bool changeActiveStatus = false)
    {
        if (imageComponent.sprite != null)
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
