using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using static RuleDefinitions;

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
        public static IEnumerator Postfix([NotNull] IEnumerator values, CharacterActionWildShape __instance)
        {
            if (!Main.Settings.EnableActionSwitching)
            {
                yield return values;

                yield break;
            }

            //PATCH: changes Wildshape action to use power as NoCost so it doesn't consume main action twice and break action switching
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            if (actionService == null)
            {
                yield break;
            }

            var newParams = __instance.ActionParams.Clone();

            newParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.PowerNoCost];

            if (__instance.ActingCharacter.RulesetCharacter is RulesetCharacterMonster { IsSubstitute: true } monster)
            {
                newParams.ActingCharacter = monster.OriginalFormCharacter.EntityImplementation as GameLocationCharacter;
                monster.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagConjure,
                    ConditionWildShapeSubstituteForm);
            }

            actionService.ExecuteAction(newParams, null, true);
        }
    }
}
