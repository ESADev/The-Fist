using UnityEngine;

public static class FormattingHelper
{
    public static string FormatNumber(int number)
    {
        if (number < 0)
            return "-" + FormatNumber(-number);

        // Less than 10,000 - show full number
        if (number < 10000)
            return number.ToString();

        // Define suffixes and their thresholds
        var suffixes = new[]
        {
            new { Threshold = 1000000000, Suffix = "b" },  // Billion
            new { Threshold = 1000000, Suffix = "m" },     // Million
            new { Threshold = 1000, Suffix = "k" }         // Thousand
        };

        foreach (var suffix in suffixes)
        {
            if (number >= suffix.Threshold)
            {
                double value = (double)number / suffix.Threshold;

                // Determine format based on value
                if (value >= 100)
                {
                    // 100k, 999k, 100m, etc. (no decimal)
                    return ((int)value).ToString() + suffix.Suffix;
                }
                else if (value >= 10)
                {
                    // 10.0k to 99.9k, 10.0m to 99.9m, etc.
                    return value.ToString("F1") + suffix.Suffix;
                }
                else
                {
                    // 1.00k to 9.99k, 1.00m to 9.99m, etc.
                    return value.ToString("F2").TrimEnd('0').TrimEnd('.') + suffix.Suffix;
                }
            }
        }

        return number.ToString();
    }

    public static string FormatNumber(float number)
    {
        return FormatNumber(Mathf.RoundToInt(number));
    }
}