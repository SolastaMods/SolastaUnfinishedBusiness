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

        public GuiPresentationBuilder(string title = null, string description = null, AssetReferenceSprite sprite = null)
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
            return new GuiPresentationBuilder(title, description, sprite).Build();
        }

        public static GuiPresentation BuildGenerate(string name, string category, AssetReferenceSprite sprite = null)
        {
            return new GuiPresentationBuilder(CreateTitleKey(name, category), CreateDescriptionKey(name, category), sprite).Build();
        }

        // TODO: More Build/Generate(...) overloads as required

        /// <summary>
        /// GuiPresentation representing 'No content title and description'
        /// </summary>
        public static GuiPresentation NoContent { get; } = Build("Feature/&NoContentTitle", "Feature/&NoContentTitle");
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

        public static TBuilder SetGuiPresentationGenerate<TBuilder>(this TBuilder builder, string name, string category, AssetReferenceSprite sprite = null)
           where TBuilder : IBaseDefinitionBuilder
        {
            return SetGuiPresentation(builder, GuiPresentationBuilder.BuildGenerate(name, category, sprite));
        }

        // TODO: More SetGuiPresentation/Generate(...) overloads as required

        public static TBuilder SetGuiPresentationNoContent<TBuilder>(this TBuilder builder)
            where TBuilder : IBaseDefinitionBuilder
        {
            ((IBaseDefinitionBuilder)builder).SetGuiPresentation(GuiPresentationBuilder.NoContent);
            return builder;
        }
    }

    internal static class BaseDefinitionGuiPresentationExtensions
    {
        public static TDefinition SetGuiPresentation<TDefinition>(this TDefinition definition, string title, string description, AssetReferenceSprite sprite = null)
            where TDefinition : BaseDefinition
        {
            definition.GuiPresentation = GuiPresentationBuilder.Build(title, description, sprite);
            return definition;
        }

        public static TDefinition SetGuiPresentationGenerate<TDefinition>(this TDefinition definition, string name, string category, AssetReferenceSprite sprite = null)
            where TDefinition : BaseDefinition
        {
            definition.GuiPresentation = GuiPresentationBuilder.BuildGenerate(name, category, sprite);
            return definition;
        }

        // TODO: More SetGuiPresentation/Generate(...) overloads as required

        public static TDefinition SetGuiPresentationNoContent<TDefinition>(this TDefinition definition)
            where TDefinition : BaseDefinition
        {
            definition.GuiPresentation = GuiPresentationBuilder.NoContent;
            return definition;
        }
    }
}
