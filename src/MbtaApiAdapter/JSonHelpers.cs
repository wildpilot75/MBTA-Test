namespace MbtaApiAdapter
{
    using Newtonsoft.Json.Linq;

    public static class JSonHelpers
    {
        public static JToken GetData(JObject jObject)
        {
            return jObject["data"];
        }

        public static JToken GetData(JToken token)
        {
            return token["data"];
        }

        public static string GetRelationshipId(JToken token)
        {
            var data = token["data"];
            if (data.HasValues)
            {
                return data["id"].ToString();
            }

            return string.Empty;
        }
    }
}
