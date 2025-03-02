using ActivityCore.Abstraction;

namespace ActivityCore.Stats;
public class ActivityStats
{
    public event EventHandler<int>? CurrentDayStepsChanged;

    private readonly ActivityTracker _activityTracker;
    private readonly IActivityStatsDataAccess _activityStatsDataAccess;

    public ActivityStats(ActivityTracker activityTracker, IActivityStatsDataAccess activityStatsDataAccess)
    {
        _activityTracker = activityTracker;
        _activityStatsDataAccess = activityStatsDataAccess;
    }


}
