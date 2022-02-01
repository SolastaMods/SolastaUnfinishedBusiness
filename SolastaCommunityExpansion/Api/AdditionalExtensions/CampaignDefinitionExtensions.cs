using System;

namespace SolastaModApi.Extensions
{
    public static partial class CampaignDefinitionExtensions
    {
        [Obsolete("Use SetMaxStartLevel")]
        public static T SetMaxLevel<T>(this T entity, int value) where T : CampaignDefinition
        {
            entity.SetMaxStartLevel(value);
            return entity;
        }

        [Obsolete("Use SetMinStartLevel")]
        public static T SetMinLevel<T>(this T entity, int value) where T : CampaignDefinition
        {
            entity.SetMinStartLevel(value);
            return entity;
        }
    }
}
