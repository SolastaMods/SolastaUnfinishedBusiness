using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock
{
    internal class FeatureDefinitionProficiencyBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        public FeatureDefinitionProficiencyBuilder(string name, string guid, RuleDefinitions.ProficiencyType type,
        List<string> proficiencies, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetProficiencyType(type);
            Definition.SetGuiPresentation(guiPresentation);

            foreach (var item in proficiencies)
            {
                Definition.Proficiencies.Add(item);
            }
        }

        internal static FeatureDefinitionProficiency Build(RuleDefinitions.ProficiencyType type, List<string> proficiencies, string name, GuiPresentation guiPresentation)
        {
            var builder = new FeatureDefinitionProficiencyBuilder(name, GuidHelper.Create(new Guid(Settings.GUID), name).ToString(), type, proficiencies, guiPresentation);

            return builder.AddToDB();
        }
    }

    internal class FeatureDefinitionPointPoolBuilder : BaseDefinitionBuilder<FeatureDefinitionPointPool>
    {
        public FeatureDefinitionPointPoolBuilder(string name, string guid, HeroDefinitions.PointsPoolType poolType, int poolAmount,
        List<string> choices, bool uniqueChoices, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetPoolType(poolType);
            Definition.SetPoolAmount(poolAmount);

            foreach (var item in choices)
            {
                Definition.RestrictedChoices.Add(item);
            }

            Definition.SetUniqueChoices(uniqueChoices);
            Definition.SetGuiPresentation(guiPresentation);
        }

        internal static FeatureDefinitionPointPool Build(HeroDefinitions.PointsPoolType poolType, int poolAmount, List<string> choices, string name, GuiPresentation guiPresentation)
        {
            var builder = new FeatureDefinitionPointPoolBuilder(name, GuidHelper.Create(new Guid(Settings.GUID), name).ToString(), poolType, poolAmount, choices, false, guiPresentation);

            return builder.AddToDB();
        }
    }
}
