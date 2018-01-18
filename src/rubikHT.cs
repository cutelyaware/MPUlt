using System;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Data;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


namespace _3dedit
{

    internal delegate void Set2Texts(string ln,string ln1);
    internal delegate void Callback();
    /// <summary>
	/// Summary description for Form1.
	/// </summary>
    public class Form1:System.Windows.Forms.Form {
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
        private _3dedit.DXControl dxControl2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem mi_Open;
        private ToolStripMenuItem mi_Save;
        private ToolStripMenuItem mi_SaveAs;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mi_Exit;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem mi_Reset;
        private ToolStripMenuItem mi_Undo;
        private ToolStripMenuItem mi_Redo;
        private ToolStripMenuItem puzzleToolStripMenuItem;
        private ToolStripMenuItem mi_FullUndo;
        private ToolStripMenuItem mi_FullScramble;
        private ToolStripMenuItem mi_ScrambleNTurns;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem mi_FullRedo;
        private ToolStripMenuItem mi_Scramble1;
        private ToolStripMenuItem mi_Scramble2;
        private ToolStripMenuItem mi_Scramble3;
        private ToolStripMenuItem mi_Scramble4;
        private ToolStripMenuItem mi_Scramble5;
        private ToolStripMenuItem stopToolStripMenuItem;
        private Label label1;
        private TrackBar trk_faceShrink;
        private Label label3;
        private TrackBar trk_ViewAngle;
        private Label label2;
        private TrackBar trk_StickerSize;
        private Label label6;
        private Label label8;
        private Label label7;
        private TrackBar trk_LightSpec;
        private TrackBar trk_LightDiff;
        private Label label10;
        private ListBox m_lbMacros;
        private ToolStripMenuItem macroToolStripMenuItem;
        private ToolStripMenuItem mi_StartRecordig;
        private ToolStripMenuItem loadMacroFileToolStripMenuItem;
        private ToolStripMenuItem saveMacroFileToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem saveMacroFileAsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem startExtraTurnsToolStripMenuItem;
        private ToolStripMenuItem stopExtraTurnsToolStripMenuItem;
        private ToolStripMenuItem undoExtraTurnsToolStripMenuItem;
        private ToolStripMenuItem commutatorToolStripMenuItem;
        private TextBox m_tbRevStack;
        private Label label11;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem recalculateToolStripMenuItem;
        private Label label12;
        private ToolStripMenuItem helpToolStripMenuItem1;
        private TrackBar trk_UndoSpeed;
        private Button m_btnDeleteMacro;
        private Button m_btnRenameMacro;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem m_miShowTimer;
        private ToolStripMenuItem optimizeToolStripMenuItem;
        private Button m_btnSwDim;
        private CheckBox m_cbRank5;
        private CheckBox m_cbRank4;
        private CheckBox m_cbRank3;
        private CheckBox m_cbRank2;
        private CheckBox m_cbRank1;
        private CheckBox m_cbRank0;
        private Panel panel3;
        private Button m_btnShift;
        private Splitter splitter2;
        private Button m_btnL6;
        private Button m_btnL5;
        private Button m_btnL4;
        private Button m_btnL3;
        private Button m_btnL2;
        private Button m_btnL1;
        private Button m_btnAlt;
        private Button m_btnCtrl;
        private Label lbl_MacroStatus;
        private Label lbl_Twists;
        private Label lbl_CTime;
        private Label label5;
        private TrackBar trk_LightAmb;
        private CheckBox m_cbRank11;
        private CheckBox m_cbRank10;
        private CheckBox m_cbRank9;
        private CheckBox m_cbRank8;
        private CheckBox m_cbRank7;
        private CheckBox m_cbRank6;
        private Label label4;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem startMacroAToolStripMenuItem;
        private ToolStripMenuItem stopPerformMacroAToolStripMenuItem;
        private ToolStripMenuItem startMacroBToolStripMenuItem;
        private ToolStripMenuItem stopPerformMacroBToolStripMenuItem;
		private System.ComponentModel.IContainer components;

		public Form1() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// 
			// dxControl2
			// 
			this.dxControl2 = new _3dedit.DXControl();


			this.dxControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dxControl2.Location = new System.Drawing.Point(0, 0);
			this.dxControl2.Name = "dxControl2";
			this.dxControl2.Size = new System.Drawing.Size(793, 600);
			this.dxControl2.TabIndex = 0;
			this.dxControl2.MouseUp += new MouseEventHandler(MouseUpEvt);
			this.dxControl2.MouseDown += new MouseEventHandler(MouseDownEvt);
			this.dxControl2.MouseMove += new MouseEventHandler(MouseEvt);

			this.panel2.Controls.Add(this.dxControl2);

            m_setgeom=false;
            ReadPuzzles("MPUlt_puzzles.txt");
            this.puzzleToolStripMenuItem.DropDownItems.AddRange(CreatePuzzleMenu(PuzzleList));
            LoadSettings("MPUlt_settings.txt");
            m_Timer=new System.Threading.Timer(this.UpdateTime,null,0,117);

            if(Macros==null || Macros.FileName==null) {
                Macros=new CMacroFile(Puz.Str.Name,Puz.Str.Dim);
                //NewScene();
            }
 
            ApplyGeomSettings();
            m_setgeom=true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		//System.Resources.ResourceManager resources = new System.Resources.ResourceManager("_3dedit.Edit3D",Assembly.GetExecutingAssembly());

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.m_cbRank11 = new System.Windows.Forms.CheckBox();
            this.m_cbRank5 = new System.Windows.Forms.CheckBox();
            this.m_cbRank10 = new System.Windows.Forms.CheckBox();
            this.m_cbRank4 = new System.Windows.Forms.CheckBox();
            this.m_cbRank9 = new System.Windows.Forms.CheckBox();
            this.m_cbRank3 = new System.Windows.Forms.CheckBox();
            this.m_cbRank8 = new System.Windows.Forms.CheckBox();
            this.m_cbRank2 = new System.Windows.Forms.CheckBox();
            this.m_cbRank7 = new System.Windows.Forms.CheckBox();
            this.m_cbRank1 = new System.Windows.Forms.CheckBox();
            this.m_cbRank6 = new System.Windows.Forms.CheckBox();
            this.m_cbRank0 = new System.Windows.Forms.CheckBox();
            this.m_btnSwDim = new System.Windows.Forms.Button();
            this.m_btnDeleteMacro = new System.Windows.Forms.Button();
            this.m_btnRenameMacro = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.m_tbRevStack = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.m_lbMacros = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.trk_LightAmb = new System.Windows.Forms.TrackBar();
            this.trk_LightSpec = new System.Windows.Forms.TrackBar();
            this.trk_LightDiff = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.trk_UndoSpeed = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.trk_ViewAngle = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.trk_StickerSize = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.trk_faceShrink = new System.Windows.Forms.TrackBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mi_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_FullScramble = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_ScrambleNTurns = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Scramble1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Scramble2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Scramble3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Scramble4 = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Scramble5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mi_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_Redo = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_FullUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_FullRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.recalculateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_miShowTimer = new System.Windows.Forms.ToolStripMenuItem();
            this.puzzleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.macroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_StartRecordig = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMacroFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMacroFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMacroFileAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.startExtraTurnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopExtraTurnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoExtraTurnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commutatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbl_CTime = new System.Windows.Forms.Label();
            this.lbl_MacroStatus = new System.Windows.Forms.Label();
            this.lbl_Twists = new System.Windows.Forms.Label();
            this.m_btnL6 = new System.Windows.Forms.Button();
            this.m_btnL5 = new System.Windows.Forms.Button();
            this.m_btnL4 = new System.Windows.Forms.Button();
            this.m_btnL3 = new System.Windows.Forms.Button();
            this.m_btnL2 = new System.Windows.Forms.Button();
            this.m_btnL1 = new System.Windows.Forms.Button();
            this.m_btnAlt = new System.Windows.Forms.Button();
            this.m_btnCtrl = new System.Windows.Forms.Button();
            this.m_btnShift = new System.Windows.Forms.Button();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.startMacroAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopPerformMacroAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startMacroBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopPerformMacroBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_LightAmb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_LightSpec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_LightDiff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_UndoSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_ViewAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_StickerSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_faceShrink)).BeginInit();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(773,0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(4,684);
            this.splitter1.TabIndex = 18;
            this.splitter1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.m_cbRank11);
            this.panel1.Controls.Add(this.m_cbRank5);
            this.panel1.Controls.Add(this.m_cbRank10);
            this.panel1.Controls.Add(this.m_cbRank4);
            this.panel1.Controls.Add(this.m_cbRank9);
            this.panel1.Controls.Add(this.m_cbRank3);
            this.panel1.Controls.Add(this.m_cbRank8);
            this.panel1.Controls.Add(this.m_cbRank2);
            this.panel1.Controls.Add(this.m_cbRank7);
            this.panel1.Controls.Add(this.m_cbRank1);
            this.panel1.Controls.Add(this.m_cbRank6);
            this.panel1.Controls.Add(this.m_cbRank0);
            this.panel1.Controls.Add(this.m_btnSwDim);
            this.panel1.Controls.Add(this.m_btnDeleteMacro);
            this.panel1.Controls.Add(this.m_btnRenameMacro);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.m_tbRevStack);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.m_lbMacros);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.trk_LightAmb);
            this.panel1.Controls.Add(this.trk_LightSpec);
            this.panel1.Controls.Add(this.trk_LightDiff);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.trk_UndoSpeed);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.trk_ViewAngle);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.trk_StickerSize);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.trk_faceShrink);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif",8F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,((byte)(204)));
            this.panel1.Location = new System.Drawing.Point(777,0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(220,684);
            this.panel1.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(122,372);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81,13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Visible Stickers:";
            // 
            // m_cbRank11
            // 
            this.m_cbRank11.AutoSize = true;
            this.m_cbRank11.Checked = true;
            this.m_cbRank11.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank11.Location = new System.Drawing.Point(168,506);
            this.m_cbRank11.Name = "m_cbRank11";
            this.m_cbRank11.Size = new System.Drawing.Size(52,17);
            this.m_cbRank11.TabIndex = 22;
            this.m_cbRank11.Text = "R11+";
            this.m_cbRank11.UseVisualStyleBackColor = true;
            this.m_cbRank11.Click += new System.EventHandler(this.m_cbRank11_Click);
            // 
            // m_cbRank5
            // 
            this.m_cbRank5.AutoSize = true;
            this.m_cbRank5.Checked = true;
            this.m_cbRank5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank5.Location = new System.Drawing.Point(122,506);
            this.m_cbRank5.Name = "m_cbRank5";
            this.m_cbRank5.Size = new System.Drawing.Size(40,17);
            this.m_cbRank5.TabIndex = 22;
            this.m_cbRank5.Text = "R5";
            this.m_cbRank5.UseVisualStyleBackColor = true;
            this.m_cbRank5.Click += new System.EventHandler(this.m_cbRank5_Click);
            // 
            // m_cbRank10
            // 
            this.m_cbRank10.AutoSize = true;
            this.m_cbRank10.Checked = true;
            this.m_cbRank10.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank10.Location = new System.Drawing.Point(168,483);
            this.m_cbRank10.Name = "m_cbRank10";
            this.m_cbRank10.Size = new System.Drawing.Size(46,17);
            this.m_cbRank10.TabIndex = 22;
            this.m_cbRank10.Text = "R10";
            this.m_cbRank10.UseVisualStyleBackColor = true;
            this.m_cbRank10.Click += new System.EventHandler(this.m_cbRank10_Click);
            // 
            // m_cbRank4
            // 
            this.m_cbRank4.AutoSize = true;
            this.m_cbRank4.Checked = true;
            this.m_cbRank4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank4.Location = new System.Drawing.Point(122,483);
            this.m_cbRank4.Name = "m_cbRank4";
            this.m_cbRank4.Size = new System.Drawing.Size(40,17);
            this.m_cbRank4.TabIndex = 22;
            this.m_cbRank4.Text = "R4";
            this.m_cbRank4.UseVisualStyleBackColor = true;
            this.m_cbRank4.Click += new System.EventHandler(this.m_cbRank4_Click);
            // 
            // m_cbRank9
            // 
            this.m_cbRank9.AutoSize = true;
            this.m_cbRank9.Checked = true;
            this.m_cbRank9.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank9.Location = new System.Drawing.Point(168,460);
            this.m_cbRank9.Name = "m_cbRank9";
            this.m_cbRank9.Size = new System.Drawing.Size(40,17);
            this.m_cbRank9.TabIndex = 22;
            this.m_cbRank9.Text = "R9";
            this.m_cbRank9.UseVisualStyleBackColor = true;
            this.m_cbRank9.Click += new System.EventHandler(this.m_cbRank9_Click);
            // 
            // m_cbRank3
            // 
            this.m_cbRank3.AutoSize = true;
            this.m_cbRank3.Checked = true;
            this.m_cbRank3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank3.Location = new System.Drawing.Point(122,460);
            this.m_cbRank3.Name = "m_cbRank3";
            this.m_cbRank3.Size = new System.Drawing.Size(40,17);
            this.m_cbRank3.TabIndex = 22;
            this.m_cbRank3.Text = "R3";
            this.m_cbRank3.UseVisualStyleBackColor = true;
            this.m_cbRank3.Click += new System.EventHandler(this.m_cbRank3_Click);
            // 
            // m_cbRank8
            // 
            this.m_cbRank8.AutoSize = true;
            this.m_cbRank8.Checked = true;
            this.m_cbRank8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank8.Location = new System.Drawing.Point(168,437);
            this.m_cbRank8.Name = "m_cbRank8";
            this.m_cbRank8.Size = new System.Drawing.Size(40,17);
            this.m_cbRank8.TabIndex = 22;
            this.m_cbRank8.Text = "R8";
            this.m_cbRank8.UseVisualStyleBackColor = true;
            this.m_cbRank8.Click += new System.EventHandler(this.m_cbRank8_Click);
            // 
            // m_cbRank2
            // 
            this.m_cbRank2.AutoSize = true;
            this.m_cbRank2.Checked = true;
            this.m_cbRank2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank2.Location = new System.Drawing.Point(122,437);
            this.m_cbRank2.Name = "m_cbRank2";
            this.m_cbRank2.Size = new System.Drawing.Size(40,17);
            this.m_cbRank2.TabIndex = 22;
            this.m_cbRank2.Text = "R2";
            this.m_cbRank2.UseVisualStyleBackColor = true;
            this.m_cbRank2.Click += new System.EventHandler(this.m_cbRank2_Click);
            // 
            // m_cbRank7
            // 
            this.m_cbRank7.AutoSize = true;
            this.m_cbRank7.Checked = true;
            this.m_cbRank7.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank7.Location = new System.Drawing.Point(168,414);
            this.m_cbRank7.Name = "m_cbRank7";
            this.m_cbRank7.Size = new System.Drawing.Size(40,17);
            this.m_cbRank7.TabIndex = 22;
            this.m_cbRank7.Text = "R7";
            this.m_cbRank7.UseVisualStyleBackColor = true;
            this.m_cbRank7.Click += new System.EventHandler(this.m_cbRank7_Click);
            // 
            // m_cbRank1
            // 
            this.m_cbRank1.AutoSize = true;
            this.m_cbRank1.Checked = true;
            this.m_cbRank1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank1.Location = new System.Drawing.Point(122,414);
            this.m_cbRank1.Name = "m_cbRank1";
            this.m_cbRank1.Size = new System.Drawing.Size(40,17);
            this.m_cbRank1.TabIndex = 22;
            this.m_cbRank1.Text = "R1";
            this.m_cbRank1.UseVisualStyleBackColor = true;
            this.m_cbRank1.Click += new System.EventHandler(this.m_cbRank1_Click);
            // 
            // m_cbRank6
            // 
            this.m_cbRank6.AutoSize = true;
            this.m_cbRank6.Checked = true;
            this.m_cbRank6.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank6.Location = new System.Drawing.Point(169,391);
            this.m_cbRank6.Name = "m_cbRank6";
            this.m_cbRank6.Size = new System.Drawing.Size(40,17);
            this.m_cbRank6.TabIndex = 22;
            this.m_cbRank6.Text = "R6";
            this.m_cbRank6.UseVisualStyleBackColor = true;
            this.m_cbRank6.Click += new System.EventHandler(this.m_cbRank6_Click);
            // 
            // m_cbRank0
            // 
            this.m_cbRank0.AutoSize = true;
            this.m_cbRank0.Checked = true;
            this.m_cbRank0.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbRank0.Location = new System.Drawing.Point(123,391);
            this.m_cbRank0.Name = "m_cbRank0";
            this.m_cbRank0.Size = new System.Drawing.Size(40,17);
            this.m_cbRank0.TabIndex = 22;
            this.m_cbRank0.Text = "R0";
            this.m_cbRank0.UseVisualStyleBackColor = true;
            this.m_cbRank0.Click += new System.EventHandler(this.m_cbRank0_Click);
            // 
            // m_btnSwDim
            // 
            this.m_btnSwDim.Location = new System.Drawing.Point(9,241);
            this.m_btnSwDim.Name = "m_btnSwDim";
            this.m_btnSwDim.Size = new System.Drawing.Size(89,28);
            this.m_btnSwDim.TabIndex = 20;
            this.m_btnSwDim.Text = "Switch Dim";
            this.m_btnSwDim.UseVisualStyleBackColor = true;
            this.m_btnSwDim.Click += new System.EventHandler(this.m_btnSwDim_Click);
            // 
            // m_btnDeleteMacro
            // 
            this.m_btnDeleteMacro.Location = new System.Drawing.Point(127,339);
            this.m_btnDeleteMacro.Name = "m_btnDeleteMacro";
            this.m_btnDeleteMacro.Size = new System.Drawing.Size(69,23);
            this.m_btnDeleteMacro.TabIndex = 18;
            this.m_btnDeleteMacro.Text = "Delete";
            this.m_btnDeleteMacro.UseVisualStyleBackColor = true;
            this.m_btnDeleteMacro.Click += new System.EventHandler(this.m_btn_DeleteMacro_Click);
            // 
            // m_btnRenameMacro
            // 
            this.m_btnRenameMacro.Location = new System.Drawing.Point(128,310);
            this.m_btnRenameMacro.Name = "m_btnRenameMacro";
            this.m_btnRenameMacro.Size = new System.Drawing.Size(69,23);
            this.m_btnRenameMacro.TabIndex = 18;
            this.m_btnRenameMacro.Text = "Rename";
            this.m_btnRenameMacro.UseVisualStyleBackColor = true;
            this.m_btnRenameMacro.Click += new System.EventHandler(this.m_btn_RenameMacro_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6,92);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(87,13);
            this.label12.TabIndex = 16;
            this.label12.Text = "Animation Speed";
            // 
            // m_tbRevStack
            // 
            this.m_tbRevStack.BackColor = System.Drawing.SystemColors.Control;
            this.m_tbRevStack.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.m_tbRevStack.Location = new System.Drawing.Point(70,535);
            this.m_tbRevStack.Name = "m_tbRevStack";
            this.m_tbRevStack.ReadOnly = true;
            this.m_tbRevStack.Size = new System.Drawing.Size(138,13);
            this.m_tbRevStack.TabIndex = 15;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10,535);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58,13);
            this.label11.TabIndex = 14;
            this.label11.Text = "RevStack:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9,294);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45,13);
            this.label10.TabIndex = 13;
            this.label10.Text = "Macros:";
            // 
            // m_lbMacros
            // 
            this.m_lbMacros.FormattingEnabled = true;
            this.m_lbMacros.Location = new System.Drawing.Point(9,310);
            this.m_lbMacros.Name = "m_lbMacros";
            this.m_lbMacros.Size = new System.Drawing.Size(107,212);
            this.m_lbMacros.Sorted = true;
            this.m_lbMacros.TabIndex = 12;
            this.m_lbMacros.MouseClick += new System.Windows.Forms.MouseEventHandler(this.m_lbMacros_MouseClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10,217);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17,13);
            this.label5.TabIndex = 8;
            this.label5.Text = "A:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10,193);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17,13);
            this.label8.TabIndex = 8;
            this.label8.Text = "S:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10,166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18,13);
            this.label7.TabIndex = 8;
            this.label7.Text = "D:";
            // 
            // trk_LightAmb
            // 
            this.trk_LightAmb.Location = new System.Drawing.Point(28,214);
            this.trk_LightAmb.Maximum = 255;
            this.trk_LightAmb.Name = "trk_LightAmb";
            this.trk_LightAmb.Size = new System.Drawing.Size(88,45);
            this.trk_LightAmb.TabIndex = 7;
            this.trk_LightAmb.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_LightAmb.Value = 100;
            this.trk_LightAmb.ValueChanged += new System.EventHandler(this.trk_LightSpec_ValueChanged);
            // 
            // trk_LightSpec
            // 
            this.trk_LightSpec.Location = new System.Drawing.Point(28,190);
            this.trk_LightSpec.Maximum = 255;
            this.trk_LightSpec.Name = "trk_LightSpec";
            this.trk_LightSpec.Size = new System.Drawing.Size(88,45);
            this.trk_LightSpec.TabIndex = 7;
            this.trk_LightSpec.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_LightSpec.Value = 100;
            this.trk_LightSpec.ValueChanged += new System.EventHandler(this.trk_LightSpec_ValueChanged);
            // 
            // trk_LightDiff
            // 
            this.trk_LightDiff.Location = new System.Drawing.Point(28,163);
            this.trk_LightDiff.Maximum = 255;
            this.trk_LightDiff.Name = "trk_LightDiff";
            this.trk_LightDiff.Size = new System.Drawing.Size(88,45);
            this.trk_LightDiff.TabIndex = 7;
            this.trk_LightDiff.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_LightDiff.Value = 150;
            this.trk_LightDiff.ValueChanged += new System.EventHandler(this.trk_LightDiff_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6,144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33,13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Light:";
            // 
            // trk_UndoSpeed
            // 
            this.trk_UndoSpeed.Location = new System.Drawing.Point(96,88);
            this.trk_UndoSpeed.Maximum = 50;
            this.trk_UndoSpeed.Name = "trk_UndoSpeed";
            this.trk_UndoSpeed.Size = new System.Drawing.Size(104,45);
            this.trk_UndoSpeed.TabIndex = 0;
            this.trk_UndoSpeed.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_UndoSpeed.ValueChanged += new System.EventHandler(this.m_trkUndoSpeed_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6,68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60,13);
            this.label3.TabIndex = 1;
            this.label3.Text = "View Angle";
            // 
            // trk_ViewAngle
            // 
            this.trk_ViewAngle.Location = new System.Drawing.Point(96,64);
            this.trk_ViewAngle.Maximum = 50;
            this.trk_ViewAngle.Minimum = 1;
            this.trk_ViewAngle.Name = "trk_ViewAngle";
            this.trk_ViewAngle.Size = new System.Drawing.Size(104,45);
            this.trk_ViewAngle.TabIndex = 0;
            this.trk_ViewAngle.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_ViewAngle.Value = 17;
            this.trk_ViewAngle.ValueChanged += new System.EventHandler(this.trk_ViewAngle_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6,44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63,13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Sticker Size";
            // 
            // trk_StickerSize
            // 
            this.trk_StickerSize.Location = new System.Drawing.Point(96,40);
            this.trk_StickerSize.Maximum = 50;
            this.trk_StickerSize.Minimum = 1;
            this.trk_StickerSize.Name = "trk_StickerSize";
            this.trk_StickerSize.Size = new System.Drawing.Size(104,45);
            this.trk_StickerSize.TabIndex = 0;
            this.trk_StickerSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_StickerSize.Value = 40;
            this.trk_StickerSize.ValueChanged += new System.EventHandler(this.trk_faceShrink_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6,20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64,13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Face Shrink";
            // 
            // trk_faceShrink
            // 
            this.trk_faceShrink.Location = new System.Drawing.Point(96,16);
            this.trk_faceShrink.Maximum = 50;
            this.trk_faceShrink.Name = "trk_faceShrink";
            this.trk_faceShrink.Size = new System.Drawing.Size(104,45);
            this.trk_faceShrink.TabIndex = 0;
            this.trk_faceShrink.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trk_faceShrink.Value = 28;
            this.trk_faceShrink.ValueChanged += new System.EventHandler(this.trk_faceShrink_ValueChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.menuStrip1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0,0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(773,684);
            this.panel2.TabIndex = 20;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.puzzleToolStripMenuItem,
            this.macroToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0,0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(773,24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_Open,
            this.mi_Save,
            this.mi_SaveAs,
            this.toolStripMenuItem1,
            this.mi_Exit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37,20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // mi_Open
            // 
            this.mi_Open.Name = "mi_Open";
            this.mi_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mi_Open.Size = new System.Drawing.Size(146,22);
            this.mi_Open.Text = "Open";
            this.mi_Open.Click += new System.EventHandler(this.mi_Open_Click);
            // 
            // mi_Save
            // 
            this.mi_Save.Name = "mi_Save";
            this.mi_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mi_Save.Size = new System.Drawing.Size(146,22);
            this.mi_Save.Text = "Save";
            this.mi_Save.Click += new System.EventHandler(this.mi_Save_Click);
            // 
            // mi_SaveAs
            // 
            this.mi_SaveAs.Name = "mi_SaveAs";
            this.mi_SaveAs.Size = new System.Drawing.Size(146,22);
            this.mi_SaveAs.Text = "Save As...";
            this.mi_SaveAs.Click += new System.EventHandler(this.mi_SaveAs_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(143,6);
            // 
            // mi_Exit
            // 
            this.mi_Exit.Name = "mi_Exit";
            this.mi_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mi_Exit.Size = new System.Drawing.Size(146,22);
            this.mi_Exit.Text = "Exit";
            this.mi_Exit.Click += new System.EventHandler(this.mi_Exit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_Reset,
            this.mi_FullScramble,
            this.mi_ScrambleNTurns,
            this.toolStripSeparator1,
            this.mi_Undo,
            this.mi_Redo,
            this.mi_FullUndo,
            this.mi_FullRedo,
            this.stopToolStripMenuItem,
            this.toolStripMenuItem4,
            this.recalculateToolStripMenuItem,
            this.optimizeToolStripMenuItem,
            this.toolStripMenuItem2,
            this.m_miShowTimer});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39,20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // mi_Reset
            // 
            this.mi_Reset.Name = "mi_Reset";
            this.mi_Reset.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.mi_Reset.Size = new System.Drawing.Size(165,22);
            this.mi_Reset.Text = "Reset";
            this.mi_Reset.Click += new System.EventHandler(this.mi_Reset_Click);
            // 
            // mi_FullScramble
            // 
            this.mi_FullScramble.Name = "mi_FullScramble";
            this.mi_FullScramble.Size = new System.Drawing.Size(165,22);
            this.mi_FullScramble.Text = "Full Scramble";
            this.mi_FullScramble.Click += new System.EventHandler(this.mi_FullScramble_Click);
            // 
            // mi_ScrambleNTurns
            // 
            this.mi_ScrambleNTurns.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_Scramble1,
            this.mi_Scramble2,
            this.mi_Scramble3,
            this.mi_Scramble4,
            this.mi_Scramble5});
            this.mi_ScrambleNTurns.Name = "mi_ScrambleNTurns";
            this.mi_ScrambleNTurns.Size = new System.Drawing.Size(165,22);
            this.mi_ScrambleNTurns.Text = "Scramble N turns";
            // 
            // mi_Scramble1
            // 
            this.mi_Scramble1.Name = "mi_Scramble1";
            this.mi_Scramble1.Size = new System.Drawing.Size(80,22);
            this.mi_Scramble1.Text = "1";
            this.mi_Scramble1.Click += new System.EventHandler(this.mi_Scramble1_Click);
            // 
            // mi_Scramble2
            // 
            this.mi_Scramble2.Name = "mi_Scramble2";
            this.mi_Scramble2.Size = new System.Drawing.Size(80,22);
            this.mi_Scramble2.Text = "2";
            this.mi_Scramble2.Click += new System.EventHandler(this.mi_Scramble2_Click);
            // 
            // mi_Scramble3
            // 
            this.mi_Scramble3.Name = "mi_Scramble3";
            this.mi_Scramble3.Size = new System.Drawing.Size(80,22);
            this.mi_Scramble3.Text = "3";
            this.mi_Scramble3.Click += new System.EventHandler(this.mi_Scramble3_Click);
            // 
            // mi_Scramble4
            // 
            this.mi_Scramble4.Name = "mi_Scramble4";
            this.mi_Scramble4.Size = new System.Drawing.Size(80,22);
            this.mi_Scramble4.Text = "4";
            this.mi_Scramble4.Click += new System.EventHandler(this.mi_Scramble4_Click);
            // 
            // mi_Scramble5
            // 
            this.mi_Scramble5.Name = "mi_Scramble5";
            this.mi_Scramble5.Size = new System.Drawing.Size(80,22);
            this.mi_Scramble5.Text = "5";
            this.mi_Scramble5.Click += new System.EventHandler(this.mi_Scramble5_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(162,6);
            // 
            // mi_Undo
            // 
            this.mi_Undo.Name = "mi_Undo";
            this.mi_Undo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.mi_Undo.Size = new System.Drawing.Size(165,22);
            this.mi_Undo.Text = "Undo";
            this.mi_Undo.Click += new System.EventHandler(this.mi_Undo_Click);
            // 
            // mi_Redo
            // 
            this.mi_Redo.Name = "mi_Redo";
            this.mi_Redo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.mi_Redo.Size = new System.Drawing.Size(165,22);
            this.mi_Redo.Text = "Redo";
            this.mi_Redo.Click += new System.EventHandler(this.mi_Redo_Click);
            // 
            // mi_FullUndo
            // 
            this.mi_FullUndo.Name = "mi_FullUndo";
            this.mi_FullUndo.Size = new System.Drawing.Size(165,22);
            this.mi_FullUndo.Text = "Full Undo";
            this.mi_FullUndo.Click += new System.EventHandler(this.mi_FullUndo_Click);
            // 
            // mi_FullRedo
            // 
            this.mi_FullRedo.Name = "mi_FullRedo";
            this.mi_FullRedo.Size = new System.Drawing.Size(165,22);
            this.mi_FullRedo.Text = "Full Redo";
            this.mi_FullRedo.Click += new System.EventHandler(this.mi_FullRedo_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(165,22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(162,6);
            // 
            // recalculateToolStripMenuItem
            // 
            this.recalculateToolStripMenuItem.Name = "recalculateToolStripMenuItem";
            this.recalculateToolStripMenuItem.Size = new System.Drawing.Size(165,22);
            this.recalculateToolStripMenuItem.Text = "Recalculate";
            this.recalculateToolStripMenuItem.Click += new System.EventHandler(this.recalculateToolStripMenuItem_Click);
            // 
            // optimizeToolStripMenuItem
            // 
            this.optimizeToolStripMenuItem.Name = "optimizeToolStripMenuItem";
            this.optimizeToolStripMenuItem.Size = new System.Drawing.Size(165,22);
            this.optimizeToolStripMenuItem.Text = "Optimize";
            this.optimizeToolStripMenuItem.Click += new System.EventHandler(this.optimizeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(162,6);
            // 
            // m_miShowTimer
            // 
            this.m_miShowTimer.Name = "m_miShowTimer";
            this.m_miShowTimer.Size = new System.Drawing.Size(165,22);
            this.m_miShowTimer.Text = "Show Timer";
            this.m_miShowTimer.Click += new System.EventHandler(this.m_miShowTimer_Click);
            // 
            // puzzleToolStripMenuItem
            // 
            this.puzzleToolStripMenuItem.Name = "puzzleToolStripMenuItem";
            this.puzzleToolStripMenuItem.Size = new System.Drawing.Size(52,20);
            this.puzzleToolStripMenuItem.Text = "Puzzle";
            // 
            // macroToolStripMenuItem
            // 
            this.macroToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_StartRecordig,
            this.loadMacroFileToolStripMenuItem,
            this.saveMacroFileToolStripMenuItem,
            this.saveMacroFileAsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.startExtraTurnsToolStripMenuItem,
            this.stopExtraTurnsToolStripMenuItem,
            this.undoExtraTurnsToolStripMenuItem,
            this.commutatorToolStripMenuItem,
            this.toolStripMenuItem6,
            this.startMacroAToolStripMenuItem,
            this.stopPerformMacroAToolStripMenuItem,
            this.startMacroBToolStripMenuItem,
            this.stopPerformMacroBToolStripMenuItem});
            this.macroToolStripMenuItem.Name = "macroToolStripMenuItem";
            this.macroToolStripMenuItem.Size = new System.Drawing.Size(58,20);
            this.macroToolStripMenuItem.Text = "Macros";
            // 
            // mi_StartRecordig
            // 
            this.mi_StartRecordig.Name = "mi_StartRecordig";
            this.mi_StartRecordig.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.mi_StartRecordig.Size = new System.Drawing.Size(213,22);
            this.mi_StartRecordig.Text = "Stop Recording";
            this.mi_StartRecordig.Click += new System.EventHandler(this.mi_StartRecordig_Click);
            // 
            // loadMacroFileToolStripMenuItem
            // 
            this.loadMacroFileToolStripMenuItem.Name = "loadMacroFileToolStripMenuItem";
            this.loadMacroFileToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.loadMacroFileToolStripMenuItem.Text = "Load Macro File";
            this.loadMacroFileToolStripMenuItem.Click += new System.EventHandler(this.loadMacroFileToolStripMenuItem_Click);
            // 
            // saveMacroFileToolStripMenuItem
            // 
            this.saveMacroFileToolStripMenuItem.Name = "saveMacroFileToolStripMenuItem";
            this.saveMacroFileToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.saveMacroFileToolStripMenuItem.Text = "Save Macro File";
            this.saveMacroFileToolStripMenuItem.Click += new System.EventHandler(this.saveMacroFileToolStripMenuItem_Click);
            // 
            // saveMacroFileAsToolStripMenuItem
            // 
            this.saveMacroFileAsToolStripMenuItem.Name = "saveMacroFileAsToolStripMenuItem";
            this.saveMacroFileAsToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.saveMacroFileAsToolStripMenuItem.Text = "Save Macro File As...";
            this.saveMacroFileAsToolStripMenuItem.Click += new System.EventHandler(this.saveMacroFileAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(210,6);
            // 
            // startExtraTurnsToolStripMenuItem
            // 
            this.startExtraTurnsToolStripMenuItem.Name = "startExtraTurnsToolStripMenuItem";
            this.startExtraTurnsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.startExtraTurnsToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.startExtraTurnsToolStripMenuItem.Text = "Start Extra Turns";
            this.startExtraTurnsToolStripMenuItem.Click += new System.EventHandler(this.startExtraTurnsToolStripMenuItem_Click);
            // 
            // stopExtraTurnsToolStripMenuItem
            // 
            this.stopExtraTurnsToolStripMenuItem.Name = "stopExtraTurnsToolStripMenuItem";
            this.stopExtraTurnsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.stopExtraTurnsToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.stopExtraTurnsToolStripMenuItem.Text = "Stop Extra Turns";
            this.stopExtraTurnsToolStripMenuItem.Click += new System.EventHandler(this.stopExtraTurnsToolStripMenuItem_Click);
            // 
            // undoExtraTurnsToolStripMenuItem
            // 
            this.undoExtraTurnsToolStripMenuItem.Name = "undoExtraTurnsToolStripMenuItem";
            this.undoExtraTurnsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.undoExtraTurnsToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.undoExtraTurnsToolStripMenuItem.Text = "Unwind Extra Turns";
            this.undoExtraTurnsToolStripMenuItem.Click += new System.EventHandler(this.undoExtraTurnsToolStripMenuItem_Click);
            // 
            // commutatorToolStripMenuItem
            // 
            this.commutatorToolStripMenuItem.Name = "commutatorToolStripMenuItem";
            this.commutatorToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.commutatorToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.commutatorToolStripMenuItem.Text = "Commutator";
            this.commutatorToolStripMenuItem.Click += new System.EventHandler(this.commutatorToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.helpToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44,20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(145,22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(145,22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lbl_CTime);
            this.panel3.Controls.Add(this.lbl_MacroStatus);
            this.panel3.Controls.Add(this.lbl_Twists);
            this.panel3.Controls.Add(this.m_btnL6);
            this.panel3.Controls.Add(this.m_btnL5);
            this.panel3.Controls.Add(this.m_btnL4);
            this.panel3.Controls.Add(this.m_btnL3);
            this.panel3.Controls.Add(this.m_btnL2);
            this.panel3.Controls.Add(this.m_btnL1);
            this.panel3.Controls.Add(this.m_btnAlt);
            this.panel3.Controls.Add(this.m_btnCtrl);
            this.panel3.Controls.Add(this.m_btnShift);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0,632);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(773,52);
            this.panel3.TabIndex = 21;
            // 
            // lbl_CTime
            // 
            this.lbl_CTime.AutoSize = true;
            this.lbl_CTime.Location = new System.Drawing.Point(184,4);
            this.lbl_CTime.Name = "lbl_CTime";
            this.lbl_CTime.Size = new System.Drawing.Size(78,13);
            this.lbl_CTime.TabIndex = 3;
            this.lbl_CTime.Text = "Time: 00:00:00";
            // 
            // lbl_MacroStatus
            // 
            this.lbl_MacroStatus.AutoSize = true;
            this.lbl_MacroStatus.Location = new System.Drawing.Point(103,7);
            this.lbl_MacroStatus.Name = "lbl_MacroStatus";
            this.lbl_MacroStatus.Size = new System.Drawing.Size(38,13);
            this.lbl_MacroStatus.TabIndex = 2;
            this.lbl_MacroStatus.Text = "Ready";
            // 
            // lbl_Twists
            // 
            this.lbl_Twists.AutoSize = true;
            this.lbl_Twists.Location = new System.Drawing.Point(13,7);
            this.lbl_Twists.Name = "lbl_Twists";
            this.lbl_Twists.Size = new System.Drawing.Size(49,13);
            this.lbl_Twists.TabIndex = 1;
            this.lbl_Twists.Text = "Twists: 0";
            // 
            // m_btnL6
            // 
            this.m_btnL6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnL6.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnL6.Location = new System.Drawing.Point(424,25);
            this.m_btnL6.Name = "m_btnL6";
            this.m_btnL6.Size = new System.Drawing.Size(47,27);
            this.m_btnL6.TabIndex = 0;
            this.m_btnL6.Text = "6";
            this.m_btnL6.UseVisualStyleBackColor = false;
            this.m_btnL6.Click += new System.EventHandler(this.m_btnL6_Click);
            // 
            // m_btnL5
            // 
            this.m_btnL5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnL5.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnL5.Location = new System.Drawing.Point(371,25);
            this.m_btnL5.Name = "m_btnL5";
            this.m_btnL5.Size = new System.Drawing.Size(47,27);
            this.m_btnL5.TabIndex = 0;
            this.m_btnL5.Text = "5";
            this.m_btnL5.UseVisualStyleBackColor = false;
            this.m_btnL5.Click += new System.EventHandler(this.m_btnL5_Click);
            // 
            // m_btnL4
            // 
            this.m_btnL4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnL4.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnL4.Location = new System.Drawing.Point(318,25);
            this.m_btnL4.Name = "m_btnL4";
            this.m_btnL4.Size = new System.Drawing.Size(47,27);
            this.m_btnL4.TabIndex = 0;
            this.m_btnL4.Text = "4";
            this.m_btnL4.UseVisualStyleBackColor = false;
            this.m_btnL4.Click += new System.EventHandler(this.m_btnL4_Click);
            // 
            // m_btnL3
            // 
            this.m_btnL3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnL3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnL3.Location = new System.Drawing.Point(265,25);
            this.m_btnL3.Name = "m_btnL3";
            this.m_btnL3.Size = new System.Drawing.Size(47,27);
            this.m_btnL3.TabIndex = 0;
            this.m_btnL3.Text = "3";
            this.m_btnL3.UseVisualStyleBackColor = false;
            this.m_btnL3.Click += new System.EventHandler(this.m_btnL3_Click);
            // 
            // m_btnL2
            // 
            this.m_btnL2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnL2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnL2.Location = new System.Drawing.Point(212,25);
            this.m_btnL2.Name = "m_btnL2";
            this.m_btnL2.Size = new System.Drawing.Size(47,27);
            this.m_btnL2.TabIndex = 0;
            this.m_btnL2.Text = "2";
            this.m_btnL2.UseVisualStyleBackColor = false;
            this.m_btnL2.Click += new System.EventHandler(this.m_btnL2_Click);
            // 
            // m_btnL1
            // 
            this.m_btnL1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnL1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnL1.Location = new System.Drawing.Point(159,25);
            this.m_btnL1.Name = "m_btnL1";
            this.m_btnL1.Size = new System.Drawing.Size(47,27);
            this.m_btnL1.TabIndex = 0;
            this.m_btnL1.Text = "1";
            this.m_btnL1.UseVisualStyleBackColor = false;
            this.m_btnL1.Click += new System.EventHandler(this.m_btnL1_Click);
            // 
            // m_btnAlt
            // 
            this.m_btnAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnAlt.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnAlt.Location = new System.Drawing.Point(106,25);
            this.m_btnAlt.Name = "m_btnAlt";
            this.m_btnAlt.Size = new System.Drawing.Size(47,27);
            this.m_btnAlt.TabIndex = 0;
            this.m_btnAlt.Text = "Alt";
            this.m_btnAlt.UseVisualStyleBackColor = false;
            this.m_btnAlt.Click += new System.EventHandler(this.m_btnAlt_Click);
            // 
            // m_btnCtrl
            // 
            this.m_btnCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnCtrl.BackColor = System.Drawing.SystemColors.Control;
            this.m_btnCtrl.Location = new System.Drawing.Point(53,25);
            this.m_btnCtrl.Name = "m_btnCtrl";
            this.m_btnCtrl.Size = new System.Drawing.Size(47,27);
            this.m_btnCtrl.TabIndex = 0;
            this.m_btnCtrl.Text = "Ctrl";
            this.m_btnCtrl.UseVisualStyleBackColor = false;
            this.m_btnCtrl.Click += new System.EventHandler(this.m_btnCtrl_Click);
            // 
            // m_btnShift
            // 
            this.m_btnShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnShift.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnShift.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnShift.Location = new System.Drawing.Point(0,25);
            this.m_btnShift.Name = "m_btnShift";
            this.m_btnShift.Size = new System.Drawing.Size(47,27);
            this.m_btnShift.TabIndex = 0;
            this.m_btnShift.Text = "Shift";
            this.m_btnShift.UseCompatibleTextRendering = true;
            this.m_btnShift.UseVisualStyleBackColor = false;
            this.m_btnShift.Click += new System.EventHandler(this.m_btnShift_Click);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0,628);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(773,4);
            this.splitter2.TabIndex = 22;
            this.splitter2.TabStop = false;
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(210,6);
            // 
            // startMacroAToolStripMenuItem
            // 
            this.startMacroAToolStripMenuItem.Name = "startMacroAToolStripMenuItem";
            this.startMacroAToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F7)));
            this.startMacroAToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.startMacroAToolStripMenuItem.Text = "Start macro A";
            this.startMacroAToolStripMenuItem.Click += new System.EventHandler(this.startMacroAToolStripMenuItem_Click);
            // 
            // stopPerformMacroAToolStripMenuItem
            // 
            this.stopPerformMacroAToolStripMenuItem.Name = "stopPerformMacroAToolStripMenuItem";
            this.stopPerformMacroAToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.stopPerformMacroAToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.stopPerformMacroAToolStripMenuItem.Text = "Stop/Perform macro A";
            this.stopPerformMacroAToolStripMenuItem.Click += new System.EventHandler(this.stopPerformMacroAToolStripMenuItem_Click);
            // 
            // startMacroBToolStripMenuItem
            // 
            this.startMacroBToolStripMenuItem.Name = "startMacroBToolStripMenuItem";
            this.startMacroBToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F8)));
            this.startMacroBToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.startMacroBToolStripMenuItem.Text = "Start macro B";
            this.startMacroBToolStripMenuItem.Click += new System.EventHandler(this.startMacroBToolStripMenuItem_Click);
            // 
            // stopPerformMacroBToolStripMenuItem
            // 
            this.stopPerformMacroBToolStripMenuItem.Name = "stopPerformMacroBToolStripMenuItem";
            this.stopPerformMacroBToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.stopPerformMacroBToolStripMenuItem.Size = new System.Drawing.Size(213,22);
            this.stopPerformMacroBToolStripMenuItem.Text = "Stop/Perform macro B";
            this.stopPerformMacroBToolStripMenuItem.Click += new System.EventHandler(this.stopPerformMacroBToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
            this.ClientSize = new System.Drawing.Size(997,684);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Magic Puzzle Ultimate";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trk_LightAmb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_LightSpec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_LightDiff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_UndoSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_ViewAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_StickerSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk_faceShrink)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

        Puzzle Puz;
        Hashtable PuzzleList;
        string m_puzzleName;
        CubeObj CubeView;
        int TRate=500;

        int m_status=STATUS_WAIT_TWIST;
        int m_curAx,m_mask,m_dir,m_tw;
        int[] m_stkseq=new int[100];
        int m_lstkseq;

        const int STATUS_WAIT_TWIST=0;
        const int STATUS_WAIT_STICKER=1;
        const int STATUS_DO_TWIST=2;
        const int STATUS_WAIT_MACRO_STICKER=3;
        
        CMacroFile Macros;
        string m_curMacroName;

        int StartA,EndA,StartB,EndB;

        bool[] m_AddMask=new bool[9];
        int m_ShowRank=0xfff;

        // for 4D:
        double[] m_macroVec;
        int[] m_macroStickers=new int[100];
        int m_NMacroStickers=0;
        int MacroStart=-1;

        bool qSolved=true;

        long m_TStart=0;
        bool m_TRun=false;

        System.Threading.Timer m_Timer;
        bool m_Closing=false;

		int ClickX,ClickY;
        double cpath=0;

        bool qLeftDown=false,qRightDown=false,qSkipClick=false;
		
		private void MouseDownEvt(object sender, MouseEventArgs e) {
            qSkipClick=false;
            if(e.Button==MouseButtons.Left) qLeftDown=true;
            if(e.Button==MouseButtons.Right) qRightDown=true;
            ClickX=e.X; ClickY=e.Y; cpath=0;           
			MouseEvt(sender,e);
		}
        void addPath(int x,int y) {
            cpath+=MyMath.pyth(ClickX-x,ClickY-y);
            ClickY=y; ClickX=x;
        }
		private void MouseUpEvt(object sender, MouseEventArgs e) {
            if(e.Button==MouseButtons.Left) qLeftDown=false;
            if(e.Button==MouseButtons.Right) qRightDown=false;
            addPath(e.X,e.Y);
				if(e.Clicks==2 || cpath<=3){
                    if(!qSkipClick) ProcessClick(e);
                    else qSkipClick=false;
				}
			MouseEvt(sender,e);
		}
		
		private void MouseEvt(object sender, MouseEventArgs e) {
            addPath(e.X,e.Y);
			dxControl2.ProcessMouse(e,ETarget.TargetCamera,null);
		}

		
		public void ProcessClick(MouseEventArgs e){
            dxControl2.ProcessPick(e,ETarget.TargetObject,new OnAction(mkPickObject));
        }
	
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		void RefreshView(){
			dxControl2.ParkCamera(false);
            dxControl2.SetSceneChanged();
		}

        double[,] _twm;
        void StartAnimation(int axis,int twist,int angle,int mask) {
            if(m_status==STATUS_DO_TWIST) return;
            m_status=STATUS_DO_TWIST;
            m_runUndo=true;
            double rangle;
            double[] twv;
            Puz.GetTwistGeom(axis,twist,angle,out twv,out rangle);

            int t=1;
            while(m_runUndo) {
                t+=50;
                if(t>=TRate) break;
                double ra=(rangle*t)/TRate;
                _twm=PGeom.GetMatrixForTwist(twv,ra);
                Puz.SetTwist(axis,mask,_SetStickerMove);
                RedrawAll();
                Thread.Sleep(50);
                Application.DoEvents();
            }
            _twm=null;
            Puz.SetTwist(axis,mask,_SetStickerMove);
            m_runUndo=false;
            m_status=STATUS_WAIT_TWIST;
        }

        bool _SetStickerMove(int n) {
            CubeView.Stks[n].ExtraTwist=_twm;
            return true;
        }

        void mkPickObject(EAction act,double x,double y,double ang) {
            m_runUndo=false;
            bool qch=false;

            if(act==EAction.ActionClick || act==EAction.ActionRClick) {
                int nf=0;
                m_dir=1;
                if(m_status==STATUS_WAIT_STICKER) {
                    int st=CubeView.FindSticker(-x,y,CubeObj.FIND_STICKER_SELECTED);
                    if(st<0) {
                        Puz.RestoreField();
                        m_curAx=-1;
                        m_status=STATUS_WAIT_TWIST;
                    } else {
                        m_stkseq[m_lstkseq++]=Puz.Str.BaseSticker(st,m_curAx);
                        int r=Puz.Str.CheckTwist(m_curAx,m_stkseq,m_lstkseq,Puz.Field,out m_tw);
                        if(r==1) m_stkseq[m_lstkseq++]=-1;
                        if(r==2) {
                            Puz.RestoreField();
                            NormTwist(ref m_curAx,ref m_tw,ref m_dir,ref m_mask);
                            StartAnimation(m_curAx,m_tw,m_dir,m_mask);
                            Puz.Twist(m_curAx,m_tw,m_dir,m_mask);
                            m_status=STATUS_WAIT_TWIST;
                            qch=true;
                        }
                    }
                } else {
                    m_mask=0;
                    for(int i=0;i<9;i++) if((i<6 && m_AddMask[i+3]) || (S3DirectX.GetAsyncKeyState(49+i) & 0x8000) != 0)
                            m_mask|=(1<<i);
                    if(m_mask==0) m_mask=1;

                    if(Puz.Str.Dim==4) {
                        if((act==EAction.ActionClick && qRightDown) || (act==EAction.ActionRClick && qLeftDown)) {
                            act=EAction.ActionLRClick;
                            qSkipClick=true;
                        }
                        switch(act) {
                            case EAction.ActionClick:
                                m_dir=-1;
                                goto case EAction.ActionRClick;
                            case EAction.ActionRClick:{
                                    double[] pt=CubeView.FindFace(-x,y,out nf);
                                    if(nf<0) return;
                                    if(Puz.Str.Faces[nf].RefAxis==0) goto case EAction.ActionLRClick;
                                    if(!Puz.Str.FindTwist(nf,pt,out m_curAx,out m_tw)) return;
                                    NormTwist(ref m_curAx,ref m_tw,ref m_dir,ref m_mask);
                                    StartAnimation(m_curAx,m_tw,m_dir,m_mask);
                                    Puz.Twist(m_curAx,m_tw,m_dir,m_mask);
                                    qch=true;
                                    break;
                                }
                            case EAction.ActionLRClick: {
                                    double[] pt=CubeView.FindFace(-x,y,out nf);
                                    if(nf<0) {
                                        if(m_curAx<0) return;
                                        StartAnimation(m_curAx,m_tw,m_dir,m_mask);
                                        Puz.Twist(m_curAx,m_tw,m_dir,m_mask);
                                        qch=true;
                                        break;
                                    }
                                    m_curAx=Puz.Str.FindNearestAxis(pt,false);
                                    m_lstkseq=0;
                                    Puz.SaveField();
                                    int r=Puz.Str.CheckTwist(m_curAx,m_stkseq,m_lstkseq,Puz.Field,out m_tw);
                                    if(r==1) m_stkseq[m_lstkseq++]=-1;
                                    if(r==2) {
                                        NormTwist(ref m_curAx,ref m_tw,ref m_dir,ref m_mask);
                                        StartAnimation(m_curAx,m_tw,m_dir,m_mask);
                                        Puz.Twist(m_curAx,m_tw,m_dir,m_mask);
                                        qch=true;
                                    } else {
                                        m_status=STATUS_WAIT_STICKER;
                                    }
                                    break;
                                }
                        }
                    } else {
                        int st=CubeView.FindSticker(-x,y,CubeObj.FIND_STICKER_CORNER);
                        if(st<0){
                            if(m_curAx>=0) {
                                StartAnimation(m_curAx,m_tw,m_dir,m_mask);
                                Puz.Twist(m_curAx,m_tw,m_dir,m_mask);
                                qch=true;
                            }
                        } else {
                            if(act==EAction.ActionClick) {
                                nf=Puz.Str.FindFaceBySticker(st);
                                int a=Puz.Str.Faces[nf].RefAxis;
                                if(a==0) return;
                                m_curAx=a;
                            } else {
                                double[] qx=PGeom.ApplyMatrix(CubeView.Stks[st].Matr,CubeView.Stks[st].Base.Ctr);
                                int a=Puz.Str.FindNearestAxis(qx,true);
                                if(a==0) return;
                                m_curAx=a;
                            }
                            m_stkseq[0]=Puz.Str.BaseSticker(st,m_curAx);
                            m_lstkseq=1;
                            Puz.SaveField();
                            int r=Puz.Str.CheckTwist(m_curAx,m_stkseq,m_lstkseq,Puz.Field,out m_tw);
                            if(r==1) m_stkseq[m_lstkseq++]=-1;
                            if(r==2) {
                                NormTwist(ref m_curAx,ref m_tw,ref m_dir,ref m_mask);
                                StartAnimation(m_curAx,m_tw,m_dir,m_mask);
                                Puz.Twist(m_curAx,m_tw,m_dir,m_mask);
                                qch=true;
                            } else {
                                m_status=STATUS_WAIT_STICKER;
                            }
                        }
                    }
                }
            } else {
                if(m_status==STATUS_WAIT_STICKER) {
                    Puz.RestoreField();
                    m_status=STATUS_WAIT_TWIST;
                }


                switch(act) {
                    case EAction.ActionCtrlRClick: {
                            int nf=CubeView.FindSticker(-x,y,CubeObj.FIND_STICKER_ANY);
                            if(nf<0) return;
                            ChangeColor(Puz.Field[nf]&Puzzle.CMask);
                            break;
                        }
                    case EAction.ActionShiftClick: {
                            int nf=CubeView.FindSticker(-x,y,CubeObj.FIND_STICKER_ANY);
                            Puz.Str.HighlightPiece(Puz.Field,nf);
                            break;
                        }
                    case EAction.ActionShiftRClick: {
                            int nf=CubeView.FindSticker(-x,y,CubeObj.FIND_STICKER_ANY);
                            nf=(nf<0 ? -1 : Puz.Field[nf]&Puzzle.CMask);
                            CubeView.WhiteColor=(nf==CubeView.WhiteColor) ? -1 : nf;
                            CubeView.SetStickerColors();
                            break;
                        }
                    case EAction.ActionCtrlClick: {
                            int nf;
                            double[] pt=CubeView.FindFace(-x,y,out nf);
                            if(nf<0) return;
                            pt=Puz.Str.Faces[nf].Pole;
                            ((S4Camera)dxControl2.Scene.Camera).Recenter(pt);
                            break;
                        }
                    case EAction.ActionCtrlAltClick: {
                            int nf;
                            double[] pt=CubeView.FindFace(-x,y,out nf);
                            if(nf<0) return;
                            MacroStart=Puz.Ptr;
                            m_macroStickers[0]=nf;
                            m_NMacroStickers=1;
                            m_macroVec=pt;
                            break;    
                    }
                    case EAction.ActionAltClick:
                    case EAction.ActionAltRClick: {
                            if(m_curMacroName==null) return;
                            CMacro m=Macros.GetMacro(m_curMacroName);
                            if(m==null) return;
                            int nf;
                            double[] pt=CubeView.FindFace(-x,y,out nf);
                            if(nf<0) return;
                            double[,] M=Puz.Str.GetBestMatrix(m.Stickers[0],m.Point,nf,pt);
                            if(M==null) return;
                            Puz.ApplyMacro(m.Code,m.LMacro,M,act==EAction.ActionAltRClick);
                            qch=true;
                            break;
                        }
                }

            }
            ShowCube(qch);
            RedrawClickStatus();

        }

        private void NormTwist(ref int ax,ref int tw,ref int dir,ref int mask) {
            int rax=Math.Abs(ax)-1;
            mask&=(1<<Puz.Str.Axes[rax].Layers.Length)-1;
            if(ax<0) {
                tw=-tw;
                mask=Puz.Str.Axes[rax].Base.ReverseMask(mask);
            }
            ax=rax;
            if(tw<0) {
                tw=-tw-1;
                dir=-dir;
            } else tw--;
        }

        void RedrawClickStatus() {
            if(MacroStart<0) lbl_MacroStatus.Text="  Ready";
            else lbl_MacroStatus.Text="  Enter macro: "+Puz.GetNTwists(MacroStart,Puz.Ptr); 
            UpdateTime(null);
        }

        void UpdateTime(object xxx) {
            if(Puz==null || m_Closing) return;
            long s=m_TRun ? (DateTime.Now.Ticks-m_TStart) : Puz.CTime;
            s/=10000; // ms
            int tms=(int)(s%1000); s/=1000;
            int ts=(int)(s%60); s/=60;
            int tm=(int)(s%60); s/=60;
            string m=CPShowTimer ? String.Format("   Time: {0}:{1:D2}:{2:D2}.{3:D3}",s,tm,ts,tms) : "";
            try {
                    if(this.lbl_CTime.InvokeRequired) {
                        Invoke(new Set2Texts(SetTimerLine),new object[] { m,"" });
                    } else {
                        SetTimerLine(m,"");
                    }
            } catch { }

        }

        void SetTimerLine(string tm,string dummy) {
            lbl_CTime.Text=tm;
        }

        void Redraw() {
            ShowRevStack();
            //CubeView.Dispose();  // colors changed
            dxControl2.SetSceneChanged();
        }
        void TestBuild() {
            if(qSolved) return;
            if(!m_TRun) {
                m_TStart=DateTime.Now.Ticks-Puz.CTime;
                m_TRun=true;
            }
            if(Puz.Check()) {
                m_TRun=false;
                Puz.CTime=DateTime.Now.Ticks-m_TStart;
                //RedrawClickStatus();
                MessageBox.Show(string.Format("You have solved {0} puzzle scrambled by {1} twists.\r\nCongratulations!",Puz.Str.Name,Puz.LShuffle));
                qSolved=true;
            }
        }

        void UpdateCamera() {
            S4Camera hcam=dxControl2.Scene.Camera as S4Camera;
            if(hcam!=null) hcam.SetChanged();
        }

        /*********** load/save scene ************/
        
  		void NewScene(){
            NewScene(Puz==null ? null : Puz.Str);
        }

        void NewScene(PuzzleStructure pstr) {
			dxControl2.ClearMeshes();
            if(pstr==null) {
                pstr=LoadStruct(PuzzleList,m_puzzleName);
                if(pstr==null){
                    pstr=PuzzleStructure.Example;
                    m_puzzleName="3^4";
                }
            }
            Puz=new Puzzle(pstr);
            InitScene();
            m_FileName=null;
        }
        void InitScene(){
            Text=Puz.Str.Name+" - MPUlt";
            CubeView=new CubeObj(Puz);
            dxControl2.AddMesh(CubeView);
            double R=Puz.Str.GetRad();
            ((S4Camera)dxControl2.Scene.Camera).Init(Puz.Str.Dim,R);
            ((S4Camera)dxControl2.Scene.Camera).SetAddMask(m_AddMask);
            
            ApplyGeomSettings();
            qSolved=true;

            CheckMacro();

//            RecordingMacroStatus=REC_MACRO_NONE;
            
            m_TRun=false;
            InitStatus();
//            dxControl2.ParkCamera(true);
            ShowCube(false);
        }
        void InitStatus() {
            m_status=STATUS_WAIT_TWIST;
            MacroStart=-1;
            StartA=StartB=EndA=EndB=0;
            m_NMacroStickers=0;
            m_curAx=-1;
        }
        private void m_miResetView_Click(object sender,EventArgs e) {
            RefreshView();
        }


        void CheckMacro() {
            if(Macros==null || !Macros.CheckSize(Puz.Str.Name)) {
                Macros=new CMacroFile(Puz.Str.Name,Puz.Str.Dim);
                m_curMacroName=null;
                InitMacroList();
            }
        }

        
        void ShowCube(bool qcheck) {
            if(CubeView==null) {
                CubeView=new CubeObj(Puz);
                dxControl2.ClearMeshes();
                dxControl2.AddMesh(CubeView);
            }

            UpdateCamera();
            CubeView.SetStickerColors();
            dxControl2.SetSceneChanged();
            ShowRevStack();

            if(qcheck) TestBuild();
        }

        private void mi_Reset_Click(object sender,EventArgs e) {
            NewScene();
        }

        private void mi_Undo_Click(object sender,EventArgs e) {
            int ax,tw,dir,mask;
            if(Puz.GetUndo(out ax,out tw,out dir,out mask)) {
                StartAnimation(ax,tw,dir,mask);
            }
            Puz.Undo();
            ShowCube(true);
        }

        private void mi_Redo_Click(object sender,EventArgs e) {
            int ax,tw,dir,mask;
            if(Puz.GetRedo(out ax,out tw,out dir,out mask)) {
                StartAnimation(ax,tw,dir,mask);
            }
            Puz.Redo();
            ShowCube(true);
        }

        private void mi_FullScramble_Click(object sender,EventArgs e) {
            Scramble(-1);
        }

        private void mi_Scramble1_Click(object sender,EventArgs e) {
            Scramble(1);
        }
        private void mi_Scramble2_Click(object sender,EventArgs e) {
            Scramble(2);
        }
        private void mi_Scramble3_Click(object sender,EventArgs e) {
            Scramble(3);
        }
        private void mi_Scramble4_Click(object sender,EventArgs e) {
            Scramble(4);
        }
        private void mi_Scramble5_Click(object sender,EventArgs e) {
            Scramble(5);
        }
        void Scramble(int N) {
            m_FileName=null;
            Puz.Scramble(N);

            InitStatus();
            qSolved=false;
            m_TRun=false;
            ShowCube(false);
        }

        bool m_runUndo=false;
        private void mi_FullUndo_Click(object sender,EventArgs e) {
            m_runUndo=true;
            while(Puz.Undo()) {
                CubeView.SetStickerColors();
                RedrawAll();
                Application.DoEvents();
                if(!m_runUndo) break;
                Thread.Sleep(TRate);
            }
        }

        void RedrawAll() {
            UpdateCamera();
            Redraw();
            dxControl2.Scene.Render3DEnvironment();
        }
        private void mi_FullRedo_Click(object sender,EventArgs e) {
            m_runUndo=true;
            while(Puz.Redo()) {
                CubeView.SetStickerColors();
                RedrawAll();
                Application.DoEvents();
                if(!m_runUndo) break;
                Thread.Sleep(TRate);
            }
        }

        private void stopToolStripMenuItem_Click(object sender,EventArgs e) {
            m_runUndo=false;
        }

        string m_FileName=null;

        private void mi_Open_Click(object sender,EventArgs e) {
            OpenFileDialog sf=new OpenFileDialog();
            sf.DefaultExt=".log";
            sf.Filter="MPUlt Log file (*.log)|*.log";
            sf.RestoreDirectory=true;
            if(sf.ShowDialog()==DialogResult.OK) {
                m_FileName=sf.FileName;
                Text=m_FileName+" - MPUltimate";
                Puz=Puzzle.LoadFromLog(m_FileName);
                dxControl2.ClearMeshes();
                CubeView=new CubeObj(Puz);
                dxControl2.AddMesh(CubeView);
                ((S4Camera)dxControl2.Scene.Camera).Init(Puz.Str.Dim,2);
                ApplyGeomSettings(); 
                ShowCube(false);
//                dxControl2.ParkCamera(true);
                qSolved=Puz.Check();
                // TODO: Check macro
                CheckMacro();
            }

        }

        private void mi_Save_Click(object sender,EventArgs e) {
            if(m_FileName==null) mi_SaveAs_Click(sender,e);
            if(m_TRun) Puz.CTime=DateTime.Now.Ticks-m_TStart;
            Puz.Save(m_FileName);
        }

        private void mi_SaveAs_Click(object sender,EventArgs e) {
            SaveFileDialog sf=new SaveFileDialog();
            sf.RestoreDirectory=true;
            sf.DefaultExt=".log";
            sf.Filter="MPUlt Log file (*.log)|*.log";
            if(sf.ShowDialog()==DialogResult.OK) {
                m_FileName=sf.FileName;
                if(m_TRun) Puz.CTime=DateTime.Now.Ticks-m_TStart;
                Puz.Save(m_FileName);
            }
        }

        bool SettingsSaved=false;
        private void mi_Exit_Click(object sender,EventArgs e) {
            m_Closing=true;
            Thread.Sleep(500);
            if(!SettingsSaved) {
                SaveSettings("MPUlt_settings.txt");
                SettingsSaved=true;
            }
            Puz=null;
            Application.Exit();
        }
        private void Form1_FormClosing(object sender,FormClosingEventArgs e) {
            m_Closing=true;
            Thread.Sleep(500);
            if(!SettingsSaved) {
                SaveSettings("MPUlt_settings.txt");
                SettingsSaved=true;
            }
            Puz=null;
        }

        // values from control panel
        double CPShrinkFace {
            set { trk_faceShrink.Value=Math.Max(1,(int)(value*trk_faceShrink.Maximum+0.5)); }
            get { return (double)trk_faceShrink.Value/trk_faceShrink.Maximum; }
        }
        double CPStickerSize {
            set { trk_StickerSize.Value=Math.Max(1,(int)(value*trk_StickerSize.Maximum+0.5)); }
            get { return (double)trk_StickerSize.Value/trk_StickerSize.Maximum; }
        }
        double CPViewAngle {
            set { trk_ViewAngle.Value=Math.Max(1,(int)(value/Math.PI*trk_ViewAngle.Maximum+0.5)); }
            get { return Math.PI*trk_ViewAngle.Value/trk_ViewAngle.Maximum; }
        }
        double CPUndoSpeed {
            set { 
                double p=(4-Math.Log10(Math.Max(value,1)))/3;
                trk_UndoSpeed.Value=(int)(Math.Min(1,Math.Max(0,p))*trk_UndoSpeed.Maximum+0.5); }
            get { 
                double p=(double)(trk_UndoSpeed.Value)/trk_UndoSpeed.Maximum;
                return Math.Pow(10,4-3*p);
            }
        }
        int CPDiffLight {
            get { return trk_LightDiff.Value; }
            set { trk_LightDiff.Value=Math.Max(0,Math.Min(255,value)); }
        }
        int CPSpecLight {
            get { return trk_LightSpec.Value; }
            set { trk_LightSpec.Value=Math.Max(0,Math.Min(255,value)); }
        }
        int CPAmbLight {
            get { return trk_LightAmb.Value; }
            set { trk_LightAmb.Value=Math.Max(0,Math.Min(255,value)); }
        }

        bool CPShowTimer {
            get { return m_miShowTimer.Checked; }
            set { m_miShowTimer.Checked=value; }
        }

        bool m_setgeom=false;

        private void trk_faceShrink_ValueChanged(object sender,EventArgs e) {
            if(m_setgeom) {
                double scf=Puz.Str.QSimplified ? 2 : 1;
                if(CubeView!=null) CubeView.SetStickerSize(CPShrinkFace,CPStickerSize*scf);
                UpdateCamera();
                Redraw();
            }
        }

        private void trk_ViewAngle_ValueChanged(object sender,EventArgs e) {
            if(m_setgeom) {
                S4Camera cam=dxControl2.Scene.Camera as S4Camera;
                if(cam!=null) {
                    cam.Angle=(float)(CPViewAngle);
                    Redraw();
                }
            }
        }

        private void trk_LightDiff_ValueChanged(object sender,EventArgs e) {
            if(m_setgeom) SetLight();
        }

        private void trk_LightSpec_ValueChanged(object sender,EventArgs e) {
            if(m_setgeom) SetLight();
        }

        void ApplyGeomSettings() {
            double scf=Puz.Str.QSimplified ? 2 : 1;
            if(CubeView!=null) CubeView.SetStickerSize(CPShrinkFace,CPStickerSize*scf);
            S4Camera cam=dxControl2.Scene.Camera as S4Camera;
            if(cam!=null) {
                cam.Angle=(float)(CPViewAngle);
            }
            TRate=(int)CPUndoSpeed;
            if(CubeView!=null) CubeView.SetStickerColors();
            Redraw();
        }

        void SaveSettings(string fn) {
            try {
                StreamWriter sw=new StreamWriter(fn);
                sw.NewLine="\r\n";
                sw.WriteLine("MPUlt Settings");
                sw.WriteLine("Puzzle {0}",m_puzzleName ?? "3^4");
                sw.WriteLine("FaceShrink {0:F3}",CPShrinkFace);
                sw.WriteLine("StickerSize {0:F3}",CPStickerSize);
                sw.WriteLine("ViewAngle {0:F3}",CPViewAngle);
                sw.WriteLine("UndoSpeed {0}",CPUndoSpeed);
                sw.WriteLine("ShowTimer {0}",CPShowTimer ? "T" : "F");

                string ln="Colors";
                foreach(int c in CubeObj.Colors) ln+=" "+c;
                sw.WriteLine(ln);
                sw.WriteLine("DiffLight {0}",CPDiffLight);
                sw.WriteLine("SpecLight {0}",CPSpecLight);
                sw.WriteLine("AmbLight {0}",CPAmbLight);
                if(m_FileName!=null) sw.WriteLine("FileName "+m_FileName);
                sw.Close();
            } catch { }
        }
        void LoadSettings(string fn) {
            PuzzleStructure pstr=null;
            if(File.Exists(fn)) {
                try {
                    StreamReader sr=new StreamReader(fn);
                    string ln=sr.ReadLine();
                    if(ln=="MPUlt Settings") {
                        for(;;) {
                            ln=sr.ReadLine();
                            if(ln==null) break;
                            string[] pars=ln.Split(' ');
                            switch(pars[0]) {
                                case "Puzzle": pstr=LoadStruct(PuzzleList,ln.Substring(7)); break;
                                case "FaceShrink": CPShrinkFace=double.Parse(pars[1]); break;
                                case "StickerSize": CPStickerSize=double.Parse(pars[1]); break;
                                case "ViewAngle": CPViewAngle=double.Parse(pars[1]); break;
                                case "UndoSpeed": CPUndoSpeed=double.Parse(pars[1]); break;
                                case "ShowTimer": CPShowTimer=pars[1][0]=='T'; break;
                                case "Colors": {
                                        int[] cc=new int[pars.Length-1];
                                        for(int i=1;i<pars.Length;i++) cc[i-1]=int.Parse(pars[i]);
                                        CubeObj.SetColors(cc);
                                        break;
                                    }
                                case "DiffLight": CPDiffLight=int.Parse(pars[1]); break;
                                case "SpecLight": CPSpecLight=int.Parse(pars[1]); break;
                                case "AmbLight": CPAmbLight=int.Parse(pars[1]); break;
                                case "FileName": m_FileName=ln.Substring(9); break;
                            }
                        }
                    }
                    sr.Close();
                } catch { }
            }

            SetLight();
            if(m_FileName==null) {
                NewScene(pstr);
            } else {
                Puz=Puzzle.LoadFromLog(m_FileName);
                InitScene();
                qSolved=Puz.Check();
            }
        }

        void ChangeColor(int cid){
            ColorDialog c=new ColorDialog();
            c.Color=Color.FromArgb((int)CubeObj.Colors[cid]);
            if(c.ShowDialog()==DialogResult.OK) {
                CubeObj.Colors[cid]=(c.Color.ToArgb());
                CubeView.SetStickerColors();
                Redraw();
            }
        }


        void SetLight() {
            CameraLight L=dxControl2.Scene.Light;
            if(L!=null){
                L.Specular=CPSpecLight;
                L.Diffuse=CPDiffLight;
                L.Ambient=CPAmbLight;
            }
            dxControl2.SetSceneChanged();
        }

        private void mi_StartRecordig_Click(object sender,EventArgs e) {
            if(MacroStart<0){
                MessageBox.Show("Use Ctrl-Alt-Click on "+(Puz.Str.Dim==4 ? "face" : "corner stricker")+" to record macro");
                return;
            }
            int ms=MacroStart;
            int ns=m_NMacroStickers;
            MacroStart=-1;
            m_NMacroStickers=0;

            int lm0=Puz.Ptr-ms;
            if(lm0<=0) MessageBox.Show("Negative Macro Length");
            else {
                TextDialog edt=new TextDialog("Enter Macro Name");
                DialogResult res=edt.ShowDialog(this);
                if(res==DialogResult.OK) {
                    int px=0;
                    long[] mcode=new long[lm0];
                    for(int i=0;i<lm0;i++) {
                        long c=Puz.Seq[i+ms];
                        if(c<0) continue;
                        mcode[px++]=c;
                    }

                    CMacro m=new CMacro(edt.Value,ns,m_macroStickers,m_macroVec,px,mcode);
                    Macros.AddMacro(m);
                    if(m_lbMacros.FindStringExact(m.Name)==ListBox.NoMatches) {
                        m_lbMacros.Items.Add(m.Name);
                        m_lbMacros.SelectedIndex=m_lbMacros.FindStringExact(m.Name);
                        m_curMacroName=(string)m_lbMacros.SelectedItem;
                    }
                }
            }
            RedrawClickStatus();
        }
        void InitMacroList(){
            string[] str=Macros.GetNamesList();
            m_lbMacros.Items.Clear();
            foreach(string s in str) m_lbMacros.Items.Add(s);
            m_curMacroName=null;
        }

        private void m_lbMacros_MouseClick(object sender,MouseEventArgs e) {
            m_curMacroName=(string)m_lbMacros.SelectedItem;
        }
#if false
        void appMacro(bool qrev,HSticker hstk){
            if(m_macroName==null) return;
            CMacro curMacro=Macros.GetMacro(m_macroName);
            if(curMacro==null) return;

            HCamera hcam=dxControl2.Scene.Camera as HCamera;
            if(hcam==null) return;

            int type=(HCube.GetStkType(hstk.RefKey)-1)%2+1;
            if(type!=curMacro.StickerType) return;
            int ornt;
            int []trans=CubeView.GetStickerOrient(hcam.Trans,hstk,out ornt);
            trans=HCube.ChangeTransByOrient(trans,type,ornt-curMacro.Orient);
            HCube.ApplyMacro(trans,curMacro.Code,curMacro.LMacro,qrev);
            CubeView.SetColors();
            Redraw();
        }
#endif
        private void m_btn_RenameMacro_Click(object sender,EventArgs e) {
            if(m_curMacroName==null) return;
            TextDialog edt=new TextDialog("Enter New Macro Name");
            edt.Value=m_curMacroName;
            DialogResult res=edt.ShowDialog(this);
            if(res==DialogResult.OK) {
                string nm=Macros.Rename(m_curMacroName,edt.Value);
                InitMacroList();
                m_lbMacros.SelectedItem=nm;
                m_curMacroName=(string)m_lbMacros.SelectedItem;
            }
        }

        private void m_btn_DeleteMacro_Click(object sender,EventArgs e) {
            Macros.Delete(m_curMacroName);
            m_lbMacros.Items.Remove(m_curMacroName);
            m_curMacroName=(string)m_lbMacros.SelectedItem;
        }



        private void saveMacroFileToolStripMenuItem_Click(object sender,EventArgs e) {
            if(Macros.FileName==null) saveMacroFileAsToolStripMenuItem_Click(sender,e);
            Macros.Save();
        }

        private void loadMacroFileToolStripMenuItem_Click(object sender,EventArgs e) {
            OpenFileDialog sf=new OpenFileDialog();
            sf.DefaultExt=".dat";
            sf.Filter="MPUlt Macro file (*.dat)|*.dat";
            sf.RestoreDirectory=true;
            if(sf.ShowDialog()==DialogResult.OK) {
                CMacroFile mf=new CMacroFile(sf.FileName);
                if(!mf.CheckSize(Puz.Str.Name)) {
                    MessageBox.Show("Wrong puzzle type");
                    return;
                }
                Macros=mf;
                InitMacroList();
            }
        }

        private void saveMacroFileAsToolStripMenuItem_Click(object sender,EventArgs e) {
            SaveFileDialog sf=new SaveFileDialog();
            sf.DefaultExt=".dat";
            sf.Filter="MPUlt Macro file (*.dat)|*.dat";
            sf.RestoreDirectory=true;
            if(sf.ShowDialog()==DialogResult.OK) {
                Macros.SaveAs(sf.FileName);
            }
        }
        private void aboutToolStripMenuItem_Click(object sender,EventArgs e) {
            MessageBox.Show("Magic Puzzle Ultimate v1.09\r\n(c)2011, Andrey Astrelin");
        }

        private void startExtraTurnsToolStripMenuItem_Click(object sender,EventArgs e) {
            Puz.StartSetup();
            ShowRevStack();
        }

        private void stopExtraTurnsToolStripMenuItem_Click(object sender,EventArgs e) {
            Puz.StopSetup();
            ShowRevStack();
        }

        private void undoExtraTurnsToolStripMenuItem_Click(object sender,EventArgs e) {
            Puz.Conjugate();
            ShowCube(true);
        }

        private void commutatorToolStripMenuItem_Click(object sender,EventArgs e) {
            Puz.Commutator();
            ShowCube(true);
        }

        private void ShowRevStack() {
            if(Puz!=null) {
                string l=Puz.GetRevStack();
                if(this.m_tbRevStack.InvokeRequired) {
                    Invoke(new Set2Texts(SetRevStackText),new object[] { l,"  Twists: "+(Puz.NTwists-Puz.LShuffle) });
                } else {
                    SetRevStackText(l,"  Twists: "+(Puz.NTwists-Puz.LShuffle));
                }
            }
        }

        void SetRevStackText(string ln,string ln1) {
            m_tbRevStack.Text=ln;
            lbl_Twists.Text=ln1;
        }

        private void recalculateToolStripMenuItem_Click(object sender,EventArgs e) {
            Puz.Recalculate();
            ShowCube(true);
        }

        private void m_trkUndoSpeed_ValueChanged(object sender,EventArgs e) {
            TRate=(int)CPUndoSpeed;
        }

        private void helpToolStripMenuItem1_Click(object sender,EventArgs e) {
            string Help=
                "MPUlt Mouse Commands:\r\n\r\n"+
                "  Navigation:\r\n"+
                "  Left Button:  Rotate around center\r\n"+
                "  Right Button:  Slide forward/backward\r\n"+
                "  Ctrl-Left Button:  Zoom to/from center\r\n"+
                "  Ctrl-Right Button:  Change view angle\r\n"+
                "  Shift-Left Button:  Rotate camera\r\n"+
                "  Ctrl-Left Click: Set new center cell\r\n\r\n"+
                "Stickers (4D):\r\n"+
                "  Left/Right Click: Rotate Cell\r\n"+
                "  Left+Right Click: Rotate Ridge/Edge/Vertex\r\n"+
                "  Ctrl-Alt-Left Click: Start macro definition\r\n"+
                "  Alt-Left Click: Apply macro\r\n"+
                "  Alt-Right Click: Apply reverse macro\r\n\r\n"+
                "Stickers (5D and more):\r\n"+
                "  Left Click: Rotate Cell\r\n"+
                "  Right Click: Rotate Vertex\r\n\r\n"+
                "Stickers (All):\r\n"+
                "  Shift-Left Click: Outline all stickers of the piece\r\n"+
                "  Shift-Right Click: Temporary set color to White\r\n"+
                "  Ctrl-Right Click: Edit color\r\n";
;
            MessageBox.Show(Help);
        }

        private void m_miShowTimer_Click(object sender,EventArgs e) {
            m_miShowTimer.Checked=!m_miShowTimer.Checked;
        }

        private void optimizeToolStripMenuItem_Click(object sender,EventArgs e) {
            if(MacroStart>=0) return;
            StartA=EndA=StartB=EndB=0;
            Puz.Optimize();
            ShowRevStack();
        }
        private void m_btnSwDim_Click(object sender,EventArgs e) {
            S4Camera cam=dxControl2.Scene.Camera as S4Camera;
            if(cam!=null) {
                cam.SwitchZ();
                Redraw();
            }
        }

        private void m_cbRank0_Click(object sender,EventArgs e) { ChangeRank(0,m_cbRank0.Checked); }
        private void m_cbRank1_Click(object sender,EventArgs e) { ChangeRank(1,m_cbRank1.Checked); }
        private void m_cbRank2_Click(object sender,EventArgs e) { ChangeRank(2,m_cbRank2.Checked); }
        private void m_cbRank3_Click(object sender,EventArgs e) { ChangeRank(3,m_cbRank3.Checked); }
        private void m_cbRank4_Click(object sender,EventArgs e) { ChangeRank(4,m_cbRank4.Checked); }
        private void m_cbRank5_Click(object sender,EventArgs e) { ChangeRank(5,m_cbRank5.Checked); }
        private void m_cbRank6_Click(object sender,EventArgs e) { ChangeRank(6,m_cbRank6.Checked); }
        private void m_cbRank7_Click(object sender,EventArgs e) { ChangeRank(7,m_cbRank7.Checked); }
        private void m_cbRank8_Click(object sender,EventArgs e) { ChangeRank(8,m_cbRank8.Checked); }
        private void m_cbRank9_Click(object sender,EventArgs e) { ChangeRank(9,m_cbRank9.Checked); }
        private void m_cbRank10_Click(object sender,EventArgs e) { ChangeRank(10,m_cbRank10.Checked); }
        private void m_cbRank11_Click(object sender,EventArgs e) { ChangeRank(11,m_cbRank11.Checked); }

        private void ChangeRank(int rnk,bool val) {
            if(val) m_ShowRank|=1<<rnk; else m_ShowRank&=~(1<<rnk);
            CubeView.ShowRank=m_ShowRank;
            Redraw();
        }

        private PuzzleStructure LoadStruct(Hashtable tbl,string name) {
            foreach(DictionaryEntry de in tbl) {
                if(de.Value is Hashtable) {
                    PuzzleStructure r=LoadStruct((Hashtable)de.Value,name);
                    if(r!=null) return r;
                } else if((string)de.Key==name) {
                    return LoadStruct((string[])de.Value,name);
                }
            }
            return null;
        }

        private PuzzleStructure LoadStruct(string[] code,string name) {
            try {
                m_puzzleName=name;
                return PuzzleStructure.Create(name,code);
            } catch(Exception e) {
                MessageBox.Show(e.Message);                
            }

            m_puzzleName="3^4";
            return PuzzleStructure.Example;
        }

        void ReadPuzzles(string fn) {
            if(File.Exists(fn)){
                StreamReader sr=new StreamReader("MPUlt_puzzles.txt");
                PuzzleList=ReadGroup(sr);
                sr.Close();
            } else {
                PuzzleList=new Hashtable();
                PuzzleList.Add("Cube4D_3_FT",PuzzleStructure.ExampleDescr);
            }
        }
        Hashtable ReadGroup(StreamReader sr) {
            Hashtable res=new Hashtable();
            string pname=null;
            ArrayList lines=new ArrayList();
            for(;;) {
                string ln=sr.ReadLine();
                if(ln=="" || (ln!=null && ln[0]=='#')) continue;
                string cmd=ln==null ? null : ln.Split(' ')[0].ToLower();
                if(cmd==null || cmd=="puzzle" || cmd=="block" || cmd=="endblock") {
                    if(pname!=null) {
                        res.Add(pname,(string[])lines.ToArray(typeof(string)));
                        pname=null;
                    }
                    if(cmd==null || cmd=="endblock") break;
                    if(cmd=="puzzle") {
                        pname=ln.Split(' ')[1];
                        lines=new ArrayList();
                    } else if(cmd=="block") {
                        string nm=ln.Split(' ')[1];
                        Hashtable r=ReadGroup(sr);
                        res.Add(nm,r);
                    }
                } else lines.Add(ln);
            }
            return res;
        }

        

        ToolStripItem[] CreatePuzzleMenu(Hashtable puz) {
            ArrayList res=new ArrayList();
            ArrayList keys=new ArrayList(puz.Keys);
            keys.Sort();
            
            foreach(string fn in keys){
                ToolStripMenuItem item=new ToolStripMenuItem(fn);
                object val=puz[fn];
                if(val is Hashtable){
                    item.DropDownItems.AddRange(CreatePuzzleMenu((Hashtable)val));
                }else{
                    item.Tag=val;
                    item.Click += new System.EventHandler(this.mi_Puzzle_Click);
                }
                res.Add(item);
            }
            return (ToolStripItem[])res.ToArray(typeof(ToolStripItem));
        }

        private void mi_Puzzle_Click(object sender,EventArgs e) {
            ToolStripMenuItem c=(ToolStripMenuItem)sender;
            PuzzleStructure pstr=LoadStruct((string[])c.Tag,c.Text);
            NewScene(pstr);
        }



        private void m_btnShift_Click(object sender,EventArgs e) { ChangeButton(0); }
        private void m_btnCtrl_Click(object sender,EventArgs e) { ChangeButton(1); }
        private void m_btnAlt_Click(object sender,EventArgs e) { ChangeButton(2); }
        private void m_btnL1_Click(object sender,EventArgs e) { ChangeButton(3); }
        private void m_btnL2_Click(object sender,EventArgs e) { ChangeButton(4); }
        private void m_btnL3_Click(object sender,EventArgs e) { ChangeButton(5); }
        private void m_btnL4_Click(object sender,EventArgs e) { ChangeButton(6); }
        private void m_btnL5_Click(object sender,EventArgs e) { ChangeButton(7); }
        private void m_btnL6_Click(object sender,EventArgs e) { ChangeButton(8); }
        private void ChangeButton(int k) {
            m_AddMask[k]=!m_AddMask[k];
            SetButtons();
        }
        void SetButtons() {
            m_btnShift.BackColor=m_AddMask[0] ? System.Drawing.SystemColors.ButtonHighlight : System.Drawing.SystemColors.ButtonFace;
            m_btnCtrl.BackColor=m_AddMask[1] ? System.Drawing.SystemColors.ButtonHighlight : System.Drawing.SystemColors.ButtonFace;
            m_btnAlt.BackColor=m_AddMask[2] ? System.Drawing.SystemColors.ButtonHighlight : System.Drawing.SystemColors.ButtonFace;
            m_btnL1.BackColor=m_AddMask[3] ? System.Drawing.SystemColors.ButtonHighlight : System.Drawing.SystemColors.ButtonFace;
            m_btnL2.BackColor=m_AddMask[4] ? System.Drawing.SystemColors.ButtonHighlight : System.Drawing.SystemColors.ButtonFace;
            m_btnL3.BackColor=m_AddMask[5] ? System.Drawing.SystemColors.ButtonHighlight : System.Drawing.SystemColors.ButtonFace;
            m_btnL4.BackColor=m_AddMask[6] ? System.Drawing.SystemColors.ButtonHighlight : System.Drawing.SystemColors.ButtonFace;
            m_btnL5.BackColor=m_AddMask[7] ? System.Drawing.SystemColors.ButtonHighlight : System.Drawing.SystemColors.ButtonFace;
            m_btnL6.BackColor=m_AddMask[8] ? System.Drawing.SystemColors.ButtonHighlight : System.Drawing.SystemColors.ButtonFace;
        }

        private void startMacroAToolStripMenuItem_Click(object sender,EventArgs e) {
            StartA=Puz.Ptr;
            EndA=-1;
        }

        private void stopPerformMacroAToolStripMenuItem_Click(object sender,EventArgs e) {
            if(EndA==StartA) return;
            if(EndA<0) EndA=Puz.Ptr;
            else {
                Puz.RepeatCode(StartA,EndA);
                ShowCube(true);
            }
        }

        private void startMacroBToolStripMenuItem_Click(object sender,EventArgs e) {
            StartB=Puz.Ptr;
            EndB=-1;
        }

        private void stopPerformMacroBToolStripMenuItem_Click(object sender,EventArgs e) {
            if(EndB==StartB) return;
            if(EndB<0) EndB=Puz.Ptr;
            else {
                Puz.RepeatCode(StartB,EndB);
                ShowCube(true);
            }
        }

    }
}
