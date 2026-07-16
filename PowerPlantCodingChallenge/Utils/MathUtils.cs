namespace PowerPlantCodingChallenge.Utils;

public static class MathUtils
{
    public static double Round(double value) => Math.Round(value, 1, MidpointRounding.AwayFromZero);
}