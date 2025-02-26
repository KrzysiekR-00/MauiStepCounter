using ActivityCore.Abstraction;

namespace ActivityCore.Stats;
public class ActivityStatsFileSystemDataAccess : IActivityStatsDataAccess
{
    private readonly string _appDataDirectory;

    public ActivityStatsFileSystemDataAccess(string appDataDirectory)
    {
        _appDataDirectory = appDataDirectory;
    }

    public void SaveHourlySteps(HourlySteps hourlySteps)
    {
        var filePath = GetFilePath(hourlySteps.Date);

        var hourUniqueId = HourUniqueId(hourlySteps.Hour);

        if (File.Exists(_appDataDirectory))
        {
            var lines = File.ReadAllLines(filePath);
            var linesToRemove = lines.Where(l => l.StartsWith(hourUniqueId));
            if (linesToRemove.Any())
            {
                lines = lines.Except(linesToRemove).ToArray();
                File.WriteAllLines(filePath, lines);
            }
        }

        string line = hourUniqueId + "," + hourlySteps.Steps.ToString() + Environment.NewLine;

        File.AppendAllText(filePath, line);
    }

    private string HourUniqueId(int hour)
    {
        return hour.ToString().PadLeft(2, '0'); ;
    }

    private string GetFilePath(DateOnly date)
    {
        string fileName = date.ToString("yyyyMMdd") + "activity.txt";
        return Path.Combine(_appDataDirectory, fileName);
    }

    public HourlySteps LoadHourlySteps(DateOnly date, int hour)
    {
        throw new NotImplementedException();
    }

    public DailySteps LoadDailySteps(DateOnly date)
    {
        throw new NotImplementedException();
    }
}
