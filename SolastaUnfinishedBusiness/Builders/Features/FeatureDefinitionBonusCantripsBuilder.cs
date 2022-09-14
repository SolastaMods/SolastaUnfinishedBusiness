// using System;
// using System.Collections.Generic;
// using System.Linq;
// using SolastaUnfinishedBusiness.Api.Infrastructure;
// using SolastaUnfinishedBusiness.CustomDefinitions;
// using SolastaUnfinishedBusiness.CustomInterfaces;
//
// namespace SolastaUnfinishedBusiness.Builders.Features;
//
// public abstract class
//     FeatureDefinitionBonusCantripsBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
//     where TDefinition : FeatureDefinitionBonusCantrips
//     where TBuilder : FeatureDefinitionBonusCantripsBuilder<TDefinition, TBuilder>
// {
//     public TBuilder ClearBonusCantrips()
//     {
//         Definition.BonusCantrips.Clear();
//         return This();
//     }
//
//     public TBuilder AddBonusCantrip(SpellDefinition spellDefinition)
//     {
//         Definition.BonusCantrips.Add(spellDefinition);
//         Definition.BonusCantrips.Sort(Sorting.Compare);
//         return This();
//     }
//
//     public TBuilder SetBonusCantrips(params SpellDefinition[] spellDefinitions)
//     {
//         SetBonusCantrips(spellDefinitions.AsEnumerable());
//         return This();
//     }
//
//     public TBuilder SetBonusCantrips(IEnumerable<SpellDefinition> spellDefinitions)
//     {
//         Definition.BonusCantrips.SetRange(spellDefinitions);
//         Definition.BonusCantrips.Sort(Sorting.Compare);
//         return This();
//     }
//
//     #region Constructors
//
//     protected FeatureDefinitionBonusCantripsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
//     {
//     }
//
//     protected FeatureDefinitionBonusCantripsBuilder(string name, string definitionGuid) : base(name, definitionGuid)
//     {
//     }
//
//     protected FeatureDefinitionBonusCantripsBuilder(TDefinition original, string name, Guid namespaceGuid) : base(
//         original, name, namespaceGuid)
//     {
//     }
//
//     protected FeatureDefinitionBonusCantripsBuilder(TDefinition original, string name, string definitionGuid) :
//         base(original, name, definitionGuid)
//     {
//     }
//
//     #endregion
// }
//
// public class FeatureDefinitionBonusCantripsBuilder : FeatureDefinitionBonusCantripsBuilder<
//     FeatureDefinitionBonusCantrips, FeatureDefinitionBonusCantripsBuilder>
// {
//     #region Constructors
//
//     public FeatureDefinitionBonusCantripsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
//     {
//     }
//
//     public FeatureDefinitionBonusCantripsBuilder(string name, string definitionGuid) : base(name, definitionGuid)
//     {
//     }
//
//     public FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name,
//         Guid namespaceGuid) : base(original, name, namespaceGuid)
//     {
//     }
//
//     public FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name,
//         string definitionGuid) : base(original, name, definitionGuid)
//     {
//     }
//
//     #endregion
// }
//
// public class FeatureDefinitionBonusCantripsWithPrerequisitesBuilder : FeatureDefinitionBonusCantripsBuilder<
//     FeatureDefinitionBonusCantripsWithPrerequisites, FeatureDefinitionBonusCantripsWithPrerequisitesBuilder>
// {
//     public FeatureDefinitionBonusCantripsWithPrerequisitesBuilder SetValidators(
//         params IFeatureDefinitionWithPrerequisites.Validate[] validators)
//     {
//         Definition.Validators.AddRange(validators);
//
//         return this;
//     }
//
//     #region Constructors
//
//     public FeatureDefinitionBonusCantripsWithPrerequisitesBuilder(string name, Guid namespaceGuid) : base(name,
//         namespaceGuid)
//     {
//     }
//
//     public FeatureDefinitionBonusCantripsWithPrerequisitesBuilder(string name, string definitionGuid) : base(
//         name,
//         definitionGuid)
//     {
//     }
//
//     public FeatureDefinitionBonusCantripsWithPrerequisitesBuilder(
//         FeatureDefinitionBonusCantripsWithPrerequisites original, string name,
//         Guid namespaceGuid) : base(original, name, namespaceGuid)
//     {
//     }
//
//     public FeatureDefinitionBonusCantripsWithPrerequisitesBuilder(
//         FeatureDefinitionBonusCantripsWithPrerequisites original, string name,
//         string definitionGuid) : base(original, name, definitionGuid)
//     {
//     }
//
//     #endregion
// }
