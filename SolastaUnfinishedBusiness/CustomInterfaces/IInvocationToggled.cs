namespace SolastaUnfinishedBusiness.CustomInterfaces;

// This only handles toggle in UI
// Bind to invocation's granted feature, not invocation itself
internal interface IInvocationToggled
{
    void OnInvocationToggled(
        GameLocationCharacter character,
        RulesetInvocation rulesetInvocation);
}
