using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Subclasses.Witch
{
    internal class PurpleWitch : AbstractSubclass
    {
        private static Guid SubclassNamespace = new Guid("32b329a3-689d-4364-bb72-d2444c788d34");
        private readonly CharacterSubclassDefinition Subclass;

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            DatabaseRepository.GetDatabase<FeatureDefinitionSubclassChoice>().TryGetElement("SubclassChoiceWitchCovens", out FeatureDefinitionSubclassChoice featureDefinitionSubclassChoice);
            return featureDefinitionSubclassChoice;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal PurpleWitch()
        {
            // Make Purple Witch subclass
            CharacterSubclassDefinitionBuilder purpleWitch = new CharacterSubclassDefinitionBuilder("PurpleWitch", GuidHelper.Create(SubclassNamespace, "PurpleWitch").ToString());
            GuiPresentationBuilder purpleWitchPresentation = new GuiPresentationBuilder(
                "Subclass/&PurpleWitchDescription",
                "Subclass/&PurpleWitchTitle");
            purpleWitchPresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RoguishShadowCaster.GuiPresentation.SpriteReference);
            purpleWitch.SetGuiPresentation(purpleWitchPresentation.Build());





            // add subclass to db and add subclass to Witch class
            Subclass = purpleWitch.AddToDB();
        }

    }
}
