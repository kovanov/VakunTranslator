namespace VakunTranslatorVol2.Views
{
    partial class LexicalAnalyzerForm
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
            if(disposing && (components != null))
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
            this.lexemeGrid = new System.Windows.Forms.DataGridView();
            this.idGrid = new System.Windows.Forms.DataGridView();
            this.constGrid = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.lexemeGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.idGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.constGrid)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lexemeGrid
            // 
            this.lexemeGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lexemeGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lexemeGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel1.SetColumnSpan(this.lexemeGrid, 2);
            this.lexemeGrid.Location = new System.Drawing.Point(3, 3);
            this.lexemeGrid.Name = "lexemeGrid";
            this.lexemeGrid.Size = new System.Drawing.Size(594, 194);
            this.lexemeGrid.TabIndex = 4;
            this.lexemeGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.lexemeGrid_CellClick);
            // 
            // idGrid
            // 
            this.idGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.idGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.idGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.idGrid.Location = new System.Drawing.Point(3, 203);
            this.idGrid.Name = "idGrid";
            this.idGrid.Size = new System.Drawing.Size(294, 194);
            this.idGrid.TabIndex = 5;
            // 
            // constGrid
            // 
            this.constGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.constGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.constGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.constGrid.Location = new System.Drawing.Point(303, 203);
            this.constGrid.Name = "constGrid";
            this.constGrid.Size = new System.Drawing.Size(294, 194);
            this.constGrid.TabIndex = 6;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lexemeGrid, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.idGrid, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.constGrid, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 400);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // AnalyzerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "AnalyzerWindow";
            this.ShowIcon = false;
            this.Text = "AnalyzerWindow";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AnalyzerWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.lexemeGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.idGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.constGrid)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView lexemeGrid;
        private System.Windows.Forms.DataGridView idGrid;
        private System.Windows.Forms.DataGridView constGrid;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}