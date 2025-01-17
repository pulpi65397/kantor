using System.Text.Json.Serialization;

namespace KantorLibrary.Models
{
    public class Kurs
    {
        public int Id { get; set; }
        [JsonPropertyName("grafika")]
        public string Grafika { get; set; }
        [JsonPropertyName("waluta")]
        public string Waluta { get; set; }
        [JsonPropertyName("kursK")]
        public decimal KursK { get; set; }
        [JsonPropertyName("kursS")]
        public decimal KursS { get; set; }
    }
}
