using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Monk.Subclasses
{
    public static class ZenArcher
    {
        // Zen Archer's Monk weapons are all ranged weapons.
        private static readonly List<WeaponTypeDefinition> MonkWeapons = new()
        {
            WeaponTypeDefinitions.ShortbowType,
            WeaponTypeDefinitions.LongbowType,
            WeaponTypeDefinitions.LightCrossbowType,
            WeaponTypeDefinitions.HeavyCrossbowType,
            WeaponTypeDefinitions.DartType,
        };

        public static CharacterSubclassDefinition Build()
        {
            return CharacterSubclassDefinitionBuilder
                .Create("ClassMonkTraditionZenArcher", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Subclass,
                    CharacterSubclassDefinitions.RangerMarksman.GuiPresentation.SpriteReference)
                .AddFeaturesAtLevel(3, BuildLevel03Features())
                .AddFeaturesAtLevel(6, BuildLevel06Features())
                .AddFeaturesAtLevel(11, BuildLevel11Features())
                .AddFeaturesAtLevel(17, BuildLevel17Features())
                .AddToDB();
        }

        public static bool IsMonkWeapon(RulesetCharacter character, ItemDefinition item)
        {
            if (character == null || item == null)
            {
                return false;
            }

            var typeDefinition = item.WeaponDescription?.WeaponTypeDefinition;

            if (typeDefinition == null)
            {
                return false;
            }

            return character.HasSubFeatureOfType<ZenArcherMarker>()
                   && MonkWeapons.Contains(typeDefinition);
        }

        private static FeatureDefinition[] BuildLevel03Features()
        {
            return System.Array.Empty<FeatureDefinition>();
        }

        private static FeatureDefinition[] BuildLevel06Features()
        {
            return System.Array.Empty<FeatureDefinition>();
        }

        private static FeatureDefinition[] BuildLevel11Features()
        {
            return System.Array.Empty<FeatureDefinition>();
        }

        private static FeatureDefinition[] BuildLevel17Features()
        {
            return System.Array.Empty<FeatureDefinition>();
        }

        private class ZenArcherMarker
        {
        }
    }
}
