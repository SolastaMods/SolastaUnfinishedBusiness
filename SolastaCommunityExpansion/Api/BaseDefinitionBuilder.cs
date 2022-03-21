using System;
using SolastaCommunityExpansion.Builders;

namespace SolastaModApi
{
    /// <summary>
    ///     Base class builder for all classes derived from BaseDefinition.  For public use by other mods.
    /// </summary>
    /// <typeparam name="TDefinition"></typeparam>
    public abstract class BaseDefinitionBuilder<TDefinition> : DefinitionBuilder<TDefinition> where TDefinition : BaseDefinition
    {
        #region Constructors

        /// <summary>
        /// Create a new instance of TDefinition.  Automatically generate a guid from namespaceGuid plus name.
        /// Assign a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="name">The name assigned to the definition (mandatory)</param>
        /// <param name="namespaceGuid">The base or namespace guid from which to generate a guid for this definition, based on baseGuid+name (mandatory)</param>
        [Obsolete("Legacy support only.  Please use BaseDefinitionBuilder('name') for future development.")]
        protected BaseDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid) { }

        /// <summary>
        /// Create a new instance of TDefinition.  Assign the supplied guid as the definition guid.
        /// Assigns a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="name">The name assigned to the definition (mandatory)</param>
        /// <param name="definitionGuid">The guid for this definition (mandatory)</param>
        [Obsolete("Legacy support only.  Please use BaseDefinitionBuilder('name') for future development.")]
        protected BaseDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid) { }

        /// <summary>
        /// TODO: ... Create definition given 'name' only.
        /// Name = _CE_{name}
        /// Guid = CreateGuid(name, CENamespaceGuid)
        /// GuiPresentation = CommunityExpansion/&{name}Title, CommunityExpansion/&{name}Description, but can be overridden.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="createGuiPresentation"></param>
        protected BaseDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation) { }

        /// <summary>
        /// TODO: ... comment
        /// </summary>
        /// <param name="original"></param>
        /// <param name="name"></param>
        /// <param name="createGuiPresentation"></param>
        protected BaseDefinitionBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation) { }

        /// <summary>
        /// Create clone and rename. Automatically generate a guid from baseGuid plus name.
        /// </summary>
        /// <param name="original">The definition being copied.</param>
        /// <param name="name">The name assigned to the definition (mandatory).</param>
        /// <param name="namespaceGuid">The base or namespace guid from which to generate a guid for this definition, based on baseGuid+name (mandatory).</param>
        [Obsolete("Legacy support only.  Please use BaseDefinitionBuilder(original, 'name') for future development.")]
        protected BaseDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid) { }

        /// <summary>
        /// Create clone and rename. Assign the supplied guid as the definition guid.
        /// Assigns a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="original">The definition being copied.</param>
        /// <param name="name">The name assigned to the definition (mandatory).</param>
        /// <param name="definitionGuid">The guid for this definition (mandatory).</param>
      //  [Obsolete("Legacy support only.  Please use BaseDefinitionBuilder(original, 'name') for future development.")]
        protected BaseDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid) { }

        /// <summary>
        /// Take ownership of a definition without changing the name or guid.
        /// </summary>
        /// <param name="original">The definition</param>
        protected BaseDefinitionBuilder(TDefinition original) : base(original) { }

        #endregion
    }
}
