namespace SolastaUnfinishedBusiness.CustomValidators;

public delegate bool
    RepertoireValidForAutoPreparedFeature(RulesetSpellRepertoire repertoire, RulesetCharacter character);

internal static class ValidateRepertoireForAutoprepared
{
    internal static readonly RepertoireValidForAutoPreparedFeature AnyClassOrSubclass = (repertoire, _) =>
        repertoire.SpellCastingClass != null || repertoire.SpellCastingSubclass != null;

    internal static RepertoireValidForAutoPreparedFeature HasSpellCastingFeature(string featureName)
    {
        return (repertoire, _) => repertoire.spellCastingFeature.Name == featureName;
    }
}
