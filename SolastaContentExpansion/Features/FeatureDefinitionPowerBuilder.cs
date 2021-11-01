

using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaContentExpansion.Features
{
    public class FeatureDefinitionPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        public FeatureDefinitionPowerBuilder(string name, string guid, int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
            string usesAbilityScoreName,
            RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
            EffectDescription effectDescription, GuiPresentation guiPresentation, bool uniqueInstance) : base(name, guid)
        {
            Definition.SetFixedUsesPerRecharge(usesPerRecharge);
            Definition.SetUsesDetermination(usesDetermination);
            Definition.SetUsesAbilityScoreName(usesAbilityScoreName);
            Definition.SetActivationTime(activationTime);
            Definition.SetCostPerUse(costPerUse);
            Definition.SetRechargeRate(recharge);
            Definition.SetProficiencyBonusToAttack(proficiencyBonusToAttack);
            Definition.SetAbilityScoreBonusToAttack(abilityScoreBonusToAttack);
            Definition.SetAbilityScore(abilityScore);
            Definition.SetEffectDescription(effectDescription);
            Definition.SetGuiPresentation(guiPresentation);
            Definition.SetUniqueInstance(uniqueInstance);
        }
    }
}
