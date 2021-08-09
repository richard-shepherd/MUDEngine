
namespace UI_WinForms
{
    partial class Form_UI
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
            this.ctrlGroupBox_Output = new System.Windows.Forms.GroupBox();
            this.ctrlOutput = new System.Windows.Forms.TextBox();
            this.ctrlGroupBox_Input = new System.Windows.Forms.GroupBox();
            this.ctrlInput = new System.Windows.Forms.TextBox();
            this.ctrlGroupBox_Output.SuspendLayout();
            this.ctrlGroupBox_Input.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctrlGroupBox_Output
            // 
            this.ctrlGroupBox_Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlGroupBox_Output.Controls.Add(this.ctrlOutput);
            this.ctrlGroupBox_Output.Location = new System.Drawing.Point(12, 12);
            this.ctrlGroupBox_Output.Name = "ctrlGroupBox_Output";
            this.ctrlGroupBox_Output.Size = new System.Drawing.Size(2432, 985);
            this.ctrlGroupBox_Output.TabIndex = 1;
            this.ctrlGroupBox_Output.TabStop = false;
            this.ctrlGroupBox_Output.Text = "Output";
            // 
            // ctrlOutput
            // 
            this.ctrlOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlOutput.BackColor = System.Drawing.Color.Black;
            this.ctrlOutput.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ctrlOutput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ctrlOutput.Location = new System.Drawing.Point(6, 46);
            this.ctrlOutput.Multiline = true;
            this.ctrlOutput.Name = "ctrlOutput";
            this.ctrlOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ctrlOutput.Size = new System.Drawing.Size(2420, 933);
            this.ctrlOutput.TabIndex = 1;
            // 
            // ctrlGroupBox_Input
            // 
            this.ctrlGroupBox_Input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlGroupBox_Input.Controls.Add(this.ctrlInput);
            this.ctrlGroupBox_Input.Location = new System.Drawing.Point(12, 1003);
            this.ctrlGroupBox_Input.Name = "ctrlGroupBox_Input";
            this.ctrlGroupBox_Input.Size = new System.Drawing.Size(2432, 118);
            this.ctrlGroupBox_Input.TabIndex = 1;
            this.ctrlGroupBox_Input.TabStop = false;
            this.ctrlGroupBox_Input.Text = "Input";
            // 
            // ctrlInput
            // 
            this.ctrlInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlInput.BackColor = System.Drawing.Color.Black;
            this.ctrlInput.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ctrlInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ctrlInput.Location = new System.Drawing.Point(6, 46);
            this.ctrlInput.Name = "ctrlInput";
            this.ctrlInput.Size = new System.Drawing.Size(2420, 53);
            this.ctrlInput.TabIndex = 0;
            this.ctrlInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ctrlInput_KeyPress);
            // 
            // Form_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(17F, 41F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2456, 1133);
            this.Controls.Add(this.ctrlGroupBox_Input);
            this.Controls.Add(this.ctrlGroupBox_Output);
            this.Name = "Form_UI";
            this.Text = "MUDEngine UI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_UI_FormClosing);
            this.Load += new System.EventHandler(this.Form_UI_Load);
            this.ctrlGroupBox_Output.ResumeLayout(false);
            this.ctrlGroupBox_Output.PerformLayout();
            this.ctrlGroupBox_Input.ResumeLayout(false);
            this.ctrlGroupBox_Input.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ctrlGroupBox_Output;
        private System.Windows.Forms.GroupBox ctrlGroupBox_Input;
        private System.Windows.Forms.TextBox ctrlInput;
        private System.Windows.Forms.TextBox ctrlOutput;
    }
}