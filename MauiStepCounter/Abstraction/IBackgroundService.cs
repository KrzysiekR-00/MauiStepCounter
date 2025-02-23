namespace MauiStepCounter.Abstraction;

public interface IBackgroundService
{
    bool IsActive { get; }

    void Start();
    void Stop();
}
