namespace ActivityCore.Stats;
public readonly struct DailySteps(DateOnly date, int steps)
{
    public readonly DateOnly Date { get; } = date;
    public readonly int Steps { get; } = steps;
}
