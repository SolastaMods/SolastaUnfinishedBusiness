using System;
using SolastaCommunityExpansion.Api.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders;

public class GuiPresentationBuilder
{
    public static readonly string NoContentTitle = "Feature/&NoContentTitle";
    public static readonly string EmptyString = "Feature/&Emptystring";
    public static readonly AssetReferenceSprite EmptySprite = new(string.Empty);
    private readonly GuiPresentation guiPresentation;

    public GuiPresentationBuilder(string title = null, string description = null,
        AssetReferenceSprite sprite = null)
    {
        guiPresentation = new GuiPresentation
        {
            Description = description ?? string.Empty, Title = title ?? string.Empty
        };

        SetSpriteReference(sprite ?? EmptySprite);
    }

    public GuiPresentationBuilder(GuiPresentation reference)
        : this(reference.Title, reference.Description, reference.SpriteReference)
    {
    }

    /// <summary>
    ///     GuiPresentation representing 'No content title and description'
    /// </summary>
    public static GuiPresentation NoContent { get; } = Build(NoContentTitle, NoContentTitle, EmptySprite);

    public static GuiPresentation NoContentHidden { get; } =
        Build(NoContentTitle, NoContentTitle, EmptySprite, 0, true);

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

    public GuiPresentationBuilder SetTitle(string title)
    {
        guiPresentation.title = title;
        return this;
    }

#if false
    public GuiPresentationBuilder SetColor(Color color)
    {
        guiPresentation.color = color;
        return this;
    }

    public GuiPresentationBuilder SetHidden(bool hidden)
    {
        guiPresentation.hidden = hidden;
        return this;
    }

    public GuiPresentationBuilder SetSortOrder(int sortOrder)
    {
        guiPresentation.sortOrder = sortOrder;
        return this;
    }
#endif

    public GuiPresentationBuilder SetSpriteReference(AssetReferenceSprite sprite)
    {
        guiPresentation.spriteReference = sprite;
        return this;
    }

    public GuiPresentation Build()
    {
        return guiPresentation;
    }

    public static GuiPresentation Build(string title, string description, AssetReferenceSprite sprite = null,
        int? sortOrder = null, bool? hidden = null)
    {
        return Build(null, title, description, sprite, sortOrder, hidden);
    }

    public static GuiPresentation Build(GuiPresentation reference, string title, string description,
        AssetReferenceSprite sprite = null, int? sortOrder = null, bool? hidden = null)
    {
        var guip = reference == null ? new GuiPresentation() : new GuiPresentation(reference);

        guip.Title = title;
        guip.Description = description;
        guip.spriteReference = sprite ?? reference?.SpriteReference ?? EmptySprite;
        guip.sortOrder = sortOrder ?? reference?.SortOrder ?? 0;
        guip.hidden = hidden ?? reference?.Hidden ?? false;

        return guip;
    }

    public static GuiPresentation Build(string name, Category category, AssetReferenceSprite sprite = null,
        int? sortOrder = null, bool? hidden = null)
    {
        return Build(null, name, category, sprite, sortOrder, hidden);
    }

    public static GuiPresentation Build(GuiPresentation reference, string name, Category category,
        AssetReferenceSprite sprite = null, int? sortOrder = null, bool? hidden = null)
    {
        return Build(reference, CreateTitleKey(name, category), CreateDescriptionKey(name, category), sprite,
            sortOrder, hidden);
    }
}

public static class BaseDefinitionBuilderGuiPresentationExtensions
{
    /// <summary>
    ///     Set the provided GuiPresentation instance on the definition owned by the builder.
    /// </summary>
    public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, GuiPresentation guiPresentation)
        where TBuilder : IDefinitionBuilder
    {
        builder.SetGuiPresentation(guiPresentation);
        return builder;
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided title, description and optional AssetReferenceSprite.
    /// </summary>
    public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string title, string description,
        AssetReferenceSprite sprite = null)
        where TBuilder : IDefinitionBuilder
    {
        return SetGuiPresentation(builder, GuiPresentationBuilder.Build(null, title, description, sprite));
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided name, category and AssetReferenceSprite.<br />
    ///     The Title is generated as "{category}/&amp;{name}Title".<br />
    ///     The Description is generated as "{category}/&amp;{name}Description".<br />
    /// </summary>
    public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, string name, Category category,
        AssetReferenceSprite sprite = null, int sortOrder = 0, bool? hidden = null)
        where TBuilder : IDefinitionBuilder
    {
        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(null, name, category, sprite, sortOrder, hidden));
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided name, category and AssetReferenceSprite.<br />
    ///     The Title is generated as "{category}/&amp;{name}Title".<br />
    ///     The Description is taken from argumants.<br />
    /// </summary>
    public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, Category category,
        string description, AssetReferenceSprite sprite = null, int sortOrder = 0, bool hidden = false)
        where TBuilder : IDefinitionBuilder
    {
        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(null, GuiPresentationBuilder.CreateTitleKey(builder.Name, category),
                description, sprite, sortOrder, hidden));
    }

    /// <summary>
    ///     Create and set a GuiPresentation from the provided builder, category and AssetReferenceSprite.<br />
    ///     The Title is generated as "{category}/&amp;{builder.Definition.Name}Title".<br />
    ///     The Description is generated as "{category}/&amp;{builder.Definition.Name}Description".<br />
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    public static TBuilder SetGuiPresentation<TBuilder>(this TBuilder builder, Category category,
        AssetReferenceSprite sprite = null, int sortOrder = 0, bool? hidden = null)
        where TBuilder : IDefinitionBuilder
    {
        var definitionName = builder.Name;

        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(null, definitionName, category, sprite, sortOrder, hidden));
    }

    /// <summary>
    ///     Either update an existing GuiPresentation with the provided provided title, description and optional
    ///     AssetReferenceSprite, else create and set a new GuiPresentation.
    /// </summary>
    public static TBuilder SetOrUpdateGuiPresentation<TBuilder>(this TBuilder builder, string title,
        string description, AssetReferenceSprite sprite = null)
        where TBuilder : IDefinitionBuilder
    {
        var guip = builder.GetGuiPresentation();

        return SetGuiPresentation(builder, GuiPresentationBuilder.Build(guip, title, description, sprite));
    }

    /// <summary>
    ///     Either update an existing GuiPresentation or
    ///     create and set a new GuiPresentation using the provided name, category, optional AssetReferenceSprite and optional
    ///     sortOrder.<br />
    ///     The Title is generated as "{category}/&amp;{name}Title".<br />
    ///     The Description is generated as "{category}/&amp;{name}Description".<br />
    /// </summary>
    public static TBuilder SetOrUpdateGuiPresentation<TBuilder>(this TBuilder builder, string name,
        Category category, AssetReferenceSprite sprite = null, int sortOrder = 0)
        where TBuilder : IDefinitionBuilder
    {
        var guip = builder.GetGuiPresentation();

        return SetGuiPresentation(builder, GuiPresentationBuilder.Build(guip, name, category, sprite, sortOrder));
    }

    /// <summary>
    ///     Either update an existing GuiPresentation or
    ///     create and set a GuiPresentation from the provided builder, category, optional AssetReferenceSprite and optional
    ///     sortOrder.<br />
    ///     The Title is generated as "{category}/&amp;{builder.Definition.Name}Title".<br />
    ///     The Description is generated as "{category}/&amp;{builder.Definition.Name}Description".<br />
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    public static TBuilder SetOrUpdateGuiPresentation<TBuilder>(this TBuilder builder, Category category,
        AssetReferenceSprite sprite = null, int sortOrder = 0)
        where TBuilder : IDefinitionBuilder
    {
        var guip = builder.GetGuiPresentation();
        var definitionName = builder.Name;

        return SetGuiPresentation(builder,
            GuiPresentationBuilder.Build(guip, definitionName, category, sprite, sortOrder));
    }

    /// <summary>
    ///     Create a GuiPresentation with Title=Feature/&amp;NoContentTitle, Description=Feature/&amp;NoContentDescription.
    /// </summary>
    public static TBuilder SetGuiPresentationNoContent<TBuilder>(this TBuilder builder, bool hidden = false)
        where TBuilder : IDefinitionBuilder
    {
        builder.SetGuiPresentation(hidden
            ? GuiPresentationBuilder.NoContentHidden
            : GuiPresentationBuilder.NoContent);
        return builder;
    }

    /// <summary>
    ///     Create a GuiPresentation with Title=Feature/&amp;NoContentTitle, Description=Feature/&amp;NoContentDescription and
    ///     sprite.
    /// </summary>
    public static TBuilder SetGuiPresentationNoContent<TBuilder>(this TBuilder builder, AssetReferenceSprite sprite)
        where TBuilder : IDefinitionBuilder
    {
        builder.SetGuiPresentation(
            GuiPresentationBuilder.Build(GuiPresentationBuilder.NoContentTitle,
                GuiPresentationBuilder.NoContentTitle, sprite));
        return builder;
    }
}
