using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.FightingStyles
{
    internal class Pugilist : AbstractFightingStyle
    {
        private CustomizableFightingStyle instance;

        internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
        {
            return new List<FeatureDefinitionFightingStyleChoice>() {
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStyleChampionAdditional,
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStyleFighter,
                DatabaseHelper.FeatureDefinitionFightingStyleChoices.FightingStyleRanger,};
        }

        private sealed class FeatureDefinitionAdditionalDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
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
                GuiPresentationBuilder gui = new GuiPresentationBuilder("FightingStyle/&PugilistFightingTitle", "FightingStyle/&PugilistFightingDescription");
                gui.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.PathBerserker.GuiPresentation.SpriteReference);

                EffectDescriptionBuilder offhandEffect = new EffectDescriptionBuilder();
                offhandEffect.SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.MeleeHit, 1, RuleDefinitions.TargetType.Individuals,
                    1, 1, ActionDefinitions.ItemSelectionType.None);
                offhandEffect.AddEffectForm(new EffectFormBuilder().CreatedByCharacter().SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus)
                    .SetDamageForm(false, RuleDefinitions.DieType.D10, "DamageBludgeoning", 1, RuleDefinitions.DieType.D8, 1,
                    RuleDefinitions.HealFromInflictedDamage.Never, new List<RuleDefinitions.TrendInfo>()).Build());
                FeatureDefinitionPower offhandAttack = new FeatureDefinitionPowerBuilder("PowerPugilistOffhandAttack", "a97a1c9c-232b-42ae-8003-30d244e958b3",
                    1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Strength, RuleDefinitions.ActivationTime.BonusAction,
                    0, RuleDefinitions.RechargeRate.AtWill, true, true, AttributeDefinitions.Strength,
                    offhandEffect.Build(), gui.Build(), false /* unique */).SetShowCasting(false).AddToDB();

                CustomizableFightingStyleBuilder builder = new CustomizableFightingStyleBuilder("PugilistFightingStlye", "b14f91dc-8706-498b-a9a0-d583b7b00d09",
                    new List<FeatureDefinition>() {
                        new FeatureDefinitionAdditionalDamageBuilder("AdditionalDamagePugilist", "36d24b2e-8ef4-4037-a82f-05e63d56f3d2", "Pugilist", RuleDefinitions.FeatureLimitedUsage.None,
                        RuleDefinitions.AdditionalDamageValueDetermination.Die, RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive, RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon,
                        true, RuleDefinitions.DieType.D8, 1, RuleDefinitions.AdditionalDamageType.SameAsBaseDamage, "", RuleDefinitions.AdditionalDamageAdvancement.None, new List<DiceByRank>(),
                        false, AttributeDefinitions.Constitution, 15, RuleDefinitions.EffectSavingThrowType.None, new List<ConditionOperationDescription>(), gui.Build()).AddToDB(),
                        offhandAttack,
                    },
                    gui.Build())
                    .SetIsActive(IsValid);
                instance = builder.AddToDB();
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
