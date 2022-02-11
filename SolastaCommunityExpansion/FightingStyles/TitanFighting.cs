using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;

namespace SolastaCommunityExpansion.FightingStyles
{
    internal class TitanFighting : AbstractFightingStyle
    {
        public readonly Guid TITAN_FIGHTING_BASE_GUID = new("3f7f25de-0ff9-4b63-b38d-8cd7f3a381fc");
        private CustomizableFightingStyle instance;

        internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
        {
            return new List<FeatureDefinitionFightingStyleChoice>() {};
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
                    .Create(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageBracersOfArchery, "TitanFighting", TITAN_FIGHTING_BASE_GUID)
                    // to extend with new condition? something like
//                        .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.IsSizeLargeOrMore)
                    .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
                    .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon)
                    .SetDamageDice(RuleDefinitions.DieType.D1, 2)
                    .AddToDB();
                
                GuiPresentationBuilder gui = new GuiPresentationBuilder("FightingStyle/&TitanFightingTitle", "FightingStyle/&TitanFightingDescription");
                gui.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.PathBerserker.GuiPresentation.SpriteReference);
                CustomizableFightingStyleBuilder builder = new CustomizableFightingStyleBuilder("TitanFighting", "edc2a2d1-9f72-4825-b204-d810e911ed12",
                    new List<FeatureDefinition>() { additionalDamage },
                    gui.Build());
                instance = builder.AddToDB();
            }
            return instance;
        }
    }
}
