namespace ServiceSwitcher
{
    partial class MainForm
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
            this.dgvUpdaterServices = new System.Windows.Forms.DataGridView();
            this.gbUpdater = new System.Windows.Forms.GroupBox();
            this.gb1C = new System.Windows.Forms.GroupBox();
            this.dgv1CServices = new System.Windows.Forms.DataGridView();
            this.gbSQL = new System.Windows.Forms.GroupBox();
            this.dgvSQLServices = new System.Windows.Forms.DataGridView();
            this.tmrRefresher = new System.Windows.Forms.Timer(this.components);
            this.btDeleteUpdates = new System.Windows.Forms.Button();
            this.btSQLStop = new System.Windows.Forms.Button();
            this.btSQLUnselectAll = new System.Windows.Forms.Button();
            this.btSQLRun = new System.Windows.Forms.Button();
            this.btSQLSelectAll = new System.Windows.Forms.Button();
            this.bt1CStop = new System.Windows.Forms.Button();
            this.bt1CUnselectAll = new System.Windows.Forms.Button();
            this.bt1CRun = new System.Windows.Forms.Button();
            this.bt1CSelectAll = new System.Windows.Forms.Button();
            this.btUpdaterStop = new System.Windows.Forms.Button();
            this.btUpdaterUnselectAll = new System.Windows.Forms.Button();
            this.btUpdaterSelectAll = new System.Windows.Forms.Button();
            this.btUpdaterRun = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUpdaterServices)).BeginInit();
            this.gbUpdater.SuspendLayout();
            this.gb1C.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1CServices)).BeginInit();
            this.gbSQL.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSQLServices)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvUpdaterServices
            // 
            this.dgvUpdaterServices.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvUpdaterServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUpdaterServices.Location = new System.Drawing.Point(9, 48);
            this.dgvUpdaterServices.MultiSelect = false;
            this.dgvUpdaterServices.Name = "dgvUpdaterServices";
            this.dgvUpdaterServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUpdaterServices.ShowEditingIcon = false;
            this.dgvUpdaterServices.Size = new System.Drawing.Size(357, 610);
            this.dgvUpdaterServices.TabIndex = 0;
            this.dgvUpdaterServices.VirtualMode = true;
            this.dgvUpdaterServices.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvUpdaterServices_Scroll);
            // 
            // gbUpdater
            // 
            this.gbUpdater.Controls.Add(this.btDeleteUpdates);
            this.gbUpdater.Controls.Add(this.btUpdaterStop);
            this.gbUpdater.Controls.Add(this.btUpdaterUnselectAll);
            this.gbUpdater.Controls.Add(this.btUpdaterSelectAll);
            this.gbUpdater.Controls.Add(this.btUpdaterRun);
            this.gbUpdater.Controls.Add(this.dgvUpdaterServices);
            this.gbUpdater.Location = new System.Drawing.Point(13, 13);
            this.gbUpdater.Name = "gbUpdater";
            this.gbUpdater.Size = new System.Drawing.Size(380, 670);
            this.gbUpdater.TabIndex = 7;
            this.gbUpdater.TabStop = false;
            this.gbUpdater.Text = "Агент обновлений филиалов ПЭК";
            // 
            // gb1C
            // 
            this.gb1C.Controls.Add(this.bt1CStop);
            this.gb1C.Controls.Add(this.dgv1CServices);
            this.gb1C.Controls.Add(this.bt1CUnselectAll);
            this.gb1C.Controls.Add(this.bt1CRun);
            this.gb1C.Controls.Add(this.bt1CSelectAll);
            this.gb1C.Location = new System.Drawing.Point(400, 13);
            this.gb1C.Name = "gb1C";
            this.gb1C.Size = new System.Drawing.Size(380, 670);
            this.gb1C.TabIndex = 8;
            this.gb1C.TabStop = false;
            this.gb1C.Text = "Агент сервера 1С:Предприятие 8.3";
            // 
            // dgv1CServices
            // 
            this.dgv1CServices.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgv1CServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1CServices.Location = new System.Drawing.Point(12, 48);
            this.dgv1CServices.MultiSelect = false;
            this.dgv1CServices.Name = "dgv1CServices";
            this.dgv1CServices.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.dgv1CServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv1CServices.ShowEditingIcon = false;
            this.dgv1CServices.Size = new System.Drawing.Size(357, 610);
            this.dgv1CServices.TabIndex = 7;
            this.dgv1CServices.VirtualMode = true;
            // 
            // gbSQL
            // 
            this.gbSQL.Controls.Add(this.btSQLStop);
            this.gbSQL.Controls.Add(this.dgvSQLServices);
            this.gbSQL.Controls.Add(this.btSQLUnselectAll);
            this.gbSQL.Controls.Add(this.btSQLRun);
            this.gbSQL.Controls.Add(this.btSQLSelectAll);
            this.gbSQL.Location = new System.Drawing.Point(786, 13);
            this.gbSQL.Name = "gbSQL";
            this.gbSQL.Size = new System.Drawing.Size(380, 670);
            this.gbSQL.TabIndex = 9;
            this.gbSQL.TabStop = false;
            this.gbSQL.Text = "SQL Server (MSSQLSERVER)";
            // 
            // dgvSQLServices
            // 
            this.dgvSQLServices.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvSQLServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSQLServices.Location = new System.Drawing.Point(12, 48);
            this.dgvSQLServices.MultiSelect = false;
            this.dgvSQLServices.Name = "dgvSQLServices";
            this.dgvSQLServices.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.dgvSQLServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSQLServices.ShowEditingIcon = false;
            this.dgvSQLServices.Size = new System.Drawing.Size(357, 610);
            this.dgvSQLServices.TabIndex = 12;
            this.dgvSQLServices.VirtualMode = true;
            // 
            // tmrRefresher
            // 
            this.tmrRefresher.Interval = 1000;
            this.tmrRefresher.Tick += new System.EventHandler(this.tmrRefresher_Tick);
            // 
            // btDeleteUpdates
            // 
            this.btDeleteUpdates.Image = global::ServiceSwitcher.Properties.Resources.action_Cancel_16xLG;
            this.btDeleteUpdates.Location = new System.Drawing.Point(67, 19);
            this.btDeleteUpdates.Name = "btDeleteUpdates";
            this.btDeleteUpdates.Size = new System.Drawing.Size(182, 23);
            this.btDeleteUpdates.TabIndex = 10;
            this.btDeleteUpdates.Text = "Удалить файлы обновлений";
            this.btDeleteUpdates.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btDeleteUpdates.UseVisualStyleBackColor = true;
            this.btDeleteUpdates.Click += new System.EventHandler(this.btDeleteUpdates_Click);
            // 
            // btSQLStop
            // 
            this.btSQLStop.AccessibleDescription = "";
            this.btSQLStop.Image = global::ServiceSwitcher.Properties.Resources.Symbols_Stop_16xLG;
            this.btSQLStop.Location = new System.Drawing.Point(41, 19);
            this.btSQLStop.Name = "btSQLStop";
            this.btSQLStop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btSQLStop.Size = new System.Drawing.Size(23, 23);
            this.btSQLStop.TabIndex = 14;
            this.btSQLStop.UseVisualStyleBackColor = true;
            this.btSQLStop.Click += new System.EventHandler(this.btSQLStop_Click);
            // 
            // btSQLUnselectAll
            // 
            this.btSQLUnselectAll.Image = global::ServiceSwitcher.Properties.Resources.control_16xLG;
            this.btSQLUnselectAll.Location = new System.Drawing.Point(279, 19);
            this.btSQLUnselectAll.Name = "btSQLUnselectAll";
            this.btSQLUnselectAll.Size = new System.Drawing.Size(23, 23);
            this.btSQLUnselectAll.TabIndex = 16;
            this.btSQLUnselectAll.UseVisualStyleBackColor = true;
            this.btSQLUnselectAll.Click += new System.EventHandler(this.btSQLUnselectAll_Click);
            // 
            // btSQLRun
            // 
            this.btSQLRun.Image = global::ServiceSwitcher.Properties.Resources.Symbols_Play_16xLG;
            this.btSQLRun.Location = new System.Drawing.Point(12, 19);
            this.btSQLRun.Name = "btSQLRun";
            this.btSQLRun.Size = new System.Drawing.Size(23, 23);
            this.btSQLRun.TabIndex = 13;
            this.btSQLRun.UseVisualStyleBackColor = true;
            this.btSQLRun.Click += new System.EventHandler(this.btSQLRun_Click);
            // 
            // btSQLSelectAll
            // 
            this.btSQLSelectAll.Image = global::ServiceSwitcher.Properties.Resources.checkbox_16xLG;
            this.btSQLSelectAll.Location = new System.Drawing.Point(253, 19);
            this.btSQLSelectAll.Name = "btSQLSelectAll";
            this.btSQLSelectAll.Size = new System.Drawing.Size(23, 23);
            this.btSQLSelectAll.TabIndex = 15;
            this.btSQLSelectAll.UseVisualStyleBackColor = true;
            this.btSQLSelectAll.Click += new System.EventHandler(this.btSQLSelectAll_Click);
            // 
            // bt1CStop
            // 
            this.bt1CStop.AccessibleDescription = "";
            this.bt1CStop.Image = global::ServiceSwitcher.Properties.Resources.Symbols_Stop_16xLG;
            this.bt1CStop.Location = new System.Drawing.Point(41, 19);
            this.bt1CStop.Name = "bt1CStop";
            this.bt1CStop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bt1CStop.Size = new System.Drawing.Size(23, 23);
            this.bt1CStop.TabIndex = 9;
            this.bt1CStop.UseVisualStyleBackColor = true;
            this.bt1CStop.Click += new System.EventHandler(this.bt1CStop_Click);
            // 
            // bt1CUnselectAll
            // 
            this.bt1CUnselectAll.Image = global::ServiceSwitcher.Properties.Resources.control_16xLG;
            this.bt1CUnselectAll.Location = new System.Drawing.Point(279, 19);
            this.bt1CUnselectAll.Name = "bt1CUnselectAll";
            this.bt1CUnselectAll.Size = new System.Drawing.Size(23, 23);
            this.bt1CUnselectAll.TabIndex = 11;
            this.bt1CUnselectAll.UseVisualStyleBackColor = true;
            this.bt1CUnselectAll.Click += new System.EventHandler(this.bt1CUnselectAll_Click);
            // 
            // bt1CRun
            // 
            this.bt1CRun.Image = global::ServiceSwitcher.Properties.Resources.Symbols_Play_16xLG;
            this.bt1CRun.Location = new System.Drawing.Point(12, 19);
            this.bt1CRun.Name = "bt1CRun";
            this.bt1CRun.Size = new System.Drawing.Size(23, 23);
            this.bt1CRun.TabIndex = 8;
            this.bt1CRun.UseVisualStyleBackColor = true;
            this.bt1CRun.Click += new System.EventHandler(this.bt1CRun_Click);
            // 
            // bt1CSelectAll
            // 
            this.bt1CSelectAll.Image = global::ServiceSwitcher.Properties.Resources.checkbox_16xLG;
            this.bt1CSelectAll.Location = new System.Drawing.Point(253, 19);
            this.bt1CSelectAll.Name = "bt1CSelectAll";
            this.bt1CSelectAll.Size = new System.Drawing.Size(23, 23);
            this.bt1CSelectAll.TabIndex = 10;
            this.bt1CSelectAll.UseVisualStyleBackColor = true;
            this.bt1CSelectAll.Click += new System.EventHandler(this.bt1CSelectAll_Click);
            // 
            // btUpdaterStop
            // 
            this.btUpdaterStop.AccessibleDescription = "";
            this.btUpdaterStop.Image = global::ServiceSwitcher.Properties.Resources.Symbols_Stop_16xLG;
            this.btUpdaterStop.Location = new System.Drawing.Point(38, 19);
            this.btUpdaterStop.Name = "btUpdaterStop";
            this.btUpdaterStop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btUpdaterStop.Size = new System.Drawing.Size(23, 23);
            this.btUpdaterStop.TabIndex = 4;
            this.btUpdaterStop.UseVisualStyleBackColor = true;
            this.btUpdaterStop.Click += new System.EventHandler(this.btUpdaterStop_Click);
            // 
            // btUpdaterUnselectAll
            // 
            this.btUpdaterUnselectAll.Image = global::ServiceSwitcher.Properties.Resources.control_16xLG;
            this.btUpdaterUnselectAll.Location = new System.Drawing.Point(281, 19);
            this.btUpdaterUnselectAll.Name = "btUpdaterUnselectAll";
            this.btUpdaterUnselectAll.Size = new System.Drawing.Size(23, 23);
            this.btUpdaterUnselectAll.TabIndex = 6;
            this.btUpdaterUnselectAll.UseVisualStyleBackColor = true;
            this.btUpdaterUnselectAll.Click += new System.EventHandler(this.btUpdaterUnselectAll_Click);
            // 
            // btUpdaterSelectAll
            // 
            this.btUpdaterSelectAll.Image = global::ServiceSwitcher.Properties.Resources.checkbox_16xLG;
            this.btUpdaterSelectAll.Location = new System.Drawing.Point(255, 19);
            this.btUpdaterSelectAll.Name = "btUpdaterSelectAll";
            this.btUpdaterSelectAll.Size = new System.Drawing.Size(23, 23);
            this.btUpdaterSelectAll.TabIndex = 5;
            this.btUpdaterSelectAll.UseVisualStyleBackColor = true;
            this.btUpdaterSelectAll.Click += new System.EventHandler(this.btUpdaterSelectAll_Click);
            // 
            // btUpdaterRun
            // 
            this.btUpdaterRun.Image = global::ServiceSwitcher.Properties.Resources.Symbols_Play_16xLG;
            this.btUpdaterRun.Location = new System.Drawing.Point(9, 19);
            this.btUpdaterRun.Name = "btUpdaterRun";
            this.btUpdaterRun.Size = new System.Drawing.Size(23, 23);
            this.btUpdaterRun.TabIndex = 2;
            this.btUpdaterRun.UseVisualStyleBackColor = true;
            this.btUpdaterRun.Click += new System.EventHandler(this.btUpdaterRun_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1176, 727);
            this.Controls.Add(this.gbSQL);
            this.Controls.Add(this.gb1C);
            this.Controls.Add(this.gbUpdater);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Управление Агентами обновлений филиалов ПЭК";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUpdaterServices)).EndInit();
            this.gbUpdater.ResumeLayout(false);
            this.gb1C.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1CServices)).EndInit();
            this.gbSQL.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSQLServices)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvUpdaterServices;
        private System.Windows.Forms.Button btUpdaterRun;
        private System.Windows.Forms.Button btUpdaterStop;
        private System.Windows.Forms.Button btUpdaterSelectAll;
        private System.Windows.Forms.Button btUpdaterUnselectAll;
        private System.Windows.Forms.GroupBox gbUpdater;
        private System.Windows.Forms.GroupBox gb1C;
        private System.Windows.Forms.GroupBox gbSQL;
        private System.Windows.Forms.Button bt1CStop;
        private System.Windows.Forms.DataGridView dgv1CServices;
        private System.Windows.Forms.Button bt1CUnselectAll;
        private System.Windows.Forms.Button bt1CRun;
        private System.Windows.Forms.Button bt1CSelectAll;
        private System.Windows.Forms.Button btSQLStop;
        private System.Windows.Forms.DataGridView dgvSQLServices;
        private System.Windows.Forms.Button btSQLUnselectAll;
        private System.Windows.Forms.Button btSQLRun;
        private System.Windows.Forms.Button btSQLSelectAll;
        private System.Windows.Forms.Timer tmrRefresher;
        private System.Windows.Forms.Button btDeleteUpdates;
    }
}

