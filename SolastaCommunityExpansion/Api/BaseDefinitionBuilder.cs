using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Diagnostics;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaModApi
{
    public abstract class BaseDefinitionBuilder
    {
        protected BaseDefinitionBuilder()
        {
        }

        // NOTE: CreateGuid uses .ToString() which results in a guid of form b503ccb3-faac-4730-804c-d537bb61a582
        public static string CreateGuid(Guid guid, string name)
        {
            return GuidHelper.Create(guid, name).ToString();
        }

        public static string CreateGuid(string guid, string name)
        {
            Preconditions.IsNotNullOrWhiteSpace(guid, nameof(guid));
            Preconditions.IsNotNullOrWhiteSpace(name, nameof(name));

            return GuidHelper.Create(new Guid(guid), name).ToString();
        }

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

        public static bool LogDefinitionCreation { get; set; }

        protected static void LogDefinition(string msg)
        {
            if (LogDefinitionCreation)
            {
                SolastaCommunityExpansion.Main.Log(msg);
            }
        }
    }

    // Used to allow extension methods in other mods to set GuiPresentation 
    // Adding SetGuiPresentation as a public method causes name clash issues.
    // Ok, could have used a different name...
    public interface IBaseDefinitionBuilder
    {
        void SetGuiPresentation(GuiPresentation presentation);
        string Name { get; }
    }

    /// <summary>
    ///     Base class builder for all classes derived from BaseDefinition
    /// </summary>
    /// <typeparam name="TDefinition"></typeparam>
    public abstract class BaseDefinitionBuilder<TDefinition> : BaseDefinitionBuilder, IBaseDefinitionBuilder where TDefinition : BaseDefinition
    {
        #region Helpers

        private static GuiPresentation BuildGuiPresentation(string title, string description)
        {
            return new GuiPresentation
            {
                Description = description ?? string.Empty,
                Title = title ?? string.Empty
            };
        }

        // Explicit implementation not visible by default so doesn't clash with other extension methods
        void IBaseDefinitionBuilder.SetGuiPresentation(GuiPresentation presentation)
        {
            Definition.GuiPresentation = presentation;
        }

        string IBaseDefinitionBuilder.Name => Definition?.Name ?? string.Empty;

        #endregion

        #region Preferred constructors (for future development)

        /// <summary>
        /// Create a new instance of TDefinition.  Automatically generate a guid from namespaceGuid plus name.
        /// Assign a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="name">The name assigned to the definition (mandatory)</param>
        /// <param name="namespaceGuid">The base or namespace guid from which to generate a guid for this definition, based on baseGuid+name (mandatory)</param>
        /// <param name="category">Used to generate title and description on the GuiPresentation.  The generated fields if
        /// name="MyDefinition" and category="MyCategory" are: MyCategory/&amp;MyDefinitionTitle and MyCategory/&amp;MyDefinitionDescription.
        /// If category=null then no GuiPresentation is created.
        /// </param>
        protected BaseDefinitionBuilder(string name, Guid namespaceGuid, Category category) :
            this(name, null, namespaceGuid, true, category)
        {
        }

        /// <summary>
        /// Create a new instance of TDefinition.  Assign the supplied guid as the definition guid.
        /// Assigns a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="name">The name assigned to the definition (mandatory)</param>
        /// <param name="definitionGuid">The guid for this definition (mandatory)</param>
        /// <param name="category">Used to generate title and description on the GuiPresentation.  The generated fields if
        /// name="MyDefinition" and category="MyCategory" are: MyCategory/&amp;MyDefinitionTitle and MyCategory/&amp;MyDefinitionDescription.
        /// If category=null then no GuiPresentation is created.
        /// </param>
        protected BaseDefinitionBuilder(string name, string definitionGuid, Category category) :
            this(name, definitionGuid, Guid.Empty, false, category)
        {
            Preconditions.IsNotNullOrWhiteSpace(definitionGuid, nameof(definitionGuid));
        }

        private BaseDefinitionBuilder(string name, string definitionGuid, Guid namespaceGuid, bool useNamespaceGuid, Category category)
        {
            Preconditions.IsNotNullOrWhiteSpace(name, nameof(name));

            Definition = ScriptableObject.CreateInstance<TDefinition>();
            Definition.name = name;

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

            if (category != Category.None)
            {
                Definition.GuiPresentation = BuildGuiPresentation(CreateTitleKey(name, category), CreateDescriptionKey(name, category));
            }
        }

        /// <summary>
        /// Create clone and rename. Automatically generate a guid from baseGuid plus name.
        /// Assign a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="name">The name assigned to the definition (mandatory)</param>
        /// <param name="namespaceGuid">The base or namespace guid from which to generate a guid for this definition, based on baseGuid+name (mandatory)</param>
        /// <param name="category">Used to generate title and description on the GuiPresentation.  The generated fields if
        /// name="MyDefinition" and category="MyCategory" are: MyCategory/&amp;MyDefinitionTitle and MyCategory/&amp;MyDefinitionDescription.
        /// If category=null then the copied GuiPresentation is not altered.
        /// </param>
        protected BaseDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid, Category category) :
            this(original, name, null, namespaceGuid, true, category)
        {
        }

        /// <summary>
        /// Create clone and rename. Assign the supplied guid as the definition guid.
        /// Assigns a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="name">The name assigned to the definition (mandatory)</param>
        /// <param name="definitionGuid">The guid for this definition (mandatory)</param>
        /// <param name="category">Used to generate title and description on the GuiPresentation.  The generated fields if
        /// name="MyDefinition" and category="MyCategory" are: MyCategory/&amp;MyDefinitionTitle and MyCategory/&amp;MyDefinitionDescription.
        /// If category=null then the copied GuiPresentation is not altered.
        /// </param>
        protected BaseDefinitionBuilder(TDefinition original, string name, string definitionGuid, Category category) :
            this(original, name, definitionGuid, Guid.Empty, false, category)
        {
        }

        private BaseDefinitionBuilder(TDefinition original, string name, string definitionGuid, Guid namespaceGuid, bool useNamespaceGuid, Category category)
        {
            Preconditions.IsNotNull(original, nameof(original));
            Preconditions.IsNotNullOrWhiteSpace(name, nameof(name));

            var originalName = original.name;
            var originalGuid = original.GUID;

            Definition = UnityEngine.Object.Instantiate(original);

            Definition.name = name;

            if (useNamespaceGuid)
            {
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

            if (category != Category.None)
            {
                if (Definition.GuiPresentation != null)
                {
                    Definition.GuiPresentation.Title = CreateTitleKey(name, category);
                    Definition.GuiPresentation.Description = CreateDescriptionKey(name, category);
                }
                else
                {
                    Definition.GuiPresentation =
                        BuildGuiPresentation(CreateTitleKey(name, category), CreateDescriptionKey(name, category));
                }
            }
        }

        /// <summary>
        /// Take ownership of a definition without changing the name or guid.
        /// </summary>
        /// <param name="original">The definition</param>
        protected BaseDefinitionBuilder(TDefinition original)
        {
            Preconditions.IsNotNull(original, nameof(original));
            Definition = original;
        }

        #endregion

        #region Backward compatibility constructors

        /// <summary>
        /// Create a new instance of TDefinition.
        /// A GuiPresentation will be assigned to the definition with the provided title and description.
        /// </summary>
        /// <param name="name">The unique name assigned to the definition (mandatory)</param>
        /// <param name="guid">The unique guid assigned to the definition (mandatory)</param>
        /// <param name="title">The title assigned to the GuiPresentation (optional)</param>
        /// <param name="description">The description assigned to the GuiPresentation (optional)</param>
        [Obsolete("Use one of the preferred constructors plus GuiPresentation extensions.")]
        protected BaseDefinitionBuilder(string name, string guid, string title, string description)
            : this(name, guid, BuildGuiPresentation(title, description))
        {
        }

        /// <summary>
        /// Create a new instance of TDefinition.
        /// A GuiPresentation will be assigned to the definition with the provided title and description.
        /// </summary>
        /// <param name="name">The unique name assigned to the definition (mandatory)</param>
        /// <param name="guid">The unique guid assigned to the definition (mandatory)</param>
        protected BaseDefinitionBuilder(string name, string guid)
            : this(name, guid, BuildGuiPresentation(null, null))
        {
        }

        /// <summary>
        /// Create a new instance of TDefinition. Assign the GuiPresentation provided.
        /// </summary>
        /// <param name="name">The unique name assigned to the definition (mandatory)</param>
        /// <param name="guid">The unique guid assigned to the definition (mandatory)</param>
        protected BaseDefinitionBuilder(string name, string guid, GuiPresentation guiPresentation)
        {
            Preconditions.IsNotNullOrWhiteSpace(name, nameof(name));
            Preconditions.IsNotNullOrWhiteSpace(guid, nameof(guid));

            Definition = ScriptableObject.CreateInstance<TDefinition>();

            Definition.name = name;
            Definition.SetField("guid", guid);

            Definition.GuiPresentation = guiPresentation;

            LogDefinition($"Old-Creating definition: ({name}, guid={Definition.GUID})");
        }

        /// <summary>
        /// Create clone and rename
        /// </summary>
        /// <param name="original">The original definition to be cloned.</param>
        /// <param name="name">The new unique name assigned to the definition (mandatory)</param>
        /// <param name="guid">The new unique guid assigned to the definition (mandatory)</param>
        protected BaseDefinitionBuilder(TDefinition original, string name, string guid) : this(original, name, guid, Category.None)
        {
        }

        /// <summary>
        /// Create clone and rename - compatibility
        /// </summary>
        /// <param name="name">The new unique name assigned to the definition (mandatory)</param>
        /// <param name="guid">The new unique guid assigned to the definition (mandatory)</param>
        /// <param name="title">The title assigned to the GuiPresentation (optional)</param>
        /// <param name="description">The description assigned to the GuiPresentation (optional)</param>
        /// <param name="base_Blueprint">The original definition to be cloned.</param>
        [Obsolete("Use one of the preferred constructors plus GuiPresentation extensions.")]
        protected BaseDefinitionBuilder(string name, string guid, string title, string description, TDefinition base_Blueprint)
            : this(base_Blueprint, name, guid, title, description)
        {
        }

        /// <summary>
        /// Create clone and rename
        /// </summary>
        /// <param name="original">The original definition to be cloned.</param>
        /// <param name="name">The new unique name assigned to the definition (mandatory)</param>
        /// <param name="guid">The new unique guid assigned to the definition (mandatory)</param>
        /// <param name="title">The title assigned to the GuiPresentation (optional)</param>
        /// <param name="description">The description assigned to the GuiPresentation (optional)</param>
        [Obsolete("Use one of the preferred constructors plus GuiPresentation extensions.")]
        protected BaseDefinitionBuilder(TDefinition original, string name, string guid, string title, string description)
        {
            Preconditions.IsNotNull(original, nameof(original));
            Preconditions.IsNotNullOrWhiteSpace(name, nameof(name));
            Preconditions.IsNotNullOrWhiteSpace(guid, nameof(guid));

            var originalName = original.name;
            var originalGuid = original.GUID;

            Definition = UnityEngine.Object.Instantiate(original);

            Definition.name = name;
            Definition.SetField("guid", guid);

            if (Definition.GuiPresentation != null)
            {
                if (title != null)
                {
                    Definition.GuiPresentation.Title = title;
                }

                if (description != null)
                {
                    Definition.GuiPresentation.Description = description;
                }
            }
            else
            {
                Definition.GuiPresentation = BuildGuiPresentation(title, description);
            }

            LogDefinition($"Old-Cloning definition: original({originalName}, {originalGuid}) => ({name}, {Definition.GUID})");
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
                var getDatabaseMethodInfoGeneric =
                    BaseDefinitionBuilderHelper.GetDatabaseMethodInfo.MakeGenericMethod(type);

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
        }
        #endregion

        protected TDefinition Definition { get; }
    }

    internal static class BaseDefinitionBuilderHelper
    {
        public static readonly MethodInfo GetDatabaseMethodInfo =
            typeof(DatabaseRepository).GetMethod("GetDatabase", BindingFlags.Public | BindingFlags.Static);
    }
}
