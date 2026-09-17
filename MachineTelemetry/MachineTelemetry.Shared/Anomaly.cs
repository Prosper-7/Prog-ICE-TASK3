namespace MachineTelemetry.Shared;

/// <summary>
/// Applies the factory's anomaly rules to machine readings:
///   - temperature above 85 °C is an anomaly;
///   - rotation speed above 4 500 RPM is an anomaly; and
///   - a machine that is not operational is an anomaly.
/// </summary>
public static class Anomaly
{
    public const double TemperatureThresholdCelsius = 85.0;
    public const int RotationSpeedThresholdRpm = 4500;

    /// <summary>True if the given reading breaks any of the anomaly rules.</summary>
    public static bool IsAnomaly(MachineReading reading)
    {
        ArgumentNullException.ThrowIfNull(reading);

        bool overheating = reading.Temperature.Value > TemperatureThresholdCelsius;
        bool overSpeeding = reading.RotationSpeed.Value > RotationSpeedThresholdRpm;
        bool notOperational = !reading.OperationalState.Value;

        return overheating || overSpeeding || notOperational;
    }

    /// <summary>
    /// Recursively counts how many readings in the list are anomalies.
    /// Base case: index has reached the end of the list -> 0.
    /// Recursive case: 1 (if this reading is an anomaly) + the count of the rest of the list.
    /// </summary>
    public static int CountAnomalies(IReadOnlyList<MachineReading> readings, int index = 0)
    {
        if (readings is null || index >= readings.Count)
        {
            return 0; // base case
        }

        int countedHere = IsAnomaly(readings[index]) ? 1 : 0;
        return countedHere + CountAnomalies(readings, index + 1); // recursive case
    }
}
