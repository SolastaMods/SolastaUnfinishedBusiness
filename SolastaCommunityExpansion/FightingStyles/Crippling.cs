using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaCommunityExpansion.FightingStyles
{
    internal class Crippling : AbstractFightingStyle
    {
        public readonly Guid Namespace = new("3f7f25de-0ff9-4b63-b38d-8cd7f3a381fc");
        private FightingStyleDefinitionCustomizable instance;

        internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
        {
            return new List<FeatureDefinitionFightingStyleChoice>() {
                FightingStyleChampionAdditional,
                FightingStyleFighter,
                FightingStyleRanger,};
        }

        internal override FightingStyleDefinition GetStyle()
        {
            if (instance == null)
            {
                //? Prevent Dash until end of next turn -> how? it's not an action, but has a lot of dedicated code
                //+ Reduce speed by 10 until end of next turn
                //+ Must be a successful melee attack
                //+ NO LIMIT per round (wow!)
                var conditionDefinition = ConditionDefinitionBuilder
                    .Create(ConditionHindered_By_Frost, "CripplingConditionDefinition", Namespace)
                    .AddToDB()
                .SetAllowMultipleInstances(true);

                var conditionOperation = new ConditionOperationDescription();
                conditionOperation
                    .SetCanSaveToCancel(false)
                    .SetConditionDefinition(conditionDefinition)
                    .SetHasSavingThrow(false)
                    .SetOperation(ConditionOperationDescription.ConditionOperation.Add)
                    .SetSaveAffinity(RuleDefinitions.EffectSavingThrowType.None)
                    .SetSaveOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);

                var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
                    .Create(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageCircleBalanceColdEmbrace, "CripplingAdditionalDamage", Namespace)
                    .SetGuiPresentation(Category.Modifier)
                    .SetDamageDice(RuleDefinitions.DieType.D1, 0)
                    .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
                    .SetNotificationTag("CripplingFightingStyle")
                    .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon)
                    .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
                    .SetConditionOperations(conditionOperation)
                    .AddToDB();

                instance = CustomizableFightingStyleBuilder
                    .Create("Crippling", "b570d166-c65c-4a68-ab78-aeb16d491fce")
                    .SetGuiPresentation("Crippling", Category.FightingStyle, DatabaseHelper.CharacterSubclassDefinitions.RangerShadowTamer.GuiPresentation.SpriteReference)
                    .SetFeatures(additionalDamage)
                    .AddToDB();
            }
            return instance;
        }
    }
}
