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

        string line = Serialize(hourUniqueId, hourlySteps.Steps);

        File.AppendAllText(filePath, line);
    }

    public HourlySteps LoadHourlySteps(DateOnly date, int hour)
    {
        var filePath = GetFilePath(date);

        if (!File.Exists(_appDataDirectory))
        {
            return new HourlySteps(date, hour, 0);
        }

        var hourUniqueId = HourUniqueId(hour);

        var line = File.ReadAllLines(filePath).Where(l => l.StartsWith(hourUniqueId)).FirstOrDefault();

        if (line != null)
        {
            if (TryDeserializeStepsValue(line, out int steps))
            {
                return new HourlySteps(date, hour, steps);
            }
        }

        return new HourlySteps(date, hour, 0);
    }

    public DailySteps LoadDailySteps(DateOnly date)
    {
        var filePath = GetFilePath(date);

        if (!File.Exists(_appDataDirectory))
        {
            return new DailySteps(date, 0);
        }

        var lines = File.ReadAllLines(filePath);

        int steps = 0;

        foreach (var line in lines)
        {
            if (TryDeserializeStepsValue(line, out int hourlySteps))
            {
                steps += hourlySteps;
            }
        }

        return new DailySteps(date, steps);
    }

    private static string HourUniqueId(int hour)
    {
        return hour.ToString().PadLeft(2, '0'); ;
    }

    private static string Serialize(string hourUniqueId, int steps)
    {
        return hourUniqueId + "," + steps.ToString() + Environment.NewLine;
    }

    private static bool TryDeserializeStepsValue(string line, out int steps)
    {
        var parts = line.Split(',');
        if (parts.Length == 2)
        {
            if (int.TryParse(parts[1], out int _steps))
            {
                steps = _steps;
                return true;
            }
        }

        steps = 0;
        return false;
    }

    private string GetFilePath(DateOnly date)
    {
        string fileName = date.ToString("yyyyMMdd") + "activity.txt";
        return Path.Combine(_appDataDirectory, fileName);
    }
}
