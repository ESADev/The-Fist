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
    public static float Randomized(this float value, float randomness = 0.1f)
    {
        if (randomness <= 0f) return value;
        return UnityEngine.Random.Range(value * (1 - randomness), value * (1 + randomness));
    }
}