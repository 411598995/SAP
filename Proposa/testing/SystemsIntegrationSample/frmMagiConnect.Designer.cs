namespace SystemsIntegrationSample
{
    partial class frmMagiConnectSample
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
            this.btnExportOrders = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.btnUpdateOrderStatus = new System.Windows.Forms.Button();
            this.btnExportOrdersUsingDateRange = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExportOrders
            // 
            this.btnExportOrders.Location = new System.Drawing.Point(31, 12);
            this.btnExportOrders.Name = "btnExportOrders";
            this.btnExportOrders.Size = new System.Drawing.Size(156, 23);
            this.btnExportOrders.TabIndex = 0;
            this.btnExportOrders.Text = "Export Orders";
            this.btnExportOrders.UseVisualStyleBackColor = true;
            this.btnExportOrders.Click += new System.EventHandler(this.btnExportOrders_Click);
            // 
            // txtResults
            // 
            this.txtResults.Location = new System.Drawing.Point(525, 13);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.Size = new System.Drawing.Size(536, 386);
            this.txtResults.TabIndex = 1;
            // 
            // btnUpdateOrderStatus
            // 
            this.btnUpdateOrderStatus.Location = new System.Drawing.Point(31, 99);
            this.btnUpdateOrderStatus.Name = "btnUpdateOrderStatus";
            this.btnUpdateOrderStatus.Size = new System.Drawing.Size(156, 23);
            this.btnUpdateOrderStatus.TabIndex = 2;
            this.btnUpdateOrderStatus.Text = "Update Order Status";
            this.btnUpdateOrderStatus.UseVisualStyleBackColor = true;
            this.btnUpdateOrderStatus.Click += new System.EventHandler(this.btnUpdateOrderStatus_Click);
            // 
            // btnExportOrdersUsingDateRange
            // 
            this.btnExportOrdersUsingDateRange.Location = new System.Drawing.Point(31, 41);
            this.btnExportOrdersUsingDateRange.Name = "btnExportOrdersUsingDateRange";
            this.btnExportOrdersUsingDateRange.Size = new System.Drawing.Size(156, 52);
            this.btnExportOrdersUsingDateRange.TabIndex = 3;
            this.btnExportOrdersUsingDateRange.Text = "Export Orders Using Date Range";
            this.btnExportOrdersUsingDateRange.UseVisualStyleBackColor = true;
            this.btnExportOrdersUsingDateRange.Click += new System.EventHandler(this.btnExportOrdersUsingDateRange_Click);
            // 
            // frmMagiConnectSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1096, 426);
            this.Controls.Add(this.btnExportOrdersUsingDateRange);
            this.Controls.Add(this.btnUpdateOrderStatus);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.btnExportOrders);
            this.Name = "frmMagiConnectSample";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MagiConnect Sample Code";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExportOrders;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Button btnUpdateOrderStatus;
        private System.Windows.Forms.Button btnExportOrdersUsingDateRange;
    }
}

