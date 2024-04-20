namespace SolastaUnfinishedBusiness.Validators;

public delegate bool
    RepertoireValidForAutoPreparedFeature(RulesetSpellRepertoire repertoire, RulesetCharacter character);

internal static class ValidateRepertoireForAutoprepared
{
    internal static readonly RepertoireValidForAutoPreparedFeature AnyClassOrSubclass = (repertoire, _) =>
        repertoire.SpellCastingClass || repertoire.SpellCastingSubclass;

    internal static RepertoireValidForAutoPreparedFeature HasSpellCastingFeature(string featureName)
    {
        return (repertoire, _) => repertoire.spellCastingFeature.Name == featureName;
    }
}
