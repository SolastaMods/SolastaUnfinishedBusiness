using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal class PrimalChampionBuilder : BaseDefinitionBuilder<PrimalChampion>
    {
        private const string PrimalChampionName = "ZSPrimalChampion";
        private const string PrimalChampionGuid = "118a5ea1-8a19-4bee-9db1-7a2464c8e7b5";

        protected PrimalChampionBuilder(string name, string guid) : base(name, guid)
        {

            Definition.GuiPresentation.Description = "Feature/&PrimalChampionDescription";
            Definition.GuiPresentation.Title = "Feature/&PrimalChampionTitle";
        }

        private static PrimalChampion CreateAndAddToDB(string name, string guid)
            => new PrimalChampionBuilder(name, guid).AddToDB();

        internal static readonly PrimalChampion PrimalChampion =
            CreateAndAddToDB(PrimalChampionName, PrimalChampionGuid);
    }

    class PrimalChampion : FeatureDefinitionCustomCode
    {
        public override void ApplyFeature(RulesetCharacterHero hero)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Strength, 4);
            ModifyAttributeAndMax(hero, AttributeDefinitions.Constitution, 4);

            hero.RefreshAll();
        }

        public override void RemoveFeature(RulesetCharacterHero hero)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Strength, -4);
            ModifyAttributeAndMax(hero, AttributeDefinitions.Constitution, -4);

            hero.RefreshAll();
        }

        private void ModifyAttributeAndMax(RulesetCharacterHero hero, string attributeName, int amount)
        {
            RulesetAttribute attribute = hero.GetAttribute(attributeName);
            attribute.BaseValue += amount;
            attribute.MaxValue += amount;
            attribute.MaxEditableValue += amount;
            attribute.Refresh();

            RulesetActor.AbilityScoreIncreasedHandler abilityScoreIncreased = hero.AbilityScoreIncreased;
            if (abilityScoreIncreased != null)
            {
                abilityScoreIncreased((RulesetActor)hero, attributeName, amount, amount);
            }
        }
    }
}
