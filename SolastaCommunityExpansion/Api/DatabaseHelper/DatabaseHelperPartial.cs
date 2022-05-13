using SolastaModApi.Diagnostics;

namespace SolastaModApi
{
    public static partial class DatabaseHelper
    {
        public static T GetDefinition<T>(string key, string guid) where T : BaseDefinition
        {
            var db = DatabaseRepository.GetDatabase<T>();

            if (db == null)
            {
                throw new SolastaModApiException($"Database of type {typeof(T).Name} not found.");
            }

            var definition = db.TryGetElement(key, guid);

            if (definition == null)
            {
                throw new SolastaModApiException($"Definition with name={key} or guid={guid} not found in database {typeof(T).Name}");
            }

            return definition;
        }

        public static bool TryGetDefinition<T>(string key, string guid, out T definition) where T : BaseDefinition
        {
            var db = DatabaseRepository.GetDatabase<T>();

            if (key == null || db == null)
            {
                definition = null;
                return false;
            }

            definition = db.TryGetElement(key, guid);
            return definition != null;
        }
    }
}
