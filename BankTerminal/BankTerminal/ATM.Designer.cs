namespace BankTerminal
{
    partial class ATM
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
            this.tbPIN = new System.Windows.Forms.TextBox();
            this.tbMoney = new System.Windows.Forms.TextBox();
            this.tbDivision = new System.Windows.Forms.TextBox();
            this.lablePINcode = new System.Windows.Forms.Label();
            this.lableSumm = new System.Windows.Forms.Label();
            this.lableBanknotes = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbPIN
            // 
            this.tbPIN.Location = new System.Drawing.Point(60, 9);
            this.tbPIN.Name = "tbPIN";
            this.tbPIN.Size = new System.Drawing.Size(67, 20);
            this.tbPIN.TabIndex = 0;
            // 
            // tbMoney
            // 
            this.tbMoney.Location = new System.Drawing.Point(184, 9);
            this.tbMoney.Name = "tbMoney";
            this.tbMoney.Size = new System.Drawing.Size(70, 20);
            this.tbMoney.TabIndex = 1;
            // 
            // tbDivision
            // 
            this.tbDivision.Location = new System.Drawing.Point(68, 40);
            this.tbDivision.Name = "tbDivision";
            this.tbDivision.Size = new System.Drawing.Size(253, 20);
            this.tbDivision.TabIndex = 2;
            // 
            // lablePINcode
            // 
            this.lablePINcode.AutoSize = true;
            this.lablePINcode.Location = new System.Drawing.Point(7, 12);
            this.lablePINcode.Name = "lablePINcode";
            this.lablePINcode.Size = new System.Drawing.Size(49, 13);
            this.lablePINcode.TabIndex = 3;
            this.lablePINcode.Text = "PINcode";
            // 
            // lableSumm
            // 
            this.lableSumm.AutoSize = true;
            this.lableSumm.Location = new System.Drawing.Point(142, 12);
            this.lableSumm.Name = "lableSumm";
            this.lableSumm.Size = new System.Drawing.Size(36, 13);
            this.lableSumm.TabIndex = 4;
            this.lableSumm.Text = "Summ";
            // 
            // lableBanknotes
            // 
            this.lableBanknotes.AutoSize = true;
            this.lableBanknotes.Location = new System.Drawing.Point(5, 43);
            this.lableBanknotes.Name = "lableBanknotes";
            this.lableBanknotes.Size = new System.Drawing.Size(58, 13);
            this.lableBanknotes.TabIndex = 5;
            this.lableBanknotes.Text = "Banknotes";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(260, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(69, 26);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.GetBanknotes);
            // 
            // ATM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 72);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lableBanknotes);
            this.Controls.Add(this.lableSumm);
            this.Controls.Add(this.lablePINcode);
            this.Controls.Add(this.tbDivision);
            this.Controls.Add(this.tbMoney);
            this.Controls.Add(this.tbPIN);
            this.Name = "ATM";
            this.Text = "ATM";
            this.Load += new System.EventHandler(this.AtmLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPIN;
        private System.Windows.Forms.TextBox tbMoney;
        private System.Windows.Forms.TextBox tbDivision;
        private System.Windows.Forms.Label lablePINcode;
        private System.Windows.Forms.Label lableSumm;
        private System.Windows.Forms.Label lableBanknotes;
        private System.Windows.Forms.Button btnOK;
    }
}

