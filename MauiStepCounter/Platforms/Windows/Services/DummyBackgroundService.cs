using MauiStepCounter.Services;

namespace MauiStepCounter.Platforms.Windows.Services;
public class DummyBackgroundService : IBackgroundService
{
    public bool IsActive { get; private set; } = false;

    public void Start()
    {
        IsActive = true;
    }

    public void Stop()
    {
        IsActive = false;
    }
}
