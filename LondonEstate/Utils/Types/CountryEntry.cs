using System.Text.Json.Serialization;

namespace LondonEstate.Utils.Types
{
    public class CountryEntry
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("prefix")]
        public string? Prefix { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
