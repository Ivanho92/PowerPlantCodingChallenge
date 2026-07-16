using FluentAssertions;
using PowerPlantCodingChallenge.Contracts;
using PowerPlantCodingChallenge.Domain;
using PowerPlantCodingChallenge.Enums;
using PowerPlantCodingChallenge.Services;

namespace PowerPlantCodingChallenge.Tests;

public class ProductionPlanServiceTests
{
    private readonly ProductionPlanService _sut;

    public ProductionPlanServiceTests()
    {
        var solver = new ProductionPlanSolver();
        _sut = new ProductionPlanService(solver);
    }
    
    private ProductionPlanRequest BuildRequest(double load, double windPercentage)
    {
        var fuels = new Fuels
        {
            GasPrice = 13.4,
            KerosinePrice = 50.8,
            Co2Price = 20,
            WindPercentage = windPercentage,
        };

        var powerPlants = new List<Powerplant>
        {
            new("gasfiredbig1", PowerPlantType.GasFired, 0.53, 100, 460),
            new("gasfiredbig2", PowerPlantType.GasFired, 0.53, 100, 460),
            new("gasfiredsomewhatsmaller", PowerPlantType.GasFired, 0.37, 40, 210),
            new("tj1", PowerPlantType.TurboJet, 0.3, 0, 16),
            new("windpark1", PowerPlantType.WindTurbine, 1, 0, 150),
            new("windpark2", PowerPlantType.WindTurbine, 1, 0, 36),
        };

        return new ProductionPlanRequest
        {
            PowerPlants = powerPlants,
            Fuels = fuels,
            Load = load,
        };
    }

    [Fact]
    public void GetProductionPlan_TotalAllocatedPower_AlwaysEqualsLoad()
    {
        var request = BuildRequest(load: 480, windPercentage: 60);

        var result = _sut.GetProductionPlan(request);

        result.Sum(r => r.Allocated).Should().Be(request.Load);
    }

    [Fact]
    public void GetProductionPlan_CheapestPlantsFillFirst_MoreExpensivePlantsStayOff()
    {
        // With this load and wind%, gasfiredbig1 alone can cover the remaining load,
        // so the pricier plants (smaller gas plant, turbojet) should stay untouched.
        var request = BuildRequest(load: 480, windPercentage: 60);

        var result = _sut.GetProductionPlan(request);

        result.Single(r => r.Plant.Name == "gasfiredsomewhatsmaller").Allocated.Should().Be(0);
        result.Single(r => r.Plant.Name == "tj1").Allocated.Should().Be(0);
    }

    [Fact]
    public void GetProductionPlan_WhenMarginalPlantWouldViolatePmin_RespectsPminForEveryActivatedPlant()
    {
        // The known "trap" case: naive greedy fill gives gasfiredbig2 only 20MW,
        // which is below its pmin of 100. The algorithm must recognize this and
        // redistribute load between the two identical big plants instead.
        var request = BuildRequest(load: 480, windPercentage: 0);

        var result = _sut.GetProductionPlan(request);

        result.Where(r => r.Allocated > 0)
            .Should().OnlyContain(
                r => r.Allocated >= r.Plant.Pmin,
                "every activated plant must produce at least its pmin");

        var big1 = result.Single(r => r.Plant.Name == "gasfiredbig1").Allocated;
        var big2 = result.Single(r => r.Plant.Name == "gasfiredbig2").Allocated;
        (big1 + big2).Should().BeApproximately(480, precision: 0.1);
    }

    [Fact]
    public void GetProductionPlan_WhenWindAloneCoversLoad_NoThermalPlantIsActivated()
    {
        var request = BuildRequest(load: 100, windPercentage: 100); // windpark1 (150) alone exceeds 100

        var result = _sut.GetProductionPlan(request);

        result.Where(r => r.Plant.Type != PowerPlantType.WindTurbine)
            .Should().OnlyContain(r => r.Allocated == 0);
    }

    [Fact]
    public void GetProductionPlan_HighLoad_ActivatesMultipleGasPlantsInMeritOrder()
    {
        var request = BuildRequest(load: 910, windPercentage: 60);

        var result = _sut.GetProductionPlan(request);

        var big1 = result.Single(r => r.Plant.Name == "gasfiredbig1").Allocated;
        var big2 = result.Single(r => r.Plant.Name == "gasfiredbig2").Allocated;

        big1.Should().Be(460); // maxed out, cheapest plant
        big2.Should().BeGreaterThan(0); // second plant needed to cover the rest
        result.Sum(r => r.Allocated).Should().BeApproximately(910, precision: 0.1);
    }

    [Fact]
    public void GetProductionPlan_EveryPlantInRequest_HasACorrespondingAllocation()
    {
        var request = BuildRequest(load: 480, windPercentage: 60);

        var result = _sut.GetProductionPlan(request);

        result.Should().HaveCount(request.PowerPlants.Count());
        result.Select(r => r.Plant.Name)
            .Should().BeEquivalentTo(request.PowerPlants.Select(p => p.Name));
    }
}