#if false
using System;
using SolastaCommunityExpansion.Builders;

namespace SolastaModApi
{
    /// <summary>
    ///     Base class builder for all classes derived from BaseDefinition.  For legacy public use by other mods.
    /// </summary>
    /// <typeparam name="TDefinition"></typeparam>
    [Obsolete("Use explicit builders. e.g. FightingStyleDefinitionBuilder")]
    public abstract class BaseDefinitionBuilder<TDefinition> : DefinitionBuilder<TDefinition> where TDefinition : BaseDefinition
    {
#region Constructors

        /// <summary>
        /// Create a new instance of TDefinition.  Automatically generate a guid from namespaceGuid plus name.
        /// Assign a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="name">The name assigned to the definition (mandatory)</param>
        /// <param name="namespaceGuid">The base or namespace guid from which to generate a guid for this definition, based on baseGuid+name (mandatory)</param>
        [Obsolete("Legacy support only.  Please use explicit builders for future development.")]
        protected BaseDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid) { }

        /// <summary>
        /// Create a new instance of TDefinition.  Assign the supplied guid as the definition guid.
        /// Assigns a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="name">The name assigned to the definition (mandatory)</param>
        /// <param name="definitionGuid">The guid for this definition (mandatory)</param>
        [Obsolete("Legacy support only.  Please use explicit builders for future development.")]
        protected BaseDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid) { }

        /// <summary>
        /// Create clone and rename. Automatically generate a guid from baseGuid plus name.
        /// </summary>
        /// <param name="original">The definition being copied.</param>
        /// <param name="name">The name assigned to the definition (mandatory).</param>
        /// <param name="namespaceGuid">The base or namespace guid from which to generate a guid for this definition, based on baseGuid+name (mandatory).</param>
        [Obsolete("Legacy support only.  Please use explicit builders for future development.")]
        protected BaseDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid) { }

        /// <summary>
        /// Create clone and rename. Assign the supplied guid as the definition guid.
        /// Assigns a GuiPresentation with a generated title key and description key.
        /// </summary>
        /// <param name="original">The definition being copied.</param>
        /// <param name="name">The name assigned to the definition (mandatory).</param>
        /// <param name="definitionGuid">The guid for this definition (mandatory).</param>
        [Obsolete("Legacy support only.  Please use explicit builders for future development.")]
        protected BaseDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid) { }

#endregion
    }
}
#endif
