using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Diagnostics;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public interface ICustomFightingStyle
    {
        bool IsActive(RulesetCharacterHero character);
    }

    public delegate bool IsActiveFightingStyleDelegate(RulesetCharacterHero character);

    public class CustomizableFightingStyle : FightingStyleDefinition, ICustomFightingStyle
    {
        private IsActiveFightingStyleDelegate isActive;

        internal void SetIsActiveDelegate(IsActiveFightingStyleDelegate del)
        {
            isActive = del;
        }

        public bool IsActive(RulesetCharacterHero character)
        {
            return isActive == null || isActive(character);
        }
    }

    public class CustomizableFightingStyleBuilder : BaseDefinitionBuilder<CustomizableFightingStyle>
    {
        public CustomizableFightingStyleBuilder(string name, string guid,
            List<FeatureDefinition> features, GuiPresentation guiPresentation) : base(name, guid)
        {
            // The features field is not automatically initialized.
            Definition.SetField("features", new List<FeatureDefinition>(features));
            Definition.SetGuiPresentation(guiPresentation);
        }

        public CustomizableFightingStyleBuilder SetIsActive(IsActiveFightingStyleDelegate del)
        {
            Definition.SetIsActiveDelegate(del);
            return this;
        }

        // Can this be removed now?
        /**
         * The ModAPI doesn't handle adding extensions of things that aren't feature definitions well.
         * Override the mod api logic and add this to the appropriate DB.
         */
        public new CustomizableFightingStyle AddToDB(bool assertIfDuplicate = true)
        {
            Preconditions.IsNotNull(Definition, nameof(Definition));
            Preconditions.IsNotNullOrWhiteSpace(Definition.Name, "definition.Name");
            Preconditions.IsNotNullOrWhiteSpace(Definition.GUID, "definition.GUID");

            var db = DatabaseRepository.GetDatabase<FightingStyleDefinition>();

            Assert.IsNotNull(db, $"Database '{typeof(FightingStyleDefinition).Name}' not found.");

            if (assertIfDuplicate)
            {
                if (db.HasElement(Definition.name))
                {
                    throw new SolastaModApiException(
                        $"The definition with name '{Definition.name}' already exists in database '{typeof(FightingStyleDefinition).Name}' by name.");
                }

                if (db.HasElementByGuid(Definition.GUID))
                {
                    throw new SolastaModApiException(
                        $"The definition with name '{Definition.name}' and guid '{Definition.GUID}' already exists in database '{typeof(FightingStyleDefinition).Name}' by GUID.");
                }
            }

            db.Add(Definition);
            return Definition;
        }
    }
}
