using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionAdditionalDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
    {
        public FeatureDefinitionAdditionalDamageBuilder(string name, string guid,
            string notificationTag, RuleDefinitions.FeatureLimitedUsage limitedUsage,
        RuleDefinitions.AdditionalDamageValueDetermination damageValueDetermination,
        RuleDefinitions.AdditionalDamageTriggerCondition triggerCondition, RuleDefinitions.AdditionalDamageRequiredProperty requiredProperty,
        bool attackModeOnly, RuleDefinitions.DieType damageDieType, int damageDiceNumber, RuleDefinitions.AdditionalDamageType additionalDamageType,
        string specificDamageType, RuleDefinitions.AdditionalDamageAdvancement damageAdvancement, List<DiceByRank> diceByRankTable,
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
            Definition.SetField("diceByRankTable", diceByRankTable);
            Definition.SetDamageDieType(damageDieType);
            Definition.SetGuiPresentation(guiPresentation);
            Definition.SetField("conditionOperations", new List<ConditionOperationDescription>());
            Definition.SetField("familiesWithAdditionalDice", new List<string>());
        }
    }
}
