using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Diagnostics;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using static SolastaUnfinishedBusiness.Main;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Builders;

internal abstract class DefinitionBuilder
{
    private protected static readonly MethodInfo GetDatabaseMethodInfo =
        typeof(DatabaseRepository).GetMethod("GetDatabase", BindingFlags.Public | BindingFlags.Static);

    internal static readonly Guid CeNamespaceGuid = new("b1ffaca74824486ea74a68d45e6b1925");

    private static Dictionary<string, (string typeName, bool isCeDef)> DefinitionNames { get; } =
        GetAllDefinitionNames();

    protected static string CreateGuid(Guid guid, string name)
    {
        return GuidHelper.Create(guid, name).ToString();
    }

#if DEBUG
    protected static void LogDefinition(string message)
    {
        if (Main.Settings.DebugLogDefinitionCreation)
        {
            Log(message);
        }
    }
#endif

    protected static void VerifyDefinitionNameIsNotInUse(string definitionTypeName, string definitionName)
    {
        if (Main.Settings.DebugDisableVerifyDefinitionNameIsNotInUse)
        {
            return;
        }

        // Verify name has not been used previously
        // 1) get all names used in all TA databases (at this point) ignoring existing duplicates 
        // 2) check 'name' hasn't been used already, but ignore names we know already have duplicates

        if (DiagnosticsContext.KnownDuplicateDefinitionNames.Contains(definitionName))
        {
            return;
        }

        if (DefinitionNames.TryGetValue(definitionName, out var item))
        {
            var msg = Environment.NewLine +
                      $"Adding definition of type '{definitionTypeName}' and name '{definitionName}'." +
                      Environment.NewLine +
                      $"A definition of type '{item.typeName} is already registered using the same name for a {(item.isCeDef ? "CE definition" : "Non-CE definition")}.";

            throw new SolastaUnfinishedBusinessException(msg);
        }

        DefinitionNames.Add(definitionName, (definitionTypeName, true));
    }

    private static Dictionary<string, (string typeName, bool isCeDef)> GetAllDefinitionNames()
    {
        var definitions = new Dictionary<string, (string typeName, bool isCeDef)>(StringComparer.OrdinalIgnoreCase);

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var db in DatabaseRepository.databases)
        {
            foreach (var bd in (IEnumerable)db.Value)
            {
                // Ignore duplicates in other (TA, other mods loaded first) definition names
                definitions.TryAdd(((BaseDefinition)bd).Name, (bd.GetType().Name, false));
            }
        }

        return definitions;
    }
}

// Used to allow extension methods in other mods to set GuiPresentation 
// Adding SetGuiPresentation as an internal method causes name clash issues.
// Ok, could have used a different name...
internal interface IDefinitionBuilder
{
    string Name { get; }
    void SetGuiPresentation(GuiPresentation presentation);
    GuiPresentation GetGuiPresentation();
}

/// <summary>
///     Base class builder for all classes derived from BaseDefinition (for internal use only)
/// </summary>
/// <typeparam name="TDefinition"></typeparam>
internal abstract class DefinitionBuilder<TDefinition> : DefinitionBuilder, IDefinitionBuilder
    where TDefinition : BaseDefinition
{
    protected TDefinition Definition { get; }

    /// <summary>
    ///     Implement in derived builders to enforce any require preconditions, values etc, e.g.
    ///     <code>Definition.EffectDescription = new ();</code>
    /// </summary>
    protected virtual void Initialise()
    {
        var fieldInDungeonEditor = AccessTools.Field(typeof(TDefinition), "inDungeonEditor");

        fieldInDungeonEditor?.SetValue(Definition, false);
    }

    /// <summary>
    ///     Called before the definition is added to the databases.
    ///     Verify post-condition checks here.
    /// </summary>
    internal virtual void Validate() { }

    #region AddToDB

    internal TDefinition AddToDB()
    {
#if DEBUG
        return AddToDB(true);
#else
        return AddToDB(false);
#endif
    }

    private TDefinition AddToDB(
        bool assertIfDuplicate,
        BaseDefinition.Copyright copyright = BaseDefinition.Copyright.UserContent,
        GamingPlatformDefinitions.ContentPack contentPack = CeContentPackContext.CeContentPack)
    {
        // ReSharper disable once InvocationIsSkipped
        PreConditions.ArgumentIsNotNull(Definition, nameof(Definition));
        // ReSharper disable once InvocationIsSkipped
        PreConditions.IsNotNullOrWhiteSpace(Definition.Name, nameof(Definition.Name));
        // ReSharper disable once InvocationIsSkipped
        PreConditions.IsNotNullOrWhiteSpace(Definition.GUID, nameof(Definition.GUID));

        if (!Guid.TryParse(Definition.GUID, out _))
        {
            throw new SolastaUnfinishedBusinessException(
                $"The string in Definition.GUID '{Definition.GUID}' is not a GUID.");
        }

        VerifyGuiPresentation();

        Definition.contentCopyright = copyright;
        Definition.contentPack = contentPack;

        Validate();

        // Get all base types for the target definition.  The definition needs to be added to all matching databases.
        // e.g. ConditionAffinityBlindnessImmunity is added to dbs: FeatureDefinitionConditionAffinity, FeatureDefinitionAffinity, FeatureDefinition
        var types = GetBaseTypes(Definition.GetType());
        var addedToAnyDB = false;

        foreach (var type in types)
        {
            if (LocalAddToDB(type))
            {
                addedToAnyDB = true;
            }
        }

        if (!addedToAnyDB)
        {
            throw new SolastaUnfinishedBusinessException(
                $"Unable to locate any database(s) matching definition type='{Definition.GetType().FullName}', name='{Definition.name}', guid='{Definition.GUID}'.");
        }

#if DEBUG
        LogDefinition($"Added to db: name={Definition.Name}, guid={Definition.GUID}");
#endif

        return Definition;

        bool LocalAddToDB(Type type)
        {
            // attempt to get database matching the target type
            var getDatabaseMethodInfoGeneric = GetDatabaseMethodInfo.MakeGenericMethod(type);
            var db = getDatabaseMethodInfoGeneric.Invoke(null, null);

            if (db == null)
            {
                return false;
            }

            var dbType = db.GetType();

            if (assertIfDuplicate)
            {
                if (DBHasElement(Definition.name))
                {
                    throw new SolastaUnfinishedBusinessException(
                        $"The definition with name '{Definition.name}' already exists in database '{type.Name}' by name.");
                }

                if (DBHasElementByGuid(Definition.GUID))
                {
                    throw new SolastaUnfinishedBusinessException(
                        $"The definition with name '{Definition.name}' and guid '{Definition.GUID}' already exists in database '{type.Name}' by GUID.");
                }
            }

            var methodInfo = dbType.GetMethod("Add");

            if (methodInfo == null)
            {
                throw new SolastaUnfinishedBusinessException(
                    $"Could not locate the 'Add' method for {dbType.FullName}.");
            }

            methodInfo.Invoke(db, [Definition]);

            return true;

            bool DBHasElement(string name)
            {
                var hasElementMethodInfo = dbType.GetMethod("HasElement");

                if (hasElementMethodInfo == null)
                {
                    throw new SolastaUnfinishedBusinessException(
                        $"Could not locate the 'HasElement' method for {dbType.FullName}.");
                }

                return (bool)hasElementMethodInfo.Invoke(db, [name, true]);
            }

            bool DBHasElementByGuid(string guid)
            {
                var hasElementByGuidMethodInfo = dbType.GetMethod("HasElementByGuid") ??
                                                 throw new SolastaUnfinishedBusinessException(
                                                     $"Could not locate the 'HasElementByGuid' method for {dbType.FullName}.");

                return (bool)hasElementByGuidMethodInfo.Invoke(db, [guid]);
            }
        }

        // Get list of base types down to but not including BaseDefinition.  
        IEnumerable<Type> GetBaseTypes(Type t)
        {
            if (t.BaseType != typeof(object) && t != typeof(BaseDefinition))
            {
                return Enumerable.Repeat(t, 1).Concat(GetBaseTypes(t.BaseType));
            }

            return [];
        }

        void VerifyGuiPresentation()
        {
            if (Definition.GuiPresentation == null)
            {
                // ReSharper disable once InvocationIsSkipped
                Log(
                    $"Verify GuiPresentation: {Definition.GetType().Name}({Definition.Name}) has no GuiPresentation, setting to NoContent.");

                Definition.GuiPresentation = GuiPresentationBuilder.NoContent;
            }
            else
            {
                if (string.IsNullOrEmpty(Definition.GuiPresentation.Title))
                {
                    // ReSharper disable once InvocationIsSkipped
                    Log(
                        $"Verify GuiPresentation: {Definition.GetType().Name}({Definition.Name}) has no GuiPresentation.Title, setting to NoContent.");

                    Definition.GuiPresentation.Title = GuiPresentationBuilder.EmptyString;
                }

                if (!string.IsNullOrEmpty(Definition.GuiPresentation.Description))
                {
                    return;
                }

                // ReSharper disable once InvocationIsSkipped
                Log(
                    $"Verify GuiPresentation: {Definition.GetType().Name}({Definition.Name}) has no GuiPresentation.Description, setting to NoContent.");

                Definition.GuiPresentation.Description = GuiPresentationBuilder.EmptyString;
            }
        }
    }

    #endregion

    #region Helpers

    // Explicit implementation not visible by default so doesn't clash with other extension methods
    void IDefinitionBuilder.SetGuiPresentation(GuiPresentation presentation)
    {
        Definition.GuiPresentation = presentation;
    }

    GuiPresentation IDefinitionBuilder.GetGuiPresentation()
    {
        return Definition.GuiPresentation;
    }

    // NOTE: don't use Definition?. which bypasses Unity object lifetime check
    string IDefinitionBuilder.Name => Definition ? Definition.Name ?? string.Empty : string.Empty;

    private void InitializeCollectionFields()
    {
#if DEBUG
        Assert.IsNotNull(Definition);
#endif
        LocalInitializeCollectionFields(Definition.GetType());

        return;

        void LocalInitializeCollectionFields(Type type)
        {
            while (true)
            {
                if (type == null || type == typeof(object) || type == typeof(BaseDefinition) || type == typeof(Object))
                {
                    return;
                }

                // Reflection will only return private fields declared on this type, not base classes
                foreach (var field in type
                             .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                             .Where(f => f.FieldType.IsGenericType)
                             .Where(f => f.GetValue(Definition) == null))
                {
                    try
                    {
#if DEBUG
                        LogFieldInitialization(
                            $"Initializing field {field.Name} on Type={Definition.GetType().Name}, Name={Definition.Name}");
#endif

                        field.SetValue(Definition, Activator.CreateInstance(field.FieldType));
                    }
                    catch (Exception ex)
                    {
                        Error(ex);
                    }
                }

                // So travel down the hierarchy
                type = type.BaseType;

#if DEBUG
                continue;

                static void LogFieldInitialization(string message)
                {
                    if (Main.Settings.DebugLogFieldInitialization)
                    {
                        Log(message);
                    }
                }
#endif
            }
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    ///     Create a new instance of TDefinition. Generate Definition.Guid from <paramref name="namespaceGuid" /> plus
    ///     <paramref name="name" />.
    /// </summary>
    /// <param name="name">The name assigned to the definition (mandatory)</param>
    /// <param name="namespaceGuid">The base or namespace guid from which to generate a guid for this definition.</param>
    private protected DefinitionBuilder(string name, Guid namespaceGuid)
    {
        // ReSharper disable once InvocationIsSkipped
        PreConditions.IsNotNullOrWhiteSpace(name, nameof(name));

        Definition = ScriptableObject.CreateInstance<TDefinition>();
        Definition.name = name;

        VerifyDefinitionNameIsNotInUse(Definition.GetType().Name, name);

        InitializeCollectionFields();

        if (namespaceGuid == Guid.Empty)
        {
            throw new SolastaUnfinishedBusinessException("The namespace guid supplied is Guid.Empty.");
        }

        // create guid from namespace+name
        Definition.SetUserContentGUID(CreateGuid(namespaceGuid, name));

#if DEBUG
        LogDefinition($"New-Creating definition: ({name}, namespace={namespaceGuid}, guid={Definition.GUID})");
#endif
    }

    /// <summary>
    ///     Create a clone of <paramref name="original" /> and rename as <paramref name="name" />. Automatically generate a
    ///     guid from <paramref name="namespaceGuid" /> plus <paramref name="name" />.
    /// </summary>
    /// <param name="original">The definition being copied.</param>
    /// <param name="name">The name assigned to the definition (mandatory).</param>
    /// <param name="namespaceGuid">The base or namespace guid from which to generate a guid for this definition.</param>
    private protected DefinitionBuilder(TDefinition original, string name, Guid namespaceGuid)
    {
        // ReSharper disable once InvocationIsSkipped
        PreConditions.ArgumentIsNotNull(original, nameof(original));
        // ReSharper disable once InvocationIsSkipped
        PreConditions.IsNotNullOrWhiteSpace(name, nameof(name));

        Definition = Object.Instantiate(original);
        Definition.name = name;

#if DEBUG
        VerifyDefinitionNameIsNotInUse(Definition.GetType().Name, name);
#endif

        InitializeCollectionFields();

        if (namespaceGuid == Guid.Empty)
        {
            throw new ArgumentException(@"Please supply a non-empty Guid", nameof(namespaceGuid));
        }

        // create guid from namespace+name
        Definition.SetUserContentGUID(CreateGuid(namespaceGuid, name));
    }

    #endregion
}

/// <summary>
///     <para>Base class builder for all classes derived from BaseDefinition (for internal use only).</para>
///     <para>
///         This version of DefinitionBuilder allows passing the builder type as <typeparamref name="TBuilder" />.
///     </para>
/// </summary>
/// <typeparam name="TDefinition"></typeparam>
/// <typeparam name="TBuilder"></typeparam>
internal abstract class DefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition>
    where TDefinition : BaseDefinition
    where TBuilder : DefinitionBuilder<TDefinition, TBuilder>
{
    private protected DefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    private protected DefinitionBuilder(TDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    /// <summary>
    ///     Override this in a derived builder (and set to true) to disable the standard set of Create methods.
    ///     You must then provide your own specialized constructor and/or Create method.
    /// </summary>
    private static TBuilder CreateImpl(params object[] parameters)
    {
        var parameterTypes = parameters.Select(p => p.GetType()).ToArray();

        var ctor = typeof(TBuilder).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, parameterTypes, null);

        if (ctor == null)
        {
            throw new SolastaUnfinishedBusinessException(
                $"No constructor found on {typeof(TBuilder).Name} with argument types {string.Join(",", parameterTypes.Select(t => t.Name))}");
        }

        var builder = (TBuilder)ctor.Invoke(parameters);

        builder.Initialise();

        return builder;
    }

    internal static TBuilder Create(string name)
    {
        return CreateImpl(name, CeNamespaceGuid);
    }

    internal static TBuilder Create(TDefinition original, string name)
    {
        return CreateImpl(original, name, CeNamespaceGuid);
    }

    internal TBuilder AddCustomSubFeatures(params object[] features)
    {
        Definition.AddCustomSubFeatures(features);
        return (TBuilder)this;
    }
}
