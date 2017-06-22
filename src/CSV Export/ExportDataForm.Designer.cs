namespace Crypton.Elasticsearch.CSVExport
{
    partial class ExportDataForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pgQueryProgress = new System.Windows.Forms.ProgressBar();
            this.btnExportDataSet = new System.Windows.Forms.Button();
            this.btnPreviewRows = new System.Windows.Forms.Button();
            this.txtLuceneQuery = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvPreview = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblOperation = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblExportProgress = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTimeTook = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTotalHits = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sfdExportCSV = new System.Windows.Forms.SaveFileDialog();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvPreview, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(8, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(992, 714);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.pgQueryProgress);
            this.groupBox1.Controls.Add(this.btnExportDataSet);
            this.groupBox1.Controls.Add(this.btnPreviewRows);
            this.groupBox1.Controls.Add(this.txtLuceneQuery);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(986, 119);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search";
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(509, 67);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pgQueryProgress
            // 
            this.pgQueryProgress.Location = new System.Drawing.Point(642, 67);
            this.pgQueryProgress.Name = "pgQueryProgress";
            this.pgQueryProgress.Size = new System.Drawing.Size(319, 30);
            this.pgQueryProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pgQueryProgress.TabIndex = 4;
            this.pgQueryProgress.Visible = false;
            // 
            // btnExportDataSet
            // 
            this.btnExportDataSet.Location = new System.Drawing.Point(238, 67);
            this.btnExportDataSet.Name = "btnExportDataSet";
            this.btnExportDataSet.Size = new System.Drawing.Size(127, 30);
            this.btnExportDataSet.TabIndex = 2;
            this.btnExportDataSet.Text = "Export";
            this.btnExportDataSet.UseVisualStyleBackColor = true;
            this.btnExportDataSet.Click += new System.EventHandler(this.btnExportDataSet_Click);
            // 
            // btnPreviewRows
            // 
            this.btnPreviewRows.Location = new System.Drawing.Point(105, 67);
            this.btnPreviewRows.Name = "btnPreviewRows";
            this.btnPreviewRows.Size = new System.Drawing.Size(127, 30);
            this.btnPreviewRows.TabIndex = 1;
            this.btnPreviewRows.Text = "Preview (500 rows)";
            this.btnPreviewRows.UseVisualStyleBackColor = true;
            this.btnPreviewRows.Click += new System.EventHandler(this.btnPreviewRows_Click);
            // 
            // txtLuceneQuery
            // 
            this.txtLuceneQuery.Location = new System.Drawing.Point(105, 29);
            this.txtLuceneQuery.Name = "txtLuceneQuery";
            this.txtLuceneQuery.Size = new System.Drawing.Size(856, 22);
            this.txtLuceneQuery.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Lucene Query:";
            // 
            // dgvPreview
            // 
            this.dgvPreview.AllowUserToAddRows = false;
            this.dgvPreview.AllowUserToDeleteRows = false;
            this.dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPreview.Location = new System.Drawing.Point(3, 203);
            this.dgvPreview.Name = "dgvPreview";
            this.dgvPreview.ReadOnly = true;
            this.dgvPreview.Size = new System.Drawing.Size(986, 508);
            this.dgvPreview.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblOperation);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.lblExportProgress);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.lblTimeTook);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lblTotalHits);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(986, 69);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Results";
            // 
            // lblOperation
            // 
            this.lblOperation.AutoSize = true;
            this.lblOperation.Location = new System.Drawing.Point(493, 34);
            this.lblOperation.Name = "lblOperation";
            this.lblOperation.Size = new System.Drawing.Size(26, 13);
            this.lblOperation.TabIndex = 7;
            this.lblOperation.Text = "N/A";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(408, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Operation:";
            // 
            // lblExportProgress
            // 
            this.lblExportProgress.AutoSize = true;
            this.lblExportProgress.Location = new System.Drawing.Point(728, 34);
            this.lblExportProgress.Name = "lblExportProgress";
            this.lblExportProgress.Size = new System.Drawing.Size(26, 13);
            this.lblExportProgress.TabIndex = 5;
            this.lblExportProgress.Text = "N/A";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(639, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Exported:";
            // 
            // lblTimeTook
            // 
            this.lblTimeTook.AutoSize = true;
            this.lblTimeTook.Location = new System.Drawing.Point(323, 34);
            this.lblTimeTook.Name = "lblTimeTook";
            this.lblTimeTook.Size = new System.Drawing.Size(13, 13);
            this.lblTimeTook.TabIndex = 3;
            this.lblTimeTook.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(235, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Time (ms):";
            // 
            // lblTotalHits
            // 
            this.lblTotalHits.AutoSize = true;
            this.lblTotalHits.Location = new System.Drawing.Point(102, 34);
            this.lblTotalHits.Name = "lblTotalHits";
            this.lblTotalHits.Size = new System.Drawing.Size(13, 13);
            this.lblTotalHits.TabIndex = 1;
            this.lblTotalHits.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Total hits:";
            // 
            // sfdExportCSV
            // 
            this.sfdExportCSV.Filter = "CSV Files|*.csv";
            this.sfdExportCSV.Title = "Export to CSV";
            // 
            // ExportDataForm
            // 
            this.AcceptButton = this.btnPreviewRows;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "ExportDataForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Text = "CSV Export";
            this.Shown += new System.EventHandler(this.ExportDataForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvPreview;
        private System.Windows.Forms.Button btnExportDataSet;
        private System.Windows.Forms.Button btnPreviewRows;
        private System.Windows.Forms.TextBox txtLuceneQuery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar pgQueryProgress;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalHits;
        private System.Windows.Forms.Label lblTimeTook;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog sfdExportCSV;
        private System.Windows.Forms.Label lblExportProgress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblOperation;
        private System.Windows.Forms.Label label5;
    }
}

