using PowerPlantCodingChallenge.Contracts;
using PowerPlantCodingChallenge.Domain;
using PowerPlantCodingChallenge.Enums;
using PowerPlantCodingChallenge.Extensions;

namespace PowerPlantCodingChallenge.Services;

public class ProductionPlanService(ProductionPlanSolver solver)
{
    public IReadOnlyList<ProductionPlan> GetProductionPlan(ProductionPlanRequest request)
    {
        var windPlants = GetWindPlants(request.PowerPlants);
        var thermalPlants = GetThermalPlants(request.PowerPlants);

        // 1. Subtract wind from target load (free energy first)
        var windAllocations = windPlants
            .Select(p => new ProductionPlan(p, p.CalculateWindOutput(request.Fuels.WindPercentage)))
            .ToList();

        var load = request.Load - windAllocations.Sum(a => a.Allocated);

        // 2. Sort thermal plants by cost ascending.
        var meritOrder = solver.GetMeritOrder(thermalPlants, request.Fuels);
        
        // 3. Allocate load based on merit order 
        var thermalAllocations = solver.AllocateLoad(meritOrder, Math.Max(load, 0), request.Fuels);

        // 4. Return final result
        return windAllocations.Concat(thermalAllocations).ToList();
    }
    
    private IEnumerable<Powerplant> GetThermalPlants(IEnumerable<Powerplant> powerPlants) => powerPlants.Where(p => p.Type != PowerPlantType.WindTurbine);
    private IEnumerable<Powerplant> GetWindPlants(IEnumerable<Powerplant> powerPlants) => powerPlants.Where(p => p.Type == PowerPlantType.WindTurbine);
}