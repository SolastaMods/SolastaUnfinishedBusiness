using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Infrastructure;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions.RollContext;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Feats
{
    internal static class OtherFeats
    {
        public static readonly Guid OtherFeatNamespace = new("655e8588-4d6e-42f3-9564-69e7345d5620");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            // Savage Attacker
            var savageAttacker = FeatDefinitionBuilder
                .Create("FeatSavageAttacker", OtherFeatNamespace)
                .SetFeatures(
                    BuildDieRollModifier("DieRollModifierFeatSavageAttacker",
                        AttackDamageValueRoll, 1 /* reroll count */, 1 /* reroll min value */ ),
                    BuildDieRollModifier("DieRollModifierFeatSavageMagicAttacker",
                        MagicDamageValueRoll, 1 /* reroll count */, 1 /* reroll min value */ ))
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Improved critical
            var improvedCritical = FeatDefinitionBuilder
                .Create("FeatImprovedCritical", OtherFeatNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    FeatureDefinitionAttributeModifierBuilder
                        .Create("AttributeModifierImprovedCriticalFeat", OtherFeatNamespace)
                        .SetGuiPresentation("FeatImprovedCritical", Category.Feat)
                        .SetModifier(AttributeModifierOperation.Set, AttributeDefinitions.CriticalThreshold, 19)
                        .AddToDB())
                .AddToDB();

            // Tough
            var tough = FeatDefinitionBuilder
                .Create("FeatTough", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionAttributeModifierBuilder
                        .Create("AttributeModifierToughFeat", OtherFeatNamespace)
                        .SetGuiPresentation("FeatTough", Category.Feat)
                        .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 2)
                        .AddToDB())
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // War Caster
            var warCaster = FeatDefinitionBuilder
                .Create("FeatWarCaster", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionMagicAffinityBuilder
                        .Create("MagicAffinityWarCasterFeat", OtherFeatNamespace)
                        .SetGuiPresentation("FeatWarCaster", Category.Feat)
                        .SetCastingModifiers(2, 0, true, false, false)
                        .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
                        .SetHandsFullCastingModifiers(true, true, true)
                        .AddToDB())
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            //
            // Zappa Feats
            //

            // Fighting Surge (Dexterity)
            var fightingSurgeDexterity = FeatDefinitionBuilder
                .Create("FeatFightingSurgeDexterity", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Misaye,
                    PowerFighterActionSurge
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Fighting Surge (Strength)
            var fightingSurgeStrength = FeatDefinitionBuilder
                .Create("FeatFightingSurgeStrength", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Einar,
                    PowerFighterActionSurge
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            //
            // WIP: metamagic selection happens before feat. need to find a patch first
            //
#if false

//Feat/&FeatMetamagicAdeptCharismaTitle	Metamagic Adept (Charisma)
//Feat/&FeatMetamagicAdeptCharismaDescription	You've learned how to exert your will on your spells to alter how they function. You gain the following benefits:\n\nIncrease your Charisma score by 1, to a maximum of 20.\n\nLearn 2 Metamagic options of your choice from the Sorcerer Class.\n\nGain 2 Sorcerer Points. These can stack with points from other sources.
//Feat/&FeatMetamagicAdeptConstitutionTitle	Metamagic Adept (Constitution)
//Feat/&FeatMetamagicAdeptConstitutionDescription	You've learned how to exert your will on your spells to alter how they function. You gain the following benefits:\n\nIncrease your Charisma score by 1, to a maximum of 20.\n\nLearn 2 Metamagic options of your choice from the Sorcerer Class.\n\nGain 2 Sorcerer Points. These can stack with points from other sources.

            // Metamagic Adept (Charisma)
            var metamagicAdeptCharisma = FeatDefinitionBuilder
                .Create("FeatMetamagicAdeptCharisma", OtherFeatNamespace)
                .SetFeatures(
                     AttributeModifierCreed_Of_Solasta,
                    // TODO: FeatureDefinitionAttributeModifierBuilder not working. Need to create a new based on this and change set to add                   
                    AttributeModifierSorcererSorceryPointsBase,
                    PointPoolSorcererMetamagic
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Metamagic Adept (Constitution)
            var metamagicAdeptConstitution = FeatDefinitionBuilder
                .Create("FeatMetamagicAdeptConstitution", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Arun,
                    // TODO: FeatureDefinitionAttributeModifierBuilder not working. Need to create a new based on this and change set to add                   
                    AttributeModifierSorcererSorceryPointsBase,
                    PointPoolSorcererMetamagic
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();
#endif

            // Practiced Expert Features
            var pointPoolFeatPracticedExpertSkill = FeatureDefinitionPointPoolBuilder
               .Create("PointPoolFeatPracticedExpertSkill")
               .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
               .AddToDB();

            var pointPoolFeatPracticedExpertExpertise = FeatureDefinitionPointPoolBuilder
                .Create("PointPoolFeatPracticedExpertExpertise")
                .SetPool(HeroDefinitions.PointsPoolType.Expertise, 1)
                .AddToDB();

            // Practiced Expert (Ingelligence)
            var practicedExpertIntelligence = FeatDefinitionBuilder
                .Create("FeatPracticedExpertIntelligence", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Pakri,
                    pointPoolFeatPracticedExpertSkill,
                    pointPoolFeatPracticedExpertExpertise
                )
                .SetGuiPresentation(Category.Feat)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Intelligence, 13)
                .AddToDB();

            // Practiced Expert (Wisdom)
            var practicedExpertWisdom = FeatDefinitionBuilder
                .Create("FeatPracticedExpertWisdom", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Maraike,
                    pointPoolFeatPracticedExpertSkill,
                    pointPoolFeatPracticedExpertExpertise
                )
                .SetGuiPresentation(Category.Feat)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Wisdom, 13)
                .AddToDB();

            // Primal (Constitution)
            var primalConstitution = FeatDefinitionBuilder
                .Create("FeatPrimalConstitution", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Arun,
                    ActionAffinityBarbarianRage,
                    AttributeModifierBarbarianRagePointsAdd,
                    AttributeModifierBarbarianRageDamageAdd,
                    PowerBarbarianRageStart
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Primal (Strength)
            var primalStrength = FeatDefinitionBuilder
                .Create("FeatPrimalStrength", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Einar,
                    ActionAffinityBarbarianRage,
                    AttributeModifierBarbarianRagePointsAdd,
                    AttributeModifierBarbarianRageDamageAdd,
                    PowerBarbarianRageStart
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Shady
            var shady = FeatDefinitionBuilder
                .Create("FeatShady", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Misaye,
                     FeatureDefinitionAdditionalDamageBuilder
                        .Create(AdditionalDamageRogueSneakAttack, "AdditionalDamageFeatShadySneakAttack", OtherFeatNamespace)
                        .SetGuiPresentation("AdditionalDamageFeatShadySneakAttack", Category.Feature)
                        .SetDiceByRank((1,1), (5,2))
                        .AddToDB(),
                     FeatureDefinitionPointPoolBuilder
                        .Create("PointPoolFeatShadyExpertise")
                        .SetPool(HeroDefinitions.PointsPoolType.Expertise, 1)
                        .AddToDB()
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Unarmored Defense (Constitution)
            var unarmoredDefenseConstitution = FeatDefinitionBuilder
                .Create("FeatUnarmoredDefenseConstitution", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Arun,
                    AttributeModifierBarbarianUnarmoredDefense
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Unarmmored Defense (Charisma)

            //
            // TODO: fix below after Imp checks why AttrModBuilder isn't working
            //
            var attributeModifierCharismaUnarmoredDefense = UnityEngine.Object.Instantiate(AttributeModifierBarbarianUnarmoredDefense);

            attributeModifierCharismaUnarmoredDefense.SetField("name", "AttributeModifierCharismaUnarmoredDefense");
            attributeModifierCharismaUnarmoredDefense.SetField("modifierAbilityScore", AttributeDefinitions.Charisma);

            var unarmoredDefenseCharisma = FeatDefinitionBuilder
                .Create("FeatUnarmoredDefenseCharisma", OtherFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Einar,
                    attributeModifierCharismaUnarmoredDefense
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            //
            // set feats to be registered in mod settings
            //

            feats.AddRange(
                savageAttacker, 
                tough, 
                warCaster,

                improvedCritical, 

                fightingSurgeDexterity, 
                fightingSurgeStrength,
                unarmoredDefenseCharisma,
                unarmoredDefenseConstitution,
                practicedExpertIntelligence,
                practicedExpertWisdom,
                primalConstitution,
                primalStrength,
                shady);
        }

        private static FeatureDefinitionDieRollModifier BuildDieRollModifier(string name,
            RuleDefinitions.RollContext context, int rerollCount, int minRerollValue)
        {
            return FeatureDefinitionDieRollModifierBuilder
                .Create(name, OtherFeatNamespace)
                .SetModifiers(context, rerollCount, minRerollValue, "Feat/&FeatSavageAttackerReroll")
                .SetGuiPresentation("FeatSavageAttacker", Category.Feat)
                .AddToDB();
        }
    }
}
