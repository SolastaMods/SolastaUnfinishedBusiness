using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaCommunityExpansion.FightingStyles
{
    internal class Pugilist : AbstractFightingStyle
    {
        private CustomizableFightingStyleDefinition instance;

        internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
        {
            return new List<FeatureDefinitionFightingStyleChoice>() {
                FeatureDefinitionFightingStyleChoices.FightingStyleChampionAdditional,
                FeatureDefinitionFightingStyleChoices.FightingStyleFighter,
                FeatureDefinitionFightingStyleChoices.FightingStyleRanger,};
        }

        private sealed class FeatureDefinitionAdditionalDamageBuilder : Builders.Features.FeatureDefinitionAdditionalDamageBuilder
        {
            internal FeatureDefinitionAdditionalDamageBuilder(string name, string guid, string notificationTag, RuleDefinitions.FeatureLimitedUsage limitedUsage,
            RuleDefinitions.AdditionalDamageValueDetermination damageValueDetermination,
            RuleDefinitions.AdditionalDamageTriggerCondition triggerCondition, RuleDefinitions.AdditionalDamageRequiredProperty requiredProperty,
            bool attackModeOnly, RuleDefinitions.DieType damageDieType, int damageDiceNumber, RuleDefinitions.AdditionalDamageType additionalDamageType,
            string specificDamageType, RuleDefinitions.AdditionalDamageAdvancement damageAdvancement, List<DiceByRank> diceByRankTable,
            bool hasSavingThrow, string savingThrowAbility, int savingThrowDC, RuleDefinitions.EffectSavingThrowType damageSaveAffinity,
            List<ConditionOperationDescription> conditionOperations,
            GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetNotificationTag(notificationTag);
                Definition.SetLimitedUsage(limitedUsage);
                Definition.SetDamageValueDetermination(damageValueDetermination);
                Definition.SetTriggerCondition(triggerCondition);
                Definition.SetRequiredProperty(requiredProperty);
                Definition.SetAttackModeOnly(attackModeOnly);
                Definition.SetDamageDieType(damageDieType);
                Definition.SetDamageDiceNumber(damageDiceNumber);
                Definition.SetAdditionalDamageType(additionalDamageType);
                Definition.SetSpecificDamageType(specificDamageType);
                Definition.SetDamageAdvancement(damageAdvancement);
                Definition.DiceByRankTable.SetRange(diceByRankTable);
                Definition.SetDamageDieType(damageDieType);

                Definition.SetHasSavingThrow(hasSavingThrow);
                Definition.SetSavingThrowAbility(savingThrowAbility);
                Definition.SetSavingThrowDC(savingThrowDC);
                Definition.SetDamageSaveAffinity(damageSaveAffinity);
                Definition.ConditionOperations.SetRange(conditionOperations);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        internal override FightingStyleDefinition GetStyle()
        {
            if (instance == null)
            {
                EffectDescriptionBuilder offhandEffect = new EffectDescriptionBuilder();
                offhandEffect.SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.MeleeHit, 1, RuleDefinitions.TargetType.Individuals,
                    1, 1, ActionDefinitions.ItemSelectionType.None);
                offhandEffect.AddEffectForm(new EffectFormBuilder().CreatedByCharacter().SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus)
                    .SetDamageForm(false, RuleDefinitions.DieType.D10, "DamageBludgeoning", 1, RuleDefinitions.DieType.D8, 1,
                    RuleDefinitions.HealFromInflictedDamage.Never, new List<RuleDefinitions.TrendInfo>()).Build());

                var gui = GuiPresentationBuilder.Build("PugilistFighting", Category.FightingStyle, PathBerserker.GuiPresentation.SpriteReference);

                FeatureDefinitionPower offhandAttack = FeatureDefinitionPowerBuilder
                    .Create("PowerPugilistOffhandAttack", "a97a1c9c-232b-42ae-8003-30d244e958b3")
                    .SetGuiPresentation(gui)
                    .Configure(
                        1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Strength, RuleDefinitions.ActivationTime.BonusAction,
                        0, RuleDefinitions.RechargeRate.AtWill, true, true, AttributeDefinitions.Strength, offhandEffect.Build(), false /* unique */)
                    .SetShowCasting(false)
                    .AddToDB();

                var pugilistAdditionalDamage =
                    new FeatureDefinitionAdditionalDamageBuilder("AdditionalDamagePugilist", "36d24b2e-8ef4-4037-a82f-05e63d56f3d2", "Pugilist", RuleDefinitions.FeatureLimitedUsage.None,
                        RuleDefinitions.AdditionalDamageValueDetermination.Die, RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive, RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon,
                        true, RuleDefinitions.DieType.D8, 1, RuleDefinitions.AdditionalDamageType.SameAsBaseDamage, "", RuleDefinitions.AdditionalDamageAdvancement.None, new List<DiceByRank>(),
                        false, AttributeDefinitions.Constitution, 15, RuleDefinitions.EffectSavingThrowType.None, new List<ConditionOperationDescription>(), gui)
                    .AddToDB();


                instance = CustomizableFightingStyleBuilder
                    .Create("PugilistFightingStlye", "b14f91dc-8706-498b-a9a0-d583b7b00d09")
                    .SetFeatures(pugilistAdditionalDamage, offhandAttack)
                    .SetGuiPresentation(gui)
                    .SetIsActive(IsValid)
                    .AddToDB();
            }

            return instance;
        }

        private static bool IsValid(RulesetCharacterHero character)
        {
            RulesetInventorySlot mainHand = character.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand];
            RulesetInventorySlot offHand = character.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand];
            return mainHand.EquipedItem == null && (offHand.EquipedItem == null || offHand.EquipedItem.ItemDefinition.IsLightSourceItem);
        }
    }
}
