namespace MachineTelemetry.Shared;

/// <summary>
/// Analyses a set of machine readings using recursion.
/// </summary>
public static class MachineAnalyzer
{
    /// <summary>
    /// Recursively walks the list of readings and returns the one with the highest
    /// temperature, using the overloaded '&gt;' operator defined on MachineReading
    /// to compare readings.
    /// </summary>
    public static MachineReading? FindHottestMachine(
        IReadOnlyList<MachineReading> readings,
        int index = 0,
        MachineReading? hottestSoFar = null)
    {
        if (readings is null || readings.Count == 0)
        {
            return null;
        }

        hottestSoFar ??= readings[0];

        if (index >= readings.Count)
        {
            return hottestSoFar; // base case: reached the end of the list
        }

        // Uses the overloaded '>' operator to compare by temperature.
        if (readings[index] > hottestSoFar)
        {
            hottestSoFar = readings[index];
        }

        return FindHottestMachine(readings, index + 1, hottestSoFar); // recursive case
    }
}
