namespace proiectSenat;

using System.Text.Json.Serialization;

public class Payload
{
    [JsonPropertyName("text")]
    public required string Text { get; set; }

    [JsonPropertyName("an")]
    public required int An { get; set; }

    [JsonPropertyName("numar_lege")]
    public required string NumarLege { get; set; }

    [JsonPropertyName("chunk")]
    public required int Chunk { get; set; }

    [JsonPropertyName("fisier")]
    public required string Fisier { get; set; }
}