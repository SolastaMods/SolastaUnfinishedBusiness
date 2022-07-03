using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Api.Extensions;

public static class UnityImageExtensions
{
    public static void SetupSprite([NotNull] this Image imageComponent, [NotNull] GuiPresentation presentation,
        bool _ = false)
    {
        SetupSprite(imageComponent, presentation.SpriteReference);
    }

    public static void SetupSprite([NotNull] this Image imageComponent,
        [CanBeNull] AssetReferenceSprite spriteReference,
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
