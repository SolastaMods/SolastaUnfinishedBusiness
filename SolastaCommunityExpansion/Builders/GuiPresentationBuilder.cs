using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static SolastaModApi.BaseDefinitionBuilder;

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

        // TODO: More Build(...) overloads as required
    }

    internal static class BaseDefinitionBuilderGuiPresentationExtensions
    {
        public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, GuiPresentation guiPresentation)
            where TBuilder : IBaseDefinitionBuilder
        {
            ((IBaseDefinitionBuilder)builder).SetGuiPresentation(guiPresentation);
            return builder;
        }

        public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string title, string description, AssetReferenceSprite sprite = null)
            where TBuilder : IBaseDefinitionBuilder
        {
            return SetGuiPresentation(builder, GuiPresentationBuilder.Build(title, description, sprite));
        }

        public static TBuilder SetGuiPresentationGenerate<TBuilder>(this TBuilder builder, string name, string prefix, AssetReferenceSprite sprite = null)
           where TBuilder : IBaseDefinitionBuilder
        {
            return SetGuiPresentation(builder, CreateTitleKey(name, prefix), CreateDescriptionKey(name, prefix), sprite);
        }

        // TODO: More SetGuiPresentation/Format(...) overloads as required
    }

    internal static class BaseDefinitionGuiPresentationExtensions
    {
        public static TDefinition SetGuiPresentation<TDefinition>(this TDefinition definition, string title, string description, AssetReferenceSprite sprite = null)
            where TDefinition : BaseDefinition
        {
            definition.GuiPresentation = GuiPresentationBuilder.Build(title, description, sprite);
            return definition;
        }

        public static TDefinition SetGuiPresentationFormat<TDefinition>(this TDefinition definition, string name, string prefix, AssetReferenceSprite sprite = null)
            where TDefinition : BaseDefinition
        {
            return SetGuiPresentation(definition, CreateTitleKey(name, prefix), CreateDescriptionKey(name, prefix), sprite);
        }

        // TODO: More SetGuiPresentation/Format(...) overloads as required
    }
}
