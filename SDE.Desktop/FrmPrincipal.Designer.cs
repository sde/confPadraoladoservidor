namespace SDE.Desktop
{
    partial class FrmPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPrincipal));
            this.IntrovertIMApp = new AxShockwaveFlashObjects.AxShockwaveFlash();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mostrarSDEDesktopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.IntrovertIMApp)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // IntrovertIMApp
            // 
            this.IntrovertIMApp.Enabled = true;
            this.IntrovertIMApp.Location = new System.Drawing.Point(12, 191);
            this.IntrovertIMApp.Name = "IntrovertIMApp";
            this.IntrovertIMApp.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("IntrovertIMApp.OcxState")));
            this.IntrovertIMApp.Size = new System.Drawing.Size(600, 300);
            this.IntrovertIMApp.TabIndex = 4;
            this.IntrovertIMApp.Visible = false;
            this.IntrovertIMApp.Enter += new System.EventHandler(this.Form1_Load);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipText = "SDE Desktop Ativo";
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "SDE Desktop";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mostrarSDEDesktopToolStripMenuItem,
            this.toolStripSeparator1});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(187, 32);
            // 
            // mostrarSDEDesktopToolStripMenuItem
            // 
            this.mostrarSDEDesktopToolStripMenuItem.Name = "mostrarSDEDesktopToolStripMenuItem";
            this.mostrarSDEDesktopToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.mostrarSDEDesktopToolStripMenuItem.Text = "Mostrar SDE Desktop";
            this.mostrarSDEDesktopToolStripMenuItem.Click += new System.EventHandler(this.mostrarSDEDesktopToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(168, -1);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(1024, 768);
            this.webBrowser1.TabIndex = 7;
            // 
            // FrmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 430);
            this.Controls.Add(this.IntrovertIMApp);
            this.Controls.Add(this.webBrowser1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Green;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmPrincipal";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Resize += new System.EventHandler(this.FrmPrincipal_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.IntrovertIMApp)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AxShockwaveFlashObjects.AxShockwaveFlash IntrovertIMApp;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mostrarSDEDesktopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.WebBrowser webBrowser1;
    }
}

