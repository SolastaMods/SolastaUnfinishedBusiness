using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Diagnostics;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Builders
{
    public abstract class DefinitionBuilder
    {
        protected DefinitionBuilder()
        {
        }

        // NOTE: CreateGuid uses .ToString() which results in a guid of form b503ccb3-faac-4730-804c-d537bb61a582
        public static string CreateGuid(Guid guid, string name)
        {
            return GuidHelper.Create(guid, name).ToString();
        }

        [Conditional("DEBUG")]
        protected static void LogDefinition(string msg)
        {
            if (Main.Settings.DebugLogDefinitionCreation)
            {
                Main.Log(msg);
            }
        }

        protected static void VerifyDefinitionNameIsNotInUse(string definitionTypeName, string definitionName)
        {
            if (Main.Settings.DebugDisableVerifyDefinitionNameIsNotInUse)
            {
                return;
            }

            // Verify name has not been used previously
            // 1) get all names used in all TA databases (at this point) ignoring existing duplicates 
            // 2) check 'name' hasn't been used already, but ignore names we know already have duplicates

            if (Main.Settings.KnownDuplicateDefinitionNames.Contains(definitionName))
            {
                return;
            }

            if (DefinitionNames.TryGetValue(definitionName, out var item))
            {
                var msg = Environment.NewLine +
                    $"Adding definition of type '{definitionTypeName}' and name '{definitionName}'." +
                    Environment.NewLine +
                    $"A definition of type '{item.typeName} is already registered using the same name for a {(item.isCeDef ? "CE definition" : "Non-CE definition")}.";

#if DEBUG
                throw new SolastaModApiException(msg);
#else
                Main.Log(msg);
#endif
            }

            DefinitionNames.Add(definitionName, (definitionTypeName, true));
        }

        private static Dictionary<string, (string typeName, bool isCeDef)> DefinitionNames { get; } = GetAllDefinitionNames();

        private static Dictionary<string, (string typeName, bool isCeDef)> GetAllDefinitionNames()
        {
            var definitions = new Dictionary<string, (string typeName, bool isCeDef)>(StringComparer.OrdinalIgnoreCase);

            foreach (var db in (Dictionary<Type, object>)AccessTools.Field(typeof(DatabaseRepository), "databases").GetValue(null))
            {
                foreach (var bd in (IEnumerable)db.Value)
                {
                    // Ignore duplicates in other (TA, other mods loaded first) definition names
                    definitions.TryAdd(((BaseDefinition)bd).Name, (bd.GetType().Name, false));
                }
            }

            return definitions;
        }

        private protected static readonly MethodInfo GetDatabaseMethodInfo =
            typeof(DatabaseRepository).GetMethod("GetDatabase", BindingFlags.Public | BindingFlags.Static);

        protected const string CENamePrefix = "_CE_";
        protected static readonly Guid CENamespaceGuid = new("4ce4cabe-0d35-4419-86ef-13ce1bef32fd");
    }

    // Used to allow extension methods in other mods to set GuiPresentation 
    // Adding SetGuiPresentation as a public method causes name clash issues.
    // Ok, could have used a different name...
    internal interface IDefinitionBuilder
    {
        void SetGuiPresentation(GuiPresentation presentation);
        string Name { get; }
    }

    /// <summary>
    ///     Base class builder for all classes derived from BaseDefinition (for internal use only)
    /// </summary>
    /// <typeparam name="TDefinition"></typeparam>
    public abstract class DefinitionBuilder<TDefinition> : DefinitionBuilder, IDefinitionBuilder where TDefinition : BaseDefinition
    {
        #region Helpers

        // Explicit implementation not visible by default so doesn't clash with other extension methods
        void IDefinitionBuilder.SetGuiPresentation(GuiPresentation presentation)
        {
            Definition.GuiPresentation = presentation;
        }

        // NOTE: don't use Definition?. which bypasses Unity object lifetime check
        string IDefinitionBuilder.Name => Definition ? Definition.Name ?? string.Empty : string.Empty;

        private void InitializeCollectionFields()
        {
            Assert.IsNotNull(Definition);

            InitializeCollectionFields(Definition.GetType());

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            void InitializeCollectionFields(Type type)
            {
                if (type == null || type == typeof(object) || type == typeof(BaseDefinition) || type == typeof(UnityEngine.Object))
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
                        LogFieldInitialization($"Initializing field {field.Name} on Type={Definition.GetType().Name}, Name={Definition.Name}");

                        field.SetValue(Definition, Activator.CreateInstance(field.FieldType));
                    }
                    catch (Exception ex)
                    {
                        Main.Error(ex);
                    }
                }

                // So travel down the hierarchy
                InitializeCollectionFields(type.BaseType);

                [Conditional("DEBUG")]
                static void LogFieldInitialization(string message)
                {
                    if (Main.Settings.DebugLogFieldInitialization)
                    {
                        Main.Log(message);
                    }
                }
            }
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of TDefinition.  Automatically generate a guid from namespaceGuid plus name.
        /// Assign a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="name">The name assigned to the definition (mandatory)</param>
        /// <param name="namespaceGuid">The base or namespace guid from which to generate a guid for this definition, based on baseGuid+name (mandatory)</param>
        private protected DefinitionBuilder(string name, Guid namespaceGuid) :
            this(name, null, namespaceGuid, true)
        {
        }

        /// <summary>
        /// Create a new instance of TDefinition.  Assign the supplied guid as the definition guid.
        /// Assigns a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="name">The name assigned to the definition (mandatory)</param>
        /// <param name="definitionGuid">The guid for this definition (mandatory)</param>
        private protected DefinitionBuilder(string name, string definitionGuid) :
            this(name, definitionGuid, Guid.Empty, false)
        {
            Preconditions.IsNotNullOrWhiteSpace(definitionGuid, nameof(definitionGuid));
        }

        /// <summary>
        /// TODO: ... Create definition given 'name' only.
        /// Name = _CE_{name}
        /// Guid = CreateGuid(name, CENamespaceGuid)
        /// GuiPresentation = CommunityExpansion/&{name}Title, CommunityExpansion/&{name}Description, but can be overridden.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="createGuiPresentation"></param>
        private protected DefinitionBuilder(string name, bool createGuiPresentation = true)
            : this(CENamePrefix + name, null, CENamespaceGuid, true)
        {
            // If we know 'name' is unique across all dbs for definitions created by CE or mods using CE we can auto-generate
            // the guid and the GuiPresentation
            if (createGuiPresentation)
            {
                Definition.GuiPresentation = GuiPresentationBuilder.Build(name, Category.CommunityExpansion);
            }
        }

        /// <summary>
        /// TODO: ...
        /// </summary>
        /// <param name="original"></param>
        /// <param name="name"></param>
        /// <param name="createGuiPresentation"></param>
        private protected DefinitionBuilder(TDefinition original, string name, bool createGuiPresentation = true)
            : this(original, CENamePrefix + name, null, CENamespaceGuid, true)
        {
            // If we know 'name' is unique across all dbs for definitions created by CE or mods using CE we can auto-generate
            // the guid and the GuiPresentation
            if (createGuiPresentation)
            {
                Definition.GuiPresentation = GuiPresentationBuilder.Build(name, Category.CommunityExpansion);
            }
        }

        // TODO: two very similar ctors - worth combining?
        private DefinitionBuilder(string name, string definitionGuid, Guid namespaceGuid, bool useNamespaceGuid)
        {
            Preconditions.IsNotNullOrWhiteSpace(name, nameof(name));

            Definition = ScriptableObject.CreateInstance<TDefinition>();
            Definition.name = name;

            VerifyDefinitionNameIsNotInUse(Definition.GetType().Name, name);

            InitializeCollectionFields();

            if (useNamespaceGuid)
            {
                if (namespaceGuid == Guid.Empty)
                {
                    throw new SolastaModApiException("The namespace guid supplied is Guid.Empty.");
                }

                // create guid from namespace+name
                Definition.SetField("guid", CreateGuid(namespaceGuid, name));

                LogDefinition($"New-Creating definition: ({name}, namespace={namespaceGuid}, guid={Definition.GUID})");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(definitionGuid))
                {
                    throw new SolastaModApiException("The guid supplied is null or empty.");
                }

                // assign guid
                Definition.SetField("guid", definitionGuid);

                LogDefinition($"New-Creating definition: ({name}, guid={Definition.GUID})");
            }
        }

        /// <summary>
        /// Create clone and rename. Automatically generate a guid from baseGuid plus name.
        /// </summary>
        /// <param name="original">The definition being copied.</param>
        /// <param name="name">The name assigned to the definition (mandatory).</param>
        /// <param name="namespaceGuid">The base or namespace guid from which to generate a guid for this definition, based on baseGuid+name (mandatory).</param>
        private protected DefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) :
            this(original, name, null, namespaceGuid, true)
        {
        }

        /// <summary>
        /// Create clone and rename. Assign the supplied guid as the definition guid.
        /// Assigns a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="original">The definition being copied.</param>
        /// <param name="name">The name assigned to the definition (mandatory).</param>
        /// <param name="definitionGuid">The guid for this definition (mandatory).</param>
        private protected DefinitionBuilder(TDefinition original, string name, string definitionGuid) :
            this(original, name, definitionGuid, Guid.Empty, false)
        {
        }

        // TODO: two very similar ctors - worth combining?
        private DefinitionBuilder(TDefinition original, string name, string definitionGuid, Guid namespaceGuid, bool useNamespaceGuid)
        {
            Preconditions.IsNotNull(original, nameof(original));
            Preconditions.IsNotNullOrWhiteSpace(name, nameof(name));

            var originalName = original.name;
            var originalGuid = original.GUID;

            Definition = UnityEngine.Object.Instantiate(original);
            Definition.name = name;

            VerifyDefinitionNameIsNotInUse(Definition.GetType().Name, name);

            InitializeCollectionFields();

            if (useNamespaceGuid)
            {
                if (namespaceGuid == Guid.Empty)
                {
                    throw new ArgumentException("Please supply a non-empty Guid", nameof(namespaceGuid));
                }

                // create guid from namespace+name
                Definition.SetField("guid", CreateGuid(namespaceGuid, name));

                LogDefinition($"New-Cloning definition: original({originalName}, {originalGuid}) => ({name}, namespace={namespaceGuid}, {Definition.GUID})");
            }
            else
            {
                // directly assign guid
                Definition.SetField("guid", definitionGuid);

                LogDefinition($"New-Cloning definition: original({originalName}, {originalGuid}) => ({name}, {Definition.GUID})");
            }
        }

        /// <summary>
        /// Take ownership of a definition without changing the name or guid.
        /// </summary>
        /// <param name="original">The definition</param>
        private protected DefinitionBuilder(TDefinition original)
        {
            Preconditions.IsNotNull(original, nameof(original));
            Definition = original;
        }

        #endregion

        #region Add to dbs
        /// <summary>
        /// Add the TDefinition to every compatible database
        /// </summary>
        /// <param name="assertIfDuplicate"></param>
        /// <returns></returns>
        /// <exception cref="SolastaModApiException"></exception>
        public TDefinition AddToDB(bool assertIfDuplicate = true)
        {
            Preconditions.IsNotNull(Definition, nameof(Definition));
            Preconditions.IsNotNullOrWhiteSpace(Definition.Name, nameof(Definition.Name));
            Preconditions.IsNotNullOrWhiteSpace(Definition.GUID, nameof(Definition.GUID));

            if (!Guid.TryParse(Definition.GUID, out var guid))
            {
                throw new SolastaModApiException($"The string in Definition.GUID '{Definition.GUID}' is not a GUID.");
            }

            VerifyGuiPresentation();

            // Get all base types for the target definition.  The definition needs to be added to all matching databases.
            // e.g. ConditionAffinityBlindnessImmunity is added to dbs: FeatureDefinitionConditionAffinity, FeatureDefinitionAffinity, FeatureDefinition
            var types = GetBaseTypes(Definition.GetType());

            var addedToAnyDB = false;

            foreach (var type in types)
            {
                if (AddToDB(type))
                {
                    addedToAnyDB = true;
                }
            }

            if (!addedToAnyDB)
            {
                throw new SolastaModApiException(
                    $"Unable to locate any database(s) matching definition type='{Definition.GetType().FullName}', name='{Definition.name}', guid='{Definition.GUID}'.");
            }

            LogDefinition($"Added to db: name={Definition.Name}, guid={Definition.GUID}");

            return Definition;

            bool AddToDB(Type type)
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
                    if (dbHasElement(Definition.name))
                    {
                        throw new SolastaModApiException(
                            $"The definition with name '{Definition.name}' already exists in database '{type.Name}' by name.");
                    }

                    if (dbHasElementByGuid(Definition.GUID))
                    {
                        throw new SolastaModApiException(
                            $"The definition with name '{Definition.name}' and guid '{Definition.GUID}' already exists in database '{type.Name}' by GUID.");
                    }
                }

                addToDB();

                return true;

                void addToDB()
                {
                    var methodInfo = dbType.GetMethod("Add");

                    if (methodInfo == null)
                    {
                        throw new SolastaModApiException($"Could not locate the 'Add' method for {dbType.FullName}.");
                    }

                    methodInfo.Invoke(db, new object[] { Definition });
                }

                bool dbHasElement(string name)
                {
                    var methodInfo = dbType.GetMethod("HasElement");

                    if (methodInfo == null)
                    {
                        throw new SolastaModApiException(
                            $"Could not locate the 'HasElement' method for {dbType.FullName}.");
                    }

                    return (bool)methodInfo.Invoke(db, new object[] { name });
                }

                bool dbHasElementByGuid(string guid)
                {
                    var methodInfo = dbType.GetMethod("HasElementByGuid");

                    if (methodInfo == null)
                    {
                        throw new SolastaModApiException(
                            $"Could not locate the 'HasElementByGuid' method for {dbType.FullName}.");
                    }

                    return (bool)methodInfo.Invoke(db, new object[] { guid });
                }
            }

            // Get list of base types down to but not including BaseDefinition.  
            IEnumerable<Type> GetBaseTypes(Type t)
            {
                if (t.BaseType != typeof(object) && t != typeof(BaseDefinition))
                {
                    return Enumerable.Repeat(t, 1).Concat(GetBaseTypes(t.BaseType));
                }
                else
                {
                    return Enumerable.Empty<Type>();
                }
            }

            void VerifyGuiPresentation()
            {
                if (Definition.GuiPresentation == null)
                {
                    Main.Log($"Verify GuiPresentation: {Definition.GetType().Name}({Definition.Name}) has no GuiPresentation, setting to NoContent.");

                    Definition.GuiPresentation = GuiPresentationBuilder.NoContent;
                }
                else
                {
                    if (string.IsNullOrEmpty(Definition.GuiPresentation.Title))
                    {
                        Main.Log($"Verify GuiPresentation: {Definition.GetType().Name}({Definition.Name}) has no GuiPresentation.Title, setting to NoContent.");

                        Definition.GuiPresentation.Title = GuiPresentationBuilder.NoContentTitle;
                    }

                    if (string.IsNullOrEmpty(Definition.GuiPresentation.Description))
                    {
                        Main.Log($"Verify GuiPresentation: {Definition.GetType().Name}({Definition.Name}) has no GuiPresentation.Description, setting to NoContent.");

                        Definition.GuiPresentation.Description = GuiPresentationBuilder.NoContentDescription;
                    }
                }
            }
        }

        #endregion

        protected TDefinition Definition { get; }

        internal TBuilder Configure<TBuilder>(Action<TDefinition> configureDefinition)
            where TBuilder : DefinitionBuilder<TDefinition>
        {
            Assert.IsNotNull(configureDefinition);
            configureDefinition.Invoke(Definition);

#if DEBUG
            if (this is not TBuilder)
            {
                throw new SolastaModApiException($"Error in Configure. TBuilder={typeof(TBuilder).Name}, this={GetType().Name}");
            }
#endif

            return (TBuilder)this;
        }
    }

    /// <summary>
    ///     <para>Base class builder for all classes derived from BaseDefinition (for internal use only).</para>
    ///     <para>
    ///     This version of DefinitionBuilder allows passing the builder type as <typeparamref name="TBuilder"/>.  This
    ///     allows <seealso cref="Configure(Action{TDefinition})">Configure</seealso> to be called without type parameters, and enables adding helper Set{PropertyName} methods to intermediate builders
    ///     that return the correct TBuilder.
    ///     </para>
    /// </summary>
    /// <typeparam name="TDefinition"></typeparam>
    /// <typeparam name="TBuilder"></typeparam>
    public abstract class DefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition>
        where TDefinition : BaseDefinition
        where TBuilder : DefinitionBuilder<TDefinition, TBuilder>
    {
        private protected DefinitionBuilder(TDefinition original) : base(original) { }
        private protected DefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid) { }
        private protected DefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid) { }
        private protected DefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation) { }
        private protected DefinitionBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation) { }
        private protected DefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid) { }
        private protected DefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid) { }

        internal TBuilder Configure(Action<TDefinition> configureDefinition)
        {
            return Configure<TBuilder>(configureDefinition);
        }

        internal TBuilder This()
        {
#if DEBUG
            if (this is not TBuilder)
            {
                throw new SolastaModApiException($"Error in Configure. TBuilder={typeof(TBuilder).Name}, this={GetType().Name}");
            }
#endif

            return (TBuilder)this;
        }
    }
}
