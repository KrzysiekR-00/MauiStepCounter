using ActivityCore.Abstraction;
using ActivityCore.Stats;

namespace ActivityCore;
public class ActivityTracker
{
    public event EventHandler<int>? CurrentHourStepsChanged;

    private readonly IPedometer _pedometer;
    private readonly IActivityStatsDataAccess _activityStatsDataAccess;

    private int _currentSteps = 0;

    private int CurrentSteps
    {
        get { return _currentSteps; }
        set
        {
            if (_currentSteps != value)
            {
                _currentSteps = value;

                CurrentHourStepsChanged?.Invoke(this, CurrentSteps);
            }
        }
    }

    private DateTime _lastStepsDateTime = DateTime.MinValue;

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

        if (CurrentSteps > 0)
        {
            if (now.Hour > _lastStepsDateTime.Hour ||
                now.Day > _lastStepsDateTime.Day ||
                now.Month > _lastStepsDateTime.Month ||
                now.Year > _lastStepsDateTime.Year
                )
            {
                SaveHourlySteps();
                CurrentSteps = 0;
            }
        }

        CurrentSteps += newSteps;
        _lastStepsDateTime = now;
    }

    private void SaveHourlySteps()
    {
        HourlySteps hourlySteps = new(
            DateOnly.FromDateTime(_lastStepsDateTime),
            _lastStepsDateTime.Hour,
            CurrentSteps
            );

        _activityStatsDataAccess.SaveHourlySteps(hourlySteps);
    }
}