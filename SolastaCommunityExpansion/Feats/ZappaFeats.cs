using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaCommunityExpansion.Feats.FeatsValidations;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaModApi.DatabaseHelper.MetamagicOptionDefinitions;

namespace SolastaCommunityExpansion.Feats
{
    internal static class ZappaFeats
    {
        private static readonly Guid ZappaFeatNamespace = new("514f14e3-db8e-47b3-950a-350e8cae37d6");

        internal static void CreateFeats(List<FeatDefinition> feats)
        {
            // Arcane Defense
            var arcaneDefense = FeatDefinitionBuilder
                .Create("FeatArcaneDefense", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Pakri,
                    FeatureDefinitionAttributeModifierBuilder
                        .Create(AttributeModifierBarbarianUnarmoredDefense, "AttributeModifierFeatArcaneDefenseAdd", ZappaFeatNamespace)
                        .SetGuiPresentationNoContent()
                        .SetSituationalContext(RuleDefinitions.SituationalContext.NotWearingArmorOrMageArmor)
                        .SetModifierAbilityScore(AttributeDefinitions.Intelligence)
                        .AddToDB()
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Intelligence, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Arcane Precision
            var attackModifierArcanePrecision = FeatureDefinitionAttackModifierBuilder
                .Create("FeatureAttackModifierArcanePrecision", ZappaFeatNamespace)
                .SetGuiPresentation("FeatArcanePrecision", Category.Feat, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
                .SetAbilityScoreReplacement(RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility)
                .SetAdditionalAttackTag(TagsDefinitions.Magical)
                .AddToDB();

            var effectArcanePrecision = EffectDescriptionBuilder
                .Create()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1 /* range */, RuleDefinitions.TargetType.Item, 1, 2, ActionDefinitions.ItemSelectionType.Weapon)
                .SetCreatedByCharacter()
                .SetDurationData(RuleDefinitions.DurationType.Minute, 1 /* duration */, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetItemPropertyForm(RuleDefinitions.ItemPropertyUsage.Unlimited, 0, new FeatureUnlockByLevel(attackModifierArcanePrecision, 0))
                        .Build()
                    )
                .Build();

            var arcanePrecisionPower = FeatureDefinitionPowerBuilder
                .Create("PowerArcanePrecision", ZappaFeatNamespace)
                .SetGuiPresentation("FeatArcanePrecision", Category.Feat, FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
                .Configure(2, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.LongRest, false, false,
                    AttributeDefinitions.Intelligence, effectArcanePrecision, false /* unique instance */)
                .AddToDB();

            var arcanePrecision = FeatDefinitionBuilder
                .Create("FeatArcanePrecision", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Pakri,
                    arcanePrecisionPower
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Intelligence, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Brutal Thug
            var brutalThug = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatBrutalThug", ZappaFeatNamespace)
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
                .Create("FeatCharismaticDefense", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionAttributeModifierBuilder
                        .Create(AttributeModifierBarbarianUnarmoredDefense, "AttributeModifierFeatCharismaticDefenseAdd", ZappaFeatNamespace)
                        .SetGuiPresentationNoContent()
                        .SetSituationalContext(RuleDefinitions.SituationalContext.NotWearingArmorOrMageArmor)
                        .SetModifierAbilityScore(AttributeDefinitions.Charisma)
                        .AddToDB()
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Charismatic Precision
            var attackModifierCharismaticPrecision = FeatureDefinitionAttackModifierBuilder
                .Create("FeatureAttackModifierCharismaticPrecision", ZappaFeatNamespace)
                .SetGuiPresentation("FeatCharismaticPrecision", Category.Feat, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
                .SetAbilityScoreReplacement(RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility)
                .SetAdditionalAttackTag(TagsDefinitions.Magical)
                .AddToDB();

            var effectCharismaticPrecision = EffectDescriptionBuilder
                .Create()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1 /* range */, RuleDefinitions.TargetType.Item, 1, 2, ActionDefinitions.ItemSelectionType.Weapon)
                .SetCreatedByCharacter()
                .SetDurationData(RuleDefinitions.DurationType.Minute, 1 /* duration */, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetItemPropertyForm(RuleDefinitions.ItemPropertyUsage.Unlimited, 0, new FeatureUnlockByLevel(attackModifierCharismaticPrecision, 0))
                        .Build()
                    )
                .Build();

            var charismaticPrecisionPower = FeatureDefinitionPowerBuilder
                .Create("PowerCharismaticPrecision", ZappaFeatNamespace)
                .SetGuiPresentation("FeatCharismaticPrecision", Category.Feat, FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
                .Configure(2, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.LongRest, false, false,
                    AttributeDefinitions.Charisma, effectCharismaticPrecision, false /* unique instance */)
                .AddToDB();

            var charismaticPrecision = FeatDefinitionBuilder
                .Create("FeatCharismaticPrecision", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    charismaticPrecisionPower
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Dual Weapon Defense
            var dualWeaponDefense = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatDualWeaponDefense", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierSwiftBladeBladeDance
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Fast Hands
            var fastHands = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatFastHands", ZappaFeatNamespace)
                .SetFeatures(
                    ActionAffinityRogueCunningAction,
                    ActionAffinityThiefFastHands
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(4), ValidateNotClass(Rogue))
                .AddToDB();

            // Fighting Surge (Dexterity)
            var fightingSurgeDexterity = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatFightingSurgeDexterity", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Misaye,
                    PowerFighterActionSurge
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateNotClass(Fighter))
                .AddToDB();

            // Fighting Surge (Strength)
            var fightingSurgeStrength = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatFightingSurgeStrength", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Einar,
                    PowerFighterActionSurge
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateNotClass(Fighter))
                .AddToDB();

            // Marksman
            var marksman = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMarksman", ZappaFeatNamespace)
                .SetFeatures(
                    ActionAffinityMarksmanReactionShot
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Metamagic Sorcery Points Feature
            var attributeModifierSorcererSorceryPointsAdd2 = FeatureDefinitionAttributeModifierBuilder
                .Create(AttributeModifierSorcererSorceryPointsBase, "AttributeModifierSorcererSorceryPointsBonus2", ZappaFeatNamespace)              
                .SetGuiPresentationNoContent(true)
                .SetModifier((FeatureDefinitionAttributeModifier.AttributeModifierOperation)ExtraAttributeModifierOperation.AdditiveAtEnd, AttributeDefinitions.SorceryPoints, 2)
                .AddToDB();

            // Metamagic Adept (Careful)
            var metamagicAdeptCareful = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptCareful", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnCareful,
                    attributeModifierSorcererSorceryPointsAdd2
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

            // Metamagic Adept (Distant)
            var metamagicAdeptDistant = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptDistant", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnDistant,
                    attributeModifierSorcererSorceryPointsAdd2
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

            // Metamagic Adept (Empowered)
            var metamagicAdeptEmpowered = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptEmpowered", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnEmpowered,
                    attributeModifierSorcererSorceryPointsAdd2
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

            // Metamagic Adept (Extended)
            var metamagicAdeptExtended = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptExtended", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnExtended,
                    attributeModifierSorcererSorceryPointsAdd2
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

            // Metamagic Adept (Heightened)
            var metamagicAdeptHeightened = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptHeightened", ZappaFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnHeightened,
                    attributeModifierSorcererSorceryPointsAdd2,
                    attributeModifierSorcererSorceryPointsAdd2 // not a dup. adding 4 points
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(8))
                .AddToDB();

            // Metamagic Adept (Quickened)
            var metamagicAdeptQuickened = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptQuickened", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnQuickened,
                    attributeModifierSorcererSorceryPointsAdd2
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

            // Metamagic Adept (Twinned)
            var metamagicAdeptTwinned = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatMetamagicAdeptTwinned", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    FeatureDefinitionMetamagicOptionBuilder.MetamagicLearnTwinned,
                    attributeModifierSorcererSorceryPointsAdd2
                 )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateMinCharLevel(4))
                .AddToDB();

            // Primal (Constitution)
            var primalConstitution = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatPrimalConstitution", ZappaFeatNamespace)
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
            var primalStrength = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
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
                .SetValidators(ValidateNotClass(Barbarian))
                .AddToDB();

            // Shady
            var shady = FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
                .Create("FeatShady", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Misaye,
                    FeatureDefinitionAdditionalDamageBuilder
                        .Create(AdditionalDamageRogueSneakAttack, "AdditionalDamageFeatShadySneakAttack", ZappaFeatNamespace)
                        .SetGuiPresentation("AdditionalDamageFeatShadySneakAttack", Category.Feature)
                        .SetDamageDice(RuleDefinitions.DieType.D6, 1)
                        .SetAdvancement(RuleDefinitions.AdditionalDamageAdvancement.ClassLevel,
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
                        .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.FinesseOrRangeWeapon)
                        .AddToDB()
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
                .SetGuiPresentation(Category.Feat)
                .SetValidators(ValidateNotClass(Rogue))
                .AddToDB();

            // Wise Defense
            var wiseDefense = FeatDefinitionBuilder
                .Create("FeatWiseDefense", ZappaFeatNamespace)
                .SetFeatures(
                    AttributeModifierCreed_Of_Maraike,
                    FeatureDefinitionAttributeModifierBuilder
                        .Create(AttributeModifierBarbarianUnarmoredDefense, "AttributeModifierFeatWiseDefenseAdd", ZappaFeatNamespace)
                        .SetGuiPresentationNoContent()
                        .SetSituationalContext(RuleDefinitions.SituationalContext.NotWearingArmorOrMageArmor)
                        .SetModifierAbilityScore(AttributeDefinitions.Wisdom)
                        .AddToDB()
                )
                .SetAbilityScorePrerequisite(AttributeDefinitions.Wisdom, 13)
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Wise Precision
            var attackModifierWisePrecision = FeatureDefinitionAttackModifierBuilder
                .Create("AttackModifierWisePrecision", ZappaFeatNamespace)
                .SetGuiPresentation("FeatWisePrecision", Category.Feat, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference)
                .SetAbilityScoreReplacement(RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility)
                .SetAdditionalAttackTag(TagsDefinitions.Magical)
                .AddToDB();

            var effectWisePrecision = EffectDescriptionBuilder
                .Create()
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1 /* range */, RuleDefinitions.TargetType.Item, 1, 2, ActionDefinitions.ItemSelectionType.Weapon)
                .SetCreatedByCharacter()
                .SetDurationData(RuleDefinitions.DurationType.Minute, 1 /* duration */, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetItemPropertyForm(RuleDefinitions.ItemPropertyUsage.Unlimited, 0, new FeatureUnlockByLevel(attackModifierWisePrecision, 0))
                        .Build()
                    )
                .Build();

            var wisePrecisionPower = FeatureDefinitionPowerBuilder
                .Create("PowerWisePrecision", ZappaFeatNamespace)
                .SetGuiPresentation("FeatWisePrecision", Category.Feat, FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
                .Configure(2, RuleDefinitions.UsesDetermination.ProficiencyBonus, AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.LongRest, false, true,
                    AttributeDefinitions.Intelligence, effectWisePrecision, false /* unique instance */)
                .AddToDB();

            var wisePrecision = FeatDefinitionBuilder
                .Create("FeatWisePrecision", ZappaFeatNamespace)
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
                wisePrecision);
        }
    }
    
    internal sealed class FeatureDefinitionMetamagicOptionBuilder : FeatureDefinitionBuilder<FeatureDefinitionMetamagicOption, FeatureDefinitionMetamagicOptionBuilder>
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

        internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnHeightened =
            CreateAndAddToDB(MetamagicLearnHeightenedName, MetamagicLearnHeightenedGuid, MetamagicHeightenedSpell);

        internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnQuickened =
            CreateAndAddToDB(MetamagicLearnQuickenedName, MetamagicLearnQuickenedGuid, MetamagicQuickenedSpell);

        internal static readonly FeatureDefinitionMetamagicOption MetamagicLearnTwinned =
            CreateAndAddToDB(MetamagicLearnTwinnedName, MetamagicLearnTwinnedGuid, MetamagicTwinnedSpell);
    }

    internal sealed class FeatureDefinitionMetamagicOption : FeatureDefinition, IFeatureDefinitionCustomCode
    {
        private bool MetamagicTrained { get; set; }

        public MetamagicOptionDefinition MetamagicOption { get; set; }

        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            if (!hero.MetamagicFeatures.ContainsKey(MetamagicOption))
            {
                hero.TrainMetaMagicOptions(new List<MetamagicOptionDefinition>() { MetamagicOption });

                MetamagicTrained = true;
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            if (MetamagicTrained)
            {
                hero.MetamagicFeatures.Remove(MetamagicOption);

                MetamagicTrained = false;
            }
        }
    }
}
