using ActivityCore.Stats;

namespace ActivityCore.Abstraction;
public interface IActivityStatsDataAccess
{
    void SaveHourlySteps(HourlySteps hourlySteps);
}
