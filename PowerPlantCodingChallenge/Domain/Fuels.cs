using System.Text.Json.Serialization;

namespace PowerPlantCodingChallenge.Domain;

public sealed record Fuels
{
    [JsonPropertyName("gas(euro/MWh)")]
    public required double GasPrice { get; init; }

    [JsonPropertyName("kerosine(euro/MWh)")]
    public required double KerosinePrice { get; init; }

    [JsonPropertyName("co2(euro/ton)")]
    public required double Co2Price { get; init; }

    [JsonPropertyName("wind(%)")]
    public required double WindPercentage { get; init; }
}