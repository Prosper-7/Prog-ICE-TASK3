using System.Net.Http.Json;
using MachineTelemetry.Shared;

namespace MachineMonitorWinForms;

/// <summary>
/// Windows Forms client. It never generates machine readings itself - every
/// set of readings shown here was requested from the Minimal API over HTTP.
/// </summary>
public class MainForm : Form
{
    // Change this if your API runs on a different port (see MachineApi's
    // Properties/launchSettings.json, "applicationUrl").
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5250/")
    };

    private DataGridView _grid = null!;
    private Button _refreshButton = null!;
    private Label _anomalyCountLabel = null!;
    private Label _hottestMachineLabel = null!;
    private Label _statusLabel = null!;

    public MainForm()
    {
        BuildUi();
        Load += async (_, _) => await LoadReadingsAsync();
    }

    private void BuildUi()
    {
        Text = "Factory Machine Monitor";
        Width = 940;
        Height = 600;
        StartPosition = FormStartPosition.CenterScreen;

        _refreshButton = new Button
        {
            Text = "Get New Readings",
            Left = 20,
            Top = 20,
            Width = 170,
            Height = 32
        };
        _refreshButton.Click += async (_, _) => await LoadReadingsAsync();

        _anomalyCountLabel = new Label
        {
            Left = 210,
            Top = 27,
            Width = 220,
            Text = "Anomalies: -",
            Font = new Font(Font, FontStyle.Bold)
        };

        _hottestMachineLabel = new Label
        {
            Left = 440,
            Top = 27,
            Width = 460,
            Text = "Hottest machine: -",
            Font = new Font(Font, FontStyle.Bold)
        };

        _statusLabel = new Label
        {
            Left = 20,
            Top = 60,
            Width = 880,
            Text = string.Empty,
            ForeColor = Color.DarkRed
        };

        _grid = new DataGridView
        {
            Left = 20,
            Top = 90,
            Width = 880,
            Height = 460,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        Controls.Add(_refreshButton);
        Controls.Add(_anomalyCountLabel);
        Controls.Add(_hottestMachineLabel);
        Controls.Add(_statusLabel);
        Controls.Add(_grid);
    }

    /// <summary>
    /// Requests a fresh, randomized set of readings from the API and refreshes the UI.
    /// Fired both on form load and when the user clicks "Get New Readings".
    /// </summary>
    private async Task LoadReadingsAsync()
    {
        _refreshButton.Enabled = false;
        _statusLabel.Text = "Requesting readings from API...";

        try
        {
            var readings = await _httpClient.GetFromJsonAsync<List<MachineReading>>("api/readings");
            readings ??= new List<MachineReading>();

            DisplayReadings(readings);
            _statusLabel.Text = $"Loaded {readings.Count} reading(s) at {DateTime.Now:T}.";
        }
        catch (Exception ex)
        {
            _statusLabel.Text = $"Could not reach the API: {ex.Message}";
        }
        finally
        {
            _refreshButton.Enabled = true;
        }
    }

    private void DisplayReadings(List<MachineReading> readings)
    {
        _grid.Rows.Clear();
        _grid.Columns.Clear();

        _grid.Columns.Add("MachineId", "Machine ID");
        _grid.Columns.Add("MachineName", "Machine Name");
        _grid.Columns.Add("Location", "Location");
        _grid.Columns.Add("Temperature", "Temperature (°C)");
        _grid.Columns.Add("RotationSpeed", "Rotation Speed (RPM)");
        _grid.Columns.Add("Operational", "Operational");
        _grid.Columns.Add("ReadingDateTime", "Reading Date/Time");
        _grid.Columns.Add("Anomaly", "Anomaly?");

        foreach (var reading in readings)
        {
            bool isAnomaly = Anomaly.IsAnomaly(reading);

            int rowIndex = _grid.Rows.Add(
                reading.MachineId,
                reading.MachineName,
                reading.Location,
                reading.Temperature.Value.ToString("F1"),
                reading.RotationSpeed.Value,
                reading.OperationalState.Value ? "Yes" : "No",
                reading.ReadingDateTime.ToString("G"),
                isAnomaly ? "Yes" : "No");

            if (isAnomaly)
            {
                _grid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
            }
        }

        // Recursive method that counts all anomalies in the reading list.
        int anomalyCount = Anomaly.CountAnomalies(readings);
        _anomalyCountLabel.Text = $"Anomalies: {anomalyCount}";

        // Recursion + the overloaded '>' operator, used to find the hottest machine.
        MachineReading? hottest = MachineAnalyzer.FindHottestMachine(readings);
        _hottestMachineLabel.Text = hottest is null
            ? "Hottest machine: -"
            : $"Hottest machine: {hottest.MachineName} ({hottest.Temperature.Value:F1} °C)";
    }
}
