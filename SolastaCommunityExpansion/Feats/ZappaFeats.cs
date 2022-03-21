using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.MetamagicOptionDefinitions;

namespace SolastaCommunityExpansion.Feats
{
    internal static class ZappaFeats
    {
        public static readonly Guid ZappaFeatNamespace = new("514f14e3-db8e-47b3-950a-350e8cae37d6");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            // Charismatic Defense
            var charismaticDefense = FeatDefinitionBuilder
                .Create("FeatCharismaticDefense", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    //FeatureDefinitionAttributeModifierBuilder
                    //    .Create(AttributeModifierMageArmor, "AttributeModifierFeatCharismaticDefenseSet", ZappaFeatNamespace)
                    //    .SetGuiPresentationNoContent()
                    //    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Set, AttributeDefinitions.ArmorClass, 12)
                    //    .AddToDB(),
                    FeatureDefinitionAttributeModifierBuilder
                        .Create(AttributeModifierBarbarianUnarmoredDefense, "AttributeModifierFeatCharismaticDefenseAdd", ZappaFeatNamespace)
                        .SetGuiPresentationNoContent()
                        .SetModifierAbilityScore(AttributeDefinitions.Charisma)
                        .AddToDB()
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Fighting Surge (Dexterity)
            var fightingSurgeDexterity = FeatDefinitionBuilder
                .Create("FeatFightingSurgeDexterity", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Misaye,
                    PowerFighterActionSurge
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Fighting Surge (Strength)
            var fightingSurgeStrength = FeatDefinitionBuilder
                .Create("FeatFightingSurgeStrength", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Einar,
                    PowerFighterActionSurge
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Metamagic Adept (Careful)
            var metamagicAdeptCareful = FeatDefinitionBuilder
                .Create("FeatMetamagicAdeptCareful", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnCareful
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Metamagic Adept (Distant)
            var metamagicAdeptDistant = FeatDefinitionBuilder
                .Create("FeatMetamagicAdeptDistant", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnDistant
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Metamagic Adept (Empowered)
            var metamagicAdeptEmpowered = FeatDefinitionBuilder
                .Create("FeatMetamagicAdeptEmpowered", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnEmpowered
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Metamagic Adept (Extended)
            var metamagicAdeptExtended = FeatDefinitionBuilder
                .Create("FeatMetamagicAdeptExtended", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnExtended
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Metamagic Adept (Quickened)
            var metamagicAdeptQuickened = FeatDefinitionBuilder
                .Create("FeatMetamagicAdeptQuickened", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnQuickened
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Metamagic Adept (Twinned)
            var metamagicAdeptTwinned = FeatDefinitionBuilder
                .Create("FeatMetamagicAdeptTwinned", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnTwinned
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Primal (Constitution)
            var primalConstitution = FeatDefinitionBuilder
                .Create("FeatPrimalConstitution", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Arun,
                    ActionAffinityBarbarianRage,
                    AttributeModifierBarbarianRagePointsAdd,
                    AttributeModifierBarbarianRageDamageAdd, // not a dup. I use add to allow compatibility with Barb class. 2 adds for +2 damage
                    AttributeModifierBarbarianRageDamageAdd,
                    PowerBarbarianRageStart,
                    AttributeModifierBarbarianUnarmoredDefense
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Primal Rage (Strength)
            var primalStrength = FeatDefinitionBuilder
                .Create("FeatPrimalStrength", ZappaFeatNamespace)
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
                .AddToDB();

            // Shady
            var shady = FeatDefinitionBuilder
                .Create("FeatShady", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Misaye,
                    FeatureDefinitionAdditionalDamageBuilder
                        .Create(AdditionalDamageRogueSneakAttack, "AdditionalDamageFeatShadySneakAttack", ZappaFeatNamespace)
                        .SetGuiPresentation("AdditionalDamageFeatShadySneakAttack", Category.Feat)
                        .SetDamageDice(RuleDefinitions.DieType.D6, 1)
                        .SetAdvancement(RuleDefinitions.AdditionalDamageAdvancement.ClassLevel,
                            (1, 1),
                            (2, 1),
                            (3, 1),
                            (4, 1),
                            (5, 1),
                            (6, 1),
                            (7, 2),
                            (8, 2),
                            (9, 2),
                            (10, 2),
                            (11, 2),
                            (12, 2),
                            (13, 3),
                            (14, 3),
                            (15, 3),
                            (16, 3),
                            (17, 3),
                            (18, 3),
                            (19, 4),
                            (20, 4)
                        )
                        .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.OncePerTurn)
                        .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
                        .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.FinesseOrRangeWeapon)
                        .AddToDB()
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Wise Defense
            var wiseDefense = FeatDefinitionBuilder
                .Create("FeatWiseDefense", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    //FeatureDefinitionAttributeModifierBuilder
                    //    .Create(AttributeModifierMageArmor, "AttributeModifierFeatWiseDefenseSet", ZappaFeatNamespace)
                    //    .SetGuiPresentationNoContent()
                    //    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Set, AttributeDefinitions.ArmorClass, 12)
                    //    .AddToDB(),
                    FeatureDefinitionAttributeModifierBuilder
                        .Create(AttributeModifierBarbarianUnarmoredDefense, "AttributeModifierFeatWiseDefenseAdd", ZappaFeatNamespace)
                        .SetGuiPresentationNoContent()
                        .SetModifierAbilityScore(AttributeDefinitions.Wisdom)
                        .AddToDB()
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Wisdom, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            //
            // set feats to be registered in mod settings
            //

            feats.AddRange(
                charismaticDefense,
                fightingSurgeDexterity,
                fightingSurgeStrength,
                metamagicAdeptCareful,
                metamagicAdeptDistant,
                metamagicAdeptEmpowered,
                metamagicAdeptExtended,
                metamagicAdeptQuickened,
                metamagicAdeptTwinned,
                primalConstitution,
                primalStrength,
                shady,
                wiseDefense);
        }
    }

    internal sealed class FeatureDefinitionMetamagicOptionBuilder : FeatureDefinitionCustomCodeBuilder<FeatureDefinitionMetamagicOption, FeatureDefinitionMetamagicOptionBuilder>
    {
        private const string MetamagicLearnCarefulName = "MetamagicLearnCareful";
        private const string MetamagicLearnCarefulGuid = "820a900b-a5f6-47d7-8860-b0d0605722b0";

        private const string MetamagicLearnDistantName = "MetamagicLearnDistant";
        private const string MetamagicLearnDistantGuid = "cb137252-d16e-4a3d-9f37-d9b5e1922424";

        private const string MetamagicLearnEmpoweredName = "MetamagicLearnEmpowered";
        private const string MetamagicLearnEmpoweredGuid = "d16671f9-af84-4f6a-84c4-1bda29a73dbe";

        private const string MetamagicLearnExtendedName = "MetamagicLearnExtended";
        private const string MetamagicLearnExtendedGuid = "944b8533-3821-496d-a200-ae5e5a0a82a9";

        private const string MetamagicLearnQuickenedName = "MetamagicLearnQuickened";
        private const string MetamagicLearnQuickenedGuid = "f1f2a8b9-e290-4ba9-9118-83c2ca19622a";

        private const string MetamagicLearnTwinnedName = "MetamagicLearnTwinned";
        private const string MetamagicLearnTwinnedGuid = "84572060-3187-41f7-abad-30ad4a217511";

        private FeatureDefinitionMetamagicOptionBuilder(string name, string guid, MetamagicOptionDefinition metamagicOption) : base(name, guid)
        {
            Definition.MetamagicOption = metamagicOption;
        }

        private static FeatureDefinitionMetamagicOption CreateAndAddToDB(string name, string guid, MetamagicOptionDefinition metamagicOption)
        {
            return new FeatureDefinitionMetamagicOptionBuilder(name, guid, metamagicOption)
                .SetGuiPresentationNoContent()
                .AddToDB();
        }

        internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnCareful =
            CreateAndAddToDB(MetamagicLearnCarefulName, MetamagicLearnCarefulGuid, MetamagicCarefullSpell);

        internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnDistant =
            CreateAndAddToDB(MetamagicLearnDistantName, MetamagicLearnDistantGuid, MetamagicDistantSpell);

        internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnEmpowered =
            CreateAndAddToDB(MetamagicLearnEmpoweredName, MetamagicLearnEmpoweredGuid, MetamagicEmpoweredSpell);

        internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnExtended =
            CreateAndAddToDB(MetamagicLearnExtendedName, MetamagicLearnExtendedGuid, MetamagicExtendedSpell);

        internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnQuickened =
            CreateAndAddToDB(MetamagicLearnQuickenedName, MetamagicLearnQuickenedGuid, MetamagicQuickenedSpell);

        internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnTwinned =
            CreateAndAddToDB(MetamagicLearnTwinnedName, MetamagicLearnTwinnedGuid, MetamagicTwinnedSpell);
    }

    internal sealed class FeatureDefinitionMetamagicOption : FeatureDefinitionCustomCode
    {
        private bool MetamagicTrained { get; set; }

        public MetamagicOptionDefinition MetamagicOption { get; set; }

        public override void ApplyFeature(RulesetCharacterHero hero)
        {
            if (!hero.MetamagicFeatures.ContainsKey(MetamagicOption))
            {
                hero.TrainMetaMagicOptions(new List<MetamagicOptionDefinition>() { MetamagicOption });

                MetamagicTrained = true;
            }

            if (!hero.ClassesAndLevels.ContainsKey(Sorcerer))
            {
                hero.GetAttribute(AttributeDefinitions.SorceryPoints).BaseValue = 2;
            }

            hero.RefreshAll();
        }

        public override void RemoveFeature(RulesetCharacterHero hero)
        {
            if (MetamagicTrained)
            {
                hero.MetamagicFeatures.Remove(MetamagicOption);
                hero.RefreshAll();

                MetamagicTrained = false;
            }

            if (!hero.ClassesAndLevels.ContainsKey(Sorcerer))
            {
                hero.GetAttribute(AttributeDefinitions.SorceryPoints).BaseValue = 0;
            }
        }
    }
}
