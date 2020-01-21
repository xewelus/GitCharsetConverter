﻿using System.Windows.Forms;

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
			this.cbScroll = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
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
			this.tbText.MaxLength = 3276700;
			this.tbText.Name = "tbText";
			this.tbText.Size = new System.Drawing.Size(770, 528);
			this.tbText.TabIndex = 0;
			this.tbText.Text = "";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button4);
			this.panel1.Controls.Add(this.button3);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.cbScroll);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(770, 44);
			this.panel1.TabIndex = 1;
			// 
			// cbScroll
			// 
			this.cbScroll.AutoSize = true;
			this.cbScroll.Checked = true;
			this.cbScroll.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbScroll.Location = new System.Drawing.Point(684, 22);
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
			this.button1.Size = new System.Drawing.Size(76, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "git";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(142, 12);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(65, 23);
			this.button2.TabIndex = 2;
			this.button2.Text = "cmd";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(222, 12);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(65, 23);
			this.button3.TabIndex = 3;
			this.button3.Text = "set";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(302, 12);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(65, 23);
			this.button4.TabIndex = 4;
			this.button4.Text = "git 2";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
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
		private Button button2;
		private Button button3;
		private Button button4;
	}
}