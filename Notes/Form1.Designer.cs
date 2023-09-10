namespace Notes
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.создатьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.заметкуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.заметкуСДатойToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьПросроченныеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(884, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.создатьToolStripMenuItem,
            this.удалитьПросроченныеToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // создатьToolStripMenuItem
            // 
            this.создатьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.заметкуToolStripMenuItem,
            this.заметкуСДатойToolStripMenuItem});
            this.создатьToolStripMenuItem.Name = "создатьToolStripMenuItem";
            this.создатьToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.создатьToolStripMenuItem.Text = "Создать";
            // 
            // заметкуToolStripMenuItem
            // 
            this.заметкуToolStripMenuItem.Name = "заметкуToolStripMenuItem";
            this.заметкуToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.заметкуToolStripMenuItem.Text = "Заметку";
            this.заметкуToolStripMenuItem.Click += new System.EventHandler(this.заметкуToolStripMenuItem_Click);
            // 
            // заметкуСДатойToolStripMenuItem
            // 
            this.заметкуСДатойToolStripMenuItem.Name = "заметкуСДатойToolStripMenuItem";
            this.заметкуСДатойToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.заметкуСДатойToolStripMenuItem.Text = "Заметку с датой";
            // 
            // удалитьПросроченныеToolStripMenuItem
            // 
            this.удалитьПросроченныеToolStripMenuItem.Name = "удалитьПросроченныеToolStripMenuItem";
            this.удалитьПросроченныеToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.удалитьПросроченныеToolStripMenuItem.Text = "Удалить просроченные";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(884, 661);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximumSize = new System.Drawing.Size(900, 700);
            this.MinimumSize = new System.Drawing.Size(900, 700);
            this.Name = "Form1";
            this.Text = "Главная";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem создатьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem заметкуToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem заметкуСДатойToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem удалитьПросроченныеToolStripMenuItem;
    }
}

