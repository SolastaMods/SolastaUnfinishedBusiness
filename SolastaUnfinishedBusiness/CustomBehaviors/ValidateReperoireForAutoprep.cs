namespace SolastaUnfinishedBusiness.CustomBehaviors;

public delegate bool
    RepertoireValidForAutoPreperedFeature(RulesetSpellRepertoire repertoire, RulesetCharacter character);

internal static class ValidateReperoireForAutoprep
{
    internal static readonly RepertoireValidForAutoPreperedFeature AnyClassOrSubclass = (repertoire, _) =>
        repertoire.SpellCastingClass != null || repertoire.SpellCastingSubclass != null;

    internal static RepertoireValidForAutoPreperedFeature HasSpellCastingFeature(string featureName) =>
        (repertoire, _) => repertoire.spellCastingFeature.Name == featureName;
}
