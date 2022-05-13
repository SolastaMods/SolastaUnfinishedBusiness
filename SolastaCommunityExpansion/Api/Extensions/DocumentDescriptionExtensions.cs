using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(DocumentDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class DocumentDescriptionExtensions
    {
        public static T AddContentFragments<T>(this T entity, params ContentFragmentDescription[] value)
            where T : DocumentDescription
        {
            AddContentFragments(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddContentFragments<T>(this T entity, IEnumerable<ContentFragmentDescription> value)
            where T : DocumentDescription
        {
            entity.ContentFragments.AddRange(value);
            return entity;
        }

        public static T AddUnlockedBestiaryContent<T>(this T entity, params MonsterKnowledgeDescription[] value)
            where T : DocumentDescription
        {
            AddUnlockedBestiaryContent(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddUnlockedBestiaryContent<T>(this T entity, IEnumerable<MonsterKnowledgeDescription> value)
            where T : DocumentDescription
        {
            entity.UnlockedBestiaryContent.AddRange(value);
            return entity;
        }

        public static T ClearContentFragments<T>(this T entity)
            where T : DocumentDescription
        {
            entity.ContentFragments.Clear();
            return entity;
        }

        public static T ClearUnlockedBestiaryContent<T>(this T entity)
            where T : DocumentDescription
        {
            entity.UnlockedBestiaryContent.Clear();
            return entity;
        }

        public static DocumentDescription Copy(this DocumentDescription entity)
        {
            return new DocumentDescription(entity);
        }

        public static T SetContentFragments<T>(this T entity, params ContentFragmentDescription[] value)
            where T : DocumentDescription
        {
            SetContentFragments(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetContentFragments<T>(this T entity, IEnumerable<ContentFragmentDescription> value)
            where T : DocumentDescription
        {
            entity.ContentFragments.SetRange(value);
            return entity;
        }

        public static T SetDestroyAfterReading<T>(this T entity, System.Boolean value)
            where T : DocumentDescription
        {
            entity.SetField("destroyAfterReading", value);
            return entity;
        }

        public static T SetFormat<T>(this T entity, DocumentDescription.FormatType value)
            where T : DocumentDescription
        {
            entity.SetField("format", value);
            return entity;
        }

        public static T SetLanguage<T>(this T entity, System.String value)
            where T : DocumentDescription
        {
            entity.SetField("language", value);
            return entity;
        }

        public static T SetLocationDefinition<T>(this T entity, LocationDefinition value)
            where T : DocumentDescription
        {
            entity.SetField("locationDefinition", value);
            return entity;
        }

        public static T SetLocationKnowledgeLevel<T>(this T entity, GameCampaignDefinitions.NodeKnowledge value)
            where T : DocumentDescription
        {
            entity.SetField("locationKnowledgeLevel", value);
            return entity;
        }

        public static T SetLoreType<T>(this T entity, RuleDefinitions.LoreType value)
            where T : DocumentDescription
        {
            entity.SetField("loreType", value);
            return entity;
        }

        public static T SetRecipeDefinition<T>(this T entity, RecipeDefinition value)
            where T : DocumentDescription
        {
            entity.SetField("recipeDefinition", value);
            return entity;
        }

        public static T SetScript<T>(this T entity, DocumentDescription.ScriptType value)
            where T : DocumentDescription
        {
            entity.SetField("script", value);
            return entity;
        }

        public static T SetUnlockedBestiaryContent<T>(this T entity, params MonsterKnowledgeDescription[] value)
            where T : DocumentDescription
        {
            SetUnlockedBestiaryContent(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetUnlockedBestiaryContent<T>(this T entity, IEnumerable<MonsterKnowledgeDescription> value)
            where T : DocumentDescription
        {
            entity.UnlockedBestiaryContent.SetRange(value);
            return entity;
        }
    }
}