using ActivityCore.Abstraction;

namespace MauiStepCounter.Platforms.Windows.Services;
internal class DummyPedometerService : IPedometer
{
    public event EventHandler<int>? StepsRegistered;

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
