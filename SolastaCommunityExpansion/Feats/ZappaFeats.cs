using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using static SolastaCommunityExpansion.Feats.FeatsValidators;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MetamagicOptionDefinitions;

namespace SolastaCommunityExpansion.Feats;

internal static class ZappaFeats
{
    internal const string ElvenAccuracyTag = "ElvenAccuracy";

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        // Arcane Defense
        var arcaneDefense = FeatDefinitionBuilder
            .Create("FeatArcaneDefense", DefinitionBuilder.CENamespaceGuid)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                FeatureDefinitionAttributeModifierBuilder
                    .Create(AttributeModifierBarbarianUnarmoredDefense, "AttributeModifierFeatArcaneDefenseAdd",
                        DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentationNoContent()
                    .SetCustomSubFeatures(ExclusiveArmorClassBonus.Marker)
                    .SetSituationalContext(RuleDefinitions.SituationalContext.NotWearingArmorOrMageArmor)
                    .SetModifierAbilityScore(AttributeDefinitions.Intelligence)
                    .AddToDB()
            )
            .SetAbilityScorePrerequisite(AttributeDefinitions.Intelligence, 13)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Arcane Precision
        var attackModifierArcanePrecision = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierArcanePrecision", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("FeatArcanePrecision", Category.Feat,
                FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
            .SetAbilityScoreReplacement(RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility)
            .SetAdditionalAttackTag(TagsDefinitions.Magical)
            .AddToDB();

        var effectArcanePrecision = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1 /* range */,
                RuleDefinitions.TargetType.Item, 1, 2, ActionDefinitions.ItemSelectionType.Weapon)
            .SetCreatedByCharacter()
            .SetDurationData(RuleDefinitions.DurationType.Minute, 1 /* duration */,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetItemPropertyForm(RuleDefinitions.ItemPropertyUsage.Unlimited, 0,
                        new FeatureUnlockByLevel(attackModifierArcanePrecision, 0))
                    .Build()
            )
            .Build();

        var arcanePrecisionPower = FeatureDefinitionPowerBuilder
            .Create("PowerArcanePrecision", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("FeatArcanePrecision", Category.Feat,
                PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .Configure(2, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.LongRest, false, false,
                AttributeDefinitions.Intelligence, effectArcanePrecision, false /* unique instance */)
            .AddToDB();

        var arcanePrecision = FeatDefinitionBuilder
            .Create("FeatArcanePrecision", DefinitionBuilder.CENamespaceGuid)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                arcanePrecisionPower
            )
            .SetAbilityScorePrerequisite(AttributeDefinitions.Intelligence, 13)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Brutal Thug
        var brutalThug =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatBrutalThug", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AdditionalDamageRoguishHoodlumNonFinesseSneakAttack,
                    ProficiencyFighterWeapon
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(4), ValidateHasStealthAttack)
                .AddToDB();

        // Charismatic Defense
        var charismaticDefense = FeatDefinitionBuilder
            .Create("FeatCharismaticDefense", DefinitionBuilder.CENamespaceGuid)
            .SetFeatures(
                AttributeModifierCreed_Of_Solasta,
                FeatureDefinitionAttributeModifierBuilder
                    .Create(AttributeModifierBarbarianUnarmoredDefense,
                        "AttributeModifierFeatCharismaticDefenseAdd", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentationNoContent()
                    .SetCustomSubFeatures(ExclusiveArmorClassBonus.Marker)
                    .SetSituationalContext(RuleDefinitions.SituationalContext.NotWearingArmorOrMageArmor)
                    .SetModifierAbilityScore(AttributeDefinitions.Charisma)
                    .AddToDB()
            )
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Charismatic Precision
        var attackModifierCharismaticPrecision = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierCharismaticPrecision", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("FeatCharismaticPrecision", Category.Feat,
                FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
            .SetAbilityScoreReplacement(RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility)
            .SetAdditionalAttackTag(TagsDefinitions.Magical)
            .AddToDB();

        var effectCharismaticPrecision = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1 /* range */,
                RuleDefinitions.TargetType.Item, 1, 2, ActionDefinitions.ItemSelectionType.Weapon)
            .SetCreatedByCharacter()
            .SetDurationData(RuleDefinitions.DurationType.Minute, 1 /* duration */,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetItemPropertyForm(RuleDefinitions.ItemPropertyUsage.Unlimited, 0,
                        new FeatureUnlockByLevel(attackModifierCharismaticPrecision, 0))
                    .Build()
            )
            .Build();

        var charismaticPrecisionPower = FeatureDefinitionPowerBuilder
            .Create("PowerCharismaticPrecision", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("FeatCharismaticPrecision", Category.Feat,
                PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .Configure(2, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.LongRest, false, false,
                AttributeDefinitions.Charisma, effectCharismaticPrecision, false /* unique instance */)
            .AddToDB();

        var charismaticPrecision = FeatDefinitionBuilder
            .Create("FeatCharismaticPrecision", DefinitionBuilder.CENamespaceGuid)
            .SetFeatures(
                AttributeModifierCreed_Of_Solasta,
                charismaticPrecisionPower
            )
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Dual Weapon Defense
        var dualWeaponDefense =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatDualWeaponDefense", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierSwiftBladeBladeDance
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

        //
        //
        //

        static (bool, string) ValidateElvenAccuracy(FeatDefinitionWithPrerequisites _,
            [NotNull] RulesetCharacterHero hero)
        {
            var isElf = hero.RaceDefinition.Name.Contains(CharacterRaceDefinitions.Elf.Name);
            var elfTitle = CharacterRaceDefinitions.Elf.FormatTitle();
            var halfElfTitle = CharacterRaceDefinitions.HalfElf.FormatTitle();
            var param = $"{elfTitle}, {halfElfTitle}";

            return isElf
                ? (true, Gui.Format("Tooltip/&FeatPrerequisiteIs", param))
                : (false, Gui.Colorize(Gui.Format("Tooltip/&FeatPrerequisiteIs", param), "EA7171"));
        }

        // Elven Accuracy (Dexterity)
        var elvenAccuracyDexterity =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatElvenAccuracyDexterity", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(AttributeModifierCreed_Of_Misaye) // accuracy roll is handled by patches
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateElvenAccuracy)
                .SetCustomSubFeatures(new ElvenPrecisionContext())
                .AddToDB();

        // Elven Accuracy (Intelligence)
        var elvenAccuracyIntelligence =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatElvenAccuracyIntelligence", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(AttributeModifierCreed_Of_Pakri) // accuracy roll is handled by patches
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateElvenAccuracy)
                .SetCustomSubFeatures(new ElvenPrecisionContext())
                .AddToDB();

        // Elven Accuracy (Wisdom)
        var elvenAccuracyWisdom =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatElvenAccuracyWisdom", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(AttributeModifierCreed_Of_Maraike) // accuracy roll is handled by patches
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateElvenAccuracy)
                .SetCustomSubFeatures(new ElvenPrecisionContext())
                .AddToDB();

        // Elven Accuracy (Charisma)
        var elvenAccuracyCharisma =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatElvenAccuracyCharisma", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(AttributeModifierCreed_Of_Solasta) // accuracy roll is handled by patches
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateElvenAccuracy)
                .SetCustomSubFeatures(new ElvenPrecisionContext())
                .AddToDB();

        // Fast Hands
        var fastHands =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatFastHands", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    ActionAffinityRogueCunningAction,
                    ActionAffinityThiefFastHands
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(4), ValidateNotClass(Rogue))
                .AddToDB();

        // Fighting Surge (Dexterity)
        var fightingSurgeDexterity =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatFightingSurgeDexterity", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierCreed_Of_Misaye,
                    PowerFighterActionSurge
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateNotClass(Fighter))
                .AddToDB();

        // Fighting Surge (Strength)
        var fightingSurgeStrength =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatFightingSurgeStrength", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierCreed_Of_Einar,
                    PowerFighterActionSurge
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateNotClass(Fighter))
                .AddToDB();

        // Marksman
        var marksman =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMarksman", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    ActionAffinityMarksmanReactionShot
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

        // Metamagic Sorcery Points Feature
        var attributeModifierSorcererSorceryPointsAdd2 = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierSorcererSorceryPointsBase, "AttributeModifierSorcererSorceryPointsBonus2",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.SorceryPoints, 2)
            .AddToDB();

        // Metamagic Adept (Careful)
        var metamagicAdeptCareful =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptCareful", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnCareful,
                    attributeModifierSorcererSorceryPointsAdd2
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Distant)
        var metamagicAdeptDistant =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptDistant", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnDistant,
                    attributeModifierSorcererSorceryPointsAdd2
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Empowered)
        var metamagicAdeptEmpowered =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptEmpowered", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnEmpowered,
                    attributeModifierSorcererSorceryPointsAdd2
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Extended)
        var metamagicAdeptExtended =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptExtended", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnExtended,
                    attributeModifierSorcererSorceryPointsAdd2
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Heightened)
        var metamagicAdeptHeightened =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptHeightened", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnHeightened,
                    attributeModifierSorcererSorceryPointsAdd2,
                    attributeModifierSorcererSorceryPointsAdd2 // not a dup. adding 4 points
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Quickened)
        var metamagicAdeptQuickened =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptQuickened", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnQuickened,
                    attributeModifierSorcererSorceryPointsAdd2
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Metamagic Adept (Twinned)
        var metamagicAdeptTwinned =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptTwinned", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnTwinned,
                    attributeModifierSorcererSorceryPointsAdd2
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                //.SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

        // Primal (Constitution)
        var primalConstitution =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatPrimalConstitution", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierCreed_Of_Arun,
                    ActionAffinityBarbarianRage,
                    AttributeModifierBarbarianRagePointsAdd,
                    AttributeModifierBarbarianRageDamageAdd,
                    AttributeModifierBarbarianRageDamageAdd, // not a dup. I use add to allow compatibility with Barb class. 2 adds for +2 damage
                    PowerBarbarianRageStart,
                    AttributeModifierBarbarianUnarmoredDefense
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateNotClass(Barbarian))
                .AddToDB();

        // Primal (Strength)
        var primalStrength =
            FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatPrimalStrength", DefinitionBuilder.CENamespaceGuid)
                .SetFeatures(
                    AttributeModifierCreed_Of_Einar,
                    ActionAffinityBarbarianRage,
                    AttributeModifierBarbarianRagePointsAdd,
                    AttributeModifierBarbarianRageDamageAdd, // not a dup. I use add to allow compatibility with Barb class. 2 adds for +2 damage
                    AttributeModifierBarbarianRageDamageAdd,
                    PowerBarbarianRageStart,
                    AttributeModifierBarbarianUnarmoredDefense
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateNotClass(Barbarian))
                .AddToDB();

        // Shady
        var shady = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
            .Create("FeatShady", DefinitionBuilder.CENamespaceGuid)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                FeatureDefinitionAdditionalDamageBuilder
                    .Create(AdditionalDamageRogueSneakAttack, "AdditionalDamageFeatShadySneakAttack",
                        DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Feature)
                    .SetDamageDice(RuleDefinitions.DieType.D6, 1)
                    .SetAdvancement(
                        (RuleDefinitions.AdditionalDamageAdvancement)ExtraAdditionalDamageAdvancement.CharacterLevel,
                        (1, 0),
                        (2, 0),
                        (3, 0),
                        (4, 1),
                        (5, 1),
                        (6, 1),
                        (7, 1),
                        (8, 1),
                        (9, 1),
                        (10, 1),
                        (11, 1),
                        (12, 2),
                        (13, 2),
                        (14, 2),
                        (15, 2),
                        (16, 2),
                        (17, 2),
                        (18, 2),
                        (19, 2),
                        (20, 4)
                    )
                    .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.OncePerTurn)
                    .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
                    .SetRequiredProperty(RuleDefinitions.RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
                    .AddToDB()
            )
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .SetGuiPresentation(Category.Feat)
            .SetValidators(ValidateNotClass(Rogue))
            .AddToDB();

        // Wise Defense
        var wiseDefense = FeatDefinitionBuilder
            .Create("FeatWiseDefense", DefinitionBuilder.CENamespaceGuid)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike,
                FeatureDefinitionAttributeModifierBuilder
                    .Create(AttributeModifierBarbarianUnarmoredDefense, "AttributeModifierFeatWiseDefenseAdd",
                        DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentationNoContent()
                    .SetCustomSubFeatures(ExclusiveArmorClassBonus.Marker)
                    .SetSituationalContext(RuleDefinitions.SituationalContext.NotWearingArmorOrMageArmor)
                    .SetModifierAbilityScore(AttributeDefinitions.Wisdom)
                    .AddToDB()
            )
            .SetAbilityScorePrerequisite(AttributeDefinitions.Wisdom, 13)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        // Wise Precision
        var attackModifierWisePrecision = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierWisePrecision", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("FeatWisePrecision", Category.Feat,
                FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
            .SetAbilityScoreReplacement(RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility)
            .SetAdditionalAttackTag(TagsDefinitions.Magical)
            .AddToDB();

        var effectWisePrecision = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1 /* range */,
                RuleDefinitions.TargetType.Item, 1, 2, ActionDefinitions.ItemSelectionType.Weapon)
            .SetCreatedByCharacter()
            .SetDurationData(RuleDefinitions.DurationType.Minute, 1 /* duration */,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetItemPropertyForm(RuleDefinitions.ItemPropertyUsage.Unlimited, 0,
                        new FeatureUnlockByLevel(attackModifierWisePrecision, 0))
                    .Build()
            )
            .Build();

        var wisePrecisionPower = FeatureDefinitionPowerBuilder
            .Create("PowerWisePrecision", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("FeatWisePrecision", Category.Feat,
                PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .Configure(2, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Wisdom,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.LongRest, false, true,
                AttributeDefinitions.Intelligence, effectWisePrecision, false /* unique instance */)
            .AddToDB();

        var wisePrecision = FeatDefinitionBuilder
            .Create("FeatWisePrecision", DefinitionBuilder.CENamespaceGuid)
            .SetFeatures(
                wisePrecisionPower
            )
            .SetAbilityScorePrerequisite(AttributeDefinitions.Wisdom, 13)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();

        //
        // set feats to be registered in mod settings
        //

        feats.AddRange(
            arcaneDefense,
            arcanePrecision,
            brutalThug,
            charismaticDefense,
            charismaticPrecision,
            dualWeaponDefense,
            elvenAccuracyDexterity,
            elvenAccuracyIntelligence,
            elvenAccuracyWisdom,
            elvenAccuracyCharisma,
            fastHands,
            fightingSurgeDexterity,
            fightingSurgeStrength,
            marksman,
            metamagicAdeptCareful,
            metamagicAdeptDistant,
            metamagicAdeptEmpowered,
            metamagicAdeptExtended,
            metamagicAdeptHeightened,
            metamagicAdeptQuickened,
            metamagicAdeptTwinned,
            primalConstitution,
            primalStrength,
            shady,
            wiseDefense,
            wisePrecision,
            FeatDeadeyeBuilder.FeatDeadeye);
    }
}

internal sealed class FeatureDefinitionMetamagicOptionBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionMetamagicOption, FeatureDefinitionMetamagicOptionBuilder>
{
    private const string MetamagicLearnCarefulName = "MetamagicLearnCareful";
    private const string MetamagicLearnCarefulGuid = "820a900b-a5f6-47d7-8860-b0d0605722b0";

    private const string MetamagicLearnDistantName = "MetamagicLearnDistant";
    private const string MetamagicLearnDistantGuid = "cb137252-d16e-4a3d-9f37-d9b5e1922424";

    private const string MetamagicLearnEmpoweredName = "MetamagicLearnEmpowered";
    private const string MetamagicLearnEmpoweredGuid = "d16671f9-af84-4f6a-84c4-1bda29a73dbe";

    private const string MetamagicLearnExtendedName = "MetamagicLearnExtended";
    private const string MetamagicLearnExtendedGuid = "944b8533-3821-496d-a200-ae5e5a0a82a9";

    private const string MetamagicLearnHeightenedName = "MetamagicLearnHeightened";
    private const string MetamagicLearnHeightenedGuid = "8a74dca9-b0a7-4519-aa84-d682a0272e7c";

    private const string MetamagicLearnQuickenedName = "MetamagicLearnQuickened";
    private const string MetamagicLearnQuickenedGuid = "f1f2a8b9-e290-4ba9-9118-83c2ca19622a";

    private const string MetamagicLearnTwinnedName = "MetamagicLearnTwinned";
    private const string MetamagicLearnTwinnedGuid = "84572060-3187-41f7-abad-30ad4a217511";

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnCareful =
        CreateAndAddToDB(MetamagicLearnCarefulName, MetamagicLearnCarefulGuid, MetamagicCarefullSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnDistant =
        CreateAndAddToDB(MetamagicLearnDistantName, MetamagicLearnDistantGuid, MetamagicDistantSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnEmpowered =
        CreateAndAddToDB(MetamagicLearnEmpoweredName, MetamagicLearnEmpoweredGuid, MetamagicEmpoweredSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnExtended =
        CreateAndAddToDB(MetamagicLearnExtendedName, MetamagicLearnExtendedGuid, MetamagicExtendedSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnHeightened =
        CreateAndAddToDB(MetamagicLearnHeightenedName, MetamagicLearnHeightenedGuid, MetamagicHeightenedSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnQuickened =
        CreateAndAddToDB(MetamagicLearnQuickenedName, MetamagicLearnQuickenedGuid, MetamagicQuickenedSpell);

    internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnTwinned =
        CreateAndAddToDB(MetamagicLearnTwinnedName, MetamagicLearnTwinnedGuid, MetamagicTwinnedSpell);

    private FeatureDefinitionMetamagicOptionBuilder(string name, string guid,
        MetamagicOptionDefinition metamagicOption) : base(name, guid)
    {
        Definition.MetamagicOption = metamagicOption;
    }

    private static FeatureDefinitionMetamagicOption CreateAndAddToDB(string name, string guid,
        MetamagicOptionDefinition metamagicOption)
    {
        return new FeatureDefinitionMetamagicOptionBuilder(name, guid, metamagicOption)
            .SetGuiPresentationNoContent()
            .AddToDB();
    }
}

internal sealed class FeatureDefinitionMetamagicOption : FeatureDefinition, IFeatureDefinitionCustomCode
{
    private bool MetamagicTrained { get; set; }

    public MetamagicOptionDefinition MetamagicOption { get; set; }

    public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
    {
        if (hero.MetamagicFeatures.ContainsKey(MetamagicOption))
        {
            return;
        }

        hero.TrainMetaMagicOptions(new List<MetamagicOptionDefinition> { MetamagicOption });

        MetamagicTrained = true;
    }

    public void RemoveFeature(RulesetCharacterHero hero, string tag)
    {
        if (!MetamagicTrained)
        {
            return;
        }

        hero.MetamagicFeatures.Remove(MetamagicOption);

        MetamagicTrained = false;
    }
}

internal sealed class ElvenPrecisionContext
{
    public bool Qualified { get; set; } = false;
}

internal sealed class DeadeyeIgnoreDefenderBuilder : FeatureDefinitionCombatAffinityBuilder
{
    private const string DeadeyeIgnoreDefenderName = "DeadeyeIgnoreDefender";
    private const string DeadeyeIgnoreDefenderGuid = "38940e1f-fc62-4a1a-aebe-b4cb7064050d";

    public static readonly FeatureDefinition DeadeyeIgnoreDefender
        = CreateAndAddToDB(DeadeyeIgnoreDefenderName, DeadeyeIgnoreDefenderGuid);

    private DeadeyeIgnoreDefenderBuilder(string name, string guid) : base(name, guid)
    {
        Definition.GuiPresentation.Title = "Feature/&DeadeyeTitle";
        Definition.GuiPresentation.Description = "Feature/&DeadeyeDescription";

        Definition.ignoreCover = true;
        Definition.SetCustomSubFeatures(new BumpWeaponAttackRangeToMax(WeaponValidators.AlwaysValid));
    }

    private static FeatureDefinition CreateAndAddToDB(string name, string guid)
    {
        return new DeadeyeIgnoreDefenderBuilder(name, guid).AddToDB();
    }
}

internal sealed class DeadeyeAttackModifierBuilder : FeatureDefinitionBuilder
{
    private const string DeadeyeAttackModifierName = "DeadeyeAttackModifier";
    private const string DeadeyeAttackModifierGuid = "473f6ab6-af46-4717-b55e-ff9e31d909e2";

    public static readonly FeatureDefinition DeadeyeAttackModifier
        = CreateAndAddToDB(DeadeyeAttackModifierName, DeadeyeAttackModifierGuid);

    private DeadeyeAttackModifierBuilder(string name, string guid) : base(name, guid)
    {
        Definition.GuiPresentation.Title = "Feature/&DeadeyeTitle";
        Definition.GuiPresentation.Description = "Feature/&DeadeyeDescription";

        Definition.SetCustomSubFeatures(new ModifyDeadeyeAttackPower());
    }

    private static FeatureDefinition CreateAndAddToDB(string name, string guid)
    {
        return new DeadeyeAttackModifierBuilder(name, guid).AddToDB();
    }
}

internal sealed class DeadeyeConditionBuilder : ConditionDefinitionBuilder
{
    private const string DeadeyeConditionName = "DeadeyeCondition";
    private const string DeadeyeConditionNameGuid = "a0d24e21-3469-43af-ad63-729552120314";

    public static readonly ConditionDefinition DeadeyeCondition =
        CreateAndAddToDB(DeadeyeConditionName, DeadeyeConditionNameGuid);

    private DeadeyeConditionBuilder(string name, string guid) : base(
        ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
    {
        Definition.GuiPresentation.Title = "Feature/&DeadeyeTitle";
        Definition.GuiPresentation.Description = "Feature/&DeadeyeDescription";

        Definition.allowMultipleInstances = false;
        Definition.Features.Clear();
        Definition.Features.Add(DeadeyeAttackModifierBuilder.DeadeyeAttackModifier);

        Definition.durationType = RuleDefinitions.DurationType.Round;
        Definition.durationParameter = 0;
        Definition.CancellingConditions.Clear();
        Definition.CancellingConditions.Add(Definition);
    }

    private static ConditionDefinition CreateAndAddToDB(string name, string guid)
    {
        return new DeadeyeConditionBuilder(name, guid).AddToDB();
    }
}

internal sealed class FeatDeadeyeBuilder : FeatDefinitionBuilder
{
    private const string FeatDeadeyeName = "FeatDeadeye";
    private const string FeatDeadeyeNameGuid = "d2ca939a-465e-4e43-8e9b-6469177e1839";

    internal static readonly FeatDefinition FeatDeadeye =
        CreateAndAddToDB(FeatDeadeyeName, FeatDeadeyeNameGuid);

    private FeatDeadeyeBuilder(string name, string guid) : base(
        FeatDefinitions.FollowUpStrike, name, guid)
    {
        var concentrationProvider = new EwFeats.StopPowerConcentrationProvider("Deadeye",
            "Tooltip/&DeadeyeConcentration",
            CustomIcons.CreateAssetReferenceSprite("DeadeyeConcentrationIcon",
                Resources.DeadeyeConcentrationIcon, 64, 64));

        var triggerCondition = ConditionDefinitionBuilder
            .Create("DeadeyeTriggerCondition", CENamespaceGuid)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(RuleDefinitions.DurationType.Permanent)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("DeadeyeTriggerFeature", CENamespaceGuid)
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(concentrationProvider)
                    .AddToDB())
            .AddToDB();

        var turnOnPower = FeatureDefinitionPowerBuilder
            .Create("Deadeye", "aa2cc094-0bf9-4e72-ac2c-50e99e680ca1")
            .SetGuiPresentation("FeatDeadeye", Category.Feat,
                CustomIcons.CreateAssetReferenceSprite("DeadeyeIcon",
                    Resources.DeadeyeIcon, 128, 64))
            .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
            .SetUsesFixed(1)
            .SetCostPerUse(0)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                    RuleDefinitions.TargetType.Self)
                .SetDurationData(RuleDefinitions.DurationType.Permanent)
                .SetEffectForms(
                    new EffectFormBuilder()
                        .SetConditionForm(triggerCondition, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    new EffectFormBuilder()
                        .SetConditionForm(DeadeyeConditionBuilder.DeadeyeCondition,
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        PowersContext.PowersThatIgnoreInterruptions.Add(turnOnPower);

        var turnOffPower = FeatureDefinitionPowerBuilder
            .Create("TurnOffDeadeyePower", CENamespaceGuid)
            .SetGuiPresentationNoContent(true)
            .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
            .SetUsesFixed(1)
            .SetCostPerUse(0)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                    RuleDefinitions.TargetType.Self)
                .SetDurationData(RuleDefinitions.DurationType.Round, 0, false)
                .SetEffectForms(
                    new EffectFormBuilder()
                        .SetConditionForm(triggerCondition, ConditionForm.ConditionOperation.Remove)
                        .Build(),
                    new EffectFormBuilder()
                        .SetConditionForm(DeadeyeConditionBuilder.DeadeyeCondition,
                            ConditionForm.ConditionOperation.Remove)
                        .Build())
                .Build())
            .AddToDB();

        PowersContext.PowersThatIgnoreInterruptions.Add(turnOffPower);
        concentrationProvider.StopPower = turnOffPower;

        Definition.GuiPresentation.Title = "Feat/&FeatDeadeyeTitle";
        Definition.GuiPresentation.Description = "Feat/&FeatDeadeyeDescription";

        Definition.Features.Clear();
        Definition.Features.Add(turnOnPower);
        Definition.Features.Add(turnOffPower);
        Definition.Features.Add(DeadeyeIgnoreDefenderBuilder.DeadeyeIgnoreDefender);
        Definition.minimalAbilityScorePrerequisite = false;
    }

    private static FeatDefinition CreateAndAddToDB(string name, string guid)
    {
        return new FeatDeadeyeBuilder(name, guid).AddToDB();
    }
}

internal sealed class ModifyDeadeyeAttackPower : IModifyAttackModeForWeapon
{
    public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode,
        RulesetItem weapon)
    {
        var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

        if (damage == null)
        {
            return;
        }

        if (attackMode is not { Reach: false, Ranged: true })
        {
            return;
        }

        const int TO_HIT = -5;
        const int TO_DAMAGE = 10;

        attackMode.ToHitBonus += TO_HIT;
        attackMode.ToHitBonusTrends.Add(new RuleDefinitions.TrendInfo(TO_HIT,
            RuleDefinitions.FeatureSourceType.Power, "Deadeye", null));

        damage.BonusDamage += TO_DAMAGE;
        damage.DamageBonusTrends.Add(new RuleDefinitions.TrendInfo(TO_DAMAGE,
            RuleDefinitions.FeatureSourceType.Power, "Deadeye", null));
    }
}
