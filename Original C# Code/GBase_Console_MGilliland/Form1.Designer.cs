namespace GBase_Console_MGilliland
{
    partial class GBaseConsole
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
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.lblDumb1 = new System.Windows.Forms.Label();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // txtCommand
            // 
            this.txtCommand.Location = new System.Drawing.Point(13, 13);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(259, 20);
            this.txtCommand.TabIndex = 0;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(104, 39);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 1;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // lblDumb1
            // 
            this.lblDumb1.AutoSize = true;
            this.lblDumb1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDumb1.Location = new System.Drawing.Point(12, 65);
            this.lblDumb1.Name = "lblDumb1";
            this.lblDumb1.Size = new System.Drawing.Size(69, 20);
            this.lblDumb1.TabIndex = 2;
            this.lblDumb1.Text = "Output:";
            // 
            // rtbOutput
            // 
            this.rtbOutput.Location = new System.Drawing.Point(16, 89);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(262, 190);
            this.rtbOutput.TabIndex = 3;
            this.rtbOutput.Text = "";
            // 
            // GBaseConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 291);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.lblDumb1);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtCommand);
            this.Name = "GBaseConsole";
            this.Text = "GBase Console";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label lblDumb1;
        private System.Windows.Forms.RichTextBox rtbOutput;
    }
}

