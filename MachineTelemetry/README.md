# Machine Telemetry Solution

A 3-project .NET 8 solution:

- **MachineTelemetry.Shared** — class library with the domain model, shared by both apps.
- **MachineApi** — ASP.NET Core Minimal API that generates the readings.
- **MachineMonitorWinForms** — Windows Forms app that requests and displays readings.

## How to run

1. Open `MachineTelemetry.sln` in Visual Studio (2022+, with the ASP.NET and
   .NET Desktop Development workloads) — or use the .NET 8 SDK from the CLI.
2. Start **MachineApi** first (F5, or `dotnet run --project MachineApi`).
   Note the port it launches on (default `http://localhost:5250`,
   see `MachineApi/Properties/launchSettings.json`).
3. Start **MachineMonitorWinForms**. If the API is running on a different
   port, update the `BaseAddress` in `MainForm.cs`.
4. Click **"Get New Readings"** to request a fresh, randomized set of
   readings from the API.

## Requirement → code attribution

| # | Requirement | Implemented in |
|---|---|---|
| 1 | ASP.NET Core Minimal API | `MachineApi/Program.cs` |
| 2 | Readings for ≥3 machines | `RegisteredMachines.Machines` has 4 rows; `Program.cs GenerateReadings()` loops over all of them |
| 3 | Each reading has ID, name, location, temperature (double), rotation speed (int), operational state (bool), date/time | `MachineTelemetry.Shared/MachineReading.cs` |
| 4 | Generic `TelemetryReading<T>` class | `MachineTelemetry.Shared/TelemetryReading.cs`, used inside `MachineReading` for `Temperature` (`T=double`), `RotationSpeed` (`T=int`), `OperationalState` (`T=bool`) |
| 5 | Registered machines in a 2-D array (ID, name, location) | `MachineTelemetry.Shared/RegisteredMachines.cs` (`string[,] Machines`) |
| 6 | Generated readings stored in a `List` | `Program.cs GenerateReadings()` returns `List<MachineReading>` |
| 7 | `Anomaly` class with the three rules (>85 °C, >4500 RPM, not operational) | `MachineTelemetry.Shared/Anomaly.cs` → `IsAnomaly()` |
| 8 | Recursive method counting anomalies in the reading list | `Anomaly.cs` → `CountAnomalies(readings, index)` |
| 9 | Overloaded `>` and `<` operators comparing readings by temperature | `MachineReading.cs` → `operator >` / `operator <` |
| 10 | Recursion + the overloaded operator to find the hottest machine | `MachineTelemetry.Shared/MachineAnalyzer.cs` → `FindHottestMachine()` |
| 11 | WinForms app using `HttpClient` to request readings from the API | `MachineMonitorWinForms/MainForm.cs` → `LoadReadingsAsync()` |
| 12 | All readings + anomaly info shown in a `DataGridView` | `MainForm.cs` → `DisplayReadings()` |
| 13 | Total anomaly count + hottest machine's name shown | `MainForm.cs` → `_anomalyCountLabel` / `_hottestMachineLabel`, set inside `DisplayReadings()` |
| 14 | Button requests a new randomized set of readings from the API (WinForms never generates readings itself) | `MainForm.cs` → `_refreshButton.Click` → `LoadReadingsAsync()`; all randomization happens only in `MachineApi/Program.cs GenerateReadings()` |

## Notes

- CORS is enabled on the API (`AllowAnyOrigin`) purely so the WinForms
  client (or a browser) can call it locally during development.
- Temperature is generated in the range 0–100 °C and rotation speed in the
  range 0–5199 RPM so anomalies appear some of the time but are not
  guaranteed on every request — click "Get New Readings" a few times to see
  anomaly rows (highlighted in pink) appear.
- This solution was not compiled in this environment (no .NET SDK was
  available here), so double-check it builds cleanly in Visual Studio /
  `dotnet build` before submitting, and adjust the API port in `MainForm.cs`
  if yours differs from the default.
