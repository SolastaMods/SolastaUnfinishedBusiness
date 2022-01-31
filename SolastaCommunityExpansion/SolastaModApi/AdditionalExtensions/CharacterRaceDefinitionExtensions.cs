using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    public static partial class CharacterRaceDefinitionExtensions
    {
        public static T RemoveFeatureUnlock<T>(this T entity, FeatureDefinition feature) where T : CharacterRaceDefinition
        {
            var fd = entity.FeatureUnlocks.SingleOrDefault(f => f.FeatureDefinition == feature);

            if (fd != null)
            {
                entity.FeatureUnlocks.Remove(fd);
            }

            return entity;
        }

        public static T AddFeatureUnlock<T>(this T entity, FeatureDefinition feature, int level) where T : CharacterRaceDefinition
        {
            entity.FeatureUnlocks.Add(new FeatureUnlockByLevel(feature, level));

            return entity;
        }

        public static T SortFeatureUnlocks<T>(this T entity) where T : CharacterRaceDefinition
        {
            entity.FeatureUnlocks.SetRange(
                entity.FeatureUnlocks.OrderBy(fu => fu.Level).ThenBy(fu => fu.FeatureDefinition.Name));

            return entity;
        }
    }
}
