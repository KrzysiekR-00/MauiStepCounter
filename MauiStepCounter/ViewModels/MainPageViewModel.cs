using ActivityCore;
using ActivityCore.Abstraction;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiStepCounter.Abstraction;
using System.Globalization;

namespace MauiStepCounter.ViewModels;
public partial class MainPageViewModel : ObservableObject
{
    public ActivityTracker ActivityTracker { get; }

    [ObservableProperty]
    private string _log = "";

    private readonly IBackgroundService _backgroundService;
    private readonly INotificationService _notificationService;
    private readonly IActivityStatsDataAccess _activityStatsDataAccess;

    public MainPageViewModel(
        IBackgroundService backgroundService,
        INotificationService notificationService,
        ActivityTracker activityTracker,
        IActivityStatsDataAccess activityStatsDataAccess
        )
    {
        _backgroundService = backgroundService;
        _notificationService = notificationService;
        _activityStatsDataAccess = activityStatsDataAccess;

        ActivityTracker = activityTracker;
        ActivityTracker.PropertyChanged += (_, _) =>
        {
            //_backgroundService
            _notificationService.Show("Today steps", ActivityTracker.CurrentDaySteps.ToString());
        };

        var today = DateTime.Now;
        int daysToShowStats = 3;
        WriteLogLine("Registered steps:");
        for (int i = 0; i <= daysToShowStats; i++)
        {
            var day = DateOnly.FromDateTime(today.AddDays(-i));

            var steps = _activityStatsDataAccess.LoadDailySteps(day).Steps;

            WriteLogLine($"{day} - {steps}");
        }
    }

    [RelayCommand]
    private void ChangeLanguage()
    {
        CultureInfo? newCulture;

        if (CultureInfo.CurrentCulture.Name.Contains("en", StringComparison.InvariantCultureIgnoreCase))
        {
            newCulture = CultureInfo.CreateSpecificCulture("pl-PL");
        }
        else
        {
            newCulture = CultureInfo.CreateSpecificCulture("en-US");
        }

        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = Thread.CurrentThread.CurrentUICulture = newCulture;
    }

    [RelayCommand]
    private void StartActivityTracker()
    {
        _backgroundService.Start();

        WriteLogLine("Background service started");

        ActivityTracker.Start();

        WriteLogLine("Activity tracker started");
    }

    [RelayCommand]
    private void StopActivityTracker()
    {
        ActivityTracker.Start();

        WriteLogLine("Activity tracker stopped");

        _backgroundService.Stop();

        WriteLogLine("Background service stopped");
    }

    private void WriteLogLine(string message)
    {
        if (!string.IsNullOrEmpty(Log)) Log += Environment.NewLine;

        Log +=
            TimeOnly.FromDateTime(DateTime.Now).ToString("O") +
            " - " +
            message;
    }

}
