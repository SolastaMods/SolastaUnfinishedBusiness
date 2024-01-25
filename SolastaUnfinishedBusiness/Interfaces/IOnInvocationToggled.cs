namespace SolastaUnfinishedBusiness.Interfaces;

// This only handles toggle in UI
// Bind to invocation's granted feature, not invocation itself
internal interface IOnInvocationToggled
{
    void OnInvocationToggled(
        GameLocationCharacter character,
        RulesetInvocation rulesetInvocation);
}
