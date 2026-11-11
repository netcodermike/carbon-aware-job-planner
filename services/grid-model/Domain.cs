using System.Text.Json;

internal static class Domain
{
    public static object Execute(JsonElement input)
    {
        var grid = input.GetProperty("gridGPerKwh").EnumerateArray().Select(value => value.GetDecimal()).ToArray();
        var duration = input.GetProperty("hours").GetInt32();
        var consumption = input.GetProperty("kwhPerHour").GetDecimal();
        if (grid.Length > 168 || grid.Any(value => value is < 0 or > 10000)
            || duration < 1 || duration > grid.Length || consumption is <= 0 or > 1000000)
        {
            throw new ArgumentException("Expected up to 168 grid values, a fitting duration and positive consumption");
        }
        var scores = new List<object>();
        var total = grid.Take(duration).Sum();
        for (var start = 0; start <= grid.Length - duration; start++)
        {
            var end = start + duration;
            scores.Add(new { startHour = start, endHour = end, grams = total * consumption });
            if (end < grid.Length)
            {
                total += grid[end] - grid[start];
            }
        }
        return new { windows = scores };
    }
}
