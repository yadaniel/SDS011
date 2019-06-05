namespace sds011_viewer
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.connectCOM = new System.Windows.Forms.Button();
			this.comboBoxSelectCOM = new System.Windows.Forms.ComboBox();
			this.groupBoxCOM = new System.Windows.Forms.GroupBox();
			this.groupBoxCOM.SuspendLayout();
			this.SuspendLayout();
			// 
			// connectCOM
			// 
			this.connectCOM.Location = new System.Drawing.Point(14, 63);
			this.connectCOM.Name = "connectCOM";
			this.connectCOM.Size = new System.Drawing.Size(121, 29);
			this.connectCOM.TabIndex = 2;
			this.connectCOM.Text = "connect";
			this.connectCOM.UseVisualStyleBackColor = true;
			this.connectCOM.Click += new System.EventHandler(this.ConnectCOMClick);
			// 
			// comboBoxSelectCOM
			// 
			this.comboBoxSelectCOM.FormattingEnabled = true;
			this.comboBoxSelectCOM.Location = new System.Drawing.Point(14, 33);
			this.comboBoxSelectCOM.Name = "comboBoxSelectCOM";
			this.comboBoxSelectCOM.Size = new System.Drawing.Size(121, 24);
			this.comboBoxSelectCOM.TabIndex = 3;
			// 
			// groupBoxCOM
			// 
			this.groupBoxCOM.Controls.Add(this.comboBoxSelectCOM);
			this.groupBoxCOM.Controls.Add(this.connectCOM);
			this.groupBoxCOM.Location = new System.Drawing.Point(637, 12);
			this.groupBoxCOM.Name = "groupBoxCOM";
			this.groupBoxCOM.Size = new System.Drawing.Size(148, 102);
			this.groupBoxCOM.TabIndex = 4;
			this.groupBoxCOM.TabStop = false;
			this.groupBoxCOM.Text = "select COM";
			this.groupBoxCOM.Enter += new System.EventHandler(this.GroupBoxCOMEnter);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(797, 323);
			this.Controls.Add(this.groupBoxCOM);
			this.Name = "MainForm";
			this.Text = "SDS011 Viewer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.DoubleClick += new System.EventHandler(this.MainFormDoubleClick);
			this.groupBoxCOM.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.GroupBox groupBoxCOM;
		private System.Windows.Forms.ComboBox comboBoxSelectCOM;
		private System.Windows.Forms.Button connectCOM;
		
	}
}
