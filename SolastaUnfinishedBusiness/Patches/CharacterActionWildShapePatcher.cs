using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public class CharacterActionWildShapePatcher
{
    [HarmonyPatch(typeof(CharacterActionWildShape), nameof(CharacterActionWildShape.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
#pragma warning disable IDE0060
        //values are not used but required for patch to work
        public static IEnumerator Postfix([NotNull] IEnumerator values, CharacterActionWildShape __instance)
#pragma warning restore IDE0060
        {
            //PATCH: changes Wildshape action to use power as NoCost so it doesn't consume main action twice and break action switching
            var service = ServiceRepository.GetService<IGameLocationActionService>();
            var newParams = __instance.ActionParams.Clone();
            newParams.ActionDefinition = service.AllActionDefinitions[ActionDefinitions.Id.PowerNoCost];
            if (__instance.ActingCharacter.RulesetCharacter is RulesetCharacterMonster {IsSubstitute: true} monster)
            {
                newParams.ActingCharacter = monster.OriginalFormCharacter.EntityImplementation as GameLocationCharacter;
                monster.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagConjure,
                    RuleDefinitions.ConditionWildShapeSubstituteForm);
            }

            service.ExecuteAction(newParams, null, true);
            yield break;
        }
    }
}
