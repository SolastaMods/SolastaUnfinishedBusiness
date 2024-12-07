using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GuiInvocationDefinitionPatcher
{
    //NOTE: There is a typo on official method name
    // ReSharper disable once StringLiteralTypo
    [HarmonyPatch(typeof(GuiInvocationDefinition), nameof(GuiInvocationDefinition.IsInvocationMacthingPrerequisites))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    // ReSharper disable once IdentifierTypo
    public static class IsInvocationMacthingPrerequisites_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            ref bool __result,
            InvocationDefinition invocation,
            RulesetCharacterHero hero,
            out string prerequisiteOutput)
        {
            __result = IsInvocationMatchingPrerequisites(invocation, hero, out prerequisiteOutput);

            return false;
        }

        private static bool IsInvocationMatchingPrerequisites(
            InvocationDefinition invocation,
            RulesetCharacterHero hero,
            out string prerequisiteOutput)
        {
            var isValid = true;

            prerequisiteOutput = string.Empty;

            if (invocation.RequiredLevel > 1)
            {
                var text = invocation.RequiredLevel.ToString();

                if (hero != null && invocation.RequiredLevel >
                    hero.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Warlock))
                {
                    isValid = false;
                    text = Gui.Colorize(text, "EA7171");
                }

                prerequisiteOutput += Gui.Format("Tooltip/&InvocationPrerequisiteLevelFormat", text);
            }

            if (invocation.RequiredKnownSpell)
            {
                if (!string.IsNullOrEmpty(prerequisiteOutput))
                {
                    prerequisiteOutput += "\n";
                }

                var text = Gui.Localize(invocation.RequiredKnownSpell.GuiPresentation.Title);

                if (hero != null)
                {
                    var hasSpell = hero.SpellRepertoires.Any(spellRepertoire =>
                        spellRepertoire.HasKnowledgeOfSpell(invocation.RequiredKnownSpell));

                    if (!hasSpell)
                    {
                        isValid = false;
                        text = Gui.Colorize(text, "EA7171");
                    }
                }

                prerequisiteOutput += Gui.Format("Tooltip/&InvocationPrerequisiteKnownSpellFormat", text);
            }

            // ReSharper disable once InvertIf
            if (invocation.RequiredPact)
            {
                if (!string.IsNullOrEmpty(prerequisiteOutput))
                {
                    prerequisiteOutput += "\n";
                }

                var text = Gui.Localize(invocation.RequiredPact.GuiPresentation.Title);

                if (hero != null)
                {
                    var hasPact = hero.ActiveFeatures.Any(activeFeature =>
                        activeFeature.Value.Contains(invocation.RequiredPact));

                    //PATCH: supports pacts as invocations as in tabletop 2024
                    if (!hasPact &&
                        Main.Settings.EnableWarlockInvocationProgression2024)
                    {
                        var invocationPactName = invocation.RequiredPact.Name.Replace("FeatureSet", "Invocation");

                        hasPact = hero.TrainedInvocations.Any(x => x.Name == invocationPactName);
                    }

                    if (!hasPact)
                    {
                        isValid = false;
                        text = Gui.Colorize(text, "EA7171");
                    }
                }

                prerequisiteOutput += Gui.Format("Tooltip/&InvocationPrerequisitePactTitle", text);
            }

            //PATCH: Enforces Invocations With PreRequisites
            if (invocation is not InvocationDefinitionWithPrerequisites invocationDefinitionWithPrerequisites
                || invocationDefinitionWithPrerequisites.Validators.Count == 0)
            {
                return isValid;
            }

            var (result, output) =
                invocationDefinitionWithPrerequisites.Validate(invocationDefinitionWithPrerequisites, hero);

            isValid = isValid && result;
            prerequisiteOutput += '\n' + output;

            return isValid;
        }
    }

    [HarmonyPatch(typeof(GuiInvocationDefinition), nameof(GuiInvocationDefinition.Subtitle), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Subtitle_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiInvocationDefinition __instance, ref string __result)
        {
            //PATCH: show custom subtitle for custom invocations
            var feature = __instance.InvocationDefinition as InvocationDefinitionCustom;

            if (!feature || feature.PoolType == null)
            {
                return;
            }

            __result = $"UI/&CustomFeatureSelectionTooltipType{feature.PoolType.Name}";
        }
    }
}
