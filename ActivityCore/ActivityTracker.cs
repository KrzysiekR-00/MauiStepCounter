using ActivityCore.Abstraction;
using ActivityCore.Stats;

namespace ActivityCore;
public class ActivityTracker
{
    private readonly IPedometer _pedometer;
    private readonly IActivityStatsDataAccess _activityStatsDataAccess;

    int _currentSteps = 0;
    DateTime _lastStepsDateTime = DateTime.MinValue;

    public ActivityTracker(IPedometer pedometer, IActivityStatsDataAccess activityStatsDataAccess)
    {
        _pedometer = pedometer;
        _activityStatsDataAccess = activityStatsDataAccess;

        _pedometer.StepsRegistered += NewStepsRegistered;
    }

    public void Start() => _pedometer.Start();
    public void Stop() => _pedometer.Stop();
    public bool IsActive => _pedometer.IsActive;

    private void NewStepsRegistered(object? sender, int newSteps)
    {
        var now = DateTime.Now;

        if (_currentSteps > 0)
        {
            if (now.Hour > _lastStepsDateTime.Hour ||
                now.Day > _lastStepsDateTime.Day ||
                now.Month > _lastStepsDateTime.Month ||
                now.Year > _lastStepsDateTime.Year
                )
            {
                SaveHourlySteps();
                _currentSteps = 0;
            }
        }

        _currentSteps += newSteps;
        _lastStepsDateTime = now;
    }

    private void SaveHourlySteps()
    {
        HourlySteps hourlySteps = new(
            DateOnly.FromDateTime(_lastStepsDateTime),
            _lastStepsDateTime.Hour,
            _currentSteps
            );

        _activityStatsDataAccess.SaveHourlySteps(hourlySteps);
    }
}