using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock
{
    internal class FeatureDefinitionPointPoolBuilder : Builders.Features.FeatureDefinitionPointPoolBuilder
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
