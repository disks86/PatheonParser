using System.ComponentModel;

namespace PatheonParser;

public partial class MainForm : Form
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CaptureManager? CaptureManager { get; set; } = new CaptureManager();

    private Queue<string> mLogQueue = new Queue<string>();
    private int maxLines = 100; // Maximum lines before rolling

    public MainForm()
    {
        InitializeComponent();
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
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {

    }

    private async void timer1_Tick(object sender, EventArgs e)
    {
        await Task.Run(() =>
        {
            if (CaptureManager is not null)
            {
                CaptureManager.CheckForChanges();
            }
        });
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        timer1.Stop();

        if (CaptureManager is not null)
        {
            CaptureManager.Dispose();
        }

        CaptureManager = null;
    }
}