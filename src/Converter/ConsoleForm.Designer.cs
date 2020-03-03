using System.Windows.Forms;

namespace Converter
{
	partial class ConsoleForm
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
			this.tbText = new System.Windows.Forms.RichTextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.cbWordWrap = new System.Windows.Forms.CheckBox();
			this.button5 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.cbScroll = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.btnGit3 = new System.Windows.Forms.Button();
			this.tbFrom = new System.Windows.Forms.TextBox();
			this.tbTo = new System.Windows.Forms.TextBox();
			this.btnCR = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbText
			// 
			this.tbText.BackColor = System.Drawing.Color.Black;
			this.tbText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbText.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tbText.ForeColor = System.Drawing.Color.White;
			this.tbText.Location = new System.Drawing.Point(0, 44);
			this.tbText.MaxLength = 32767000;
			this.tbText.Name = "tbText";
			this.tbText.Size = new System.Drawing.Size(770, 528);
			this.tbText.TabIndex = 0;
			this.tbText.Text = "";
			this.tbText.WordWrap = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnCR);
			this.panel1.Controls.Add(this.tbTo);
			this.panel1.Controls.Add(this.tbFrom);
			this.panel1.Controls.Add(this.btnGit3);
			this.panel1.Controls.Add(this.cbWordWrap);
			this.panel1.Controls.Add(this.button5);
			this.panel1.Controls.Add(this.button4);
			this.panel1.Controls.Add(this.cbScroll);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(770, 44);
			this.panel1.TabIndex = 1;
			// 
			// cbWordWrap
			// 
			this.cbWordWrap.AutoSize = true;
			this.cbWordWrap.Location = new System.Drawing.Point(663, 4);
			this.cbWordWrap.Name = "cbWordWrap";
			this.cbWordWrap.Size = new System.Drawing.Size(102, 17);
			this.cbWordWrap.TabIndex = 6;
			this.cbWordWrap.Text = "Перенос строк";
			this.cbWordWrap.UseVisualStyleBackColor = true;
			this.cbWordWrap.CheckedChanged += new System.EventHandler(this.cbWordWrap_CheckedChanged);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(383, 12);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(65, 23);
			this.button5.TabIndex = 5;
			this.button5.Text = "copy";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(82, 12);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(65, 23);
			this.button4.TabIndex = 4;
			this.button4.Text = "git 2";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// cbScroll
			// 
			this.cbScroll.AutoSize = true;
			this.cbScroll.Checked = true;
			this.cbScroll.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbScroll.Location = new System.Drawing.Point(663, 23);
			this.cbScroll.Name = "cbScroll";
			this.cbScroll.Size = new System.Drawing.Size(80, 17);
			this.cbScroll.TabIndex = 1;
			this.cbScroll.Text = "Скроллинг";
			this.cbScroll.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(12, 12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "git";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// btnGit3
			// 
			this.btnGit3.Location = new System.Drawing.Point(300, 12);
			this.btnGit3.Name = "btnGit3";
			this.btnGit3.Size = new System.Drawing.Size(65, 23);
			this.btnGit3.TabIndex = 7;
			this.btnGit3.Text = "git 3";
			this.btnGit3.UseVisualStyleBackColor = true;
			this.btnGit3.Click += new System.EventHandler(this.btnGit3_Click);
			// 
			// tbFrom
			// 
			this.tbFrom.Location = new System.Drawing.Point(174, 14);
			this.tbFrom.Name = "tbFrom";
			this.tbFrom.Size = new System.Drawing.Size(57, 20);
			this.tbFrom.TabIndex = 8;
			// 
			// tbTo
			// 
			this.tbTo.Location = new System.Drawing.Point(237, 14);
			this.tbTo.Name = "tbTo";
			this.tbTo.Size = new System.Drawing.Size(57, 20);
			this.tbTo.TabIndex = 9;
			// 
			// btnCR
			// 
			this.btnCR.Location = new System.Drawing.Point(544, 12);
			this.btnCR.Name = "btnCR";
			this.btnCR.Size = new System.Drawing.Size(65, 23);
			this.btnCR.TabIndex = 10;
			this.btnCR.Text = "C:\\r";
			this.btnCR.UseVisualStyleBackColor = true;
			this.btnCR.Click += new System.EventHandler(this.btnCR_Click);
			// 
			// ConsoleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(770, 572);
			this.Controls.Add(this.tbText);
			this.Controls.Add(this.panel1);
			this.Name = "ConsoleForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Консоль";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox tbText;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button1;
		private CheckBox cbScroll;
		private Button button4;
		private Button button5;
		private CheckBox cbWordWrap;
		private Button btnGit3;
		private TextBox tbFrom;
		private TextBox tbTo;
		private Button btnCR;
	}
}