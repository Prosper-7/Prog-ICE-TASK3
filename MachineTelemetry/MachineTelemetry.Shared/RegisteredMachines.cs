namespace MachineTelemetry.Shared;

/// <summary>
/// Holds the factory's registered machines in a two-dimensional array.
/// Each row is one machine; the columns are [0] = ID, [1] = Name, [2] = Location.
/// </summary>
public static class RegisteredMachines
{
    public static readonly string[,] Machines =
    {
        { "MCH-001", "Conveyor Belt Alpha", "Assembly Line 1" },
        { "MCH-002", "Hydraulic Press Beta", "Assembly Line 2" },
        { "MCH-003", "Robotic Arm Gamma", "Packaging Bay" },
        { "MCH-004", "CNC Mill Delta", "Machining Floor" }
    };

    public const int IdColumn = 0;
    public const int NameColumn = 1;
    public const int LocationColumn = 2;

    /// <summary>Number of registered machines (rows in the array).</summary>
    public static int Count => Machines.GetLength(0);
}
