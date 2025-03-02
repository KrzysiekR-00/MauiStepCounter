using Plugin.Maui.Pedometer;

namespace MauiStepCounter.Platforms.Android.Services;
internal class PedometerService : ActivityCore.Abstraction.IPedometer
{
    public event EventHandler<int>? StepsRegistered;

    private readonly IPedometer _pedometer;

    private int _steps = 0;

    public bool IsActive { get; private set; }

    public PedometerService(IPedometer pedometer)
    {
        _pedometer = pedometer;
    }

    public void Start()
    {
        if (_pedometer.IsSupported && !_pedometer.IsMonitoring)
        {
            _pedometer.ReadingChanged += PedometerReadingChanged;

            _pedometer.Start();

            IsActive = true;
        }
    }

    public void Stop()
    {
        if (_pedometer.IsSupported && _pedometer.IsMonitoring)
        {
            _pedometer.ReadingChanged -= PedometerReadingChanged;

            _pedometer.Stop();

            IsActive = false;
        }
    }

    private void PedometerReadingChanged(object? sender, PedometerData pedometerData)
    {
        var stepsDelta = pedometerData.NumberOfSteps - _steps;
        if (stepsDelta > 0)
        {
            _steps = pedometerData.NumberOfSteps;
            StepsRegistered?.Invoke(this, stepsDelta);
        }
    }
}
