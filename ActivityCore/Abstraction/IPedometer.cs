namespace ActivityCore.Abstraction;
public interface IPedometer
{
    event EventHandler<int>? StepsRegistered;

    bool IsActive { get; }

    void Start();
    void Stop();
}
