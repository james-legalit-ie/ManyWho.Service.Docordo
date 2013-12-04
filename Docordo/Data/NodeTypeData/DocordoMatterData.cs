namespace ManyWho.Service.Docordo.Data.NodeTypeData
{
    using System;
    using Newtonsoft.Json;

    public class DocordoMatterData
    {
        [JsonProperty(PropertyName = "cada17e7ace149c9a476d300a41e131d")]
        public string MatterDescription { get; set; }

        [JsonProperty(PropertyName = "cdda28982c004e209ec371038e2da527")]
        public string MatterStatus { get; set; }

        public static DocordoMatterData New(string matterDescription)
        {
            DocordoMatterData docordoMatterData = new DocordoMatterData()
            {
                MatterDescription = matterDescription,
                MatterStatus = "Active"
            };

            return docordoMatterData;
        }
    }
}
