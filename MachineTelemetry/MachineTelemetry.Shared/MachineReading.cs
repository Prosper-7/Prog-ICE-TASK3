namespace MachineTelemetry.Shared;

/// <summary>
/// A single reading captured from one factory machine. Each sensor value is
/// stored using the generic TelemetryReading&lt;T&gt; class so the correct
/// value type (double / int / bool) is preserved for each sensor.
/// </summary>
public class MachineReading
{
    public string MachineId { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public TelemetryReading<double> Temperature { get; set; } = new();
    public TelemetryReading<int> RotationSpeed { get; set; } = new();
    public TelemetryReading<bool> OperationalState { get; set; } = new();

    /// <summary>Date and time the reading was taken.</summary>
    public DateTime ReadingDateTime { get; set; }

    public MachineReading()
    {
    }

    public MachineReading(
        string machineId,
        string machineName,
        string location,
        double temperature,
        int rotationSpeed,
        bool operationalState,
        DateTime readingDateTime)
    {
        MachineId = machineId;
        MachineName = machineName;
        Location = location;
        ReadingDateTime = readingDateTime;

        Temperature = new TelemetryReading<double>(temperature, readingDateTime);
        RotationSpeed = new TelemetryReading<int>(rotationSpeed, readingDateTime);
        OperationalState = new TelemetryReading<bool>(operationalState, readingDateTime);
    }

    /// <summary>
    /// Overloaded '&gt;' operator: compares two machine readings by temperature.
    /// Used by MachineAnalyzer.FindHottestMachine to identify the hottest machine.
    /// </summary>
    public static bool operator >(MachineReading left, MachineReading right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.Temperature.Value > right.Temperature.Value;
    }

    /// <summary>
    /// Overloaded '&lt;' operator: compares two machine readings by temperature.
    /// C# requires '&lt;' to be defined whenever '&gt;' is overloaded.
    /// </summary>
    public static bool operator <(MachineReading left, MachineReading right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.Temperature.Value < right.Temperature.Value;
    }

    public override string ToString() =>
        $"{MachineId} | {MachineName} | {Location} | {Temperature.Value:F1}°C | " +
        $"{RotationSpeed.Value} RPM | Operational: {OperationalState.Value} | {ReadingDateTime:G}";
}
