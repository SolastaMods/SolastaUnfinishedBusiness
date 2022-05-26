using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(LootPackDefinition))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class LootPackDefinitionExtensions
    {
        public static T AddItemOccurencesList<T>(this T entity, params ItemOccurence[] value)
            where T : LootPackDefinition
        {
            AddItemOccurencesList(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddItemOccurencesList<T>(this T entity, IEnumerable<ItemOccurence> value)
            where T : LootPackDefinition
        {
            entity.ItemOccurencesList.AddRange(value);
            return entity;
        }

        public static T ClearItemOccurencesList<T>(this T entity)
            where T : LootPackDefinition
        {
            entity.ItemOccurencesList.Clear();
            return entity;
        }

        public static T SetInDungeonEditor<T>(this T entity, Boolean value)
            where T : LootPackDefinition
        {
            entity.InDungeonEditor = value;
            return entity;
        }

        public static T SetItemOccurencesList<T>(this T entity, params ItemOccurence[] value)
            where T : LootPackDefinition
        {
            SetItemOccurencesList(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetItemOccurencesList<T>(this T entity, IEnumerable<ItemOccurence> value)
            where T : LootPackDefinition
        {
            entity.ItemOccurencesList.SetRange(value);
            return entity;
        }

        public static T SetLootChallengeMode<T>(this T entity, LootPackDefinition.LootChallenge value)
            where T : LootPackDefinition
        {
            entity.SetField("lootChallengeMode", value);
            return entity;
        }

        public static T SetLootMagnitudeMode<T>(this T entity, LootPackDefinition.LootMagnitude value)
            where T : LootPackDefinition
        {
            entity.SetField("lootMagnitudeMode", value);
            return entity;
        }

        public static T SetLootSpawnMode<T>(this T entity, LootPackDefinition.LootSpawn value)
            where T : LootPackDefinition
        {
            entity.SetField("lootSpawnMode", value);
            return entity;
        }

        public static T SetUserLootPack<T>(this T entity, Boolean value)
            where T : LootPackDefinition
        {
            entity.SetField("<UserLootPack>k__BackingField", value);
            return entity;
        }
    }
}
