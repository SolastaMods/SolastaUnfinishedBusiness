using System;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders
{
    public class GuiPresentationBuilder
    {
        private readonly GuiPresentation guiPresentation;

        public static string CreateTitleKey(string name, Category category)
        {
            Preconditions.IsNotNullOrWhiteSpace(name, nameof(name));

            if (category == Category.None)
            {
                throw new ArgumentException("The parameter must not be Category.None.", nameof(category));
            }

            return $"{category}/&{name}Title";
        }

        public static string CreateDescriptionKey(string description, Category category)
        {
            Preconditions.IsNotNullOrWhiteSpace(description, nameof(description));

            if (category == Category.None)
            {
                throw new ArgumentException("The parameter must not be Category.None.", nameof(category));
            }

            return $"{category}/&{description}Description";
        }

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

        public static GuiPresentation Build(string title, string description, AssetReferenceSprite sprite = null, int? sortOrder = null)
        {
            return new GuiPresentationBuilder(title, description, sprite)
                .SetSortOrder(sortOrder ?? 0)
                .Build();
        }

        public static GuiPresentation Build(string name, Category category, AssetReferenceSprite sprite = null, int? sortOrder = null)
        {
            return new GuiPresentationBuilder(CreateTitleKey(name, category), CreateDescriptionKey(name, category), sprite)
                .SetSortOrder(sortOrder ?? 0)
                .Build();
        }

        // TODO: More Build/Generate(...) overloads as required

        private const string NothingToSee = "NoContent";

        public static readonly string NoContentTitle = CreateTitleKey(NothingToSee, Category.Feature);
        public static readonly string NoContentDescription = CreateDescriptionKey(NothingToSee, Category.Feature);

        /// <summary>
        /// GuiPresentation representing 'No content title and description'
        /// </summary>
        public static GuiPresentation NoContent { get; } = Build(NothingToSee, Category.Feature);
    }

    internal static class BaseDefinitionBuilderGuiPresentationExtensions
    {
        /// <summary>
        /// Set the provided GuiPresentation instance on the definition owned by the builder.
        /// </summary>
        public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, GuiPresentation guiPresentation)
            where TBuilder : IDefinitionBuilder
        {
            ((IDefinitionBuilder)builder).SetGuiPresentation(guiPresentation);
            return builder;
        }

        /// <summary>
        /// Create and set a GuiPresentation from the provided title, description and AssetReferenceSprite.
        /// </summary>
        public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string title, string description, AssetReferenceSprite sprite = null)
            where TBuilder : IDefinitionBuilder
        {
            return SetGuiPresentation(builder, GuiPresentationBuilder.Build(title, description, sprite));
        }

        /// <summary>
        /// Create and set a GuiPresentation from the provided name, category and AssetReferenceSprite.<br/>
        /// The Title is generated as "{category}/&amp;{name}Title".<br/>
        /// The Description is generated as "{category}/&amp;{name}Description".<br/>
        /// </summary>
        public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder,
                string name, Category category, AssetReferenceSprite sprite = null, int? sortOrder = null)
            where TBuilder : IDefinitionBuilder
        {
            return SetGuiPresentation(builder, GuiPresentationBuilder.Build(name, category, sprite, sortOrder));
        }

        /// <summary>
        /// Create and set a GuiPresentation from the provided builder, category and AssetReferenceSprite.<br/>
        /// The Title is generated as "{category}/&amp;{builder.Definition.Name}Title".<br/>
        /// The Description is generated as "{category}/&amp;{builder.Definition.Name}Description".<br/>
        /// </summary>
        /// <typeparam name="TBuilder"></typeparam>
        public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder,
                Category category, AssetReferenceSprite sprite = null, int? sortOrder = null)
            where TBuilder : IDefinitionBuilder
        {
            return SetGuiPresentation(builder, GuiPresentationBuilder.Build(((IDefinitionBuilder)builder).Name, category, sprite, sortOrder));
        }

        // TODO: More SetGuiPresentation/Generate(...) overloads as required

        /// <summary>
        /// Create a GuiPresentation with Title=Feature/&amp;NoContentTitle, Description=Feature/&amp;NoContentDescription.
        /// </summary>
        public static TBuilder SetGuiPresentationNoContent<TBuilder>(this TBuilder builder)
            where TBuilder : IDefinitionBuilder
        {
            ((IDefinitionBuilder)builder).SetGuiPresentation(GuiPresentationBuilder.NoContent);
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
            definition.GuiPresentation = GuiPresentationBuilder.Build(name, category, sprite);
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
            definition.GuiPresentation = GuiPresentationBuilder.Build(definition.Name, category, sprite);
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
