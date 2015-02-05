namespace DispetcherWF
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn1 = new System.Data.DataColumn();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbUpdateStatus = new System.Windows.Forms.Label();
            this.lbCheckMailStatus = new System.Windows.Forms.Label();
            this.pnErrors = new System.Windows.Forms.Panel();
            this.tbErrors = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.routeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.scheduleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SideNumberPlan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SideNumberFact = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.tbStatistics = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pnErrors.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataMember = "Table1";
            this.bindingSource1.DataSource = this.dataSet1;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn1,
            this.dataColumn2});
            this.dataTable1.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "Id"}, false)});
            this.dataTable1.TableName = "Table1";
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "Route";
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "SideNumberPlan";
            // 
            // dataColumn5
            // 
            this.dataColumn5.ColumnName = "Schedule";
            // 
            // dataColumn6
            // 
            this.dataColumn6.ColumnName = "SideNumberFact";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "Id";
            this.dataColumn1.DataType = typeof(long);
            this.dataColumn1.ReadOnly = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(572, 634);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.pnErrors);
            this.tabPage1.Controls.Add(this.panel3);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(564, 608);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Редактирование";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbStatistics);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(564, 608);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Статистика";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Info;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(558, 52);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(518, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Можно вносить изменения в колонку \"Фактический № ПС\" по двойному щелчку на нужной" +
    " ячейке. ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(548, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Если для данного маршрута не планируется выход ПС, то оставьте колонку \"Фактическ" +
    "ий № ПС\" пустой.";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lbCheckMailStatus);
            this.panel3.Controls.Add(this.lbUpdateStatus);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(3, 562);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(558, 43);
            this.panel3.TabIndex = 3;
            // 
            // lbUpdateStatus
            // 
            this.lbUpdateStatus.Location = new System.Drawing.Point(2, 6);
            this.lbUpdateStatus.Name = "lbUpdateStatus";
            this.lbUpdateStatus.Size = new System.Drawing.Size(540, 13);
            this.lbUpdateStatus.TabIndex = 0;
            // 
            // lbCheckMailStatus
            // 
            this.lbCheckMailStatus.Location = new System.Drawing.Point(2, 25);
            this.lbCheckMailStatus.Name = "lbCheckMailStatus";
            this.lbCheckMailStatus.Size = new System.Drawing.Size(540, 13);
            this.lbCheckMailStatus.TabIndex = 1;
            // 
            // pnErrors
            // 
            this.pnErrors.Controls.Add(this.tbErrors);
            this.pnErrors.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnErrors.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pnErrors.Location = new System.Drawing.Point(3, 464);
            this.pnErrors.Name = "pnErrors";
            this.pnErrors.Size = new System.Drawing.Size(558, 98);
            this.pnErrors.TabIndex = 5;
            this.pnErrors.Visible = false;
            // 
            // tbErrors
            // 
            this.tbErrors.BackColor = System.Drawing.Color.Pink;
            this.tbErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbErrors.Location = new System.Drawing.Point(0, 0);
            this.tbErrors.Multiline = true;
            this.tbErrors.Name = "tbErrors";
            this.tbErrors.ReadOnly = true;
            this.tbErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbErrors.Size = new System.Drawing.Size(558, 98);
            this.tbErrors.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 55);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panel2.Size = new System.Drawing.Size(558, 409);
            this.panel2.TabIndex = 6;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.routeDataGridViewTextBoxColumn,
            this.scheduleDataGridViewTextBoxColumn,
            this.SideNumberPlan,
            this.SideNumberFact});
            this.dataGridView1.DataSource = this.bindingSource1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 5);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.Size = new System.Drawing.Size(558, 404);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // routeDataGridViewTextBoxColumn
            // 
            this.routeDataGridViewTextBoxColumn.DataPropertyName = "Route";
            this.routeDataGridViewTextBoxColumn.HeaderText = "Маршрут";
            this.routeDataGridViewTextBoxColumn.Name = "routeDataGridViewTextBoxColumn";
            this.routeDataGridViewTextBoxColumn.ReadOnly = true;
            this.routeDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // scheduleDataGridViewTextBoxColumn
            // 
            this.scheduleDataGridViewTextBoxColumn.DataPropertyName = "Schedule";
            this.scheduleDataGridViewTextBoxColumn.HeaderText = "Расписание";
            this.scheduleDataGridViewTextBoxColumn.Name = "scheduleDataGridViewTextBoxColumn";
            this.scheduleDataGridViewTextBoxColumn.ReadOnly = true;
            this.scheduleDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // SideNumberPlan
            // 
            this.SideNumberPlan.DataPropertyName = "SideNumberPlan";
            this.SideNumberPlan.FillWeight = 150F;
            this.SideNumberPlan.HeaderText = "Плановый № ПС";
            this.SideNumberPlan.Name = "SideNumberPlan";
            this.SideNumberPlan.ReadOnly = true;
            this.SideNumberPlan.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SideNumberPlan.Width = 150;
            // 
            // SideNumberFact
            // 
            this.SideNumberFact.DataPropertyName = "SideNumberFact";
            this.SideNumberFact.HeaderText = "Фактический № ПС";
            this.SideNumberFact.Name = "SideNumberFact";
            this.SideNumberFact.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SideNumberFact.Width = 170;
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "VehicleType";
            // 
            // tbStatistics
            // 
            this.tbStatistics.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStatistics.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbStatistics.Location = new System.Drawing.Point(3, 3);
            this.tbStatistics.Multiline = true;
            this.tbStatistics.Name = "tbStatistics";
            this.tbStatistics.ReadOnly = true;
            this.tbStatistics.Size = new System.Drawing.Size(558, 602);
            this.tbStatistics.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 634);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Рабочее место диспетчера";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.pnErrors.ResumeLayout(false);
            this.pnErrors.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lbUpdateStatus;
        private System.Windows.Forms.Label lbCheckMailStatus;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn routeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn scheduleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SideNumberPlan;
        private System.Windows.Forms.DataGridViewTextBoxColumn SideNumberFact;
        private System.Windows.Forms.Panel pnErrors;
        private System.Windows.Forms.TextBox tbErrors;
        private System.Data.DataColumn dataColumn2;
        private System.Windows.Forms.TextBox tbStatistics;
    }
}

