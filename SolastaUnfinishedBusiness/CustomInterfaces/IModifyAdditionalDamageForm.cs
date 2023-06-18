namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyAdditionalDamageForm
{
    void OnModifyAdditionalDamageForm(
        FeatureDefinition featureDefinition, 
        DamageForm additionalDamageForm, 
        RulesetAttackMode attackMode, 
        GameLocationCharacter attacker, 
        GameLocationCharacter defender);
    
}
