namespace PowerPlantCodingChallenge.Domain;

public sealed record ProductionPlan(
    Powerplant Plant,
    double Allocated
);