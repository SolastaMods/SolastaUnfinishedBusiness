using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
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

        public static GuiPresentation BuildGenerate(string name, Category category, AssetReferenceSprite sprite = null)
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
        /// <summary>
        /// Set the provided GuiPresentation instance on the definition owned by the builder.
        /// </summary>
        public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, GuiPresentation guiPresentation)
            where TBuilder : IBaseDefinitionBuilder
        {
            ((IBaseDefinitionBuilder)builder).SetGuiPresentation(guiPresentation);
            return builder;
        }

        /// <summary>
        /// Create and set a GuiPresentation from the provided title, description and AssetReferenceSprite.
        /// </summary>
        [Obsolete("Use alternative method.")]
        public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string title, string description, AssetReferenceSprite sprite = null)
            where TBuilder : IBaseDefinitionBuilder
        {
            return SetGuiPresentation(builder, GuiPresentationBuilder.Build(title, description, sprite));
        }

        /// <summary>
        /// Create and set a GuiPresentation from the provided name, category and AssetReferenceSprite.<br/>
        /// The Title is generated as "{category}/&amp;{name}Title".<br/>
        /// The Description is generated as "{category}/&amp;{name}Description".<br/>
        /// </summary>
        public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string name, Category category, AssetReferenceSprite sprite = null)
           where TBuilder : IBaseDefinitionBuilder
        {
            return SetGuiPresentation(builder, GuiPresentationBuilder.BuildGenerate(name, category, sprite));
        }

        /// <summary>
        /// Create and set a GuiPresentation from the provided builder, category and AssetReferenceSprite.<br/>
        /// The Title is generated as "{category}/&amp;{builder.Definition.Name}Title".<br/>
        /// The Description is generated as "{category}/&amp;{builder.Definition.Name}Description".<br/>
        /// </summary>
        /// <typeparam name="TBuilder"></typeparam>
        public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, Category category, AssetReferenceSprite sprite = null)
           where TBuilder : IBaseDefinitionBuilder
        {
            return SetGuiPresentation(builder, GuiPresentationBuilder.BuildGenerate(((IBaseDefinitionBuilder)builder).Name, category, sprite));
        }

        // TODO: More SetGuiPresentation/Generate(...) overloads as required

        /// <summary>
        /// Create a GuiPresentation with Title=Feature/&amp;NoContentTitle, Description=Feature/&amp;NoContentDescription.
        /// </summary>
        public static TBuilder SetGuiPresentationNoContent<TBuilder>(this TBuilder builder)
            where TBuilder : IBaseDefinitionBuilder
        {
            ((IBaseDefinitionBuilder)builder).SetGuiPresentation(GuiPresentationBuilder.NoContent);
            return builder;
        }
    }

    // NOTE: not used at all - remove?
    [Obsolete("Use BaseDefinitionBuilderGuiPresentationExtensions.")]
    internal static class BaseDefinitionGuiPresentationExtensions
    {
        /// <summary>
        /// Create a GuiPresentation from the provided title, description and AssetReferenceSprite.
        /// </summary>
        public static TDefinition SetGuiPresentation<TDefinition>(this TDefinition definition, string title, string description, AssetReferenceSprite sprite = null)
            where TDefinition : BaseDefinition
        {
            definition.GuiPresentation = GuiPresentationBuilder.Build(title, description, sprite);
            return definition;
        }

        /// <summary>
        /// Create a GuiPresentation from the provided name, category and AssetReferenceSprite.<br/>
        /// The Title is generated as "{category}/&amp;{name}Title".<br/>
        /// The Description is generated as "{category}/&amp;{name}Description".<br/>
        /// </summary>
        public static TDefinition SetGuiPresentation<TDefinition>(this TDefinition definition, string name, Category category, AssetReferenceSprite sprite = null)
            where TDefinition : BaseDefinition
        {
            definition.GuiPresentation = GuiPresentationBuilder.BuildGenerate(name, category, sprite);
            return definition;
        }

        /// <summary>
        /// Create a GuiPresentation from the provided name, category and AssetReferenceSprite.<br/>
        /// The Title is generated as "{category}/&amp;{definition.Name}Title".<br/>
        /// The Description is generated as "{category}/&amp;{definition.Name}Description".<br/>
        /// </summary>
        public static TDefinition SetGuiPresentation<TDefinition>(this TDefinition definition, Category category, AssetReferenceSprite sprite = null)
            where TDefinition : BaseDefinition
        {
            Preconditions.IsNotNull(definition, nameof(definition));
            definition.GuiPresentation = GuiPresentationBuilder.BuildGenerate(definition.Name, category, sprite);
            return definition;
        }

        // TODO: More SetGuiPresentation/Generate(...) overloads as required

        /// <summary>
        /// Create a GuiPresentation with Title=Feature/&amp;NoContentTitle, Description=Feature/&amp;NoContentDescription.
        /// </summary>
        public static TDefinition SetGuiPresentationNoContent<TDefinition>(this TDefinition definition)
            where TDefinition : BaseDefinition
        {
            definition.GuiPresentation = GuiPresentationBuilder.NoContent;
            return definition;
        }
    }
}
