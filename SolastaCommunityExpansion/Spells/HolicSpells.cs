using System;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static ActionDefinitions;
using static EffectForm;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Models.SpellsContext;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static SolastaModApi.DatabaseHelper.SpellListDefinitions;

namespace SolastaCommunityExpansion.Spells
{
    internal static class HolicSpells
    {
        internal static readonly Guid HOLIC_SPELLS_BASE_GUID = new("9e55f5f8-4c71-47c2-8a55-2a2cf06d8ce5");

        private static readonly SpellDefinition AcidClaw = BuildAcidClaw();
        private static readonly SpellDefinition AirBlast = BuildAirBlast();
        private static readonly SpellDefinition BurstOfRadiance = BuildBurstOfRadiance();
        private static readonly SpellDefinition ThunderStrike = BuildThunderStrike();
        private static readonly SpellDefinition WinterBreath = BuildWinterBreath();
        private static readonly SpellDefinition EarthTremor = BuildEarthTremor();

        internal static void AddToDB()
        {
            _ = AcidClaw;
            _ = AirBlast;
            _ = BurstOfRadiance;
            _ = ThunderStrike;
            _ = EarthTremor;
            _ = WinterBreath;
        }

        internal static void Register()
        {
            RegisterSpell(AcidClaw, 0, SpellListDruid);
            RegisterSpell(AirBlast, 0, SpellListWizard, SpellListSorcerer, SpellListDruid);
            RegisterSpell(BurstOfRadiance, 0, SpellListCleric);
            RegisterSpell(ThunderStrike, 0, SpellListWizard, SpellListSorcerer, SpellListDruid);
            RegisterSpell(EarthTremor, 0, SpellListWizardGreenmage, SpellListWizard, SpellListSorcerer, SpellListDruid);
            RegisterSpell(WinterBreath, 0, SpellListWizardGreenmage, SpellListDruid);
        }

        private static SpellDefinition BuildAcidClaw()
        {
            const string name = "AcidClaws";

            var spriteReference = Utils.CustomIcons.CreateAssetReferenceSprite(name, Properties.Resources.AcidClaws, 128, 128);

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetEffectAdvancement(
                    EffectIncrementMethod.CasterLevelTable, 1, 0, 1, 0, 0, 0, 0, 0, 0, AdvancementDuration.None)
                .SetDurationData(DurationType.Instantaneous)
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 6, TargetType.Individuals, 1, 2, ItemSelectionType.None)
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(false, DieType.D1, DamageTypeNecrotic, 0, DieType.D10, 1, HealFromInflictedDamage.Never)
                        .HasSavingThrow(EffectSavingThrowType.None).Build()
                ).Build();

            var spell = SpellDefinitionBuilder
                .Create(name, HOLIC_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, spriteReference)
                .SetEffectDescription(effectDescription)
                .SetCastingTime(ActivationTime.Action)
                .SetSpellLevel(0)
                .SetRequiresConcentration(false)
                .SetVerboseComponent(false)
                .SetSomaticComponent(true)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
                .AddToDB();

            return spell;
        }

        internal static SpellDefinition BuildAirBlast()
        {
            const string name = "AirBlast";

            var spriteReference = Utils.CustomIcons.CreateAssetReferenceSprite(name, Properties.Resources.AirBlast, 128, 128);

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1, 0, 0, 0, 0, 0, 0, AdvancementDuration.None)
                .SetSavingThrowData(true, false, AttributeDefinitions.Strength, false, EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 15, false)
                .SetDurationData(DurationType.Instantaneous)
                .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals, 1, 2, ItemSelectionType.None)
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                        .HasSavingThrow(EffectSavingThrowType.Negates).Build())
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(false, DieType.D1, DamageTypeBludgeoning, 0, DieType.D6, 1, HealFromInflictedDamage.Never)
                        .HasSavingThrow(EffectSavingThrowType.Negates).Build()
                ).Build();

            var spell = SpellDefinitionBuilder
                .Create(name, HOLIC_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, spriteReference)
                .SetEffectDescription(effectDescription)
                .SetCastingTime(ActivationTime.Action)
                .SetSpellLevel(0)
                .SetRequiresConcentration(false)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetMaterialComponent(MaterialComponentType.None)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .AddToDB();

            return spell;
        }

        internal static SpellDefinition BuildBurstOfRadiance()
        {
            const string name = "BurstOfRadiance";

            var spriteReference = Utils.CustomIcons.CreateAssetReferenceSprite(name, Properties.Resources.BurstOfRadiance, 128, 128);

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1, 0, 0, 0, 0, 0, 0, AdvancementDuration.None)
                .SetSavingThrowData(true, true, AttributeDefinitions.Constitution, false, EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 13, false)
                .SetDurationData(DurationType.Instantaneous)
                .SetParticleEffectParameters(BurningHands.EffectDescription.EffectParticleParameters)
                .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 1, 2, ItemSelectionType.None)
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(false, DieType.D1, DamageTypeRadiant, 0, DieType.D6, 1, HealFromInflictedDamage.Never)
                        .HasSavingThrow(EffectSavingThrowType.Negates).Build()
                ).Build();

            var spell = SpellDefinitionBuilder
                .Create(name, HOLIC_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, spriteReference)
                .SetEffectDescription(effectDescription)
                .SetCastingTime(ActivationTime.Action)
                .SetSpellLevel(0)
                .SetRequiresConcentration(false)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
                .AddToDB();

            return spell;
        }

        private static SpellDefinition BuildThunderStrike()
        {
            const string name = "ThunderStrike";

            var spriteReference = Shield.GuiPresentation.SpriteReference;

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetEffectAdvancement(
                    EffectIncrementMethod.CasterLevelTable, 1, 0, 1, 0, 0, 0, 0, 0, 0, AdvancementDuration.None)
                .SetSavingThrowData(true, true, AttributeDefinitions.Constitution, false, EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 15, false)
                .SetDurationData(DurationType.Instantaneous)
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere, 1, 1, ItemSelectionType.None)
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(false, DieType.D1, DamageTypeThunder, 0, DieType.D6, 1, HealFromInflictedDamage.Never)
                        .HasSavingThrow(EffectSavingThrowType.Negates).Build()
                ).Build();

            effectDescription.SetTargetExcludeCaster(true);

            var spell = SpellDefinitionBuilder
                .Create(name, HOLIC_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, spriteReference)
                .SetEffectDescription(effectDescription)
                .SetCastingTime(ActivationTime.Action)
                .SetSpellLevel(0)
                .SetRequiresConcentration(false)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
                .AddToDB();

            return spell;
        }

        private static SpellDefinition BuildEarthTremor()
        {
            const string name = "EarthTremor";

            var spriteReference = Utils.CustomIcons.CreateAssetReferenceSprite(name, Properties.Resources.EarthTremor, 128, 128);

            //var rubbleProxy = EffectProxyDefinitionBuilder
            //    .Create(EffectProxyDefinitions.ProxyGrease, "RubbleProxy", "")
            //    .SetGuiPresentation(Category.EffectProxy, spriteReference)
            //    .AddToDB();

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetEffectAdvancement(
                    EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1, 0, 0, 0, 0, 0, 0, AdvancementDuration.None)
                .SetSavingThrowData(true, true, AttributeDefinitions.Dexterity, false, EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom, 12, false)
                .SetDurationData(DurationType.Minute, 10)
                .SetParticleEffectParameters(Grease.EffectDescription.EffectParticleParameters)
                .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Cylinder, 1, 1, ItemSelectionType.None)
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                        .CreatedByCharacter()
                        .HasSavingThrow(EffectSavingThrowType.Negates).Build())
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(false, DieType.D1, DamageTypeBludgeoning, 0, DieType.D12, 3, HealFromInflictedDamage.Never)
                        .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build()
                ).Build();

            effectDescription.EffectForms.AddRange(
                Grease.EffectDescription.EffectForms.Find(e => e.GetField<EffectForm, EffectFormType>("formType") == EffectForm.EffectFormType.Topology));

            var spell = SpellDefinitionBuilder
                .Create(name, HOLIC_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, spriteReference)
                .SetEffectDescription(effectDescription)
                .SetCastingTime(ActivationTime.Action)
                .SetSpellLevel(3)
                .SetRequiresConcentration(false)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .AddToDB();

            return spell;
        }

        private static SpellDefinition BuildWinterBreath()
        {
            const string name = "WinterBreath";

            var spriteReference = Utils.CustomIcons.CreateAssetReferenceSprite(name, Properties.Resources.WinterBreath, 128, 128);

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
                .SetSavingThrowData(true, true, AttributeDefinitions.Dexterity, false, EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom, 12, false)
                .SetDurationData(DurationType.Minute, 1)
                .SetParticleEffectParameters(ConeOfCold.EffectDescription.EffectParticleParameters)
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3, 2)
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                        .HasSavingThrow(EffectSavingThrowType.Negates).Build())
                .AddEffectForm(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(damageType: DamageTypeCold, dieType: DieType.D8, diceNumber: 4)
                        .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build()
                ).Build();

            var spell = SpellDefinitionBuilder
                .Create(name, HOLIC_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, spriteReference)
                .SetEffectDescription(effectDescription)
                .SetCastingTime(ActivationTime.Action)
                .SetSpellLevel(3)
                .SetRequiresConcentration(false)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
                .AddToDB();

            return spell;
        }
    }
}
