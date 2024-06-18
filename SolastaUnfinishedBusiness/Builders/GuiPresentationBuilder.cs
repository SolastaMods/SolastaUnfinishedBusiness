using SolastaUnfinishedBusiness.Api.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Builders;

internal class GuiPresentationBuilder
{
    internal const string EmptyString = "Feature/&Emptystring";
    internal static readonly AssetReferenceSprite EmptySprite = new(string.Empty);
    private readonly GuiPresentation _guiPresentation;

    internal GuiPresentationBuilder(
        string title = null,
        string description = null,
        AssetReferenceSprite spriteReference = null)
    {
        _guiPresentation = new GuiPresentation
        {
            Description = description ?? string.Empty,
            Title = title ?? string.Empty,
            spriteReference = spriteReference ?? EmptySprite
        };
    }

    internal GuiPresentationBuilder(GuiPresentation reference)
        : this(reference.Title, reference.Description, reference.SpriteReference)
    {
    }

    internal static GuiPresentation NoContent { get; } = Build(Gui.NoLocalization, Gui.NoLocalization, EmptySprite);

    internal static GuiPresentation NoContentHidden { get; } =
        Build(Gui.NoLocalization, Gui.NoLocalization, EmptySprite, 0, true);

    internal static string CreateTitleKey(string name, Category category)
    {
        // ReSharper disable once InvocationIsSkipped
        PreConditions.IsNotNullOrWhiteSpace(name, nameof(name));

        return $"{category}/&{name}Title";
    }

    internal static string CreateDescriptionKey(string description, Category category)
    {
        // ReSharper disable once InvocationIsSkipped
        PreConditions.IsNotNullOrWhiteSpace(description, nameof(description));

        return $"{category}/&{description}Description";
    }

    internal GuiPresentationBuilder SetTitle(string title)
    {
        _guiPresentation.title = title;
        return this;
    }

#if false
    internal GuiPresentationBuilder SetColor(Color color)
    {
        guiPresentation.color = color;
        return this;
    }

    internal GuiPresentationBuilder SetHidden(bool hidden)
    {
        guiPresentation.hidden = hidden;
        return this;
    }

    internal GuiPresentationBuilder SetSortOrder(int sortOrder)
    {
        guiPresentation.sortOrder = sortOrder;
        return this;
    }
#endif

    internal GuiPresentation Build()
    {
        return _guiPresentation;
    }

    private static GuiPresentation Build(
        string title,
        string description,
        AssetReferenceSprite sprite = null,
        int? sortOrder = null,
        bool? hidden = null)
    {
        return Build(null, title, description, sprite, sortOrder, hidden);
    }

    internal static GuiPresentation Build(
        GuiPresentation reference,
        string title,
        string description,
        AssetReferenceSprite sprite = null,
        int? sortOrder = null,
        bool? hidden = null)
    {
        var guiPresentation = reference == null ? new GuiPresentation() : new GuiPresentation(reference);

        guiPresentation.Title = title;
        guiPresentation.Description = description;
        guiPresentation.spriteReference = sprite ?? reference?.SpriteReference ?? EmptySprite;
        guiPresentation.sortOrder = sortOrder ?? reference?.SortOrder ?? 0;
        guiPresentation.hidden = hidden ?? reference?.Hidden ?? false;

        return guiPresentation;
    }

    internal static GuiPresentation Build(
        string name,
        Category category,
        AssetReferenceSprite sprite = null,
        int? sortOrder = null,
        bool? hidden = null)
    {
        return Build(null, name, category, sprite, sortOrder, hidden);
    }

    internal static GuiPresentation Build(
        GuiPresentation reference,
        string name,
        Category category,
        AssetReferenceSprite sprite = null,
        int? sortOrder = null,
        bool? hidden = null)
    {
        return Build(
            reference, CreateTitleKey(name, category), CreateDescriptionKey(name, category), sprite, sortOrder, hidden);
    }
}

internal static class BaseDefinitionBuilderGuiPresentationExtensions
{
    /// <summary>
    ///     Set the provided GuiPresentation instance on the definition owned by the builder.
    /// </summary>
    internal static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, GuiPresentation guiPresentation)
        where TBuilder : IDefinitionBuilder
    {
        builder.SetGuiPresentation(guiPresentation);
        return builder;
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided title, description and optional AssetReferenceSprite.
    /// </summary>
    internal static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string title, string description,
        AssetReferenceSprite sprite = null)
        where TBuilder : IDefinitionBuilder
    {
        return SetGuiPresentation(builder, GuiPresentationBuilder.Build(null, title, description, sprite));
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided title, description and optional AssetReferenceSprite.
    /// </summary>
    internal static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string title, string description,
        BaseDefinition spriteDefinition, bool hidden = false)
        where TBuilder : IDefinitionBuilder
    {
        AssetReferenceSprite sprite = null;

        if (spriteDefinition)
        {
            sprite = spriteDefinition.GuiPresentation.spriteReference;
        }

        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(null, title, description, sprite, hidden: hidden));
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided name, category and AssetReferenceSprite.<br />
    ///     The Title is generated as "{category}/&amp;{name}Title".<br />
    ///     The Description is generated as "{category}/&amp;{name}Description".<br />
    /// </summary>
    internal static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string name, Category category,
        AssetReferenceSprite sprite = null, int sortOrder = 0, bool? hidden = null)
        where TBuilder : IDefinitionBuilder
    {
        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(null, name, category, sprite, sortOrder, hidden));
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided name, category, description and AssetReferenceSprite.<br />
    ///     The Title is generated as "{category}/&amp;{name}Title".<br />
    ///     The Description is taken from parameters.<br />
    /// </summary>
    internal static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string name, Category category,
        string description, AssetReferenceSprite sprite = null, int sortOrder = 0, bool? hidden = null)
        where TBuilder : IDefinitionBuilder
    {
        var guiPresentation = GuiPresentationBuilder.Build(null, name, category, sprite, sortOrder, hidden);
        guiPresentation.Description = description;
        return SetGuiPresentation(builder, guiPresentation);
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided name, category and AssetReferenceSprite.<br />
    ///     The Title is generated as "{category}/&amp;{name}Title".<br />
    ///     The Description is generated as "{category}/&amp;{name}Description".<br />
    /// </summary>
    internal static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string name, Category category,
        BaseDefinition sprite, int sortOrder = 0, bool? hidden = null)
        where TBuilder : IDefinitionBuilder
    {
        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(null, name, category, sprite.GuiPresentation.spriteReference, sortOrder,
                hidden));
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided name, category and AssetReferenceSprite.<br />
    ///     The Title is generated as "{category}/&amp;{name}Title".<br />
    ///     The Description is taken from arguments.<br />
    /// </summary>
    internal static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, Category category,
        string description, AssetReferenceSprite sprite = null, int sortOrder = 0, bool hidden = false)
        where TBuilder : IDefinitionBuilder
    {
        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(null, GuiPresentationBuilder.CreateTitleKey(builder.Name, category),
                description, sprite, sortOrder, hidden));
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided name, category and AssetReferenceSprite.<br />
    ///     The Title is generated as "{category}/&amp;{name}Title".<br />
    ///     The Description is taken from arguments.<br />
    /// </summary>
    internal static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, Category category,
        string description, BaseDefinition spriteDefinition, int sortOrder = 0, bool hidden = false)
        where TBuilder : IDefinitionBuilder
    {
        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(null, GuiPresentationBuilder.CreateTitleKey(builder.Name, category),
                description, spriteDefinition.GuiPresentation.spriteReference, sortOrder, hidden));
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided builder, category and AssetReferenceSprite.<br />
    ///     The Title is generated as "{category}/&amp;{builder.Definition.Name}Title".<br />
    ///     The Description is generated as "{category}/&amp;{builder.Definition.Name}Description".<br />
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    internal static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, Category category,
        AssetReferenceSprite sprite, int sortOrder = 0, bool? hidden = null)
        where TBuilder : IDefinitionBuilder
    {
        var definitionName = builder.Name;

        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(null, definitionName, category, sprite, sortOrder, hidden));
    }

    internal static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, Category category,
        BaseDefinition definition = null, int sortOrder = 0, bool? hidden = null)
        where TBuilder : IDefinitionBuilder
    {
        var definitionName = builder.Name;
        var sprite = !definition ? null : definition.GuiPresentation.spriteReference;

        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(null, definitionName, category, sprite, sortOrder, hidden));
    }

    /// <summary>
    ///     Either update an existing GuiPresentation with the provided provided title, description and optional
    ///     AssetReferenceSprite, else create and set a new GuiPresentation.
    /// </summary>
    internal static TBuilder SetOrUpdateGuiPresentation<TBuilder>(this TBuilder builder, string title,
        string description, BaseDefinition definition = null)
        where TBuilder : IDefinitionBuilder
    {
        var gui = builder.GetGuiPresentation();
        var sprite = !definition ? null : definition.GuiPresentation.spriteReference;

        return SetGuiPresentation(builder, GuiPresentationBuilder.Build(gui, title, description, sprite));
    }

    /// <summary>
    ///     Either update an existing GuiPresentation or
    ///     create and set a new GuiPresentation using the provided name, category, optional AssetReferenceSprite and optional
    ///     sortOrder.<br />
    ///     The Title is generated as "{category}/&amp;{name}Title".<br />
    ///     The Description is generated as "{category}/&amp;{name}Description".<br />
    /// </summary>
    internal static TBuilder SetOrUpdateGuiPresentation<TBuilder>(this TBuilder builder, string name,
        Category category, BaseDefinition definition = null, int sortOrder = 0)
        where TBuilder : IDefinitionBuilder
    {
        var gui = builder.GetGuiPresentation();
        var sprite = !definition ? null : definition.GuiPresentation.spriteReference;

        return SetGuiPresentation(builder, GuiPresentationBuilder.Build(gui, name, category, sprite, sortOrder));
    }

    /// <summary>
    ///     Either update an existing GuiPresentation or
    ///     create and set a GuiPresentation from the provided builder, category, optional AssetReferenceSprite and optional
    ///     sortOrder.<br />
    ///     The Title is generated as "{category}/&amp;{builder.Definition.Name}Title".<br />
    ///     The Description is generated as "{category}/&amp;{builder.Definition.Name}Description".<br />
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    internal static TBuilder SetOrUpdateGuiPresentation<TBuilder>(this TBuilder builder, Category category,
        BaseDefinition definition = null, int sortOrder = 0)
        where TBuilder : IDefinitionBuilder
    {
        var gui = builder.GetGuiPresentation();
        var definitionName = builder.Name;
        var sprite = !definition ? null : definition.GuiPresentation.spriteReference;

        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(gui, definitionName, category, sprite, sortOrder));
    }

    /// <summary>
    ///     Create a GuiPresentation with Title=Feature/&amp;NoContentTitle, Description=Feature/&amp;NoContentDescription.
    /// </summary>
    internal static TBuilder SetGuiPresentationNoContent<TBuilder>(this TBuilder builder, bool hidden = false)
        where TBuilder : IDefinitionBuilder
    {
        builder.SetGuiPresentation(hidden
            ? GuiPresentationBuilder.NoContentHidden
            : GuiPresentationBuilder.NoContent);
        return builder;
    }
}
