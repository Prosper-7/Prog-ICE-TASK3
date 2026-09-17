namespace MachineTelemetry.Shared;

/// <summary>
/// Generic wrapper used to store a single sensor value (of any type T) together
/// with the date/time it was captured. One MachineReading is made up of three
/// of these: TelemetryReading&lt;double&gt; for temperature, TelemetryReading&lt;int&gt;
/// for rotation speed and TelemetryReading&lt;bool&gt; for operational state.
/// </summary>
/// <typeparam name="T">The type of the sensor value (double, int, bool, etc.)</typeparam>
public class TelemetryReading<T>
{
    public T Value { get; set; }

    public DateTime Timestamp { get; set; }

    public TelemetryReading()
    {
        Value = default!;
    }

    public TelemetryReading(T value, DateTime timestamp)
    {
        Value = value;
        Timestamp = timestamp;
    }

    public override string ToString() => $"{Value} @ {Timestamp:G}";
}
