using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerFeyWanderer : AbstractSubclass
{
    private const string Name = "RangerFeyWanderer";

    public RangerFeyWanderer()
    {
        //
        // LEVEL 03
        //

        // Fey Wanderer Magic

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, CharmPerson),
                BuildSpellGroup(5, MistyStep),
                BuildSpellGroup(9, DispelMagic),
                BuildSpellGroup(13, DimensionDoor),
                BuildSpellGroup(17, SpellsContext.SteelWhirlwind))
            .AddToDB();

        // Dreadful Strikes

        var additionalDamageDreadfulStrikes = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DreadfulStrikes")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DreadfulStrikes")
            .SetDamageDice(DieType.D4, 1)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetAttackModeOnly()
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddToDB();

        additionalDamageDreadfulStrikes.AddCustomSubFeatures(
            new ModifyAdditionalDamageDreadfulStrikes(additionalDamageDreadfulStrikes));

        // Otherworldly Glamour

        var pointPoolOtherworldlyGlamour = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{Name}OtherworldlyGlamour")
            .SetGuiPresentationNoContent(true)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 3)
            .RestrictChoices(
                SkillDefinitions.Deception,
                SkillDefinitions.Performance,
                SkillDefinitions.Persuasion)
            .AddToDB();

        var abilityCheckAffinityOtherworldlyGlamour = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}OtherworldlyGlamour")
            .SetGuiPresentation($"FeatureSet{Name}OtherworldlyGlamour", Category.Feature, Gui.NoLocalization)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, AttributeDefinitions.Charisma)
            .AddToDB();

        var featureSetOtherworldlyGlamour = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}OtherworldlyGlamour")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(pointPoolOtherworldlyGlamour, abilityCheckAffinityOtherworldlyGlamour)
            .AddToDB();

        //
        // LEVEL 07
        //

        // Beguiling Twist
        
        // You have advantage on saving throws against being charmed or frightened.
        // In addition, whenever you or a creature you can see within 120 feet of you succeeds on a saving throw against being charmed or frightened,
        // you can use your reaction to force a different creature you can see within 120 feet of you to make a Wisdom saving throw against your spell save DC.
        // If the save fails, the target is charmed or frightened by you (your choice) for 1 minute. The target can repeat the saving throw at the end of each of its turns, ending the effect on itself on a successful save.

        //
        // LEVEL 11
        //

        // Fey Reinforcements

        // you know the spell Summon Fey. It doesn't count against the number of ranger spells you know, and you can cast it without a material component.
        // You can also cast it once without using a spell slot, and you regain the ability to do so when you finish a long rest.
        // Whenever you start casting the spell, you can modify it so that it doesn't require concentration. If you do so, the spell's duration becomes 1 minute for that casting.

        //
        // LEVEL 15
        //

        // Misty Wanderer

        var powerMistyWanderer = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MistyWanderer")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(MistyStep)
                    .InviteOptionalAlly()
                    .Build())
            .AddToDB();

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CircleOfTheAncientForest, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells, additionalDamageDreadfulStrikes, featureSetOtherworldlyGlamour)
            .AddFeaturesAtLevel(7)
            .AddFeaturesAtLevel(11)
            .AddFeaturesAtLevel(15, powerMistyWanderer)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Ranger;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyAdditionalDamageDreadfulStrikes(
        FeatureDefinitionAdditionalDamage additionalDamageDreadfulStrikes) : IModifyAdditionalDamage
    {
        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm damageForm)
        {
            if (featureDefinitionAdditionalDamage != additionalDamageDreadfulStrikes)
            {
                return;
            }

            var classLevel = attacker.RulesetCharacter.GetClassLevel(CharacterClassDefinitions.Ranger);

            if (classLevel < 11)
            {
                return;
            }

            damageForm.DieType = DieType.D6;
        }
    }
}
