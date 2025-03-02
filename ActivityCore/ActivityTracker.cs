using ActivityCore.Abstraction;
using ActivityCore.Stats;
using System.ComponentModel;

namespace ActivityCore;
public class ActivityTracker : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly IPedometer _pedometer;
    private readonly IActivityStatsDataAccess _activityStatsDataAccess;

    private int _currentHourSteps = 0;
    private int _currentDaySteps = 0;

    public int CurrentHourSteps
    {
        get { return _currentHourSteps; }
        private set
        {
            if (_currentHourSteps != value)
            {
                _currentHourSteps = value;

                OnPropertyChanged(nameof(CurrentHourSteps));
            }
        }
    }

    public int CurrentDaySteps
    {
        get { return _currentDaySteps; }
        private set
        {
            if (_currentDaySteps != value)
            {
                _currentDaySteps = value;

                OnPropertyChanged(nameof(CurrentDaySteps));
            }
        }
    }

    private DateTime _lastStepsDateTime = DateTime.MinValue;

    public ActivityTracker(IPedometer pedometer, IActivityStatsDataAccess activityStatsDataAccess)
    {
        _pedometer = pedometer;
        _activityStatsDataAccess = activityStatsDataAccess;

        var _lastStepsDateTime = DateTime.Now;
        var currentDay = DateOnly.FromDateTime(_lastStepsDateTime);
        var currentHour = _lastStepsDateTime.Hour;

        CurrentHourSteps = activityStatsDataAccess.LoadHourlySteps(currentDay, currentHour).Steps;
        CurrentDaySteps = activityStatsDataAccess.LoadDailySteps(currentDay).Steps;

        _pedometer.StepsRegistered += NewStepsRegistered;
    }

    ~ActivityTracker()
    {
        SaveHourlySteps();
        CurrentHourSteps = 0;
        CurrentDaySteps = 0;
    }

    public void Start() => _pedometer.Start();
    public void Stop() => _pedometer.Stop();
    public bool IsActive => _pedometer.IsActive;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void NewStepsRegistered(object? sender, int newSteps)
    {
        var now = DateTime.Now;

        if (CurrentHourSteps > 0)
        {
            if (now.Hour > _lastStepsDateTime.Hour ||
                now.Day > _lastStepsDateTime.Day ||
                now.Month > _lastStepsDateTime.Month ||
                now.Year > _lastStepsDateTime.Year
                )
            {
                SaveHourlySteps();
                CurrentHourSteps = 0;
            }
        }

        if (CurrentDaySteps > 0)
        {
            if (now.Day > _lastStepsDateTime.Day ||
                now.Month > _lastStepsDateTime.Month ||
                now.Year > _lastStepsDateTime.Year
                )
            {
                CurrentDaySteps = 0;
            }
        }

        CurrentHourSteps += newSteps;
        CurrentDaySteps += newSteps;

        _lastStepsDateTime = now;
    }

    private void SaveHourlySteps()
    {
        HourlySteps hourlySteps = new(
            DateOnly.FromDateTime(_lastStepsDateTime),
            _lastStepsDateTime.Hour,
            CurrentHourSteps
            );

        _activityStatsDataAccess.SaveHourlySteps(hourlySteps);
    }
}