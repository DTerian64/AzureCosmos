
using Newtonsoft.Json;

namespace ConsoleAppCosmosdb.Models
{
    public class Product
    {
        [JsonProperty("id")]
        public required string id { get; set; }
        public required string categoryid { get; set; }
        public required string name { get; set; }
        public required double price { get; set; }
        public string[]? tags { get; set; }

        public Product(){}
    }
}