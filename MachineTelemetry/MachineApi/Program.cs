using MachineTelemetry.Shared;

var builder = WebApplication.CreateBuilder(args);

// Allow the WinForms client (or any local client) to call this API.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

var app = builder.Build();
app.UseCors();

var random = new Random();

// Generates a brand-new, randomized reading for every registered machine.
// This is the ONLY place readings are generated - the WinForms client never
// generates its own readings, it only requests them from here.
List<MachineReading> GenerateReadings()
{
    var readings = new List<MachineReading>();
    var now = DateTime.Now;

    for (int i = 0; i < RegisteredMachines.Count; i++)
    {
        string id = RegisteredMachines.Machines[i, RegisteredMachines.IdColumn];
        string name = RegisteredMachines.Machines[i, RegisteredMachines.NameColumn];
        string location = RegisteredMachines.Machines[i, RegisteredMachines.LocationColumn];

        // Ranges are chosen so that anomalies (temp > 85°C, speed > 4500 RPM,
        // or not operational) show up from time to time, but are not guaranteed.
        double temperature = Math.Round(random.NextDouble() * 100, 1);      // 0.0 - 100.0 °C
        int rotationSpeed = random.Next(0, 5200);                          // 0 - 5199 RPM
        bool operational = random.NextDouble() > 0.1;                      // ~90% operational

        readings.Add(new MachineReading(id, name, location, temperature, rotationSpeed, operational, now));
    }

    return readings;
}

app.MapGet("/api/readings", () =>
{
    List<MachineReading> readings = GenerateReadings();
    return Results.Ok(readings);
})
.WithName("GetMachineReadings")
.WithDescription("Generates and returns a new randomized reading for every registered machine.");

app.MapGet("/", () => "Machine Telemetry API is running. Try GET /api/readings.");

app.Run();
