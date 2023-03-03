
namespace USBcontrol_v1
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Btn_SendCmd = new System.Windows.Forms.Button();
            this.Btn_StrReceiveData = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Cmb_INEndpoint = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Cmb_OutEndPoint = new System.Windows.Forms.ComboBox();
            this.Cmb_DeviceConnect = new System.Windows.Forms.ComboBox();
            this.BytesInLabel = new System.Windows.Forms.Label();
            this.BytesOutLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TBox_ShowReceiveData = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn_SendCmd
            // 
            this.Btn_SendCmd.Location = new System.Drawing.Point(66, 223);
            this.Btn_SendCmd.Name = "Btn_SendCmd";
            this.Btn_SendCmd.Size = new System.Drawing.Size(104, 50);
            this.Btn_SendCmd.TabIndex = 0;
            this.Btn_SendCmd.Text = "Send Command";
            this.Btn_SendCmd.UseVisualStyleBackColor = true;
            this.Btn_SendCmd.Click += new System.EventHandler(this.Btn_SendCmd_Click);
            // 
            // Btn_StrReceiveData
            // 
            this.Btn_StrReceiveData.Location = new System.Drawing.Point(220, 223);
            this.Btn_StrReceiveData.Name = "Btn_StrReceiveData";
            this.Btn_StrReceiveData.Size = new System.Drawing.Size(104, 49);
            this.Btn_StrReceiveData.TabIndex = 1;
            this.Btn_StrReceiveData.Text = "Receive Data";
            this.Btn_StrReceiveData.UseVisualStyleBackColor = true;
            this.Btn_StrReceiveData.Click += new System.EventHandler(this.Btn_StrReceiveData_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Connected Devices";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Cmb_INEndpoint);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.Cmb_OutEndPoint);
            this.groupBox2.Location = new System.Drawing.Point(20, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(354, 89);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Endpoint Pair (Out / In) ";
            // 
            // Cmb_INEndpoint
            // 
            this.Cmb_INEndpoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_INEndpoint.FormattingEnabled = true;
            this.Cmb_INEndpoint.Location = new System.Drawing.Point(97, 25);
            this.Cmb_INEndpoint.Name = "Cmb_INEndpoint";
            this.Cmb_INEndpoint.Size = new System.Drawing.Size(247, 20);
            this.Cmb_INEndpoint.TabIndex = 6;
            this.Cmb_INEndpoint.SelectedIndexChanged += new System.EventHandler(this.Cmb_INEndpoint_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "IN Endpoints";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "Out Endpoints";
            // 
            // Cmb_OutEndPoint
            // 
            this.Cmb_OutEndPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_OutEndPoint.FormattingEnabled = true;
            this.Cmb_OutEndPoint.Location = new System.Drawing.Point(97, 59);
            this.Cmb_OutEndPoint.Name = "Cmb_OutEndPoint";
            this.Cmb_OutEndPoint.Size = new System.Drawing.Size(247, 20);
            this.Cmb_OutEndPoint.TabIndex = 2;
            this.Cmb_OutEndPoint.SelectedIndexChanged += new System.EventHandler(this.Cmb_OutEndPoint_SelectedIndexChanged);
            // 
            // Cmb_DeviceConnect
            // 
            this.Cmb_DeviceConnect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmb_DeviceConnect.FormattingEnabled = true;
            this.Cmb_DeviceConnect.Location = new System.Drawing.Point(121, 26);
            this.Cmb_DeviceConnect.Name = "Cmb_DeviceConnect";
            this.Cmb_DeviceConnect.Size = new System.Drawing.Size(247, 20);
            this.Cmb_DeviceConnect.TabIndex = 7;
            this.Cmb_DeviceConnect.SelectedIndexChanged += new System.EventHandler(this.Cmb_DeviceConnect_SelectedIndexChanged);
            // 
            // BytesInLabel
            // 
            this.BytesInLabel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BytesInLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BytesInLabel.Location = new System.Drawing.Point(244, 189);
            this.BytesInLabel.Name = "BytesInLabel";
            this.BytesInLabel.Size = new System.Drawing.Size(120, 18);
            this.BytesInLabel.TabIndex = 12;
            this.BytesInLabel.Text = "0";
            this.BytesInLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BytesOutLabel
            // 
            this.BytesOutLabel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BytesOutLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BytesOutLabel.Location = new System.Drawing.Point(244, 163);
            this.BytesOutLabel.Name = "BytesOutLabel";
            this.BytesOutLabel.Size = new System.Drawing.Size(120, 18);
            this.BytesOutLabel.TabIndex = 11;
            this.BytesOutLabel.Text = "0";
            this.BytesOutLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "Bytes transferred IN ..............";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "Bytes transferred OUT .........";
            // 
            // TBox_ShowReceiveData
            // 
            this.TBox_ShowReceiveData.Location = new System.Drawing.Point(23, 289);
            this.TBox_ShowReceiveData.Multiline = true;
            this.TBox_ShowReceiveData.Name = "TBox_ShowReceiveData";
            this.TBox_ShowReceiveData.Size = new System.Drawing.Size(351, 126);
            this.TBox_ShowReceiveData.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 486);
            this.Controls.Add(this.BytesInLabel);
            this.Controls.Add(this.TBox_ShowReceiveData);
            this.Controls.Add(this.Cmb_DeviceConnect);
            this.Controls.Add(this.BytesOutLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Btn_StrReceiveData);
            this.Controls.Add(this.Btn_SendCmd);
            this.Name = "Form1";
            this.Text = "USBcontorl_v1";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_SendCmd;
        private System.Windows.Forms.Button Btn_StrReceiveData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox Cmb_INEndpoint;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox Cmb_OutEndPoint;
        private System.Windows.Forms.ComboBox Cmb_DeviceConnect;
        private System.Windows.Forms.Label BytesInLabel;
        private System.Windows.Forms.Label BytesOutLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TBox_ShowReceiveData;
    }
}

