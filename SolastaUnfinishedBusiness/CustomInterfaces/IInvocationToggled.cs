using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

// This only handles toggle in UI
// Bind to invocation's grantedfeature, not invocation itself
internal interface IInvocationToggled
{
    void OnInvocationToggled(
        GameLocationCharacter character,
        RulesetInvocation rulesetInvocation);
}
