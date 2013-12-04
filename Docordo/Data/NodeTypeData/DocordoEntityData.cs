namespace ManyWho.Service.Docordo.Data.NodeTypeData
{
    using Newtonsoft.Json;

    public class DocordoEntityData
    {
        [JsonProperty(PropertyName = "h64f5f096c5247e7ad20daa46f5bc5b2")]
        public string EntityType { get; set; }

        [JsonProperty(PropertyName = "d26e23701a0a49f288e6246943a30b0c")]
        public string Name { get; set; }

        public static DocordoEntityData New(string entityType, string clientName)
        {
            DocordoEntityData docordoMatterData = new DocordoEntityData()
            {
                EntityType = entityType,
                Name = clientName
            };

            return docordoMatterData;
        }
    }
}
