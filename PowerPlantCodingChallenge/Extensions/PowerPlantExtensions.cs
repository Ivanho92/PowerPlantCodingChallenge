using PowerPlantCodingChallenge.Domain;
using PowerPlantCodingChallenge.Enums;
using PowerPlantCodingChallenge.Utils;

namespace PowerPlantCodingChallenge.Extensions;

public static class PowerPlantExtensions
{
    extension(Powerplant powerPlant)
    {
        public double CalculateCostPerMwh(Fuels fuels, double co2TonsPerMwhGenerated)
        {
            return powerPlant.Type switch
            {
                PowerPlantType.WindTurbine => 0,
                PowerPlantType.GasFired => fuels.GasPrice / powerPlant.Efficiency +
                                           fuels.Co2Price * co2TonsPerMwhGenerated,
                PowerPlantType.TurboJet => fuels.KerosinePrice / powerPlant.Efficiency,
                _ => throw new ArgumentOutOfRangeException(nameof(powerPlant.Type))
            };
        }

        public double CalculateWindOutput(double windPercentage)
        {
            if (powerPlant.Type != PowerPlantType.WindTurbine) return 0;
            return MathUtils.Round(powerPlant.Pmax * windPercentage / 100);
        }
    }
}