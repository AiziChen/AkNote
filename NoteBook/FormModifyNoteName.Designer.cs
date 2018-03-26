namespace AkNote
{
    partial class FormModifyNoteName
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxModifyNote = new System.Windows.Forms.TextBox();
            this.buttonModifyNote = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "请输入新笔记名：";
            // 
            // textBoxModifyNote
            // 
            this.textBoxModifyNote.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxModifyNote.Location = new System.Drawing.Point(128, 12);
            this.textBoxModifyNote.Name = "textBoxModifyNote";
            this.textBoxModifyNote.Size = new System.Drawing.Size(247, 21);
            this.textBoxModifyNote.TabIndex = 1;
            this.textBoxModifyNote.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxModifyNote_KeyDown);
            // 
            // buttonModifyNote
            // 
            this.buttonModifyNote.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonModifyNote.Location = new System.Drawing.Point(300, 38);
            this.buttonModifyNote.Name = "buttonModifyNote";
            this.buttonModifyNote.Size = new System.Drawing.Size(75, 23);
            this.buttonModifyNote.TabIndex = 3;
            this.buttonModifyNote.Text = "确定";
            this.buttonModifyNote.UseVisualStyleBackColor = true;
            this.buttonModifyNote.Click += new System.EventHandler(this.buttonModifyNote_Click);
            // 
            // FormModifyNoteName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 84);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxModifyNote);
            this.Controls.Add(this.buttonModifyNote);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormModifyNoteName";
            this.Text = "修改";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textBoxModifyNote;
        private System.Windows.Forms.Button buttonModifyNote;
    }
}