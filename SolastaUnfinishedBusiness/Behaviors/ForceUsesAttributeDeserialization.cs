using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Behaviors;

public class ForceUsesAttributeDeserialization
{
    private ForceUsesAttributeDeserialization()
    {
    }

    public static ForceUsesAttributeDeserialization Mark { get; } = new();

    internal static void Process(RulesetCharacter character, IElementsSerializer serializer)
    {
        if (serializer.Mode != Serializer.SerializationMode.Read) { return; }

        var usablePowers = character.usablePowers;

        foreach (var usablePower in usablePowers)
        {
            var powerDefinition = usablePower.PowerDefinition;
            if (!powerDefinition.HasSubFeatureOfType<ForceUsesAttributeDeserialization>()) { continue; }

            usablePower.UsesAttribute = character.GetAttribute(powerDefinition.UsesAbilityScoreName);
        }
    }
}
