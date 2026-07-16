using PowerPlantCodingChallenge.Enums;

namespace PowerPlantCodingChallenge.Domain;

public sealed record Powerplant(
    string Name,
    PowerPlantType Type,
    double Efficiency,
    double Pmin,
    double Pmax
);