using SharpDX.Direct3D11;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PatheonParser;

public partial class MainForm : Form
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CaptureManager? CaptureManager { get; set; } = new CaptureManager();

    private Queue<string> mLogQueue = new Queue<string>();
    private int maxLines = 100; // Maximum lines before rolling
    private static bool isAlreadyRunning = false;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static MainForm? Instance { get; set; } = null;

    public MainForm()
    {
        InitializeComponent();
        EncounterTimeoutTextBox.Text = Properties.Settings.Default.EncounterTimeout.ToString();
        Instance = this;
    }

    public void AddLog(string message)
    {
        if (mLogQueue.Count >= maxLines)
        {
            mLogQueue.Dequeue(); // Remove oldest entry
        }

        mLogQueue.Enqueue(message); // Add new log

        // Update TextBox
        LogTextBox.Lines = mLogQueue.ToArray();
        LogTextBox.SelectionStart = LogTextBox.Text.Length;
        LogTextBox.ScrollToCaret(); // Auto-scroll to the latest entry

        RefreshEncounters();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        DamageChart.Series.Clear();
        HealChart.Series.Clear();
    }

    private async void timer1_Tick(object sender, EventArgs e)
    {
        if (!isAlreadyRunning)
        {
            isAlreadyRunning = true;
            await Task.Run(() =>
            {
                if (CaptureManager is not null)
                {
                    CaptureManager.CheckForChanges();
                }
            });
            isAlreadyRunning = false;
        }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        isAlreadyRunning = true;
        timer1.Stop();

        if (CaptureManager is not null)
        {
            CaptureManager.Dispose();
        }

        CaptureManager = null;

        Properties.Settings.Default.Save();
    }

    public DataTable GetEncounters()
    {
        using var connection = new SQLiteConnection("Data Source=PatheonParser.db");
        connection.Open();
        using var command = new SQLiteCommand("SELECT id FROM encounters ORDER BY id DESC", connection);

        using var dapter = new SQLiteDataAdapter(command);
        var table = new DataTable();
        dapter.Fill(table);
        return table;
    }

    public DataTable GetAttackStats(int encounterId)
    {
        using var connection = new SQLiteConnection("Data Source=PatheonParser.db");
        connection.Open();
        using var command = new SQLiteCommand("SELECT source_name,SUM(damage_amount) as amount FROM attacks WHERE encounter_id = @EncounterId GROUP BY source_name", connection);
        command.Parameters.AddWithValue("@EncounterId", encounterId);
        using var dapter = new SQLiteDataAdapter(command);
        var table = new DataTable();
        dapter.Fill(table);
        return table;
    }

    public DataTable GetHealStats(int encounterId)
    {
        using var connection = new SQLiteConnection("Data Source=PatheonParser.db");
        connection.Open();
        using var command = new SQLiteCommand("SELECT source_name,SUM(amount) as amount FROM heals WHERE encounter_id = @EncounterId GROUP BY source_name", connection);
        command.Parameters.AddWithValue("@EncounterId", encounterId);
        using var dapter = new SQLiteDataAdapter(command);
        var table = new DataTable();
        dapter.Fill(table);
        return table;
    }

    private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MessageBox.Show("There is no help yet", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void EncounterTimeoutTextBox_TextChanged(object sender, EventArgs e)
    {
        if (int.TryParse(EncounterTimeoutTextBox.Text, out var result))
        {
            Properties.Settings.Default.EncounterTimeout = result;
        }
    }

    public void RefreshEncounters()
    {
        var oldValue = EncounterListBox.SelectedValue;
        EncounterListBox.DisplayMember = "id";
        EncounterListBox.DataSource = GetEncounters();

        if (EncounterListBox.SelectedValue is DataRowView dataRow)
        {
            if (int.TryParse(dataRow.Row.ItemArray[0]?.ToString(), out var result))
            {
                RefreshCharts(result);
            }
        }
    }

    public void RefreshCharts(int encounterId)
    {
        DamageChart.Series.Clear();
        foreach (DataRow row in GetAttackStats(encounterId).Rows)
        {
            Series series = new Series(row["source_name"].ToString())
            {
                ChartType = SeriesChartType.Column
            };

            series.Points.AddXY(row["source_name"].ToString(), row["amount"]);
            series.LegendText = row["source_name"].ToString();
            DamageChart.Series.Add(series);
        }

        HealChart.Series.Clear();
        foreach (DataRow row in GetHealStats(encounterId).Rows)
        {
            Series series = new Series(row["source_name"].ToString())
            {
                ChartType = SeriesChartType.Column
            };

            series.Points.AddXY(row["source_name"].ToString(), row["amount"]);
            series.LegendText = row["source_name"].ToString();
            HealChart.Series.Add(series);
        }
    }

    public void RefreshCharts()
    {
        if (int.TryParse(EncounterListBox.SelectedValue?.ToString(), out var result))
        {
            RefreshCharts(result);
        }
    }

    private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
    {
        RefreshEncounters();
    }

    private void EncounterListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(EncounterListBox.SelectedValue is DataRowView dataRow)
        { 
            if (int.TryParse(dataRow.Row.ItemArray[0]?.ToString(), out var result))
            {
                RefreshCharts(result);
            }
        }
    }
}