using PowerPlantCodingChallenge.Domain;

namespace PowerPlantCodingChallenge.Contracts;

public sealed record ProductionPlanRequest
{
    public double Load { get; init; }
    public required Fuels Fuels { get; init; }
    public required IEnumerable<Powerplant> PowerPlants { get; init; }
}