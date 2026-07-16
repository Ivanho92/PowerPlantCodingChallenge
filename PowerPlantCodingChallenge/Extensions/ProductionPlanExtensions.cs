using PowerPlantCodingChallenge.Contracts;
using PowerPlantCodingChallenge.Domain;

namespace PowerPlantCodingChallenge.Extensions;

public static class ProductionPlanExtensions
{
    extension(ProductionPlan productionPlan)
    {
        public ProductionPlanDto ToDto()
        {
            return new ProductionPlanDto(productionPlan.Plant.Name, productionPlan.Allocated);
        }
    }
}