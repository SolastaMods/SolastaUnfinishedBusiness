using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SkillDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Backgrounds;

internal static partial class BackgroundsBuilders
{
    private const string BackgroundDevoted = "BackgroundDevoted";

    internal static CharacterBackgroundDefinition BuildBackgroundDevoted()
    {
        return CharacterBackgroundDefinitionBuilder
            .Create(BackgroundDevoted)
            .SetGuiPresentation(Category.Background,
                Sprites.GetSprite(BackgroundDevoted, Resources.BackgroundDevoted, 1024, 512))
            .SetBanterList(BanterDefinitions.BanterList.Formal)
            .SetFeatures(
                FeatureDefinitionCastSpellBuilder
                    // kept name for backward compatibility
                    .Create(CastSpellGnomeShadow, $"BonusCantrips{BackgroundDevoted}")
                    .SetGuiPresentation(Category.Feature)
                    .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
                    .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
                    .SetSpellList(
                        SpellListDefinitionBuilder
                            .Create($"SpellList{BackgroundDevoted}")
                            .SetGuiPresentationNoContent(true)
                            .ClearSpells()
                            .SetSpellsAtLevel(0, SacredFlame)
                            .FinalizeSpells()
                            .AddToDB())
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{BackgroundDevoted}Skills")
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(ProficiencyType.Skill,
                        Religion,
                        Insight,
                        Investigation)
                    .AddToDB())
            .AddDefaultOptionalPersonality("Pragmatism")
            .AddDefaultOptionalPersonality("Friendliness")
            .AddOptionalPersonality("Self-Preservation", 8)
            .AddOptionalPersonality("Selfishness", 8)
            .AddOptionalPersonality("Pragmatism", 8)
            .AddOptionalPersonality("Friendliness", 8)
            .AddStaticPersonality("Slang", 30)
            .AddStaticPersonality("Normal", 5)
            .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(ClothesNoble_Valley, EquipmentDefinitions.OptionArmor, 1),
                EquipmentOptionsBuilder.Option(PotionOfHealing, EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(Torch, EquipmentDefinitions.OptionGenericItem, 1),
                EquipmentOptionsBuilder.Option(Food_Ration, EquipmentDefinitions.OptionGenericItem, 5)
            })
            .AddToDB();
    }
}
