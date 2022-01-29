using SolastaModApi.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
    public class GuiPresentationBuilder
    {
        private readonly GuiPresentation guiPresentation;

        public GuiPresentationBuilder(string description = null, string title = null, AssetReferenceSprite sprite = null)
        {
            guiPresentation = new GuiPresentation
            {
                Description = description ?? string.Empty,
                Title = title ?? string.Empty
            };

            SetSpriteReference(sprite ?? new AssetReferenceSprite(string.Empty));
        }

        public GuiPresentationBuilder SetColor(Color color)
        {
            guiPresentation.SetColor(color);
            return this;
        }

        public GuiPresentationBuilder SetHidden(bool hidden)
        {
            guiPresentation.SetHidden(hidden);
            return this;
        }

        public GuiPresentationBuilder SetSortOrder(int sortOrder)
        {
            guiPresentation.SetSortOrder(sortOrder);
            return this;
        }

        public GuiPresentationBuilder SetSpriteReference(AssetReferenceSprite sprite)
        {
            guiPresentation.SetSpriteReference(sprite);
            return this;
        }

        public GuiPresentation Build()
        {
            return guiPresentation;
        }

        public static GuiPresentation Build(string title, string description, AssetReferenceSprite sprite = null)
        {
            return new GuiPresentationBuilder(description, title, sprite).Build();
        }
    }
}
