namespace ActivityCore.Stats;
public readonly struct HourlySteps(DateOnly date, int hour, int steps)
{
    public readonly DateOnly Date { get; } = date;
    public readonly int Hour { get; } = hour;
    public readonly int Steps { get; } = steps;
}
