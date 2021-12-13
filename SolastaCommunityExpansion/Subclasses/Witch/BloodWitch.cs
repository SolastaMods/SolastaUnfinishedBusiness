using SolastaModApi;
using System;

namespace SolastaCommunityExpansion.Subclasses.Witch
{
    internal class BloodWitch : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new Guid("7141b0af-cc97-4e66-9700-e91deede0640");
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

        internal BloodWitch()
        {
            // Make Blood Witch subclass
            CharacterSubclassDefinitionBuilder bloodWitch = new CharacterSubclassDefinitionBuilder("BloodWitch", GuidHelper.Create(SubclassNamespace, "BloodWitch").ToString());
            GuiPresentationBuilder bloodWitchPresentation = new GuiPresentationBuilder(
                "Subclass/&BloodWitchDescription",
                "Subclass/&BloodWitchTitle");
            bloodWitchPresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RoguishShadowCaster.GuiPresentation.SpriteReference);
            bloodWitch.SetGuiPresentation(bloodWitchPresentation.Build());

            // add subclass to db and add subclass to Witch class
            Subclass = bloodWitch.AddToDB();
        }
    }
}
