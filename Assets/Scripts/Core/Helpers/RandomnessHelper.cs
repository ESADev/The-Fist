using UnityEngine;

public static class RandomnessHelper
{
    /// <summary>
    /// Returns a random boolean value based on the given probability.
    /// </summary>
    /// <param name="probability"></param>
    /// <returns></returns>
    public static bool GetRandomBool(float probability = 0.5f)
    {
        return UnityEngine.Random.value < probability;
    }

    /// <summary>
    /// Returns a value randomized within a range defined by the given value and randomness factor.
    /// The randomness factor defines the percentage of variation allowed.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="randomness"></param>
    /// <returns></returns>
    public static float Randomized(this float value, float randomness = 0.2f)
    {
        if (randomness <= 0f) return value;
        return UnityEngine.Random.Range(value * (1 - randomness), value * (1 + randomness));
    }

    /// <summary>
    /// Generates a random value following a Gaussian distribution.
    /// </summary>
    /// <returns></returns>
    public static float RandomGaussian01()
    {
        float u1 = 1.0f - Random.value; // [0,1) -> (0,1]
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        // randStdNormal is now a standard normal distributed value (mean = 0, std = 1)

        // To fit it into the [0, 1] range, we can map N(0,1) into [0,1] using the CDF approach (approximate)
        // But for simplicity, just clamp after shifting and scaling
        float mean = 0f;
        float stdDev = 1f;
        float scaled = randStdNormal * stdDev + mean;

        // Clamp to [0,1] or use a tighter stdDev to naturally stay inside
        return Mathf.Clamp01((scaled + 3f) / 6f); // Map [-3,3] to [0,1]
    }
}