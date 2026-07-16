using PowerPlantCodingChallenge.Config;
using PowerPlantCodingChallenge.Extensions;
using PowerPlantCodingChallenge.Utils;

namespace PowerPlantCodingChallenge.Domain;

public class ProductionPlanSolver {
    public List<Powerplant> GetMeritOrder(IEnumerable<Powerplant> powerPlants, Fuels fuels)
    {
        return powerPlants
            .OrderBy(p => p.CalculateCostPerMwh(fuels, AppConfig.Co2TonsPerMwhGenerated))
            .ToList();
    }

    public List<ProductionPlan> AllocateLoad(IReadOnlyList<Powerplant> meritOrder, double load, Fuels fuels)
    {
        // Loop through merit order, filling pmax greedily
        var allocations = GreedyFill(meritOrder, load);

        // Get last plant to be activated
        var lastIndex = allocations.FindLastIndex(a => a.Allocated > 0);
        if (lastIndex == -1)
            return allocations;
        
        // Check if last plant allocation respects Pmin 
        var last = allocations[lastIndex];
        
        // No conflict: return result
        if (last.Allocated >= last.Plant.Pmin)
            return allocations;

        // Conflict: last plant does not respect Pmin, two options:
        // Option A: remove the marginal plant entirely and re-solve from scratch.
        // If this creates a new conflict further down, the recursive call handles it again.
        var withoutLast = meritOrder.Where(p => p != last.Plant).ToList();
        var optionA = PadMissing(AllocateLoad(withoutLast, load, fuels), meritOrder);

        // Option B: force the marginal plant on at pmin, and reduce from cheaper plants.
        var optionB = ForceMarginalPlantOnAtPmin(allocations, lastIndex);

        return TotalCost(optionA, fuels) <= TotalCost(optionB, fuels) ? optionA : optionB;
    }

    // private helpers
    private static List<ProductionPlan> GreedyFill(IReadOnlyList<Powerplant> meritOrder, double load)
    {
        var allocations = new List<ProductionPlan>(meritOrder.Count);
        var remainingLoad = load;

        foreach (var plant in meritOrder)
        {
            var toGive = remainingLoad <= 0 ? 0 : Math.Min(plant.Pmax, remainingLoad);
            allocations.Add(new ProductionPlan(plant, MathUtils.Round(toGive)));
            remainingLoad -= toGive;
        }

        return allocations;
    }

    private static List<ProductionPlan> ForceMarginalPlantOnAtPmin(List<ProductionPlan> allocations, int marginalIndex)
    {
        var result = new List<ProductionPlan>(allocations);
        var marginal = result[marginalIndex];
        var excess = marginal.Plant.Pmin - marginal.Allocated;
        result[marginalIndex] = marginal with { Allocated = marginal.Plant.Pmin };

        for (var i = marginalIndex - 1; i >= 0 && excess > 0; i--)
        {
            var current = result[i];
            var reducible = Math.Max(current.Allocated - current.Plant.Pmin, 0);
            var reduceBy = Math.Min(reducible, excess);
            result[i] = current with { Allocated = MathUtils.Round(current.Allocated - reduceBy) };
            excess -= reduceBy;
        }

        return result;
    }
    
    private static List<ProductionPlan> PadMissing(List<ProductionPlan> allocations, IReadOnlyList<Powerplant> fullMeritOrder)
    {
        var byPlant = allocations.ToDictionary(a => a.Plant);
        return fullMeritOrder
            .Select(p => byPlant.TryGetValue(p, out var a) ? a : new ProductionPlan(p, 0))
            .ToList();
    }
    
    private double TotalCost(IEnumerable<ProductionPlan> allocations, Fuels fuels) =>
        allocations.Sum(a => a.Allocated * a.Plant.CalculateCostPerMwh(fuels, AppConfig.Co2TonsPerMwhGenerated));
}