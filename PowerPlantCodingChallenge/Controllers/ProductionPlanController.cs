using Microsoft.AspNetCore.Mvc;
using PowerPlantCodingChallenge.Contracts;
using PowerPlantCodingChallenge.Extensions;
using PowerPlantCodingChallenge.Services;

namespace PowerPlantCodingChallenge.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductionPlanController(ProductionPlanService productionPlanService) : ControllerBase
{
    [HttpPost]
    public IReadOnlyList<ProductionPlanDto> Get([FromBody] ProductionPlanRequest request)
    {
        var productionPlan = productionPlanService.GetProductionPlan(request);
        return productionPlan.Select(p => p.ToDto()).ToList();
    }
}