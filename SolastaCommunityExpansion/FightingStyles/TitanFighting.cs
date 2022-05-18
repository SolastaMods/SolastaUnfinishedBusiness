using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaCommunityExpansion.FightingStyles
{
    internal class TitanFighting : AbstractFightingStyle
    {
        internal const int IsSizeLargeOrMore = 1000;

        private readonly Guid TITAN_FIGHTING_BASE_GUID = new("3f7f25de-0ff9-4b63-b38d-8cd7f3a381fc");
        private FightingStyleDefinitionCustomizable instance;

        internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
        {
            return new List<FeatureDefinitionFightingStyleChoice>() {
                FightingStyleChampionAdditional,
                FightingStyleFighter,
                FightingStylePaladin};
        }

        internal override FightingStyleDefinition GetStyle()
        {
            if (instance == null)
            {
                var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
                    .Create(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageBracersOfArchery, "ModifierTitanFighting", TITAN_FIGHTING_BASE_GUID)
                    .SetTriggerCondition((RuleDefinitions.AdditionalDamageTriggerCondition)IsSizeLargeOrMore)
                    .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon)
                    .SetDamageDice(RuleDefinitions.DieType.D1, 2)
                    .AddToDB();

                instance = CustomizableFightingStyleBuilder
                    .Create("TitanFighting", "edc2a2d1-9f72-4825-b204-d810e911ed12")
                    .SetGuiPresentation("TitanFighting", Category.FightingStyle, DatabaseHelper.CharacterSubclassDefinitions.PathBerserker.GuiPresentation.SpriteReference)
                    .SetFeatures(additionalDamage)
                    .AddToDB();

            }
            return instance;
        }
    }
}
