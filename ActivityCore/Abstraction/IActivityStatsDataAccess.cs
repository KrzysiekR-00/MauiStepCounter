using ActivityCore.Stats;

namespace ActivityCore.Abstraction;
public interface IActivityStatsDataAccess
{
    void SaveHourlySteps(HourlySteps hourlySteps);

    HourlySteps LoadHourlySteps(DateOnly date, int hour);

    DailySteps LoadDailySteps(DateOnly date);
}
