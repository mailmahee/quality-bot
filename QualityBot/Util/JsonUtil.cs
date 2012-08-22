namespace QualityBot.Util
{
    using System.Web.Script.Serialization;
    using Newtonsoft.Json;
    using QualityBot.Enums;

    public static class JsonUtil
    {
        /// <summary>
        /// Serialized the specified object to a json string.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A json object.</returns>
        public static string ObjectToJson(object obj)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// Deserializes the specified object.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="json">The json object.</param>
        /// <returns>An object of type T.</returns>
        public static T JsonToType<T>(string json)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            var obj = serializer.Deserialize<T>(json);

            return obj;
        }

        /// <summary>
        /// Convert object to JSON.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}