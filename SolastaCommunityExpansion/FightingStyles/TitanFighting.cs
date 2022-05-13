using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi;

namespace SolastaCommunityExpansion.FightingStyles
{
    internal class TitanFighting : AbstractFightingStyle
    {
        public readonly Guid TITAN_FIGHTING_BASE_GUID = new("3f7f25de-0ff9-4b63-b38d-8cd7f3a381fc");
        private FightingStyleDefinitionCustomizable instance;

        internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
        {
            return new List<FeatureDefinitionFightingStyleChoice>() { };
        }

        internal override FightingStyleDefinition GetStyle()
        {
            if (instance == null)
            {

                // This seems to be in HandleCharacterAttackDamage in GameLocationBattleManager.cs
                // Perhaps adding a new TriggerConditionAdditionalDamage to check for enemy size (Large or more)?
                // This feels like deja vu with doing some patchwork in a very long function
                // For now, give a flat +2 melee dmg
                var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
                    .Create(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageBracersOfArchery, "ModifierTitanFighting", TITAN_FIGHTING_BASE_GUID)
                    // to extend with new condition? something like
                    //                        .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.IsSizeLargeOrMore)
                    .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
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
