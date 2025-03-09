namespace PatheonParser;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
        System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
        System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
        System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
        menuStrip1 = new MenuStrip();
        fileToolStripMenuItem = new ToolStripMenuItem();
        exitToolStripMenuItem = new ToolStripMenuItem();
        toolsToolStripMenuItem = new ToolStripMenuItem();
        refreshToolStripMenuItem = new ToolStripMenuItem();
        optionsToolStripMenuItem = new ToolStripMenuItem();
        encounterTimeoutToolStripMenuItem = new ToolStripMenuItem();
        EncounterTimeoutTextBox = new ToolStripTextBox();
        helpToolStripMenuItem = new ToolStripMenuItem();
        aboutToolStripMenuItem = new ToolStripMenuItem();
        splitContainer1 = new SplitContainer();
        splitContainer2 = new SplitContainer();
        EncounterListBox = new ListBox();
        splitContainer3 = new SplitContainer();
        DamageChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
        HealChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
        LogTextBox = new TextBox();
        timer1 = new System.Windows.Forms.Timer(components);
        menuStrip1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
        splitContainer2.Panel1.SuspendLayout();
        splitContainer2.Panel2.SuspendLayout();
        splitContainer2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
        splitContainer3.Panel1.SuspendLayout();
        splitContainer3.Panel2.SuspendLayout();
        splitContainer3.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)DamageChart).BeginInit();
        ((System.ComponentModel.ISupportInitialize)HealChart).BeginInit();
        SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(892, 24);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // fileToolStripMenuItem
        // 
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(37, 20);
        fileToolStripMenuItem.Text = "&File";
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(92, 22);
        exitToolStripMenuItem.Text = "E&xit";
        exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
        // 
        // toolsToolStripMenuItem
        // 
        toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { refreshToolStripMenuItem, optionsToolStripMenuItem });
        toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
        toolsToolStripMenuItem.Size = new Size(47, 20);
        toolsToolStripMenuItem.Text = "&Tools";
        // 
        // refreshToolStripMenuItem
        // 
        refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
        refreshToolStripMenuItem.Size = new Size(116, 22);
        refreshToolStripMenuItem.Text = "Refresh";
        refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
        // 
        // optionsToolStripMenuItem
        // 
        optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { encounterTimeoutToolStripMenuItem });
        optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
        optionsToolStripMenuItem.Size = new Size(116, 22);
        optionsToolStripMenuItem.Text = "&Options";
        optionsToolStripMenuItem.Click += optionsToolStripMenuItem_Click;
        // 
        // encounterTimeoutToolStripMenuItem
        // 
        encounterTimeoutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { EncounterTimeoutTextBox });
        encounterTimeoutToolStripMenuItem.Name = "encounterTimeoutToolStripMenuItem";
        encounterTimeoutToolStripMenuItem.Size = new Size(176, 22);
        encounterTimeoutToolStripMenuItem.Text = "Encounter Timeout";
        // 
        // EncounterTimeoutTextBox
        // 
        EncounterTimeoutTextBox.Name = "EncounterTimeoutTextBox";
        EncounterTimeoutTextBox.Size = new Size(100, 23);
        EncounterTimeoutTextBox.ToolTipText = "The enounter timeout in seconds.";
        EncounterTimeoutTextBox.TextChanged += EncounterTimeoutTextBox_TextChanged;
        // 
        // helpToolStripMenuItem
        // 
        helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
        helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        helpToolStripMenuItem.Size = new Size(44, 20);
        helpToolStripMenuItem.Text = "&Help";
        // 
        // aboutToolStripMenuItem
        // 
        aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        aboutToolStripMenuItem.Size = new Size(116, 22);
        aboutToolStripMenuItem.Text = "&About...";
        aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
        // 
        // splitContainer1
        // 
        splitContainer1.Dock = DockStyle.Fill;
        splitContainer1.Location = new Point(0, 24);
        splitContainer1.Name = "splitContainer1";
        splitContainer1.Orientation = Orientation.Horizontal;
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(splitContainer2);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(LogTextBox);
        splitContainer1.Size = new Size(892, 594);
        splitContainer1.SplitterDistance = 431;
        splitContainer1.TabIndex = 1;
        // 
        // splitContainer2
        // 
        splitContainer2.Dock = DockStyle.Fill;
        splitContainer2.Location = new Point(0, 0);
        splitContainer2.Name = "splitContainer2";
        // 
        // splitContainer2.Panel1
        // 
        splitContainer2.Panel1.Controls.Add(EncounterListBox);
        // 
        // splitContainer2.Panel2
        // 
        splitContainer2.Panel2.Controls.Add(splitContainer3);
        splitContainer2.Size = new Size(892, 431);
        splitContainer2.SplitterDistance = 244;
        splitContainer2.TabIndex = 0;
        // 
        // EncounterListBox
        // 
        EncounterListBox.Dock = DockStyle.Fill;
        EncounterListBox.FormattingEnabled = true;
        EncounterListBox.Location = new Point(0, 0);
        EncounterListBox.Name = "EncounterListBox";
        EncounterListBox.Size = new Size(244, 431);
        EncounterListBox.TabIndex = 0;
        EncounterListBox.SelectedIndexChanged += EncounterListBox_SelectedIndexChanged;
        // 
        // splitContainer3
        // 
        splitContainer3.Dock = DockStyle.Fill;
        splitContainer3.Location = new Point(0, 0);
        splitContainer3.Name = "splitContainer3";
        splitContainer3.Orientation = Orientation.Horizontal;
        // 
        // splitContainer3.Panel1
        // 
        splitContainer3.Panel1.Controls.Add(DamageChart);
        // 
        // splitContainer3.Panel2
        // 
        splitContainer3.Panel2.Controls.Add(HealChart);
        splitContainer3.Size = new Size(644, 431);
        splitContainer3.SplitterDistance = 214;
        splitContainer3.TabIndex = 0;
        // 
        // DamageChart
        // 
        chartArea1.Name = "ChartArea1";
        DamageChart.ChartAreas.Add(chartArea1);
        DamageChart.Dock = DockStyle.Fill;
        legend1.Name = "Legend1";
        DamageChart.Legends.Add(legend1);
        DamageChart.Location = new Point(0, 0);
        DamageChart.Name = "DamageChart";
        DamageChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Fire;
        DamageChart.Size = new Size(644, 214);
        DamageChart.TabIndex = 0;
        DamageChart.Text = "Damage Chart";
        // 
        // HealChart
        // 
        chartArea2.Name = "ChartArea1";
        HealChart.ChartAreas.Add(chartArea2);
        HealChart.Dock = DockStyle.Fill;
        legend2.Name = "Legend1";
        HealChart.Legends.Add(legend2);
        HealChart.Location = new Point(0, 0);
        HealChart.Name = "HealChart";
        HealChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
        HealChart.Size = new Size(644, 213);
        HealChart.TabIndex = 0;
        HealChart.Text = "Healing Chart";
        // 
        // LogTextBox
        // 
        LogTextBox.Dock = DockStyle.Fill;
        LogTextBox.HideSelection = false;
        LogTextBox.Location = new Point(0, 0);
        LogTextBox.Multiline = true;
        LogTextBox.Name = "LogTextBox";
        LogTextBox.ReadOnly = true;
        LogTextBox.ScrollBars = ScrollBars.Both;
        LogTextBox.Size = new Size(892, 159);
        LogTextBox.TabIndex = 0;
        // 
        // timer1
        // 
        timer1.Enabled = true;
        timer1.Interval = 1000;
        timer1.Tick += timer1_Tick;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(892, 618);
        Controls.Add(splitContainer1);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Name = "MainForm";
        Text = "Patheon Parser";
        FormClosing += MainForm_FormClosing;
        Load += MainForm_Load;
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        splitContainer1.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        splitContainer2.Panel1.ResumeLayout(false);
        splitContainer2.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
        splitContainer2.ResumeLayout(false);
        splitContainer3.Panel1.ResumeLayout(false);
        splitContainer3.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
        splitContainer3.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)DamageChart).EndInit();
        ((System.ComponentModel.ISupportInitialize)HealChart).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;

    private System.Windows.Forms.MenuStrip menuStrip1;

    #endregion

    private SplitContainer splitContainer1;
    private TextBox LogTextBox;
    private System.Windows.Forms.Timer timer1;
    private SplitContainer splitContainer2;
    private ListBox EncounterListBox;
    private SplitContainer splitContainer3;
    private System.Windows.Forms.DataVisualization.Charting.Chart DamageChart;
    private System.Windows.Forms.DataVisualization.Charting.Chart HealChart;
    private ToolStripMenuItem encounterTimeoutToolStripMenuItem;
    private ToolStripTextBox EncounterTimeoutTextBox;
    private ToolStripMenuItem refreshToolStripMenuItem;
}